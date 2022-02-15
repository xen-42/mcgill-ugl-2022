using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace DecisionMaking
{
    /// <summary>
    /// StateBehaviour is the base class used by Decision Maker.
    /// </summary>
    public abstract class StateBehaviour : MonoBehaviour
    {
        protected BlackboardManager m_blackboardManager;

        protected virtual void Awake()
        {
            m_blackboardManager = BlackboardManager.Instance;
        }

        [Tooltip("Is the State Behaviour activated")]
        public bool isActive = true;

        /// <summary>
        /// Is the State Execution completed
        /// </summary>
        /// <returns></returns>
        public virtual bool IsComplete() => false;

        /// <summary>
        /// Execute the State
        /// </summary>
        public void OnUpdate()
        {
            if (isActive && !IsComplete())
                Execute();
        }

        protected abstract void Execute();

        public T GetStateBehaviour<T>() where T : StateBehaviour => GetComponents<StateBehaviour>().First(state => state is T) as T;

        public T[] GetStateBehaviours<T>() where T : StateBehaviour => GetComponents<T>();
    }

    /// <summary>
    /// Compound Form of the StateBehaviour
    /// </summary>
    public class CompoundStateBehaviour : StateBehaviour
    {
        protected LinkedList<StateBehaviour> m_states;

        public override bool IsComplete() => m_states.All(action => action.IsComplete());

        protected override void Execute()
        {
            if (m_states.Count == 0)
                return;

            foreach (var state in m_states)
                state.OnUpdate();
        }

        public void AddState(StateBehaviour pState) => m_states.AddLast(pState);

        public void RemoveState(StateBehaviour pState) => m_states.Remove(pState);

        public void RemoveCompleted()
        {
            foreach (var state in m_states)
            {
                if (state.IsComplete())
                    m_states.Remove(state);
            }
        }
    }

    /// <summary>
    /// Sequence Form of the State Behaviour
    /// </summary>
    public class SequenceStateBehaviour : StateBehaviour
    {
        protected LinkedList<StateBehaviour> m_states;
        protected LinkedListNode<StateBehaviour> m_curStateNode = null;

        protected override void Execute()
        {
            if (m_curStateNode == null || !m_curStateNode.Value.isActive)
                return;

            StateBehaviour m_curState = m_curStateNode.Value;
            if (m_curState.IsComplete())
                m_curStateNode = m_curStateNode.Next;

            m_curStateNode.Value.OnUpdate();
        }

        public override bool IsComplete() => m_curStateNode == null;
    }
}