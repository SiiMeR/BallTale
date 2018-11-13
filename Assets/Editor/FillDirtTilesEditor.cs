using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FillDirtTiles))]
public class FillDirtTilesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var fillDirtTiles = (FillDirtTiles) target;

        if (GUILayout.Button("Fill world with dirt!"))
        {
            fillDirtTiles.FillWithDirt(Vector3Int.left);
            fillDirtTiles.FillWithDirt(Vector3Int.right);
            fillDirtTiles.FillWithDirt(Vector3Int.up);
            fillDirtTiles.FillWithDirt(Vector3Int.down);
        } 
        if (GUILayout.Button("Fill right side of tiles with dirt"))
        {
            fillDirtTiles.FillWithDirt(Vector3Int.right);
        }
        if (GUILayout.Button("Fill left side of tiles with dirt"))
        {
            fillDirtTiles.FillWithDirt(Vector3Int.left);
        }
        if (GUILayout.Button("Fill upper side of tiles with dirt"))
        {
            fillDirtTiles.FillWithDirt(Vector3Int.up);
        }
        if (GUILayout.Button("Fill under side of tiles with dirt"))
        {
            fillDirtTiles.FillWithDirt(Vector3Int.down);
        }  
        if (GUILayout.Button("Remove all dirt"))
        {
            fillDirtTiles.RemoveAllDirt();
        }
    }
}
