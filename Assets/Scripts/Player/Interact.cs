using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Interact : MonoBehaviour
{
    public LayerMask interactableLayermask;
    public Transform cam;
    public Interactable interactable;
    public Image interactImage;
    public Sprite defaultIcon;
    public Sprite defaultInteractIcon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
         if(Physics.Raycast(cam.position, cam.forward, out hit, 2, interactableLayermask))
        {
             if (hit.collider.GetComponent<Interactable>() != false){
                if(interactable == null || interactable.ID != hit.collider.GetComponent<Interactable>().ID){
                    interactable = hit.collider.GetComponent<Interactable>();

                }
                if(interactable.interactIcon != null){
                    interactImage.sprite = interactable.interactIcon;
                }else{
                    interactImage.sprite = defaultInteractIcon;
                }
                 if(Input.GetKeyDown(KeyCode.E)){
                     //
                     interactable.onInteract.Invoke();
                 }
             }
        }else{
            if (interactImage.sprite != defaultIcon){
                interactImage.sprite = defaultIcon;
            }
        }
    }
}
