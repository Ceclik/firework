using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using UnityEngine;

namespace Assets.Scripts.Tracking.CamFilters
{
    public class DrawRect : ICamFilter
    {
        private readonly Vector2Int[] _corners = new Vector2Int[4];

        public Mat ProcessFrame(Mat input)
        {
            //Mat output = new Mat();
            //Debug.Log("drawing line:" + new Point(_corners[0].x, _corners[0].y) + " : " + new Point(_corners[1].x, _corners[1].y));
            CvInvoke.Line(input, new Point(_corners[0].x, _corners[0].y), new Point(_corners[1].x, _corners[1].y),
                new MCvScalar(255, 0, 0));
            CvInvoke.Line(input, new Point(_corners[0].x, _corners[0].y), new Point(_corners[2].x, _corners[2].y),
                new MCvScalar(255, 0, 0));
            CvInvoke.Line(input, new Point(_corners[3].x, _corners[3].y), new Point(_corners[1].x, _corners[1].y),
                new MCvScalar(255, 0, 0));
            CvInvoke.Line(input, new Point(_corners[3].x, _corners[3].y), new Point(_corners[2].x, _corners[2].y),
                new MCvScalar(255, 0, 0));
            return input;
        }

        public void UpdateCorners(Vector2Int leftTop, Vector2Int leftBottom, Vector2Int rightTop,
            Vector2Int rightBottom)
        {
            _corners[0] = leftTop;
            _corners[1] = leftBottom;
            _corners[2] = rightTop;
            _corners[3] = rightBottom;
        }
    }
}