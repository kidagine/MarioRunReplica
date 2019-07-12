using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventHandler : MonoBehaviour
{

    [SerializeField] private PlayerMovement playerMovement;

    
    public void TriggerResetFromSpinJump()
    {
        playerMovement.ResetFromSpinJump();
    }

}
