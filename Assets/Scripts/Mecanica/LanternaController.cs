using UnityEngine;

public class LanternaController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform lanternaHolder;

    [Header("Configurações")]
    [SerializeField] private float velocidadeRotacao = 15f;

    private Vector2 inputDirection;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        inputDirection.x = Input.GetAxisRaw("Horizontal");
        inputDirection.y = Input.GetAxisRaw("Vertical");

        HandleRotacaoLanterna();
    }

    private void HandleRotacaoLanterna()
    {
        if (inputDirection == Vector2.zero) return;

        var forward = mainCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        var right = mainCamera.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 direcaoMundo = (forward * inputDirection.y + right * inputDirection.x).normalized;

        if (direcaoMundo != Vector3.zero)
        {
            Quaternion rotacaoAlvo = Quaternion.LookRotation(direcaoMundo);
            lanternaHolder.rotation = Quaternion.Slerp(lanternaHolder.rotation, rotacaoAlvo, Time.deltaTime * velocidadeRotacao);
        }
    }
}