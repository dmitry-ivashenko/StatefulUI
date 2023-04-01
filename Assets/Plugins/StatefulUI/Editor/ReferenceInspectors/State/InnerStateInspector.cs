using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors.State
{
    public class InnerStateInspector : DescriptionInspector
    {
        protected override int LineCode => 2;
        
        protected override void OnInspectorGUI(SerializedProperty property, Rect position)
        {
            DrawProperty(property, ref position, nameof(StateDescription.InnerStatefulComponent), "Inner Reference View");
            DrawProperty(property, ref position, nameof(StateDescription.StateRole), "State Role");
        }
    }
}