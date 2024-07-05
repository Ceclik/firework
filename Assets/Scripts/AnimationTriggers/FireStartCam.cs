using FireAimScripts;
using Instructions;
using UnityEngine;

namespace AnimationTriggers
{
    public class FireStartCam : StateMachineBehaviour
    {
        [SerializeField] private float fireStartDelay;
        private StartLevelFireSpawner _fireSpawner;

        private LevelInstructionShower _instruction;
        private Animator _selfAnimator;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _selfAnimator = animator;
            _instruction = GameObject.Find("LevelInstructionShower").GetComponent<LevelInstructionShower>();
            _instruction.OnStartButtonClicked += StartFire;
            _fireSpawner = GameObject.Find("Fires").GetComponent<StartLevelFireSpawner>();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            _instruction.OnStartButtonClicked -= StartFire;
        }

        private void StartFire()
        {
            _selfAnimator.SetTrigger("Start");
            GameManager.Instance.StartCoroutine(GameManager.Instance.StartFireWithDelay(fireStartDelay));
            _fireSpawner.StartCoroutine(_fireSpawner.StartRandomFiresDelayed(fireStartDelay));
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