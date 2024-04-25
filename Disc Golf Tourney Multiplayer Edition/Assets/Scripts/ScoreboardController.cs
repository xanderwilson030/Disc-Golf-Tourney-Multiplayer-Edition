using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Photon.Pun.UtilityScripts;

public class ScoreboardController : MonoBehaviourPunCallbacks
{
    [Header("Object References")]
    public GameObject scoreBoard;
    public GameObject playerOneRow;
    public GameObject playerTwoRow;
    public GameObject playerThreeRow;
    public GameObject playerFourRow;

    [Header("Text References")]
    public TMP_Text[] parScores;
    public TMP_Text[] playerOneScores;
    public TMP_Text[] playerTwoScores;
    public TMP_Text[] playerThreeScores;
    public TMP_Text[] playerFourScores;
    public TMP_Text[] playerNames;

    [Header("Score Stats")]
    public int currentHole;
    public int p1Final = 0, p2Final = 0, p3Final = 0, p4Final = 0;
    public bool gameOver = false;

    [Header("Debug Values")]
    public bool showDebugMessages;
    public static ScoreboardController instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (!gameOver)
        {
            // Reveal the scoreboard while holding TAB
            if (Input.GetKey(KeyCode.Tab))
            {
                scoreBoard.SetActive(true);
            }
            else
            {
                scoreBoard.SetActive(false);
            }
        }
    }

    /*
     *  The following method updates the scoreboard with the names of each player at the start of the game 
     */
    public void UpdatePlayerScoreboardNames()
    {
        //playerOneScores[0] = PhotonNetwork.PlayerList[0].NickName;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (i == 0)
            {
                playerOneRow.SetActive(true);
                playerNames[0].text = PhotonNetwork.PlayerList[i].NickName;
            }
            else if (i == 1)
            {
                playerTwoRow.SetActive(true);
                playerNames[1].text = PhotonNetwork.PlayerList[i].NickName;
            }
            else if (i == 2)
            {
                playerThreeRow.SetActive(true);
                playerNames[2].text = PhotonNetwork.PlayerList[i].NickName;
            }
            else if (i == 3)
            {
                playerFourRow.SetActive(true);
                playerNames[3].text = PhotonNetwork.PlayerList[i].NickName;
            }
        }

        currentHole = 1;

        OutputDebugMessage("Intialized scoreboard with player names", "orange", false);
    }

    /*
     *  The following method sets the par scores for the course
     */
    public void SetParScoresForCourse(HoleData[] holes)
    {
        int parTotal = 0;

        for (int i = 0; i < holes.Length; i++)
        {
            parScores[i].text = (holes[i].parScore).ToString();
            parTotal += holes[i].parScore;
        }

        parScores[parScores.Length - 1].text = parTotal.ToString();

        OutputDebugMessage("Initialized Par Scores", "orange", false);
    }

    /*
     *  The following method updates the scoreboard
     */
    public void UpdateScoreboardWithHoleScores()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (i == 0)
            {
                playerOneScores[currentHole - 1].text = PhotonNetwork.PlayerList[0].GetScore().ToString();
                p1Final += PhotonNetwork.PlayerList[0].GetScore();
                PhotonNetwork.PlayerList[0].SetScore(0);
            }
            else if (i == 1)
            {
                playerTwoScores[currentHole - 1].text = PhotonNetwork.PlayerList[1].GetScore().ToString();
                p2Final += PhotonNetwork.PlayerList[1].GetScore();
                PhotonNetwork.PlayerList[1].SetScore(0);
            }
            else if (i == 2)
            {
                playerThreeScores[currentHole - 1].text = PhotonNetwork.PlayerList[2].GetScore().ToString();
                p3Final += PhotonNetwork.PlayerList[2].GetScore();
                PhotonNetwork.PlayerList[2].SetScore(0);
            }
            else if (i == 3)
            {
                playerFourScores[currentHole - 1].text = PhotonNetwork.PlayerList[3].GetScore().ToString();
                p4Final += PhotonNetwork.PlayerList[3].GetScore();
                PhotonNetwork.PlayerList[3].SetScore(0);
            }
        }

        currentHole++;
    }

    /*
     *  This method updates the final scores
     */
    public void SetFinalScores()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (i == 0)
            {
                playerOneScores[currentHole].text = p1Final.ToString();
            }
            else if (i == 1)
            {
                playerTwoScores[currentHole].text = p2Final.ToString();
            }
            else if (i == 2)
            {
                playerThreeScores[currentHole].text = p3Final.ToString();
            }
            else if (i == 3)
            {
                playerFourScores[currentHole].text = p4Final.ToString();
            }
        }
    }

    /*
     * The following method is used to determine if debugging is currently activated and then
     * outputs the given message
     */
    private void OutputDebugMessage(string message, string color, bool isError)
    {
        if (showDebugMessages)
        {
            if (isError)
            {
                Debug.LogError(message);
            }
            else
            {
                Debug.Log($"<color={color}>{message}</color>");
            }
        }

    }
}
