using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//In this script stars are accured according to time spent by fire extinguishing. Required time get from game settings.
//Also here stars and empty places for them are shown with fading effect handled also here.

namespace Scenes
{
    public class Stars : MonoBehaviour {
        /*Image[] starsEmpty = new Image[3];
        public Image[] starsFull = new Image[3];
        public GameObject[] starsEffects = new GameObject[3];*/
        
        public float fadeTime;
        public GameObject winner;
        public GameObject looser;

        private CanvasGroup _ending;
        private AudioSource _winnerSound;
        private AudioSource _looserSound;
        // Use this for initialization
        void Awake () {
            _ending = GetComponent<CanvasGroup>();
            var audios = GetComponents<AudioSource>();
            if (audios.Length > 1)
            {
                _winnerSound = audios[0];
                _looserSound = audios[1];
            }
        }
	
        // Update is called once per frame
        void Update () {
		
        }

        private IEnumerator Fade(int star, int total)
        {
        
            Debug.Log("startring stars coroutine, STAR:"+star+" total:"+total);
            var ending = GetComponent<CanvasGroup>();

        

            if (star == 0) //Fade in canvas group
            {
                float sTime = 0;
                if (total>0)
                {
                    winner.SetActive(true);
                    looser.SetActive(false);
                }
                else
                {
                    winner.SetActive(false);
                    looser.SetActive(true);
                }
                while (sTime <= fadeTime*2f)
                {
                    ending.alpha = sTime / (fadeTime*2f);                
                    sTime = sTime + Time.deltaTime;
                    yield return null;
                }
            }

            ending.alpha = 1.0f;

            if (total < 1) yield break;

            /*starsEffects[star].SetActive(true);
            Debug.Log("activating star effect:" + star);
            starsFull[star].gameObject.SetActive(true);*/

            float fTime = 0;
            while (fTime<fadeTime) // Fade in star
            {            
                //starsFull[star].color = new Color(starsFull[star].color.r, starsFull[star].color.g, starsFull[star].color.b, fTime / fadeTime);
                fTime = fTime + Time.deltaTime;
                yield return null;
            }
            if (star<total-1)
            {
                yield return new WaitForSeconds(fadeTime*2f);
                StartCoroutine(Fade(star + 1, total));
            }        
        }

        public void Show(int stars)
        {
            StartCoroutine(Fade(0, stars));
            if (stars > 0)
            {
                Debug.Log("playing win sound");
                if (_winnerSound != null)
                {
                    _winnerSound.Play();
                    Debug.Log("winsound not null!!");
                }
            }
            else
            {
                Debug.Log("playing loose sound");
                if (_looserSound!=null) _looserSound.Play();
            }
        }
    }
}
