using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

//This script is using for handling timer in UI. Counting time down and formatting it into minutes and seconds

public class Timer : MonoBehaviour {

    public float startTime;
    public float value;
    public Text text;
    

    private void OnEnable()
    {
        //startTime = GameManager.Instance.GetLevelSettings().deadTimer / 60f;
        value = startTime * 60f;
        Debug.Log("level timer:" + value);
    }

    // Update is called once per frame
    void Update () {
        value -= Time.deltaTime;
        text.text = FormatTime(value);  
        if (value<=0)
        {
            value = 0;
            GameManager.Instance.EndScene(false);
        }
	}

    private string FormatTime(float timer)
    {
        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);

        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        return niceTime;
    }
}
