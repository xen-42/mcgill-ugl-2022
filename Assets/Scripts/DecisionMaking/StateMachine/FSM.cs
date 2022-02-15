using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DecisionMaking.StateMachine
{
    public class FSM : MonoBehaviour
    {
        #region Blackboard

        private BlackboardManager m_blackboardManager;

        #endregion Blackboard

        #region Caches

        private List<FSMStateBehaviour> m_states;
        [SerializeField] private FSMStateBehaviour m_currentState;
        [SerializeField] private FSMStateBehaviour m_globalState;
        private FSMStateBehaviour m_trigState;

        #endregion Caches

        public FSMStateBehaviour GlobalState { get => m_globalState; }
        public FSMStateBehaviour CurrentState { get => m_currentState; set => m_currentState = value; }

        private void Awake()
        {
            //Caches all the FSM States
            m_states = new List<FSMStateBehaviour>();
            m_states.AddRange(GetComponents<FSMStateBehaviour>());

            //Get Singleton Blackboard
            m_blackboardManager = BlackboardManager.Instance;
            //Assign to non-mono class
        }

        // Start is called before the first frame update
        private void Start()
        {
            //Entering the Initial State
            m_currentState.OnEnter();
            print(m_blackboardManager);
        }

        // Update is called once per frame
        private void Update()
        {
            //First Run the global state
            if (m_globalState != null)
            {
                m_globalState.OnUpdate();

                if ((m_trigState = m_globalState.TriggeredState()) != null)
                {
                    m_globalState.OnExit();
                    CurrentState = m_trigState;
                    CurrentState.OnEnter();
                }
            }

            //If there is a triggered transition in either global state or current State
            if ((m_trigState = CurrentState.TriggeredState()) != null)
            {
                //Exiting the Current State
                CurrentState.OnExit();
                CurrentState = m_trigState;
                //Entering the new State
                CurrentState.OnEnter();
            }

            //Execute the current state
            m_currentState.OnUpdate();
        }
    }
}