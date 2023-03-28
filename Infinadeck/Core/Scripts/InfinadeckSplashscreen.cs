using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * ------------------------------------------------------------
 * Script to control the existence of the Infinadeck Splashscreen.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

public class InfinadeckSplashscreen : MonoBehaviour {
    public InfinadeckInterpreter iI;
    public Text deckSN;
	public Text modelNumber;
	public Text APIVersion;
    public Text pluginVersion;
    public GameObject headset;

    public Vector3 worldScale = Vector3.one;

    /**
     * Runs once on the object's first frame.
     */
    void Start() {
        // position the splashscreen such that it's visible.
        this.transform.eulerAngles = new Vector3(0, headset.transform.eulerAngles.y, 0);
        this.transform.position += new Vector3(0, headset.transform.position.y, 0);
        this.transform.localScale = worldScale;
        Destroy(this.gameObject, 3.0f);
    }

    private void Update()
    {
        // update the splashscreen with relevant info, pull from deck itself
        if (iI.InfIntGetTreadmillInfoID != null) { deckSN.text = "Deck SN: " + iI.InfIntGetTreadmillInfoID; }
        if (iI.InfIntGetTreadmillInfoModel_Number != null) { modelNumber.text = "Model Number: " + iI.InfIntGetTreadmillInfoModel_Number; }
        if (iI.InfIntGetTreadmillInfoDLL_Version != null) { APIVersion.text = "API Version: " + iI.InfIntGetTreadmillInfoDLL_Version; }
    }
}