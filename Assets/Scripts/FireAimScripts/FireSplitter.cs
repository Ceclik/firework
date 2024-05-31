using TMPro;
using UnityEngine;

namespace FireAimScripts
{
    public class FireSplitter : MonoBehaviour
    {
        [SerializeField] private float timeToSplit;

        [SerializeField] private TextMeshProUGUI timerText;
        
        private float _timer;
        
        public delegate void DecreaseLives();
        public event DecreaseLives OnFireSplitted;

        private void Start()
        {
            _timer = timeToSplit;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            timerText.text = ((int)_timer).ToString();

            if (_timer <= 0)
            {
                _timer = timeToSplit;
                SplitFire();
            }
        }

        private void SplitFire()
        {
            OnFireSplitted?.Invoke();
        }
    }
}
