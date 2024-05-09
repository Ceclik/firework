using System.Collections;
using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.UI;

//This script use for handling graphic UI in settings scene. Set time values for star amount when completing level.
//Getting time values from ui handle and translating it into understandable format of minutes and seconds

public class SettingsInterface : MonoBehaviour
{
    LevelSettings[] settings = new LevelSettings[4];
    public LevelGroup[] groups;

    public void DisableAllSliders() //there are no reason to leave these methods public if they don't used anywhere else 
    {
        foreach (var slider in GetComponentsInParent<Slider>())
        {
            slider.gameObject.SetActive(false);
        }
    }
    public void EnableAllSliders()
    {
        foreach (var slider in GetComponentsInParent<Slider>())
        {
            slider.gameObject.SetActive(true);
        }
    }
    // Use this for initialization
    void Start()
    {
        DisableAllSliders();
        GameManager.Instance.InitSettings();
        for (int i = 0; i < settings.Length; i++)
        {
            settings[i] = GameManager.Instance.GetLevelSettings(i);
            Debug.Log("timer settings:" + settings[i].deadTimer);
        }
        if (groups != null)
            for (int i = 0; i < groups.Length; i++)
            {
                Debug.Log("timer[" + i + "] value:" + settings[i].deadTimer);
                groups[i].timer.value = settings[i].deadTimer;
                if (groups[i].star1 != null)
                {
                    groups[i].star1.maxValue = settings[i].deadTimer;
                    groups[i].star2.maxValue = settings[i].deadTimer;
                }
                if (groups[i].star2 != null)
                {
                    groups[i].star2.value = settings[i].star2Time;
                    groups[i].star1.value = settings[i].star1Time;
                }
            }
        EnableAllSliders();
    }

    public void UpdateSlider(Slider slider)
    {
        LevelGroup levelGroup = slider.gameObject.GetComponentInParent<LevelGroup>();

        if (slider == levelGroup.star2)
        {
            slider.value = Mathf.Clamp(levelGroup.star2.value, levelGroup.star1.value, levelGroup.star2.maxValue);
            int minutes = Mathf.FloorToInt(slider.value / 60F);
            int seconds = Mathf.FloorToInt(slider.value - minutes * 60);

            levelGroup.star2Text.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
        if (slider == levelGroup.star1)
        {
            slider.value = Mathf.Clamp(levelGroup.star1.value, levelGroup.star1.minValue, levelGroup.star2.value);
            int minutes = Mathf.FloorToInt(slider.value / 60F);
            int seconds = Mathf.FloorToInt(slider.value - minutes * 60);

            levelGroup.star1Text.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
        if (slider == levelGroup.timer)
        {
            if (levelGroup.star1 != null && levelGroup.star2 != null)
            {
                levelGroup.star1.maxValue = levelGroup.timer.value;
                levelGroup.star2.maxValue = levelGroup.timer.value;
            }
            int minutes = Mathf.FloorToInt(slider.value / 60F);
            int seconds = Mathf.FloorToInt(slider.value - minutes * 60);

            levelGroup.timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
    }

    public void SaveSettings() //onClick methods should be named like for example OnSaveSettingsButtonClick() to show
                               //what method is doing in its name
    {
        for (int i = 0; i < groups.Length; i++)
        {
            if (groups[i].star1!=null) settings[i].star1Time = groups[i].star1.value;
            if (groups[i].star2!=null) settings[i].star2Time = groups[i].star2.value;
            settings[i].deadTimer = groups[i].timer.value;
            GameManager.Instance.UpdateLevelSettings(i, settings[i]);
        }
        GameManager.Instance.SaveSettings();
        Cursor.visible = false;
        GameManager.Instance.MainMenu();
    }

    public void CancelSettings()
    {
        Cursor.visible = false;
        GameManager.Instance.MainMenu();
    }

    public void Calibrate()
    {
        GameManager.Instance.SceneCalibrate();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
        {
            Calibrate();
        }
    }
}
