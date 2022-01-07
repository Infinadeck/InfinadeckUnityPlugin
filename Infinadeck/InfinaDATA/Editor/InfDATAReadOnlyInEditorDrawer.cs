using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/**
 * ------------------------------------------------------------
 * Allows for Read Only style attributes in the Unity Editor for InfinaDATA elements.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

[CustomPropertyDrawer(typeof(InfDATAReadOnlyInEditorAttribute))]
public class InfDATAReadOnlyInEditorDrawer : PropertyDrawer
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
