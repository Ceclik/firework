using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebCam : Singleton<WebCam> {

    [HideInInspector]    
    public int camWidth = 800;
    public int camHeight = 600;
    public bool CameraFlip = false;
    public WebCamTexture _webcam;

    //private WebCamTexture _webcam;

    IEnumerator Start()
    {
        Debug.Log("webcam start");
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            for (int i = 0; i < devices.Length; i++)
                Debug.Log(devices[i].name);

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

    
    void Update () {
        
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
        float x = (float)screenCoords.x / ((float)Screen.width / (float)camWidth);
        float y = (float)screenCoords.y / ((float)Screen.height / (float)camHeight);
        return new Vector2Int((int)x, (int)y);
    }

    public Vector2Int CamToScreen(Vector2Int camCoords)
    {
        float coofX = ((float)Screen.width / (float)camWidth);
        float coofY = ((float)Screen.height / (float)camHeight);
        int x = (int)((float)camCoords.x * coofX);
        int y = (int)((float)camCoords.y * coofY);
        return new Vector2Int(x, y);
    }
}
