using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is using for fade in level name in it beginning

public class FadeByTime : MonoBehaviour
{
    public float startTime;
    public float fadeInTime;

    private CanvasGroup _fadeGroup;


    private IEnumerator FadeIn()
    {
        float timer = 0;
        while (timer < fadeInTime)
        {
            _fadeGroup.alpha = timer / fadeInTime;
            timer = timer + Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator WaitForStart()
    {
        yield return new WaitForSeconds(startTime);
        StartCoroutine(FadeIn());
    }
    private void Awake()
    {
        _fadeGroup = GetComponent<CanvasGroup>();
    }
    // Use this for initialization
    void Start()
    {        
        _fadeGroup.alpha = 0;
        StartCoroutine(WaitForStart());
    }

    // Update is called once per frame
    void Update()
    {

    }


}
