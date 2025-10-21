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
    [SerializeField] private float distanciaDoPontoDeTiro = 2.5f;

    private AimController aimController;

    void Awake()
    {
        aimController = GetComponent<AimController>();
    }

    void Update()
    {
        PosicionarPontoDeTiro();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void PosicionarPontoDeTiro()
    {
        Vector3 aimDirection = aimController.AimDirection;
        Vector3 playerCenterWithOffset = transform.position;
        Vector3 offset = aimDirection * distanciaDoPontoDeTiro;

        firePoint.position = new Vector3(
            playerCenterWithOffset.x + offset.x,
            firePoint.position.y,
            playerCenterWithOffset.z + offset.z
        );
    }

    void Shoot()
    {
        Vector3 aimDirection = aimController.AimDirection;
        if (aimDirection == Vector3.zero) return;

        firePoint.rotation = Quaternion.LookRotation(aimDirection);

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = aimDirection * projectileSpeed;
        }
    }
}