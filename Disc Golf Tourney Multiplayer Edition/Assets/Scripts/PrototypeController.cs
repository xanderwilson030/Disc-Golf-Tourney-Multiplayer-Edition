using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrototypeController : MonoBehaviourPunCallbacks, IPunObservable
{
    public float rotationSpeed = 50f;
    

    [HideInInspector]
    public int id;
    public Player photonPlayer;

    [Header("Disc Physics Values")]
    public float liftCoefficient = 0.3f;
    public float dragCoefficient = 0.25f;
    public float liftArea = .1f;
    public float airDensity = 1.2f;
    public float discRadius = 0.1f;
    private Rigidbody rb;

    private DiscState currentState;

    [Header("Disc Throw Speed Values")]
    public float throwSpeed;
    public float throwSpeedIncrease;
    public float maxThrowSpeed;
    public Slider throwSpeedSlider;

    [Header("Debug Values")]
    public bool debugThrowSpeedMode;
    public float debugThrowSpeed;
    public bool showDebugMessages;

    /*
     * Lift Coefficients
     * Distance Drivers:​ ≈ 0.2 - 0.5
     * Fairway Drivers: ≈ 0.1 - 0.3
       Mid-Range Discs:  ≈ 0.1 - 0.3
       Putters / Approach Discs: ≈ 0.05 - 0.2
     */

    // called when the player object is instantiated
    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        GameManager.instance.players[id - 1] = this;
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            rb = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            currentState = DiscState.Aiming;

            // Intializing the slider
            throwSpeedSlider.maxValue = maxThrowSpeed;
            throwSpeedSlider.value = throwSpeed;
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            IntakeThrowSpeed();
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            //Debug.Log(rb.velocity);

            if (rb.velocity.magnitude > 0)
            {
                // Calculate relative velocity
                Vector3 relativeVelocity = -rb.velocity;
                relativeVelocity.y = 0;

                // Calculate lift force
                //float liftForceMagnitude = 0.5f * airDensity * (relativeVelocity.z * relativeVelocity.z) * liftArea * liftCoefficient;
                float liftForceMagnitude = 0.5f * airDensity * relativeVelocity.sqrMagnitude * liftArea * liftCoefficient;
                //Debug.Log("Lift force magnitude: " + liftForceMagnitude);
                Vector3 liftForce = liftForceMagnitude * rb.transform.up;
                //Debug.Log("Lift force: " + liftForce);

                // Calculate drag force
                // Experimenting with using drag coefficient in rigidbody instead
                //float dragForceMagnitude = 0.5f * airDensity * (relativeVelocity.z * relativeVelocity.z) * liftArea * dragCoefficient;
                float dragForceMagnitude = 0.5f * airDensity * relativeVelocity.sqrMagnitude * liftArea * dragCoefficient;
                Vector3 dragForce = dragForceMagnitude * relativeVelocity.normalized;

                // Apply forces
                rb.AddForce(liftForce);
                rb.AddForce(dragForce);
                //rb.drag = dragForceMagnitude;
            }
            else if (currentState == DiscState.Flying)
            {
                currentState = DiscState.Aiming;
                OutputDebugMessage("Disc can be thrown again", "orange", false);
            }
        }
    }

    /*
     *  The following method intakes the throw speed from the user and applies it to the disc
     */
    private void IntakeThrowSpeed()
    {
        // The debug mode for throw speed allows you to input a set value and skip the flexible power intake option
        if (Input.GetMouseButton(0) && currentState == DiscState.Aiming && !debugThrowSpeedMode)
        {
            // Increasing the throw speed based on how long the user holds down the mouse
            throwSpeed += throwSpeedIncrease * Time.deltaTime;

            // Caps the throw speed
            if (throwSpeed >= maxThrowSpeed)
            {
                throwSpeed = maxThrowSpeed;
            }

            // Setting the value on the slider
            throwSpeedSlider.value = throwSpeed;

            OutputDebugMessage($"Throwspeed is: {throwSpeed}", "blue", false);
        }
        // When the user stops holding down the mouse button we apply the force to the disc
        else if (Input.GetMouseButtonUp(0) && currentState == DiscState.Aiming)
        {
            Vector3 throwDirection = Camera.main.transform.forward;
            Vector3 initialVelocity = throwDirection * throwSpeed;

            // Turn off kinematic so the disc can move, and then set the velocity (NOT ADD FORCE)
            rb.isKinematic = false;
            rb.velocity = initialVelocity;

            // Change the state so we can no longer interact with the disc while it is flying
            currentState = DiscState.Flying;
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
