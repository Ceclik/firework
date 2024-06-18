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
        
        public float FireStopSpeed { get; private set; }
        
        public delegate void DecreaseLives();
        public event DecreaseLives OnFireSplitted;

        private void Start()
        {
            _timer = Random.Range(minTimeToSplit, maxTimeToSplit + 1.0f);
            _startLocalScale = new Vector3(1.0f, 1.0f, 1.0f);
            _endLocalScale = new Vector3(3.0f, 3.0f, 3.0f);
            FireStopSpeed = CountFireStopSpeed();
        }
        
        private float CountFireStopSpeed()  //this counting must be remade in universal 
        {
            if (_timer is >= 8.0f and < 9.0f) return 0.7f;
            if (_timer is >= 9.0f and < 10.0f) return 0.65f;
            if (_timer is >= 10.0f and < 11.0f) return 0.55f;
            if (_timer is >= 11.0f and < 12.0f) return 0.45f;
            if (_timer is >= 12.0f and < 13.0f) return 0.4f;
            if (_timer is >= 13.0f and < 14.0f) return 0.35f;
            if (_timer >= 14.0f) return 0.3f;
            return 0.3f;
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
