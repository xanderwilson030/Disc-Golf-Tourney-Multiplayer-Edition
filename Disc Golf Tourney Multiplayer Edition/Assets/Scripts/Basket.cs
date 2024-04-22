using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Basket : MonoBehaviourPunCallbacks
{
    [Header("References")]
    public CourseController courseController;

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
            courseController.photonView.RPC("FinishedHole", RpcTarget.All);
        }
    }
}
