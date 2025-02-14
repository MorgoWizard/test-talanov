using UnityEngine;

public abstract class BaseState : IState
{
    protected readonly PlayerController PlayerController;
    protected readonly Animator Animator;
    
    protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    protected static readonly int JumpHash = Animator.StringToHash("Jump");

    protected BaseState(PlayerController playerController, Animator animator)
    {
        PlayerController = playerController;
        Animator = animator;
    }
    
    
    public virtual void OnEnter()
    {
        // noon
    }

    public virtual void Update()
    {
        // noon
    }

    public virtual void FixedUpdate()
    {
        // noon
    }

    public virtual void OnExit()
    {
        // noon
    }
}