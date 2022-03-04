using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSwapper : MonoBehaviour
{
    public void SwitchState(GameObject state)
    {
        foreach(Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
        state.SetActive(true);
    }
}
