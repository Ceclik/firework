using Assets.Scripts.Tracking;
using Assets.Scripts.Tracking.Previewvers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropRectPreview : MonoBehaviour {

    public Transform leftTop;
    public Transform leftBottom;
    public Transform rightTop;
    public Transform rightBottom;

    public Vector2Int leftTopScreen;
    public Vector2Int leftBottomScreen;
    public Vector2Int rightTopScreen;
    public Vector2Int rightBottomScreen;

    private Camera _camera;
    private Canvas _canvas;
    private RectPreviewer _rectPreviewer;

    // Use this for initialization
    void Start () {
        _camera = FindObjectOfType<Camera>();
        _canvas = GetComponentInParent<Canvas>();
        _rectPreviewer = GetComponentInParent<RectPreviewer>();        

        leftTop.position = EmguCamera.Instance.CameraToScreen(EmguCamera.Instance.LeftTop,_canvas);
        leftBottom.position = EmguCamera.Instance.CameraToScreen(EmguCamera.Instance.LeftBottom, _canvas);
        rightTop.position = EmguCamera.Instance.CameraToScreen(EmguCamera.Instance.RightTop, _canvas);
        rightBottom.position = EmguCamera.Instance.CameraToScreen(EmguCamera.Instance.RightBottom, _canvas);

        UpdateCoords();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateCoords()
    {
        leftTopScreen = Vector2Int.FloorToInt(RectTransformUtility.PixelAdjustPoint(leftTop.position, leftTop, _canvas));
        leftBottomScreen = Vector2Int.FloorToInt(RectTransformUtility.PixelAdjustPoint(leftBottom.position, leftTop, _canvas));
        rightTopScreen = Vector2Int.FloorToInt(RectTransformUtility.PixelAdjustPoint(rightTop.position, leftTop, _canvas));
        rightBottomScreen = Vector2Int.FloorToInt(RectTransformUtility.PixelAdjustPoint(rightBottom.position, leftTop, _canvas));

        _rectPreviewer.LeftTop = EmguCamera.Instance.ScreenToCamera(leftTopScreen, _canvas);        
        _rectPreviewer.LeftBottom = EmguCamera.Instance.ScreenToCamera(leftBottomScreen, _canvas);
        _rectPreviewer.RightTop = EmguCamera.Instance.ScreenToCamera(rightTopScreen, _canvas);
        _rectPreviewer.RightBottom = EmguCamera.Instance.ScreenToCamera(rightBottomScreen, _canvas);
    }

    public void SaveCoords()
    {
        UpdateCoords();
        EmguCamera.Instance.LeftTop = EmguCamera.Instance.ScreenToCamera(leftTopScreen, _canvas);
        EmguCamera.Instance.LeftBottom = EmguCamera.Instance.ScreenToCamera(leftBottomScreen, _canvas);
        EmguCamera.Instance.RightTop = EmguCamera.Instance.ScreenToCamera(rightTopScreen, _canvas);
        EmguCamera.Instance.RightBottom = EmguCamera.Instance.ScreenToCamera(rightBottomScreen, _canvas);
        EmguCamera.Instance.SaveCurrentSettings();
    }

    private Vector2Int Vec2(Vector3 coords)
    {        
        Vector2Int position = new Vector2Int();
        position.x = (int)coords.x;
        position.y = (int)coords.y;
        return position;
    }
}
