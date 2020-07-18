using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGen2))]
public class MapGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGen2 mapGen = (MapGen2)target;

        if (DrawDefaultInspector()) // Runs everytime editor UI changes
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate")) // Adds button to Editor UI
        {
            mapGen.GenerateMap();
        }
    }
}