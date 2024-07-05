using System.Collections;
using UnityEngine;
using Utils;

public class WebCam : Singleton<WebCam>
{
    [HideInInspector] public int camWidth = 800;

    public int camHeight = 600;
    public bool CameraFlip;
    public WebCamTexture _webcam;

    //private WebCamTexture _webcam;

    private IEnumerator Start()
    {
        UnityEngine.Debug.Log("webcam start");
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            var devices = WebCamTexture.devices;
            for (var i = 0; i < devices.Length; i++)
                UnityEngine.Debug.Log(devices[i].name);

            _webcam = new WebCamTexture
            {
                requestedWidth = camWidth,
                requestedHeight = camHeight
            };
            _webcam.deviceName = devices[devices.Length - 1].name;
            _webcam.Play();
            camWidth = _webcam.width;
            camHeight = _webcam.height;
        }
    }


    private void Update()
    {
    }

    public void Restart()
    {
        _webcam.Play();
    }

    public void Stop()
    {
        _webcam.Stop();
    }

    public Vector2Int ScreenToCam(Vector2Int screenCoords)
    {
        var x = screenCoords.x / (Screen.width / (float)camWidth);
        var y = screenCoords.y / (Screen.height / (float)camHeight);
        return new Vector2Int((int)x, (int)y);
    }

    public Vector2Int CamToScreen(Vector2Int camCoords)
    {
        var coofX = Screen.width / (float)camWidth;
        var coofY = Screen.height / (float)camHeight;
        var x = (int)(camCoords.x * coofX);
        var y = (int)(camCoords.y * coofY);
        return new Vector2Int(x, y);
    }
}