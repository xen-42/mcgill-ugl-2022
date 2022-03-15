using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionMaking.StateMachine;

public class Walking : FSMStateBehaviour
{
    private CatAgent cat;
    protected override void Enter()
    {
        print("Entered: Walking");
        cat = GameObject.Find("Cat").GetComponent<CatAgent>();
        cat.EnterWalk();
    }

    protected override void Execute()
    {
        cat.Walk();
    }

    protected override void Exit()
    {
    }
}