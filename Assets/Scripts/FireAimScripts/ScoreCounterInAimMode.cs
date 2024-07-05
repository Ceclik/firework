using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FireAimScripts
{
    public class ScoreCounterInAimMode : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        public int AmountOfA { get; set; }
        public int AmountOfB { get; set; }
        public int AmountOfC { get; set; }


        public void CountScore()
        {
            var accuracy = Random.Range(0.01f, 1.0f);
            UnityEngine.Debug.Log($"A: {AmountOfA} + B: {AmountOfB} + C: {AmountOfC}");
            var score = AmountOfA * accuracy * Mathf.Pow(10, 4) + AmountOfB * accuracy * Mathf.Pow(10, 3) +
                        AmountOfC * accuracy * 100;
            scoreText.text =
                $"Количество очков: {(int)score}\nРекорд: {PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Higscore")}";
            if (score > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Higscore"))
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "Higscore", (int)score);
        }
    }
}