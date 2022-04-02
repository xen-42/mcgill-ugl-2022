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
    #region Caches

    private SpawningSocks m_ss;

    #endregion Caches

    [SerializeField] private float m_exitTime;
    private float m_timeElapsed;

    protected override void Awake()
    {
        base.Awake();
        m_ss = GetComponent<SpawningSocks>();
    }

    protected override void Enter()
    {
        //base.Enter();
        cat.EnterPet();
        m_ss.isActive = false;
    }

    protected override void Execute()
    {
        //base.Execute();
        cat.Pet();
        m_timeElapsed += m_stateMachine.TimeElapsed;
    }

    protected override void Exit()
    {
        // base.Exit();
        cat.ExitPet();
        m_ss.isActive = true;
        m_timeElapsed = 0f;
    }

    public override bool IsComplete() => m_timeElapsed > m_exitTime;
}