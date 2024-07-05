using Assets.Scripts.Tracking.CamFilters;
using UnityEngine;

namespace Assets.Scripts.Tracking.Previewvers
{
    public class RectPreviewer : Previewer
    {
        private DrawRect _rectFilter;
        private Vector2Int leftBottom;

        private Vector2Int leftTop;
        private Vector2Int rightBottom;
        private Vector2Int rightTop;

        private void Awake()
        {
            _rectFilter = new DrawRect();
            _filters = new ICamFilter[]
            {
                new BgrToRgb(),
                _rectFilter
            };
        }

        #region corners setup

        public Vector2Int LeftTop
        {
            get => leftTop;
            set
            {
                leftTop = value;
                _rectFilter.UpdateCorners(leftTop, leftBottom, rightTop, rightBottom);
            }
        }

        public Vector2Int LeftBottom
        {
            get => leftBottom;
            set
            {
                leftBottom = value;
                _rectFilter.UpdateCorners(leftTop, leftBottom, rightTop, rightBottom);
            }
        }

        public Vector2Int RightTop
        {
            get => rightTop;
            set
            {
                rightTop = value;
                _rectFilter.UpdateCorners(leftTop, leftBottom, rightTop, rightBottom);
            }
        }

        public Vector2Int RightBottom
        {
            get => rightBottom;
            set
            {
                rightBottom = value;
                _rectFilter.UpdateCorners(leftTop, leftBottom, rightTop, rightBottom);
            }
        }

        #endregion
    }
}