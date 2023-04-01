using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors.State
{
    public class AnimationInspector : DescriptionInspector
    {
        protected override int LineCode => 4;
        
        protected override void OnInspectorGUI(SerializedProperty property, Rect position)
        {
            DrawProperty(property, ref position, nameof(StateDescription.AnimationImpactType), "Impact");
            DrawProperty(property, ref position, nameof(StateDescription.AnimationClip), "Clip");
            DrawProperty(property, ref position, nameof(StateDescription.AnimationTarget), "Target");
            DrawProperty(property, ref position, nameof(StateDescription.IsStartTimeRandomized), "Randomize start time");
        }
    }
}