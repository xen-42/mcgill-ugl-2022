using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionMaking.StateMachine;

public class Sitting : FSMStateBehaviour
{
    private CatAgent cat;

    protected override void Enter()
    {
        print("Entered: Sitting");
        cat = GameObject.Find("Cat").GetComponent<CatAgent>();

        cat.EnterSit();
    }

    protected override void Execute()
    {
        cat.Sit();
    }

    protected override void Exit()
    { }
}