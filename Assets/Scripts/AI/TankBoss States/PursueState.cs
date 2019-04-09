public class PursueState : AIState
{
    protected override string DefaultName { get { return "PursueState"; } }

    public PursueState(AIStateData AIStateData) : base(AIStateData)
    {

    }

    /// <summary>
    /// Resume navMeshAgent
    /// </summary>
    public override void OnEnter()
    {
        SetBool("shouldPursue", false);
        navMeshAgent.isStopped = false;
    }

    /// <summary>
    /// Stop navMeshAgent
    /// </summary>
    public override void OnExit()
    {
        navMeshAgent.isStopped = true;
    }

    /// <summary>
    /// If hit by missle, enter stunned state. Else, pursue the player
    /// </summary>
    public override void Update()
    {
        if (!IsHit())
        {
            Pursue();
        }
    }

    /// <summary>
    /// If the player is within attack range and is in sight, attack the player.
    /// Else, continue pursuing the player.
    /// </summary>
    private void Pursue()
    {
        if ((DistanceToPlayer() <= AIStateData.AIStats.AttackRange) && IsPlayerInSight())
        {
            SetBool("shouldAttack", true);
        }
        else
        {
            navMeshAgent.destination = AIStateData.player.transform.position;
        }
    }
}
