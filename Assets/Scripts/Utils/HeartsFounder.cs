using UnityEngine;

namespace Utils
{
    public class HeartsFounder : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance.Hearts = gameObject;
            gameObject.SetActive(false);
        }
    }
}