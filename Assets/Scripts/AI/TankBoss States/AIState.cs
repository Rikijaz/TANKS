using UnityEngine;
using UnityEngine.AI;

public abstract class AIState : BGC.StateMachine.State
{
    protected AIStateData AIStateData;
    protected NavMeshAgent navMeshAgent;
    protected TankHealth AIHealth;
    protected Rigidbody AIRigidbody;

    protected AIState(AIStateData AIStateData)
    {
        this.AIStateData = AIStateData;
        navMeshAgent = AIStateData.AI.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = AIStateData.AIStats.Speed;
        navMeshAgent.angularSpeed = AIStateData.AIStats.TurnSpeed;
        AIHealth = AIStateData.AI.GetComponent<TankHealth>();
        AIRigidbody = AIStateData.AI.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Determine whether the player is in sight of the AI
    /// </summary>
    protected bool IsPlayerInSight()
    {
        bool playerSpotted = false;

        RaycastHit playerHit;

        playerSpotted = (Physics.Raycast(
            AIStateData.AI.transform.position,
            AIStateData.AI.transform.TransformDirection(Vector3.forward),
            out playerHit,
            AIStateData.AIStats.SightRange,
            AIStateData.playerLayerMask));

        return playerSpotted;
    }

    /// <summary>
    /// Determine whether the AI is hit by a missle. If true, enter stunned
    /// state
    /// </summary>
    protected bool IsHit()
    {
        bool isHit = false;

        if (AIHealth.CurrentHealth < AIHealth.CachedHealth)
        {
            AIHealth.CachedHealth = AIHealth.CurrentHealth;
            SetBool(TransitionKey.shouldBeStunned, true);
            isHit = true;
        }

        return isHit;
    }

    /// <summary>
    /// Set rigidbody velocity and angular velocity to zero
    /// </summary>
    protected void ResetRigidBodyPhysics()
    {
        AIRigidbody.velocity = Vector3.zero;
        AIRigidbody.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// Get distance to player
    /// </summary>
    protected float DistanceToPlayer()
    {
        return Vector3.Distance(
            AIStateData.player.transform.position,
            AIStateData.AI.transform.position);
    }

    protected bool IsPlayerInRadius(float radius)
    {
        return (DistanceToPlayer() <= radius);
    }

    protected bool ShouldStop(Vector3 destination)
    {
        float distanceToDestination = Vector3.Distance(
            destination, 
            AIStateData.AI.transform.position);

        return (distanceToDestination <= navMeshAgent.stoppingDistance);
    }
}
