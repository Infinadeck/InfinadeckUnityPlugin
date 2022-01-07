using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Infinadeck;

/**
 * ------------------------------------------------------------
 * Script to translate Infinadeck motion into game motion.
 * http://tinyurl.com/InfinadeckSDK
 * Created by George Burger & Griffin Brunner @ Infinadeck, 2019
 * Attribution required.
 * ------------------------------------------------------------
 */

public class InfinadeckDeckModelSurfaceMotion : MonoBehaviour
{
    public Material mat;
    public bool demo = false;
    public Transform anchor;
    public Vector3 anchorPoint;
    private Vector3 deviance;
    public InfinadeckLocomotion motion;
    public Vector3 worldScale = Vector3.one;
    private float speedToDeckSurface = 1.2192f;

    [InfReadOnlyInEditor] public float xDistance;
    [InfReadOnlyInEditor] public float yDistance;

    InfinadeckLocomotion CheckForLocomotionInScene()
    {
        motion = FindObjectOfType<InfinadeckLocomotion>();
        return motion;
    }

    void Start()
    {
        if (!motion) { CheckForLocomotionInScene(); }
        if (motion)
        {
            worldScale = motion.worldScale;
            speedToDeckSurface *= worldScale.x;
        }
    }

    /**
     * Runs once per frame update.
     */
    void Update () {

        if (anchor) // only run if there is a successful connection
        {
            deviance = anchor.position - anchorPoint;
            mat.SetTextureOffset("_MainTex", new Vector2(-deviance.x / 1.2192f, -deviance.z / 1.2192f));
        }
        else
        {
            if (demo)
            {
                xDistance += .01f * Mathf.Cos(Time.time);
                yDistance += .01f * Mathf.Sin(Time.time);
            }
            else if (!motion && !CheckForLocomotionInScene()) // will not handle world scale or rotation in this instance, developers 
            {                                                 // using this feature in this context should be able to fix the problem
                xDistance += (float)Infinadeck.Infinadeck.GetFloorSpeeds().v0 * (Time.deltaTime) * 1;
                yDistance += (float)Infinadeck.Infinadeck.GetFloorSpeeds().v1 * (Time.deltaTime) * 1;
            }
            else
            {
                xDistance += motion.xDistance;
                yDistance += motion.yDistance;
            }

            mat.SetTextureOffset("_MainTex", new Vector2(-xDistance / speedToDeckSurface, -yDistance / speedToDeckSurface));
        }
    }
}