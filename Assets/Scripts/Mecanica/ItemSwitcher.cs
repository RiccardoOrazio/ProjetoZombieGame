using UnityEngine;

public class ItemSwitcher : MonoBehaviour
{
    [Header("Sistemas para Gerenciar")]
    [SerializeField] private PlayerShooting shootingSystem;
    [SerializeField] private LanternaController lanternSystem;
    [SerializeField] private GameObject lanternaHolder;

    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        SwitchToPistol();
    }

    void Update()
    {
        if (DialogueManager.instance != null && DialogueManager.instance.IsDialogueActive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToPistol();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToLantern();
        }
    }

    private void SwitchToPistol()
    {
        if (shootingSystem != null) shootingSystem.enabled = true;
        if (lanternSystem != null) lanternSystem.enabled = false;
        if (lanternaHolder != null) lanternaHolder.SetActive(false);

        if (animator != null)
        {
            animator.SetLayerWeight(1, 0f);
        }
    }

    private void SwitchToLantern()
    {
        if (shootingSystem != null) shootingSystem.enabled = false;
        if (lanternSystem != null) lanternSystem.enabled = true;
        if (lanternaHolder != null) lanternaHolder.SetActive(true);

        if (animator != null)
        {
            animator.SetLayerWeight(1, 1f);
        }
    }
}