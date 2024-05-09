using UnityEngine;

namespace Instructions
{
    public class ActionCompleteHandler : MonoBehaviour
    {
        public delegate void CompleteAction();

        public event CompleteAction OnActionComplete;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.RightShift))
                OnActionComplete?.Invoke();
        }
    }
}
