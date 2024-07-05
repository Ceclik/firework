using Instructions;
using UnityEngine;

public class SchoolSeekFireStarter : StateMachineBehaviour
{
    private LevelInstructionShower _instruction;
    private Animator _selfAnimator;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _instruction = GameObject.Find("LevelInstructionShower").GetComponent<LevelInstructionShower>();
        _selfAnimator = animator;
        _instruction.OnStartButtonClicked += StartFire;
    }

    private void StartFire()
    {
        _selfAnimator.SetTrigger("Fire");
    }
}