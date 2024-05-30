using TMPro;
using UnityEngine;

namespace FireAimScripts
{
    public class FireSplitter : MonoBehaviour
    {
        [SerializeField] private float timeToSplit;
        [SerializeField] private GameObject complexClone;

        [SerializeField] private TextMeshProUGUI timerText;

        private bool _isSplitted;
        private float _timer;
        
        public delegate void DecreaseLives();
        public event DecreaseLives OnFireSplitted;

        private void Start()
        {
            _timer = timeToSplit;
            if(complexClone == null)
                timerText.gameObject.SetActive(false);
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            timerText.text = ((int)_timer).ToString();

            if (_timer <= 0 && !_isSplitted)
            {
                _isSplitted = true;
                timerText.gameObject.SetActive(false);
                _timer = timeToSplit;
                SplitFire();
            }
        }

        private void SplitFire()
        {
            if (!complexClone.activeSelf && complexClone != null)
            {
                OnFireSplitted?.Invoke();
                complexClone.SetActive(true);
            }
        }
    }
}
