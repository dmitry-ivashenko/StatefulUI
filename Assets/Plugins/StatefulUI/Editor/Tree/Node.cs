using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.Tree
{
    public class NodeConfig
    {
        public Vector2 Position { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public GUIStyle NodeStyle { get; set; }
        public GUIStyle SelectedStyle { get; set; }
        public GUIStyle InPointStyle { get; set; }
        public GUIStyle OutPointStyle { get; set; }
        public Action<ConnectionPoint> OnClickInPoint { get; set; }
        public Action<ConnectionPoint> OnClickOutPoint { get; set; }
        public Action<Node> OnClickRemoveNode { get; set; }
        public Action<Node> OnDuplicateNode { get; set; }
        public StateReference StateReference { get; set; }
    }

    public class Node
    {
        private const float pulseAnimationDuration = 0.6f;
        private const float pulseAnimationSize = 80f;
        
        public Rect rect;
        public bool isDragged;
        public bool isSelected;
 
        public readonly ConnectionPoint inPoint;
        public readonly ConnectionPoint outPoint;
 
        public GUIStyle style;
        public readonly GUIStyle defaultNodeStyle;
        public readonly GUIStyle selectedNodeStyle;
 
        public readonly Action<Node> OnRemoveNode;
        public readonly Action<Node> OnDuplicateNode;
        public readonly NodeConfig Conf;
        
        public double LastClickTime;
        public double StartPulseTime;

        public Node(NodeConfig conf)
        {
            Conf = conf;
            rect = new Rect(conf.Position.x, conf.Position.y, conf.Width, conf.Height);
            style = new GUIStyle(conf.NodeStyle);
            inPoint = new ConnectionPoint(this, ConnectionPointType.In, conf.InPointStyle, conf.OnClickInPoint);
            outPoint = new ConnectionPoint(this, ConnectionPointType.Out, conf.OutPointStyle, conf.OnClickOutPoint);
            defaultNodeStyle = style;
            selectedNodeStyle = new GUIStyle(conf.SelectedStyle);
            OnRemoveNode = conf.OnClickRemoveNode;
            OnDuplicateNode = conf.OnDuplicateNode;
        }

        public void Drag(Vector2 delta)
        {
            rect.position += delta;
            Conf.StateReference.NodeData.Position = rect.position;
        }

        public void Pulse(float delay)
        {
            StartPulseTime = EditorApplication.timeSinceStartup + delay;
        }

        public void Draw()
        {
            var time = EditorApplication.timeSinceStartup;
            
            if (time > StartPulseTime && time - StartPulseTime <  pulseAnimationDuration)
            {
                var t = Mathf.Clamp01((float)(time - StartPulseTime) / pulseAnimationDuration);
                
                var color = Handles.color;
                Handles.color = new Color(0.35f, 0.71f, 1f, Mathf.Clamp01(1 - t));
                Handles.DrawSolidArc(rect.center, Vector3.forward, Vector3.up, 360, t * pulseAnimationSize);
                Handles.color = color;

                GUI.changed = true;
            }
            
            inPoint.Draw();
            outPoint.Draw();
            var displayTitle = Title;

            var size = style.CalcSize(new GUIContent(displayTitle));
            
            while (size.x > StateTreeEditor.nodeWidth - StateTreeEditor.nodeTextPadding)
            {
                size = style.CalcSize(new GUIContent(displayTitle));
                style.fontSize--;
            }
            
            GUI.Box(rect, displayTitle, style);
        }

        private string Title => RoleUtils.GetName(typeof(StateRoleAttribute), Conf.StateReference.Role).SplitPascalCase();

        public struct NodeClickInfo
        {
            public bool guiChanged;
            public bool IsDoubleClick;
            public Vector2 DragDelta;
        }

        public void SetSelected(bool selected)
        {
            GUI.changed = true;
            isSelected = selected;
            style = selected ? selectedNodeStyle : defaultNodeStyle;
        }

        public void ProcessContextMenu()
        {
            var genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Duplicate"), false, () => OnDuplicateNode?.Invoke(this));
            genericMenu.AddItem(new GUIContent("Remove"), false, () => OnRemoveNode?.Invoke(this));
            genericMenu.ShowAsContext();
        }
    }
}
