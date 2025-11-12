using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class IsometricPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Object References")]
    [SerializeField] private Transform spriteTransform;

    [Header("Sprite Settings")]
    [SerializeField] private bool spriteFacesRightByDefault = true;

    public bool IsCurrentlyFacingRight { get; private set; }
    public Vector2 InputDirection { get; private set; }
    public bool CanMove { get; set; } = true;

    private Rigidbody rb;
    private Animator animator;
    private Vector2 lastInputDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        IsCurrentlyFacingRight = spriteFacesRightByDefault;
        animator = GetComponentInChildren<Animator>();
        lastInputDirection = new Vector2(0, -1);
        CanMove = true;
    }

    void Update()
    {
        if (!CanMove)
        {
            InputDirection = Vector2.zero;
        }
        else
        {
            InputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        bool isMoving = InputDirection.magnitude > 0.1f;
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            lastInputDirection = InputDirection.normalized;
        }

        animator.SetFloat("DirX", lastInputDirection.x);
        animator.SetFloat("DirY", lastInputDirection.y);
    }

    void FixedUpdate()
    {
        if (!CanMove)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        var forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();

        var right = Camera.main.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 moveDirection = (forward * InputDirection.y + right * InputDirection.x).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }
}