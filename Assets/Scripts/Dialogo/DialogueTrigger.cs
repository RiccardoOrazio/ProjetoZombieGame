using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class DialogueTrigger : MonoBehaviour
{
    [Header("Configuração do Diálogo")]
    [TextArea(3, 10)]
    [SerializeField] private string[] lines;
    [SerializeField] private bool startAutomatically = false;

    [Header("UI de Interação")]
    [SerializeField] private TextMeshProUGUI interactionPromptText;
    [SerializeField] private string promptMessage = "Pressione E para conversar";
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    private bool playerIsInRange = false;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = true;
            if (startAutomatically)
            {
                StartDialogue();
            }
            else if (interactionPromptText != null)
            {
                interactionPromptText.text = promptMessage;
                interactionPromptText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = false;
            if (interactionPromptText != null)
            {
                interactionPromptText.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (playerIsInRange && !startAutomatically && Input.GetKeyDown(interactionKey))
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
        DialogueManager.instance.StartDialogue(lines);
    }
}