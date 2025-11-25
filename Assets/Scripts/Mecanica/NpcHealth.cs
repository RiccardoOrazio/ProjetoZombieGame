using UnityEngine;
using System.Collections;

public class NpcHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private GameObject targetCircle;
    [SerializeField] private float hitStunDuration = 0.5f;

    private int currentHealth;
    private Animator animator;
    private EnemyAI enemyAI;
    private AudioSource audioSource;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (targetCircle != null)
        {
            targetCircle.SetActive(false);
        }
    }

    public void SetTargeted(bool isTargeted)
    {
        if (targetCircle != null)
        {
            targetCircle.SetActive(isTargeted);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth > 0)
        {
            if (animator != null) animator.SetTrigger("Hit");
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySound(audioSource, AudioManager.instance.zombieHurt);
            if (enemyAI != null) StartCoroutine(StunRoutine());
        }
        else
        {
            Die();
        }
    }

    private IEnumerator StunRoutine()
    {
        enemyAI.SetStunned(true);
        yield return new WaitForSeconds(hitStunDuration);
        enemyAI.SetStunned(false);
    }

    private void Die()
    {
        if (GameManagerTemporario.instance != null)
        {
            GameManagerTemporario.instance.OnEnemyKilled();
        }

        if (AudioManager.instance != null)
            AudioManager.instance.PlayAtPoint(AudioManager.instance.zombieDeath, transform.position);

        SetTargeted(false);
        Destroy(gameObject);
    }
}