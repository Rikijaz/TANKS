﻿using UnityEngine;

public class HealState : AIState
{
    protected override string DefaultName { get { return "HealState"; } }

    public HealState(AIStateData AIStateData) : base(AIStateData)
    {
        //empty
    }

    /// <summary>
    /// Start navMeshAgent
    /// </summary>
    public override void OnEnter()
    {
        SetBool(TransitionKey.shouldHeal, false);

        ResetRigidBodyPhysics();
    }

    public override void OnExit()
    {
        // empty
    }

    /// <summary>
    /// If hit by missle, enter stunned state. Else, heal
    /// </summary>
    public override void Update()
    {
        if (!IsHit() )
        {
            Heal();
        }
    }

    /// <summary>
    /// If health is fully recovered then pursue player. Else if player is in 
    /// the heal radius, flee. Else, heal.
    /// </summary>
    private void Heal()
    {
        if (AIHealth.CurrentHealth < AIStateData.AIStats.Health)
        {
            if (IsPlayerInRadius(AIStats.HealRadius))
            {
                SetBool(TransitionKey.shouldFlee, true);
            }
            else
            {
                float healAmount = AIStateData.AIStats.HealthRegeneration * Time.deltaTime;
                AIHealth.Heal(healAmount);
            } 
        }
        else
        {
            SetBool(TransitionKey.shouldPursue, true);
        }
    }
}
