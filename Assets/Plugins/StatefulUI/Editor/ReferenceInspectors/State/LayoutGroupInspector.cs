using System;
using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors.State
{
    public class LayoutGroupInspector : DescriptionInspector
    {
        protected override int LineCode => 3;
        
        protected override void OnInspectorGUI(SerializedProperty property, Rect position)
        {
            DrawProperty(property, ref position, nameof(StateDescription.LayoutGroup), "Target");
            DrawProperty(property, ref position, nameof(StateDescription.LayoutGroupImpactType), "Type");
            
            var impact = property.FindPropertyRelative(nameof(StateDescription.LayoutGroupImpactType));

            switch ((LayoutGroupImpactType) impact.intValue)
            {
                case LayoutGroupImpactType.SetReverseArrangement:
                    DrawProperty(property, ref position, nameof(StateDescription.IsLayoutGroupReversed), "Reverse Element Order");
                    break;
                case LayoutGroupImpactType.BottomPadding:
                    DrawProperty(property, ref position, nameof(StateDescription.PaddingValue), "Padding Value");
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }



        }
    }
}