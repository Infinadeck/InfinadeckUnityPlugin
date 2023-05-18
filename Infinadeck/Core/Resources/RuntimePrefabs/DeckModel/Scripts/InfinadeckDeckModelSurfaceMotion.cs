using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float speedToDeckSurface = 1.2192f;

    [InfReadOnlyInEditor] public float xDistance;
    [InfReadOnlyInEditor] public float yDistance;
    public InfinadeckInterpreter iI;

    /**
     * Runs once per frame update.
     */
    void Update () {
        if (!motion) { motion = FindObjectOfType<InfinadeckLocomotion>(); }
        else if (!iI) { iI = FindObjectOfType<InfinadeckInterpreter>(); }
        else
        {
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
                else if (motion)
                {
                    xDistance += (float)iI.InfIntGetFloorSpeeds.v0 * (Time.deltaTime);
                    yDistance += (float)iI.InfIntGetFloorSpeeds.v1 * (Time.deltaTime);
                }

                mat.SetTextureOffset("_MainTex", new Vector2(-xDistance / speedToDeckSurface, -yDistance / speedToDeckSurface));
            }
        }
    }
}