using UnityEngine;
using UnityEngine.AI;

public class FleeState : AIState
{
    protected override string DefaultName { get { return "FleeState"; } }

    public FleeState(AIStateData AIStateData) : base(AIStateData)
    {

    }

    private static readonly float fleeRadius = 40f;

    /// <summary>
    /// Resume navMeshAgent
    /// </summary>
    public override void OnEnter()
    {
        SetBool("shouldFlee", false);
        navMeshAgent.isStopped = false;
    }

    /// <summary>
    /// Stop navMeshAgent and reset stunnedTimer
    /// </summary>
    public override void OnExit()
    {
        navMeshAgent.isStopped = true;
    }

    /// <summary>
    /// If hit by missle, enter stunned state. Else, flee.
    /// </summary>
    public override void Update()
    {
        if (!IsHit())
        {
            Flee();
        }
    }

    /// <summary>
    /// If the player is outside the flee radius, don't move. Else, keep fleeing
    /// from player. If fleeing is impossible, start pursuing player
    /// </summary>
    private void Flee()
    {
        if (DistanceToPlayer() < fleeRadius)
        {
            Vector3 directionToPlayer = 
                AIStateData.AI.transform.position - AIStateData.player.transform.position;
            Vector3 fleePosition = 
                AIStateData.AI.transform.position + directionToPlayer;
            
            float distanceToFleePosition = Vector3.Distance(
                fleePosition,
                AIStateData.AI.transform.position);

            NavMeshHit navMeshHit;

            if (NavMesh.SamplePosition(fleePosition, out navMeshHit, 30f, NavMesh.AllAreas) &&
                distanceToFleePosition > 1f)
            {
                navMeshAgent.SetDestination(fleePosition);
            }
            else 
            {
                SetBool("shouldPursue", true);
            }
        }
        else
        {
            navMeshAgent.velocity = Vector3.zero;
        }
    }

}
