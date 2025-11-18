using UnityEngine;

public class ItemSwitcher : MonoBehaviour
{
    [Header("Sistemas para Gerenciar")]
    [SerializeField] private PlayerShooting shootingSystem;
    [SerializeField] private LanternaController lanternSystem;
    [SerializeField] private GameObject lanternaHolder;

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
    }

    private void SwitchToLantern()
    {
        if (shootingSystem != null) shootingSystem.enabled = false;

        if (lanternSystem != null) lanternSystem.enabled = true;
        if (lanternaHolder != null) lanternaHolder.SetActive(true);
    }
}