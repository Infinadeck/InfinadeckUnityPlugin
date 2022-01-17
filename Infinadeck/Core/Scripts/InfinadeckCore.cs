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
    [InfReadOnlyInEditor] public string pluginVersion = "3.0.1";
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
    private bool initialized = false;

    public InfinaDATA preferences;
    public Dictionary<string, InfinaDATA.DataEntry> defaultPreferences;
    private string defaultKeybinds = "#PAD";
    private KeyCode boundKey01 = KeyCode.None;
    private KeyCode boundKey02 = KeyCode.None;
    private KeyCode boundKey03 = KeyCode.None;
    private KeyCode boundKey04 = KeyCode.None;
    private KeyCode boundKey05 = KeyCode.None;
    private KeyCode boundKey06 = KeyCode.None;
    private KeyCode boundKey07 = KeyCode.None;
    private KeyCode boundKey08 = KeyCode.None;
    private KeyCode boundKey09 = KeyCode.None;
    private KeyCode boundKey10 = KeyCode.None;
    private KeyCode boundKey11 = KeyCode.None;
    private KeyCode boundKey12 = KeyCode.None;

    public GameObject cameraRig;
    public GameObject headset;
    public float speedGain = 1;
    public Vector3 worldScale = Vector3.one;
    private string testMap = "infinadeckTestMap";
    private string returnMap = "";


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
            { "keyboardInputEnabled", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = "true" } },
            { "keybindProfile", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = defaultKeybinds } },
            { "customBinding", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = "Alpha1-Alpha2-Alpha3-Alpha4-Alpha5-Alpha6-Alpha7-Alpha8-Alpha9-Alpha0-Minus-Equals" } },

            { "exportBindings", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = "false" } },
            { "link01", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = "null" } },
            { "link02", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = "null" } },
            { "link03", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = "null" } },
            { "link04", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = "null" } },
            { "link05", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = "null" } },
            { "link06", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = "null" } },
            { "link07", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = "null" } },
            { "link08", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = "null" } },
            { "link09", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = "null" } },
            { "link10", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = "null" } },
            { "link11", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = "null" } },
            { "link12", new InfinaDATA.DataEntry { EntryName = "KeybindPreferences", EntryValue = "null" } }
        };
        preferences.all = defaultPreferences;
        preferences.InitMe();

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

    /**
     * Runs with the Physics update.
     */
    private void FixedUpdate()
    {
        if (preferences.ReadBool("keyboardInputEnabled")) { InputCheck(); }
    }

    /**
     * Keybind Accessible Functions.
     */
    public void ButtonMap(int i)
    {
        switch (i)
        {
            case 1:
                if (infinadeckReferenceObjects) { infinadeckReferenceObjects.ToggleDeckRing(); }
                break;
            case 2:
                if (infinadeckReferenceObjects) { infinadeckReferenceObjects.ToggleDeckEdge(); }
                break;
            case 3:
                if (infinadeckReferenceObjects) { infinadeckReferenceObjects.ToggleDeckCenter(); }
        break;
            case 4:
                if (infinadeckReferenceObjects) { infinadeckReferenceObjects.ToggleReferencePanel(); }
                break;
            case 5:
                if (infinadeckReferenceObjects) { infinadeckReferenceObjects.ToggleInEngineDeck();}
                break;
            case 6:
                if (infinadeckReferenceObjects) { infinadeckReferenceObjects.ToggleHeading();}
                break; 
            case 7:
                if (infinadeckReferenceObjects) { infinadeckReferenceObjects.ToggleColorblind();}
                break;
            case 8:
                if (infinadeckReferenceObjects) { infinadeckReferenceObjects.CyclePanelTheme();}
                break;
            case 9:
                BootOrShutdown();
                break;
            case 10:
                StartStopTreadmill();
                break;
            case 11:
                ImportPreferences();
                break;
            case 12:
                ResetPreferences();
                break;
            default:
                Debug.LogError("INFINADECK ERROR: Infinadeck Keybinding broken, resetting to default link");
                preferences.Write("keybindProfile", defaultKeybinds);
                break;
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
     * Loads the user into a default testing room optimized for the Infinadeck plugin.
     */
    private void LoadTestRoom()
    {
        if (SceneManager.GetActiveScene().name == testMap)
        {
            if (returnMap == "")
            {
                Debug.LogError("INFINADECK ERROR: Attempting to return home, but no Return Map is specified on [Infinadeck].");
            }
            else { AsyncSceneLoader(returnMap); }
        }
        else
        {
            bool success = false;
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                if (SceneManager.GetSceneByBuildIndex(i).name == testMap) { success = true; }
            }
            if (success) { AsyncSceneLoader(testMap); }
            else { Debug.LogError("INFINADECK ERROR: Access to InfinadeckTestMap is limited; please add the map to the Build Settings."); }
        }
    }

    /**
     * Loads the scene in the background.
     */
    IEnumerator AsyncSceneLoader(string map)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(map);
        while (!asyncLoad.isDone)
        {
            yield return null;
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

    /**
     * Keybind Checking Loop.
     */
    public void InputCheck()
    {
        if (Input.anyKeyDown)
        {
            string button01 = "Toggle Deck Ring";
            string button02 = "Toggle Deck Edge";
            string button03 = "Toggle Deck Center";
            string button04 = "Toggle Reference Panel";
            string button05 = "Toggle In-Engine Deck";
            string button06 = "Toggle Heading";
            string button07 = "Toggle Colorblind";
            string button08 = "Cycle Panel Theme";
            string button09 = "Boot/Shutdown/Reboot Plugin";
            string button10 = "Start/Stop Treadmill";
            string button11 = "Import Preferences";
            string button12 = "Reset Preferences";

            //set up custom bindings
            if (preferences.ReadString("keybindProfile") == "Custom")
            {
                string customBindingSegment = preferences.ReadString("customBinding");
                //string[] keyArray = customBindingSegment.Split(customBindingSegment.ToCharArray(), (Char)"-"[0]);
                string[] keyArray = customBindingSegment.Split((Char)"-"[0]);
                if (keyArray.Length == 12)
                {
                    boundKey01 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyArray[0]);
                    boundKey02 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyArray[1]);
                    boundKey03 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyArray[2]);
                    boundKey04 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyArray[3]);
                    boundKey05 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyArray[4]);
                    boundKey06 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyArray[5]);
                    boundKey07 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyArray[6]);
                    boundKey08 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyArray[7]);
                    boundKey09 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyArray[8]);
                    boundKey10 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyArray[9]);
                    boundKey11 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyArray[10]);
                    boundKey12 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyArray[11]);
                }
                else
                {
                    Debug.Log(keyArray[0]);
                    Debug.Log(keyArray[1]);
                    Debug.Log(keyArray[2]);
                    Debug.Log(keyArray[3]);
                    Debug.Log(keyArray[4]);
                    Debug.Log(preferences.ReadString("customBinding"));
                    Debug.LogError("INFINADECK ERROR: Infinadeck Keybinding broken, resetting to default link");
                    preferences.Write("keybindProfile", defaultKeybinds);
                }
            }

            //visualize bindings onscreen or update binding callouts 
            if (preferences.ReadBool("exportBindings"))
            {
                switch (preferences.ReadString("keybindProfile"))
                {
                    case "FUNC":
                        preferences.Write("link01", "F1 to " + button01);
                        preferences.Write("link02", "F2 to " + button02);
                        preferences.Write("link03", "F3 to " + button03);
                        preferences.Write("link04", "F4 to " + button04);
                        preferences.Write("link05", "F5 to " + button05);
                        preferences.Write("link06", "F6 to " + button06);
                        preferences.Write("link07", "F7 to " + button07);
                        preferences.Write("link08", "F8 to " + button08);
                        preferences.Write("link09", "F9 to " + button09);
                        preferences.Write("link10", "F10 to " + button10);
                        preferences.Write("link11", "F11 to " + button11);
                        preferences.Write("link12", "F12 to " + button12);
                        break;
                    case "1234":
                        preferences.Write("link01", "Alpha1 to " + button01);
                        preferences.Write("link02", "Alpha2 to " + button02);
                        preferences.Write("link03", "Alpha3 to " + button03);
                        preferences.Write("link04", "Alpha4 to " + button04);
                        preferences.Write("link05", "Alpha5 to " + button05);
                        preferences.Write("link06", "Alpha6 to " + button06);
                        preferences.Write("link07", "Alpha7 to " + button07);
                        preferences.Write("link08", "Alpha8 to " + button08);
                        preferences.Write("link09", "Alpha9 to " + button09);
                        preferences.Write("link10", "Alpha0 to " + button10);
                        preferences.Write("link11", "Minus to " + button11);
                        preferences.Write("link12", "Equals to " + button12);
                        break;
                    case "#PAD":
                        preferences.Write("link01", "Keypad1 to " + button01);
                        preferences.Write("link02", "Keypad2 to " + button02);
                        preferences.Write("link03", "Keypad3 to " + button03);
                        preferences.Write("link04", "Keypad4 to " + button04);
                        preferences.Write("link05", "Keypad5 to " + button05);
                        preferences.Write("link06", "Keypad6 to " + button06);
                        preferences.Write("link07", "Keypad7 to " + button07);
                        preferences.Write("link08", "Keypad8 to " + button08);
                        preferences.Write("link09", "Keypad9 to " + button09);
                        preferences.Write("link10", "Keypad0 to " + button10);
                        preferences.Write("link11", "KeypadPeriod to " + button11);
                        preferences.Write("link12", "KeypadEnter to " + button12);
                        break;
                    case "STND":
                        preferences.Write("link01", "LeftShift to " + button01);
                        preferences.Write("link02", "LeftControl to " + button02);
                        preferences.Write("link03", "LeftAlt to " + button03);
                        preferences.Write("link04", "Space to " + button04);
                        preferences.Write("link05", "RightShift to " + button05);
                        preferences.Write("link06", "RightControl to " + button06);
                        preferences.Write("link07", "RightAlt to " + button07);
                        preferences.Write("link08", "Menu to " + button08);
                        preferences.Write("link09", "BackQuote to " + button09);
                        preferences.Write("link10", "Tab to " + button10);
                        preferences.Write("link11", "Backslash to " + button11);
                        preferences.Write("link12", "Backspace to " + button12);
                        break;
                    case "CPAD":
                        preferences.Write("link01", "LeftArrow to " + button01);
                        preferences.Write("link02", "DownArrow to " + button02);
                        preferences.Write("link03", "RightArrow to " + button03);
                        preferences.Write("link04", "UpArrow to " + button04);
                        preferences.Write("link05", "Delete to " + button05);
                        preferences.Write("link06", "End to " + button06);
                        preferences.Write("link07", "PageDown to " + button07);
                        preferences.Write("link08", "Insert to " + button08);
                        preferences.Write("link09", "Home to " + button09);
                        preferences.Write("link10", "PageUp to " + button10);
                        preferences.Write("link11", "Print to " + button11);
                        preferences.Write("link12", "Pause to " + button12);
                        break;
                    case "QWER":
                        preferences.Write("link01", "Q to " + button01);
                        preferences.Write("link02", "W to " + button02);
                        preferences.Write("link03", "E to " + button03);
                        preferences.Write("link04", "R to " + button04);
                        preferences.Write("link05", "T to " + button05);
                        preferences.Write("link06", "Y to " + button06);
                        preferences.Write("link07", "U to " + button07);
                        preferences.Write("link08", "I to " + button08);
                        preferences.Write("link09", "O to " + button09);
                        preferences.Write("link10", "P to " + button10);
                        preferences.Write("link11", "LeftBracket to " + button11);
                        preferences.Write("link12", "RightBracket to " + button12);
                        break;
                    case "ASDF":
                        preferences.Write("link01", "A to " + button01);
                        preferences.Write("link02", "S to " + button02);
                        preferences.Write("link03", "D to " + button03);
                        preferences.Write("link04", "F to " + button04);
                        preferences.Write("link05", "G to " + button05);
                        preferences.Write("link06", "H to " + button06);
                        preferences.Write("link07", "J to " + button07);
                        preferences.Write("link08", "K to " + button08);
                        preferences.Write("link09", "L to " + button09);
                        preferences.Write("link10", "Semicolon to " + button10);
                        preferences.Write("link11", "Quote to " + button11);
                        preferences.Write("link12", "Slash to " + button12);
                        break;
                    case "Custom":
                        string customBindingSegment = preferences.ReadString("customBinding");
                        string[] keyArray = customBindingSegment.Split(customBindingSegment.ToCharArray(), (Char)"-"[0]);
                        preferences.Write("link01", keyArray[0] + " to " + button01);
                        preferences.Write("link02", keyArray[1] + " to " + button02);
                        preferences.Write("link03", keyArray[2] + " to " + button03);
                        preferences.Write("link04", keyArray[3] + " to " + button04);
                        preferences.Write("link05", keyArray[4] + " to " + button05);
                        preferences.Write("link06", keyArray[5] + " to " + button06);
                        preferences.Write("link07", keyArray[6] + " to " + button07);
                        preferences.Write("link08", keyArray[7] + " to " + button08);
                        preferences.Write("link09", keyArray[8] + " to " + button09);
                        preferences.Write("link10", keyArray[9] + " to " + button10);
                        preferences.Write("link11", keyArray[10] + " to " + button11);
                        preferences.Write("link12", keyArray[11] + " to " + button12);
                        break;
                    default:
                        Debug.LogError("INFINADECK ERROR: Infinadeck Keybinding broken, resetting to default link");
                        preferences.Write("keybindProfile", defaultKeybinds);
                        break;
                }
            }

            //call the correct function for binding
            switch (preferences.ReadString("keybindProfile"))
            {
                case "FUNC":
                    if (Input.GetKeyDown(KeyCode.F1)) { ButtonMap(1); }
                    else if (Input.GetKeyDown(KeyCode.F2)) { ButtonMap(2); }
                    else if (Input.GetKeyDown(KeyCode.F3)) { ButtonMap(3); }
                    else if (Input.GetKeyDown(KeyCode.F4)) { ButtonMap(4); }
                    else if (Input.GetKeyDown(KeyCode.F5)) { ButtonMap(5); }
                    else if (Input.GetKeyDown(KeyCode.F6)) { ButtonMap(6); }
                    else if (Input.GetKeyDown(KeyCode.F7)) { ButtonMap(7); }
                    else if (Input.GetKeyDown(KeyCode.F8)) { ButtonMap(8); }
                    else if (Input.GetKeyDown(KeyCode.F9)) { ButtonMap(9); }
                    else if (Input.GetKeyDown(KeyCode.F10)) { ButtonMap(10); }
                    else if (Input.GetKeyDown(KeyCode.F11)) { ButtonMap(11); }
                    else if (Input.GetKeyDown(KeyCode.F12)) { ButtonMap(12); }
                    break;
                case "1234":
                    if (Input.GetKeyDown(KeyCode.Alpha1)) { ButtonMap(1); }
                    else if (Input.GetKeyDown(KeyCode.Alpha2)) { ButtonMap(2); }
                    else if (Input.GetKeyDown(KeyCode.Alpha3)) { ButtonMap(3); }
                    else if (Input.GetKeyDown(KeyCode.Alpha4)) { ButtonMap(4); }
                    else if (Input.GetKeyDown(KeyCode.Alpha5)) { ButtonMap(5); }
                    else if (Input.GetKeyDown(KeyCode.Alpha6)) { ButtonMap(6); }
                    else if (Input.GetKeyDown(KeyCode.Alpha7)) { ButtonMap(7); }
                    else if (Input.GetKeyDown(KeyCode.Alpha8)) { ButtonMap(8); }
                    else if (Input.GetKeyDown(KeyCode.Alpha9)) { ButtonMap(9); }
                    else if (Input.GetKeyDown(KeyCode.Alpha0)) { ButtonMap(10); }
                    else if (Input.GetKeyDown(KeyCode.Minus)) { ButtonMap(11); }
                    else if (Input.GetKeyDown(KeyCode.Equals)) { ButtonMap(12); }
                    break;
                case "#PAD":
                    if (Input.GetKeyDown(KeyCode.Keypad1)) { ButtonMap(1); }
                    else if (Input.GetKeyDown(KeyCode.Keypad2)) { ButtonMap(2); }
                    else if (Input.GetKeyDown(KeyCode.Keypad3)) { ButtonMap(3); }
                    else if (Input.GetKeyDown(KeyCode.Keypad4)) { ButtonMap(4); }
                    else if (Input.GetKeyDown(KeyCode.Keypad5)) { ButtonMap(5); }
                    else if (Input.GetKeyDown(KeyCode.Keypad6)) { ButtonMap(6); }
                    else if (Input.GetKeyDown(KeyCode.Keypad7)) { ButtonMap(7); }
                    else if (Input.GetKeyDown(KeyCode.Keypad8)) { ButtonMap(8); }
                    else if (Input.GetKeyDown(KeyCode.Keypad9)) { ButtonMap(9); }
                    else if (Input.GetKeyDown(KeyCode.Keypad0)) { ButtonMap(10); }
                    else if (Input.GetKeyDown(KeyCode.KeypadPeriod)) { ButtonMap(11); }
                    else if (Input.GetKeyDown(KeyCode.KeypadEnter)) { ButtonMap(12); }
                    break;
                case "STND":
                    if (Input.GetKeyDown(KeyCode.LeftShift)) { ButtonMap(1); }
                    else if (Input.GetKeyDown(KeyCode.LeftControl)) { ButtonMap(2); }
                    else if (Input.GetKeyDown(KeyCode.LeftAlt)) { ButtonMap(3); }
                    else if (Input.GetKeyDown(KeyCode.Space)) { ButtonMap(4); }
                    else if (Input.GetKeyDown(KeyCode.RightShift)) { ButtonMap(5); }
                    else if (Input.GetKeyDown(KeyCode.RightControl)) { ButtonMap(6); }
                    else if (Input.GetKeyDown(KeyCode.RightAlt)) { ButtonMap(7); }
                    else if (Input.GetKeyDown(KeyCode.Menu)) { ButtonMap(8); }
                    else if (Input.GetKeyDown(KeyCode.BackQuote)) { ButtonMap(9); }
                    else if (Input.GetKeyDown(KeyCode.Tab)) { ButtonMap(10); }
                    else if (Input.GetKeyDown(KeyCode.Backslash)) { ButtonMap(11); }
                    else if (Input.GetKeyDown(KeyCode.Backspace)) { ButtonMap(12); }
                    break;
                case "CPAD":
                    if (Input.GetKeyDown(KeyCode.LeftArrow)) { ButtonMap(1); }
                    else if (Input.GetKeyDown(KeyCode.DownArrow)) { ButtonMap(2); }
                    else if (Input.GetKeyDown(KeyCode.RightArrow)) { ButtonMap(3); }
                    else if (Input.GetKeyDown(KeyCode.UpArrow)) { ButtonMap(4); }
                    else if (Input.GetKeyDown(KeyCode.Delete)) { ButtonMap(5); }
                    else if (Input.GetKeyDown(KeyCode.End)) { ButtonMap(6); }
                    else if (Input.GetKeyDown(KeyCode.PageDown)) { ButtonMap(7); }
                    else if (Input.GetKeyDown(KeyCode.Insert)) { ButtonMap(8); }
                    else if (Input.GetKeyDown(KeyCode.Home)) { ButtonMap(9); }
                    else if (Input.GetKeyDown(KeyCode.PageUp)) { ButtonMap(10); }
                    else if (Input.GetKeyDown(KeyCode.Print)) { ButtonMap(11); }
                    else if (Input.GetKeyDown(KeyCode.Pause)) { ButtonMap(12); }
                    break;
                case "QWER":
                    if (Input.GetKeyDown(KeyCode.Q)) { ButtonMap(1); }
                    else if (Input.GetKeyDown(KeyCode.W)) { ButtonMap(2); }
                    else if (Input.GetKeyDown(KeyCode.E)) { ButtonMap(3); }
                    else if (Input.GetKeyDown(KeyCode.R)) { ButtonMap(4); }
                    else if (Input.GetKeyDown(KeyCode.T)) { ButtonMap(5); }
                    else if (Input.GetKeyDown(KeyCode.Y)) { ButtonMap(6); }
                    else if (Input.GetKeyDown(KeyCode.U)) { ButtonMap(7); }
                    else if (Input.GetKeyDown(KeyCode.I)) { ButtonMap(8); }
                    else if (Input.GetKeyDown(KeyCode.O)) { ButtonMap(9); }
                    else if (Input.GetKeyDown(KeyCode.P)) { ButtonMap(10); }
                    else if (Input.GetKeyDown(KeyCode.LeftBracket)) { ButtonMap(11); }
                    else if (Input.GetKeyDown(KeyCode.RightBracket)) { ButtonMap(12); }
                    break;
                case "ASDF":
                    if (Input.GetKeyDown(KeyCode.A)) { ButtonMap(1); }
                    else if (Input.GetKeyDown(KeyCode.S)) { ButtonMap(2); }
                    else if (Input.GetKeyDown(KeyCode.D)) { ButtonMap(3); }
                    else if (Input.GetKeyDown(KeyCode.F)) { ButtonMap(4); }
                    else if (Input.GetKeyDown(KeyCode.G)) { ButtonMap(5); }
                    else if (Input.GetKeyDown(KeyCode.H)) { ButtonMap(6); }
                    else if (Input.GetKeyDown(KeyCode.J)) { ButtonMap(7); }
                    else if (Input.GetKeyDown(KeyCode.K)) { ButtonMap(8); }
                    else if (Input.GetKeyDown(KeyCode.L)) { ButtonMap(9); }
                    else if (Input.GetKeyDown(KeyCode.Semicolon)) { ButtonMap(10); }
                    else if (Input.GetKeyDown(KeyCode.Quote)) { ButtonMap(11); }
                    else if (Input.GetKeyDown(KeyCode.Slash)) { ButtonMap(12); }
                    break;
                case "Custom":
                    if (Input.GetKeyDown(boundKey01)) { ButtonMap(1); }
                    else if (Input.GetKeyDown(boundKey02)) { ButtonMap(2); }
                    else if (Input.GetKeyDown(boundKey03)) { ButtonMap(3); }
                    else if (Input.GetKeyDown(boundKey04)) { ButtonMap(4); }
                    else if (Input.GetKeyDown(boundKey05)) { ButtonMap(5); }
                    else if (Input.GetKeyDown(boundKey06)) { ButtonMap(6); }
                    else if (Input.GetKeyDown(boundKey07)) { ButtonMap(7); }
                    else if (Input.GetKeyDown(boundKey08)) { ButtonMap(8); }
                    else if (Input.GetKeyDown(boundKey09)) { ButtonMap(9); }
                    else if (Input.GetKeyDown(boundKey10)) { ButtonMap(10); }
                    else if (Input.GetKeyDown(boundKey11)) { ButtonMap(11); }
                    else if (Input.GetKeyDown(boundKey12)) { ButtonMap(12); }
                    break;
                default:
                    Debug.LogError("INFINADECK ERROR: Infinadeck Keybinding broken, resetting to default link");
                    preferences.Write("keybindProfile", defaultKeybinds);
                    break;
            }
        }
    }
}