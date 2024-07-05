using System;
using Assets.Scripts.Tracking;
using Assets.Scripts.Tracking.Previewvers;
using Tracking;
using UnityEngine;
using UnityEngine.UI;

public class CalibrateMenu : MonoBehaviour
{
    public GameObject previewImage;
    public GameObject hsvImage;
    public GameObject resultImage;
    public GameObject cameraSettingsPanel;
    public GameObject hsvPanel;
    public GameObject whiteBorder;

    [Space(20)] [Header("HSV Sliders")] public Slider HueMinSlider;

    public Slider HueMaxSlider;

    [Space(5)] public Slider SatMinSlider;

    public Slider SatMaxSlider;

    [Space(5)] public Slider ValMinSlider;

    public Slider ValMaxSlider;

    [Space(20)] [Header("Camera Sliders")] public Slider ExposureSlider;

    public Slider SaturationSlider;
    public Slider FocusSlider;

    private HsvPreviewer _hsvPreviewer;
    private ResultPreviewer _resultPreviewer;
    private int hsvSliderInitialized;

    private void Awake()
    {
        if (Tracker.Instance != null)
        {
            //Tracker.Instance.TurnOff();
        }
    }

    private void Start()
    {
        cameraSettingsPanel.SetActive(false);
        _hsvPreviewer = hsvImage.GetComponent<HsvPreviewer>();
        _resultPreviewer = resultImage.GetComponent<ResultPreviewer>();

        ExposureSlider.value = (float)EmguCamera.Instance.GetExposure();
        SaturationSlider.value = (float)EmguCamera.Instance.GetSaturation();
        FocusSlider.value = (float)EmguCamera.Instance.GetFocus();

        Preview();
    }

    public void Preview()
    {
        previewImage.SetActive(true);
        resultImage.SetActive(false);
        hsvImage.SetActive(false);
    }

    public void Hsv()
    {
        _hsvPreviewer.SettingsFromCamera();

        HueMinSlider.value = _hsvPreviewer.HueMin;
        HueMaxSlider.value = _hsvPreviewer.HueMax;
        SatMinSlider.value = _hsvPreviewer.SatMin;
        SatMaxSlider.value = _hsvPreviewer.SatMax;
        ValMinSlider.value = _hsvPreviewer.ValMin;
        ValMaxSlider.value = _hsvPreviewer.ValMax;

        previewImage.SetActive(false);
        resultImage.SetActive(false);
        hsvImage.SetActive(true);
    }

    public void WhiteBorder(bool border)
    {
        whiteBorder.SetActive(border);
    }

    public void Result()
    {
        _resultPreviewer.SettingsFromCamera();
        previewImage.SetActive(false);
        hsvImage.SetActive(false);
        resultImage.SetActive(true);
    }

    public void OpenCameraSettingsPanel()
    {
        cameraSettingsPanel.SetActive(true);
    }

    public void ChangeExposure(float exposure)
    {
        EmguCamera.Instance.SetExposure(exposure);
    }

    public void ChangeSaturation(float saturation)
    {
        EmguCamera.Instance.SetSaturation(saturation);
    }

    public void ChangeFocus(float focus)
    {
        EmguCamera.Instance.SetFocus(focus);
    }

    public void ChangeHsvThreshold(float Hmin, float Hmax, float Smin, float Smax, float Vmin, float Vmax)
    {
        _hsvPreviewer.HueMin = Hmin;
        _hsvPreviewer.HueMax = Hmax;
        _hsvPreviewer.SatMin = Smin;
        _hsvPreviewer.SatMax = Smax;
        _hsvPreviewer.ValMin = Vmin;
        _hsvPreviewer.ValMax = Vmax;
        EmguCamera.Instance.SaveCurrentSettings();
    }

    public void SaveSettings()
    {
        EmguCamera.Instance.SaveCurrentSettings();
        cameraSettingsPanel.SetActive(false);
    }

    public void CancelSettings()
    {
        EmguCamera.Instance.CancelSettings();
        cameraSettingsPanel.SetActive(false);
    }

    public void OnUpdateHueSlider(Slider slider)
    {
        if (hsvSliderInitialized < 6)
        {
            hsvSliderInitialized++;
            return;
        }

        UnityEngine.Debug.Log("SLIDER UPDATE!");
        Slider sliderMin;
        Slider sliderMax;

        var isMin = false; //is slider in event for min or max value

        if (slider.name.Contains("Min")) isMin = true;

        //Find min and max sliders for change properly baseed on min and max values 
        //(min slider value cannot be greater then max slider value etc)
        if (slider.name.Contains("Hue"))
        {
            sliderMin = HueMinSlider;
            sliderMax = HueMaxSlider;
        }
        else if (slider.name.Contains("Sat"))
        {
            sliderMin = SatMinSlider;
            sliderMax = SatMaxSlider;
        }
        else if (slider.name.Contains("Val"))
        {
            sliderMin = ValMinSlider;
            sliderMax = ValMaxSlider;
        }
        else
        {
            throw new ArgumentException("Invalid slider in event");
        }

        if (isMin)
            sliderMin.value = Mathf.Clamp(sliderMin.value, sliderMin.minValue, sliderMax.value);
        else
            sliderMax.value = Mathf.Clamp(sliderMax.value, sliderMin.value, sliderMax.maxValue);

        ChangeHsvThreshold(HueMinSlider.value, HueMaxSlider.value, SatMinSlider.value, SatMaxSlider.value,
            ValMinSlider.value, ValMaxSlider.value);
    }
}