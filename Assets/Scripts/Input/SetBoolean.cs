using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBoolean : StateMachineBehaviour
{
    public string propertyName;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(propertyName, true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(propertyName, false);
    }

    
}
