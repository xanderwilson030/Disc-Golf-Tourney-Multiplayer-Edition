using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  This class handles the out of bounds check for the player disc 
 * 
 */

public class OutOfBoundsCheck : MonoBehaviour
{
    [Header("Debug Data")]
    public bool showDebugMessages;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                //other.gameObject.GetComponent<PrototypeController>().DiscInHole(gameObject);
                other.gameObject.GetComponent<PrototypeController>().DiscWentOutOfBounds();
                OutputDebugMessage("Disc went out of bounds", "red", false);
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
