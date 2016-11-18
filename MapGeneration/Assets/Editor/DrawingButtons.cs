using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(DrawingConfig))]
public class DrawingButtons : Editor {

    public override void OnInspectorGUI()
    {
        DrawingConfig drawCon = (DrawingConfig)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Reset"))
            drawCon.Reset();


        if (GUILayout.Button("NomalizeRGB"))
            drawCon.NormalizeRGB();

        if (GUILayout.Button("Isolate Water"))
            drawCon.IsolateWater();

        if (GUILayout.Button("Isolate Mountains"))
            drawCon.IsolateMountains();

        if (GUILayout.Button("Isolate Trees"))
            drawCon.findTrees();

        if (GUILayout.Button("Isolate Houses"))
            drawCon.isolateHouses();
    }



}
