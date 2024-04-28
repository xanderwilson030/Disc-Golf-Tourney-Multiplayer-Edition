using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 *  This class controls the pause menu that can be accessed when playing the game
 */



public class PauseMenu : MonoBehaviour
{
    [Header("Object References")]
    public GameObject pauseMenu;
    public GameObject settingsMenu;

    /*
     *  This method resets the player to the start of the current hole
     */
    public void ResetToHoleStart()
    {
        Debug.Log("<color=purple>Resetting player to hole start</color>");
    }

    /*
     *  This method forfeits the current hole for the player
     */
    public void ForfeitHole()
    {
        Debug.Log("<color=purple>Player forfeiting hole</color>");
    }

    /*
     *  This method handles the player leaving the game
     */
    public void LeaveGame()
    {
        Debug.Log("<color=purple>Player leaving game</color>");
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
}
