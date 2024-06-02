using TMPro;
using UnityEngine;

namespace FireAimScripts
{
    public class FireSplitter : MonoBehaviour
    {
        [SerializeField] private float minTimeToSplit;
        [SerializeField] private float maxTimeToSplit;

        [SerializeField] private TextMeshProUGUI timerText;
        
        private float _timer;
        
        public delegate void DecreaseLives();
        public event DecreaseLives OnFireSplitted;

        private void Start()
        {
            _timer = Random.Range(minTimeToSplit, maxTimeToSplit + 1.0f);
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            timerText.text = ((int)_timer).ToString();

            if (_timer <= 0)
            {
                _timer = minTimeToSplit;
                SplitFire();
            }
        }

        private void SplitFire()
        {
            OnFireSplitted?.Invoke();
        }
    }
}
