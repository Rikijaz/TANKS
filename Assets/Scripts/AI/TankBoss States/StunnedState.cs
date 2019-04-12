using UnityEngine;

public class StunnedState : AIState
{
    protected override string DefaultName { get { return "StunnedState"; } }

    private float stunnedTimer;

    public StunnedState(AIStateData AIStateData) : base(AIStateData)
    {
        //empty
    }

    /// <summary>
    /// Stop navMeshAgent and reset stunnedTimer
    /// </summary>
    public override void OnEnter()
    {
        SetBool(TransitionKey.shouldBeStunned, false);

        AIRigidbody.isKinematic = false;
        navMeshAgent.isStopped = true;
        stunnedTimer = 0f;
    }

    public override void OnExit()
    {
        AIRigidbody.isKinematic = true;
    }

    public override void Update()
    {
        Stunned();
    }

    /// <summary>
    /// Temporarily disable the AI for X duration. Reset rigidbody physics
    /// so the NavMeshAgent can properly operate
    /// </summary>
    private void Stunned()
    {
        stunnedTimer += Time.deltaTime;

        if (stunnedTimer >= AIStateData.AIStats.StunDuration)
        {
            ResetRigidBodyPhysics();

            if (AIHealth.CachedHealth < AIStateData.AIStats.CriticalHealth)
            {
                SetBool(TransitionKey.shouldFlee, true);
            }
            else
            {
                SetBool(TransitionKey.shouldPursue, true);
            }
        }
    }
}
