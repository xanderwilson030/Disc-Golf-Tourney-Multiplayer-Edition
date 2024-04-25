using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    [HideInInspector]
    public bool gameEnded = false;          // has the game ended?
    [Header("Players")]
    public string playerPrefabLocation;     // player prefab path in the Resources folder
    public Transform[] spawnPoints;         // array of player spawn points
    [HideInInspector]
    public PrototypeController[] players;      // array of all players
    [HideInInspector]
    private int playersInGame;              // number of players currently in the Game scene
    
    public static GameManager instance; // Singleton instance

    void Awake()
    {
        // Creating the singleton
        instance = this;
    }

    void Start()
    {
        players = new PrototypeController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
        Debug.Log(PhotonNetwork.PlayerList.Length);
    }
    
    /*
     *  This method runs when a player joins the game, and when all players are in the if statement
     *  below runs and calls StartCourse() in courseController
     */
    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        
        // When every player is in the scene... start the course
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            CourseController.instance.StartCourse();
            ScoreboardController.instance.UpdatePlayerScoreboardNames();
            // SpawnPlayer();
        }
    }

    /*
     *  This method returns the player script with the given player id
     */
    public PrototypeController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    /*
     *  This method returns the player gameobject with the given player id
     */
    public GameObject GetPlayerGameObject(int playerId)
    {
        return players.First(x => x.id == playerId).gameObject;
    }

    /*
     *  This method returns the player script associated with the given gameobject
     */
    public PrototypeController GetPlayer(GameObject playerObject)
    {
        return players.First(x => x.gameObject == playerObject);
    }
}