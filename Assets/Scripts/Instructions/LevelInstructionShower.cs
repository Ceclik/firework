using UnityEngine;
using UnityEngine.UI;

namespace Instructions
{
    public class LevelInstructionShower : MonoBehaviour
    {
        [SerializeField] private GameObject levelInstruction;
        [SerializeField] private Button startButton;

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
            Debug.LogError("in button click");
            levelInstruction.SetActive(false);
            OnStartButtonClicked?.Invoke();
        }
    }
}
