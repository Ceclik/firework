using System.Collections;
using UnityEngine;

public class FadeInFadeOut : MonoBehaviour
{
    [SerializeField] private float fadeInTime;
    [SerializeField] private float enabledTime;
    [SerializeField] private float disabledTime;

    private CanvasGroup _group;

    private void OnEnable()
    {
        _group = GetComponent<CanvasGroup>();
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
            _group.alpha = timer / fadeInTime;
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
            _group.alpha = 1 - timer / fadeInTime;
            timer = timer + Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(disabledTime);
        StartCoroutine(FadeIn());
    }
}