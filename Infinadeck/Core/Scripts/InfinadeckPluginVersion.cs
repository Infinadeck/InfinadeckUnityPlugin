using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode]

/**
 * ------------------------------------------------------------
 * Visually updates the Infinadeck plugin to display the correct version in-editor.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

public class InfinadeckPluginVersion : MonoBehaviour
{
    public Text version;
    private InfinadeckCore core;

    // Update is called once per frame
    void Update()
    {
        if (!core) { core = FindObjectOfType<InfinadeckCore>(); }
        else { version.text = core.pluginVersion; }
    }
}
