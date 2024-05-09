using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInFadeOut : MonoBehaviour {

    CanvasGroup group;    

    public float fadeInTime;
    public float enabledTime;
    public float disabledTime;

    private void OnEnable()
    {
        group = GetComponent<CanvasGroup>();
        StartCoroutine(FadeIn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }    

    private IEnumerator FadeIn()
    {
        float timer = 0;
        while (timer < fadeInTime)
        {
            group.alpha = timer / fadeInTime;
            timer = timer + Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(enabledTime);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float timer = 0;
        while (timer < fadeInTime)
        {
            group.alpha = 1 - (timer / fadeInTime);
            timer = timer + Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(disabledTime);
        StartCoroutine(FadeIn());
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
