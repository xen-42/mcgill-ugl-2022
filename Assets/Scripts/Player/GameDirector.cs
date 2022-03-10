using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : NetworkBehaviour
{
    public static GameDirector Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }
}
