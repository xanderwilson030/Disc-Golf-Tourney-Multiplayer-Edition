using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

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

    [Header("Debug Values")]
    public bool showDebugMessages;
    public static ScoreboardController instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
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
                playerNames[0].text = PhotonNetwork.PlayerList[i].NickName;
            }
            else if (i == 2)
            {
                playerThreeRow.SetActive(true);
                playerNames[0].text = PhotonNetwork.PlayerList[i].NickName;
            }
            else if (i == 3)
            {
                playerFourRow.SetActive(true);
                playerNames[0].text = PhotonNetwork.PlayerList[i].NickName;
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
