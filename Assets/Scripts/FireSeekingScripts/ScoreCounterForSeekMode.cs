using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FireSeekingScripts
{
    public class ScoreCounterForSeekMode : MonoBehaviour
    {
        [SerializeField] private float streakTimeMax;
        [SerializeField] private Text scoreText;
        private float _levelTime;
        public int MistakesAmount { get; set; }

        private void Update()
        {
            if (GameManager.Instance.IsFireStarted)
                _levelTime += Time.deltaTime;
        }

        public float CountScore()
        {
            UnityEngine.Debug.LogError($"level time: {_levelTime / 60}\nmistakes amount: {MistakesAmount}");
            var score = streakTimeMax / (_levelTime / 60 * MistakesAmount);
            score *= 100;
            scoreText.text =
                $"Количество очков: {(int)score}\nРекорд: {PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Higscore")}";
            if (score > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Higscore"))
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "Higscore", (int)score);
            return score;
        }
    }
}