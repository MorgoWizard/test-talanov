using UnityEngine;

public class LocomotionState : BaseState
{
    public LocomotionState(PlayerController playerController, Animator animator) : base(playerController, animator)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("LocomotionState.OnEnter");
        Animator.CrossFade(LocomotionHash, 0.1f);
    }

    public override void OnExit()
    {
        Debug.Log("LocomotionState.OnExit");
    }

    public override void Update()
    {
        PlayerController.HandleMove();
    }
}