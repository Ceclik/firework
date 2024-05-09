using Assets.Scripts.Tracking;
using Assets.Scripts.Tracking.Previewvers;
using Assets.Scripts.Tracking.Trapezoid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackingTest : MonoBehaviour {

    private TrapezoidConverter _converter;
    private ResultPreviewer _previewer;
    private Canvas _canvas;

    private void OnEnable()
    {
        var emgu = EmguCamera.Instance;
        _converter = new TrapezoidConverter(emgu.LeftTop, emgu.LeftBottom, emgu.RightTop, emgu.RightBottom, emgu.Width, emgu.Height);
        _canvas = GetComponentInParent<Canvas>();
        _previewer = GetComponentInParent<ResultPreviewer>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var point = _previewer.GetTrackingPoint();

        if (point !=Vector2.zero)
        {
            var intPoint = Vector2Int.RoundToInt(point);            
            var distPoint = _converter.TransformMatrix[Mathf.Clamp(intPoint.x,0,_converter.TransformMatrix.GetLength(0)-1), Mathf.Clamp(intPoint.y, 0, _converter.TransformMatrix.GetLength(1)-1)];
          if (distPoint!=null)
            {
                transform.position = EmguCamera.Instance.CameraToScreen(new Vector2Int(distPoint.distX,distPoint.distY),_canvas);
            }
        }
	}
}
