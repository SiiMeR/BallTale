using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(BasicEnemy))]
public class EnemyPathEditor : Editor {

	// Use this for initialization
	void Start () {
		
	}

	// http://catlikecoding.com/unity/tutorials/curves-and-splines/
	private void OnSceneGUI()
	{
		BasicEnemy enemy = target as BasicEnemy;
		
		
		Transform enemyTransform = enemy.transform;

		Quaternion enemyRotation = Tools.pivotRotation == PivotRotation.Local ?
			enemyTransform.rotation : Quaternion.identity;
		
		Vector3 p0 = enemy.PathFirstPos;
		Vector3 p1 = enemyTransform.position;
		Vector3 p2 = enemy.PathLastPos;

		Handles.color = Color.cyan;
		
		Handles.DrawLine(p0,p1);
		Handles.DrawLine(p1, p2);


		EditorGUI.BeginChangeCheck();
		p0 = Handles.PositionHandle(p0, enemyRotation);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(enemy, "Move Path Point");
			EditorUtility.SetDirty(enemy);
			enemy.PathFirstPos = p0;
		}
		
	/*	EditorGUI.BeginChangeCheck();
		p1 = Handles.PositionHandle(p1, enemyRotation);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(enemy, "Move Path Point");
			EditorUtility.SetDirty(enemy);
			enemy.PathMiddlePos = p1;
		}*/
		
		
		EditorGUI.BeginChangeCheck();
		p2 = Handles.PositionHandle(p2, enemyRotation);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(enemy, "Move Path Point");
			EditorUtility.SetDirty(enemy);
			enemy.PathLastPos = p2;
		}
		
	}

	// Update is called once per frame
	void Update () {

	}
}
