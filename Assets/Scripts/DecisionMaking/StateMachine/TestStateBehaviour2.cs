using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionMaking.StateMachine;

public class TestStateBehaviour2 : FSMStateBehaviour
{
    protected override void Enter()
    {
        print("Entered" + nameof(TestStateBehaviour2));
    }

    protected override void Execute()
    {
        print("In" + nameof(TestStateBehaviour2));
    }

    protected override void Exit()
    {
        print("Exit" + nameof(TestStateBehaviour2));
    }
}