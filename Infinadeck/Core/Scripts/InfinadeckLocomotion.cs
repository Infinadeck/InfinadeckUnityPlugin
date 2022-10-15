using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Infinadeck;

/**
 * ------------------------------------------------------------
 * Script to translate Infinadeck motion into game motion.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by George Burger & Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

public class InfinadeckLocomotion : MonoBehaviour
{
    public GameObject cameraRig;

    [InfReadOnlyInEditor] public float xDistance;
    [InfReadOnlyInEditor] public float yDistance;

    private float fixAngle;
    private float calcX;
    private float calcY;
    public Vector3 worldScale = Vector3.one;
    public float speedGain = 1;
    public InfinadeckReferenceObjects infinadeckReferenceObj;
    public bool showCollisions = false;
    private Vector3 targetPosition = Vector3.zero;
    private Vector3 previousFramePosition = Vector3.zero;
    public bool showTreadmillVelocity = false;

    void Start()
    {
        targetPosition = cameraRig.transform.position;
        previousFramePosition = cameraRig.transform.position;
    }

    /**
     * Runs once per frame update.
     */
    void Update()
    {
        if (Infinadeck.Infinadeck.CheckConnection()) // only run if there is a successful connection
        {
            if (Vector3.Distance(cameraRig.transform.position, previousFramePosition) > 0.001f)
            {
                if (showCollisions) { Debug.Log("Infinadeck Pre - Locomotion Check: User moved since last frame.Collision has occurred."); }
            }

            // Import speeds from Infinadeck
            SpeedVector2 speeds = Infinadeck.Infinadeck.GetFloorSpeeds();

            if (showTreadmillVelocity) { Debug.Log(speeds.v0 + " " + speeds.v1); }

            // Distance = speed * time between samples
            //Debug.Log(speeds.v0 + "    " + speeds.v1);
            calcX = (float)speeds.v0 * (Time.deltaTime);
            calcY = (float)speeds.v1 * (Time.deltaTime);
            // Convert for any weird world rotation or scale
            fixAngle = this.transform.eulerAngles.y* Mathf.Deg2Rad;
            xDistance =  (calcX * Mathf.Cos(fixAngle) + calcY * Mathf.Sin(fixAngle)) * worldScale.x * speedGain;
            yDistance = (-calcX * Mathf.Sin(fixAngle) + calcY * Mathf.Cos(fixAngle)) * worldScale.z * speedGain;

            targetPosition = cameraRig.transform.position + new Vector3(xDistance, 0, yDistance);

            // Move user based on treadmill motion as long as the deck is not calibrating
            cameraRig.transform.position = targetPosition;

            if (Vector3.Distance(cameraRig.transform.position, targetPosition) > 0.001f)
            {
                if (showCollisions) { Debug.Log("Infinadeck Post-Locomotion Check: User not moved to target location. Collision has occurred."); }
            }

            previousFramePosition = cameraRig.transform.position;

            if (infinadeckReferenceObj) { infinadeckReferenceObj.currentTreadmillSpeed = Vector3.Magnitude(new Vector3((float)speeds.v0, (float)speeds.v1, 0)); }
        }
    }
}