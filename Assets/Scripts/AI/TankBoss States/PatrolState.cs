using UnityEngine;

public class PatrolState : AIState
{
    protected override string DefaultName { get { return "PatrolState"; } }

    private Transform[] patrolPoints;
    private int currentPatrolPoint;

    public PatrolState(
        AIStateData AIStateData, 
        Transform[] patrolPoints) : base(AIStateData)
    {
        this.patrolPoints = patrolPoints;
        currentPatrolPoint = 0;
    }

    /// <summary>
    /// Resume NavMeshAgent and set current patrol point
    /// </summary>
    public override void OnEnter()
    {
        SetBool(TransitionKey.shouldPatrol, false);

        navMeshAgent.isStopped = false;
    }

    /// <summary>
    /// Stop NavMeshAgent.
    /// </summary>
    public override void OnExit()
    {
        navMeshAgent.isStopped = true;
    }

    /// <summary>
    /// If hit by missle, enter stunned state. Else, if the player is within the
    /// alert radius pursue the player. Otherwise, patrol the waypoints
    /// </summary>
    public override void Update()
    {
        if (!IsHit())
        {
            if (IsPlayerInRadius(AIStateData.AIStats.AlertRadius))
            {
                SetBool(TransitionKey.shouldPursue, true);
            }
            else
            {
                Patrol();
            }
        }
    }

    /// <summary>
    /// If the distance to the current waypoint is than 1 unit, then go to next
    /// waypoint. Else, continue to the current waypoint.
    /// </summary>
    private void Patrol()
    {
        if (ShouldStop(patrolPoints[currentPatrolPoint].position))
        {
            currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
            SetBool(TransitionKey.shouldScan, true);
        }
        else
        {
            navMeshAgent.destination = patrolPoints[currentPatrolPoint].position;
        }
    }
}
