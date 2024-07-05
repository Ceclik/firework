using UnityEngine;
using UnityEngine.UI;

//This script use to handle the scale of fire progress and to move pointer on that scale in UI. 

namespace Scenes
{
    public class FireMeter : MonoBehaviour //The name of the script should be given according to the action this script
    //is handling. Example for this script: FireScaleHandler
    {
        public Image pile; //fields should be private
        public Image fire;
        public Image ok;
        public Timer timer;

        public float pileMin = 19.3f;
        public float pileMax = 360.1f;
        public float percent = 1f;
        public float fadeTime = 2f;

        private bool _active;
        private CanvasGroup _canvasGroup;
        private float _fadeTimer; //initialization of such fields should be in Start() or Awake() methods

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            if (_active && _fadeTimer < fadeTime)
            {
                _fadeTimer += Time.deltaTime;
                _canvasGroup.alpha = _fadeTimer / fadeTime;
            }

            pile.transform.position = new Vector2(pile.rectTransform.position.x, pileMin + pileMax * (1f - percent));
        }

        public void Show()
        {
            _active = true;
            timer.gameObject.SetActive(true);
        }
    }
}