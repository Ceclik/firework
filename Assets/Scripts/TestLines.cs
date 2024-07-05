using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLines : MonoBehaviour
{
    public RawImage image1;
    public RawImage image2;
    public Texture2D sourceImage;

    private readonly Vector2Int leftBottom = new(10, 10);

    private Vector2Int leftBottomF = new(0, 0);
    private readonly Vector2Int leftTop = new(10, 299);
    private Vector2Int leftTopF = new(0, 299);
    private readonly Vector2Int rightBottom = new(289, 10);
    private Vector2Int rightBottomF = new(299, 0);
    private readonly Vector2Int rightTop = new(279, 279);
    private Vector2Int rightTopF = new(299, 299);
    public Pixel[,] screenMatrix;

    private Texture2D texture1;
    private Texture2D texture2;

    private void Awake()
    {
        CreateTextures();
    }


    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }


    public Vector2Int[] Line(int x, int y, int x2, int y2)
    {
        var pixels = new List<Vector2Int>();

        var w = x2 - x;
        var h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1;
        else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1;
        else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1;
        else if (w > 0) dx2 = 1;
        var longest = Mathf.Abs(w);
        var shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1;
            else if (h > 0) dy2 = 1;
            dx2 = 0;
        }

        var numerator = longest >> 1;
        for (var i = 0; i <= longest; i++)
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

    private Pixel[,] TrapezoidToRect(Vector2Int[][] sourceTrapezoid, int sourceWidth, int sourceHeight, int distWidth,
        int distHeight)
    {
        var points = new List<Point2>();
        var sourceScreen = new Pixel[sourceWidth, sourceHeight];

        var coofH = distHeight / (float)sourceTrapezoid.Length;
        UnityEngine.Debug.Log("trapezoid transform coof height = " + distHeight + "/" + sourceTrapezoid.Length + "=" +
                              coofH);

        for (var i = 0; i < distHeight; i++)
        {
            var sourceLine = sourceTrapezoid[(int)(i / coofH)];
            var coofW = distWidth / (float)sourceLine.Length;

            for (var h = 0; h < distWidth; h++)
            {
                var point = new Point2();
                //Debug.Log("trapezoid transform coof width = " + rectWidth + "/" + sourceLine.Length + "=" + coofW);
                point.sourcePoint = new Vector2Int(sourceTrapezoid[(int)(i / coofH)][(int)(h / coofW)].x,
                    sourceTrapezoid[(int)(i / coofH)][(int)(h / coofW)].y);
                point.distPoint = new Vector2Int(h, i);
                sourceScreen[point.sourcePoint.x, point.sourcePoint.y] = new Pixel
                    { distX = point.distPoint.x, distY = point.distPoint.y };
            }
        }

        return sourceScreen;
    }

    private Vector2Int[][] Trapezoid(Vector2Int leftBottom, Vector2Int leftTop, Vector2Int rightBottom,
        Vector2Int rightTop)
    {
        var lines = new List<Vector2Int[]>();

        var leftLine = Line(leftBottom.x, leftBottom.y, leftTop.x, leftTop.y);
        var rightLine = Line(rightBottom.x, rightBottom.y, rightTop.x, rightTop.y);


        var coof = leftLine.Length / (float)rightLine.Length;
        UnityEngine.Debug.Log("coof:" + leftLine.Length + "/" + rightLine.Length + "=" + coof);

        for (var i = 0; i < leftLine.Length; i++)
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
        foreach (var pixel in pixels) texture.SetPixel(pixel.x, pixel.y, color);
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

    private bool isPointInTrapezoid(Vector2Int point, Vector2Int leftBottom, Vector2Int leftTop, Vector2Int rightBottom,
        Vector2Int rightTop)
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

    private void FillHoles(Vector2Int leftBottom, Vector2Int leftTop, Vector2Int rightBottom, Vector2Int rightTop,
        Pixel[,] matrix)
    {
        var trapPoints = TrapezoidPoints(leftBottom, leftTop, rightBottom, rightTop);
        var emptyPixelsInsideTrap = new List<Pixel>();
        for (var i = 0; i < trapPoints.Length; i++)
            if (matrix[trapPoints[i].x, trapPoints[i].y] == null)
            {
                if (matrix[trapPoints[i].x + 1, trapPoints[i].y] != null)
                    matrix[trapPoints[i].x, trapPoints[i].y] = matrix[trapPoints[i].x + 1, trapPoints[i].y];
                else if (matrix[trapPoints[i].x - 1, trapPoints[i].y] != null)
                    matrix[trapPoints[i].x, trapPoints[i].y] = matrix[trapPoints[i].x - 1, trapPoints[i].y];
                else if (matrix[trapPoints[i].x, trapPoints[i].y + 1] != null)
                    matrix[trapPoints[i].x, trapPoints[i].y] = matrix[trapPoints[i].x, trapPoints[i].y + 1];
                else if (matrix[trapPoints[i].x, trapPoints[i].y - 1] != null)
                    matrix[trapPoints[i].x, trapPoints[i].y] = matrix[trapPoints[i].x, trapPoints[i].y - 1];
            }
    }

    private Vector2Int[] TrapezoidPoints(Vector2Int leftBottom, Vector2Int leftTop, Vector2Int rightBottom,
        Vector2Int rightTop)
    {
        var points = new List<Vector2Int>();
        var corners = new Vector2Int[4] { leftBottom, leftTop, rightBottom, rightTop };
        var minX = 100000;
        var maxX = 0;
        var minY = 100000;
        var maxY = 0;

        for (var i = 0; i < corners.Length; i++)
        {
            if (corners[i].x < minX) minX = corners[i].x;
            if (corners[i].y < minY) minY = corners[i].y;
            if (corners[i].x > maxX) maxX = corners[i].x;
            if (corners[i].y > maxY) maxY = corners[i].y;
        }

        UnityEngine.Debug.Log("minx:" + minX);
        UnityEngine.Debug.Log("miny:" + minY);
        UnityEngine.Debug.Log("maxx:" + maxX);
        UnityEngine.Debug.Log("maxy:" + maxY);

        for (var x = 0; x < maxX; x++)
        for (var y = 0; y < maxY; y++)
        {
            var point = new Vector2Int(x, y);
            if (isPointInTrapezoid(point, leftBottom, leftTop, rightBottom, rightTop)) points.Add(point);
        }

        return points.ToArray();
    }

    private void DrawTrapezoid(Vector2Int leftBottom, Vector2Int leftTop, Vector2Int rightBottom, Vector2Int rightTop,
        Texture2D texture, Color color)
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
        for (var x = 0; x < sourec.width; x++)
        for (var y = 0; y < sourec.height; y++)
        {
            var color = sourec.GetPixel(x, y);
            dest.SetPixel(x, y, color);
        }

        dest.Apply();
    }

    private void CreateTextures()
    {
        var time = Time.realtimeSinceStartup;
        texture1 = new Texture2D(300, 300, TextureFormat.RGB24, false, true);
        CopyTex(sourceImage, texture1);
        texture2 = new Texture2D(300, 300, TextureFormat.RGB24, false, true);

        //var linePixels = Line(leftBottom, 0, 299, 299);
        //DrawTrapezoid(leftBottom, leftTop, rightBottom, rightTop, texture1, Color.green);
        //        DrawTrapezoid(leftBottomF, leftTopF, rightBottomF, rightTopF, texture2, Color.blue);

        //DrawPixels(texture1, TrapezoidPoints(leftBottom, leftTop, rightBottom, rightTop), Color.red);

        var lines = Trapezoid(leftBottom, leftTop, rightBottom, rightTop);
        var transf = TrapezoidToRect(lines, texture1.width, texture2.height, texture2.width, texture2.height);
        FillHoles(leftBottom, leftTop, rightBottom, rightTop, transf);

        for (var x = 0; x < texture1.width; x++)
        for (var y = 0; y < texture1.height; y++)
            if (transf[x, y] != null)
            {
                var color = texture1.GetPixel(x, y);
                texture2.SetPixel(transf[x, y].distX, transf[x, y].distY, color);
                texture1.SetPixel(x, y, Color.green);
            }

        texture1.Apply();
        texture2.Apply();
        screenMatrix = transf;

        image1.texture = texture1;
        image2.texture = texture2;
        var endtime = Time.timeSinceLevelLoad;
        UnityEngine.Debug.Log(Time.realtimeSinceStartup);
        UnityEngine.Debug.Log("completed in:" + (time - endtime));
    }
}

public struct Point2
{
    public Vector2Int sourcePoint;
    public Vector2Int distPoint;

    public override string ToString()
    {
        return sourcePoint + "->" + distPoint;
    }
}

public class Pixel
{
    public int distX;
    public int distY;

    public override string ToString()
    {
        return distX + "," + distY;
    }
}