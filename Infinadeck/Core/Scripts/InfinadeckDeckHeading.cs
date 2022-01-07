using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * ------------------------------------------------------------
 * Script to modify the position of the Infinadeck Deck Heading.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

public class InfinadeckDeckHeading : MonoBehaviour
{
    public float xin;
    public float yin;
    public float angle;
    public float mag;
    public float threshold;
    public float boxsize;
    public InfinadeckLocomotion motion;
    public float elevation;

    InfinadeckLocomotion CheckForLocomotionInScene()
    {
        motion = FindObjectOfType<InfinadeckLocomotion>();
        return motion;
    }

    // Update is called once per frame
    void Update()
    {
        if (!motion && !CheckForLocomotionInScene())                  // will not handle world scale or rotation 
        {                                                             // in this instance, developers using this 
            xin = (float)Infinadeck.Infinadeck.GetFloorSpeeds().v0;   // feature in this context should be 
            yin = (float)Infinadeck.Infinadeck.GetFloorSpeeds().v1;   // able to fix the problem
        }
        else
        {
            xin = motion.xDistance;
            yin = motion.yDistance;
        }
        mag = Mathf.Sqrt(xin * xin + yin * yin);
        if (mag > threshold)
        {
            this.GetComponent<Renderer>().enabled = true;
            if (Mathf.Abs(xin) > Mathf.Abs(yin))
            {
                yin *= boxsize / Mathf.Abs(xin);
                xin *= boxsize / Mathf.Abs(xin);
            }
            else
            {
                xin *= boxsize / Mathf.Abs(yin);
                yin *= boxsize / Mathf.Abs(yin);
            }
            transform.localPosition = new Vector3(xin, elevation, yin);
        }
        else
        {
            this.GetComponent<Renderer>().enabled = false;
            
        }
    }
}
