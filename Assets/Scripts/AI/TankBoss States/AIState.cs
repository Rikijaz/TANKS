using UnityEngine;
using UnityEngine.AI;

public abstract class AIState : BGC.StateMachine.State
{
    protected AIStateData AIStateData;

    protected NavMeshAgent navMeshAgent;

    protected TankHealth AIHealth;

    private Rigidbody AIRigidbody;

    protected AIState(AIStateData AIStateData)
    {
        this.AIStateData = AIStateData;

        navMeshAgent = AIStateData.AI.GetComponent<NavMeshAgent>();

        navMeshAgent.speed = AIStateData.AIStats.Speed;
        navMeshAgent.angularSpeed = AIStateData.AIStats.TurnSpeed;

        AIHealth = AIStateData.AI.GetComponent<TankHealth>();

        AIRigidbody = AIStateData.AI.GetComponent<Rigidbody>();
    }

    public override void Update()
    {

    }

    /// <summary>
    /// Determine whether the player is in sight of the Tank Boss
    /// </summary>
    protected bool IsPlayerInSight()
    {
        bool playerSpotted;

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
        if (AIHealth.currentHealth != AIHealth.cachedHealth)
        {
            AIHealth.cachedHealth = AIHealth.currentHealth;
            SetBool("shouldBeStunned", true);
            return true;
        }
        else
        {
            return false;
        }
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
}
