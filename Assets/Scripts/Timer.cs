using UnityEngine;
using UnityEngine.UI;

//This script is using for handling timer in UI. Counting time down and formatting it into minutes and seconds

public class Timer : MonoBehaviour
{
    public float startTime;
    public float value;
    public Text text;

    // Update is called once per frame
    private void Update()
    {
        value -= Time.deltaTime;
        text.text = FormatTime(value);
        if (value <= 0)
        {
            value = 0;
            GameManager.Instance.EndScene(false);
        }
    }


    private void OnEnable()
    {
        //startTime = GameManager.Instance.GetLevelSettings().deadTimer / 60f;
        value = startTime * 60f;
        UnityEngine.Debug.Log("level timer:" + value);
    }

    private string FormatTime(float timer)
    {
        var minutes = Mathf.FloorToInt(timer / 60F);
        var seconds = Mathf.FloorToInt(timer - minutes * 60);

        var niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        return niceTime;
    }
}