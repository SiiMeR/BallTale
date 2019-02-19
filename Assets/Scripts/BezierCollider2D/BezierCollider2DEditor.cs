#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

// from https://stackoverflow.com/a/25992574
[CustomEditor(typeof(BezierCollider2D))]
public class BezierCollider2DEditor : Editor
{
    private BezierCollider2D _bezierCollider;
    private EdgeCollider2D _edgeCollider;
    private Vector2 _lastFirstPoint = Vector2.zero;
    private Vector2 _lastHandlerFirstPoint = Vector2.zero;
    private Vector2 _lastHandlerSecondPoint = Vector2.zero;

    private int _lastPointsQuantity;
    private Vector2 _lastSecondPoint = Vector2.zero;

    public override void OnInspectorGUI()
    {
        _bezierCollider = (BezierCollider2D) target;

        _edgeCollider = _bezierCollider.GetComponent<EdgeCollider2D>();

        if (_edgeCollider.hideFlags != HideFlags.HideInInspector) _edgeCollider.hideFlags = HideFlags.HideInInspector;

        if (_edgeCollider != null)
        {
            _bezierCollider.pointsQuantity = EditorGUILayout.IntField("curve points", _bezierCollider.pointsQuantity,
                GUILayout.MinWidth(100));
            _bezierCollider.firstPoint =
                EditorGUILayout.Vector2Field("first point", _bezierCollider.firstPoint, GUILayout.MinWidth(100));
            _bezierCollider.handlerFirstPoint = EditorGUILayout.Vector2Field("handler first Point",
                _bezierCollider.handlerFirstPoint, GUILayout.MinWidth(100));
            _bezierCollider.secondPoint =
                EditorGUILayout.Vector2Field("second point", _bezierCollider.secondPoint, GUILayout.MinWidth(100));
            _bezierCollider.handlerSecondPoint = EditorGUILayout.Vector2Field("handler secondPoint",
                _bezierCollider.handlerSecondPoint, GUILayout.MinWidth(100));

            EditorUtility.SetDirty(_bezierCollider);

            if (_bezierCollider.pointsQuantity > 0 && !_bezierCollider.firstPoint.Equals(_bezierCollider.secondPoint) &&
                (
                    _lastPointsQuantity != _bezierCollider.pointsQuantity ||
                    _lastFirstPoint != _bezierCollider.firstPoint ||
                    _lastHandlerFirstPoint != _bezierCollider.handlerFirstPoint ||
                    _lastSecondPoint != _bezierCollider.secondPoint ||
                    _lastHandlerSecondPoint != _bezierCollider.handlerSecondPoint
                ))
            {
                _lastPointsQuantity = _bezierCollider.pointsQuantity;
                _lastFirstPoint = _bezierCollider.firstPoint;
                _lastHandlerFirstPoint = _bezierCollider.handlerFirstPoint;
                _lastSecondPoint = _bezierCollider.secondPoint;
                _lastHandlerSecondPoint = _bezierCollider.handlerSecondPoint;
                _edgeCollider.points = _bezierCollider.Calculate2DPoints();
            }
        }
    }

    private void OnSceneGUI()
    {
        if (_bezierCollider != null)
        {
            Handles.color = Color.grey;

            Handles.DrawLine(_bezierCollider.transform.position + (Vector3) _bezierCollider.handlerFirstPoint,
                _bezierCollider.transform.position + (Vector3) _bezierCollider.firstPoint);
            Handles.DrawLine(_bezierCollider.transform.position + (Vector3) _bezierCollider.handlerSecondPoint,
                _bezierCollider.transform.position + (Vector3) _bezierCollider.secondPoint);

            _bezierCollider.firstPoint = Handles.FreeMoveHandle(
                                             _bezierCollider.transform.position + (Vector3) _bezierCollider.firstPoint,
                                             Quaternion.identity,
                                             0.04f * HandleUtility.GetHandleSize(
                                                 _bezierCollider.transform.position +
                                                 (Vector3) _bezierCollider.firstPoint), Vector3.zero, Handles.DotCap) -
                                         _bezierCollider.transform.position;
            _bezierCollider.secondPoint = Handles.FreeMoveHandle(
                                              _bezierCollider.transform.position +
                                              (Vector3) _bezierCollider.secondPoint, Quaternion.identity,
                                              0.04f * HandleUtility.GetHandleSize(
                                                  _bezierCollider.transform.position +
                                                  (Vector3) _bezierCollider.secondPoint), Vector3.zero,
                                              Handles.DotCap) - _bezierCollider.transform.position;
            _bezierCollider.handlerFirstPoint = Handles.FreeMoveHandle(
                                                    _bezierCollider.transform.position +
                                                    (Vector3) _bezierCollider.handlerFirstPoint, Quaternion.identity,
                                                    0.04f * HandleUtility.GetHandleSize(
                                                        _bezierCollider.transform.position +
                                                        (Vector3) _bezierCollider.handlerFirstPoint), Vector3.zero,
                                                    Handles.DotCap) - _bezierCollider.transform.position;
            _bezierCollider.handlerSecondPoint = Handles.FreeMoveHandle(
                                                     _bezierCollider.transform.position +
                                                     (Vector3) _bezierCollider.handlerSecondPoint, Quaternion.identity,
                                                     0.04f * HandleUtility.GetHandleSize(
                                                         _bezierCollider.transform.position +
                                                         (Vector3) _bezierCollider.handlerSecondPoint), Vector3.zero,
                                                     Handles.DotCap) - _bezierCollider.transform.position;

            if (GUI.changed) EditorUtility.SetDirty(target);
        }
    }
}

#endif