﻿using UnityEngine;

public class AIStats
{
    readonly private struct InitialStats
    {
        public const float Speed = 10f;
        public const float TurnSpeed = 180f;

        public const float Health = 100f;
        public const float CriticalHealth = 55f;
        public const float HealthRegeneration = 5f;
        public const float StunDuration = 1.10f;

        public const float MissleMissMargin = 3f;
        public const float MissleCooldownDuration = 0.65f;

        public const float ScanSpeed = 60f;
        public const float ScanDegrees = 180f;

        public const float AttackRange = 12f;
        public const float SightRange = 60f;
        public const float AlertRadius = 25f;

        public const float CoverQuality = -0.50f;
    }

    readonly private struct StatMultipliers
    {
        public const float Speed = 0.18f;
        public const float TurnSpeed = 0.20f;

        public const float Health = 0.25f;
        public const float CriticalHealth = 0.25f;
        public const float HealthRegeneration = 0.25f;
        public const float StunDuration = 0.35f;

        public const float MissleMissMargin = 0.25f;
        public const float MissleCooldownDuration = 0.20f;

        public const float ScanSpeed = 0.20f;
        public const float ScanDegrees = 0.25f;

        public const float AttackRange = 0.25f;
        public const float SightRange = 0.20f;
        public const float AlertRadius = 0.25f;

        public const float CoverQuality = 0.05f;
    }

    readonly private struct StatLimits
    {
        public const float ScanDegrees = 360f;
        public const float AttackRange = 25f;
        public const float CoverQuality = -0.75f;
    }

    // Movement
    public float Speed { get; private set; }
    public float TurnSpeed { get; private set; }

    // Health
    public float Health { get; private set; }
    public float CriticalHealth { get; private set; }
    public float HealthRegeneration { get; private set; }
    public const float HealRadius = 15f;
    public float StunDuration { get; private set; }

    // Missle
    public float MissleMissMargin { get; private set; }
    public float MissleCooldownDuration { get; private set; }

    // Scan
    public float ScanDegrees { get; private set; }
    public float ScanSpeed { get; private set; }

    // Range
    public float AlertRadius { get; private set; }
    public float AttackRange { get; private set; }
    public float SightRange { get; private set; }

    // Cover
    public float CoverQuality { get; private set; }

    public AIStats()
    {
        Speed = InitialStats.Speed;
        TurnSpeed = InitialStats.TurnSpeed;

        Health = InitialStats.Health;
        CriticalHealth = InitialStats.CriticalHealth;
        HealthRegeneration = InitialStats.HealthRegeneration;
        StunDuration = InitialStats.StunDuration;

        MissleMissMargin = InitialStats.MissleMissMargin;
        MissleCooldownDuration = InitialStats.MissleCooldownDuration;

        ScanSpeed = InitialStats.ScanSpeed;
        ScanDegrees = InitialStats.ScanDegrees;

        AttackRange = InitialStats.AttackRange;
        SightRange = InitialStats.SightRange;
        AlertRadius = InitialStats.AlertRadius;

        CoverQuality = InitialStats.CoverQuality;
    }

    /// <summary>
    /// Level up the AI's stats
    /// </summary>
    public void LevelUp()
    {
        Speed += Speed * StatMultipliers.Speed;
        TurnSpeed += TurnSpeed * StatMultipliers.TurnSpeed;

        Health += Health * StatMultipliers.Health;
        CriticalHealth = Health * StatMultipliers.CriticalHealth;
        HealthRegeneration += StatMultipliers.HealthRegeneration;
        StunDuration -= StunDuration * StatMultipliers.StunDuration;
        StunDuration = Mathf.Clamp(StunDuration, 0f, StunDuration);

        MissleMissMargin -= MissleMissMargin * StatMultipliers.MissleMissMargin;
        MissleMissMargin = Mathf.Clamp(MissleMissMargin, 0f, MissleMissMargin);
        MissleCooldownDuration -= 
            MissleCooldownDuration * StatMultipliers.MissleCooldownDuration;
        MissleCooldownDuration = Mathf.Clamp(
            MissleCooldownDuration, 
            0, 
            MissleCooldownDuration);

        ScanSpeed += ScanSpeed * StatMultipliers.ScanSpeed;
        ScanDegrees += ScanDegrees * StatMultipliers.ScanDegrees;
        ScanDegrees = Mathf.Clamp(ScanDegrees, ScanDegrees, StatLimits.ScanDegrees);

        AttackRange += AttackRange * StatMultipliers.AttackRange;
        AttackRange = Mathf.Clamp(AttackRange, AttackRange, StatLimits.AttackRange);
        SightRange += SightRange * StatMultipliers.SightRange;
        AlertRadius += AlertRadius * StatMultipliers.AlertRadius;

        CoverQuality -= StatMultipliers.CoverQuality;
        CoverQuality = Mathf.Clamp(CoverQuality, StatLimits.CoverQuality, CoverQuality);
    }
}
