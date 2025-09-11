using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePointRight; 
    [SerializeField] private Transform firePointLeft; 
    [SerializeField] private IsometricPlayerMovement playerMovement; 

    [Header("Configurações do Tiro")]
    [SerializeField] private float projectileSpeed = 20f;

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

        GameObject projectile = Instantiate(projectilePrefab, currentFirePoint.position, currentFirePoint.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = currentFirePoint.forward * projectileSpeed;
        }
    }
}