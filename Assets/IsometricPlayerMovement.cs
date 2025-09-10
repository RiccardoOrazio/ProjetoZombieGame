using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class IsometricPlayerMovement : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 5f;


    private Rigidbody rb;


    private Vector2 inputDirection;

    void Awake()
    {

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

        inputDirection.x = Input.GetAxisRaw("Horizontal");
        inputDirection.y = Input.GetAxisRaw("Vertical");
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
}