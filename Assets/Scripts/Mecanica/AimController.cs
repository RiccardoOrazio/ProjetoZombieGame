using UnityEngine;

public class AimController : MonoBehaviour
{
    public Vector3 AimDirection { get; private set; }
    public bool IsAiming { get; private set; }

    private IsometricPlayerMovement playerMovement;
    private Camera mainCamera;

    void Awake()
    {
        playerMovement = GetComponent<IsometricPlayerMovement>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        var right = mainCamera.transform.right;
        right.y = 0;
        AimDirection = playerMovement.IsCurrentlyFacingRight ? right.normalized : -right.normalized;
    }

    void Update()
    {
        IsAiming = Input.GetMouseButton(1);

        var forward = mainCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        var right = mainCamera.transform.right;
        right.y = 0;
        right.Normalize();

        bool isMoving = playerMovement.InputDirection.magnitude > 0.1f;

        if (IsAiming)
        {
            Plane playerPlane = new Plane(Vector3.up, transform.position);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (playerPlane.Raycast(ray, out float distance))
            {
                Vector3 worldPoint = ray.GetPoint(distance);

                Vector3 newAimDirection = (worldPoint - transform.position);
                newAimDirection.y = 0;
                newAimDirection.Normalize();

                AimDirection = newAimDirection;
            }
        }
        else if (isMoving)
        {
            Vector2 currentInput = playerMovement.InputDirection;
            AimDirection = (forward * currentInput.y + right * currentInput.x).normalized;
        }
    }
}