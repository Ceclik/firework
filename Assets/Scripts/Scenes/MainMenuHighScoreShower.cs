using UnityEngine;
using UnityEngine.UI;

namespace Scenes
{
    public class MainMenuHighScoreShower : MonoBehaviour
    {
        [SerializeField] private Text homeTVText;
        [SerializeField] private Text homeGasText;
        [SerializeField] private Text schoolSceneText;

        [SerializeField] private Button homeFireAimTVButton;
        [SerializeField] private Button homeFireAimGasButton;
        [SerializeField] private Button schoolFireButton;

        private void Start()
        {
            if (GameManager.Instance.FireSeekGameMode)
            {
                schoolFireButton.gameObject.SetActive(true);
                schoolFireButton.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }

            if (GameManager.Instance.FireAimGameMode)
            {
                schoolFireButton.gameObject.SetActive(false);
                homeFireAimGasButton.gameObject.SetActive(true);
                homeFireAimTVButton.gameObject.SetActive(true);
            }
        }

        private void OnEnable()
        {
            if (GameManager.Instance.FireSeekGameMode)
            {
                schoolSceneText.text = $"Highscore: {PlayerPrefs.GetInt("SchoolFireSeekSceneHigscore")}";
            }
            else
            {
                homeTVText.text = $"Highscore: {PlayerPrefs.GetInt("HomeFireAimTVSceneHigscore")}";
                homeGasText.text = $"Highscore: {PlayerPrefs.GetInt("HomeFireAimGasSceneHigscore")}";
            }
        }
    }
}