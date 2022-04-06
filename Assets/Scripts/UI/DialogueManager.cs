using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    private Queue<string> sentences;
    [SerializeField] AudioSource typewriterSound;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            typewriterSound.Stop();
            SceneManager.LoadScene(Scenes.MainMenu);
            return;
        }

        string sent = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sent));
    }

    IEnumerator TypeSentence(string sent)
    {
        typewriterSound.Play();
        dialogueText.text = "";
        foreach (char letter in sent.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(.06f);
        }
        typewriterSound.Stop();
    }
}
