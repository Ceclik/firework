using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.EventSystems;


namespace ExternalAssets.Standard_Assets.CrossPlatformInput.Scripts
{
    [ExecuteInEditMode]
    public class MobileControlRig : MonoBehaviour
#if UNITY_EDITOR
        , IActiveBuildTargetChanged
#endif
    {
        // this script enables or disables the child objects of a control rig
        // depending on whether the USE_MOBILE_INPUT define is declared.

        // This define is set or unset by a menu item that is included with
        // the Cross Platform Input package.


#if !UNITY_EDITOR
	void OnEnable()
	{
		CheckEnableControlRig();
	}
#else
        public int callbackOrder => 1;
#endif

        private void Start()
        {
#if UNITY_EDITOR
            if (Application
                .isPlaying) //if in the editor, need to check if we are playing, as start is also called just after exiting play
#endif
            {
                var system = FindObjectOfType<EventSystem>();

                if (system == null)
                {
                    //the scene have no event system, spawn one
                    var o = new GameObject("EventSystem");

                    o.AddComponent<EventSystem>();
                    o.AddComponent<StandaloneInputModule>();
                }
            }
        }

#if UNITY_EDITOR

        private void OnEnable()
        {
            EditorApplication.update += Update;
        }


        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }


        private void Update()
        {
            CheckEnableControlRig();
        }
#endif


        private void CheckEnableControlRig()
        {
#if MOBILE_INPUT
		EnableControlRig(true);
#else
            EnableControlRig(false);
#endif
        }


        private void EnableControlRig(bool enabled)
        {
            foreach (Transform t in transform) t.gameObject.SetActive(enabled);
        }

#if UNITY_EDITOR
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            CheckEnableControlRig();
        }
#endif
    }
}