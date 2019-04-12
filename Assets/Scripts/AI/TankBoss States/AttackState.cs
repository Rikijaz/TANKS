using UnityEngine;

public class AttackState : AIState
{
    protected override string DefaultName { get { return "AttackState"; } }

    private TankShooting shooting;
    private TankMovement playerMovement;

    private bool isCooledDown;
    private bool isPreparedToFire;
    private bool isMissleCharged;
    private bool hasFired; 

    private float missleCooldownTimer;

    private static readonly Vector3 missleLocalPosition = new Vector3(0f, 1.7f, 1.35f);
    private const float missleColliderRadius = 0.15f;

    public AttackState(AIStateData AIStateData) : base(AIStateData)
    { 
        shooting = AIStateData.AI.GetComponent<TankShooting>();
        playerMovement = AIStateData.player.GetComponent<TankMovement>();

        isCooledDown = false;
        isPreparedToFire = false;
        isMissleCharged = false;
        hasFired = false;

        missleCooldownTimer = AIStateData.AIStats.MissleCooldownDuration;
    }

    /// <summary>
    /// Halt position and rotation 
    /// </summary>
    public override void OnEnter()
    {
        SetBool(TransitionKey.shouldAttack, false);

        hasFired = false;

        ResetRigidBodyPhysics();
    }

    /// <summary>
    ///  Reset attack settings except missle cooldown timer
    /// </summary>
    public override void OnExit()
    {
        isPreparedToFire = false;
        isMissleCharged = false;
    }

    /// <summary>
    /// If hit by missle, enter stunned state. Else, face the player then attack 
    /// the player. If the player is within attack range and is in sight, then 
    /// attack again. Else, pursue the player.
    /// </summary>
    public override void Update() {
        if (!IsHit())
        {
            if (!hasFired)
            {
                Aim();
                Attack();
            }
            else if (IsPlayerInRadius(AIStateData.AIStats.AttackRange)
                && IsPlayerInSight())
            {
                hasFired = false;
            }
            else
            {
                SetBool(TransitionKey.shouldPursue, true);
            }
        }
    }

    /// <summary>
    /// Aim at the player's future position
    /// </summary>
    private void Aim()
    {
        Vector3 futurePosition = CalculateFuturePosition();
        futurePosition += Random.insideUnitSphere * AIStateData.AIStats.MissleMissMargin;
        futurePosition.y = 0f;

        Vector3 displacement = futurePosition - AIStateData.AI.transform.position;

        if (displacement.magnitude >= Mathf.Epsilon)
        {
            float turn = AIStateData.AIStats.TurnSpeed * Time.deltaTime;
            Quaternion rotation = Quaternion.LookRotation(displacement);

            AIStateData.AI.transform.rotation = Quaternion.RotateTowards(
                AIStateData.AI.transform.rotation,
                rotation,
                turn);
        }
    }

    /// <summary>
    /// Wait for missle cooldown timer then start charging missle. While charging
    /// missle, calculate the force needed to hit the player. When the missle has 
    /// the required force OR the player has left the attack range, fire the missle, 
    /// then restart missle cooldown timer
    /// </summary>
    private void Attack()
    {
        missleCooldownTimer += Time.deltaTime;

        isCooledDown = (missleCooldownTimer >= AIStateData.AIStats.MissleCooldownDuration);

        if (isCooledDown)
        {
            if (!isPreparedToFire)
            {
                shooting.PrepareToFire();
                isPreparedToFire = true;
            }
            else if (!isMissleCharged)
            {
                shooting.ChargeMissle();

                if ((shooting.currentLaunchForce >= CalculateMissleMagnitude()) 
                    || IsPlayerInRadius(AIStateData.AIStats.AttackRange))
                {
                    isMissleCharged = true;
                }
            }
            else
            {
                shooting.Fire();
                missleCooldownTimer = 0f;
                isCooledDown = false;
                isPreparedToFire = false;
                isMissleCharged = false;
                hasFired = true;
            }
        }
    }

    /// <summary>
    /// Given the player's position, calculate the force needed to hit the player
    /// </summary>
    private float CalculateMissleMagnitude()
    {
        float gravity = Physics.gravity.magnitude;
        float height = missleLocalPosition.y + missleColliderRadius;

        float a = -(gravity / 2);
        float b = 0;
        float c = height;

        float time = (-b + Mathf.Sqrt(Mathf.Pow(b, 2) - (4 * a * c))) / (2 * a);

        Vector3 missleVelocity = (
            missleLocalPosition + 
            AIStateData.AI.transform.position - 
            AIStateData.player.transform.position) / time;

        return missleVelocity.magnitude;
    }

    /// <summary>
    /// Given the player's current position and velocity, and the current velocity 
    /// of the charging missle, calculate the position where the player and missle
    /// will intersect. If the player is not moving or the missle is too slow to
    /// reach the player, then return the player's current position. Else, calculate
    /// </summary>
    private Vector3 CalculateFuturePosition()
    {
        Vector3 predictedPosition = AIStateData.player.transform.position;

        Vector3 vectorB = playerMovement.Velocity;
        Vector3 vectorC =
            AIStateData.player.transform.position - AIStateData.AI.transform.position;

        float angleA = Vector3.Angle(-vectorC, vectorB) * Mathf.Deg2Rad;

        float sideA = shooting.currentLaunchForce * Time.deltaTime;
        float sideB = vectorB.magnitude;

        float sinReciprocalA = Mathf.Sin(angleA) / sideA;
        float cosReciprocalA = Mathf.Cos(angleA) / sideB;

        if ((sideB != 0 || sideB <= sideA) || (sinReciprocalA <= cosReciprocalA))
        {
            float angleB = Mathf.Asin(Mathf.Sin(angleA) * sideB / sideA);
            float angleC = Mathf.Sin(Mathf.PI - angleA - angleB);
            float sideC = vectorC.magnitude;

            Vector3 vectorA = (((vectorB * sideC) / angleC) * Mathf.Sin(angleB)) / sideB;

            predictedPosition = AIStateData.player.transform.position + vectorA;
        }

        return predictedPosition;
    }
}
