using UnityEngine;

namespace ExternalAssets.Standard_Assets.CrossPlatformInput.Scripts
{
    public class ButtonHandler : MonoBehaviour
    {
        public string Name;

        public void Update()
        {
        }

        private void OnEnable()
        {
        }

        public void SetDownState()
        {
            CrossPlatformInputManager.SetButtonDown(Name);
        }


        public void SetUpState()
        {
            CrossPlatformInputManager.SetButtonUp(Name);
        }


        public void SetAxisPositiveState()
        {
            CrossPlatformInputManager.SetAxisPositive(Name);
        }


        public void SetAxisNeutralState()
        {
            CrossPlatformInputManager.SetAxisZero(Name);
        }


        public void SetAxisNegativeState()
        {
            CrossPlatformInputManager.SetAxisNegative(Name);
        }
    }
}