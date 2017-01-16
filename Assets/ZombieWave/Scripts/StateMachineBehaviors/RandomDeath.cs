using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDeath : StateMachineBehaviour
{
    public int DeathAnimations = 4;

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash);

        animator.SetInteger("DeathIndex", Random.Range(0, DeathAnimations));
    }
}
