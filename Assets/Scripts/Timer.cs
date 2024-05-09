using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script is using for handling timer in UI. Counting time down and formatting it into minutes and seconds

public class Timer : MonoBehaviour {

    public float startTime;
    public float timer;
    public Text text;

    public float star1Time = 1f;
    public float star2Time = 3f;    
    
	// Use this for initialization
	void Start () {        
        Debug.Log("getted start timer for level:"+startTime);
        star1Time = GameManager.Instance.GetLevelSettings().star1Time / 60f;
        star2Time = GameManager.Instance.GetLevelSettings().star2Time / 60f;
    }

    public int Stars()
    {
        if (timer / 60f <= 0) return 0;
        if (timer/60f < star1Time) return 1;
        if (timer/60f < star2Time) return 2;
        return 3;
    }

    private void OnEnable()
    {
        startTime = GameManager.Instance.GetLevelSettings().deadTimer / 60f;
        timer = startTime * 60f;
        Debug.Log("level timer:" + timer);
    }

    // Update is called once per frame
    void Update () {
        timer -= Time.deltaTime;
        text.text = FormatTime(timer);
        if (timer<=0)
        {
            timer = 0;
            GameManager.Instance.EndScene();
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
