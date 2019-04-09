using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private class TankType
    {
        public static readonly int player = 0;
        public static readonly int AI = 1;
    }

    [SerializeField] CameraControl cameraControl;
    [SerializeField] Text messageText;
    [SerializeField] Text bossText;
    [SerializeField] Slider bossTankHealthSlider;
    [SerializeField] Image bossTankHealthFillImage;
    [SerializeField] GameObject[] tankPrefab;
    [SerializeField] TankManager[] tanks;

    private int numRoundsToWin = 6;
    private float startDelay = 3f;
    private float endDelay = 3f;
    private int roundNumber;              
    private WaitForSeconds startWait;     
    private WaitForSeconds endWait;
    private TankManager roundWinner;
    private TankManager gameWinner;



    private void Start()
    {
        startWait = new WaitForSeconds(startDelay);
        endWait = new WaitForSeconds(endDelay);

        SpawnAllTanks();
        SetCameraTargets();

        StartCoroutine(GameLoop());
    }

    private void SpawnAllTanks()
    {
        tanks[TankType.player].instance =
            Instantiate(
                tankPrefab[TankType.player],
                tanks[TankType.player].spawnPoint.position,
                tanks[TankType.player].spawnPoint.rotation) as GameObject;
        tanks[TankType.player].playerNumber = 1;
        tanks[TankType.player].SetupPlayer();

        tanks[TankType.AI].instance =
                Instantiate(
                    tankPrefab[TankType.AI],
                    tanks[TankType.AI].spawnPoint.position,
                    tanks[TankType.AI].spawnPoint.rotation) as GameObject;
        tanks[TankType.AI].playerNumber = 2;
        tanks[TankType.AI].SetupBoss(
            tanks[TankType.player].instance, 
            bossTankHealthSlider, 
            bossTankHealthFillImage);
    }

    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = tanks[i].instance.transform;
        }

        cameraControl.targets = targets;
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (gameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }

    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();

        cameraControl.SetStartPositionAndSize();

        roundNumber++;
        messageText.text = "ROUND " + roundNumber;

        bossText.text = string.Format(
            "TANK BOSS (LVL {0}) ", 
            (tanks[TankType.player].wins + 1));

        yield return startWait;
    }

    private IEnumerator RoundPlaying()
    {
        EnableTankControl();

        messageText.text = string.Empty;

        while (!OneTankLeft())
        {
            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        DisableTankControl();

        roundWinner = null;

        roundWinner = GetRoundWinner();

        if (roundWinner != null)
        {
            roundWinner.wins++;

            if (roundWinner.playerNumber == TankType.AI)
            {
                tanks[TankType.AI].LevelUpAI();
            }
        }

        gameWinner = GetGameWinner();

        messageText.text = EndMessage();

        yield return endWait;
    }

    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < tanks.Length; i++)
        {
            if (tanks[i].instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }

    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < tanks.Length; i++)
        {
            if (tanks[i].instance.activeSelf)
                return tanks[i];
        }

        return null;
    }

    private TankManager GetGameWinner()
    {
        for (int i = 0; i < tanks.Length; i++)
        {
            if (tanks[i].wins == numRoundsToWin)
                return tanks[i];
        }

        return null;
    }

    private string EndMessage()
    {
        string message = "DRAW!";

        if (roundWinner != null)
        {
            message = roundWinner.coloredPlayerText + " WINS THE ROUND!";
        }

        message += "\n\n\n\n";

        for (int i = 0; i < tanks.Length; i++)
        {
            message += tanks[i].coloredPlayerText + ": " + tanks[i].wins + 
                " WINS\n";
        }

        if (gameWinner != null)
        {
            message = gameWinner.coloredPlayerText + " WINS THE GAME!";
        }

        return message;
    }

    private void ResetAllTanks()
    {
        for (int i = 0; i < tanks.Length; i++)
        {
            tanks[i].Reset();
        }
    }

    private void EnableTankControl()
    {
        for (int i = 0; i < tanks.Length; i++)
        {
            tanks[i].EnableControl();
        }
    }

    private void DisableTankControl()
    {
        for (int i = 0; i < tanks.Length; i++)
        {
            tanks[i].DisableControl();
        }
    }
}