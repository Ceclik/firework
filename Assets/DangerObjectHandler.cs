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

    private Collider _collider;
    private Animator _anim;
    private bool _falled;
    private AudioSource _audio;
    private SerialPort _com;

    private bool _gasCrane;

    // Use this for initialization
    void Start()
    {
        _anim = GetComponent<Animator>();
        _collider = GetComponentInChildren<BoxCollider>();
        _audio = GetComponent<AudioSource>();
        _com = new SerialPort(GameManager.Instance.comPort, 9600);
        Debug.Log("Opening com port:" + GameManager.Instance.comPort);
        _com.Open();
        _com.ReadTimeout = 1;
        if (_com.IsOpen) Debug.Log("COM PORT OPENED");
        _gasCrane = gas == null ? false : true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_falled && GameManager.Instance.IsFireStarted)
        {
            int fromCom = int.MaxValue;
            int comExpected = _gasCrane ? 3 : 2;
            if (_com.IsOpen)
            {
                try
                {
                    fromCom = _com.ReadByte();
                    Debug.Log("Serial:" + fromCom);
                }
                catch (System.Exception e)
                {

                }
            }
            if (!keyControlled)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (_collider.Raycast(ray, out hit, Mathf.Infinity))
                {
                    OutletFall();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Return) || fromCom == comExpected)
                {
                    OutletFall();
                }
            }
        }
    }

    private void OutletFall()
    {
        _audio.Play();
        if (kettleSound != null)
        {
            StartCoroutine(AudioFade.FadeOut(kettleSound, 1f));
        }

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
        if (gas != null) { gas.Stop(); }
    }
    private void OnDisable()
    {
        _com.Close();
        _com.Dispose();
    }
}
