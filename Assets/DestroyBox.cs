using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBox : StateMachineBehaviour
{
    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        animator.SendMessage("DestroyDamageBox");
    }

}
