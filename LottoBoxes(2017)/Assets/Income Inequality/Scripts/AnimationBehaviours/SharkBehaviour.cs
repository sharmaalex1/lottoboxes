using UnityEngine;
using System.Collections;

public class SharkBehaviour : StateMachineBehaviour
{
    // When the shark completes its animation, destroy it.
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.gameObject);
    }
}
