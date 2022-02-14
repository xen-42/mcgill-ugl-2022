using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomNetworkManager))]
[CanEditMultipleObjects]
public class CustomNetworkManagerEditor : Editor
{
    SerializedProperty transportType;
    SerializedProperty steamNetworkUI;
    SerializedProperty playerPrefab;

    void OnEnable()
    {
        transportType = serializedObject.FindProperty("transportType");
        steamNetworkUI = serializedObject.FindProperty("steamNetworkUI");
        playerPrefab = serializedObject.FindProperty("playerPrefab");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(transportType);
        EditorGUILayout.PropertyField(steamNetworkUI);

        EditorGUILayout.PropertyField(playerPrefab);
        serializedObject.ApplyModifiedProperties();
    }

}
