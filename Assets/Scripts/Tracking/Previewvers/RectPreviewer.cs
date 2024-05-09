using Assets.Scripts.Tracking.CamFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tracking.Previewvers
{
    public class RectPreviewer : Previewer
    {
        DrawRect _rectFilter;

        private Vector2Int leftTop;
        private Vector2Int leftBottom;
        private Vector2Int rightTop;
        private Vector2Int rightBottom;

        #region corners setup
        public Vector2Int LeftTop
        {
            get { return leftTop; }
            set
            {
                leftTop = value;
                _rectFilter.UpdateCorners(leftTop, leftBottom, rightTop, rightBottom);
            }
        }

        public Vector2Int LeftBottom
        {
            get { return leftBottom; }
            set
            {
                leftBottom = value;
                _rectFilter.UpdateCorners(leftTop, leftBottom, rightTop, rightBottom);
            }
        }

        public Vector2Int RightTop
        {
            get { return rightTop; }
            set
            {
                rightTop = value;
                _rectFilter.UpdateCorners(leftTop, leftBottom, rightTop, rightBottom);
            }
        }

        public Vector2Int RightBottom
        {
            get { return rightBottom; }
            set
            {
                rightBottom = value;
                _rectFilter.UpdateCorners(leftTop, leftBottom, rightTop, rightBottom);
            }
        }

        #endregion

        private void Awake()
        {
            _rectFilter = new DrawRect();
            _filters = new ICamFilter[]
                {
                 new BgrToRgb(),
                 _rectFilter
                };
        }
    }
}
