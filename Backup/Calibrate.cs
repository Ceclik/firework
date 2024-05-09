using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Calibrate : MonoBehaviour
{

    public GameObject[] corners;
    public GameObject displayPlane;
    public Texture2D displayTexture;

    private CalibrateData _calibrateData;
    private WebCam _camTexture;
    private bool started;
    private readonly int _maxDist = 200;

    void Start()
    {
        //_camTexture = FindObjectOfType<WebCam>();
        //_camTexture.Restart();
        //_calibrateData = FindObjectOfType<CalibrateData>();
        //displayTexture = new Texture2D(_camTexture.camWidth, _camTexture.camHeight,TextureFormat.ARGB32,false,true);        
        //StartCoroutine(FindCorners());
    }

    void Update()
    {
        if (_camTexture.data.Length > 0)
        {
            displayTexture.SetPixels32(_camTexture.data);
            displayTexture.Apply();
        }
    }

    IEnumerator FindCorners()
    {
        _calibrateData.corners = new Vector2Int[4];
        byte r = 130;
        byte g = 70;
        byte b = 70;
        while (_camTexture.data.Length < 1)
        {
            yield return null;
        }
        ShowCorners(0);
        yield return new WaitForSeconds(1f);
        Vector2Int leftTop = Vector2Int.zero;
        while (leftTop == Vector2Int.zero)
        {
            leftTop = FindRed(r, g, b, true, true);
            yield return null;
        }
        Debug.Log(leftTop);
        _calibrateData.corners[0] = leftTop;
        ShowCorners(1);
        Vector2Int leftBottom = Vector2Int.zero;
        yield return new WaitForSeconds(1f);
        while (leftBottom == Vector2Int.zero)
        {
            leftBottom = FindRed(r, g, b, true, false);
            yield return null;
        }
        Debug.Log(leftBottom);
        _calibrateData.corners[1] = leftBottom;
        ShowCorners(2);
        Vector2Int rightTop = Vector2Int.zero;
        yield return new WaitForSeconds(1f);
        while (rightTop == Vector2Int.zero)
        {
            rightTop = FindRed(r, g, b, false, true);
            yield return null;
        }
        Debug.Log(rightTop);
        _calibrateData.corners[2] = rightTop;
        ShowCorners(3);
        Vector2Int rightBottom = Vector2Int.zero;
        yield return new WaitForSeconds(1f);
        while (rightBottom == Vector2Int.zero)
        {
            rightBottom = FindRed(r, g, b, false, false);
            yield return null;
        }
        Debug.Log(rightBottom);
        _calibrateData.corners[3] = rightBottom;
        ShowCorners(4);
        var trPlane = FindObjectOfType<TrackingPlane>();
        trPlane.CreatePlane();
        //SceneManager.LoadScene(0);
    }

    private Vector2Int FindRed(byte redValue, byte greenValue, byte blueValue, bool left, bool top)
    {

        int maxX = 0, minX = _camTexture.camWidth, maxY = 0, minY = _camTexture.camHeight;
        bool finded = false;
        int index = 0;
        for (int y = _camTexture.camHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < _camTexture.camWidth; x++)
            {
                if (_camTexture.data[index].r > redValue && _camTexture.data[index].g < greenValue && _camTexture.data[index].b < blueValue)
                {
                    if (x > maxX) maxX = x;
                    if (x < minX) minX = x;
                    if (y > maxY) maxY = y;
                    if (y < minY) minY = y;
                    finded = true;
                }
                index++;
            }
        }
        if (!finded) return Vector2Int.zero;
        int xx, yy;

        if (left) xx = minX;
        else xx = maxX;

        if (top) yy = minY;
        else yy = maxY;

        return new Vector2Int(xx, yy);
    }

    private void ShowCorners(byte corner)
    {
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i].SetActive(false);
        }
        if (corner < 4)
            corners[corner].SetActive(true);
    }
}
