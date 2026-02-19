using UnityEngine;
using System.Collections;

public class HitMarker : MonoBehaviour
{
    Timer life_timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        life_timer = GetComponent<Timer>();
    }

    IEnumerator Shrink()
    {
        //Shrinks the hitmarker down over the course of a second.
        //Then destroys once fully shrunken down.
        float end_time = Time.time + 1;
        while (Time.time < end_time)
        {
            transform.localScale *= (end_time - Time.time);
            yield return null; //Wait untill the next frame.
        }
        Destroy(gameObject);
    }

    public void StartShrink()
    {
        StartCoroutine(Shrink());
    }
}
