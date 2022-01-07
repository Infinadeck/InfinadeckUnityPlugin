using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using IniParser;
using IniParser.Model;

/**
 * ------------------------------------------------------------
 * Core Script of InfinaDATA
 * Assignable script that allows for easy management of DataEntry Dictionaries with Ricardo Amores Hernández's "Ini Parser" DLL.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by Griffin Brunner @ Infinadeck, 2019
 * Attribution required.
 * 
 * Ricardo Amores Hernández's "Ini Parser" DLL can be found under Assets/Infinadeck/InfinaDATA/Plugins,
 * and can be acquired directly from his GitHub @ https://github.com/rickyah/ini-parser
 * ------------------------------------------------------------
 */
public class InfinaDATA : MonoBehaviour
{
    FileIniDataParser parser = new FileIniDataParser();
    IniData data;
    public string fileLocation;
    public string fileName;
    private string path;
    public DateTime lastSync;
    [InfDATAReadOnlyInEditor] public string lastSync_ymd;
    [InfDATAReadOnlyInEditor] public string lastSync_hms;
    public float gameUpdateRate = 1f;
    public float systemUpdateRate = 10f;

    public class DataEntry
    {
        public string EntryName { get; set; }
        public string EntryValue { get; set; }
        public bool WriteFlag { get; set; }
    }

    public Dictionary<string, DataEntry> all;

    /**
     * Initializes the system.
     * - assumes the DataEntry Dictionary, fileLocation, and fileName have all been filled in
     * - finds and/or creates all files
     * - runs one Read-Write cycle to get things initialized
     */
    public void InitMe()
    {
        Debug.Log("[InfinaDATA] Initializing " + fileName + " from " + fileLocation);
        path = fileLocation + fileName;
        Directory.CreateDirectory(fileLocation);
        if (!File.Exists(path))
        {
            Debug.Log("File does not exist, generating empty- Directory: " + path);
            File.Create(path).Close(); // Creates the file, and keeps it empty
        }
        LoadSettings(); // Grabs values from file for local
        SaveSettings(); // Saves local values to file
        StartCoroutine(CheckGamePreferences());
        StartCoroutine(CheckSystemPreferences());
    }

    /**
     * Checks to see if any DataEntries have been flagged as changed;
     * if so, saves everything.
     */
    IEnumerator CheckGamePreferences()
    {
        while (true)
        {
            yield return new WaitForSeconds(gameUpdateRate);
            foreach (KeyValuePair<string, DataEntry> pref in all)
            {
                if (pref.Value.WriteFlag == true)
                {
                    Debug.Log("[InfinaDATA] " + pref.Value.EntryName + " changed; saving to " + path);
                    SaveSettings();
                    break;
                }
            }
        }
    }

    IEnumerator CheckSystemPreferences()
    {
        while (true)
        {
            yield return new WaitForSeconds(systemUpdateRate);
            if (lastSync != FindTime()) { LoadSettings(); }
        }
    }

    private DateTime FindTime()
    {
        return File.GetLastWriteTime(path);
    }

    public DateTime GetSyncTime()
    {
        return lastSync;
    }

    /**
     * An external overwrite of one of the DataEntries.
     * Triggers the DataEntry's write flag.
     */
    public void Write(string key, string val)
    {
        all[key].EntryValue = val;
        all[key].WriteFlag = true;
    }

    /**
     * Parses one of the DataEntries as a string.
     */
    public string ReadString(string key)
    {
        return all[key].EntryValue;
    }

    /**
     * Parses one of the DataEntries as a boolean.
     */
    public bool ReadBool(string key)
    {
        if (all[key].EntryValue == "true" || all[key].EntryValue == "True") { return true; }
        else if (all[key].EntryValue == "false" || all[key].EntryValue == "False") { return false; }
        else
        {
            Debug.LogError(key + "PREFERENCE NOT OF TYPE BOOLEAN");
            return false;
        }
    }

    /**
     * Parses one of the DataEntries as a float.
     */
    public float ReadFloat(string key)
    {
        float outfloat = 0f;
        if (float.TryParse(all[key].EntryValue, out outfloat))
        {
            return outfloat;
        }
        else
        {
            Debug.LogError(key + "PREFERENCE NOT OF TYPE FLOAT");
            return 0;
        }
    }

    /**
     * Parses one of the DataEntries as an int.
     */
    public int ReadInt(string key)
    {
        int outint = 0;
        if (int.TryParse(all[key].EntryValue, out outint))
        {
            return outint;
        }
        else
        {
            Debug.LogError(key + "PREFERENCE NOT OF TYPE INT");
            return 0;
        }
    }

    /**
     * Reads all data in from the ini file to a data structure,
     * and updates the DataEntries from that newly processed data structure.
     */
    public void LoadSettings()
    {
        data = parser.ReadFile(path);
        UpdatePreferencesFromData();
        lastSync = FindTime();
        lastSync_ymd = lastSync.Year + " " + lastSync.Month + " " + lastSync.Day;
        lastSync_hms = lastSync.Hour + " " + lastSync.Minute + " " + lastSync.Second;
    }

    /**
     * Updates the data structure with the current DataEntries,
     * then saves that data structure to the ini file.
     */
    public void SaveSettings()
    {
        PushPreferencesToData();
        parser.WriteFile(path, data);
        lastSync = FindTime();
        lastSync_ymd = lastSync.Year + " " + lastSync.Month + " " + lastSync.Day;
        lastSync_hms = lastSync.Hour + " " + lastSync.Minute + " " + lastSync.Second;
    }

    /**
     * Iterates across every DataEntry.
     * Transfers the data structure's value into the appropriate DataEntry.
     * Will not read in information for DataEntries that do not exist.
     */
    public void UpdatePreferencesFromData()
    {
        foreach (KeyValuePair<string, DataEntry> pref in all)
        {
            if (!string.IsNullOrEmpty(data[pref.Value.EntryName][pref.Key])) { pref.Value.EntryValue = data[pref.Value.EntryName][pref.Key]; }
        }
    }

    /**
     * Iterates across every DataEntry.
     * If a DataEntry does not have a home within the data structure, create it.
     * Regardless, transfers the DataEntry's value into the data structure.
     * Resets all write flags.
     */
    public void PushPreferencesToData()
    {
        foreach (KeyValuePair<string, DataEntry> pref in all)
        {
            if (data[pref.Value.EntryName][pref.Key] == null) { data[pref.Value.EntryName].AddKey(pref.Key, "init"); }
            data[pref.Value.EntryName][pref.Key] = pref.Value.EntryValue;
            pref.Value.WriteFlag = false;
        }
    }
}
