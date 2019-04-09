using UnityEngine;

public class AttackState : AIState
{
    protected override string DefaultName { get { return "AttackState"; } }

    private TankShooting shooting;

    private bool isCooledDown;
    private bool isPreparedToFire;
    private bool isMissleCharged;
    private bool hasFired;

    private float missleCooldownTimer;

    public AttackState(AIStateData AIStateData) : base(AIStateData)
    { 
        shooting = AIStateData.AI.GetComponent<TankShooting>();

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
        SetBool("shouldAttack", false);

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
                FacePlayer();
                Attack();
            }
            else if ((DistanceToPlayer() <= AIStateData.AIStats.AttackRange)
                && IsPlayerInSight())
            {
                hasFired = false;
            }
            else
            {
                SetBool("shouldPursue", true);
            }
        }
    }

    /// <summary>
    /// Rotate to face the player
    /// </summary>
    private void FacePlayer()
    {
        Quaternion rotation = Quaternion.LookRotation(
            AIStateData.player.transform.position - AIStateData.AI.transform.position);
        float turn = AIStateData.AIStats.TurnSpeed * Time.deltaTime;

        AIStateData.AI.transform.rotation = Quaternion.Slerp(
            AIStateData.AI.transform.rotation,
            rotation,
            turn);
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
                    || (DistanceToPlayer() > AIStateData.AIStats.AttackRange))
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
        float height = AIStats.MissleLocalPosition.y + 
            AIStats.MissleColliderRadius;

        float a = -(gravity / 2);
        float b = 0;
        float c = height;

        float time = (-b + Mathf.Sqrt(Mathf.Pow(b, 2) - (4 * a * c))) / (2 * a);

        Vector3 missleVelocity = Vector3.zero;

        float missleXPosition =
            AIStats.MissleLocalPosition.x + AIStateData.AI.transform.position.x;
        float xDisplacement = missleXPosition - AIStateData.player.transform.position.x;
        missleVelocity.x = xDisplacement / time;

        float missleYPosition =
            AIStats.MissleLocalPosition.y + AIStateData.AI.transform.position.y;
        float yDisplacement = missleYPosition - AIStateData.player.transform.position.y;
        missleVelocity.y = yDisplacement / time;

        float missleZPosition =
            AIStats.MissleLocalPosition.z + AIStateData.AI.transform.position.z;
        float zDisplacement = missleZPosition - AIStateData.player.transform.position.z;
        missleVelocity.z = zDisplacement / time;

        float missleMagnitude = missleVelocity.magnitude + Random.Range(
            -(AIStateData.AIStats.MissleMissMargin / 2), 
            (AIStateData.AIStats.MissleMissMargin / 2));

        return missleVelocity.magnitude;
    }
}
