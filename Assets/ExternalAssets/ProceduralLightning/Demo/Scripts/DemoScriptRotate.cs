using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
    public class DemoScriptRotate : MonoBehaviour
    {
        public Vector3 Rotation;

        private void Update()
        {
            gameObject.transform.Rotate(Rotation * Time.deltaTime);
        }
    }
}