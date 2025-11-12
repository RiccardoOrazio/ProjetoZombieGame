using UnityEngine;

public class PlayerMovementLocker : StateMachineBehaviour
{
    private Rigidbody rb;
    private PlayerShooting playerShooting;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        IsometricPlayerMovement playerMovement = animator.GetComponentInParent<IsometricPlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.CanMove = false;
        }

        rb = animator.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        IsometricPlayerMovement playerMovement = animator.GetComponentInParent<IsometricPlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.CanMove = true;
        }

        playerShooting = animator.GetComponentInParent<PlayerShooting>();
        if (playerShooting != null)
        {
            playerShooting.CanShoot = true;
        }
    }
}