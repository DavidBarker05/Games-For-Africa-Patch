using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    int nextScene;
    [SerializeField]
    Button nextDialogueButton;
    [SerializeField]
    Button skipCurrentDialogueButton;
    [SerializeField]
    Button skipAllDialogueButton;
    [SerializeField]
    Image speakerImage;
    [SerializeField]
    TextMeshProUGUI speakerName;
    [SerializeField]
    TextMeshProUGUI dialogueText;
    [SerializeField]
    List<Dialogue> dialogueList = new List<Dialogue>();

    Dialogue currentDialogue;
    int currentDialogueIndex = -1;
    float readTime;
    float secondsPerCharacter;
    bool isFinishedReading;

    void Awake()
    {
        NextDialogue();
        nextDialogueButton.onClick.AddListener(NextDialogue);
        skipCurrentDialogueButton.onClick.AddListener(() => isFinishedReading = true);
        skipAllDialogueButton.onClick.AddListener(NextScene);
    }

    void Update()
    {
        if (isFinishedReading)
        {
            if (dialogueText.text.Length != currentDialogue.text.Length) dialogueText.text = currentDialogue.text;
            if (!nextDialogueButton.gameObject.activeSelf) nextDialogueButton.gameObject.SetActive(true);
            return;
        }
        if (nextDialogueButton.gameObject.activeSelf) nextDialogueButton.gameObject.SetActive(false);
        readTime += Time.deltaTime;
        if (readTime < secondsPerCharacter) return;
        int numNewCharacters = (int)(readTime * currentDialogue.charactersPerSecond);
        dialogueText.text = currentDialogue.text[..Mathf.Clamp(dialogueText.text.Length + numNewCharacters, 0, currentDialogue.text.Length)];
        readTime = 0f;
        if (dialogueText.text.Length == currentDialogue.text.Length) isFinishedReading = true;
    }

    void NextDialogue()
    {
        ++currentDialogueIndex;
        if (currentDialogueIndex == dialogueList.Count) { NextScene(); return; }
        currentDialogue = dialogueList[currentDialogueIndex];
        isFinishedReading = false;
        dialogueText.text = "";
        readTime = 0f;
        secondsPerCharacter = 1f / currentDialogue.charactersPerSecond;
        dialogueText.fontSize = currentDialogue.fontSize;
        speakerImage.sprite = currentDialogue.speakerImage;
        speakerName.text = currentDialogue.speaker;
    }

    void NextScene() => SceneManager.LoadSceneAsync(nextScene);
}
