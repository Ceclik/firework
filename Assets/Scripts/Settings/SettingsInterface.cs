using Settings;
using UnityEngine;
using UnityEngine.UI;

//This script use for handling graphic UI in settings scene. Set time values for star amount when completing level.
//Getting time values from ui handle and translating it into understandable format of minutes and seconds

public class SettingsInterface : MonoBehaviour
{
    public LevelGroup[] groups;

    private readonly LevelSettings[] settings = new LevelSettings[4];

    // Use this for initialization
    private void Start()
    {
        DisableAllSliders();
        GameManager.Instance.InitSettings();
        for (var i = 0; i < settings.Length; i++)
        {
            settings[i] = GameManager.Instance.GetLevelSettings(i);
            UnityEngine.Debug.Log("timer settings:" + settings[i].deadTimer);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) Calibrate();
    }

    public void
        DisableAllSliders() //there are no reason to leave these methods public if they don't used anywhere else 
    {
        foreach (var slider in GetComponentsInParent<Slider>()) slider.gameObject.SetActive(false);
    }

    public void EnableAllSliders()
    {
        foreach (var slider in GetComponentsInParent<Slider>()) slider.gameObject.SetActive(true);
    }

    public void SaveSettings() //onClick methods should be named like for example OnSaveSettingsButtonClick() to show
        //what method is doing in its name
    {
        /*for (int i = 0; i < groups.Length; i++)
        {
            if (groups[i].star1!=null) settings[i].star1Time = groups[i].star1.value;
            if (groups[i].star2!=null) settings[i].star2Time = groups[i].star2.value;
            settings[i].deadTimer = groups[i].timer.value;
            GameManager.Instance.UpdateLevelSettings(i, settings[i]);
        }*/
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
}