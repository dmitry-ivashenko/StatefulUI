using StatefulUI.Editor.Core;
using StatefulUI.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public abstract class TwoColumnsInspector : ReadOnlyTwoColumnsInspector
    {
        protected TwoColumnsInspector(SerializedObject serializedObject, SerializedProperty serializedProperty, string propertyName)
            : base(serializedObject, serializedProperty, propertyName)
        { }
        
        protected TwoColumnsInspector(SerializedObject serializedObject, string propertyName)
            : base(serializedObject, propertyName)
        { }
        
        protected override void DrawTitle()
        {
            DrawTitleField(FirstFieldName);
            DrawTitleField(SecondFieldName);

            DrawOtherTitles();
            
            if (GUILayout.Button("+", GUILayout.Width(ToggleTitleSize)))
            {
                Property.arraySize++;
            }
        }

        protected virtual void DrawOtherTitles()
        { }

        protected override void DrawElement(SerializedProperty element, int index)
        {
            var role = element.FindPropertyRelative(FirstFieldName);
            var image = element.FindPropertyRelative(SecondFieldName);
            
            EditorGUILayout.PropertyField(role, GUIContent.none);
            EditCommentWindow.Button(_serializedObject, element, RoleUtils.GetName(RoleType, role.intValue));
            EditorGUILayout.PropertyField(image, GUIContent.none);

            DrawOtherFields(element, index);

            if (GUILayout.Button("✖", GUILayout.Width(ToggleTitleSize)))
            {
                Property.DeleteArrayElementAtIndex(index);
            }
        }
        
        protected virtual void DrawOtherFields(SerializedProperty element, int index)
        { }
    }
}