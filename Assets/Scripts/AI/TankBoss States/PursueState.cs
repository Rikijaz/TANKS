using UnityEngine;

public class PursueState : AIState
{
    protected override string DefaultName { get { return "PursueState"; } }

    public PursueState(AIStateData AIStateData) : base(AIStateData)
    {
        //empty
    }

    /// <summary>
    /// Resume navMeshAgent
    /// </summary>
    public override void OnEnter()
    {
        SetBool(TransitionKey.shouldPursue, false);

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
    /// Else, if close to the player, face the player. Otherwise, continue 
    /// pursuing the player.
    /// </summary>
    private void Pursue()
    {
        if (IsPlayerInRadius(AIStateData.AIStats.AttackRange) && IsPlayerInSight())
        {
            SetBool(TransitionKey.shouldAttack, true);
        }
        else
        {
            if (ShouldStop(AIStateData.player.transform.position))
            {
                FacePlayer();
            }
            else
            {
                navMeshAgent.destination = AIStateData.player.transform.position;
            }
        }
    }

    /// <summary>
    /// Rotate to face the player
    /// </summary>
    private void FacePlayer()
    {
        Quaternion rotation = Quaternion.LookRotation(
            AIStateData.player.transform.position - AIStateData.AI.transform.position);
        float turn = AIStateData.AIStats.TurnSpeed * Time.deltaTime;

        AIStateData.AI.transform.rotation = Quaternion.Slerp(
            AIStateData.AI.transform.rotation,
            rotation,
            turn);
    }
}
