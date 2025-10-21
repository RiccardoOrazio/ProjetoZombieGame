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

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        IsCurrentlyFacingRight = spriteFacesRightByDefault;
    }

    void Update()
    {
        InputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (InputDirection.x != 0)
        {
            HandleSpriteDirection();
        }
    }

    void FixedUpdate()
    {
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

    private void HandleSpriteDirection()
    {
        float directionMultiplier = spriteFacesRightByDefault ? 1f : -1f;
        float scaleX = Mathf.Abs(spriteTransform.localScale.x);

        spriteTransform.localScale = new Vector3(
            scaleX * Mathf.Sign(InputDirection.x) * directionMultiplier,
            spriteTransform.localScale.y,
            spriteTransform.localScale.z
        );

        if (spriteTransform.localScale.x > 0)
        {
            IsCurrentlyFacingRight = spriteFacesRightByDefault;
        }
        else
        {
            IsCurrentlyFacingRight = !spriteFacesRightByDefault;
        }
    }
}