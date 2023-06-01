using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(InfinadeckCore))]
[CanEditMultipleObjects]

/**
 * ------------------------------------------------------------
 * Editor Panel for InfinadeckCore.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

public class InfinadeckMasterEditor : Editor
{

    SerializedProperty pluginVersion;
    SerializedProperty cameraRig;
    SerializedProperty headset;
    SerializedProperty autoStart;
    SerializedProperty originOffsetPosition;
    SerializedProperty originOffsetRotation;
    SerializedProperty originOffsetScale;
    SerializedProperty firstLevel;
    SerializedProperty movementLevel;
    SerializedProperty guaranteeDestroyOnLoad;
    SerializedProperty speedGain;
    SerializedProperty showCollisions;
    SerializedProperty showTreadmillVelocity;
    SerializedProperty guiOutput;

    void OnEnable()
    {
        pluginVersion = serializedObject.FindProperty("pluginVersionForEditorReference");
        cameraRig = serializedObject.FindProperty("cameraRig");
        headset = serializedObject.FindProperty("headset");
        originOffsetPosition = serializedObject.FindProperty("originOffsetPosition");
        originOffsetRotation = serializedObject.FindProperty("originOffsetRotation");
        originOffsetScale = serializedObject.FindProperty("originOffsetScale");
        autoStart = serializedObject.FindProperty("autoStart");
        firstLevel = serializedObject.FindProperty("firstLevel");
        movementLevel = serializedObject.FindProperty("movementLevel");
        guaranteeDestroyOnLoad = serializedObject.FindProperty("guaranteeDestroyOnLoad");
        speedGain = serializedObject.FindProperty("speedGain");
        showCollisions = serializedObject.FindProperty("showCollisions");
        showTreadmillVelocity = serializedObject.FindProperty("showTreadmillVelocity");
        guiOutput = serializedObject.FindProperty("guiOutput");
        if (PlayerSettings.legacyClampBlendShapeWeights)
        {
            Debug.Log("INFINADECK NOTICE: Blendshapes with non 0-100 values are used for our reference objects. Player Settings -> Legacy Clamp Blend Shape Weights has been disabled.");
            PlayerSettings.legacyClampBlendShapeWeights = false;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.LabelField("Plugin Version " + pluginVersion.stringValue);

        if (cameraRig.objectReferenceValue && headset.objectReferenceValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Key References");
        }
        EditorGUILayout.PropertyField(cameraRig);
        EditorGUILayout.PropertyField(headset);
        EditorGUILayout.PropertyField(originOffsetPosition);
        EditorGUILayout.PropertyField(originOffsetRotation);
        EditorGUILayout.PropertyField(originOffsetScale);
        if (cameraRig.objectReferenceValue && headset.objectReferenceValue)
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Optional Settings");
            EditorGUILayout.PropertyField(autoStart);
            EditorGUILayout.PropertyField(firstLevel);
            EditorGUILayout.PropertyField(movementLevel);
            EditorGUILayout.PropertyField(guaranteeDestroyOnLoad);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Advanced Settings");
            EditorGUILayout.PropertyField(speedGain);
            EditorGUILayout.PropertyField(showCollisions);
            EditorGUILayout.PropertyField(showTreadmillVelocity);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Keybinds");
            EditorGUILayout.TextArea(guiOutput.stringValue);
        }

        serializedObject.ApplyModifiedProperties();
    }
}