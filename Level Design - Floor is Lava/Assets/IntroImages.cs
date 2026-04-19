using UnityEngine;
using System.Collections;

public class IntroSequence : MonoBehaviour
{
    [SerializeField] GameObject[] canvases;
    [SerializeField] float displayTime = 3f;

    void Start()
    {
        StartCoroutine(ShowCanvases());
    }

    IEnumerator ShowCanvases()
    {
        foreach (GameObject canvas in canvases)
        {
            canvas.SetActive(true);
            yield return new WaitForSeconds(displayTime);
            canvas.SetActive(false);
        }
    }
}
