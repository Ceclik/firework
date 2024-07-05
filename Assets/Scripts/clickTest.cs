using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class clickTest : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    public TestLines testLines;
    private Texture2D distTex;

    private Texture2D sourceTex;

    // Use this for initialization
    private void Start()
    {
        sourceTex = GetComponent<RawImage>().texture as Texture2D;
        distTex = testLines.image2.texture as Texture2D;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnDrag(PointerEventData ped)
    {
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), ped.position,
                ped.pressEventCamera, out localCursor))
            return;
        localCursor.x = (int)(localCursor.x + sourceTex.width / 2);
        localCursor.y = (int)(localCursor.y + sourceTex.height / 2);
        var distPix = testLines.screenMatrix[(int)localCursor.x, (int)localCursor.y];
        UnityEngine.Debug.Log("LocalCursor:" + localCursor + "->" +
                              testLines.screenMatrix[(int)localCursor.x, (int)localCursor.y]);
        if (distPix != null)
        {
            distTex.SetPixel(distPix.distX, distPix.distY, Color.red);
            distTex.Apply();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DebugPoint(eventData);
    }

    private void DebugPoint(PointerEventData ped)
    {
        //Vector2 localCursor;
        //if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), ped.position, ped.pressEventCamera, out localCursor))
        //    return;
        //localCursor.x = (int)(localCursor.x + sourceTex.width/2);
        //localCursor.y = (int)(localCursor.y + sourceTex.height/2);
        //var distPix = testLines.screenMatrix[(int)localCursor.x, (int)localCursor.y];
        //Debug.Log("LocalCursor:" + localCursor+"->"+testLines.screenMatrix[(int)localCursor.x,(int)localCursor.y]);
        //if (distPix!=null)
        //{
        //    distTex.SetPixel(distPix.distX, distPix.distY, Color.red);
        //    distTex.Apply();
        //}
    }
}