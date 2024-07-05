using System;
using System.IO.Ports;
using FireAimScripts;
using FireSeekingScripts;
using UnityEngine;

//This script is handling outlet object in scene. Interaction with it and its behaviour.

public class DangerObjectHandler : MonoBehaviour
{
    [SerializeField] private AimFireSystemHandler aimFireSystem;
    [SerializeField] private SeekingFireSystemHandler seekingFireSystem;
    [SerializeField] private bool keyControlled;
    [SerializeField] private ParticleSystem gas;
    [SerializeField] private AudioSource kettleSound;
    private Animator _anim;
    private AudioSource _audio;

    private Collider _collider;
    private SerialPort _com;
    private bool _falled;

    private bool _gasCrane;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _collider = GetComponentInChildren<BoxCollider>();
        _audio = GetComponent<AudioSource>();
        _com = new SerialPort(GameManager.Instance.ComPort, 9600);
        UnityEngine.Debug.Log("Opening com port:" + GameManager.Instance.ComPort);
        _com.Open();
        _com.ReadTimeout = 1;
        if (_com.IsOpen) UnityEngine.Debug.Log("COM PORT OPENED");
        _gasCrane = gas != null;
    }

    private void Update()
    {
        if (!_falled && GameManager.Instance.IsFireStarted)
        {
            var fromCom = int.MaxValue;
            var comExpected = _gasCrane ? 3 : 2;
            if (_com.IsOpen)
                try
                {
                    fromCom = _com.ReadByte();
                    UnityEngine.Debug.Log("Serial:" + fromCom);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e);
                }

            if (!keyControlled)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (_collider.Raycast(ray, out _, Mathf.Infinity))
                    OutletFall();
            }
            else if (Input.GetKeyDown(KeyCode.Return) || fromCom == comExpected)
            {
                OutletFall();
            }
        }
    }

    private void OnDisable()
    {
        _com.Close();
        _com.Dispose();
    }

    private void OutletFall()
    {
        _audio.Play();
        if (kettleSound != null) StartCoroutine(AudioFade.FadeOut(kettleSound, 1f));

        if (GameManager.Instance.FireAimGameMode)
        {
            aimFireSystem.fake = false;
            aimFireSystem.startOver = false;
        }
        else if (GameManager.Instance.FireSeekGameMode)
        {
            seekingFireSystem.fake = false;
            seekingFireSystem.startOver = false;
        }

        _anim.SetTrigger("Fall");
        _falled = true;
        if (gas != null) gas.Stop();
    }
}