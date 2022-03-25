using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionMaking.StateMachine;

/// <summary>
/// <para>The cat will slowdown to sit, turn around to the player, and change its color</para>
/// while being pet, the cat wont spawn any socks
/// </summary>
public class Petting : Sitting
{
    protected override void Enter()
    {
        base.Enter();
    }

    protected override void Execute()
    {
        base.Execute();
    }

    protected override void Exit()
    {
        base.Exit();
    }
}