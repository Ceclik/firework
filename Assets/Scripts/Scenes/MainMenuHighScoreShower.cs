using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes
{
    public class MainMenuHighScoreShower : MonoBehaviour
    {
        [SerializeField] private Text homeSceneText;
        [SerializeField] private Text schoolSceneText;

        [SerializeField] private Button homeFireButton;

        private void OnEnable()
        {
            if (GameManager.Instance.FireSeekGameMode)
            {
                homeSceneText.text = $"Highscore: {PlayerPrefs.GetInt("HomeFireSeekSceneHigscore")}";
                schoolSceneText.text = $"Highscore: {PlayerPrefs.GetInt("SchoolFireSeekSceneHigscore")}";
            }
            else
            {
                homeSceneText.text = $"Highscore: {PlayerPrefs.GetInt("HomeFireAimSceneHigscore")}";
                schoolSceneText.text = $"Highscore: {PlayerPrefs.GetInt("SchoolFireAimHigscore")}";
            }
        }

        private void Start()
        {
            if (GameManager.Instance.FireSeekGameMode)
                homeFireButton.interactable = false;
        }
    }
}
