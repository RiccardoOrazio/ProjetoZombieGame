using UnityEngine;
using UnityEngine.VFX; // Necessário para controlar o Visual Effect

public class PlayerShooting : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePointRight;
    [SerializeField] private Transform firePointLeft;
    [SerializeField] private IsometricPlayerMovement playerMovement;
    [SerializeField] private VisualEffect muzzleFlashRight;
    [SerializeField] private VisualEffect muzzleFlashLeft;

    [Header("Configurações do Tiro")]
    [SerializeField] private float projectileSpeed = 20f;

    void Start()
    {
        if (muzzleFlashRight != null) muzzleFlashRight.Stop();
        if (muzzleFlashLeft != null) muzzleFlashLeft.Stop();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Transform currentFirePoint = playerMovement.IsCurrentlyFacingRight ? firePointRight : firePointLeft;
        VisualEffect currentMuzzleFlash = playerMovement.IsCurrentlyFacingRight ? muzzleFlashRight : muzzleFlashLeft;

        if (currentMuzzleFlash != null)
        {
            currentMuzzleFlash.Play();
        }

        GameObject projectile = Instantiate(projectilePrefab, currentFirePoint.position, currentFirePoint.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = currentFirePoint.forward * projectileSpeed;
        }
    }
}