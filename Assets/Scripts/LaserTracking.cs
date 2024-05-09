using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WebCam))]
[RequireComponent(typeof(CameraTexture))]
[RequireComponent(typeof(CalibrateData))]
public class LaserTracking : Singleton<LaserTracking> {

    [DllImport("ImageDistortion", EntryPoint = "ProcessFrame")]
    public static extern void ProcessFrame(int width, int height);
    [DllImport("ImageDistortion", EntryPoint = "Initialize")]
    public static extern int Initialize();
    // Use this for initialization
    void Start () {
         Initialize();
	}
	
	// Update is called once per frame
	void Update () {        
         ProcessFrame(800, 600);       
	}
}
