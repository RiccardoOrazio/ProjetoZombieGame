using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class IsometricPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Object References")]
    [SerializeField] private Transform spriteTransform;

    [Header("Sprite Settings")]
    [SerializeField] private bool spriteFacesRightByDefault = true; // Marque isso se seu sprite original olha para a direita

    private Rigidbody rb;
    private Vector2 inputDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (spriteTransform == null)
        {
            Debug.LogError("A referência para o 'Sprite Transform' não foi definida no Inspector!");
            enabled = false;
        }
    }

    void Update()
    {
        inputDirection.x = Input.GetAxisRaw("Horizontal");
        inputDirection.y = Input.GetAxisRaw("Vertical");
        HandleSpriteDirection();
    }

    void FixedUpdate()
    {
        var forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();

        var right = Camera.main.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 moveDirection = (forward * inputDirection.y + right * inputDirection.x).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    private void HandleSpriteDirection()
    {
        if (inputDirection.x != 0)
        {
            float directionMultiplier = spriteFacesRightByDefault ? 1f : -1f;
            float scaleX = Mathf.Abs(spriteTransform.localScale.x);

            spriteTransform.localScale = new Vector3(
                scaleX * Mathf.Sign(inputDirection.x) * directionMultiplier,
                spriteTransform.localScale.y,
                spriteTransform.localScale.z
            );
        }
    }
}