using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Basket : MonoBehaviourPunCallbacks
{
    [Header("References")]
    public CourseController courseController;
    public ParticleSystem discEnterEffect;
    public AudioSource crowdCheer;

    [Header("Debug Data")]
    public bool showDebugMessages;

    private void Awake()
    {
        courseController = GameObject.FindGameObjectWithTag("CourseController").gameObject.GetComponent<CourseController>();    
    }

    // TODO
    // Get course controller reference
    // Get photon view reference from that
    // Call RPC using the located photon view
    // Then SUCCESS

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            discEnterEffect.Play();
            crowdCheer.Play();

            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                //other.gameObject.GetComponent<PrototypeController>().DiscInHole(gameObject);
                other.gameObject.GetComponent<PrototypeController>().DiscInHole(gameObject);
                OutputDebugMessage("Disc entered hole", "green", false);
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
