using FireAimScripts;
using UnityEngine;

namespace AnimationTriggers
{
	public class FireTrigger : StateMachineBehaviour
	{

		[SerializeField] private float delayTime;
		private StartLevelFireSpawner _fireSpawner;
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{        
			
			FindFirstObjectByType<TvPlay>().ChangeMaterial();
			GameManager.Instance.StartCoroutine(GameManager.Instance.StartFireWithDelay(delayTime));
			_fireSpawner = GameObject.Find("Fires").GetComponent<StartLevelFireSpawner>();
			_fireSpawner.StartCoroutine(_fireSpawner.StartRandomFiresDelayed(delayTime));
		}
	}
}
