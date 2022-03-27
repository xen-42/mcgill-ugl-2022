using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DecisionMaking.StateMachine
{
    public enum UpdateMode
    {
        Update, FixedUpdate, TimeStep
    }

    public class FSM : MonoBehaviour
    {
        public UpdateMode mode = UpdateMode.Update;

        [Tooltip("how often the state machine executes in seconds")]
        [SerializeField] private float m_executionTimeStep = .02f;

        #region Caches

        private BlackboardManager m_blackboardManager;
        private List<FSMStateBehaviour> m_states;

        private FSMStateBehaviour m_trigState;
        private FSMStateBehaviour m_curState;

        [Tooltip("The Current State the state machine is at")]
        [SerializeField] private FSMStateBehaviour m_initialState;

        [Tooltip("The Global State in the state machine. Global State will be executed and test its transitions before the current state in each executation.\n Only OnUpdate() and OnExit() will be called by the State Machine.")]
        [SerializeField] private FSMStateBehaviour m_globalState;

        #endregion Caches

        public float TimeElapsed
            => mode switch
            {
                UpdateMode.FixedUpdate => Time.fixedDeltaTime,
                UpdateMode.TimeStep => m_executionTimeStep,
                UpdateMode.Update => Time.deltaTime,
                _ => throw new System.NotImplementedException(),
            };

        protected bool m_isRunning;
        public bool IsRunning => m_isRunning;

        public FSMStateBehaviour GlobalState { get => m_globalState; set => m_globalState = value; }
        public FSMStateBehaviour InitialState { get => m_initialState; set => m_initialState = value; }
        public FSMStateBehaviour CurrentState { get => m_curState; set => m_curState = value; }

        public T GetFSMStateBehaviour<T>() where T : FSMStateBehaviour
        {
            foreach (var st in m_states)
                if (st is T stt)
                    return stt;

            return null;
        }

        public T[] GetFSMStateBehaviours<T>() where T : FSMStateBehaviour
        {
            return (T[])m_states.FindAll(state => state is T).ToArray();
        }

        /// <summary>
        /// Turn the State Machine On
        /// </summary>
        public void TurnOn()
        {
            if (!m_isRunning)
            {
                //Entering the Current State
                m_curState.OnEnter();

                StartCoroutine(nameof(StateMachineRoutine));
                m_isRunning = true;
            }
        }

        /// <summary>
        /// Turn the State Machine Off and Reset the Current State to Initial State
        /// </summary>
        public void TurnOff()
        {
            if (m_isRunning)
            {
                StopCoroutine(nameof(StateMachineRoutine));
                m_isRunning = false;
                m_curState = m_initialState;
            }
        }

        /// <summary>
        /// Pause the State Machine
        /// </summary>
        public void Pause()
        {
            if (m_isRunning)
            {
                StopCoroutine(nameof(StateMachineRoutine));
                m_isRunning = false;
            }
        }

        private void Awake()
        {
            //Caches all the FSM States
            m_states = new List<FSMStateBehaviour>();
            m_states.AddRange(GetComponents<FSMStateBehaviour>());

            //Get Singleton Blackboard
            m_blackboardManager = BlackboardManager.Instance;

            //SetUp State
            m_curState = m_initialState;
        }

        // Start is called before the first frame update
        private void Start()
        {
        }

        private IEnumerator StateMachineRoutine()
        {
            while (true)
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

                //Execute the current state
                CurrentState.OnUpdate();

                //If there is a triggered transition in either global state or current State
                if ((m_trigState = CurrentState.TriggeredState()) != null)
                {
                    //Exiting the Current State
                    CurrentState.OnExit();
                    CurrentState = m_trigState;
                    //Entering the new State
                    CurrentState.OnEnter();
                }

                switch (mode)
                {
                    case UpdateMode.Update:
                        yield return null;
                        break;

                    case UpdateMode.FixedUpdate:
                        yield return new WaitForFixedUpdate();
                        break;

                    case UpdateMode.TimeStep:
                        yield return new WaitForSeconds(m_executionTimeStep);
                        break;
                }
            }
        }

        private void OnDrawGizmos()
        {
            //Show current state
            if (CurrentState != null)
            {
                Handles.Label(transform.position, CurrentState.ToString());
            }
        }
    }
}