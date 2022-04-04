using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionMaking.StateMachine;

public class Sitting : FSMStateBehaviour
{
    [SerializeField] protected CatAgent cat;

    protected override void Awake()
    {
        base.Awake();
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
    {
        cat.ExitSit();
    }
}