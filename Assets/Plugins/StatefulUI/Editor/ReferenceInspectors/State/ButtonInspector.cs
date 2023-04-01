using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors.State
{
    public class ButtonInspector : DescriptionInspector
    {
        protected override int LineCode => 2;
        
        protected override void OnInspectorGUI(SerializedProperty property, Rect position)
        {
            DrawProperty(property, ref position, nameof(StateDescription.Button));
            DrawProperty(property, ref position, nameof(StateDescription.ButtonInteractable), "Interactable");
        }
    }
}