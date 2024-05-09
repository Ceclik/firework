using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaserCalibrate : MonoBehaviour {
    public RawImage displayImage;

    private WebCam webCam;

	// Use this for initialization
	void Start () {
        webCam = WebCam.Instance;
     //   displayImage.texture = webCam._webcam;
	}
	
	// Update is called once per frame
	void Update () {
		//if (displayImage.texture==null)
  //      {
  //          displayImage.texture = webCam._webcam;
  //      }
	}
}
