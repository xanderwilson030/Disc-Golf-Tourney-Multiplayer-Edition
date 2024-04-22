using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

/*
 *  This method controls the state and flow of each individual course, while the game manager script controls the finer networking details
 * 
 * 
 *  This class is also a singleton
 */

public class CourseController : MonoBehaviourPunCallbacks
{
    [Header("Course Data")]
    public CourseData currentCourse;

    [Header("Course Data From File - NOTE: will be empty in editor")]
    public LevelNames currentLevelName;
    public int numHoles;
    public HoleData[] holes;
    public int currentHole = 0;

    [Header("Course/Hole Stats")]
    public int numPlayersFinishedAtHole;

    [Header("Object References")]
    public string playerPrefabLocation;

    [Header("Debug Values")]
    public bool showDebugMessages;
    public static CourseController instance;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Load the course data from the scriptable object
        LoadCourseFile();
    }

    /*
     *  The following method loads the course data from the scriptable object
     */
    private void LoadCourseFile()
    {
        OutputDebugMessage("Loading the current course scriptable object...", "green", false);

        currentLevelName = currentCourse.courseName;
        numHoles = currentCourse.numHoles;
        //holes = new HoleData[numHoles];
        
        //for (int i = 0; i < numHoles; i++)
        //{
        //    holes[i] = currentCourse.holes[i];
        //}

        if (holes == null || holes.Length == 0)
        {
            OutputDebugMessage("The current course has either no holes or is missing its references", "red", true);
        }
    }

    /*
     * The following method starts the course
     */
    public void StartCourse()
    {
        OutputDebugMessage($"Starting course named {currentLevelName.ToString()}", "green", false);
        numPlayersFinishedAtHole = 0;
        SpawnPlayer();
    }

    /*
     *  This method is called in start course and spawns all of the players
     *  
     *  Method was originally found in GameManager
     */
    private void SpawnPlayer()
    {
        // First we instantiate the player prefab across the network
        //GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity, 0);
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, GetCurrentHole().startingPosition, Quaternion.identity, 0);

        // Next, we need to access the player script and initialize the player
        PrototypeController playerScript = playerObj.GetComponent<PrototypeController>();
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    /*
     *  The following method returns the scriptable object associated with the current hole
     */
    private HoleData GetCurrentHole()
    {
        return holes[currentHole];
    }

    /*
     *  This method is called when a player finishes a hole
     *  
     *  If every player finishes a hole the players move onto the next one
     *  
     *  TODO... Link this up in basket script
     */
    [PunRPC]
    void FinishedHole()
    {
        numPlayersFinishedAtHole++;

        // When every player has finished the current hole... move to the next one
        if (numPlayersFinishedAtHole == PhotonNetwork.PlayerList.Length)
        {
            GoToNextHole();
        }
    }

    /*
     *  The following method moves to the next hole
     */
    private void GoToNextHole()
    {
        currentHole++;

        if (currentHole > numHoles)
        {
            // TODO --- Polish
            Debug.Log("Course Over");
            photonView.RPC("WinGame", RpcTarget.All, GameManager.instance);
        }

        numPlayersFinishedAtHole = 0;

        // Now we need to move all the players to the new starting position for the next whole
        
        foreach (PrototypeController player in GameManager.instance.players)
        {
            player.gameObject.transform.position = GetCurrentHole().startingPosition;
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
