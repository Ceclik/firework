using System.Collections;
using UnityEngine;

namespace FireSeekingScripts
{
    public class SceneFinisher : MonoBehaviour
    {
        [SerializeField] private float sceneFinishDelay;
        [SerializeField] private Animator cameraAnimator;

        public IEnumerator FinishScene()
        {
            cameraAnimator.SetTrigger("FinishScene");
            yield return new WaitForSeconds(sceneFinishDelay);
            GameManager.Instance.EndScene(true);
        }
    }
}
