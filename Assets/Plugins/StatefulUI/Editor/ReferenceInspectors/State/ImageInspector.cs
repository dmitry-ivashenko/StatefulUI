using System;
using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors.State
{
    public class ImageInspector : DescriptionInspector
    {
        protected override int LineCode => 3;
        
        protected override void OnInspectorGUI(SerializedProperty property, Rect position)
        {
            DrawProperty(property, ref position, nameof(StateDescription.Image));
            DrawProperty(property, ref position, nameof(StateDescription.ImageImpact), "Impact");
            
            var impact = property.FindPropertyRelative(nameof(StateDescription.ImageImpact));

            switch ((ImageImpactType)impact.intValue)
            {
                case ImageImpactType.SetColor :
                    DrawProperty(property, ref position, nameof(StateDescription.ImageSetColor), "Color");
                    break;
                case ImageImpactType.SetEnabled:
                    DrawProperty(property, ref position, nameof(StateDescription.ImageEnabled), "Enabled");
                    break;
                case ImageImpactType.SetSprite:
                    DrawProperty(property, ref position, nameof(StateDescription.ImageSetSprite), "Sprite");
                    break;
                case ImageImpactType.SetMaterialFloatValue:
                    DrawProperty(property, ref position, nameof(StateDescription.ImageSetMaterialFloatParam), "Material Float Param Name");
                    DrawProperty(property, ref position, nameof(StateDescription.ImageSetMaterialFloatValue), "Material Float Value");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        protected override float GetLineCount(SerializedProperty property)
        {
            var result = base.GetLineCount(property);
            var impact = (ImageImpactType)property.FindPropertyRelative(nameof(StateDescription.ImageImpact)).intValue;
            if (impact == ImageImpactType.SetMaterialFloatValue) result++;
            return result;
        }
    }
}