using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    [SerializeField] private int maxHealth = 100;

    [Header("Referências da UI (Canvas)")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject deathTextObject;

    private int currentHealth;
    private bool isDead = false;

    private IsometricPlayerMovement playerMovement;
    private PlayerShooting playerShooting;
    private AimController aimController;

    void Awake()
    {
        playerMovement = GetComponent<IsometricPlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();
        aimController = GetComponent<AimController>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;

        if (deathTextObject != null)
        {
            deathTextObject.SetActive(false);
        }

        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player Morreu!");

        if (deathTextObject != null)
        {
            deathTextObject.SetActive(true);
        }

        if (playerMovement != null) playerMovement.CanMove = false;
        if (playerShooting != null) playerShooting.CanShoot = false;
        if (aimController != null) aimController.enabled = false;
    }
}