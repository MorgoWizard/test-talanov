using System;
using System.Threading.Tasks;
using Dolls.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    
    [SerializeField] private GroundChecker groundChecker;
    
    private PlayerInputs _playerInputs;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    private Vector2 _lastMoveInput;
    
    private Transform _mainCamera;
    
    private StateMachine _stateMachine;
    
    private bool _isGrounded;
    private bool _isTryingToJump;

    private void Awake()
    {
        _playerInputs = new PlayerInputs();
        _mainCamera = Camera.main.transform;
        
        SetupStateMachine();
    }

    private void OnEnable()
    {
        _playerInputs.Enable();
        _playerInputs.Player.Move.performed += OnMove;
        _playerInputs.Player.Move.canceled += OnMove;
        _playerInputs.Player.Jump.performed += OnJump;
    }
    private void OnDisable()
    {
        _playerInputs.Disable();
        _playerInputs.Player.Move.performed -= OnMove;
        _playerInputs.Player.Move.canceled -= OnMove;
        _playerInputs.Player.Jump.performed -= OnJump;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        var moveInput = context.ReadValue<Vector2>();
        _lastMoveInput = moveInput.normalized;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        TryToJump();
    }

    private async void TryToJump()
    {
        _isTryingToJump = true;
        await Task.Delay(200);
        _isTryingToJump = false;
    }

    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void HandleMove()
    {
        rb.linearVelocity = new Vector3(_lastMoveInput.x * moveSpeed, rb.linearVelocity.y, _lastMoveInput.y * moveSpeed);
    }

    private void SetupStateMachine()
    {
        _stateMachine = new StateMachine();

        var locomotionState = new LocomotionState(this, animator);
        var jumpState = new JumpState(this, animator);
        
        At(locomotionState,jumpState, new FuncPredicate(() => _isGrounded && _isTryingToJump));
        At(jumpState,locomotionState, new FuncPredicate(() => _isGrounded && !_isTryingToJump));
        
        _stateMachine.SetState(locomotionState);
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }

    public void SetGrounded(bool state)
    {
        _isGrounded = state;
    }
    
    private void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
    private void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
}
