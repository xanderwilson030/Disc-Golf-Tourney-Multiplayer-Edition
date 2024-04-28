using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using Photon.Pun.UtilityScripts;
using System.Linq;

public class PrototypeController : MonoBehaviourPunCallbacks, IPunObservable
{   

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
    public float rotationSpeed = 100f;

    public DiscState currentState;
    public bool movementPaused = false;
    public bool canSpectate = false;
    [SerializeField] int spectateIndex = 0;

    [Header("Disc Throw Speed Values")]
    public float throwSpeed;
    public float throwSpeedIncrease;
    public float maxThrowSpeed;
    public float discPositionResetSpeedLimit;

    [Header("UI References")]
    public Slider throwSpeedSlider;
    public TMP_Text parText;
    public TMP_Text scoreText;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject pauseMenu;
    public TMP_Text spectateText;

    [Header("Disc Placement Setup")]
    public float groundOffset;
    public float discResetDelay;

    [Header("Player Stats")]
    public int totalScore = 0;
    public int scoreForCurrentHole = 0;
    public Vector3 previousPosition;

    [Header("Debug Values")]
    public bool debugThrowSpeedMode;
    public float debugThrowSpeed;
    public bool showDebugMessages;
    public bool doSingleplayer;
    public bool debugMode;
    public CourseController courseController;

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

        if (photonView.IsMine)
        {
            rb = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            currentState = DiscState.Aiming;

            // Locating UI Elements
            throwSpeedSlider = GameObject.FindGameObjectWithTag("ThrowSlider").GetComponent<Slider>();
            parText = GameObject.FindGameObjectWithTag("ParText").GetComponent<TMP_Text>();
            scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<TMP_Text>();
            courseController = GameObject.FindGameObjectWithTag("CourseController").gameObject.GetComponent<CourseController>();
            pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").gameObject.transform.GetChild(0).gameObject;
            spectateText = GameObject.FindGameObjectWithTag("SpectateText").GetComponent<TMP_Text>();

            // Finding the camera
            virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamPlayer").GetComponent<CinemachineVirtualCamera>();
            virtualCamera.Follow = gameObject.transform;
            virtualCamera.LookAt = gameObject.transform;

            // Intializing the slider
            throwSpeedSlider.maxValue = maxThrowSpeed;
            throwSpeedSlider.value = throwSpeed;

            // Initalizing Score UI
            scoreText.text = scoreForCurrentHole.ToString();
            parText.text = CourseController.instance.GetCurrentPar().ToString();

            // Setting previous position
            previousPosition = gameObject.transform.position;

            // Debug
            OutputDebugMessage($"Current Position When Spawning in is {gameObject.transform.position}", "green", false);
        }
    }

    private void Start()
    {
        // Comment out when testing in singleplayer
        if (doSingleplayer)
        {
            if (photonView.IsMine)
            {
                rb = GetComponent<Rigidbody>();
                Cursor.lockState = CursorLockMode.Locked;
                currentState = DiscState.Aiming;

                // Locating UI Elements
                throwSpeedSlider = GameObject.FindGameObjectWithTag("ThrowSlider").GetComponent<Slider>();
                parText = GameObject.FindGameObjectWithTag("ParText").GetComponent<TMP_Text>();
                scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<TMP_Text>();
                courseController = GameObject.FindGameObjectWithTag("CourseController").gameObject.GetComponent<CourseController>();
                pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").gameObject.transform.GetChild(0).gameObject;

                // Finding the camera
                virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamPlayer").GetComponent<CinemachineVirtualCamera>();
                virtualCamera.Follow = gameObject.transform;
                virtualCamera.LookAt = gameObject.transform;

                // Intializing the slider
                throwSpeedSlider.maxValue = maxThrowSpeed;
                throwSpeedSlider.value = throwSpeed;

                // Initalizing Score UI
                scoreText.text = scoreForCurrentHole.ToString();
                parText.text = CourseController.instance.GetCurrentPar().ToString();
            }
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            IntakeThrowSpeed();
            RotateDisc();
        }

        if (photonView.IsMine)
        {
            if (debugMode)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    ResetDiscForFlight();
                }
            }
        }

        if (photonView.IsMine)
        {
            PauseHandler();
        }

        if (photonView.IsMine)
        {
            if (canSpectate)
            {
                SpectateOtherPlayers();
            }
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine && currentState != DiscState.Immobile)
        {
            //Debug.Log(rb.velocity);

            // If the the disc is slow enough, it will be reset to fly again
            if (currentState == DiscState.Flying && rb.velocity.magnitude < discPositionResetSpeedLimit)
            {
                OutputDebugMessage("Disc can be thrown again", "orange", false);
                ResetDiscForFlight();
            }
            else if (currentState == DiscState.Flying)  
            {
                //OutputDebugMessage($"Disc Velocity is: {rb.velocity} and magnitude is {rb.velocity.magnitude}", "green", false);
            }

            if (rb.velocity.magnitude > 0)
            {
                // Calculate relative velocity
                Vector3 relativeVelocity = -rb.velocity;

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
        }
    }

    /*
     *  This method handles disc rotation
     */
    private void RotateDisc()
    {
        if (currentState == DiscState.Aiming && currentState != DiscState.Immobile)
        {
            // This line might break everything
            //transform.forward = new Vector3(virtualCamera.transform.forward.x, 0, virtualCamera.transform.forward.z);

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 rotation = new Vector3(-verticalInput, 0f, -horizontalInput) * rotationSpeed * Time.deltaTime;

            transform.Rotate(rotation);
        }
    }

    /*
     *  The following method intakes the throw speed from the user and applies it to the disc
     */
    private void IntakeThrowSpeed()
    {
        // Pause menu check to not allow input
        if (movementPaused)
        {
            return;
        }

        // The debug mode for throw speed allows you to input a set value and skip the flexible power intake option
        if (Input.GetMouseButton(0) && currentState == DiscState.Aiming && !debugThrowSpeedMode && currentState != DiscState.Immobile)
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

            //OutputDebugMessage($"Throwspeed is: {throwSpeed}", "blue", false);
        }
        // When the user stops holding down the mouse button we apply the force to the disc
        else if (Input.GetMouseButtonUp(0) && currentState == DiscState.Aiming && currentState != DiscState.Immobile)
        {
            Vector3 throwDirection = Camera.main.transform.forward;
            Vector3 initialVelocity = throwDirection * throwSpeed;

            // Turn off kinematic so the disc can move, and then set the velocity (NOT ADD FORCE)
            rb.isKinematic = false;
            rb.velocity = initialVelocity;
            float forceValue = .18f * throwSpeed;
            //Vector3 initialVelocity = throwDirection * forceValue;
           //rb.AddForce(initialVelocity, ForceMode.Impulse);

            //rb.AddForce(initialVelocity, ForceMode.Impulse);

            // Change the state so we can no longer interact with the disc while it is flying
            currentState = DiscState.Flying;

            // Increment the score
            scoreForCurrentHole++;
            photonPlayer.SetScore(scoreForCurrentHole);
            scoreText.text = scoreForCurrentHole.ToString();
        }
    }

    /*
     *  The following method resets the disc and allows it to fly again
     */
    public void ResetDiscForFlight()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        Vector3 position = gameObject.transform.position;
        gameObject.transform.position = new Vector3(position.x, position.y + groundOffset, position.z);
        gameObject.transform.eulerAngles = Vector3.zero;
        previousPosition = gameObject.transform.position;
        currentState = DiscState.Aiming;
        throwSpeedSlider.value = 0;
        throwSpeed = 0;
        OutputDebugMessage("Disc is ready for flight again", "green", false);
    }

    /*
     *  The following method is called when the disc enters a basket
     */
    public void DiscInHole(GameObject hole)
    {
        //if (photonView.IsMine)
        //{

        //}

        currentState = DiscState.Immobile;
        virtualCamera.Follow = hole.transform;
        virtualCamera.LookAt = hole.transform;
        canSpectate = true;
        spectateText.text = "Spectating\nBasket";

        // Trigger Hole Rating Event
        GameEvents.instance.e_TriggerHoleRatingText.Invoke(scoreForCurrentHole);

        //ResetDiscForFlight();

        Invoke("SendCourseControllerHoleCompleted", 2f);
    }

    /*
     *  This method handles spectating after they have made it into a hole
     */
    public void SpectateOtherPlayers()
    {
        List<GameObject> playerList = FindSpectateablePlayer();

        if (playerList.Count > 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                spectateIndex++;

                if (spectateIndex >= playerList.Count)
                {
                    spectateIndex = 0;
                }

                virtualCamera.Follow = playerList.ElementAt(spectateIndex).transform;
                virtualCamera.LookAt = playerList.ElementAt(spectateIndex).transform;
                spectateText.text = $"Spectating\n{playerList.ElementAt(spectateIndex).GetComponent<PrototypeController>().photonPlayer.NickName}";
            }
        }
        else
        {
            virtualCamera.Follow = this.gameObject.transform;
            virtualCamera.LookAt= this.gameObject.transform;
        }
    }

    /*
     *  This method finds available players to spectate
     */
    public List<GameObject> FindSpectateablePlayer()
    {
        List<GameObject> spectatablePlayers = new List<GameObject>();

        for (int i = 0; i < GameManager.instance.players.Length; i++)
        {
            if (GameManager.instance.players[i].currentState != DiscState.Immobile)
            {
                spectatablePlayers.Add(GameManager.instance.players[i].gameObject);
            }
        }

        return spectatablePlayers;
    }

    public void SendCourseControllerHoleCompleted()
    {
        courseController.photonView.RPC("FinishedHole", RpcTarget.All);
    }

    /*
     *  The following method resets the disc from being out of bounds
     */
    public void DiscWentOutOfBounds()
    {
        if (photonView.IsMine)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            gameObject.transform.position = previousPosition;
            gameObject.transform.eulerAngles = Vector3.zero;
            currentState = DiscState.Aiming;
            throwSpeedSlider.value = 0;
            throwSpeed = 0;
            OutputDebugMessage("Disc is ready for flight again after going out of bounds", "green", false);
        }
    }

    /*
     *  The following method resets the disc for the next hole
     */
    public void ResetDiscForNextHole()
    {
        if (photonView.IsMine)
        {
            canSpectate = false;
            virtualCamera.Follow = this.gameObject.transform;
            virtualCamera.LookAt = this.gameObject.transform;

            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            gameObject.transform.eulerAngles = Vector3.zero;
            currentState = DiscState.Aiming;
            throwSpeedSlider.value = 0;
            throwSpeed = 0;

            totalScore += scoreForCurrentHole;
            scoreForCurrentHole = 0;
            scoreText.text = "0";
            spectateText.text = "";

            previousPosition = gameObject.transform.position;

            parText.text = CourseController.instance.GetCurrentPar().ToString();

            OutputDebugMessage("ResetDiscForNextHole Called", "green", false);
        }
    }

    /*
     *  The following method handles the pause menu
     */
    private void PauseHandler()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                movementPaused = false;
            }
            else if (!pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                movementPaused = true;
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // we want to sync the 'curHatTime' between all clients
        if (stream.IsWriting)
        {
            //stream.SendNext(curHatTime);
        }
        else if (stream.IsReading)
        {
            //curHatTime = (float)stream.ReceiveNext();
        }
    }
}
