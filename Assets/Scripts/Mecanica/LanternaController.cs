using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LanternaController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Light lanternaLight;

    [Header("Configurações")]
    [SerializeField] private float velocidadeRotacao = 15f;
    [SerializeField] private float distanciaDaLanterna = 2.5f;

    private AimController aimController;
    private AudioSource audioSource;
    private bool wasAiming = false;

    void Awake()
    {
        aimController = GetComponent<AimController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (lanternaLight != null)
        {
            lanternaLight.enabled = false;
        }
    }

    public void SetLightSourceEnabled(bool state)
    {
        if (lanternaLight != null)
        {
            if (state && !aimController.IsAiming) return;
            lanternaLight.enabled = state;
        }
    }

    void Update()
    {
        if (DialogueManager.instance != null && DialogueManager.instance.IsDialogueActive)
        {
            if (lanternaLight != null) lanternaLight.enabled = false;
            return;
        }

        if (aimController.IsAiming)
        {
            PosicionarLanterna();
            RotacionarLanterna();

            if (!wasAiming)
            {
                if (AudioManager.instance != null)
                    AudioManager.instance.PlaySound(audioSource, AudioManager.instance.flashlightClick);
                wasAiming = true;
            }
        }
        else
        {
            if (wasAiming)
            {
                if (AudioManager.instance != null)
                    AudioManager.instance.PlaySound(audioSource, AudioManager.instance.flashlightClick);
                wasAiming = false;
            }
        }

        if (lanternaLight != null && lanternaLight.enabled)
        {
            if (DialogueManager.instance != null && DialogueManager.instance.IsDialogueActive)
            {
                lanternaLight.enabled = false;
            }
        }
    }

    private void PosicionarLanterna()
    {
        Vector3 aimDirection = aimController.AimDirection;
        Vector3 playerCenterWithOffset = transform.position;
        Vector3 offset = aimDirection * distanciaDaLanterna;

        lanternaLight.transform.position = new Vector3(
            playerCenterWithOffset.x + offset.x,
            lanternaLight.transform.position.y,
            playerCenterWithOffset.z + offset.z
        );
    }

    private void RotacionarLanterna()
    {
        Vector3 aimDirection = aimController.AimDirection;
        if (aimDirection != Vector3.zero)
        {
            Quaternion rotacaoAlvo = Quaternion.LookRotation(aimDirection);
            lanternaLight.transform.rotation = Quaternion.Slerp(lanternaLight.transform.rotation, rotacaoAlvo, Time.deltaTime * velocidadeRotacao);
        }
    }
}