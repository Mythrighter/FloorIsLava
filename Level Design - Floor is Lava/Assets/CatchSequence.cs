using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Attach to the Player.
/// Called by Babysitter when the player enters the catch radius.
///
/// Sequence:
///   1. Lock player input
///   2. Player walks toward the babysitter until close
///   3. Player camera rotates to look at babysitter
///   4. Fade to black
///   5. End screen appears
///
/// Safety net: if the end screen hasn't triggered within escapeTimeout seconds,
/// the player is released and the babysitter resumes patrolling.
/// </summary>
public class CatchSequence : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    public CharacterController characterController;
    public Camera playerCam;
    public CanvasGroup fadeOverlay;
    public GameObject endScreen;

    [Header("Timing & Movement")]
    [Tooltip("How close the player needs to walk before the look + fade begins.")]
    public float walkUntilDistance = 1.5f;
    [Tooltip("Speed the player walks toward the babysitter.")]
    public float walkTowardSpeed = 3f;
    [Tooltip("How long the player looks at the babysitter before fading.")]
    public float lookAtDuration = 2f;
    [Tooltip("How long the fade to black takes.")]
    public float fadeDuration = 1.5f;
    [Tooltip("Seconds before the safety net releases the player if end screen never triggers.")]
    public float escapeTimeout = 8f;

    private bool sequenceStarted = false;
    private bool sequenceCompleted = false;

    // Stored so ReleasePlayer can tell the babysitter to resume
    private Babysitter babysitterScript;

    /// <summary>Called by Babysitter when player enters catch radius.</summary>
    public void TriggerCatch(Transform babysitter, NavMeshAgent babysitterAgent)
    {
        if (sequenceStarted) return;
        sequenceStarted = true;

        // Cache the Babysitter script so we can resume it on escape
        babysitterScript = babysitter.GetComponent<Babysitter>();

        // Fully stop the babysitter
        babysitterAgent.isStopped = true;
        babysitterAgent.ResetPath();
        babysitterAgent.velocity = Vector3.zero;

        StartCoroutine(RunCatchSequence(babysitter));
        StartCoroutine(EscapeTimeout(babysitterAgent));
    }

    private IEnumerator RunCatchSequence(Transform babysitter)
    {
        // 1 ── Lock player input
        playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2 ── Walk player toward babysitter until close enough
        while (Vector3.Distance(transform.position, babysitter.position) > walkUntilDistance)
        {
            if (!sequenceStarted) yield break;

            Vector3 dir = (babysitter.position - transform.position).normalized;
            dir.y = 0f;

            Vector3 motion = dir * walkTowardSpeed * Time.deltaTime;
            motion.y = -9.8f * Time.deltaTime;
            characterController.Move(motion);

            Vector3 lookDir = (babysitter.position - playerCam.transform.position).normalized;
            playerCam.transform.rotation = Quaternion.Slerp(
                playerCam.transform.rotation,
                Quaternion.LookRotation(lookDir),
                Time.deltaTime * 3f
            );

            yield return null;
        }

        // 3 ── Smoothly look at babysitter
        float elapsed = 0f;
        Quaternion startRot = playerCam.transform.rotation;

        while (elapsed < lookAtDuration)
        {
            if (!sequenceStarted) yield break;

            elapsed += Time.deltaTime;
            Vector3 dir = (babysitter.position - playerCam.transform.position).normalized;
            playerCam.transform.rotation = Quaternion.Slerp(
                startRot,
                Quaternion.LookRotation(dir),
                elapsed / lookAtDuration
            );
            yield return null;
        }

        // 4 ── Fade to black
        elapsed = 0f;
        fadeOverlay.gameObject.SetActive(true);

        while (elapsed < fadeDuration)
        {
            if (!sequenceStarted) yield break;

            elapsed += Time.deltaTime;
            fadeOverlay.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        fadeOverlay.alpha = 1f;

        // 5 ── Show end screen
        sequenceCompleted = true;
        endScreen.SetActive(true);
    }

    private IEnumerator EscapeTimeout(NavMeshAgent babysitterAgent)
    {
        float elapsed = 0f;

        while (elapsed < escapeTimeout)
        {
            if (sequenceCompleted) yield break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.LogWarning("[CatchSequence] Timed out — releasing player and resuming babysitter.");
        ReleasePlayer();
        ResumeBabysitter(babysitterAgent);
    }

    private void ReleasePlayer()
    {
        sequenceStarted = false;

        playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
            fadeOverlay.gameObject.SetActive(false);
        }
    }

    private void ResumeBabysitter(NavMeshAgent babysitterAgent)
    {
        if (babysitterAgent != null)
        {
            babysitterAgent.isStopped = false;
        }

        if (babysitterScript != null)
        {
            babysitterScript.ResumeAfterFailedCatch();
        }
    }
}