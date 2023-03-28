using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * ------------------------------------------------------------
 * Main script for management of the Infinadeck plugin.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

public class InfinadeckCore : MonoBehaviour
{
    [InfReadOnlyInEditor] public string pluginVersion = "3.3.0";
    [InfReadOnlyInEditor] public GameObject refObjects;
    [InfReadOnlyInEditor] public GameObject locomotion;
    [InfReadOnlyInEditor] public GameObject splashScreen;
    [InfReadOnlyInEditor] public GameObject demo;
    private InfinadeckReferenceObjects infinadeckReferenceObjects;
    private InfinadeckLocomotion infinadeckLocomotion;
    private InfinadeckSplashscreen infinadeckSplashScreen;
    private InfinaDEMO infinaDEMO;
    public InfinadeckInterpreter iI;

    public bool autoStart = true;
    private bool booted = false;
    public bool firstLevel = true;
    public bool movementLevel = true;
    public bool guaranteeDestroyOnLoad = false;
    public bool correctPosition = false;
    public bool correctRotation = false;
    public bool correctScale = false;
    public bool showCollisions = false;
    public bool showTreadmillVelocity = false;
    private bool initialized = false;
    private bool localPLUGINactive = false;
    private bool localKEYBINDSactive = false;
    private bool localDEMOactive = false;
    private bool localHIDEactive = false;
    public string guiOutput;

    public InfinaDATA preferences;
    public Dictionary<string, InfinaDATA.DataEntry> defaultPreferences;
    public InfinaDATA keybinds;
    public Dictionary<string, InfinaDATA.DataEntry> defaultKeybinds;
    public InfinaDATA gamePreferences;
    public Dictionary<string, InfinaDATA.DataEntry> defaultGamePreferences;


    public GameObject cameraRig;
    public GameObject headset;
    public float speedGain = 1;
    public Vector3 worldScale = Vector3.one;

    private Texture2D textBG;

    /**
     * Runs upon the moment of creation of this object.
     */
    void Awake()
    {
        // Delete Placement Geometry
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        iI = this.gameObject.AddComponent<InfinadeckInterpreter>();
        iI.enabled = false;

        // Initialize Preferences
        preferences = this.gameObject.AddComponent<InfinaDATA>();
        preferences.fileLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Infinadeck/Config/";
        preferences.fileName = "settings.ini";
        defaultPreferences = new Dictionary<string, InfinaDATA.DataEntry>
        {
            // 000: General Preferences
            { "pluginEnabled", new InfinaDATA.DataEntry { EntryName = "000 General", EntryValue = "true" } },
            { "hideNotifications", new InfinaDATA.DataEntry { EntryName = "000 General", EntryValue = "false" } },
            { "demoMode", new InfinaDATA.DataEntry { EntryName = "000 General", EntryValue = "false" } },
            { "keyboardInputEnabled", new InfinaDATA.DataEntry { EntryName = "000 General", EntryValue = "true" } },
            { "rightHandDominant", new InfinaDATA.DataEntry { EntryName = "000 General", EntryValue = "true" } },
            { "rightFootDominant", new InfinaDATA.DataEntry { EntryName = "000 General", EntryValue = "true" } },
            
            // 010: Reference Object General Preferences
            { "overrideTreadmillPosition", new InfinaDATA.DataEntry { EntryName = "010 ReferenceObject - General", EntryValue = "true" } },
            { "overrideX", new InfinaDATA.DataEntry { EntryName = "010 ReferenceObject - General", EntryValue = "0.0000" } },
            { "overrideY", new InfinaDATA.DataEntry { EntryName = "010 ReferenceObject - General", EntryValue = "0.0000" } },
            { "overrideZ", new InfinaDATA.DataEntry { EntryName = "010 ReferenceObject - General", EntryValue = "1.0000" } },

            // 011: Reference Object #1- Ring Preferences
            { "ringVisibility", new InfinaDATA.DataEntry { EntryName = "011 ReferenceObject01 - Ring", EntryValue = "true" } },
            { "ringModel", new InfinaDATA.DataEntry { EntryName = "011 ReferenceObject01 - Ring", EntryValue = "1" } },
            { "ringDiameter", new InfinaDATA.DataEntry { EntryName = "011 ReferenceObject01 - Ring", EntryValue = "1.2954" } },
            { "ringThickness", new InfinaDATA.DataEntry { EntryName = "011 ReferenceObject01 - Ring", EntryValue = ".0381" } },

            // 012: Reference Object #2- Center Mark Preferences
            { "centerVisibility", new InfinaDATA.DataEntry { EntryName = "012 ReferenceObject02 - CenterMark", EntryValue = "true" } },
            { "centerModel", new InfinaDATA.DataEntry { EntryName = "012 ReferenceObject02 - CenterMark", EntryValue = "0" } },

            // 013: Reference Object #3- Edge Preferences
            { "edgeVisibility", new InfinaDATA.DataEntry { EntryName = "013 ReferenceObject03 - Edge", EntryValue = "true" } },
            { "walkingSurfaceWidth", new InfinaDATA.DataEntry { EntryName = "013 ReferenceObject03 - Edge", EntryValue = "1.2192" } },
            { "walkingSurfaceEdgeThickness", new InfinaDATA.DataEntry { EntryName = "013 ReferenceObject03 - Edge", EntryValue = ".03" } },

            // 014: Reference Object #4- (In Engine) Deck Preferences
            { "deckVisibility", new InfinaDATA.DataEntry { EntryName = "014 ReferenceObject04 - Deck", EntryValue = "false" } },
            { "deckHeadingVisibility", new InfinaDATA.DataEntry { EntryName = "014 ReferenceObject04 - Deck", EntryValue = "false" } },

            // 015: Reference Object #5- Dynamic Panel Preferences
            { "dynamicRingPanel", new InfinaDATA.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "false" } },
            { "panelWidthM", new InfinaDATA.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "0.15" } },
            { "panelHeightM", new InfinaDATA.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "0.05" } },
            { "panelDiameterM", new InfinaDATA.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "1.3" } },
            { "bandThicknessPercent", new InfinaDATA.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "6" } },
            { "topBoundaryThicknessPercent", new InfinaDATA.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "3" } },
            { "bottomBoundaryThicknessPercent", new InfinaDATA.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "4" } },
            { "dynamicBackdrop", new InfinaDATA.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "false" } },
            { "panelPalette", new InfinaDATA.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "0" } },
            { "colorblindMode", new InfinaDATA.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "false" } },
            { "dynamicColorblindElements", new InfinaDATA.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "true" } },
            { "dynamicColorblindFrames", new InfinaDATA.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "2000" } },
            { "maxTreadmillSpeedMetersPerSecond", new InfinaDATA.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "2" } },

            // 019: Reference Object Reference Info
            { "ringModel_AS_0", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "minimum_detail:_48_verts_(96_tris)" } },
            { "ringModel_AS_1", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "low_detail:_128_verts_(256_tris)" } },
            { "ringModel_AS_2", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "medium_detail:_256_verts_(512_tris)" } },
            { "ringModel_AS_3", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "high_detail:_512_verts_(1024_tris)" } },
            { "ringModel_AS_4", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "maximum_detail:_1024_verts_(2048_tris)" } },
            { "ringModel_AS_5", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "ludicrous_detail:_4096_verts_(8192_tris)" } },
            { "centerModel_AS_0", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "target" } },
            { "centerModel_AS_1", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "cross" } },
            { "centerModel_AS_2", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "extended" } },
            { "centerModel_AS_3", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "feet" } },
            { "centerModel_AS_4", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "star" } },
            { "panelPalette_AS_0", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "black-BG_grey-boundary_white-band" } },
            { "panelPalette_AS_1", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "grey-BG_white-boundary_synced-band" } },
            { "panelPalette_AS_2", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "synced-BG_white-boundary_white-band" } },
            { "panelPalette_AS_3", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "no-BG_white-boundary_synced-band" } },
            { "panelPalette_AS_4", new InfinaDATA.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "no-BG_white-boundary_white-band" } },

            // 700: Operational
            { "crashCheck", new InfinaDATA.DataEntry { EntryName = "700 Operational", EntryValue = "false" } }
        };
        preferences.all = defaultPreferences;
        preferences.InitMe(false); // keep any existing settings

        keybinds = this.gameObject.AddComponent<InfinaDATA>();
        keybinds.fileLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Infinadeck/Config/";
        keybinds.fileName = "keybinds.ini";
        defaultKeybinds = new Dictionary<string, InfinaDATA.DataEntry>
        {
            // 999: Keybind Reference Info
            { "FUNC", new InfinaDATA.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "F1-F2-F3-F4-F5-F6-F7-F8-F9-F10-F11-F12" } },
            { "1234", new InfinaDATA.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "Alpha1-Alpha2-Alpha3-Alpha4-Alpha5-Alpha6-Alpha7-Alpha8-Alpha9-Alpha0-Minus-Equals" } },
            { "#PAD", new InfinaDATA.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "Keypad1-Keypad2-Keypad3-Keypad4-Keypad5-Keypad6-Keypad7-Keypad8-Keypad9-KeypadDivide-KeypadMultiply-KeypadMinus" } },
            { "STND", new InfinaDATA.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "LeftShift-LeftControl-LeftAlt-Space-RightShift-RightControl-RightAlt-Return-BackQuote-Tab-Backslash-Backspace" } },
            { "CPAD", new InfinaDATA.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "LeftArrow-DownArrow-RightArrow-UpArrow-Delete-End-PageDown-Insert-Home-PageUp-ScrollLock-Pause" } },
            { "QWER", new InfinaDATA.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "Q-W-E-R-T-Y-U-I-O-P-LeftBracket-RightBracket" } },
            { "ASDF", new InfinaDATA.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "A-S-D-F-G-H-J-K-L-Semicolon-Quote-Slash" } },
            { "Custom", new InfinaDATA.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "karses_the_12_keys_listed_in_the_keybindProfile" } }
        };
        keybinds.all = defaultKeybinds;
        keybinds.InitMe(false);// keep any existing settings


        gamePreferences = this.gameObject.AddComponent<InfinaDATA>();
        string[] projectPath = Application.dataPath.Split('/');
        string projectName = projectPath[projectPath.Length - 2];
        gamePreferences.fileLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Infinadeck/Games/" + projectName + "/";
        gamePreferences.fileName = "gameSettings.ini";
        defaultGamePreferences = new Dictionary<string, InfinaDATA.DataEntry>
        {
            // 800: General Game Preferences
            { "gameOverrideX", new InfinaDATA.DataEntry { EntryName = "800 General Game Preferences", EntryValue = "0.0000" } },
            { "gameOverrideY", new InfinaDATA.DataEntry { EntryName = "800 General Game Preferences", EntryValue = "0.0000" } },
            { "gameOverrideZ", new InfinaDATA.DataEntry { EntryName = "800 General Game Preferences", EntryValue = "0.0000" } }
        };
        gamePreferences.all = defaultGamePreferences;
        gamePreferences.InitMe(false); // keep any existing settings

        textBG = Resources.Load("Textures/Inf_blackBG") as Texture2D;

        InputCheck();

        if (preferences.ReadBool("crashCheck"))
        {
            Debug.Log("INFINADECK NOTICE: Prior project crashed or closed without calling OnApplicationQuit on InfinadeckCore; starting in Safe Mode. Press Ctrl+I to enable the Infinadeck Plugin.");
            preferences.Write("pluginEnabled", "false");
            iI.enabled = false;
        }

        else
        {
            preferences.Write("crashCheck", "true");
            if (autoStart) { Boot(); }
            else { Debug.Log("INFINADECK NOTICE: 'Auto Start' is disabled. Please start Infinadeck Plugin by calling the Boot() function on the instance of [Infinadeck] in your Scene."); }
            iI.enabled = true;
        }
    }

    /**
     * Core Boot function. Only manually call to initialize the setup when AutoStart is disabled, or to re-enable after calling Shutdown.
     */
    public void Boot()
    {
        if (!booted)
        {
            booted = true;
            StartCoroutine(InitializeWithErrorChecks());
            StartCoroutine(SpawnSubcomponents());
        }
        else Debug.LogWarning("INFINADECK WARNING: Infinadeck Plugin is already booted; if manual boot is desired, un-check 'Auto Start' on the instance of [Infinadeck] in your Scene.");
    }

    /**
     * Core Shutdown function. Only manually call to stop existing threads- removes treadmill code entirely.
     */
    public void Shutdown()
    {
        if (booted)
        {
            booted = false;
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
        }
        else Debug.LogWarning("INFINADECK WARNING: Infinadeck Plugin is not currently booted; shutdown request ignored.");
    }

    private void OnApplicationQuit()
    {
        preferences.Write("crashCheck", "false");
        preferences.SaveSettings();
    }

    /**
     * Initialization of the Infinadeck plugin, parenting it to the appropriate components, along with error checking.
     */
    private IEnumerator InitializeWithErrorChecks()
    {
        if (!cameraRig)
        {
            Debug.LogWarning("INFINADECK WARNING: No CameraRig Reference Assigned, Assuming Parented to CameraRig");
            if (this.transform.parent == null)
            {
                Debug.LogWarning("INFINADECK WARNING: No CameraRig Reference Assigned and No Parent, Self is CameraRig");
                cameraRig = this.gameObject;
            }
            else { cameraRig = this.transform.parent.gameObject; }
        }
        else
        {
            this.transform.parent = cameraRig.transform;
        }
        if (correctPosition) { this.transform.localPosition = Vector3.zero; }
        if (correctRotation) { this.transform.localRotation = Quaternion.identity; }
        if (correctScale) { this.transform.localScale = Vector3.one; }

        if (!headset)
        {
            Debug.LogWarning("INFINADECK WARNING: No Headset Reference Assigned, Assuming Main Camera is Correct");
            headset = Camera.main.gameObject;
        }
        initialized = true;
        yield return null;
    }

    /**
     * Spawn the individual elements of the Infinadeck plugin, based on the needs of the current scene.
     */
    private IEnumerator SpawnSubcomponents()
    {
        while (!initialized) { yield return new WaitForSeconds(1f); }
        if (firstLevel) // Only spawn the following if actually needed this level
        {
            //Spawn Splashscreen
            splashScreen = Instantiate(Resources.Load("RuntimePrefabs/InfinadeckSplashscreen") as GameObject, transform.position, Quaternion.identity);
            infinadeckSplashScreen = splashScreen.GetComponent<InfinadeckSplashscreen>();
            infinadeckSplashScreen.headset = headset;
            infinadeckSplashScreen.worldScale = worldScale;
            infinadeckSplashScreen.pluginVersion.text = "Plugin Version: " + pluginVersion;
            infinadeckSplashScreen.iI = iI;
        }

        // Spawn Reference Objects
        refObjects = Instantiate(Resources.Load("RuntimePrefabs/InfinadeckReferenceObjects") as GameObject, transform.position, Quaternion.identity);
        refObjects.transform.parent = this.transform;
        infinadeckReferenceObjects = refObjects.GetComponent<InfinadeckReferenceObjects>();
        infinadeckReferenceObjects.worldScale = worldScale;
        infinadeckReferenceObjects.preferences = preferences;
        infinadeckReferenceObjects.gamePreferences = gamePreferences;
        infinadeckReferenceObjects.iI = iI;

        if (movementLevel) // Only spawn the following if actually needed this level
        {
            // Spawn Locomotion
            locomotion = Instantiate(Resources.Load("RuntimePrefabs/InfinadeckLocomotion") as GameObject, transform.position, Quaternion.identity);
            locomotion.transform.parent = this.transform;
            infinadeckLocomotion = locomotion.GetComponent<InfinadeckLocomotion>();
            infinadeckLocomotion.cameraRig = cameraRig;
            infinadeckLocomotion.worldScale = worldScale;
            infinadeckLocomotion.speedGain = speedGain;
            infinadeckLocomotion.infinadeckReferenceObj = refObjects.GetComponent<InfinadeckReferenceObjects>();
            infinadeckLocomotion.showCollisions = showCollisions;
            infinadeckLocomotion.showTreadmillVelocity = showTreadmillVelocity;
            infinadeckLocomotion.iI = iI;
        }

        // Spawn InfinaDEMO
        demo = Instantiate(Resources.Load("RuntimePrefabs/InfinaDEMO") as GameObject, transform.position, Quaternion.identity);
        demo.transform.parent = this.transform;
        infinaDEMO = demo.GetComponent<InfinaDEMO>();
        infinaDEMO.holder = this.gameObject;
        infinaDEMO.iI = iI;
        infinaDEMO.preferences = preferences;
    }

    /**
     * Runs whenever the object is enabled.
     */
    void OnEnable()
    {
        SceneManager.sceneUnloaded += LevelChange;
    }

    /**
     * Runs whenever the object is disabled.
     */
    void OnDisable()
    {
        SceneManager.sceneUnloaded -= LevelChange;
    }

    /**
     * Runs when the level is changing or reloading.
     */
    private void LevelChange(Scene scene)
    {
        if (guaranteeDestroyOnLoad) { Destroy(this.gameObject); }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            EnableOrDisable();
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) // Exits the game.
        {
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Equals)) // Hides notifications.
        {
            if (localHIDEactive) { preferences.Write("hideNotifications", "false"); }
            else { preferences.Write("hideNotifications", "true"); }
        }

        else
        {
            localPLUGINactive = preferences.ReadBool("pluginEnabled");
            localKEYBINDSactive = preferences.ReadBool("keyboardInputEnabled");
            localDEMOactive = preferences.ReadBool("demoMode");
            localHIDEactive = preferences.ReadBool("hideNotifications");

            if (!localPLUGINactive)
            {
                if (refObjects && refObjects.activeSelf) { refObjects.SetActive(false); }
                if (locomotion && locomotion.activeSelf) { locomotion.SetActive(false); }
                if (demo && demo.activeSelf) { demo.SetActive(false); }
            }
            else
            {
                iI.enabled = true;
                if (refObjects && !refObjects.activeSelf) { refObjects.SetActive(true); }
                if (locomotion && !locomotion.activeSelf) { locomotion.SetActive(true); }
                if (demo)
                {
                    if (!demo.activeSelf && localDEMOactive) { demo.SetActive(true); }
                    else if (demo.activeSelf && !localDEMOactive) { demo.SetActive(false); }
                }
                
                if (Input.anyKeyDown)
                {
                    InputCheck();
                }
            }
        }
    }

    public void InputCheck()
    {
        if (CheckFuncByKeybind("ReloadCurrentLevel", "901- Treadmill", "LeftShift"))
        {
            if (guaranteeDestroyOnLoad) {
                preferences.Write("crashCheck", "false");
                preferences.SaveSettings();
            }  
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (CheckFuncByKeybind("StopTreadmill", "901- Treadmill", "Space"))
        {
            iI.InfIntStopTreadmill();
        }
        if (CheckFuncByKeybind("StartTreadmill", "901- Treadmill", "RightShift"))
        {
            iI.InfIntStartTreadmillUserControl();
        }
        if (CheckFuncByKeybind("ImportPreferences", "901- Treadmill", "Backslash"))
        {
            ImportPreferences();
        }
        if (CheckFuncByKeybind("ResetPreferences", "901- Treadmill", "Backspace"))
        {
            ResetPreferences();
        }

        if (CheckFuncByKeybind("ToggleDeckRing", "902- Reference Objects", "Q"))
        {
            if (infinadeckReferenceObjects) infinadeckReferenceObjects.ToggleDeckRing();
        }
        if (CheckFuncByKeybind("ToggleDeckEdge", "902- Reference Objects", "W"))
        {
            if (infinadeckReferenceObjects) infinadeckReferenceObjects.ToggleDeckEdge();
        }
        if (CheckFuncByKeybind("ToggleDeckCenter", "902- Reference Objects", "E"))
        {
            if (infinadeckReferenceObjects) infinadeckReferenceObjects.ToggleDeckCenter();
        }
        if (CheckFuncByKeybind("ToggleReferencePanel", "902- Reference Objects", "R"))
        {
            if (infinadeckReferenceObjects) infinadeckReferenceObjects.ToggleReferencePanel();
        }
        if (CheckFuncByKeybind("ToggleInEngineDeck", "902- Reference Objects", "T"))
        {
            if (infinadeckReferenceObjects) infinadeckReferenceObjects.ToggleInEngineDeck();
        }
        if (CheckFuncByKeybind("ToggleHeading", "902- Reference Objects", "Y"))
        {
            if (infinadeckReferenceObjects) infinadeckReferenceObjects.ToggleHeading();
        }
        if (CheckFuncByKeybind("ToggleColorblind", "902- Reference Objects", "U"))
        {
            if (infinadeckReferenceObjects) infinadeckReferenceObjects.ToggleColorblind();
        }
        if (CheckFuncByKeybind("CyclePanelTheme", "902- Reference Objects", "I"))
        {
            if (infinadeckReferenceObjects) infinadeckReferenceObjects.CyclePanelTheme();
        }
        if (CheckFuncByKeybind("CycleDeckCenter", "902- Reference Objects", "O"))
        {
            if (infinadeckReferenceObjects) infinadeckReferenceObjects.CycleDeckCenter();
        }

        if (CheckFuncByKeybind("SetTimer1Minute", "903- Demo", "Alpha1"))
        {
            if (infinaDEMO) infinaDEMO.SetTheTimer(60);
        }
        if (CheckFuncByKeybind("SetTimer2Minute", "903- Demo", "Alpha2"))
        {
            if (infinaDEMO) infinaDEMO.SetTheTimer(120);
        }
        if (CheckFuncByKeybind("SetTimer3Minute", "903- Demo", "Alpha3"))
        {
            if (infinaDEMO) infinaDEMO.SetTheTimer(180);
        }
        if (CheckFuncByKeybind("SetTimer4Minute", "903- Demo", "Alpha4"))
        {
            if (infinaDEMO) infinaDEMO.SetTheTimer(240);
        }
        if (CheckFuncByKeybind("SetTimer5Minute", "903- Demo", "Alpha5"))
        {
            if (infinaDEMO) infinaDEMO.SetTheTimer(300);
        }
        if (CheckFuncByKeybind("SetTimer6Minute", "903- Demo", "Alpha6"))
        {
            if (infinaDEMO) infinaDEMO.SetTheTimer(360);
        }
        if (CheckFuncByKeybind("SetTimer7Minute", "903- Demo", "Alpha7"))
        {
            if (infinaDEMO) infinaDEMO.SetTheTimer(420);
        }
        if (CheckFuncByKeybind("SetTimer8Minute", "903- Demo", "Alpha8"))
        {
            if (infinaDEMO) infinaDEMO.SetTheTimer(480);
        }
        if (CheckFuncByKeybind("SetTimer9Minute", "903- Demo", "Alpha9"))
        {
            if (infinaDEMO) infinaDEMO.SetTheTimer(540);
        }
        if (CheckFuncByKeybind("SetTimer10Minute", "903- Demo", "Alpha0"))
        {
            if (infinaDEMO) infinaDEMO.SetTheTimer(600);
        }
        if (CheckFuncByKeybind("ToggleDemoMode", "903- Demo", "Minus"))
        {
            if (infinaDEMO) infinaDEMO.ToggleDemoMode();
        }
    }

    public bool CheckFuncByKeybind(string funcName, string funcGroup, string defaultKey)
    {
        if (!keybinds.all.ContainsKey(funcName)) // if the function is not one we are already watching for:
        {
            // set the default values in keybinds.all
            keybinds.all.Add(funcName, new InfinaDATA.DataEntry { EntryName = funcGroup, EntryValue = defaultKey, WriteFlag = true });
            // tell keybinds to reimport, now that it knows about this keybind, to grab the user preferred keybind if it exists
            keybinds.LoadSettings();
            Debug.Log(funcName + "_" + funcGroup + "_" + defaultKey + " assigned");
        }
        return Input.GetKeyDown(StringToKey(keybinds.ReadString(funcName)));
    }

    public KeyCode StringToKey(string keyName)
    {
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), keyName);
    }

    /**
     * Enables the plugin if not running; Disables it if running. The only user accessible way to toggle the plugin, shy of modifying settings.ini
     */
    public void EnableOrDisable()
    {
        if (preferences.ReadBool("pluginEnabled")) { preferences.Write("pluginEnabled", "false"); }
        else { preferences.Write("pluginEnabled", "true"); if (!booted) Boot(); }
    }

    /**
     * Imports the preferences from the settings file.
     */
    public void ImportPreferences()
    {
        preferences.LoadSettings();
        keybinds.LoadSettings();
        gamePreferences.LoadSettings();
    }

    /**
     * Resets the settings file to the default preferences.
     */
    public void ResetPreferences()
    {
        preferences.all = defaultPreferences;
        foreach (KeyValuePair<string, InfinaDATA.DataEntry> pref in preferences.all)
        {
            pref.Value.WriteFlag = true;
        }
        keybinds.all = defaultKeybinds;
        foreach (KeyValuePair<string, InfinaDATA.DataEntry> pref in keybinds.all)
        {
            pref.Value.WriteFlag = true;
        }
        gamePreferences.all = defaultGamePreferences;
        foreach (KeyValuePair<string, InfinaDATA.DataEntry> pref in gamePreferences.all)
        {
            pref.Value.WriteFlag = true;
        }
    }

    /**
     * Graphical elements for usage clarity.
     */
    void OnGUI()
    {
        if (!localHIDEactive)
        {
            if (iI.Connected)
            {
                if (localPLUGINactive) // IDA present, Plugin is Enabled
                {
                    if (localKEYBINDSactive)
                    {
                        string keybindTREADString = "";
                        string keybindREFOBJString = "";
                        string keybindDEMOString = "";
                        foreach (KeyValuePair<string, InfinaDATA.DataEntry> pref in keybinds.all)
                        {
                            if (pref.Value.EntryName == "901- Treadmill") { keybindTREADString += pref.Value.EntryValue + " to " + pref.Key + "\n"; }
                            else if (pref.Value.EntryName == "902- Reference Objects") { keybindREFOBJString += pref.Value.EntryValue + " to " + pref.Key + "\n"; }
                            else if (pref.Value.EntryName == "903- Demo") { keybindDEMOString += pref.Value.EntryValue + " to " + pref.Key + "\n"; }
                        }
                        guiOutput = "<b>INFINADECK</b>" + "   <color=red>" + iI.errorInfo + "</color>\n"
                            + "Escape to QuitGame, Ctrl+I to TogglePlugin, = to HideGUI\n"
                            + "\n[Treadmill]\n"
                            + keybindTREADString
                            + "\n[Reference Objects]\n"
                            + keybindREFOBJString
                            + "\n[Demo]\n"
                            + keybindDEMOString
                            + "\n\nAll keybinds listed in\n"
                            + "My Documents/My Games/Infinadeck/Config/keybinds.ini\n"
                            + "\n"
                            + "Contact us at <b>support@infinadeck.com</b> for more assistance\n";
                        GUI.DrawTexture(new Rect(Screen.width - 700, Screen.height - 700, 400, 1200), textBG, ScaleMode.StretchToFill, false, 0);
                        GUI.Label(new Rect(Screen.width - 685, Screen.height - 685, 400, 1200), guiOutput);
                    }
                    else
                    {
                        guiOutput = "<b>INFINADECK</b>" + "   <color=red>" + iI.errorInfo + "</color>\n"
                            + "No keybinds active\n"
                            + "\n"
                            + "Enable them by setting 'keyboardInputEnabled = true in'\n"
                            + "My Documents/My Games/Infinadeck/Config/settings.ini\n"
                            + "\n"
                            + "\n"
                            + "\n"
                            + "Contact us at <b>support@infinadeck.com</b> for more assistance\n"
                            + "\n"
                            + "Press = to Hide Infinadeck GUI Notifications\n";
                        GUI.DrawTexture(new Rect(Screen.width - 700, Screen.height - 500, 400, 200), textBG, ScaleMode.StretchToFill, false, 0);
                        GUI.Label(new Rect(Screen.width - 685, Screen.height - 485, 400, 200), guiOutput);
                    }
                }
                else// IDA present, Plugin is Disabled
                {
                    guiOutput = "<b>INFINADECK</b>" + "   <color=red>" + iI.errorInfo + "</color>\n"
                        + "Infinadeck Plugin disabled, but IDA is open\n"
                        + "\n"
                        + "Re-enable by pressing Ctrl+I\n"
                        + "or by setting 'pluginEnabled = true' in\n"
                        + "My Documents/My Games/Infinadeck/Config/settings.ini\n"
                        + "\n"
                        + "\n"
                        + "Contact us at <b>support@infinadeck.com</b> for more assistance\n"
                        + "\n"
                        + "Press = to Hide Infinadeck GUI Notifications\n";
                    GUI.DrawTexture(new Rect(Screen.width - 700, Screen.height - 500, 400, 200), textBG, ScaleMode.StretchToFill, false, 0);
                    GUI.Label(new Rect(Screen.width - 685, Screen.height - 485, 400, 200), guiOutput);
                }
            }
            else
            {
                if (localPLUGINactive) // IDA not present, Plugin is Enabled
                {
                    guiOutput = "<b>INFINADECK</b>" + "   <color=red>" + iI.errorInfo + "</color>\n"
                        + "Infinadeck Plugin enabled, but IDA is not open\n"
                        + "\n"
                        + "Download IDA (Infinadeck Desktop Application)\n"
                        + "<b>https://tinyurl.com/InfinadeckPlugin</b>\n"
                        + "\n"
                        + "<color=grey>(OR Disable the plugin by pressing Ctrl+I)</color>\n"
                        + "\n"
                        + "Contact us at <b>support@infinadeck.com</b> for more assistance\n"
                        + "\n"
                        + "Press = to Hide Infinadeck GUI Notifications\n";
                    GUI.DrawTexture(new Rect(Screen.width - 700, Screen.height - 500, 400, 200), textBG, ScaleMode.StretchToFill, false, 0);
                    GUI.Label(new Rect(Screen.width - 685, Screen.height - 485, 400, 200), guiOutput);
                }
                else // IDA not present, Plugin is Disabled
                {
                    if (iI.enabled) { guiOutput = ""; }
                    else //Safe Mode Message
                    {
                        guiOutput = "<b>INFINADECK</b>" + "   <color=red>" + iI.errorInfo + "</color>\n"
                        + "SAFE MODE\n"
                        + "\n"
                        + "Prior project crashed or closed without\n"
                        + "calling OnApplicationQuit on InfinadeckCore\n"
                        + "\n"
                        + "\n"
                        + "Press Ctrl + I to enable the Infinadeck Plugin\n"
                        + "\n"
                        + "\n"
                        + "Press = to Hide Infinadeck GUI Notifications\n";
                        GUI.DrawTexture(new Rect(Screen.width - 700, Screen.height - 500, 400, 200), textBG, ScaleMode.StretchToFill, false, 0);
                        GUI.Label(new Rect(Screen.width - 685, Screen.height - 485, 400, 200), guiOutput);
                    }
                }
            }
        }
        else
        {
            if (iI.enabled) { guiOutput = ""; }
            else //Safe Mode Message even though messages hidden
            {
                guiOutput = "<b>INFINADECK</b>" + "   <color=red>" + iI.errorInfo + "</color>\n"
                + "SAFE MODE\n"
                + "\n"
                + "Prior project crashed or closed without\n"
                + "calling OnApplicationQuit on InfinadeckCore\n"
                + "\n"
                + "\n"
                + "Press Ctrl + I to enable the Infinadeck Plugin\n"
                + "\n"
                + "\n"
                + "Press = to Hide Infinadeck GUI Notifications\n";
                GUI.DrawTexture(new Rect(Screen.width - 700, Screen.height - 500, 400, 200), textBG, ScaleMode.StretchToFill, false, 0);
                GUI.Label(new Rect(Screen.width - 685, Screen.height - 485, 400, 200), guiOutput);
            }
        }
    }
}