using UnityEngine;
using UnityEngine.UI;

namespace Scenes
{
    public class MainMenuHighScoreShower : MonoBehaviour
    {
        [SerializeField] private Text homeSceneText;
        [SerializeField] private Text schoolSceneText;

        [SerializeField] private Button homeFireAimTVButton;
        [SerializeField] private Button homeFireAimGasButton;
        [SerializeField] private Button schoolFireButton;

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
    }
}
