using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CornerDrag : MonoBehaviour,IDragHandler,IEndDragHandler {

    private CropRectPreview _rect;
    private Canvas _canvas;

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        transform.position = new Vector2(
            Mathf.Clamp(transform.position.x, 0, _canvas.pixelRect.width - 1),
            Mathf.Clamp(transform.position.y, 0, _canvas.pixelRect.height - 1)
            );
        _rect.UpdateCoords();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _rect.SaveCoords();
    }

    // Use this for initialization
    void Start () {
        _rect = GetComponentInParent<CropRectPreview>();
        _canvas = GetComponentInParent<Canvas>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
