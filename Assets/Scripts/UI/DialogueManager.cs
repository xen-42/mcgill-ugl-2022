using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _root;

    public TextMeshProUGUI dialogueText;
    private Queue<string> sentences;
    [SerializeField] AudioSource typewriterSound;

    public Dialogue dialogue;

    void Awake()
    {
        sentences = new Queue<string>();
    }

    void Start()
    {
        StartCoroutine(waitASec());
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
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
            // To prevent spam clicking
            _button.enabled = false;
            _root.SetActive(false);
            typewriterSound.Stop();
			
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

    IEnumerator waitASec()
    {
        yield return new WaitForSeconds(.05f);
    }
}
