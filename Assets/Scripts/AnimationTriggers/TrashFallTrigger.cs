using FireAimScripts;
using Instructions;
using Scenes;
using UnityEngine;

namespace AnimationTriggers
{
    public class TrashFallTrigger : StateMachineBehaviour
    {
        private LevelInstructionShower _instruction;
        private Animator _selfAnimator;
        private StartLevelFireSpawner _startFireSpawner;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            _selfAnimator = animator;
            _instruction = GameObject.Find("LevelInstructionShower").GetComponent<LevelInstructionShower>();
            _instruction.OnStartButtonClicked += StartFire;
            _startFireSpawner = GameObject.Find("Fires").GetComponent<StartLevelFireSpawner>();
        }

        private void StartFire()
        {
            var can = GameObject.Find("TrashCan");
            TrashCan trashCan = can.GetComponent<TrashCan>();
            _selfAnimator.SetTrigger("StartFire");
            
            if (!trashCan.falled)
                trashCan.TrashFall();
            
            _startFireSpawner.StartRandomFires();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            _instruction.OnStartButtonClicked -= StartFire;
        }
        
        
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

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
