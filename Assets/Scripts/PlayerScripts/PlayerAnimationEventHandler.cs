using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventHandler : MonoBehaviour
{

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Flagpole flagPole;

    
    public void TriggerResetFromSpinJump()
    {
        playerMovement.ResetFromSpinJump();
    }

    public void TriggerSwapFlags()
    {
        flagPole.SwapFlags();
    }

    public void TriggerEndStageCamera()
    {
        if (playerMovement.IsPoweredUp)
        {
            playerMovement.CinematicPosition(0.0f, 0.08f);
        }
        else
        {
            playerMovement.CinematicPosition(0.0f, -0.052f);
        }
        FindObjectOfType<GameManager>().EndStageCamera();
    }

    public void TriggerCourseCompleted()
    {
        FindObjectOfType<GameManager>().CourseCompleted();
    }

    public void TriggerStartScaleDownCircle()
    {
        FindObjectOfType<GameManager>().StartScaleDownCircle();
    }

    public void TriggerPlayerDeath()
    {
        playerMovement.Death();
    }

    public void TriggerSwitchScene()
    {
        FindObjectOfType<GameManager>().SwitchScene();
    }

}
