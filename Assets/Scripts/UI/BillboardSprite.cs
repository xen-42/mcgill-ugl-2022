using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BillboardSprite : MonoBehaviour
{
    [SerializeField] Interactable interactable;
    [SerializeField] bool showIfInteractable;
    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();    
    }

    void Update()
    {
        if(interactable != null)
        {
            var shouldBeActive = interactable.IsInteractable == showIfInteractable;
            if (shouldBeActive != sprite.enabled)
            {
                sprite.enabled = !sprite.enabled;
            }
        }

        transform.LookAt(Camera.main.transform); 
    }
}
