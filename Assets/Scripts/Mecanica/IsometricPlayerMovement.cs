using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class IsometricPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Object References")]
    [SerializeField] private Transform spriteTransform;

    [Header("Sprite Settings")]
    [SerializeField] private bool spriteFacesRightByDefault = true;

    [Header("Audio Settings")]
    [SerializeField] private float stepInterval = 0.4f;

    public bool IsCurrentlyFacingRight { get; private set; }
    public Vector2 InputDirection { get; private set; }
    public Vector2 LastInputDirection { get; private set; }
    public bool CanMove { get; set; } = true;

    private Rigidbody rb;
    private Animator animator;
    private AimController aimController;
    private AudioSource audioSource;
    private float stepTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        IsCurrentlyFacingRight = spriteFacesRightByDefault;
        animator = GetComponentInChildren<Animator>();
        aimController = GetComponent<AimController>();
        audioSource = GetComponent<AudioSource>();
        LastInputDirection = new Vector2(0, -1);
        CanMove = true;
    }

    void Update()
    {
        bool isAiming = aimController.IsAiming;
        bool dialogueActive = (DialogueManager.instance != null && DialogueManager.instance.IsDialogueActive);

        if (!CanMove || isAiming || dialogueActive)
        {
            InputDirection = Vector2.zero;
        }
        else
        {
            InputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        bool isMoving = InputDirection.magnitude > 0.1f;
        animator.SetBool("IsMoving", isMoving);

        animator.SetBool("IsAiming", isAiming);

        if (isMoving)
        {
            LastInputDirection = InputDirection.normalized;
            HandleFootsteps();
        }
        else
        {
            stepTimer = 0f;
        }

        Vector3 directionToAnimate = aimController.AimDirection;
        var forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();
        var right = Camera.main.transform.right;
        right.y = 0;
        right.Normalize();

        float dirX = Vector3.Dot(directionToAnimate, right);
        float dirY = Vector3.Dot(directionToAnimate, forward);

        animator.SetFloat("DirX", dirX);
        animator.SetFloat("DirY", dirY);
    }

    void FixedUpdate()
    {
        if (!CanMove || aimController.IsAiming || (DialogueManager.instance != null && DialogueManager.instance.IsDialogueActive))
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

    private void HandleFootsteps()
    {
        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0)
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayRandomFootstep(audioSource);
            }
            stepTimer = stepInterval;
        }
    }
}