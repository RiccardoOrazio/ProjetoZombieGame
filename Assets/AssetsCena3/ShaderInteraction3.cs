using UnityEngine;

public class ShaderInteraction3 : MonoBehaviour
{
    private Material material;
    private float forcaOriginal;
    public float forcaAtual = 0f;
    public float sensibilidadeRetorno = 2f;

    void Start()
    {
        material = GetComponent<Renderer>().material;
        forcaOriginal = material.GetFloat("_ForcaDeformacao");
        forcaAtual = forcaOriginal;
    }

    void Update()
    {
        bool mousePressionado = Input.GetMouseButton(0);

        if (mousePressionado)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                material.SetVector("_PontoInteracao", hit.point);
                forcaAtual = forcaOriginal;
            }
        }
        else
        {
            forcaAtual = Mathf.Lerp(forcaAtual, 0, Time.deltaTime * sensibilidadeRetorno);
        }

        material.SetFloat("_ForcaDeformacao", forcaAtual);
    }
}
