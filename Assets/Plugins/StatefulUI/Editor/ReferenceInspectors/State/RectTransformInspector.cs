using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors.State
{
    public class RectTransformInspector : DescriptionInspector
    {
        protected override int LineCode => 3;

        protected override void OnInspectorGUI(SerializedProperty property, Rect position)
        {
            DrawProperty(property, ref position, nameof(StateDescription.RectTransform));
            DrawProperty(property, ref position, nameof(StateDescription.RectTransformImpactType), "Impact");
            
            var impact = property.FindPropertyRelative(nameof(StateDescription.RectTransformImpactType));

            switch ((RectTransformImpactType)impact.intValue)
            {
                case RectTransformImpactType.SetPosition:
                    DrawProperty(property, ref position, nameof(StateDescription.RectTransformPosition), "Position");
                    break;
                case RectTransformImpactType.SetSize:
                    DrawProperty(property, ref position, nameof(StateDescription.RectTransformSize), "Size");
                    break;
                case RectTransformImpactType.SetRotation:
                    DrawProperty(property, ref position, nameof(StateDescription.RectTransformRotation), "Rotation");
                    break;
                case RectTransformImpactType.SetScale:
                    DrawProperty(property, ref position, nameof(StateDescription.RectTransformScale), "Scale");
                    break;
                case RectTransformImpactType.SetWidth:
                    DrawProperty(property, ref position, nameof(StateDescription.RectTransformWidth), "Width");
                    break;
                case RectTransformImpactType.SetHeight:
                    DrawProperty(property, ref position, nameof(StateDescription.RectTransformHeight), "Height");
                    break;
                case RectTransformImpactType.SetTop:
                    DrawProperty(property, ref position, nameof(StateDescription.RectTransformTop), "Top");
                    break;
                case RectTransformImpactType.SetBottom:
                    DrawProperty(property, ref position, nameof(StateDescription.RectTransformBottom), "Bottom");
                    break;
                case RectTransformImpactType.SetLeft:
                    DrawProperty(property, ref position, nameof(StateDescription.RectTransformLeft), "Left");
                    break;
                case RectTransformImpactType.SetRight:
                    DrawProperty(property, ref position, nameof(StateDescription.RectTransformRight), "Right");
                    break;
                case RectTransformImpactType.SetAnchors:
                    DrawProperty(property, ref position, nameof(StateDescription.RectTransformAnchorMin), "Anchor min");
                    DrawProperty(property, ref position, nameof(StateDescription.RectTransformAnchorMax), "Anchor max");
                    break;
                case RectTransformImpactType.SetPivot:
                    DrawProperty(property, ref position, nameof(StateDescription.RectTransformPivot), "Pivot");
                    break;
            }
        }

        protected override float GetLineCount(SerializedProperty property)
        {
            var result = base.GetLineCount(property);
            var impact = (RectTransformImpactType)property.FindPropertyRelative(nameof(StateDescription.RectTransformImpactType)).intValue;
            if (impact == RectTransformImpactType.SetAnchors) result++;
            return result;
        }
    }
}