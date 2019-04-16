using UnityEngine;

public class EntryState : AIState
{
    protected override string DefaultName => "EntryState";

    public EntryState(AIStateData AIStateData) : base(AIStateData)
    {
        // empty
    }

    public override void OnEnter()
    {
        // empty
    }

    public override void OnExit()
    {
        // empty
    }

    /// <summary>
    /// Should be called before every match. If the player has won the previous 
    /// match, then the stats will be updated to the new ones. Else, the stats
    /// remains the same.
    /// </summary>
    public override void Update()
    {
        AIStateData.AIHealth.CachedHealth = AIStateData.AIHealth.StartingHealth;
        AIStateData.AIHealth.StartingHealth = AIStateData.AIStats.Health;

        navMeshAgent.speed = AIStateData.AIStats.Speed;
        navMeshAgent.angularSpeed = AIStateData.AIStats.TurnSpeed;

        SetBool(TransitionKey.shouldPatrol, true);
    }
}
