using TMPro;
using UnityEngine;

namespace FireAimScripts
{
    public class FireSplitter : MonoBehaviour
    {
        [Space(10)][Header("Timer parameters")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private float timerIncreasingTime;
        [SerializeField] private float timerStartIncreasingValue;

        public float Timer { get; set; }
        private Vector3 _startLocalScale;
        private Vector3 _endLocalScale;
        private float _scaleTimer;
        
        public float FireStopSpeed { get; private set; }
        
        public delegate void DecreaseLives();
        public event DecreaseLives OnFireSplitted;

        private void Start()
        {
            _startLocalScale = new Vector3(1.0f, 1.0f, 1.0f);
            _endLocalScale = new Vector3(3.0f, 3.0f, 3.0f);
            FireStopSpeed = CountFireStopSpeed();
        }
        
        private float CountFireStopSpeed()  //this counting must be remade in universal 
        {
            if (Timer is >= 8.0f and < 9.0f) return 0.55f;
            if (Timer is >= 9.0f and < 10.0f) return 0.5f;
            if (Timer is >= 10.0f and < 11.0f) return 0.4f;
            if (Timer is >= 11.0f and < 12.0f) return 0.35f;
            if (Timer is >= 12.0f and < 13.0f) return 0.25f;
            if (Timer is >= 13.0f and < 14.0f) return 0.2f;
            if (Timer >= 14.0f) return 0.2f;
            return 0.2f;
        }

        private void Update()
        {
            Timer -= Time.deltaTime;
            timerText.text = ((int)Timer).ToString();

            if (Timer <= timerStartIncreasingValue)
            {
                _scaleTimer += Time.deltaTime;
                timerText.rectTransform.localScale =
                    Vector3.Lerp(_startLocalScale, _endLocalScale, _scaleTimer / timerIncreasingTime);
            }

            if (Timer <= 0)
            {
                //Timer = MinTimeToSplit;
                _scaleTimer = 0;
                SplitFire();
            }
        }

        private void SplitFire()
        {
            timerText.rectTransform.localScale = _startLocalScale;
            OnFireSplitted?.Invoke();
        }
    }
}
