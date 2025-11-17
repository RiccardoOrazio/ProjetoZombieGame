using UnityEngine;

public class ManageShootingState : StateMachineBehaviour
{
    [SerializeField] private bool canShootOnEnter = false;
    private PlayerShooting playerShooting;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerShooting == null)
        {
            playerShooting = animator.GetComponentInParent<PlayerShooting>();
        }

        if (playerShooting != null)
        {
            playerShooting.CanShoot = canShootOnEnter;
        }
    }

}