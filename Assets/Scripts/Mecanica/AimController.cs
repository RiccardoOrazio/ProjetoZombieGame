using UnityEngine;

public class AimController : MonoBehaviour
{
    public Vector3 AimDirection { get; private set; }

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
        Vector2 inputAtual = playerMovement.InputDirection;

        if (inputAtual.magnitude > 0.1f)
        {
            var forward = mainCamera.transform.forward;
            forward.y = 0;
            var right = mainCamera.transform.right;
            right.y = 0;

            AimDirection = (forward * inputAtual.y + right * inputAtual.x).normalized;
        }
    }
}