using System;
using UnityEngine;

namespace StatefulUI.Editor.Tree
{
    public enum ConnectionPointType { In, Out }
    
    public class ConnectionPoint
    {
        public Rect Rect;
        public readonly ConnectionPointType Type;
        public readonly Node Node;
        public readonly GUIStyle Style;
        public readonly Action<ConnectionPoint> OnClickConnectionPoint;
    
        public ConnectionPoint(Node node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> onClickConnectionPoint)
        {
            Node = node;
            Type = type;
            Style = style;
            OnClickConnectionPoint = onClickConnectionPoint;
            Rect = new Rect(0, 0, StateTreeEditor.connectorWidth, StateTreeEditor.connectorHeight);
        }
 
        public void Draw()
        {
            Rect.y = Node.rect.y + Node.rect.height * 0.5f - Rect.height * 0.5f;
 
            switch (Type)
            {
                case ConnectionPointType.In:
                    Rect.x = Node.rect.x + Node.rect.width / 2 - Rect.width / 2f;
                    Rect.y = Node.rect.y - Rect.height + Rect.height * 0.75f;
                    break;
 
                case ConnectionPointType.Out:
                    Rect.x = Node.rect.x + Node.rect.width / 2 - Rect.width / 2f;
                    Rect.y = Node.rect.y + Node.rect.height - Rect.height * 0.75f;
                    break;
            }
        
            if (GUI.Button(Rect, "", Style))
            {
                OnClickConnectionPoint?.Invoke(this);
            }
        }
    }
}
