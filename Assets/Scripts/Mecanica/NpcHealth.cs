using UnityEngine;

public class NpcHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
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
        Destroy(gameObject);
    }
}