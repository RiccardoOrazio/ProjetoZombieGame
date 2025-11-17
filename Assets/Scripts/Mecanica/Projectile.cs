using UnityEngine;

public class Projectile : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NpcHealth>(out NpcHealth npcHealth))
        {
            npcHealth.TakeDamage(1); 
        }

        Destroy(gameObject);
    }
}