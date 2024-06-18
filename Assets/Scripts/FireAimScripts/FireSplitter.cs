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
        private bool _isTimerIncreased;
        private Vector3 _startLocalScale;
        private Vector3 _endLocalScale;
        
        public delegate void DecreaseLives();
        public event DecreaseLives OnFireSplitted;

        private void Start()
        {
            _timer = Random.Range(minTimeToSplit, maxTimeToSplit + 1.0f);
            _startLocalScale = new Vector3(1.0f, 1.0f, 1.0f);
            _endLocalScale = new Vector3(3.0f, 3.0f, 3.0f);
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            timerText.text = ((int)_timer).ToString();

            if (_timer < 3.0f && !_isTimerIncreased)
            {
                timerText.rectTransform.localScale = Vector3.Lerp(_startLocalScale, _endLocalScale, 2.0f);
                _isTimerIncreased = true;
            }

            if (_timer <= 0)
            {
                _timer = minTimeToSplit;
                SplitFire();
            }
        }

        private void SplitFire()
        {
            timerText.rectTransform.localScale = _startLocalScale;
            _isTimerIncreased = false;
            OnFireSplitted?.Invoke();
        }
    }
}
