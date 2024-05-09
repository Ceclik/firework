using Assets.Scripts.Tracking;
using DigitalRuby.ThunderAndLightning;
using System.Collections;
using System.Collections.Generic;
using Tracking;
using UnityEngine;
using UnityEngine.UI;

//This script handled extinguisher. Activate and deactivate (when not interacting with fire) extinguisher particles
//effect and play sound effect. Also activates lightning.

public class FireStopper : MonoBehaviour
{

    public AudioSource[] soundStarts;
    public AudioSource[] soundLoops;
    public AudioSource[] soundEnds;
    public float fadeTime = 0.07f;    
    public float lightFade = 0.5f;
    public float lightFadeTime = 0.5f;
    public GameObject lightning;
    public GameObject danger;

    private ParticleSystem pSystem;
    private bool soundPlaying;
    private float fadingColor;
    private float startLight;
    private float fadingTime;

    private MyInput _input;

    public void Orient(Vector3 point)
    {
        pSystem.transform.LookAt(point);        
    }

    public void StartFireStopping(int fireIndex,bool fake, bool startOver)
    {     
        if (!pSystem.isEmitting)
        {
            fadingColor = lightFade;
            fadingTime = 0;
            pSystem.Play();            
        }
        if (!soundPlaying) StartCoroutine(StartAudio());
        if (lightning!=null && !lightning.activeSelf && fireIndex==0 && (fake))
        {
            lightning.SetActive(true);
            if (danger!=null)
            {
                danger.SetActive(true);
            }
        }        
        if (startOver)
        {
            if (danger != null)
            {
                danger.SetActive(true);
            }
        }
    }

    public void Hide()
    {        
        pSystem.Stop();
        if (lightning!=null)
        {
            lightning.SetActive(false);            
        }
        if (danger != null)
        {
            danger.SetActive(false);
        }
    }

    public void StopFireStopping()
    {
        if (pSystem.isEmitting)
        {
            fadingColor = startLight;
            fadingTime = 0;
            pSystem.Stop();
        }
        if (lightning != null && lightning.activeSelf)
        {
            lightning.SetActive(false);
        }
        if (danger != null && danger.activeSelf)
        {
            danger.SetActive(false);
        }
    }

    private IEnumerator StartAudio()
    {
        if (soundPlaying) yield break;

        soundPlaying = true;

        Debug.Log("startsound");
        var startSound = soundStarts[Random.Range(0, soundStarts.Length)];
        StartCoroutine(AudioFade.FadeIn(startSound, fadeTime));
        yield return new WaitForSeconds(startSound.clip.length - fadeTime);

        var previousSound = startSound;

        int n = Random.Range(0, soundLoops.Length);

        while (pSystem.isEmitting)
        {
            Debug.Log("loopsound"+pSystem.isPlaying+":"+pSystem.isEmitting);
            var loopSound = soundLoops[n];
            StartCoroutine(AudioFade.FadeOut(previousSound, fadeTime));
            StartCoroutine(AudioFade.FadeIn(loopSound, fadeTime));
            previousSound = loopSound;            
            yield return new WaitForSeconds(loopSound.clip.length - fadeTime);
            int newN = n;
            while (n==newN)
            { n = Random.Range(0, soundLoops.Length); }            
        }

        Debug.Log("endsound");
        var endSound = soundEnds[Random.Range(0, soundEnds.Length)];
        StartCoroutine(AudioFade.FadeOut(previousSound, fadeTime));
        StartCoroutine(AudioFade.FadeIn(endSound, fadeTime));
        yield return new WaitForSeconds(endSound.clip.length - fadeTime);
        StartCoroutine(AudioFade.FadeOut(endSound, fadeTime));

        soundPlaying = false;
    }


    private void Awake()
    {
        pSystem = GetComponent<ParticleSystem>();        
        fadingTime = 0;
        fadingColor = startLight;
        _input = MyInput.Instance;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 mouseScreenPosition = Input.mousePosition;
        //mouseScreenPosition.z = 4.0f;
        //Vector3 mouseWorldSpace = Camera.main.ScreenToWorldPoint(mouseScreenPosition);     
    }
}


public static class AudioFade
{
    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = 1.0f;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        float startVolume = 0.2f;

        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < 1.0f)
        {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.volume = 1f;
    }
}
