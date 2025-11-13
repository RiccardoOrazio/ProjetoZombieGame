using UnityEngine;

public class AimController : MonoBehaviour
{
    public Vector3 AimDirection { get; private set; }
    public bool IsAiming { get; private set; }
    public Transform TargetedEnemy { get; private set; } // Referência pública para o alvo

    private IsometricPlayerMovement playerMovement;
    private Camera mainCamera;
    private NpcHealth currentTargetComponent; // Referência para o script do alvo

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
            NpcHealth newTargetComponent = null;
            TargetedEnemy = null;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider.CompareTag("Inimigo"))
                {
                    newTargetComponent = hit.collider.GetComponent<NpcHealth>();
                    if (newTargetComponent != null)
                    {
                        TargetedEnemy = newTargetComponent.transform;
                    }
                }
            }

            if (newTargetComponent != currentTargetComponent)
            {
                if (currentTargetComponent != null)
                {
                    currentTargetComponent.SetTargeted(false);
                }
                if (newTargetComponent != null)
                {
                    newTargetComponent.SetTargeted(true);
                }
                currentTargetComponent = newTargetComponent;
            }

            Plane playerPlane = new Plane(Vector3.up, transform.position);
            if (playerPlane.Raycast(ray, out float distance))
            {
                Vector3 worldPoint = ray.GetPoint(distance);
                Vector3 newAimDirection = (worldPoint - transform.position);
                newAimDirection.y = 0;
                newAimDirection.Normalize();
                AimDirection = newAimDirection;
            }
        }
        else
        {
            if (currentTargetComponent != null)
            {
                currentTargetComponent.SetTargeted(false);
                currentTargetComponent = null;
            }
            TargetedEnemy = null;

            if (isMoving)
            {
                Vector2 currentInput = playerMovement.InputDirection;
                AimDirection = (forward * currentInput.y + right * currentInput.x).normalized;
            }
        }
    }
}