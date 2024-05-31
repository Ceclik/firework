using System.Collections;
using Tracking;
using UnityEngine;

//This script handled extinguisher. Activate and deactivate (when not interacting with fire) extinguisher particles
//effect and play sound effect. Also activates lightning.

public class FireStopper : MonoBehaviour
{

    [SerializeField] private AudioSource[] soundStarts;
    [SerializeField] private AudioSource[] soundLoops;
    [SerializeField] private AudioSource[] soundEnds;
    [SerializeField] private float fadeTime = 0.07f;    
    [SerializeField] private float lightFade = 0.5f;
    [SerializeField] private float lightFadeTime = 0.5f;
    [SerializeField] private GameObject lightning;
    [SerializeField] private GameObject danger;

    private ParticleSystem _pSystem;
    public bool SoundPlaying { get; private set; }
    private float _fadingColor;
    private float _startLight;
    private float _fadingTime;

    private MyInput _input;

    public void Orient(Vector3 point)
    {
        _pSystem.transform.LookAt(point);        
    }

    public void StartFireStopping(int fireIndex,bool fake, bool startOver)
    {     
        if (!_pSystem.isEmitting)
        {
            _fadingColor = lightFade;
            _fadingTime = 0;
            _pSystem.Play();            
        }
        if (!SoundPlaying) StartCoroutine(StartAudio());
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
        _pSystem.Stop();
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
        if (_pSystem.isEmitting)
        {
            _fadingColor = _startLight;
            _fadingTime = 0;
            _pSystem.Stop();
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
        if (SoundPlaying) yield break;

        SoundPlaying = true;

        Debug.Log("startsound");
        var startSound = soundStarts[Random.Range(0, soundStarts.Length)];
        StartCoroutine(AudioFade.FadeIn(startSound, fadeTime));
        yield return new WaitForSeconds(startSound.clip.length - fadeTime);

        var previousSound = startSound;

        int n = Random.Range(0, soundLoops.Length);

        while (_pSystem.isEmitting)
        {
            Debug.Log("loopsound"+_pSystem.isPlaying+":"+_pSystem.isEmitting);
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

        SoundPlaying = false;
    }


    private void Awake()
    {
        _pSystem = GetComponent<ParticleSystem>();        
        _fadingTime = 0;
        _fadingColor = _startLight;
        _input = MyInput.Instance;
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
