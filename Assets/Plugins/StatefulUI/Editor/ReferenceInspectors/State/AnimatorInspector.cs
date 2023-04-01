using System;
using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors.State
{
    public class AnimatorInspector : DescriptionInspector
    {
        protected override int LineCode => 3;
        
        protected override void OnInspectorGUI(SerializedProperty property, Rect position)
        {
            DrawProperty(property, ref position, nameof(StateDescription.Animator));
            DrawProperty(property, ref position, nameof(StateDescription.AnimImpact), "Impact");
            
            var impact = property.FindPropertyRelative(nameof(StateDescription.AnimImpact));
            
            switch ((AnimatorImpactType)impact.intValue)
            {
                case AnimatorImpactType.SetBoolTrue:
                    DrawProperty(property, ref position, nameof(StateDescription.AnimParamSetBoolTrue), "Param Name");
                    break;
                case AnimatorImpactType.SetBoolFalse:
                    DrawProperty(property, ref position, nameof(StateDescription.AnimParamSetBoolFalse), "Param Name");
                    break;
                case AnimatorImpactType.SetTrigger:
                    DrawProperty(property, ref position, nameof(StateDescription.AnimParamSetTrigger), "Param Name");
                    break;
                case AnimatorImpactType.ResetTrigger:
                    DrawProperty(property, ref position, nameof(StateDescription.AnimParamResetTrigger), "Param Name");
                    break;
                case AnimatorImpactType.SetInteger:
                    DrawProperty(property, ref position, nameof(StateDescription.AnimParamSetInteger), "Param Name");
                    DrawProperty(property, ref position, nameof(StateDescription.AnimParamSetIntegerValue), "Value");
                    break;
                case AnimatorImpactType.SetFloat:
                    DrawProperty(property, ref position, nameof(StateDescription.AnimParamSetFloat), "Param Name");
                    DrawProperty(property, ref position, nameof(StateDescription.AnimParamSetFloatValue), "Value");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override float GetLineCount(SerializedProperty property)
        {
            var result = base.GetLineCount(property);
            var impact = (AnimatorImpactType)property.FindPropertyRelative(nameof(StateDescription.AnimImpact)).intValue;
            if (impact == AnimatorImpactType.SetInteger || impact == AnimatorImpactType.SetFloat) result++;
            return result;
        }
    }
}