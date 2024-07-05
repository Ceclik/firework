using System.Runtime.InteropServices;
using UnityEngine;
using Utils;

[RequireComponent(typeof(WebCam))]
[RequireComponent(typeof(CameraTexture))]
[RequireComponent(typeof(CalibrateData))]
public class LaserTracking : Singleton<LaserTracking>
{
    // Use this for initialization
    private void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    private void Update()
    {
        ProcessFrame(800, 600);
    }

    [DllImport("ImageDistortion", EntryPoint = "ProcessFrame")]
    public static extern void ProcessFrame(int width, int height);

    [DllImport("ImageDistortion", EntryPoint = "Initialize")]
    public static extern int Initialize();
}