﻿//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEditor;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
    public class LightningBoltShapeConeScript : LightningBoltPrefabScriptBase
    {
        [Header("Lightning Cone Properties")] [Tooltip("Radius at base of cone where lightning can emit from")]
        public float InnerRadius = 0.1f;

        [Tooltip("Radius at outer part of the cone where lightning emits to")]
        public float OuterRadius = 4.0f;

        [Tooltip("The length of the cone from the center of the inner and outer circle")]
        public float Length = 4.0f;

#if UNITY_EDITOR

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Handles.DrawWireDisc(transform.position, transform.forward, InnerRadius);
            Handles.DrawWireDisc(transform.position + transform.forward * Length, transform.forward, OuterRadius);

            Handles.DrawLine(transform.position + transform.rotation * Vector3.right * InnerRadius,
                transform.position + transform.rotation * Vector3.right * OuterRadius + transform.forward * Length);

            Handles.DrawLine(transform.position + transform.rotation * -Vector3.right * InnerRadius,
                transform.position + transform.rotation * -Vector3.right * OuterRadius + transform.forward * Length);

            Handles.DrawLine(transform.position + transform.rotation * Vector3.up * InnerRadius,
                transform.position + transform.rotation * Vector3.up * OuterRadius + transform.forward * Length);

            Handles.DrawLine(transform.position + transform.rotation * -Vector3.up * InnerRadius,
                transform.position + transform.rotation * -Vector3.up * OuterRadius + transform.forward * Length);
        }

#endif

        public override void CreateLightningBolt(LightningBoltParameters parameters)
        {
            var circle1 = Random.insideUnitCircle * InnerRadius;
            var start = transform.rotation * new Vector3(circle1.x, circle1.y, 0.0f);
            var circle2 = Random.insideUnitCircle * OuterRadius;
            var end = transform.rotation * new Vector3(circle2.x, circle2.y, 0.0f) + transform.forward * Length;

            parameters.Start = start;
            parameters.End = end;

            base.CreateLightningBolt(parameters);
        }
    }
}