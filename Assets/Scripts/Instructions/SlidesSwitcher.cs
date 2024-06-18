using System.Collections;
using Scenes;
using Tracking;
using UnityEngine;

namespace Instructions
{
    public class SlidesSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject[] instructionSlides;
        [SerializeField] private ActionCompleteHandler[] completeHandlers;

        [Space(10)] [SerializeField] private float switchDelay;
        

        [Space(20)] [SerializeField] private bool forMainMenu;
        [SerializeField] private float startAfterNotActiveTime;
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private SplashingImagesAfterStartShower logosShower;

        private int _index = 0;
        private float _waitingTime;

        private Vector3 _currentPointerPosition;

        private bool _isShowingSlides;

        private void Start()
        {
            _currentPointerPosition = MyInput.Instance.CursorImage.transform.position;
            foreach (var handler in completeHandlers)
                handler.OnActionComplete += SwitchSlide;

            if(forMainMenu)
                logosShower.OnLogosFinish += StartShowingSlides;
        }

        private void OnDestroy()
        {
            foreach (var handler in completeHandlers)
                handler.OnActionComplete -= SwitchSlide;
            
            if(forMainMenu)
                logosShower.OnLogosFinish -= StartShowingSlides;
        }

        private void SwitchSlide()
        {
            StartCoroutine(SlideSwitcher());
            _waitingTime = 0;
        }

        private IEnumerator SlideSwitcher()
        {
            yield return new WaitForSeconds(switchDelay);
            instructionSlides[_index].SetActive(false);
            _index++;
            if (_index == instructionSlides.Length && forMainMenu)
            {
                _isShowingSlides = false;
                _index = 0;
                mainMenu.SetActive(true);
            }
            else
                instructionSlides[_index].SetActive(true);
        }

        private void Update()
        {
            if (forMainMenu && mainMenu.activeSelf &&
                _currentPointerPosition != MyInput.Instance.CursorImage.transform.position)
            {
                _currentPointerPosition = MyInput.Instance.CursorImage.transform.position;
                _waitingTime = 0;
            }

            if (forMainMenu && mainMenu.activeSelf &&
                _currentPointerPosition == MyInput.Instance.CursorImage.transform.position)
            {
                _waitingTime += Time.deltaTime;
            }

            if (_waitingTime >= startAfterNotActiveTime && !_isShowingSlides)
            {
                StartShowingSlides();
            }
        }

        private void StartShowingSlides()
        {
            if(forMainMenu)
                mainMenu.SetActive(false);
            _isShowingSlides = true;
            instructionSlides[_index].SetActive(true);
        }
    }
}
