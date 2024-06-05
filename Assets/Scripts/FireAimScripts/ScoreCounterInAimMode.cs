using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FireAimScripts
{
    public class ScoreCounterInAimMode : MonoBehaviour
    {

        private float _levelTime;
        [SerializeField] private Text scoreText;
        private float _accuracy;

        private void Update()
        {
            if (GameManager.Instance.IsFireStarted)
                _levelTime += Time.deltaTime;
        }

        public float CountScore()
        {
            _accuracy = (float)Random.Range(40, 91);
            //Debug.LogError($"Accuracy: {_accuracy}\nScene name: {SceneManager.GetActiveScene().name}");
            float score = _accuracy * 10000 / (_levelTime + GetComponent<AimFireSystemHandler>().LivesCount * 10000);
            scoreText.text =
                $"Количество очков: {(int)score}\nРекорд: {PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Higscore")}";
            if(score > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name+"Higscore"))
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name+"Higscore", (int)score);
            return score;
        }
    }
}
