using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionMaking.StateMachine;

public class SpawningSocks : FSMStateBehaviour
{
    [SerializeField] private CatAgent cat;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Enter()
    {
        Debug.Log("Sock");
    }

    protected override void Execute()
    {
        cat.SpawnSock();
    }

    protected override void Exit()
    {
        Debug.Log("Should never reach here as it is a global state.");
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}