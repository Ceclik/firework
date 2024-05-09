using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComPort : MonoBehaviour
{

    public Text comText;

    private int counter = 0;
    // Use this for initialization
    void Start()
    {
        GetComponent<InputField>().text = GameManager.Instance.ComPort;
        Debug.Log("setting comport string");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeCom(string com)
    {
        if (counter > 0)
        {
            GameManager.Instance.ComPort = com;
            PlayerPrefs.SetString("COM", com);
            Debug.Log("Com port settings saved:" + com);
        }
        counter++;
    }
}
