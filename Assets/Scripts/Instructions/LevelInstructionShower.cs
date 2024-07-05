using UnityEngine;
using UnityEngine.UI;

namespace Instructions
{
    public class LevelInstructionShower : MonoBehaviour
    {
        public delegate void StartFire();

        [SerializeField] private GameObject levelInstruction;
        [SerializeField] private Button startButton;

        [Space(10)] [Header("For aim level")] [SerializeField]
        private bool isForAimLevel;

        [SerializeField] private GameObject buttonSlide;

        private void Start()
        {
            startButton.onClick.AddListener(OnStartButtonClick);
        }

        public event StartFire OnStartButtonClicked;

        public void ShowInstructionWindow()
        {
            levelInstruction.SetActive(true);
        }

        public void OnStartButtonClick()
        {
            if (!isForAimLevel)
                levelInstruction.SetActive(false);
            else buttonSlide.SetActive(false);
            OnStartButtonClicked?.Invoke();
        }
    }
}