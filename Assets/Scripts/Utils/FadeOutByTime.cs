using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is using for fade out level name in it beginning

public class FadeOutByTime : MonoBehaviour
{

    public float startTime;
    public float fadeOutTime;

    private CanvasGroup _fadeGroup;


    private IEnumerator FadeOut()
    {
        float timer = 0;
        while (timer < fadeOutTime)
        {
            _fadeGroup.alpha = 1 - (timer / fadeOutTime);
            timer = timer + Time.deltaTime;
            yield return null;
        }
        _fadeGroup.gameObject.SetActive(false);
    }

    private IEnumerator WaitForStart()
    {
        yield return new WaitForSeconds(startTime);
        StartCoroutine(FadeOut());
    }
    private void Awake()
    {
        _fadeGroup = GetComponent<CanvasGroup>();
    }
    // Use this for initialization
    void Start()
    {
        //_fadeGroup.alpha = 0;
        StartCoroutine(WaitForStart());
    }

    // Update is called once per frame
    void Update()
    {

    }


}
