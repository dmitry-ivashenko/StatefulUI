using System;
using StatefulUI.Editor.Core;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.RoleAttributes;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class DropdownsInspector : ItemsReferenceInspector
    {
        private const string DropdownsTabName = nameof(StatefulComponent.Dropdowns);
        
        protected override Type RoleType => typeof(DropdownRoleAttribute);
        protected override string TabName => DropdownsTabName;
        protected override SerializedProperty Property { get; }
        
        private readonly StatefulComponent _statefulComponent;
        private readonly SerializedObject _serializedObject;

        public DropdownsInspector(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;
            _statefulComponent = serializedObject.targetObject as StatefulComponent;
            Property = serializedObject.FindProperty(DropdownsTabName);
        }
        
        protected override void DrawTitle()
        {
            DrawTitleField(nameof(DropdownReference.Role));
            DrawTitleField("Object");
            DrawTitleField("TMP", ToggleTitleSize);
        }

        protected override void DrawElement(SerializedProperty element, int index)
        {
            var role = element.FindPropertyRelative(nameof(DropdownReference.Role));
            var dropdown = element.FindPropertyRelative(nameof(DropdownReference.DropdownField));
            var dropdownTMP = element.FindPropertyRelative(nameof(DropdownReference.DropdownFieldTMP));
            var isTextMeshPro = element.FindPropertyRelative(nameof(DropdownReference.IsTMP));
            
            EditorGUILayout.PropertyField(role, GUIContent.none);
            EditCommentWindow.Button(_serializedObject, element, RoleUtils.GetName(RoleType, role.intValue));
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(isTextMeshPro.boolValue ? dropdownTMP : dropdown, GUIContent.none);
            EditorGUILayout.PropertyField(isTextMeshPro, GUIContent.none, GUILayout.Width(ToggleSize));
            EditorGUI.EndDisabledGroup();
        }
        
        protected override string CreateItemAPI(string prefix, string name, SerializedProperty element)
        {
            return $"var selectedValue = {prefix}.Value;\n";
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