using System.Collections;
using Tracking;
using UnityEngine;

namespace Instructions
{
    public class SlidesSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject[] instructionSlides;
        [SerializeField] private ActionCompleteHandler[] completeHandlers;

        [Space(10)] [SerializeField] private float switchDelay;
        [SerializeField] private float startAfterNotActiveTime;

        [Space(20)] [SerializeField] private GameObject mainMenu;

        private int _index = 0;
        private float _waitingTime;

        private Vector3 _currentPointerPosition;

        private bool _isShowingSlides;

        private void Start()
        {
            _currentPointerPosition = MyInput.Instance.CursorImage.transform.position;
            foreach (var handler in completeHandlers)
                handler.OnActionComplete += SwitchSlide;
        }

        private void OnDestroy()
        {
            foreach (var handler in completeHandlers)
                handler.OnActionComplete -= SwitchSlide;
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
            if (_index == instructionSlides.Length)
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
            if(mainMenu.activeSelf && _currentPointerPosition != MyInput.Instance.CursorImage.transform.position){
                _currentPointerPosition = MyInput.Instance.CursorImage.transform.position;
                _waitingTime = 0;
            }
            if (mainMenu.activeSelf && _currentPointerPosition == MyInput.Instance.CursorImage.transform.position)
            {
                _waitingTime += Time.deltaTime;
            }

            if (_waitingTime >= startAfterNotActiveTime && !_isShowingSlides)
            {
                _isShowingSlides = true;
                mainMenu.SetActive(false);
                instructionSlides[_index].SetActive(true);
            }
        }
    }
}
