using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class PauseHandler : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;

        public void OnPauseMenuClick()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }

        public void OnResumeButtonClick()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }

        public void OnMenuButtonClick()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
