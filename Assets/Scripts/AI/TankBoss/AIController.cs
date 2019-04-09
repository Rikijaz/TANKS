using UnityEngine;

public struct AIStateData
{
    public GameObject AI;
    public GameObject player;
    public LayerMask playerLayerMask;
    public AIStats AIStats;
    public AIStateData(
        GameObject AI,
        GameObject player,
        LayerMask playerLayerMask,
        AIStats AIStats)
    {
        this.AI = AI;
        this.player = player;
        this.playerLayerMask = playerLayerMask;
        this.AIStats = AIStats;
    }
}

public class AIController : MonoBehaviour
{
    [Header("AI State Data")]
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] LayerMask playerLayerMask;

    private AIStateData AIStateData;

    private AIStats AIStats;

    private Rigidbody AIRigidbody;

    private BGC.StateMachine.StateMachine stateMachine;

    void Awake()
    {
        AIRigidbody = GetComponent<Rigidbody>();

        stateMachine = new BGC.StateMachine.StateMachine();

        EntryState entryState = new EntryState(AIStateData);
        entryState.SetStateMachineFunctions(
            stateMachine.ActivateTrigger,
            stateMachine.GetTrigger,
            stateMachine.GetBool,
            stateMachine.SetBool);

        PatrolState patrolState = new PatrolState(AIStateData, patrolPoints);
        patrolState.SetStateMachineFunctions(
            stateMachine.ActivateTrigger,
            stateMachine.GetTrigger,
            stateMachine.GetBool,
            stateMachine.SetBool);

        ScanState scanState = new ScanState(AIStateData);
        scanState.SetStateMachineFunctions(
            stateMachine.ActivateTrigger,
            stateMachine.GetTrigger,
            stateMachine.GetBool,
            stateMachine.SetBool);

        PursueState pursueState = new PursueState(AIStateData);
        pursueState.SetStateMachineFunctions(
            stateMachine.ActivateTrigger,
            stateMachine.GetTrigger,
            stateMachine.GetBool,
            stateMachine.SetBool);

        AttackState attackState = new AttackState(AIStateData);
        attackState.SetStateMachineFunctions(
            stateMachine.ActivateTrigger,
            stateMachine.GetTrigger,
            stateMachine.GetBool,
            stateMachine.SetBool);

        StunnedState stunnedState = new StunnedState(AIStateData);
        stunnedState.SetStateMachineFunctions(
            stateMachine.ActivateTrigger,
            stateMachine.GetTrigger,
            stateMachine.GetBool,
            stateMachine.SetBool);

        //FleeState fleeState = new FleeState(AIStateData);
        //fleeState.SetStateMachineFunctions(
        //    stateMachine.ActivateTrigger,
        //    stateMachine.GetTrigger,
        //    stateMachine.GetBool,
        //    stateMachine.SetBool);

        stateMachine.AddEntryState(entryState);
        stateMachine.AddState(patrolState);
        stateMachine.AddState(scanState);
        stateMachine.AddState(pursueState);
        stateMachine.AddState(attackState);
        stateMachine.AddState(stunnedState);
        //stateMachine.AddState(fleeState);

        stateMachine.AddBool("shouldPatrol", false);
        stateMachine.AddBool("shouldScan", false);
        stateMachine.AddBool("shouldPursue", false);
        stateMachine.AddBool("shouldAttack", false);
        stateMachine.AddBool("shouldBeStunned", false);
        //stateMachine.AddBool("shouldFlee", false);

        BGC.StateMachine.BoolCondition shouldPatrol =
            new BGC.StateMachine.BoolCondition("shouldPatrol", true);
        stateMachine.AddAnyStateTransition(patrolState, shouldPatrol);

        BGC.StateMachine.BoolCondition shouldScan =
            new BGC.StateMachine.BoolCondition("shouldScan", true);
        stateMachine.AddAnyStateTransition(scanState, shouldScan);

        BGC.StateMachine.BoolCondition shouldPursue =
            new BGC.StateMachine.BoolCondition("shouldPursue", true);
        stateMachine.AddAnyStateTransition(pursueState, shouldPursue);

        BGC.StateMachine.BoolCondition shouldAttack =
            new BGC.StateMachine.BoolCondition("shouldAttack", true);
        stateMachine.AddAnyStateTransition(attackState, shouldAttack);

        BGC.StateMachine.BoolCondition shouldBeStunned =
            new BGC.StateMachine.BoolCondition("shouldBeStunned", true);
        stateMachine.AddAnyStateTransition(stunnedState, shouldBeStunned);

        //BGC.StateMachine.BoolCondition shouldFlee =
        //    new BGC.StateMachine.BoolCondition("shouldFlee", true);
        //stateMachine.AddTransition(stunnedState, fleeState, shouldFlee);
    }

    /// <summary>
    /// Unfreeze AI and start StateMachine
    /// </summary>
    private void OnEnable()
    {
        AIRigidbody.isKinematic = false;
        stateMachine.Start();
    }

    /// <summary>
    /// Freeze AI and reset StateMachine
    /// </summary>
    private void OnDisable()
    {
        AIRigidbody.isKinematic = true;
        stateMachine.Reset();
    }

    /// <summary>
    /// Set player gameobject and instantiate stats
    /// </summary>
    public void Setup(GameObject player)
    {
        AIStats = new AIStats();

        AIStateData = new AIStateData(
            gameObject,
            player,
            playerLayerMask,
            AIStats);
    }

    /// <summary>
    /// Level up the AI
    /// </summary>
    public void LevelUpAI()
    {
        AIStats.LevelUp();
    }

    /// <summary>
    /// Update stateMachine states
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {

        }

        stateMachine.Update();
    }
}
