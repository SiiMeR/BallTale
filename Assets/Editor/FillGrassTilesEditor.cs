using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FillGrassTiles))]
public class FillGrassTilesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var fillGrassTiles = (FillGrassTiles) target;

        if (GUILayout.Button("Fill world with grass!"))
        {
            fillGrassTiles.FillWithGrass(Vector3Int.left);
            fillGrassTiles.FillWithGrass(Vector3Int.right);
            fillGrassTiles.FillWithGrass(Vector3Int.up);
            fillGrassTiles.FillWithGrass(Vector3Int.down);
        } 
        if (GUILayout.Button("Fill right side of tiles with grass"))
        {
            fillGrassTiles.FillWithGrass(Vector3Int.right);
        }
        if (GUILayout.Button("Fill left side of tiles with grass"))
        {
            fillGrassTiles.FillWithGrass(Vector3Int.left);
        }
        if (GUILayout.Button("Fill upper side of tiles with grass"))
        {
            fillGrassTiles.FillWithGrass(Vector3Int.up);
        }
        if (GUILayout.Button("Fill under side of tiles with grass"))
        {
            fillGrassTiles.FillWithGrass(Vector3Int.down);
        }  
        if (GUILayout.Button("Remove all grass"))
        {
            fillGrassTiles.RemoveAllGrass();
        }
    }
}
