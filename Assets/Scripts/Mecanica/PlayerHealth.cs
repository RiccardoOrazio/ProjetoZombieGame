using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float hitStunDuration = 0.5f;

    [Header("Referências da UI (Canvas)")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject deathTextObject;
    [SerializeField] private TextMeshProUGUI respawnTimerText;

    [Header("Configuração de Respawn")]
    [SerializeField] private float timeToRespawn = 5f;

    public bool IsDead { get; private set; } = false;
    private int currentHealth;
    private bool isHurting = false;

    private IsometricPlayerMovement playerMovement;
    private PlayerShooting playerShooting;
    private AimController aimController;
    private Animator animator;
    private LanternaController lanternaController;
    private AudioSource audioSource;
    private Rigidbody rb;
    private ItemSwitcher itemSwitcher;

    void Awake()
    {
        playerMovement = GetComponent<IsometricPlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();
        aimController = GetComponent<AimController>();
        animator = GetComponentInChildren<Animator>();
        lanternaController = GetComponent<LanternaController>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        itemSwitcher = GetComponent<ItemSwitcher>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        IsDead = false;
        isHurting = false;

        if (deathTextObject != null)
        {
            deathTextObject.SetActive(false);
        }

        if (respawnTimerText != null)
        {
            respawnTimerText.gameObject.SetActive(false);
        }

        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        if (isHurting) return;

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
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySound(audioSource, AudioManager.instance.playerHurt);
            }
            StartCoroutine(HandleHitStun());
        }
    }

    private IEnumerator HandleHitStun()
    {
        isHurting = true;

        if (rb != null) rb.isKinematic = true;

        if (animator != null) animator.SetTrigger("Hit");

        if (playerMovement != null) playerMovement.CanMove = false;
        if (playerShooting != null) playerShooting.CanShoot = false;
        if (aimController != null) aimController.enabled = false;
        if (lanternaController != null) lanternaController.enabled = false;
        if (itemSwitcher != null) itemSwitcher.enabled = false;

        yield return new WaitForSeconds(hitStunDuration);

        if (!IsDead)
        {
            if (rb != null) rb.isKinematic = false;

            if (playerMovement != null) playerMovement.CanMove = true;
            if (playerShooting != null) playerShooting.CanShoot = true;
            if (aimController != null) aimController.enabled = true;
            if (lanternaController != null) lanternaController.enabled = true;
            if (itemSwitcher != null) itemSwitcher.enabled = true;
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
        IsDead = true;
        StopAllCoroutines();

        Debug.Log("Player Morreu!");

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(audioSource, AudioManager.instance.playerDeath);
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (deathTextObject != null)
        {
            deathTextObject.SetActive(true);
        }

        if (playerMovement != null) playerMovement.DisableMovement();
        if (playerShooting != null) playerShooting.enabled = false;
        if (aimController != null) aimController.enabled = false;
        if (lanternaController != null) lanternaController.enabled = false;
        if (itemSwitcher != null) itemSwitcher.enabled = false;

        Cursor.visible = true;

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        if (respawnTimerText != null)
        {
            respawnTimerText.gameObject.SetActive(true);
        }

        float timer = timeToRespawn;

        while (timer > 0)
        {
            if (respawnTimerText != null)
            {
                respawnTimerText.text = Mathf.Ceil(timer).ToString();
            }
            yield return null;
            timer -= Time.deltaTime;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}