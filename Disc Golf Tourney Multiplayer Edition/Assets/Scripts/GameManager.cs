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
    // instance
    public static GameManager instance;
    void Awake()
    {
        // set the instance to this script
        instance = this;
    }
    void Start()
    {
        players = new PrototypeController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }
    // when a player loads into the game scene - tell everyone
    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        // when all the players are in the scene - spawn the players
        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }

    // spawns a player and initializes it
    void SpawnPlayer()
    {
        // instantiate the player across the network
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity, 0);
        // get the player script
        PrototypeController playerScript = playerObj.GetComponent<PrototypeController>();
        // initialize the player
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    // returns the player who has the requested id
    public PrototypeController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }
    // returns the player of the requested GameObject
    public PrototypeController GetPlayer(GameObject playerObject)
    {
        return players.First(x => x.gameObject == playerObject);
    }
    // called when a player's held the hat for the winning amount of time
    [PunRPC]
    void WinGame(int playerId)
    {
        gameEnded = true;
        PrototypeController player = GetPlayer(playerId);
        //GameUI.instance.SetWinText(player.photonPlayer.NickName);
        Invoke("GoBackToMenu", 3.0f);
    }
    // called after the game has been won - navigates back to the Menu scene
    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}