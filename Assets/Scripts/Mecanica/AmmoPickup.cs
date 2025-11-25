using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 8;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerShooting playerShooting = other.GetComponent<PlayerShooting>();

            if (playerShooting != null)
            {
                playerShooting.AddAmmo(ammoAmount);
                if (AudioManager.instance != null)
                    AudioManager.instance.PlayAtPoint(AudioManager.instance.ammoPickup, transform.position);
                Destroy(gameObject);
            }
        }
    }
}