using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DiaryPages : MonoBehaviour
{
    public enum TransitionType { None, Fade, Slide, SlideAndFade }

    [Header("Panel")]
    public GameObject panel;

    [Header("Pages")]
    [Tooltip("Container the pages are parented under. Usually a child of the panel that the arrows sit outside of.")]
    public RectTransform pagesContainer;
    [Tooltip("Pages present from the start. Leave empty if every page is collected during play.")]
    public List<GameObject> startingPages = new List<GameObject>();
    public bool wrap = true;

    [Header("Navigation Buttons")]
    public Button previousButton;
    public Button nextButton;

    [Header("Transition")]
    public TransitionType transition = TransitionType.SlideAndFade;
    [Tooltip("How long a page transition takes, in seconds.")]
    public float transitionDuration = 0.25f;

    private readonly List<GameObject> pages = new List<GameObject>();
    private readonly Dictionary<GameObject, Vector2> homePositions = new Dictionary<GameObject, Vector2>();
    private int currentPage = 0;

    private Coroutine activeTransition;
    private GameObject transitionIncoming;
    private GameObject transitionOutgoing;
    private bool panelWasActive;

    void Start()
    {
        if (previousButton != null) previousButton.onClick.AddListener(PreviousPage);
        if (nextButton != null) nextButton.onClick.AddListener(NextPage);

        foreach (var p in startingPages)
            RegisterPage(p);

        for (int i = 0; i < pages.Count; i++)
            SetPageHiddenInstant(pages[i]);

        if (pages.Count > 0)
        {
            currentPage = pages.Count - 1; // newest
            SetPageShownInstant(pages[currentPage]);
        }

        panelWasActive = panel != null && panel.activeSelf;

        UpdateButtonStates();

        SetCursorState(panelWasActive);
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
                panel.SetActive(!panel.activeSelf);
            

            //if (Keyboard.current.escapeKey.wasPressedThisFrame)
            //    panel.SetActive(false);
        }

        // Detect open/close (from any source) and react.
        bool panelActive = panel != null && panel.activeSelf;
        if (panelActive && !panelWasActive)
        {
            ShowNewestInstant();
            SetCursorState(true);   // panel opened → free the cursor
        }
        else if (!panelActive && panelWasActive)
        {
            SetCursorState(false);  // panel closed → relock the cursor
        }
        panelWasActive = panelActive;

    }

    private void SetCursorState(bool unlocked)
    {
        if (unlocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // --- Called by pickups when the player collects a page ---
    public void AddPage(GameObject page, bool openAndShow = false)
    {
        int newIndex = RegisterPage(page);
        if (newIndex < 0) return; // null or already added

        bool panelWasOpen = panel != null && panel.activeSelf;

        if (openAndShow && panel != null)
            panel.SetActive(true);

        if (pages.Count == 1)
        {
            // First page ever — just show it.
            currentPage = 0;
            SetPageShownInstant(pages[0]);
        }
        else if (openAndShow && panelWasOpen)
        {
            // Panel was already open: animate across to the freshly collected (newest) page.
            GoToPage(newIndex, +1);
        }
        // else: panel was closed; the open detector in Update shows the newest page when it opens.

        UpdateButtonStates();
    }

    public void NextPage()
    {
        if (pages.Count == 0) return;
        int next = currentPage + 1;
        if (next >= pages.Count) next = wrap ? 0 : pages.Count - 1;
        GoToPage(next, +1);
    }

    public void PreviousPage()
    {
        if (pages.Count == 0) return;
        int prev = currentPage - 1;
        if (prev < 0) prev = wrap ? pages.Count - 1 : 0;
        GoToPage(prev, -1);
    }

    private int RegisterPage(GameObject page)
    {
        if (page == null || pages.Contains(page))
            return -1;

        if (pagesContainer != null)
            page.transform.SetParent(pagesContainer, false);

        GetOrAddCanvasGroup(page);

        RectTransform rt = page.GetComponent<RectTransform>();
        if (rt != null)
            homePositions[page] = rt.anchoredPosition;

        page.SetActive(false);
        pages.Add(page);
        return pages.Count - 1;
    }

    private void GoToPage(int index, int direction)
    {
        if (index < 0 || index >= pages.Count || index == currentPage)
            return;

        FinalizeActiveTransition();

        GameObject outgoing = pages[currentPage];
        GameObject incoming = pages[index];
        currentPage = index;

        transitionOutgoing = outgoing;
        transitionIncoming = incoming;
        activeTransition = StartCoroutine(TransitionRoutine(outgoing, incoming, direction));

        UpdateButtonStates();
    }

    private IEnumerator TransitionRoutine(GameObject outgoing, GameObject incoming, int direction)
    {
        bool doFade = transition == TransitionType.Fade || transition == TransitionType.SlideAndFade;
        bool doSlide = transition == TransitionType.Slide || transition == TransitionType.SlideAndFade;

        RectTransform inRt = incoming.GetComponent<RectTransform>();
        RectTransform outRt = outgoing != null ? outgoing.GetComponent<RectTransform>() : null;
        CanvasGroup inCg = GetOrAddCanvasGroup(incoming);
        CanvasGroup outCg = outgoing != null ? GetOrAddCanvasGroup(outgoing) : null;

        Vector2 inHome = GetHome(incoming);
        Vector2 outHome = outgoing != null ? GetHome(outgoing) : Vector2.zero;

        float width = pagesContainer != null ? pagesContainer.rect.width : 0f;
        if (width <= 0f && inRt != null) width = inRt.rect.width;
        if (width <= 0f) width = Screen.width;

        incoming.SetActive(true);

        if (doSlide && inRt != null)
            inRt.anchoredPosition = inHome + new Vector2(direction * width, 0f);
        if (doFade) inCg.alpha = 0f;

        float dur = transition == TransitionType.None ? 0f : transitionDuration;
        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime; // animates even when the game is paused
            float k = Mathf.Clamp01(t / dur);
            float s = k * k * (3f - 2f * k); // smoothstep easing

            if (doSlide)
            {
                if (inRt != null)
                    inRt.anchoredPosition = Vector2.Lerp(inHome + new Vector2(direction * width, 0f), inHome, s);
                if (outRt != null)
                    outRt.anchoredPosition = Vector2.Lerp(outHome, outHome + new Vector2(-direction * width, 0f), s);
            }
            if (doFade)
            {
                inCg.alpha = s;
                if (outCg != null) outCg.alpha = 1f - s;
            }
            yield return null;
        }

        // Finalize (also covers the None / zero-duration case)
        if (inRt != null) inRt.anchoredPosition = inHome;
        inCg.alpha = 1f;

        if (outgoing != null)
        {
            if (outRt != null) outRt.anchoredPosition = outHome;
            if (outCg != null) outCg.alpha = 1f;
            outgoing.SetActive(false);
        }

        transitionIncoming = null;
        transitionOutgoing = null;
        activeTransition = null;
    }

    private void FinalizeActiveTransition()
    {
        if (activeTransition != null)
            StopCoroutine(activeTransition);
        activeTransition = null;

        if (transitionIncoming != null)
        {
            SetPageShownInstant(transitionIncoming);
            transitionIncoming = null;
        }
        if (transitionOutgoing != null)
        {
            SetPageHiddenInstant(transitionOutgoing);
            transitionOutgoing = null;
        }
    }

    private void SetPageShownInstant(GameObject page)
    {
        page.SetActive(true);
        GetOrAddCanvasGroup(page).alpha = 1f;
        RectTransform rt = page.GetComponent<RectTransform>();
        if (rt != null) rt.anchoredPosition = GetHome(page);
    }

    private void SetPageHiddenInstant(GameObject page)
    {
        GetOrAddCanvasGroup(page).alpha = 1f;
        RectTransform rt = page.GetComponent<RectTransform>();
        if (rt != null) rt.anchoredPosition = GetHome(page);
        page.SetActive(false);
    }

    private void ShowNewestInstant()
    {
        if (pages.Count == 0) return;

        FinalizeActiveTransition();

        currentPage = pages.Count - 1; // newest is always the last one added
        for (int i = 0; i < pages.Count; i++)
        {
            if (i == currentPage)
                SetPageShownInstant(pages[i]);
            else
                SetPageHiddenInstant(pages[i]);
        }

        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        bool multiple = pages.Count > 1;

        if (wrap)
        {
            if (previousButton != null) previousButton.interactable = multiple;
            if (nextButton != null) nextButton.interactable = multiple;
        }
        else
        {
            if (previousButton != null) previousButton.interactable = multiple && currentPage > 0;
            if (nextButton != null) nextButton.interactable = multiple && currentPage < pages.Count - 1;
        }
    }

    private CanvasGroup GetOrAddCanvasGroup(GameObject page)
    {
        CanvasGroup cg = page.GetComponent<CanvasGroup>();
        if (cg == null) cg = page.AddComponent<CanvasGroup>();
        return cg;
    }

    private Vector2 GetHome(GameObject page)
    {
        if (homePositions.TryGetValue(page, out Vector2 home))
            return home;
        RectTransform rt = page.GetComponent<RectTransform>();
        return rt != null ? rt.anchoredPosition : Vector2.zero;
    }
}
