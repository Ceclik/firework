//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
    public class LightningFieldScript : LightningBoltPrefabScriptBase
    {
        [Header("Lightning Field Properties")] [Tooltip("The minimum length for a field segment")]
        public float MinimumLength = 0.01f;

        [Tooltip("The bounds to put the field in.")]
        public Bounds FieldBounds;

        [Tooltip("Optional light for the lightning field to emit")]
        public Light Light;

        private float lightGrow;
        private float minimumLengthSquared;

        protected override void Start()
        {
            base.Start();

            if (Light != null)
            {
                Light.enabled = false;
                Light.intensity = 0;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (Light != null)
            {
                Light.transform.position = FieldBounds.center;
                if (lightGrow < 3.2f)
                {
                    lightGrow += Time.deltaTime * 2f;
                    Light.intensity = lightGrow;
                }
                else
                {
                    Light.intensity = Random.Range(2.8f, 3.2f);
                }
            }
        }

        private void OnEnable()
        {
            lightGrow = 0;
        }

        private Vector3 RandomPointInBounds()
        {
            var x = Random.Range(FieldBounds.min.x, FieldBounds.max.x);
            var y = Random.Range(FieldBounds.min.y, FieldBounds.max.y);
            var z = Random.Range(FieldBounds.min.z, FieldBounds.max.z);

            return new Vector3(x, y, z);
        }

        public override void CreateLightningBolt(LightningBoltParameters parameters)
        {
            minimumLengthSquared = MinimumLength * MinimumLength;

            for (var i = 0; i < 16; i++)
            {
                // get two random points in the bounds
                parameters.Start = RandomPointInBounds();
                parameters.End = RandomPointInBounds();
                if ((parameters.End - parameters.Start).sqrMagnitude >= minimumLengthSquared) break;
            }

            if (Light != null) Light.enabled = true;

            base.CreateLightningBolt(parameters);
        }
    }
}