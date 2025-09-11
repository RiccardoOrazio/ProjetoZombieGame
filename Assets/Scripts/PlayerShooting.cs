using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePointRight; // Ponto de tiro para a direita
    [SerializeField] private Transform firePointLeft;  // Ponto de tiro para a esquerda
    [SerializeField] private IsometricPlayerMovement playerMovement; // Referência ao script de movimento

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
        // 1. Decide qual firePoint usar
        Transform currentFirePoint = playerMovement.IsCurrentlyFacingRight ? firePointRight : firePointLeft;

        // 2. Instancia o projétil no ponto escolhido
        GameObject projectile = Instantiate(projectilePrefab, currentFirePoint.position, currentFirePoint.rotation);

        // 3. Adiciona velocidade
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = currentFirePoint.forward * projectileSpeed;
        }
    }
}