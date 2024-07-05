using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//This script shows splash images before the game starts in coroutine Fade, using values set for each splash image in
//inspector using structure Slide
namespace Scenes
{
    public class SplashingImagesAfterStartShower : MonoBehaviour
    {
        public delegate void ShowAfterLogos();

        [SerializeField] private Slide[] slides;
        [SerializeField] private GameObject mainMenu;
        private float _currentFadeTime;

        private float _currentImageTime;
        private bool _fading;


        private void Start()
        {
            if (GameManager.Instance.firstRun)
            {
                StartCoroutine(Fade(0));
                GameManager.Instance.firstRun = false;
            }
            else
            {
                mainMenu.SetActive(true);
                gameObject.SetActive(false);
            }
        }

        public event ShowAfterLogos OnLogosFinish;

        private IEnumerator Fade(int index)
        {
            while (index < slides.Length)
            {
                _currentFadeTime = 0;
                _currentImageTime = 0;
                while (_currentImageTime < slides[index].seconds)
                {
                    _currentImageTime += Time.deltaTime;
                    yield return null;
                }

                var currentFade = 0f;

                while (_currentFadeTime < slides[index].fadeTime)
                {
                    currentFade = _currentFadeTime / slides[index].fadeTime;
                    _currentFadeTime += Time.deltaTime;
                    var imColor = slides[index].image.color;
                    var tarColor = new Color(imColor.r, imColor.g, imColor.b, 1f - currentFade);
                    slides[index].image.color = tarColor;
                    yield return null;
                }

                slides[index].image.gameObject.SetActive(false);
                index++;
            }

            OnLogosFinish?.Invoke();
            mainMenu.SetActive(true);
        }

        [Serializable]
        public struct Slide
        {
            public Image image;
            public float seconds;
            public float fadeTime;
        }
    }
}