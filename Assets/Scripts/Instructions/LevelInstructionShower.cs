using UnityEngine;
using UnityEngine.UI;

namespace Instructions
{
    public class LevelInstructionShower : MonoBehaviour
    {
        [SerializeField] private GameObject levelInstruction;
        [SerializeField] private Button startButton;
        [Space(10)][Header("For aim level")]
        [SerializeField] private bool isForAimLevel;
        [SerializeField] private GameObject buttonSlide;
        

        public delegate void StartFire();
        public event StartFire OnStartButtonClicked;

        private void Start()
        {
            startButton.onClick.AddListener(OnStartButtonClick);
        }

        public void ShowInstructionWindow()
        {
            levelInstruction.SetActive(true);
        }

        public void OnStartButtonClick()
        {
            if (!isForAimLevel)
            {
                levelInstruction.SetActive(false);
            }
            else buttonSlide.SetActive(false);
            OnStartButtonClicked?.Invoke();
        }
    }
}
