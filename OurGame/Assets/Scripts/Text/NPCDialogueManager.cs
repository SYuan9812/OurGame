using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCDialogueManager : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [TextArea(2, 5)]
    public string[] dialogueLines;
    public bool autoStartOnLoad = true;
    public KeyCode nextDialogueKey = KeyCode.Space;
    public float typeWriterDelay = 0.02f;

    [Header("Component References")]
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;

    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool isWaitingForInput = true;
    private Coroutine typeWriterCoroutine;

    private void Awake()
    {
        if (dialogueText == null) dialogueText = GetComponentInChildren<TextMeshProUGUI>(true);
        if (dialoguePanel == null) dialoguePanel = gameObject;

        dialoguePanel.SetActive(true);
    }

    private void Start()
    {
        if (autoStartOnLoad && dialogueLines != null && dialogueLines.Length > 0)
        {
            StartDialogue();
        }
        else if (dialogueLines == null || dialogueLines.Length == 0)
        {
            dialoguePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (isDialogueActive && isWaitingForInput && Input.GetKeyDown(nextDialogueKey))
        {
            ShowNextDialogueLine();
        }
    }

    public void StartDialogue()
    {
        if (!dialoguePanel || !dialogueText) return;

        currentLineIndex = 0;
        isDialogueActive = true;
        isWaitingForInput = true;

        PlayTypeWriterForCurrentLine();
    }

    private void ShowNextDialogueLine()
    {
        if (typeWriterCoroutine != null) StopCoroutine(typeWriterCoroutine);

        currentLineIndex++;

        if (currentLineIndex >= dialogueLines.Length)
        {
            dialoguePanel.SetActive(false);
            isDialogueActive = false;
            return;
        }

        PlayTypeWriterForCurrentLine();
    }

    private void PlayTypeWriterForCurrentLine()
    {
        if (currentLineIndex < 0 || currentLineIndex >= dialogueLines.Length) return;
        typeWriterCoroutine = StartCoroutine(TypeWriterEffect(dialogueLines[currentLineIndex]));
    }

    private IEnumerator TypeWriterEffect(string textToShow)
    {
        isWaitingForInput = false;
        dialogueText.text = "";
        foreach (char c in textToShow.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeWriterDelay);
        }
        isWaitingForInput = true;
    }
}