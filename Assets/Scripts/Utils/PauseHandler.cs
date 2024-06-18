using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class PauseHandler : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject pauseButton;

        private void Start()
        {
            GameManager.Instance.PauseButton = pauseButton;
        }

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
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
