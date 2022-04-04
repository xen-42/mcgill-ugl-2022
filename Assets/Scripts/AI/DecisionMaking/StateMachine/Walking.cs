using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionMaking.StateMachine;

public class Walking : FSMStateBehaviour
{
    [SerializeField] private CatAgent cat;

    protected override void Enter()
    {
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