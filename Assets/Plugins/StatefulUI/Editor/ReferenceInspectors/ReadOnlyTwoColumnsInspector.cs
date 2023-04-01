using StatefulUI.Editor.Core;
using StatefulUI.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public abstract class ReadOnlyTwoColumnsInspector : ItemsReferenceInspector
    {
        protected override string TabName { get; }

        protected abstract string FirstFieldName { get; }
        protected abstract string SecondFieldName { get; }
        protected override SerializedProperty Property { get; }

        protected readonly StatefulComponent _statefulComponent;
        protected readonly SerializedObject _serializedObject;

        protected ReadOnlyTwoColumnsInspector(SerializedObject serializedObject, string propertyName)
        {
            _serializedObject = serializedObject;
            _statefulComponent = serializedObject.targetObject as StatefulComponent;
            Property = serializedObject.FindProperty(propertyName);
            TabName = propertyName;
        }

        protected ReadOnlyTwoColumnsInspector(SerializedObject serializedObject, SerializedProperty serializedProperty, string propertyName)
        {
            _serializedObject = serializedObject;
            Property = serializedProperty.FindPropertyRelative(propertyName);
            TabName = propertyName;
        }

        protected override void DrawTitle()
        {
            DrawTitleField(FirstFieldName);
            DrawTitleField(SecondFieldName);
        }

        protected override void DrawElement(SerializedProperty element, int index)
        {
            var firstField = element.FindPropertyRelative(FirstFieldName);
            var secondField = element.FindPropertyRelative(SecondFieldName);

            EditorGUILayout.PropertyField(firstField, GUIContent.none);
            
            EditCommentWindow.Button(_serializedObject, element, RoleUtils.GetName(RoleType, firstField.intValue));
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(secondField, GUIContent.none);
            EditorGUI.EndDisabledGroup();
        }

        protected override void DrawExtraButtons()
        {
            if (GUILayout.Button("Update"))
            {
                _statefulComponent.OnValidate();
            }
        }
    }
}