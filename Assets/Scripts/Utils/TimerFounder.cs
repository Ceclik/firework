using UnityEngine;

namespace Utils
{
    public class TimerFounder : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance.Timer = GetComponent<Timer>();
            gameObject.SetActive(false);
        }
    }
}