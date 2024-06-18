using FireAimScripts;
using Instructions;
using UnityEngine;

namespace AnimationTriggers
{
	public class ThrowTrigger : StateMachineBehaviour
	{

		private LevelInstructionShower _instructions;
		private Animator _selfAnimator;
		
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			_selfAnimator = animator;
			_instructions = GameObject.Find("LevelInstructionShower").GetComponent<LevelInstructionShower>();
			_instructions.OnStartButtonClicked += ThrowRemote;
			
		}

		private void ThrowRemote()
		{
			var remote = GameObject.FindGameObjectWithTag("Remote");
			var anim = remote.GetComponent<Animator>();
			anim.SetTrigger("Throw");
			_selfAnimator.SetTrigger("TeenThrowing");
			
		}
		
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
		{
			_instructions.OnStartButtonClicked -= ThrowRemote;
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
		//}
		// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
		//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
		//}
		// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
		//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
		//}
	}
}
