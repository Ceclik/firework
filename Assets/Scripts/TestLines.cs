using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestLines : MonoBehaviour
{

    public RawImage image1;
    public RawImage image2;
    public Texture2D sourceImage;
    public Pixel[,] screenMatrix;

    private Texture2D texture1;
    private Texture2D texture2;

    Vector2Int leftBottom = new Vector2Int(10, 10);
    Vector2Int rightBottom = new Vector2Int(289, 10);
    Vector2Int leftTop = new Vector2Int(10, 299);
    Vector2Int rightTop = new Vector2Int(279, 279);

    Vector2Int leftBottomF = new Vector2Int(0, 0);
    Vector2Int rightBottomF = new Vector2Int(299, 0);
    Vector2Int leftTopF = new Vector2Int(0, 299);
    Vector2Int rightTopF = new Vector2Int(299, 299);

    

    public Vector2Int[] Line(int x, int y, int x2, int y2)
    {
        List<Vector2Int> pixels = new List<Vector2Int>();

        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            pixels.Add(new Vector2Int(x, y));
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }
        return pixels.ToArray();
    }


    // Use this for initialization
    void Start()
    {
        
    }
    private void Awake()
    {
        CreateTextures();
    }
    // Update is called once per frame
    void Update()
    {

    }

    private Pixel[,] TrapezoidToRect(Vector2Int[][] sourceTrapezoid, int sourceWidth,int sourceHeight,int distWidth, int distHeight)
    {
        List<Point2> points = new List<Point2>();
        var sourceScreen = new Pixel[sourceWidth,sourceHeight];

        float coofH = (float)distHeight / (float)sourceTrapezoid.Length;
        Debug.Log("trapezoid transform coof height = " + distHeight + "/" + sourceTrapezoid.Length + "=" + coofH);

        for (int i = 0; i < distHeight; i++)
        {
            var sourceLine = sourceTrapezoid[(int)(i / coofH)];            
            float coofW = (float)distWidth / (float)sourceLine.Length;

            for (int h = 0; h < distWidth; h++)
            {
                var point = new Point2();
                //Debug.Log("trapezoid transform coof width = " + rectWidth + "/" + sourceLine.Length + "=" + coofW);
                point.sourcePoint = new Vector2Int(sourceTrapezoid[(int)(i / coofH)][(int)(h / coofW)].x, sourceTrapezoid[(int)(i / coofH)][(int)(h / coofW)].y);
                point.distPoint = new Vector2Int(h, i);
                sourceScreen[point.sourcePoint.x, point.sourcePoint.y] = new Pixel { distX = point.distPoint.x, distY = point.distPoint.y };
            }
        }
        return sourceScreen;
    }

    private Vector2Int[][] Trapezoid(Vector2Int leftBottom, Vector2Int leftTop, Vector2Int rightBottom, Vector2Int rightTop)
    {
        List<Vector2Int[]> lines = new List<Vector2Int[]>();

        var leftLine = Line(leftBottom.x, leftBottom.y, leftTop.x, leftTop.y);
        var rightLine = Line(rightBottom.x, rightBottom.y, rightTop.x, rightTop.y);


        float coof = (float)leftLine.Length / (float)rightLine.Length;
        Debug.Log("coof:" + leftLine.Length + "/" + rightLine.Length + "=" + coof);

        for (int i = 0; i < leftLine.Length; i++)
        {
            var pixelS = leftLine[i];
            var pixelT = rightLine[(int)(i / coof)];
            lines.Add(Line(pixelS.x, pixelS.y, pixelT.x, pixelT.y));
        }


        //var bottomLine = Line(leftBottom.x, leftBottom.y, rightBottom.x, rightBottom.y);
        //var topLine = Line(leftTop.x, leftTop.y, rightTop.x, rightTop.y);


        //float coof2 = (float)bottomLine.Length / (float)topLine.Length;
        //Debug.Log("coof:" + bottomLine.Length + "/" + topLine.Length + "=" + coof2);

        //for (int i = 0; i < leftLine.Length; i++)
        //{
        //    var pixelS = bottomLine[i];
        //    var pixelT = topLine[(int)(i / coof2)];
        //    lines.Add(Line(pixelS.x, pixelS.y, pixelT.x, pixelT.y));
        //}

        return lines.ToArray();
    }

    private void DrawPixels(Texture2D texture, Vector2Int[] pixels, Color color)
    {
        foreach (var pixel in pixels)
        {
            texture.SetPixel(pixel.x, pixel.y, color);
        }
        texture.Apply();
    }


    private float TrapezoidArea(Vector2Int leftBottom, Vector2Int leftTop, Vector2Int rightBottom, Vector2Int rightTop)
    {
        var tri1 = TriangleArea(leftBottom.x, leftBottom.y, leftTop.x, leftTop.y, rightBottom.x, rightBottom.y);
        var tri2 = TriangleArea(leftTop.x, leftTop.y, rightBottom.x, rightBottom.y, rightTop.x, rightTop.y);

        var area = tri1 + tri2;

        return area;
        //Debug.Log("rect area: " +tri1+"+"+tri2+"="+ area);
    }

    private bool isPointInTrapezoid(Vector2Int point, Vector2Int leftBottom, Vector2Int leftTop, Vector2Int rightBottom, Vector2Int rightTop)
    {
        var tri1 = TriangleArea(leftBottom.x, leftBottom.y, point.x, point.y, rightBottom.x, rightBottom.y);
        var tri2 = TriangleArea(leftBottom.x, leftBottom.y, leftTop.x, leftTop.y, point.x, point.y);
        var tri3 = TriangleArea(leftTop.x, leftTop.y, point.x, point.y, rightTop.x, rightTop.y);
        var tri4 = TriangleArea(point.x, point.y, rightTop.x, rightTop.y, rightBottom.x, rightBottom.y);
        var pointArea = tri1 + tri2 + tri3 + tri4;
        var rectArea = TrapezoidArea(leftBottom, leftTop, rightBottom, rightTop);
        if (pointArea == rectArea) return true;
        return false;
    }

    private float TriangleArea(int x1, int y1, int x2,
                       int y2, int x3, int y3)
    {
        return Mathf.Abs((x1 * (y2 - y3) +
                         x2 * (y3 - y1) +
                         x3 * (y1 - y2)) / 2.0f);
    }

    private void FillHoles(Vector2Int leftBottom, Vector2Int leftTop, Vector2Int rightBottom, Vector2Int rightTop, Pixel[,] matrix)
    {
        var trapPoints = TrapezoidPoints(leftBottom, leftTop, rightBottom, rightTop);
        List<Pixel> emptyPixelsInsideTrap = new List<Pixel>();
        for (int i = 0; i < trapPoints.Length; i++)
        {
            if (matrix[trapPoints[i].x,trapPoints[i].y]==null)
            {
                if (matrix[trapPoints[i].x+1,trapPoints[i].y]!=null)
                {
                    matrix[trapPoints[i].x, trapPoints[i].y] = matrix[trapPoints[i].x + 1, trapPoints[i].y];
                }
                else if(matrix[trapPoints[i].x - 1, trapPoints[i].y] != null)
                {
                    matrix[trapPoints[i].x, trapPoints[i].y] = matrix[trapPoints[i].x - 1, trapPoints[i].y];
                }
                else if (matrix[trapPoints[i].x, trapPoints[i].y+1] != null)
                {
                    matrix[trapPoints[i].x, trapPoints[i].y] = matrix[trapPoints[i].x, trapPoints[i].y+1];
                }
                else if (matrix[trapPoints[i].x, trapPoints[i].y - 1] != null)
                {
                    matrix[trapPoints[i].x, trapPoints[i].y] = matrix[trapPoints[i].x, trapPoints[i].y - 1];
                }

            }
        }        
    }

    private Vector2Int[] TrapezoidPoints(Vector2Int leftBottom, Vector2Int leftTop, Vector2Int rightBottom, Vector2Int rightTop)
    {
        List<Vector2Int> points = new List<Vector2Int>();
        Vector2Int[] corners = new Vector2Int[4] { leftBottom, leftTop, rightBottom, rightTop };
        int minX = 100000;
        int maxX = 0;
        int minY = 100000;
        int maxY = 0;

        for (int i = 0; i < corners.Length; i++)
        {
            if (corners[i].x < minX) minX = corners[i].x;
            if (corners[i].y < minY) minY = corners[i].y;
            if (corners[i].x > maxX) maxX = corners[i].x;
            if (corners[i].y > maxY) maxY = corners[i].y;
        }

        Debug.Log("minx:" + minX);
        Debug.Log("miny:" + minY);
        Debug.Log("maxx:" + maxX);
        Debug.Log("maxy:" + maxY);

        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                var point = new Vector2Int(x, y);                
                if (isPointInTrapezoid(point, leftBottom, leftTop, rightBottom, rightTop))
                {
                    points.Add(point);
                }
            }
        }

        return points.ToArray();
    }

    private void DrawTrapezoid(Vector2Int leftBottom, Vector2Int leftTop, Vector2Int rightBottom, Vector2Int rightTop, Texture2D texture, Color color)
    {
        var leftLine = Line(leftBottom.x, leftBottom.y, leftTop.x, leftTop.y);
        var rightLine = Line(rightBottom.x, rightBottom.y, rightTop.x, rightTop.y);
        var topLine = Line(leftTop.x, leftTop.y, rightTop.x, rightTop.y);
        var bottomLine = Line(leftBottom.x, leftBottom.y, rightBottom.x, rightBottom.y);

        //var lines = Trapezoid(leftBottom, leftTop, rightBottom, rightTop);
        //foreach (var line in lines)
        //{
        //    DrawPixels(texture, line, Color.cyan);
        //}

        DrawPixels(texture, leftLine, color);
        DrawPixels(texture, rightLine, color);
        DrawPixels(texture, topLine, color);
        DrawPixels(texture, bottomLine, color);
    }

    private void CopyTex(Texture2D sourec, Texture2D dest)
    {
        for (int x = 0; x < sourec.width; x++)
        {
            for (int y = 0; y < sourec.height; y++)
            {
                var color = sourec.GetPixel(x, y);
                dest.SetPixel(x, y, color);
            }
        }
        dest.Apply();
    }

    private void CreateTextures()
    {
        float time = Time.realtimeSinceStartup;
        texture1 = new Texture2D(300, 300, TextureFormat.RGB24, false, true);
        CopyTex(sourceImage, texture1);
        texture2 = new Texture2D(300, 300, TextureFormat.RGB24, false, true);

        //var linePixels = Line(leftBottom, 0, 299, 299);
        //DrawTrapezoid(leftBottom, leftTop, rightBottom, rightTop, texture1, Color.green);
        //        DrawTrapezoid(leftBottomF, leftTopF, rightBottomF, rightTopF, texture2, Color.blue);

        //DrawPixels(texture1, TrapezoidPoints(leftBottom, leftTop, rightBottom, rightTop), Color.red);

        var lines = Trapezoid(leftBottom, leftTop, rightBottom, rightTop);
        var transf = TrapezoidToRect(lines, texture1.width,texture2.height, texture2.width, texture2.height);
        FillHoles(leftBottom, leftTop, rightBottom, rightTop, transf);        

        for (int x = 0; x < texture1.width; x++)
        {
            for (int y = 0; y < texture1.height; y++)
            {
                if (transf[x,y]!=null)
                {
                    var color = texture1.GetPixel(x, y);
                    texture2.SetPixel(transf[x,y].distX, transf[x,y].distY, color);
                    texture1.SetPixel(x, y, Color.green);
                }
            }
        }
       
        texture1.Apply();
        texture2.Apply();
        screenMatrix = transf;

        image1.texture = texture1;
        image2.texture = texture2;
        float endtime = Time.timeSinceLevelLoad;
        Debug.Log(Time.realtimeSinceStartup);
        Debug.Log("completed in:" + (time - endtime));
    }    
}

public struct Point2
{
    public Vector2Int sourcePoint;
    public Vector2Int distPoint;
    public override string ToString()
    {
        return sourcePoint.ToString() + "->" + distPoint.ToString();
    }
}

public class Pixel
{
    public int distX;
    public int distY;
    public override string ToString()
    {
        return distX.ToString() + "," + distY.ToString();
    }
}