#pragma warning disable 0649
using System;
using UnityEngine;

public class VisualizeRectIntersectionWithRayFromCenter : MonoBehaviour
{
    [SerializeField] private Rect rect;
    [SerializeField] private Vector2 point;
    [SerializeField] private Colors colors;

    private void OnDrawGizmos()
    {
        Gizmos.color = colors.rect;
        Vector2[] corners =
        {
            new(rect.xMin, rect.yMin), new(rect.xMin, rect.yMax),
            rect.Max(), new(rect.xMax, rect.yMin)
        };
        var i = 0;
        while (i < 3) Gizmos.DrawLine(corners[i], corners[++i]);
        Gizmos.DrawLine(corners[3], corners[0]);

        Gizmos.color = colors.point;
        Gizmos.DrawLine(rect.center, point);

        Gizmos.color = colors.intersection;
        Gizmos.DrawLine(rect.center, rect.IntersectionWithRayFromCenter(point));
    }

    [Serializable]
    private class Colors
    {
        public Color rect, point, intersection;
    }
}