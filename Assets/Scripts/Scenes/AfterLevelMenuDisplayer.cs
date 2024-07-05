using UnityEngine;

namespace Scenes
{
    public class AfterLevelMenuDisplayer : MonoBehaviour
    {
        [SerializeField] private float fadeTime;
        [SerializeField] private GameObject winner;
        [SerializeField] private GameObject looser;
        private CanvasGroup _canvasGroup;

        private bool _isFading;
        private AudioSource _looserSound;

        private AudioSource _winnerSound;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            var audios = GetComponents<AudioSource>();
            if (audios.Length > 1)
            {
                _winnerSound = audios[0];
                _looserSound = audios[1];
            }
        }

        private void Update()
        {
            if (_isFading && _canvasGroup.alpha < 1) _canvasGroup.alpha += fadeTime * Time.deltaTime;
        }

        public void Show(bool isWin)
        {
            _isFading = true;
            if (isWin)
            {
                winner.gameObject.SetActive(true);
                looser.gameObject.SetActive(false);
                _winnerSound.Play();
            }
            else
            {
                looser.gameObject.SetActive(true);
                winner.gameObject.SetActive(false);
                _looserSound.Play();
            }
        }
    }
}