using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTakeHit : StateMachineBehaviour
{
    public int TakeHitAnimations = 2;

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash);

        animator.SetInteger("TakeHitIndex", Random.Range(0, TakeHitAnimations));
    }
}
