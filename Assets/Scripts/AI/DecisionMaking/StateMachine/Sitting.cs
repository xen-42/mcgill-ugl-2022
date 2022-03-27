using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionMaking.StateMachine;

public class Sitting : FSMStateBehaviour
{
    protected CatAgent cat;

    protected override void Awake()
    {
        base.Awake();
        cat = GetComponent<CatAgent>();
    }

    protected override void Enter()
    {
        print("Entered: Sitting");

        cat.EnterSit();
    }

    protected override void Execute()
    {
        cat.Sit();
    }

    protected override void Exit()
    { }
}