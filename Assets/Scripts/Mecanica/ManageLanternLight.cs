using UnityEngine;

public class ManageLanternLight : StateMachineBehaviour
{
    [SerializeField] private bool lightOnOnEnter = false;
    private LanternaController lanternaController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (lanternaController == null)
        {
            lanternaController = animator.GetComponentInParent<LanternaController>();
        }

        if (lanternaController != null)
        {
            lanternaController.SetLightSourceEnabled(lightOnOnEnter);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (lanternaController != null)
        {
            lanternaController.SetLightSourceEnabled(false);
        }
    }
}