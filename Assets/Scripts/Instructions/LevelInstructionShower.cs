using Scenes;
using UnityEngine;
using UnityEngine.UI;

namespace Instructions
{
    public class LevelInstructionShower : MonoBehaviour
    {
        [SerializeField] private GameObject levelInstruction;
        [SerializeField] private Button startButton;

        private void Start()
        {
            startButton.onClick.AddListener(OnStartButtonClick);
        }

        public void ShowInstructionWindow()
        {
            levelInstruction.SetActive(true);
        }

        private void OnStartButtonClick()
        {
            var can = GameObject.Find("TrashCan");
            TrashCan trashCan = can.GetComponent<TrashCan>();
            if (!trashCan.falled)
                trashCan.TrashFall();
            levelInstruction.SetActive(false);
        }
    }
}
