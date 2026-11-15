using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityScale;
    [SerializeField] private float movementSmoothTime;

    [Header("Ground Detection")]
    [SerializeField] private Transform feet;
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask whatIsGround;

    private CharacterController controller;
    private Camera cam;

    private bool isGrounded;
    private Vector2 inputVector;
    private Vector3 verticalMovement;
    private Vector3 horizontalVelocity, horizontalVelocityRef;

    public PlayerInput PlayerInput { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        PlayerInput = GetComponent<PlayerInput>();
        cam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        if (PlayerInput != null && PlayerInput.actions != null)
        {
            PlayerInput.actions["Move"].performed += UpdateMovement;
            PlayerInput.actions["Move"].canceled += UpdateMovement;
            PlayerInput.actions["Jump"].started += Jump;
        }
    }

    private void OnDisable()
    {
        if (PlayerInput != null && PlayerInput.actions != null)
        {
            PlayerInput.actions["Move"].performed -= UpdateMovement;
            PlayerInput.actions["Move"].canceled -= UpdateMovement;
            PlayerInput.actions["Jump"].started -= Jump;
        }
    }

    private void UpdateMovement(InputAction.CallbackContext ctx)
    {
        inputVector = ctx.ReadValue<Vector2>();
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (isGrounded)
        {
            verticalMovement.y = Mathf.Sqrt(-2f * gravityScale * jumpHeight);
        }
    }

    private void Update()
    {
        GroundCheck();
        ApplyGravity();
        MoveAndRotate();
    }

    private void MoveAndRotate()
    {
        transform.rotation = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);

        Vector3 targetVelocity = transform.TransformDirection(inputVector.x, 0f, inputVector.y) * movementSpeed;
        horizontalVelocity = Vector3.SmoothDamp(horizontalVelocity, targetVelocity, ref horizontalVelocityRef, movementSmoothTime);

        controller.Move((horizontalVelocity + verticalMovement) * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (isGrounded && verticalMovement.y < 0f)
        {
            verticalMovement.y = -2f;
        }
        else
        {
            verticalMovement.y += gravityScale * Time.deltaTime;
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(feet.position, detectionRadius, whatIsGround);
    }

    public void DisableControl()
    {
        inputVector = Vector2.zero;
        horizontalVelocity = Vector3.zero;
        verticalMovement = Vector3.zero;

        if (PlayerInput != null)
        {
            PlayerInput.enabled = false;
        }

        var cinemachineInput = FindFirstObjectByType<CinemachineInputAxisController>();
        if (cinemachineInput != null)
        {
            cinemachineInput.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        enabled = false;
    }

    private void OnDrawGizmos()
    {
        if (feet != null)
        {
            Gizmos.DrawSphere(feet.position, detectionRadius);
        }
    }
}