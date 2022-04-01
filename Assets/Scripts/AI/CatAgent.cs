using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionMaking.StateMachine;
using Mirror;

[RequireComponent(typeof(FSM), typeof(FSMStateBehaviour))]
public class CatAgent : NetworkBehaviour
{
    //Caches
    private FSM m_fsm;
    private BlackboardManager m_manager;

    [SerializeField] private int energy;

    // Movement
    //private float speed = 1f;
    private float[] x_limits = new float[] { 18f, 2f };
    private float[] z_limits = new float[] { -18f, -3.5f };

    private Vector3 destination;
    private UnityEngine.AI.NavMeshAgent NMAgent;
    private UnityEngine.AI.NavMeshPath path;
    private float elapsed = 0.0f;

    // Cat's components
    private Rigidbody rb;
    private Collider collider;

    private void Awake()
    {
        m_fsm = GetComponent<FSM>();
        m_manager = BlackboardManager.Instance;

        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        destination = this.transform.position;
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_fsm.TurnOn();

        energy = 5;
        NMAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //path = new UnityEngine.AI.NavMeshPath();
        destination = new Vector3(10f, 1.77f, -4f);
        //elapsed = 0.0f;
    }

    private void FixedUpdate()
    {
        m_manager.SetInteger("Energy", energy);
    }

    // Called when we first enter the 'Walking' state
    public void EnterWalk()
    {
        //transform.position += new Vector3(0,0,0.5f) * Time.deltaTime * speed;

        NMAgent.enabled = true;
        NMAgent.speed = 2;

        PickRandomPos();

        /*
        elapsed += Time.deltaTime;
        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;
            UnityEngine.AI.NavMesh.CalculatePath(transform.position, destination, UnityEngine.AI.NavMesh.AllAreas, path);
        }*/
    }

    public void Walk()
    {
        energy -= 2;

        // Reached destination, needs to find new one
        if (NMAgent.remainingDistance <= NMAgent.stoppingDistance)
        {
            if (!NMAgent.hasPath || NMAgent.velocity.sqrMagnitude == 0f)
            {
                PickRandomPos();
            }
        }

    }

    public void Sit()
    {
        energy += 10;

        //NMAgent.speed = 0;
    }

    public void PickRandomPos()
    {
        int walk_radius = 20;
        Vector3 random_dir = Random.insideUnitSphere * walk_radius;
        random_dir += this.transform.position;
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(random_dir, out hit, walk_radius, 1);
        NMAgent.destination = hit.position;
    }
}
