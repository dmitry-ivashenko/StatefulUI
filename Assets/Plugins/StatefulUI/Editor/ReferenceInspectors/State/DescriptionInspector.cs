using StatefulUI.Editor.Core;
using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors.State
{
    public abstract class DescriptionInspector
    {
        private static readonly float LineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        protected abstract int LineCode { get; }

        public void OnGUI(Rect rect, SerializedProperty property)
        {
            var typeProperty = property.FindPropertyRelative(nameof(StateDescription.Type));
            var stateDescription = (StateDescription) typeProperty.GetTargetObjectWithProperty();
            var type = (StateDescriptionTargetType) typeProperty.intValue;
            var title = new GUIContent(type + " - " + stateDescription.ReadableDescription);
            var position = new Rect(rect.position.x, rect.position.y, rect.width, EditorGUIUtility.singleLineHeight);
            
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, title, true);
            position.y += LineHeight;

            if (property.isExpanded)
            {
                EditorGUI.PropertyField(position, typeProperty, new GUIContent("Type"));
                position.y += LineHeight;

                OnInspectorGUI(property, position);
            }
        }

        protected abstract void OnInspectorGUI(SerializedProperty property, Rect position);

        protected void DrawProperty(SerializedProperty property, ref Rect rect, string name)
        {
            DrawProperty(property, ref rect, name, name);
        }
        
        protected void DrawProperty(SerializedProperty property, ref Rect rect, string name, string caption)
        {
            var relativeProperty = property.FindPropertyRelative(name);
            EditorGUI.PropertyField(rect, relativeProperty, new GUIContent(caption));
            rect.y += LineHeight;
        }

        public float GetPropertyHeight(SerializedProperty property)
        {
            var lineCount = property.isExpanded ? GetLineCount(property) + 2 : 1;
            return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount-1);
        }

        protected virtual float GetLineCount(SerializedProperty property)
        {
            return LineCode;
        }
    }
}