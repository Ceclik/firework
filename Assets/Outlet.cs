using System.IO.Ports;
using Scenes;
using UnityEngine;

//This script is handling outlet object in scene. Interaction with it and its behaviour.

public class Outlet : MonoBehaviour
{

    public FireSystem fireSystem;
    public bool keyControlled;
    public ParticleSystem gas;
    public AudioSource kettleSound;

    private Collider collider;
    private Animator anim;
    private bool falled;
    private AudioSource audio;
    private SerialPort _com;

    private bool gasCrane;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        collider = GetComponentInChildren<BoxCollider>();
        audio = GetComponent<AudioSource>();
        _com = new SerialPort(GameManager.Instance.comPort, 9600);
        Debug.Log("Opening com port:" + GameManager.Instance.comPort);
        _com.Open();
        _com.ReadTimeout = 1;
        if (_com.IsOpen) Debug.Log("COM PORT OPENED");
        gasCrane = gas == null ? false : true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!falled && GameManager.Instance.IsFireStarted)
        {
            int fromCom = int.MaxValue;
            int comExpected = gasCrane ? 3 : 2;
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

                if (collider.Raycast(ray, out hit, Mathf.Infinity))
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

    public void OutletFall()
    {
        audio.Play();
        if (kettleSound != null)
        {
            StartCoroutine(AudioFade.FadeOut(kettleSound, 1f));
        }
        fireSystem.fake = false;
        fireSystem.startOver = false;
        anim.SetTrigger("Fall");
        falled = true;
        if (gas != null) { gas.Stop(); }
    }
    private void OnDisable()
    {
        _com.Close();
        _com.Dispose();
    }
}
