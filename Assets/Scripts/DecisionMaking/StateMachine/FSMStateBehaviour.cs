using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace DecisionMaking.StateMachine
{
    /// <summary>
    /// FSMStateBehaviour can be attached to a Finite State Machine. It's the base class every StateMachineBehaviour attached to a State Machine derived from.
    /// </summary>
    [RequireComponent(typeof(FSM))]
    public abstract class FSMStateBehaviour : StateBehaviour
    {
        [Serializable]
        protected class Transition : IComponentReveiver<BlackboardManager>, IComponentReveiver<FSMStateBehaviour>
        {
            internal BlackboardManager m_blackboardManager;

            internal FSMStateBehaviour m_fromState;
            [SerializeField] internal FSMStateBehaviour m_toState;

            [Tooltip("The transition is evaluated valid only after the from state behaviour is completed.")]
            [SerializeField] internal bool m_afterCompletion;

            internal void LoadBlackboardConditions()
            {
                m_andCondition = new AndCondition();

                intConditions.ForEach(cond => cond.AddComponent(m_blackboardManager));
                floatConditions.ForEach(cond => cond.AddComponent(m_blackboardManager));
                boolConditions.ForEach(cond => cond.AddComponent(m_blackboardManager));
                triggerConditions.ForEach(cond => cond.AddComponent(m_blackboardManager));

                m_andCondition.conditions.AddRange(intConditions);
                m_andCondition.conditions.AddRange(floatConditions);
                m_andCondition.conditions.AddRange(boolConditions);
                m_andCondition.conditions.AddRange(triggerConditions);
            }

            #region Inspector GUI

            public List<IntBlackboardCondition> intConditions;
            public List<FloatBlackboardCondition> floatConditions;
            public List<BoolBlackboardCondition> boolConditions;
            public List<TriggerBlackboardCondition> triggerConditions;

            #endregion Inspector GUI

            internal AndCondition m_andCondition;

            public bool IsValid() => (!m_afterCompletion || m_fromState.IsComplete()) && m_andCondition.IsValid();

            public FSMStateBehaviour FromState => m_toState;
            public FSMStateBehaviour ToState => m_toState;

            public BlackboardManager AddComponent(BlackboardManager component)
            {
                m_blackboardManager = component;
                return m_blackboardManager;
            }

            public FSMStateBehaviour AddComponent(FSMStateBehaviour component)
            {
                return m_fromState = component;
            }
        }

        [SerializeField] protected FSM m_stateMachine;
        [SerializeField] protected List<Transition> m_transitions;

        protected override void Awake()
        {
            base.Awake();

            foreach (var trans in m_transitions)
            {
                trans.AddComponent(m_blackboardManager);
                trans.AddComponent(this);
                trans.LoadBlackboardConditions();
            }
        }

        public void OnEnter()
        {
            if (isActive)
                Enter();
        }

        public void OnExit()
        {
            if (isActive)
                Exit();
        }

        public FSMStateBehaviour TriggeredState()
        {
            Transition triggered;
            triggered = m_transitions.FirstOrDefault(transition => transition.IsValid());

            return (triggered == null) ? null : triggered.ToState;
        }

        protected abstract void Enter();

        protected abstract void Exit();

        //    public void AddTransition(Transition pTrans) => m_transitions.Add(pTrans);
        //    public void RemoveTransition(Transition pTrans) => m_transitions.Remove(pTrans);
    }
}