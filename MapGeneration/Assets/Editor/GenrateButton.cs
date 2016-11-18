using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapGenrator))]
public class GenrateButton : Editor {

    public override void OnInspectorGUI()
    {
        MapGenrator mapGen = (MapGenrator)target;
       if( DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
                mapGen.GenerateMap();
        }


        if (GUILayout.Button("Generate"))
            mapGen.GenerateMap();

        if (GUILayout.Button("Add Trees"))
        {
            mapGen.Addtrees = true;
            mapGen.GenerateMap();
        }
        if (GUILayout.Button("Remove Trees"))
        {
            mapGen.Addtrees = false;
            mapGen.GenerateMap();
        }
        if (GUILayout.Button("Add Houses"))
        {
            mapGen.Addhouses = true;
            mapGen.GenerateMap();
        }

        if (GUILayout.Button("Remove houses"))
        {
            mapGen.Addhouses = false;
            mapGen.GenerateMap();
        }
    }
}
