using UnityEngine;
using System.Collections;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("Componentes da UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI textComponent;

    [Header("Configurações")]
    public float textSpeed;
    public KeyCode advanceKey = KeyCode.Mouse1;

    private string[] currentLines;
    private int index;
    private bool dialogueIsActive = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (dialoguePanel == null)
        {
            Debug.LogError("Painel de Diálogo não atribuído no DialogueManager!");
            return;
        }
        textComponent.text = string.Empty;
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (!dialogueIsActive) return;

        if (Input.GetKeyDown(advanceKey))
        {
            if (textComponent.text == currentLines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = currentLines[index];
            }
        }
    }

    public void StartDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0 || dialoguePanel == null) return;

        dialoguePanel.SetActive(true);
        dialogueIsActive = true;
        currentLines = lines;
        index = 0;
        textComponent.text = string.Empty;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in currentLines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < currentLines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueIsActive = false;
        textComponent.text = string.Empty;
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }
}