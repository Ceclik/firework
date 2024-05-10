using System.Collections;
using UnityEngine;


namespace Scenes
{
    public class AfterLevelMenuDisplayer : MonoBehaviour {
        
        [SerializeField] private float fadeTime;
        [SerializeField] private GameObject winner;
        [SerializeField] private GameObject looser;
        
        private AudioSource _winnerSound;
        private AudioSource _looserSound;
        
        void Awake () 
        {
            var audios = GetComponents<AudioSource>();
            if (audios.Length > 1)
            {
                _winnerSound = audios[0];
                _looserSound = audios[1];
            }
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

            float fTime = 0;
            while (fTime<fadeTime) // Fade in star
            {            
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
