using UnityEngine;

namespace Instructions
{
    public class ActionCompleteHandler : MonoBehaviour
    {
        public delegate void CompleteAction();

        [SerializeField] protected bool keyControlled;

        protected bool IsKeyPressed;
        //private SerialPort _com;


        private void Start()
        {
            /*_com = new SerialPort(GameManager.Instance.comPort, 9600);
            _com.Open();
            _com.ReadTimeout = 1;*/
        }


        private void Update()
        {
            if (keyControlled)
                if (Input.GetKeyDown(KeyCode.RightShift) && !IsKeyPressed)
                {
                    IsKeyPressed = true;
                    InvokeCompleteActionEvent();
                }

            /*int fromCom = int.MaxValue;
            int comExpected = 2;
            if (_com.IsOpen)
            {
                try
                {
                    fromCom = _com.ReadByte();
                    Debug.Log("Serial:" + fromCom);
                }
                catch (System.Exception e)
                {
                    // Обработка исключения при чтении
                }
            }

            if(fromCom == comExpected)
                OnActionComplete?.Invoke();*/
        }

        public event CompleteAction OnActionComplete;

        protected void InvokeCompleteActionEvent()
        {
            OnActionComplete?.Invoke();
        }

        /*private void OnDisable()
        {
            _com.Close();
            _com.Dispose();
        }*/
    }
}