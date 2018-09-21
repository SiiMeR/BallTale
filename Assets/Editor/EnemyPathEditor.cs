using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BasicEnemy))]
public class EnemyPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var basicEnemy = (BasicEnemy) target;

        if (GUILayout.Button("Reset movement waypoints"))
        {
            basicEnemy.ResetWaypoints();
        }
    }

    // http://catlikecoding.com/unity/tutorials/curves-and-splines/
    private void OnSceneGUI()
    {
        var enemy = target as BasicEnemy;


        var enemyTransform = enemy?.transform;

        if (enemyTransform == null) return;
        
        var enemyRotation = Tools.pivotRotation == PivotRotation.Local ? enemyTransform.rotation : Quaternion.identity;

        var p0 = enemy.PathFirstPos;
        var p1 = enemyTransform.position;
        var p2 = enemy.PathLastPos;

        Handles.color = Color.cyan;

        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p1, p2);


        EditorGUI.BeginChangeCheck();
        p0 = Handles.PositionHandle(p0, enemyRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(enemy, "Move Path Point");
            EditorUtility.SetDirty(enemy);
            enemy.PathFirstPos = p0;
        }


        EditorGUI.BeginChangeCheck();
        p2 = Handles.PositionHandle(p2, enemyRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(enemy, "Move Path Point");
            EditorUtility.SetDirty(enemy);
            enemy.PathLastPos = p2;
        }
    }

}