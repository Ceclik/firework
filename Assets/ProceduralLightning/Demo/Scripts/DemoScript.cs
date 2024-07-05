//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#define UNITY_4

#endif

using UnityEngine;
using UnityEngine.SceneManagement;

namespace DigitalRuby.ThunderAndLightning
{
    public class DemoScript : MonoBehaviour
    {
        private const float fastCloudSpeed = 50.0f;

        private static readonly GUIStyle style = new();
        public ThunderAndLightningScript ThunderAndLightningScript;
        public LightningBoltScript LightningBoltScript;
        public ParticleSystem CloudParticleSystem;
        public float MoveSpeed = 250.0f;
        public bool EnableMouseLook = true;
        private readonly RotationAxes axes = RotationAxes.MouseXAndY;

        private float deltaTime;
        private float fpsIncrement;
        private string fpsText;
        private readonly float maximumX = 360F;
        private readonly float maximumY = 60F;
        private readonly float minimumX = -360F;
        private readonly float minimumY = -60F;
        private Quaternion originalRotation;
        private float rotationX;
        private float rotationY;
        private readonly float sensitivityX = 15F;
        private readonly float sensitivityY = 15F;

        private void Start()
        {
            originalRotation = transform.localRotation;

            if (CloudParticleSystem != null)
            {
                var m = CloudParticleSystem.main;
                m.simulationSpeed = fastCloudSpeed;
            }
        }

        private void Update()
        {
            UpdateThunder();
            UpdateMovement();
            UpdateMouseLook();
            UpdateQuality();
            UpdateOther();
        }

        private void OnGUI()
        {
            int w = Screen.width, h = Screen.height;
            var rect = new Rect(10, h - 50, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 50;
            style.normal.textColor = Color.white;

            if ((fpsIncrement += Time.deltaTime) > 1.0f)
            {
                fpsIncrement -= 1.0f;
                var msec = deltaTime * 1000.0f;
                var fps = 1.0f / deltaTime;
                fpsText = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            }

            GUI.Label(rect, fpsText, style);
        }

        private void UpdateThunder()
        {
            if (ThunderAndLightningScript != null)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    ThunderAndLightningScript.CallNormalLightning();
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                    ThunderAndLightningScript.CallIntenseLightning();
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                    if (CloudParticleSystem != null)
                    {
                        var m = CloudParticleSystem.main;
                        m.simulationSpeed = m.simulationSpeed == 1.0f ? fastCloudSpeed : 1.0f;
                    }
            }
        }

        private void UpdateMovement()
        {
            var speed = MoveSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.W)) Camera.main.transform.Translate(0.0f, 0.0f, speed);
            if (Input.GetKey(KeyCode.S)) Camera.main.transform.Translate(0.0f, 0.0f, -speed);
            if (Input.GetKey(KeyCode.A)) Camera.main.transform.Translate(-speed, 0.0f, 0.0f);
            if (Input.GetKey(KeyCode.D)) Camera.main.transform.Translate(speed, 0.0f, 0.0f);
        }

        private void UpdateMouseLook()
        {
            if (!EnableMouseLook) return;

            if (axes == RotationAxes.MouseXAndY)
            {
                // Read the mouse input axis
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

                rotationX = ClampAngle(rotationX, minimumX, maximumX);
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                var xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                var yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

                transform.localRotation = originalRotation * xQuaternion * yQuaternion;
            }
            else if (axes == RotationAxes.MouseX)
            {
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationX = ClampAngle(rotationX, minimumX, maximumX);

                var xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                transform.localRotation = originalRotation * xQuaternion;
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                var yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
                transform.localRotation = originalRotation * yQuaternion;
            }
        }

        private void UpdateQuality()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                QualitySettings.SetQualityLevel(0);
            else if (Input.GetKeyDown(KeyCode.F2))
                QualitySettings.SetQualityLevel(1);
            else if (Input.GetKeyDown(KeyCode.F3))
                QualitySettings.SetQualityLevel(2);
            else if (Input.GetKeyDown(KeyCode.F4))
                QualitySettings.SetQualityLevel(3);
            else if (Input.GetKeyDown(KeyCode.F5))
                QualitySettings.SetQualityLevel(4);
            else if (Input.GetKeyDown(KeyCode.F6)) QualitySettings.SetQualityLevel(5);
        }

        private void UpdateOther()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

            if (Input.GetKeyDown(KeyCode.Escape)) ReloadCurrentScene();
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F) angle += 360F;
            if (angle > 360F) angle -= 360F;

            return Mathf.Clamp(angle, min, max);
        }

        public static void ReloadCurrentScene()
        {
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
            UnityEngine.Application.LoadLevel(0);

#else

            SceneManager.LoadScene(0, LoadSceneMode.Single);

#endif
        }

        private enum RotationAxes
        {
            MouseXAndY = 0,
            MouseX = 1,
            MouseY = 2
        }
    }
}