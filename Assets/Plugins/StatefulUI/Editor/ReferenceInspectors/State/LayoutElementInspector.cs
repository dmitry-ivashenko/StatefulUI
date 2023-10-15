using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors.State
{
    public class LayoutElementInspector : DescriptionInspector
    {
        protected override int LineCode => 3;

        protected override void OnInspectorGUI(SerializedProperty property, Rect position)
        {
            DrawProperty(property, ref position, nameof(StateDescription.LayoutElement));
            DrawProperty(property, ref position, nameof(StateDescription.LayoutElementImpactType), "Impact");

            var impact = property.FindPropertyRelative(nameof(StateDescription.LayoutElementImpactType));

            switch ((LayoutElementImpactType)impact.intValue)
            {
                case LayoutElementImpactType.LayoutElementPreferredWidth:
                    DrawProperty(property, ref position, nameof(StateDescription.LayoutElementPreferredWidth), "Preferred Width");
                    break;
                case LayoutElementImpactType.LayoutElementPreferredHeight:
                    DrawProperty(property, ref position, nameof(StateDescription.LayoutElementPreferredHeight), "Preferred Height");
                    break;
                case LayoutElementImpactType.LayoutElementMinWidth:
                    DrawProperty(property, ref position, nameof(StateDescription.LayoutElementMinWidth), "Min Width");
                    break;
                case LayoutElementImpactType.LayoutElementMinHeight:
                    DrawProperty(property, ref position, nameof(StateDescription.LayoutElementMinHeight), "Min Height");
                    break;
                case LayoutElementImpactType.LayoutElementFlexibleWidth:
                    DrawProperty(property, ref position, nameof(StateDescription.LayoutElementFlexibleWidth), "Flexible Width");
                    break;
                case LayoutElementImpactType.LayoutElementFlexibleHeight:
                    DrawProperty(property, ref position, nameof(StateDescription.LayoutElementFlexibleHeight), "Flexible Height");
                    break;
            }
        }

        protected override float GetLineCount(SerializedProperty property)
        {
            var result = base.GetLineCount(property);
            return result;
        }
    }
}