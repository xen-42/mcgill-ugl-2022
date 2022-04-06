using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueStart : MonoBehaviour
{
   public Dialogue dialogue;

   void Start(){
       StartCoroutine(waitASec());
       FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
   }

   IEnumerator waitASec(){
       yield return new WaitForSeconds(.05f);
   }
}
