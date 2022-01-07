using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Infinadeck;

/**
 * ------------------------------------------------------------
 * Script to control the existence of the Infinadeck Splashscreen.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

public class InfinadeckSplashscreen : MonoBehaviour {
	private TreadmillInfo deckInfo;
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
        // update the splashscreen with relevant info, pull from deck itself
        deckInfo = Infinadeck.Infinadeck.GetTreadmillInfo();
        if (deckInfo.id != null) { deckSN.text = "Deck SN: " + deckInfo.id; }
        if (deckInfo.model_number != null) { modelNumber.text = "Model Number: " + deckInfo.model_number; }
        if (deckInfo.dll_version != null) { APIVersion.text = "API Version: " + deckInfo.dll_version; }
        // pluginVersion.text already set by Spawner when instantiated

        // position the splashscreen such that it's visible.
        this.transform.eulerAngles = new Vector3(0, headset.transform.eulerAngles.y, 0);
        this.transform.position += new Vector3(0, headset.transform.position.y, 0);
        this.transform.localScale = worldScale;
        Destroy(this.gameObject, 3.0f);
    }
}