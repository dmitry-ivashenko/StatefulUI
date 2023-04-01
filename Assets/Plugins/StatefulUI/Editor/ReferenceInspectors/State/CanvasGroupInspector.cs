using System;
using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors.State
{
    public class CanvasGroupInspector : DescriptionInspector
    {
        protected override int LineCode => 3;
        
        protected override void OnInspectorGUI(SerializedProperty property, Rect position)
        {
            DrawProperty(property, ref position, nameof(StateDescription.CanvasGroup));
            DrawProperty(property, ref position, nameof(StateDescription.CanvasGroupImpactType), "Impact");
            
            var impact = property.FindPropertyRelative(nameof(StateDescription.CanvasGroupImpactType));

            switch ((CanvasGroupImpactType)impact.intValue)
            {
                case CanvasGroupImpactType.SetAlpha:
                    DrawProperty(property, ref position, nameof(StateDescription.CanvasGroupAlpha), "Alpha");
                    break;
                case CanvasGroupImpactType.SetInteractable:
                    DrawProperty(property, ref position, nameof(StateDescription.CanvasGroupInteractable), "Interactable");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}