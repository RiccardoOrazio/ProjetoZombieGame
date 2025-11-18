using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float hitStunDuration = 0.5f;

    [Header("Referências da UI (Canvas)")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject deathTextObject;

    private int currentHealth;
    private bool isDead = false;
    private bool isHurting = false;

    private IsometricPlayerMovement playerMovement;
    private PlayerShooting playerShooting;
    private AimController aimController;
    private Animator animator;

    void Awake()
    {
        playerMovement = GetComponent<IsometricPlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();
        aimController = GetComponent<AimController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
        isHurting = false;

        if (deathTextObject != null)
        {
            deathTextObject.SetActive(false);
        }

        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isHurting) return;

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
        else
        {
            StartCoroutine(HandleHitStun());
        }
    }

    private IEnumerator HandleHitStun()
    {
        isHurting = true;

        if (animator != null) animator.SetTrigger("Hit");

        if (playerMovement != null) playerMovement.CanMove = false;
        if (playerShooting != null) playerShooting.CanShoot = false;
        if (aimController != null) aimController.enabled = false;

        yield return new WaitForSeconds(hitStunDuration);

        if (!isDead)
        {
            if (playerMovement != null) playerMovement.CanMove = true;
            if (playerShooting != null) playerShooting.CanShoot = true;
            if (aimController != null) aimController.enabled = true;
            isHurting = false;
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

        if (animator != null) animator.SetTrigger("Hit");

        if (deathTextObject != null)
        {
            deathTextObject.SetActive(true);
        }

        if (playerMovement != null) playerMovement.CanMove = false;
        if (playerShooting != null) playerShooting.CanShoot = false;
        if (aimController != null) aimController.enabled = false;
    }
}