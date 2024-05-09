using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//This script shows splash images before the game starts in coroutine Fade, using values seted for each splash image in
//inspector using structure Slide
public class Slideshow : MonoBehaviour
{

    [SerializeField] private Slide[] slides;
    [SerializeField] private GameObject mainMenu;

    [System.Serializable]
    public struct Slide
    {
        public Image image;
        public float seconds;
        public float fadeTime;
    }

    private float _currentImageTime;
    private float _currentFadeTime;
    private bool _fading;

    
    private void Start()
    {
        if (GameManager.Instance.FirstRun)
        {
            StartCoroutine(Fade(0));
            GameManager.Instance.FirstRun = false;
        }
        else
        {
            mainMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }
    
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

            float currentFade = 0f;

            while (_currentFadeTime < slides[index].fadeTime)
            {
                currentFade = _currentFadeTime / slides[index].fadeTime;
                _currentFadeTime += Time.deltaTime;
                Color imColor = slides[index].image.color;
                Color tarColor = new Color(imColor.r, imColor.g, imColor.b, 1f - currentFade);
                slides[index].image.color = tarColor;
                yield return null;
            }
            slides[index].image.gameObject.SetActive(false);
            index++;
        }
        mainMenu.SetActive(true);
    }
}
