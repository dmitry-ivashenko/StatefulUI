using System;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.Tree
{
    public class Connection
    {
        public readonly ConnectionPoint inPoint;
        public readonly ConnectionPoint outPoint;
        public readonly Action<Connection> OnClickRemoveConnection;

        public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection)
        {
            this.inPoint = inPoint;
            this.outPoint = outPoint;
            this.OnClickRemoveConnection = OnClickRemoveConnection;
        }

        public void Draw()
        {
            var color = Handles.color;
            Handles.color = Color.white;
            Handles.DrawSolidArc(inPoint.Rect.center, Vector3.forward, Vector3.up, 360, 3);
            Handles.DrawSolidArc(outPoint.Rect.center, Vector3.forward, Vector3.up, 360, 3);
            Handles.color = color;
            
            Handles.DrawBezier(
                inPoint.Rect.center,
                outPoint.Rect.center,
                inPoint.Rect.center + Vector2.down * 50f,
                outPoint.Rect.center - Vector2.down * 50f,
                Color.white,
                null,
                5f
            );

            var rectCenter = (inPoint.Rect.center + outPoint.Rect.center) * 0.5f;
            
            if (Handles.Button(rectCenter, Quaternion.identity, 4, 8, CapFunction))
            {
                OnClickRemoveConnection?.Invoke(this);
            }
        }

        private static void CapFunction(int id, Vector3 position, Quaternion rotation, float size, EventType type)
        {
            var color = Handles.color;
            Handles.color = Color.red;
            Handles.DrawLine(position + Vector3.left * size + Vector3.up * size, position - Vector3.left * size - Vector3.up * size);
            Handles.DrawLine(position + Vector3.right * size + Vector3.up * size, position - Vector3.right * size - Vector3.up * size);
            Handles.RectangleHandleCap(id, position, rotation, size, type);
            Handles.color = color;
        }
    }
}