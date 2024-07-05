using UnityEngine;
using UnityEngine.UI;

public class ComPort : MonoBehaviour
{
    public Text comText;

    private int counter;

    // Use this for initialization
    private void Start()
    {
        GetComponent<InputField>().text = GameManager.Instance.ComPort;
        UnityEngine.Debug.Log("setting comport string");
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void ChangeCom(string com)
    {
        if (counter > 0)
        {
            GameManager.Instance.ComPort = com;
            PlayerPrefs.SetString("COM", com);
            UnityEngine.Debug.Log("Com port settings saved:" + com);
        }

        counter++;
    }
}