using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Infinadeck;

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
    [InfReadOnlyInEditor] public string pluginVersion = "3.2.0";
    [InfReadOnlyInEditor] public GameObject refObjects;
    [InfReadOnlyInEditor] public GameObject locomotion;
    [InfReadOnlyInEditor] public GameObject splashScreen;
    private InfinadeckReferenceObjects infinadeckReferenceObjects;
    private InfinadeckLocomotion infinadeckLocomotion;
    private InfinadeckSplashscreen infinadeckSplashScreen;

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

    public InfinaDATA preferences;
    public Dictionary<string, InfinaDATA.DataEntry> defaultPreferences;

    public InfinaKEYBIND keybinds;
    private string defaultKeybinds = "QWER";
    public KeyCode[] myKeys = new KeyCode[12];
    public string[] keybindNames = new string[12];

    public GameObject cameraRig;
    public GameObject headset;
    public float speedGain = 1;
    public Vector3 worldScale = Vector3.one;

    /**
     * Runs upon the moment of creation of this object.
     */
    void Awake() {
        // Delete Placement Geometry
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        // Initialize Preferences
        preferences = this.gameObject.AddComponent<InfinaDATA>();
        preferences.fileLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Infinadeck/Core/";
        preferences.fileName = "infPlugin_preferences.ini";
        defaultPreferences = new Dictionary<string, InfinaDATA.DataEntry>
        {
            // Keybind Preferences:
            { "keyboardInputEnabled", new InfinaDATA.DataEntry { EntryName = "Keybind Preferences", EntryValue = "true" } },
            { "exportBindings", new InfinaDATA.DataEntry { EntryName = "Keybind Preferences", EntryValue = "false" } },

            { "keybindProfile", new InfinaDATA.DataEntry { EntryName = "Keybinds", EntryValue = defaultKeybinds } },
            { "customBinding", new InfinaDATA.DataEntry { EntryName = "Keybinds", EntryValue = "Alpha1-Alpha2-Alpha3-Alpha4-Alpha5-Alpha6-Alpha7-Alpha8-Alpha9-Alpha0-Minus-Equals" } },
            { "link01", new InfinaDATA.DataEntry { EntryName = "Keybinds", EntryValue = "null" } },
            { "link02", new InfinaDATA.DataEntry { EntryName = "Keybinds", EntryValue = "null" } },
            { "link03", new InfinaDATA.DataEntry { EntryName = "Keybinds", EntryValue = "null" } },
            { "link04", new InfinaDATA.DataEntry { EntryName = "Keybinds", EntryValue = "null" } },
            { "link05", new InfinaDATA.DataEntry { EntryName = "Keybinds", EntryValue = "null" } },
            { "link06", new InfinaDATA.DataEntry { EntryName = "Keybinds", EntryValue = "null" } },
            { "link07", new InfinaDATA.DataEntry { EntryName = "Keybinds", EntryValue = "null" } },
            { "link08", new InfinaDATA.DataEntry { EntryName = "Keybinds", EntryValue = "null" } },
            { "link09", new InfinaDATA.DataEntry { EntryName = "Keybinds", EntryValue = "null" } },
            { "link10", new InfinaDATA.DataEntry { EntryName = "Keybinds", EntryValue = "null" } },
            { "link11", new InfinaDATA.DataEntry { EntryName = "Keybinds", EntryValue = "null" } },
            { "link12", new InfinaDATA.DataEntry { EntryName = "Keybinds", EntryValue = "null" } },

            { "FUNC", new InfinaDATA.DataEntry { EntryName = "Reference: Keybind Profiles", EntryValue = "F1-F2-F3-F4-F5-F6-F7-F8-F9-F10-F11-F12" } },
            { "1234", new InfinaDATA.DataEntry { EntryName = "Reference: Keybind Profiles", EntryValue = "Alpha1-Alpha2-Alpha3-Alpha4-Alpha5-Alpha6-Alpha7-Alpha8-Alpha9-Alpha0-Minus-Equals" } },
            { "#PAD", new InfinaDATA.DataEntry { EntryName = "Reference: Keybind Profiles", EntryValue = "Keypad1-Keypad2-Keypad3-Keypad4-Keypad5-Keypad6-Keypad7-Keypad8-Keypad9-KeypadDivide-KeypadMultiply-KeypadMinus" } },
            { "STND", new InfinaDATA.DataEntry { EntryName = "Reference: Keybind Profiles", EntryValue = "LeftShift-LeftControl-LeftAlt-Space-RightShift-RightControl-RightAlt-Return-BackQuote-Tab-Backslash-Backspace" } },
            { "CPAD", new InfinaDATA.DataEntry { EntryName = "Reference: Keybind Profiles", EntryValue = "LeftArrow-DownArrow-RightArrow-UpArrow-Delete-End-PageDown-Insert-Home-PageUp-ScrollLock-Pause" } },
            { "QWER", new InfinaDATA.DataEntry { EntryName = "Reference: Keybind Profiles", EntryValue = "Q-W-E-R-T-Y-U-I-O-P-LeftBracket-RightBracket" } },
            { "ASDF", new InfinaDATA.DataEntry { EntryName = "Reference: Keybind Profiles", EntryValue = "A-S-D-F-G-H-J-K-L-Semicolon-Quote-Slash" } },
            { "Custom", new InfinaDATA.DataEntry { EntryName = "Reference: Keybind Profiles", EntryValue = "parses the 12 keys listed in customBinding" } }
        };
        preferences.all = defaultPreferences;
        preferences.InitMe();

        if (keybinds == null)
        {
            keybinds = FindObjectOfType<InfinaKEYBIND>();
            if (keybinds == null)
            {
                keybinds = this.gameObject.AddComponent<InfinaKEYBIND>();
            }
        }

        if (autoStart) { Boot(); }
        else { Debug.Log("INFINADECK NOTICE: 'Auto Start' is disabled. Please start Infinadeck Plugin by calling the Boot() function on the instance of [Infinadeck] in your Scene."); }
    }

    /**
     * Core Boot function. Only manually called to initialize the setup.
     */
    public void Boot()
    {
        if (!booted)
        {
            booted = true;
            StartCoroutine(InitializeWithErrorChecks());
            StartCoroutine(SpawnSubcomponents());
        }
        else { Debug.LogWarning("INFINADECK WARNING: Infinadeck Plugin is already booted; if manual boot is desired, un-check 'Auto Start' on the instance of [Infinadeck] in your Scene."); }
    }

    /**
     * Core Shutdown function. Only manually called to stop existing threads.
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
        else { Debug.LogWarning("INFINADECK WARNING: Infinadeck Plugin is not currently booted; shutdown request ignored."); }
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
            infinadeckSplashScreen.pluginVersion.text = pluginVersion;
        }

        // Spawn Reference Objects
        refObjects = Instantiate(Resources.Load("RuntimePrefabs/InfinadeckReferenceObjects") as GameObject, transform.position, Quaternion.identity);
        refObjects.transform.parent = this.transform;
        infinadeckReferenceObjects = refObjects.GetComponent<InfinadeckReferenceObjects>();
        infinadeckReferenceObjects.worldScale = worldScale;

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
        }
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

    private void FixedUpdate()
    {
        if (preferences.ReadBool("keyboardInputEnabled"))
        {
            //determine which profile is in use
            myKeys = keybinds.GetMyKeys(preferences.ReadString("keybindProfile"), preferences.ReadString("customBinding"));

            keybindNames = new string[] 
            {
                "Toggle Deck Ring",
                "Toggle Deck Edge",
                "Toggle Deck Center",
                "Toggle Reference Panel",
                "Toggle In-Engine Deck",
                "Toggle Heading",
                "Toggle Colorblind",
                "Cycle Panel Theme",
                "Boot/Shutdown/Reboot Plugin",
                "Start/Stop Treadmill",
                "Import Preferences",
                "Reset Preferences"
            };

            //export bindings once whenever exportBindings set to true
            if (preferences.ReadBool("exportBindings"))
            {
                for (int j = 0; j < 12; j++)
                {
                    if (keybindNames[j] != "") { preferences.Write("link" + String.Format("{0:00}", j + 1), myKeys[j] + " to " + keybindNames[j]); }
                    else { preferences.Write("link" + String.Format("{0:00}", j + 1), "---"); }
                }
                preferences.Write("exportBindings", "false");
            }

            // determine which function, if any, are being called
            if (!keybinds.isInputEnabled) { keybinds.isInputEnabled = true; }
            else {
                switch (keybinds.KeybindRequest(myKeys))
                {
                    case 0: break; // no button was pressed
                    case 1: if (infinadeckReferenceObjects) infinadeckReferenceObjects.ToggleDeckRing(); break;
                    case 2: if (infinadeckReferenceObjects) infinadeckReferenceObjects.ToggleDeckEdge(); break;
                    case 3: if (infinadeckReferenceObjects) infinadeckReferenceObjects.ToggleDeckCenter(); break;
                    case 4: if (infinadeckReferenceObjects) infinadeckReferenceObjects.ToggleReferencePanel(); break;
                    case 5: if (infinadeckReferenceObjects) infinadeckReferenceObjects.ToggleInEngineDeck(); break;
                    case 6: if (infinadeckReferenceObjects) infinadeckReferenceObjects.ToggleHeading(); break;
                    case 7: if (infinadeckReferenceObjects) infinadeckReferenceObjects.ToggleColorblind(); break;
                    case 8: if (infinadeckReferenceObjects) infinadeckReferenceObjects.CyclePanelTheme(); break;
                    case 9: BootOrShutdown(); break;
                    case 10: StartStopTreadmill(); break;
                    case 11: ImportPreferences(); break;
                    case 12: ResetPreferences(); break;
                    default: break; // no button was pressed
                }
            }
        }
    }

    /**
     * Boots if not running; Shuts down if running.
     */
    public void BootOrShutdown()
    {
        if (booted) { Shutdown(); }
        else { Boot(); }
    }

    /**
     * Starts the treadmill if it is not running; Stops the treadmill if it is running.
     */
    public void StartStopTreadmill()
    {
        if (!Infinadeck.Infinadeck.GetTreadmillRunState())
        {
            Infinadeck.Infinadeck.StartTreadmillUserControl();
        }
        else
        {
            Infinadeck.Infinadeck.StopTreadmill();
        }
    }

    /**
     * Imports the preferences from the settings file.
     */
    public void ImportPreferences()
    {
        preferences.LoadSettings();
        infinadeckReferenceObjects.preferences.LoadSettings();
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
        infinadeckReferenceObjects.preferences.all = infinadeckReferenceObjects.defaultPreferences;
        foreach (KeyValuePair<string, InfinaDATA.DataEntry> pref in infinadeckReferenceObjects.preferences.all)
        {
            pref.Value.WriteFlag = true;
        }
    }
}