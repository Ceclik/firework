using System.Collections;
using UnityEngine;

namespace FireSeekingScripts
{
    public class SceneFinisher : MonoBehaviour
    {
        [SerializeField] private float sceneFinishDelay;
        [SerializeField] private Animator cameraAnimator;
        [SerializeField] private GameObject timer;

        public IEnumerator FinishScene()
        {
            timer.SetActive(false);
            cameraAnimator.SetTrigger("FinishScene");
            yield return new WaitForSeconds(sceneFinishDelay);
            GameManager.Instance.EndScene(true);
        }
    }
}
