using UnityEngine;

public class AIStats
{
    public float Speed { get; private set; }
    public float TurnSpeed { get; private set; }

    public float Health { get; private set; }
    public float CriticalHealth { get; private set; }

    public float MissleMissMargin { get; private set; }
    public float MissleCooldownDuration { get; private set; }
    public static readonly Vector3 MissleLocalPosition = new Vector3(0f, 1.7f, 1.35f);
    public static readonly float MissleColliderRadius = 0.15f;

    public float ScanDegrees { get; private set; }
    public float ScanSpeed { get; private set; }

    public float AttackRange { get; private set; }
    public float SightRange { get; private set; }
    public float AlertRadius { get; private set; }

    public float StunDuration { get; private set; }

    public AIStats()
    {
        Speed = 10f;
        TurnSpeed = 180f;

        Health = 100f;
        CriticalHealth = 20f;

        MissleMissMargin = 0.175f;
        MissleCooldownDuration = 0.65f;

        ScanSpeed = 60f;
        ScanDegrees = 180f;

        AttackRange = 12f;
        SightRange = 60f;
        AlertRadius = 25f;

        StunDuration = 1.10f;
    }

    /// <summary>
    /// Level up the AI's stats
    /// </summary>
    public void LevelUp()
    {
        Speed += Speed * 0.18f;
        TurnSpeed += TurnSpeed * 0.20f;

        Health += Health * 0.25f;
        CriticalHealth -= 5f;
        CriticalHealth = Mathf.Clamp(
            CriticalHealth,
            0,
            CriticalHealth);

        MissleMissMargin -= MissleMissMargin * 0.25f;
        MissleMissMargin = Mathf.Clamp(MissleMissMargin, 0, MissleMissMargin);
        MissleCooldownDuration -= MissleCooldownDuration * 0.20f;
        MissleCooldownDuration = Mathf.Clamp(
            MissleCooldownDuration, 
            0, 
            MissleCooldownDuration);

        ScanSpeed += ScanSpeed * 0.20f;
        ScanDegrees += ScanDegrees * 0.25f;
        ScanDegrees = Mathf.Clamp(ScanDegrees, ScanDegrees, 360f);

        AttackRange += AttackRange * 0.25f;
        AttackRange = Mathf.Clamp(AttackRange, AttackRange, 25f);
        SightRange += SightRange * 0.20f;
        AlertRadius += AlertRadius * 0.25f;

        StunDuration -= StunDuration * 0.25f;
        StunDuration = Mathf.Clamp(StunDuration, 0, StunDuration);
    }
}
