using UnityEngine;
using UnityEngine.VFX;

public class PlayerShooting : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private VisualEffect muzzleFlash;

    [Header("Configurações do Tiro")]
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float distanciaDoPontoTiro = 2.5f;

    public bool CanShoot { get; set; } = true;

    private AimController aimController;
    private Animator animator;

    void Awake()
    {
        aimController = GetComponent<AimController>();
        animator = GetComponentInChildren<Animator>();
        CanShoot = true;
    }

    void Update()
    {
        PosicionarPontoDeTiro();

        if (Input.GetMouseButtonDown(0) && CanShoot)
        {
            Shoot();
        }
    }

    private void PosicionarPontoDeTiro()
    {
        Vector3 aimDirection = aimController.AimDirection;
        Vector3 playerCenterWithOffset = transform.position;
        Vector3 offset = aimDirection * distanciaDoPontoTiro;

        firePoint.position = new Vector3(
            playerCenterWithOffset.x + offset.x,
            1.55f,
            playerCenterWithOffset.z + offset.z
        );
    }

    void Shoot()
    {
        Vector3 finalDirection;
        Transform target = aimController.TargetedEnemy;

        if (target != null)
        {
            Vector3 targetPosition = target.position;
            Collider targetCollider = target.GetComponent<Collider>();
            if (targetCollider != null)
            {
                targetPosition = targetCollider.bounds.center;
            }

            finalDirection = (targetPosition - firePoint.position).normalized;
        }
        else if (aimController.AimDirection.sqrMagnitude > 0.01f)
        {
            finalDirection = aimController.AimDirection;
        }
        else
        {
            return;
        }

        CanShoot = false;
        animator.SetTrigger("Shoot");

        firePoint.rotation = Quaternion.LookRotation(finalDirection);

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = finalDirection * projectileSpeed;
        }
    }
}