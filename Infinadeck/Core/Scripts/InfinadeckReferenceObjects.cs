using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * ------------------------------------------------------------
 * Script to modify the appearance and position of the Infinadeck Reference Objects.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

public class InfinadeckReferenceObjects : MonoBehaviour
{
	public GameObject referenceRing;
	public GameObject referenceEdge;
	public GameObject referenceCenter;
    public GameObject referencePanel;
    public GameObject heading;
    public Material syncMaterial;
    public GameObject deckModel;
    public GameObject referenceRig;
    public float currentTreadmillSpeed;
    public SkinnedMeshRenderer referencePanelSymbols;
    public SkinnedMeshRenderer referencePanelBand;
    public SkinnedMeshRenderer referencePanelTopBoundary;
    public SkinnedMeshRenderer referencePanelBottomBoundary;
    public SkinnedMeshRenderer referencePanelBackdrop;
    public SkinnedMeshRenderer[] rendRing;
    public SkinnedMeshRenderer rendEdge;
    public Material referencePanelBandMat;
    public Material referencePanelBoundaryMat;
    public Material referencePanelBackdropMat;
    public Material referencePanelSymbolMat;
    public Texture symbolStop;
    public Texture symbolGo;
    public Gradient grad;
    private int frameCount = 0;
    public InfinaDATA preferences;
    public InfinaDATA gamePreferences;
    private bool geoUpdating;
    private bool modelUpdating;
    public InfinadeckInterpreter iI;
    /**
     * Runs once on the object's first frame.
     */
    void OnEnable()
    {
        StartCoroutine(UpdateObjectModels());
    }

    private void Start()
    {
        if (!referenceRig)
        {
            Debug.LogWarning("INFINADECK WARNING: No RefObj ReferenceRig Assigned, Assuming Parent");
            if (this.transform.parent == null)
            {
                Debug.LogWarning("INFINADECK WARNING: No RefObj Parent, Assuming Self");
                referenceRig = this.gameObject;
            }
            else { referenceRig = this.transform.parent.gameObject; }
        }
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;
    }

    void OnDisable()
    {
        if (geoUpdating)
        {
            StopCoroutine(UpdateObjectGeometry());
            geoUpdating = false;
        }
        if (modelUpdating)
        {
            StopCoroutine(UpdateObjectModels());
            modelUpdating = false;
        }
    }

    /**
     * Runs every frame.
     */
    void Update()
    {
        frameCount++;
        if (frameCount >= preferences.ReadInt("dynamicColorblindFrames")) { frameCount = 0; }
        if (iI.InfIntGetTreadmillRunState)
        {
            SyncColor(Color.green);
            if (preferences.ReadBool("colorblindMode"))
            {
                referencePanelSymbolMat.SetTexture("_CutTex", symbolGo);
                if (preferences.ReadBool("dynamicColorblindElements"))
                {
                    referencePanelSymbolMat.SetTextureOffset("_CutTex", new Vector2(0, (float) frameCount / preferences.ReadInt("dynamicColorblindFrames")));
                }
            }
        }
        else
        {
            SyncColor(Color.red);
            if (preferences.ReadBool("colorblindMode"))
            {
                referencePanelSymbolMat.SetTexture("_CutTex", symbolStop);
                if (preferences.ReadBool("dynamicColorblindElements"))
                {
                    referencePanelSymbolMat.SetTextureOffset("_CutTex", new Vector2((float) frameCount / preferences.ReadInt("dynamicColorblindFrames"), 0));
                }
            }
        }

        if ((preferences.ReadBool("dynamicRingPanel")) && (preferences.ReadFloat("maxTreadmillSpeedMetersPerSecond") != 0))
        {
            float ratio = currentTreadmillSpeed / preferences.ReadFloat("maxTreadmillSpeedMetersPerSecond");
            if (ratio > 1) ratio = 1;
            if (ratio < 0) ratio = 0;

            if (preferences.ReadFloat("bandThicknessPercent") != 0)
            {
                float bt = preferences.ReadFloat("bandThicknessPercent");
                if (bt > 100) bt = 100;
                if (bt < 0.1f) bt = 0.1f;
                float workingRange = 100 - bt;
                float bottomMult = workingRange * ratio;
                float topMult = workingRange - bottomMult;
                referencePanelBand.SetBlendShapeWeight(0, topMult); //Top
                referencePanelBand.SetBlendShapeWeight(1, bottomMult); //Bottom
            }

            if (preferences.ReadBool("dynamicBackdrop")) { referencePanelBackdrop.SetBlendShapeWeight(0, 100 * (1 - ratio)); } //Top
            else { referencePanelBackdrop.SetBlendShapeWeight(0, 0); } //Top

            if (preferences.ReadInt("panelPalette") == 1) // [1] grey-BG white-boundary synced-band
            {
                referencePanelBandMat.SetColor("_Color2", grad.Evaluate(ratio));
            }
            else if (preferences.ReadInt("panelPalette") == 2) // [2] synced-BG white-boundary white-band
            {
                referencePanelBackdropMat.SetColor("_Color2", grad.Evaluate(ratio));
            }
            else if (preferences.ReadInt("panelPalette") == 3) // [3] no-BG white-boundary synced-band
            {
                referencePanelBandMat.SetColor("_Color2", grad.Evaluate(ratio));
            }
        }
    }

    /**
     * Set the sync material's color based on an input color.
     */
    void SyncColor(Color inColor)
    {
        syncMaterial.SetColor("_Color2", inColor);
    }

    /**
     * Infinite co-routine that cycles every 0.1 seconds;
     * Update the visibility of the Infinadeck Reference Objects in-game from the settings file.
     */
    IEnumerator UpdateObjectModels()
    {
        modelUpdating = true;
        while (true)
        {
            if (!preferences) yield return new WaitForSeconds(0.05f);
            if (!geoUpdating)
            {
                StartCoroutine(UpdateObjectGeometry());
                yield return new WaitForSeconds(0.05f);
            }
            deckModel.SetActive(preferences.ReadBool("deckVisibility"));
            heading.SetActive(preferences.ReadBool("deckHeadingVisibility"));
            referenceRing.SetActive(preferences.ReadBool("ringVisibility"));
            referenceEdge.SetActive(preferences.ReadBool("edgeVisibility"));
            referencePanel.SetActive(preferences.ReadBool("dynamicRingPanel"));
            referenceCenter.SetActive(preferences.ReadBool("centerVisibility"));
            
            int currentCenter = preferences.ReadInt("centerModel");
            foreach (Transform child in referenceCenter.transform)
            {
                if (child.GetSiblingIndex() == currentCenter) { child.gameObject.SetActive(true); }
                else { child.gameObject.SetActive(false); }
            }

            int currentRing = preferences.ReadInt("ringModel");
            foreach (Transform child in referenceRing.transform)
            {
                if (child.GetSiblingIndex() == currentRing) { child.gameObject.SetActive(true); }
                else { child.gameObject.SetActive(false); }
            }

            yield return new WaitForSeconds(.1f);
        }
    }

    /**
     * Infinite co-routine that cycles every 5 seconds;
     * Updates the geometry and colors of the Reference Objects in-game from the settings file.
     */
    IEnumerator UpdateObjectGeometry() {
        geoUpdating = true;
        while (true) {
            if (!preferences) yield return new WaitForSeconds(0.05f);
            Vector3 ringposition = Vector3.zero;
            if (preferences.ReadBool("overrideTreadmillPosition"))
            {
                this.transform.localPosition = new Vector3(preferences.ReadFloat("overrideX"), 0, preferences.ReadFloat("overrideY")); // 0 out vertical axis
                referenceRing.transform.localPosition = new Vector3(0, preferences.ReadFloat("overrideZ"), 0); // use ONLY vertical axis
                referencePanel.transform.localPosition = new Vector3(0, preferences.ReadFloat("overrideZ"), 0); // use ONLY vertical axis
            }
            else
            {
                ringposition = new Vector3((float)iI.InfIntGetRingValues.x, (float)iI.InfIntGetRingValues.z, (float)iI.InfIntGetRingValues.y);
                this.transform.localPosition = new Vector3(ringposition.x, 0, ringposition.z); // 0 out vertical axis
                referenceRing.transform.localPosition = new Vector3(0, ringposition.y, 0); // use ONLY vertical axis
                referencePanel.transform.localPosition = new Vector3(0, ringposition.y, 0); // use ONLY vertical axis
                PushCenterPreferencesToINI();
            }
            //offset based on specific game needs (typically only for debug, not intended for shipping)
            this.transform.localPosition += new Vector3
                    (
                    gamePreferences.ReadFloat("gameOverrideX"),
                    gamePreferences.ReadFloat("gameOverrideY"),
                    gamePreferences.ReadFloat("gameOverrideZ")
                    );

            foreach (SkinnedMeshRenderer r in rendRing)
            {
                // Thickness, .02m at 0, .04m at 100, equation:   diam = .02m + percent*.02m/100   (diam - .02m)*5000 = percent
                r.SetBlendShapeWeight(0, 5000 * (preferences.ReadFloat("ringThickness") - 0.02f));
                // Diameter, 2m at 0, 4m at 100, equation:   diam = 2m + percent*2m/100   (diam - 2m)*50 = percent 
                r.SetBlendShapeWeight(1, 50 * (preferences.ReadFloat("ringDiameter") - 2));
            }



            //OuterEdge, 4m at 0, 6m at 100, equation:   boxsize = 4m + percent*2m/100   (boxsize - 4m)*50 = percent         
            rendEdge.SetBlendShapeWeight(0, 50 * (preferences.ReadFloat("walkingSurfaceWidth") - 4));
            //InnerEdge, 1m at 0, 0m at 100, equation:   edgethk = 1m - percent*1m/100   (edgethk - 1m)*-100 = percent  
            rendEdge.SetBlendShapeWeight(1, -100 * (preferences.ReadFloat("walkingSurfaceEdgeThickness") - 1));
            heading.GetComponent<InfinadeckDeckHeading>().boxsize = preferences.ReadFloat("walkingSurfaceWidth") / 2;



            float widthRatio = preferences.ReadFloat("panelWidthM");
            float heightRatio = preferences.ReadFloat("panelHeightM");
            float diamMult = (preferences.ReadFloat("panelDiameterM") * 50 / widthRatio) - 100;
            referencePanel.transform.localScale = new Vector3(widthRatio, heightRatio, widthRatio);
            referencePanelBand.SetBlendShapeWeight(2, diamMult);
            referencePanelTopBoundary.SetBlendShapeWeight(2, diamMult);
            referencePanelBottomBoundary.SetBlendShapeWeight(2, diamMult);
            referencePanelBackdrop.SetBlendShapeWeight(2, diamMult);
            referencePanelBand.SetBlendShapeWeight(2, diamMult);
            referencePanelSymbols.SetBlendShapeWeight(2, diamMult);
            referencePanelTopBoundary.SetBlendShapeWeight(1, 100 - preferences.ReadFloat("topBoundaryThicknessPercent")); // Bottom
            referencePanelBottomBoundary.SetBlendShapeWeight(0, 100 - preferences.ReadFloat("bottomBoundaryThicknessPercent")); // Top

            if ((preferences.ReadFloat("bandThicknessPercent") != 0) && (preferences.ReadFloat("maxTreadmillSpeedMetersPerSecond") != 0)) { referencePanelBand.gameObject.SetActive(true); }
            else { referencePanelBand.gameObject.SetActive(false); }

            if (preferences.ReadInt("panelPalette") == 0) // [0] black-BG grey-boundary white-band
            {
                referencePanelBackdrop.gameObject.SetActive(true);
                referencePanelBackdropMat.SetColor("_Color2", Color.black);
                referencePanelBoundaryMat.SetColor("_Color2", Color.grey);
                referencePanelBandMat.SetColor("_Color2", Color.white);
            }
            else if (preferences.ReadInt("panelPalette") == 1) // [1] grey-BG white-boundary synced-band
            {
                referencePanelBackdrop.gameObject.SetActive(true);
                referencePanelBackdropMat.SetColor("_Color2", Color.grey);
                referencePanelBoundaryMat.SetColor("_Color2", Color.white);
            }
            else if (preferences.ReadInt("panelPalette") == 2) // [2] synced-BG white-boundary white-band
            {
                referencePanelBackdrop.gameObject.SetActive(true);
                referencePanelBoundaryMat.SetColor("_Color2", Color.white);
                referencePanelBandMat.SetColor("_Color2", Color.white);

            }
            else if (preferences.ReadInt("panelPalette") == 3) // [3] no-BG white-boundary synced-band
            {
                referencePanelBackdrop.gameObject.SetActive(false);
                referencePanelBoundaryMat.SetColor("_Color2", Color.white);
            }
            else if (preferences.ReadInt("panelPalette") == 4) // [4] no-BG white-boundary white-band
            {
                referencePanelBackdrop.gameObject.SetActive(false);
                referencePanelBoundaryMat.SetColor("_Color2", Color.white);
                referencePanelBandMat.SetColor("_Color2", Color.white);
            }

            if (preferences.ReadBool("colorblindMode"))
            {
                if (!preferences.ReadBool("dynamicRingPanel"))
                {
                    Debug.Log("Warning: Infinadeck Colorblind Mode requires that the dynamicPanel is active. Settings file updating accordingly.");
                    preferences.Write("dynamicRingPanel", "true");
                }
                referencePanelSymbols.gameObject.SetActive(true); 
            }
            else
            {
                referencePanelSymbols.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(5.0f);
        }
        
	}

    /**
     * Change the visibility of the Reference Ring.
     */
    public void ToggleDeckRing()
    {
        referenceRing.SetActive(!referenceRing.activeSelf);
        preferences.Write("ringVisibility", referenceRing.activeSelf.ToString());
    }

    /**
     * Change the visibility of the Reference Edge.
     */
    public void ToggleDeckEdge()
    {
        referenceEdge.SetActive(!referenceEdge.activeSelf);
        preferences.Write("edgeVisibility", referenceEdge.activeSelf.ToString());
    }

    /**
     * Change the visibility of the Reference Center.
     */
    public void ToggleDeckCenter()
    {
        referenceCenter.SetActive(!referenceCenter.activeSelf);
        preferences.Write("centerVisibility", referenceCenter.activeSelf.ToString());
    }

    /**
     * Change the active Reference Center model.
     */
    public void CycleDeckCenter()
    {
        int currentChild = 0;
        foreach (Transform child in referenceCenter.transform)
        {
            if (child.gameObject.activeSelf) { currentChild = child.GetSiblingIndex(); }
        }
        int nextChild = currentChild + 1;
        if (nextChild >= referenceCenter.transform.childCount) { nextChild = 0; }
        referenceCenter.transform.GetChild(currentChild).gameObject.SetActive(false);
        referenceCenter.transform.GetChild(nextChild).gameObject.SetActive(true);
        preferences.Write("centerModel", nextChild.ToString());
    }

    /**
     * Change the active Reference Ring model.
     */
    public void CycleRingModel()
    {
        int currentChild = 0;
        foreach (Transform child in referenceRing.transform)
        {
            if (child.gameObject.activeSelf) { currentChild = child.GetSiblingIndex(); }
        }
        int nextChild = currentChild + 1;
        if (nextChild >= referenceRing.transform.childCount) { nextChild = 0; }
        referenceRing.transform.GetChild(currentChild).gameObject.SetActive(false);
        referenceRing.transform.GetChild(nextChild).gameObject.SetActive(true);
        preferences.Write("ringModel", nextChild.ToString());
    }

    /**
     * Change the visibility of the in-game Deck.
     */
    public void ToggleInEngineDeck()
    {
        deckModel.SetActive(!deckModel.activeSelf);
        preferences.Write("deckVisibility", deckModel.activeSelf.ToString());
    }

    /**
     * Change the visibility of the Deck Heading.
     */
    public void ToggleHeading()
    {
        heading.SetActive(!heading.activeSelf);
        preferences.Write("deckHeadingVisibility", heading.activeSelf.ToString());
    }

    /**
     * Change the visibility of the Reference Panel.
     */
    public void ToggleReferencePanel()
    {
        referencePanel.SetActive(!referencePanel.activeSelf);
        preferences.Write("dynamicRingPanel", referencePanel.activeSelf.ToString());
    }

    /**
     * Change the presence of colorblind mode, enabling the ReferencePanel if necessary.
     */
    public void ToggleColorblind()
    {
        referencePanelSymbols.gameObject.SetActive(!referencePanelSymbols.gameObject.activeSelf);
        preferences.Write("colorblindMode", referencePanelSymbols.gameObject.activeSelf.ToString());
        if (preferences.ReadBool("colorblindMode"))
        {
            if (!preferences.ReadBool("dynamicRingPanel"))
            {
                Debug.Log("Warning: Infinadeck Colorblind Mode requires that the dynamicPanel is active. Settings file updating accordingly.");
                preferences.Write("dynamicRingPanel", "true");
            }
            referencePanelSymbols.gameObject.SetActive(true);
        }
        else
        {
            referencePanelSymbols.gameObject.SetActive(false);
        }
    }

    /**
     * Change the active Panel theme.
     */
    public void CyclePanelTheme()
    {
        preferences.Write("panelPalette", ((preferences.ReadInt("panelPalette") + 1) % 5).ToString());

        if (preferences.ReadInt("panelPalette") == 0) // [0] black-BG grey-boundary white-band
        {
            referencePanelBackdrop.gameObject.SetActive(true);
            referencePanelBackdropMat.SetColor("_Color2", Color.black);
            referencePanelBoundaryMat.SetColor("_Color2", Color.grey);
            referencePanelBandMat.SetColor("_Color2", Color.white);
        }
        else if (preferences.ReadInt("panelPalette") == 1) // [1] grey-BG white-boundary synced-band
        {
            referencePanelBackdrop.gameObject.SetActive(true);
            referencePanelBackdropMat.SetColor("_Color2", Color.grey);
            referencePanelBoundaryMat.SetColor("_Color2", Color.white);
        }
        else if (preferences.ReadInt("panelPalette") == 2) // [2] synced-BG white-boundary white-band
        {
            referencePanelBackdrop.gameObject.SetActive(true);
            referencePanelBoundaryMat.SetColor("_Color2", Color.white);
            referencePanelBandMat.SetColor("_Color2", Color.white);

        }
        else if (preferences.ReadInt("panelPalette") == 3) // [3] no-BG white-boundary synced-band
        {
            referencePanelBackdrop.gameObject.SetActive(false);
            referencePanelBoundaryMat.SetColor("_Color2", Color.white);
        }
        else if (preferences.ReadInt("panelPalette") == 4) // [4] no-BG white-boundary white-band
        {
            referencePanelBackdrop.gameObject.SetActive(false);
            referencePanelBoundaryMat.SetColor("_Color2", Color.white);
            referencePanelBandMat.SetColor("_Color2", Color.white);
        }

    }

    /**
     * Updates the deck center preference values from the in game values.
     */
    public void PushCenterPreferencesToINI()
    {
        if ((preferences.ReadFloat("overrideX") != this.transform.localPosition.x) ||
            (preferences.ReadFloat("overrideY") != this.transform.localPosition.z) ||
            (preferences.ReadFloat("overrideZ") != referenceRing.transform.localPosition.y))
        {
            preferences.Write("overrideX", this.transform.localPosition.x.ToString());
            preferences.Write("overrideY", this.transform.localPosition.z.ToString());
            preferences.Write("overrideZ", referenceRing.transform.localPosition.y.ToString());
        }
    }
}