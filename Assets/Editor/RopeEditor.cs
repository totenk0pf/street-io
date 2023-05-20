using Game;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RopeBehaviour))]
public class RopeEditor : Editor {
    protected virtual void OnSceneGUI() {
        switch (Event.current.type) {
            case EventType.Repaint:
                RopeBehaviour rope = (RopeBehaviour) target;
                DrawArrows(rope.start, 0, HandleUtility.GetHandleSize(rope.start));
                DrawArrows(rope.end, 1, HandleUtility.GetHandleSize(rope.end));
                break;
            
            case EventType.Layout:
                
                break;
        }
    }

    private void DrawArrows(Vector3 pos, int id, float size = 1f) {
        Handles.color = Handles.xAxisColor;
        Handles.ArrowHandleCap(
            id,
            pos,
            Quaternion.identity * Quaternion.LookRotation(Vector3.right),
            size,
            EventType.Repaint
        );
        Handles.color = Handles.yAxisColor;
        Handles.ArrowHandleCap(
            id,
            pos,
            Quaternion.identity * Quaternion.LookRotation(Vector3.up),
            size,
            EventType.Repaint
        );
        Handles.color = Handles.zAxisColor;
        Handles.ArrowHandleCap(
            id,
            pos,
            Quaternion.identity * Quaternion.LookRotation(Vector3.forward),
            size,
            EventType.Repaint
        );
    }
}