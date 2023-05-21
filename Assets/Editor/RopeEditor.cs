using System.Collections.Generic;
using Core.Logging;
using Game;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RopeBehaviour))]
public class RopeEditor : OdinEditor {
    private enum HandleAxis {
        X,
        Y,
        Z
    }
    
    private struct HandleData {
        public HandleAxis Axis { get; }
        public bool Start { get; }
        public int ID { get; }

        public HandleData(HandleAxis axis, bool start, int id = -1) {
            Axis  = axis;
            Start = start;
            ID    = id == -1 ? GUIUtility.GetControlID(FocusType.Passive) : id;
        }
    }

    private class RopeHandle {
        public HandleData[] Data { get; }
        public bool Start { get; }
        public Vector3 Position { get; }
        public int[] AxisIDs { get; }

        public RopeHandle(Vector3 position, bool start) {
            Position = position;
            Start     = start;
            Data = new[] {
                new HandleData(HandleAxis.X, start),
                new HandleData(HandleAxis.Y, start),
                new HandleData(HandleAxis.Z, start),
            };
            AxisIDs = new[] {
                Data[0].ID,
                Data[1].ID,
                Data[2].ID
            };
        }
    }
    
    private RopeHandle startHandle;
    private RopeHandle endHandle;
    
    private bool cached;
    private RopeBehaviour rope;
    private int nearestID;
    
    protected virtual void OnSceneGUI() {
        if (!cached) {
            rope        = (RopeBehaviour) target;
            startHandle = new RopeHandle(rope.start, true);
            endHandle   = new RopeHandle(rope.end, false);
            cached      = true;
        }
        
        switch (Event.current.type) {
            case EventType.Repaint:
                DrawPointGizmo(rope.GetStartPos(), startHandle, EventType.Repaint);
                DrawPointGizmo(rope.GetEndPos(), endHandle, EventType.Repaint);
                break;
            
            case EventType.Layout:
                DrawPointGizmo(rope.GetStartPos(), startHandle, EventType.Layout);
                DrawPointGizmo(rope.GetEndPos(), endHandle, EventType.Layout);
                break;
            
            case EventType.MouseDown:
                nearestID = HandleUtility.nearestControl;
                break;
            
            case EventType.MouseUp:
                nearestID = -1;
                break;
            
            case EventType.MouseDrag:
                if (Event.current.button == 0) {
                    
                }
                break;
        }
    }

    private void DrawPointGizmo(Vector3 point, RopeHandle handle, EventType eventType) {
        DrawArrows(point, handle.Data, eventType, 0.5f);
        var label = handle.Start ? "Start" : "End";
        DrawLabels(point, label);
    }

    private void DrawArrows(Vector3 pos, HandleData[] data, EventType eventType, float size = 1f) {
        // Vector4 drawPos = rope.transform.worldToLocalMatrix * pos;
        HandleData xData = data[0];
        HandleData yData = data[1];
        HandleData zData = data[2];
        if (IsEventRepaint()) Handles.color = GetHandleColor(xData);
        Handles.ArrowHandleCap(
            xData.ID,
            pos,
            Quaternion.identity * Quaternion.LookRotation(Vector3.right),
            size,
            eventType
        );
        if (IsEventRepaint()) Handles.color = GetHandleColor(yData);
        Handles.ArrowHandleCap(
            yData.ID,
            pos,
            Quaternion.identity * Quaternion.LookRotation(Vector3.up),
            size,
            eventType
        );
        if (IsEventRepaint()) Handles.color = GetHandleColor(zData);
        Handles.ArrowHandleCap(
            zData.ID,
            pos,
            Quaternion.identity * Quaternion.LookRotation(Vector3.forward),
            size,
            eventType
        );
    }
    
    private void DrawLabels(Vector3 pos, string text, float distance = 1f) {
        Handles.Label(pos + new Vector3(0f, distance, 0f), text);
    }

    private static bool IsEventRepaint() => Event.current.type == EventType.Repaint;
    
    private Color GetHandleColor(HandleData data) {
        Color returnColor = default;
        if (nearestID == data.ID) return Color.yellow;
        switch (data.Axis) {
            case HandleAxis.X:
                returnColor = Handles.xAxisColor;
                break;
            case HandleAxis.Y:
                returnColor = Handles.yAxisColor;
                break;
            case HandleAxis.Z:
                returnColor = Handles.zAxisColor;
                break;
        }
        return returnColor;
    }

    private RopeHandle GetNearestHandle() {
        return null;
    }
}