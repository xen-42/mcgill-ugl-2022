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
    private CatInteractable m_petInteract;
    private MeshRenderer m_renderer;

    [SerializeField] private int energy;

    // Movement
    //private float speed = 1f;
    private float[] x_limits = new float[] { 18f, 2f };
    private float[] z_limits = new float[] { -18f, -3.5f };

    private Vector3 destination;
    private UnityEngine.AI.NavMeshAgent NMAgent;
    private UnityEngine.AI.NavMeshPath path;
    private float elapsed = 0.0f;

    [Header("Cat Moving Speed Params")]
    [SerializeField] private float m_maxSpeed = 2f;
    [SerializeField] private float m_speedLerpFactor = .5f;

    [Header("Cat Sitting State Params")]
    [SerializeField] private int m_energyIncreasingAmount = 3;

    private bool m_arrivedCurrentPath;
    private bool m_reachedMaxSpeed;

    [Header("Cat Spawning Socks Params")]
    [SerializeField] private GameObject m_sockPrefab;
    [SerializeField] private SpawnPointsManager m_spawnManager;
    [SerializeField] private float m_spawnRadius = 3f;
    [SerializeField] [Range(0, 10)] public int m_spawnLimit;
    [SerializeField] [Range(0, 1)] private float m_spawnProbility = .1f;
    [SyncVar] private int m_curSpawnNum = 0;

    [Header("Cat Petting Params")]
    [Tooltip("When the cat  is being pet, the rendering color will be changed.")]
    [SerializeField] private Color m_petColor;

    [Tooltip("When the cat turns around to face you")]
    [SerializeField] private float m_petAngularSpeed;

    [Tooltip("When the cat approches you")]
    [SerializeField] private float m_petLinearSpeed;

    [SerializeField] private float m_petStoppingDistance;

    [SerializeField] public float sockSpawnCooldown = 20f;
    [SyncVar] private float _sockTimer = 0f;

    private Color m_normalColor;
    private Player m_petter;

    public static CatAgent Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        Debug.Log($"Cat awake {(isServer ? "on server" : "on client")}");

        NMAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        NMAgent.speed = 0;
        m_fsm = GetComponent<FSM>();
        m_manager = BlackboardManager.Instance;
        m_petInteract = GetComponent<CatInteractable>();
        if (m_spawnManager == null)
            m_spawnManager = GameObject.Find("Waypoints").GetComponent<SpawnPointsManager>();
        destination = this.transform.position;

        m_renderer = GetComponent<MeshRenderer>();
        EventManager.AddListener("PetCat", OnUpdatePetStatus);

        NetworkClient.RegisterPrefab(m_sockPrefab);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener("PetCat", OnUpdatePetStatus);
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (isServer)
        {
            m_fsm.TurnOn();

            energy = 5;

            //path = new UnityEngine.AI.NavMeshPath();
            destination = new Vector3(10f, 1.77f, -4f);
            //elapsed = 0.0f;
        }

        m_normalColor = m_renderer.material.color;
    }

    private void FixedUpdate()
    {
        if (!isServer) return;

        m_manager.SetInteger("Energy", energy);

        if (_sockTimer > 0)
        {
            _sockTimer -= Time.deltaTime;
        }
    }

    #region On pet
    public void OnUpdatePetStatus()
    {
        if (isServer)
        {
            ServerUpdatePetStatus(true);
        }
        else
        {
            Debug.Log("Client pet the cat");
            Player.Instance.DoWithAuthority(netIdentity, CmdUpdatePetStatus);
        }
    }

    [Server]
    private void ServerUpdatePetStatus(bool serverPlayer)
    {
        Debug.Log($"Cat was pet by {(serverPlayer ? "server" : "client")}");
        m_petter = serverPlayer ? Player.Instance : Player.OtherPlayer;
        m_manager.SetTrigger("OnPet");
        print("OnPet: " + m_manager.GetTrigger("OnPet"));
    }

    [Command]
    private void CmdUpdatePetStatus()
    {
        // This was called from the client
        ServerUpdatePetStatus(false);
    }
    #endregion On pet

    // Called when we first enter the 'Walking' state
    public void EnterWalk()
    {
        //transform.position += new Vector3(0,0,0.5f) * Time.deltaTime * speed;

        NMAgent.enabled = true;
        m_reachedMaxSpeed = false;
        m_arrivedCurrentPath = false;

        //NMAgent.speed = 2;

        PickRandomPos();

        /*
        elapsed += Time.deltaTime;
        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;
            UnityEngine.AI.NavMesh.CalculatePath(transform.position, destination, UnityEngine.AI.NavMesh.AllAreas, path);
        }*/
    }

    public void EnterSit()
    {
        //NMAgent.enabled = false;
    }

    public void Walk()
    {
        energy -= 2;

        m_arrivedCurrentPath = NMAgent.remainingDistance <= NMAgent.stoppingDistance;
        m_reachedMaxSpeed = (m_maxSpeed - NMAgent.speed) <= float.Epsilon;
        if (!m_reachedMaxSpeed && !m_arrivedCurrentPath)
        {
            NMAgent.speed = Mathf.Lerp(NMAgent.speed, m_maxSpeed, m_speedLerpFactor);
        }

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
        energy += m_energyIncreasingAmount;
        if ((NMAgent.speed = Mathf.Lerp(NMAgent.speed, 0f, m_speedLerpFactor)) < .001f)
        {
            SetStillState();
        }
    }

    public void ExitSit()
    {
        ResetStillState();
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

    #region Spawn sock
    public void SpawnSock()
    {
        if (isServer)
        {
            ServerSpawnSock();
        }
        else
        {
            Player.Instance.DoWithAuthority(netIdentity, ServerSpawnSock);
        }
    }

    [Command]
    private void CmdSpawnSock()
    {
        ServerSpawnSock();
    }

    [Server]
    private void ServerSpawnSock()
    {
        if (m_curSpawnNum < m_spawnLimit)
        {
            if (_sockTimer > 0f) return;

            if (Random.Range(0, 1f) > m_spawnProbility) return;

            m_curSpawnNum++;
            //Vector2 spawnOffset = Random.insideUnitCircle * m_spawnRadius;
            //Vector3 spawnPos = transform.position + new Vector3(spawnOffset.x, 0, spawnOffset.y);

            Vector3 spawnPos;
            if (m_spawnManager.TryQueryNeighbour(transform.position, out spawnPos))
            {
                var sock = Instantiate(m_sockPrefab, spawnPos, m_sockPrefab.transform.rotation);
                sock.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
                NetworkServer.Spawn(sock);
                _sockTimer = sockSpawnCooldown;
            }
        }
    }
    #endregion Spawn sock

    public void EnterPet()
    {
        m_arrivedCurrentPath = false;
        //NMAgent.stoppingDistance = m_petStoppingDistance;
    }

    public void Pet()
    {
        if (!m_arrivedCurrentPath)
        {
            NMAgent.speed = Mathf.Lerp(NMAgent.speed, m_petLinearSpeed, m_speedLerpFactor);
            NMAgent.destination = m_petter.transform.position;
            if (m_arrivedCurrentPath = (NMAgent.remainingDistance < m_petStoppingDistance)) //Should modify in the future
            {
                NMAgent.speed = 0;
                SetStillState();
                StartCoroutine(nameof(FacePlayer));
            }
        }
        //else
        //{
        //    transform.forward = Vector3.Slerp(transform.forward, m_petter.transform.position - transform.position, .1f);
        //}
    }

    public void ExitPet()
    {
        m_petter = null;
        ResetStillState();
        NMAgent.stoppingDistance = 0f;
        StopCoroutine(nameof(FacePlayer));
    }

    private IEnumerator FacePlayer()
    {
        while (true)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, m_petter.transform.position - transform.position, NMAgent.angularSpeed * Time.deltaTime * Mathf.PI / 180f, 1f);
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetStillState()
    {
        m_renderer.material.color = m_petColor;
    }

    public void ResetStillState()
    {
        m_renderer.material.color = m_normalColor;
    }

    public int GetNumberOfSocks()
    {
        return m_curSpawnNum;
    }

    public void OnSockReturned()
    {
        Debug.Log($"Sock returned, there were {m_curSpawnNum}");
        if (isServer)
        {
            m_curSpawnNum -= 1;
        }
        else
        {
            Player.Instance.DoWithAuthority(netIdentity, CmdOnSockReturned);
        }
    }

    [Command]
    public void CmdOnSockReturned()
    {
        m_curSpawnNum -= 1;
    }
}