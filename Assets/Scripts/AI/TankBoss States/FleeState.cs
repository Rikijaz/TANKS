using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleeState : AIState
{
    protected override string DefaultName { get { return "FleeState"; } }

    private const float fleeRadius = 40f;
    private const float fleeSearchRange = 14f;
    private const float playerAvoidanceRadius = 3.5f;
    private const float coverSearchRadius = 5f;
    private const float sampleCoverPositionRange = 4f;

    private static readonly Vector3[] sampleCoverPositions = new Vector3[]
    {
        Vector3.forward * sampleCoverPositionRange,
        (Vector3.forward + Vector3.left) * sampleCoverPositionRange,
        Vector3.left * sampleCoverPositionRange,
        (Vector3.forward + Vector3.right)  * sampleCoverPositionRange,
        Vector3.right * sampleCoverPositionRange,
    };

    private bool isFleeing;

    public FleeState(AIStateData AIStateData) : base(AIStateData)
    {
        // empty
    }

    /// <summary>
    /// Resume navMeshAgent
    /// </summary>
    public override void OnEnter()
    {
        SetBool(TransitionKey.shouldFlee, false);

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
    /// Attempt to flee. If arrived at safe location, player is not in the HealRadius
    /// and needs healing, enter heal state. Else, keep moving to safe location.
    /// If there are no safe locations and the player is in the HealRadius, fight
    /// back
    /// </summary>
    private void Flee()
    {
        Vector3 fleePosition = Vector3.zero;
        bool canFlee = CanFlee(ref fleePosition);

        if (NeedsHealing() && 
            ShouldStop(navMeshAgent.destination) &&
            !IsPlayerInRadius(AIStats.HealRadius))
        {
            SetBool(TransitionKey.shouldHeal, true);
        }
        else if (canFlee && ShouldStop(navMeshAgent.destination))
        {
            navMeshAgent.SetDestination(fleePosition);
        }
        else if (IsPlayerInRadius(AIStats.HealRadius))
        {
            if (canFlee)
            {
                navMeshAgent.SetDestination(fleePosition);
            }
            else
            {
                SetBool(TransitionKey.shouldPursue, true);
            }
        }
    }

    /// <summary>
    /// Determines whether there is a viable flee position. If viable flee position 
    /// found, set it. Then seek viable cover positions and choose the one furthest 
    /// from the player. If a viable cover position is not found/set then use original
    /// flee position and check whether it is a valid path
    /// </summary>
    private bool CanFlee(ref Vector3 fleePosition)
    {
        bool canFlee = false;

        if (IsPlayerInRadius(fleeRadius))
        {
            Vector3 directionToPlayer =
                AIStateData.AI.transform.position - AIStateData.player.transform.position;
            fleePosition = AIStateData.AI.transform.position + directionToPlayer;

            NavMeshHit navMeshHit;

            bool fleeSpotFound = NavMesh.SamplePosition(
                fleePosition, 
                out navMeshHit,
                fleeRadius, 
                NavMesh.AllAreas);

            if (fleeSpotFound)
            {
                fleePosition = navMeshHit.position;
                canFlee = (SetBestCoverPosition(ref fleePosition) || 
                    IsPlayerNearPathToDestination(fleePosition));
            }
        }

        return canFlee;
    }

    /// <summary>
    /// Given the flee position, find all colliders within coverSearchRadius. Parse 
    /// through the colliders and determine viable cover positions. Choose the 
    /// cover position that is furthest from the player. If no cover positions 
    /// are found, return default flee position
    /// </summary>
    private bool SetBestCoverPosition(ref Vector3 fleePosition)
    {
        bool coverPositionFound = false;

        Collider[] hitColliders = Physics.OverlapSphere(
            fleePosition,
            coverSearchRadius);

        List<Vector3> possibleCoverPositions = PossibleCoverPositions(hitColliders);

        if (possibleCoverPositions.Count > 0)
        {
            Vector3 displacement =
                AIStateData.player.transform.position - AIStateData.AI.transform.position;
            possibleCoverPositions.Sort(SortByDistance);
            fleePosition = possibleCoverPositions[possibleCoverPositions.Count - 1];
            coverPositionFound = true;
        }
        
        return coverPositionFound;
    }

    /// <summary>
    /// Given each collider, determine the nearest edge from North, West, and 
    /// East of collider. If nearest edge found, then determine if a player is 
    /// near the path to the edge and if it is a viable cover. If true the 
    /// player is not near the path and it is viable cover, add it to the list
    /// </summary>
    private List<Vector3> PossibleCoverPositions(Collider[] hitColliders)
    {
        List<Vector3> possibleCoverPositions = new List<Vector3>();
        Vector3 displacement = 
            AIStateData.player.transform.position - AIStateData.AI.transform.position;

        for (var i = 0; i < hitColliders.Length; ++i)
        {
            Vector3 currentColliderPosition = hitColliders[i].transform.position;
            NavMeshHit hit;

            for (var j = 0; j < sampleCoverPositions.Length; ++j)
            {
                Vector3 samplePosition = currentColliderPosition + sampleCoverPositions[j];

                bool nearestEdgeFound = 
                    NavMesh.FindClosestEdge(samplePosition, out hit, NavMesh.AllAreas);

                if (nearestEdgeFound && 
                    IsViableCover(hit.normal, displacement) && 
                    !IsPlayerNearPathToDestination(hit.position))
                {
                    if (!possibleCoverPositions.Contains(hit.position))
                    {
                        possibleCoverPositions.Add(hit.position);
                    }
                }
            }
        }

        return possibleCoverPositions;
    }

    /// <summary>
    /// Determine whether if the player is near the path to a certain destination.
    /// </summary>
    private bool IsPlayerNearPathToDestination(Vector3 destination)
    {
        bool isPlayerNearPath = false;

        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(destination, path);

        for (var i = 0; i < path.corners.Length && !isPlayerNearPath; ++i)
        {
            float distanceToPlayer = Vector3.Distance(
                path.corners[i],
                AIStateData.player.transform.position);

            if (distanceToPlayer <= playerAvoidanceRadius)
            {
                isPlayerNearPath = true;
            }
        }

        return isPlayerNearPath;
    }

    /// <summary>
    /// Determine whether the normal of the vector is pointing away from the player.
    /// If pointing away from the enemy, it is viable cover
    /// </summary>
    private bool IsViableCover(Vector3 normal, Vector3 displacement)
    {
        return (Vector3.Dot(normal, displacement) < AIStateData.AIStats.CoverQuality);
    }

    /// <summary>
    /// Determine whether the AI needs to heal. If health is in the critical zone,
    /// heal
    /// </summary>
    private bool NeedsHealing()
    {
        bool needsHealing = false;

        if (AIHealth.CurrentHealth < AIStateData.AIStats.CriticalHealth)
        {
            needsHealing = true;
        }

        return needsHealing;
    }

    private int SortByDistance(Vector3 a, Vector3 b)
    {
        float distanceToA = Vector3.Distance(
            a,
            AIStateData.player.transform.position);
        float distanceToB = Vector3.Distance(
            b,
            AIStateData.player.transform.position);

        return distanceToA.CompareTo(distanceToB);
    }
}