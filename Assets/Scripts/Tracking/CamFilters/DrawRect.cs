using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using UnityEngine;

namespace Assets.Scripts.Tracking.CamFilters
{
    public class DrawRect : ICamFilter
    {
        private Vector2Int[] _corners = new Vector2Int[4];

        public void UpdateCorners(Vector2Int leftTop, Vector2Int leftBottom, Vector2Int rightTop, Vector2Int rightBottom)
        {
            _corners[0] = leftTop;
            _corners[1] = leftBottom;
            _corners[2] = rightTop;
            _corners[3] = rightBottom;
         }

        public Mat ProcessFrame(Mat input)
        {
            //Mat output = new Mat();
            //Debug.Log("drawing line:" + new Point(_corners[0].x, _corners[0].y) + " : " + new Point(_corners[1].x, _corners[1].y));
            CvInvoke.Line(input, new Point(_corners[0].x, _corners[0].y), new Point(_corners[1].x, _corners[1].y), new Emgu.CV.Structure.MCvScalar(255,0,0));
            CvInvoke.Line(input, new Point(_corners[0].x, _corners[0].y), new Point(_corners[2].x, _corners[2].y), new Emgu.CV.Structure.MCvScalar(255, 0, 0));
            CvInvoke.Line(input, new Point(_corners[3].x, _corners[3].y), new Point(_corners[1].x, _corners[1].y), new Emgu.CV.Structure.MCvScalar(255, 0, 0));
            CvInvoke.Line(input, new Point(_corners[3].x, _corners[3].y), new Point(_corners[2].x, _corners[2].y), new Emgu.CV.Structure.MCvScalar(255, 0, 0));
            return input;
        }
    }
}
