#pragma warning disable 0649
using System;
using UnityEngine;

public class VisualizeRectIntersectionWithRayFromCenter : MonoBehaviour {

[SerializeField] Rect rect;
[SerializeField] Vector2 point;
	
[Serializable] class Colors {
	public Color rect, point, intersection;
} [SerializeField] Colors colors;
	
void OnDrawGizmos() {
	Gizmos.color = colors.rect;
	Vector2[] corners = {new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMin, rect.yMax),
		rect.Max(), new Vector2(rect.xMax, rect.yMin)};
	int i = 0;
	while (i < 3) Gizmos.DrawLine(corners[i], corners[++i]);
	Gizmos.DrawLine(corners[3], corners[0]);
	
	Gizmos.color = colors.point;
	Gizmos.DrawLine(rect.center, point);
		
	Gizmos.color = colors.intersection;
	Gizmos.DrawLine(rect.center, rect.IntersectionWithRayFromCenter(pointOnRay: point));
}
	
}