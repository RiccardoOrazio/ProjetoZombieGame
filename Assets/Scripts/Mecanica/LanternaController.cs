using UnityEngine;

public class LanternaController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform lanternaHolder;

    [Header("Configurações")]
    [SerializeField] private float velocidadeRotacao = 15f;
    [SerializeField] private float distanciaDaLanterna = 2.5f;

    private AimController aimController;

    void Awake()
    {
        aimController = GetComponent<AimController>();
    }

    void Update()
    {
        PosicionarLanterna();
        RotacionarLanterna();
    }

    private void PosicionarLanterna()
    {
        Vector3 aimDirection = aimController.AimDirection;
        Vector3 playerCenterWithOffset = transform.position;
        Vector3 offset = aimDirection * distanciaDaLanterna;

        lanternaHolder.position = new Vector3(
            playerCenterWithOffset.x + offset.x,
            lanternaHolder.position.y,
            playerCenterWithOffset.z + offset.z
        );
    }

    private void RotacionarLanterna()
    {
        Vector3 aimDirection = aimController.AimDirection;
        if (aimDirection != Vector3.zero)
        {
            Quaternion rotacaoAlvo = Quaternion.LookRotation(aimDirection);
            lanternaHolder.rotation = Quaternion.Slerp(lanternaHolder.rotation, rotacaoAlvo, Time.deltaTime * velocidadeRotacao);
        }
    }
}