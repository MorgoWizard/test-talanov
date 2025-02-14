using UnityEngine;

public class JumpState : BaseState
{
    public JumpState(PlayerController playerController, Animator animator) : base(playerController, animator)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("JumpState.OnEnter");
        Animator.CrossFade(JumpHash, 0.1f);
        PlayerController.Jump();
    }

    public override void Update()
    {
        PlayerController.HandleMove();
    }

    public override void OnExit()
    {
        Debug.Log("JumpState.OnExit");
    }
}