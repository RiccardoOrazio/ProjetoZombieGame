using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Configurações do Projétil")]
    [SerializeField] private float maxLifetime = 5f;
    [SerializeField] private int damage = 1;

    void Start()
    {
        Destroy(gameObject, maxLifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Inimigo"))
        {
            NpcHealth npcHealth = other.gameObject.GetComponent<NpcHealth>();
            if (npcHealth != null)
            {
                npcHealth.TakeDamage(damage);
            }
        }

        if (!other.gameObject.CompareTag("Player"))
        {
            if (AudioManager.instance != null)
                AudioManager.instance.PlayAtPoint(AudioManager.instance.bulletImpact, transform.position);
            Destroy(gameObject);
        }
    }
}