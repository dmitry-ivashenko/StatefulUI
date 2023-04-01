using System;
using StatefulUI.Editor.Core;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.RoleAttributes;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class TextInputsInspector : ItemsReferenceInspector
    {
        private const string TextsInputsTabName = nameof(StatefulComponent.TextsInputs);
        
        protected override Type RoleType => typeof(TextInputRoleAttribute);
        protected override string TabName => TextsInputsTabName;
        protected override SerializedProperty Property { get; }
        
        private readonly StatefulComponent _statefulComponent;
        private readonly SerializedObject _serializedObject;

        public TextInputsInspector(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;
            _statefulComponent = serializedObject.targetObject as StatefulComponent;
            Property = serializedObject.FindProperty(TextsInputsTabName);
        }
        
        protected override void DrawTitle()
        {
            DrawTitleField(nameof(TextInputReference.Role));
            DrawTitleField("Object");
            DrawTitleField("TMP", ToggleTitleSize);
        }

        protected override void DrawElement(SerializedProperty element, int index)
        {
            var role = element.FindPropertyRelative(nameof(TextInputReference.Role));
            var inputField = element.FindPropertyRelative(nameof(TextInputReference.InputField));
            var inputFieldTMP = element.FindPropertyRelative(nameof(TextInputReference.InputFieldTMP));
            var isTextMeshPro = element.FindPropertyRelative(nameof(TextInputReference.IsTMP));

            EditorGUILayout.PropertyField(role, GUIContent.none);
            EditCommentWindow.Button(_serializedObject, element, RoleUtils.GetName(RoleType, role.intValue));
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(isTextMeshPro.boolValue ? inputFieldTMP : inputField, GUIContent.none);
            EditorGUILayout.PropertyField(isTextMeshPro, GUIContent.none, GUILayout.Width(ToggleSize));
            EditorGUI.EndDisabledGroup();
        }

        protected override string CreateItemAPI(string prefix, string name, SerializedProperty element)
        {
            return $"var value = {prefix}.text;\n";
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