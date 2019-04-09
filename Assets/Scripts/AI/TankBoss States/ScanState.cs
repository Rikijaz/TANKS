using UnityEngine;

public class ScanState : AIState
{
    protected override string DefaultName { get { return "ScanState"; } }

    private float degreesRotated;
    private float degreeToRotate;
    private bool hasTurnLeft;
    private bool hasTurnRight;

    public ScanState(AIStateData AIStateData) : base(AIStateData)
    {

    }

    /// <summary>
    /// Set scan settings and halt position and rotation
    /// </summary>
    public override void OnEnter()
    {
        SetBool("shouldScan", false);

        ResetRigidBodyPhysics();

        degreesRotated = 0f;
        hasTurnLeft = false;
        hasTurnRight = false;
    }

    public override void OnExit() { }

    /// <summary>
    /// If hit by missle, enter stunned state. Else, scan for the player
    /// </summary>
    public override void Update()
    {
        if (!IsHit())
        {
            Scan();
        }
    }

    /// <summary>
    /// If the player is in sight, pursue the player. Else, turn X degrees left
    /// then 2X degrees right, then continue patrolling.
    /// </summary>
    private void Scan()
    {
        if (IsPlayerInSight())
        {
            SetBool("shouldPursue", true);
        }
        else
        {
            if (!hasTurnLeft)
            {
                degreeToRotate = AIStateData.AIStats.ScanDegrees / 2;
                TurnLeft();
            }
            else if (!hasTurnRight)
            {
                degreeToRotate = -(AIStateData.AIStats.ScanDegrees / 2);
                TurnRight();
            }
            else
            {
                SetBool("shouldPatrol", true);
            }
        }

    }

    /// <summary>
    /// Turn left X degrees
    /// </summary>
    private void TurnLeft()
    {
        if (degreesRotated < degreeToRotate)
        {
            float turn = AIStateData.AIStats.ScanSpeed * Time.deltaTime;
            AIStateData.AI.transform.Rotate(0f, turn, 0f);
            degreesRotated += turn;
        }
        else
        {
            hasTurnLeft = true;
        }
    }

    /// <summary>
    /// Turn right 2X degrees.
    /// </summary>
    private void TurnRight()
    {
        if (degreesRotated > degreeToRotate)
        {
            float turn = (-AIStateData.AIStats.ScanSpeed) * Time.deltaTime;
            AIStateData.AI.transform.Rotate(0f, turn, 0f);
            degreesRotated += turn;
        }
        else
        {
            hasTurnRight = true;
        }
    }
}
