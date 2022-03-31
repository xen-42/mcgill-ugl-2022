using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerMinigame : DragDropMinigame
{
    [SerializeField] public GameObject openScanner;
    [SerializeField] public GameObject closedScanner;

    [SerializeField] public float delayTime = 1f;
    private float _delay = 0f;
    private bool _completed;

    public void Update()
    {
        if (_completed)
        {
            _delay += Time.deltaTime;
            if(_delay > delayTime)
            {
                CompleteMinigame();
            }
        }
        else
        {
            // Check for completion
            if (!holdableObject.IsHeld && dropCollider.bounds.Intersects(_heldCollider.bounds))
            {
                ScanItem();
            }
        }
    }

    public void ScanItem()
    {
        // Get rid of the paper and close the scanner
        holdableObject.gameObject.SetActive(false);

        openScanner.SetActive(false);
        closedScanner.SetActive(true);

        _completed = true;
    }
}
