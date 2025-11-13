using UnityEngine;

public class NpcHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private GameObject targetCircle;
    private int currentHealth;

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

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (GameManagerTemporario.instance != null)
        {
            GameManagerTemporario.instance.OnEnemyKilled();
        }
        SetTargeted(false);
        Destroy(gameObject);
    }
}