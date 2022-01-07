using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(InfReadOnlyInEditorAttribute))]

/**
 * ------------------------------------------------------------
 * Drawer for the Read-only parameter for Infinadeck plugin components in the Unity Editor.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

public class InfReadOnlyInEditorDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
