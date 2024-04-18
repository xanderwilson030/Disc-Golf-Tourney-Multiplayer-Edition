using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject lobbyScreen;
    [Header("Main Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;
    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;      // text displaying all the players currently in the lobby
    public Button startGameButton;              // button the host can press to start the game
    void Start()
    {
        // disable the buttons at the start as we're not connected to the server yet
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }
    // called when we connect to the master server
    // enable the "Create Room" and "Join Room" buttons
    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }
    // sets the currently visible screen
    void SetScreen(GameObject screen)
    {
        // deactivate all screens
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        // enable the requested screen
        screen.SetActive(true);
    }

    // called when the player name input field has been updated
    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }
    // called when the "Create Room" button is pressed
    public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }
    // called when the "Join Room" button is pressed
    public void OnJoinRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }
    // called when we join a room
    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);
        // since there's now a new player in the lobby, tell everyone to update the lobby UI
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }
    // called when a player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // we don't RPC it like when we join the lobby
        // that's because OnJoinedRoom is only called for the client who just joined the room
        // OnPlayerLeftRoom gets called for all clients in the room, so we don't need to RPC it
        UpdateLobbyUI();
    }
    // update the lobby UI to show player list and host buttons
    [PunRPC]
    public void UpdateLobbyUI()
    {
        playerListText.text = "";
        // display all players currently in the lobby
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            playerListText.text += player.Value.NickName + "\n";
        }
        // only the host can start the game
        if (PhotonNetwork.IsMasterClient)
            startGameButton.interactable = true;
        else
            startGameButton.interactable = false;
    }
    // called when the "Leave Lobby" button is pressed
    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }
    // called when the "Start Game" button is pressed
    // only the host can click this button
    public void OnStartGameButton()
    {
        // tell all players in the room to load the Game scene
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "DiscGolfTestingGround");
    }
}