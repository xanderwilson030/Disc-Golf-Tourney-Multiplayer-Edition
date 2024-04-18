using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeController : MonoBehaviour
{
    public float rotationSpeed = 50f;
    private Rigidbody rb;

    public float liftCoefficient = 0.3f;
    public float dragCoefficient = 0.25f;
    public float liftArea = .1f;
    public float airDensity = 1.2f;
    public float throwSpeed = 10f;
    public float discRadius = 0.1f;

    private DiscState currentState;

    /*
     * Lift Coefficients
     * Distance Drivers:​ ≈ 0.2 - 0.5
     * Fairway Drivers: ≈ 0.1 - 0.3
       Mid-Range Discs:  ≈ 0.1 - 0.3
       Putters / Approach Discs: ≈ 0.05 - 0.2
     */

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        currentState = DiscState.Aiming;
    }

    void Update()
    {
        CheckMouse();
    }

    private void FixedUpdate()
    {
        Debug.Log(rb.velocity);

        if (rb.velocity.magnitude > 0)
        {
            // Calculate relative velocity
            Vector3 relativeVelocity = -rb.velocity;
            relativeVelocity.y = 0;

            // Calculate lift force
            //float liftForceMagnitude = 0.5f * airDensity * (relativeVelocity.z * relativeVelocity.z) * liftArea * liftCoefficient;
            float liftForceMagnitude = 0.5f * airDensity * relativeVelocity.sqrMagnitude * liftArea * liftCoefficient;
            Debug.Log("Lift force magnitude: " + liftForceMagnitude);
            Vector3 liftForce = liftForceMagnitude * rb.transform.up;
            Debug.Log("Lift force: " + liftForce);

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
            Debug.Log("Disc able to be thrown again");
        }
    }

    private void CheckMouse()
    {
        if (Input.GetMouseButton(0) && currentState == DiscState.Aiming)
        {
            Vector3 throwDirection = Camera.main.transform.forward;
            Vector3 initialVelocity = throwDirection * throwSpeed;

            rb.isKinematic = false;
            //rb.AddForce(initialVelocity);
            rb.velocity = initialVelocity;

            currentState = DiscState.Flying;
        }
    }
}
