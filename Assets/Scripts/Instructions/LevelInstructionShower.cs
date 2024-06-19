using UnityEngine;
using UnityEngine.UI;

namespace Instructions
{
    public class LevelInstructionShower : MonoBehaviour
    {
        [SerializeField] private GameObject levelInstruction;
        [SerializeField] private Button startButton;
        [SerializeField] private bool isForAimLevel;

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
            if(!isForAimLevel)
                levelInstruction.SetActive(false);
            OnStartButtonClicked?.Invoke();
        }
    }
}
