using System.Threading.Tasks;
using Dolls.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
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
        Vector3 camForwardXZ = Vector3.ProjectOnPlane(_mainCamera.forward, Vector3.up).normalized;
        Vector3 camRightXZ = Vector3.ProjectOnPlane(_mainCamera.right, Vector3.up).normalized;
        
        Vector3 moveDirection = camRightXZ * _lastMoveInput.x + camForwardXZ * _lastMoveInput.y;
        
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            transform.forward = moveDirection;
        }
        
        var newSpeed = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
        rb.linearVelocity = newSpeed;
        
        animator.SetFloat(Speed, moveDirection.magnitude > 0.1f ? 1 : 0);
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
