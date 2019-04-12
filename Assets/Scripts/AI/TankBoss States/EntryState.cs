using UnityEngine;

public class EntryState : AIState
{
    protected override string DefaultName { get { return "EntryState"; } }

    public EntryState(AIStateData AIStateData) : base(AIStateData)
    {
        SetStats();
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
    /// Set AI stats accordingly
    /// </summary>
    public override void Update()
    {
        SetStats();
        SetBool(TransitionKey.shouldPatrol, true);
    }

    /// <summary>
    /// Should be called before every match. If the player has won the previous 
    /// match, then the stats will be updated to the new ones. Else, the stats
    /// remains the same.
    /// </summary>
    public void SetStats()
    {
        AIHealth.CachedHealth = AIHealth.StartingHealth;
        AIHealth.StartingHealth = AIStateData.AIStats.Health;
        
        navMeshAgent.speed = AIStateData.AIStats.Speed;
        navMeshAgent.angularSpeed = AIStateData.AIStats.TurnSpeed;
    }
}
