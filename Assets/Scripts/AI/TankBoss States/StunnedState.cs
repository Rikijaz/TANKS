using UnityEngine;

public class StunnedState : AIState
{
    protected override string DefaultName { get { return "StunnedState"; } }

    private float stunnedTimer;

    public StunnedState(AIStateData AIStateData) : base(AIStateData)
    {

    }

    /// <summary>
    /// Stop navMeshAgent and reset stunnedTimer
    /// </summary>
    public override void OnEnter()
    {
        SetBool("shouldBeStunned", false);

        navMeshAgent.isStopped = true;

        stunnedTimer = 0f;
    }

    public override void OnExit() { }

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

            SetBool("shouldPursue", true);

            //if (AIHealth.cachedHealth < AIStateData.AIStats.CriticalHealth)
            //{
            //    SetBool("shouldFlee", true);
            //}
            //else
            //{
            //    SetBool("shouldPursue", true);
            //}
        }
    }
}
