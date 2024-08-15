using System;
using StatefulUI.Editor.Core;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.RoleAttributes;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class ContainersInspector : ItemsReferenceInspector
    {
        private readonly SerializedObject _serializedObject;
        private const string ContainersTabName = nameof(StatefulComponent.Containers);

        protected override Type RoleType => typeof(ContainerRoleAttribute);
        protected override string TabName => ContainersTabName;
        protected override SerializedProperty Property { get; }
        private readonly StatefulComponent _statefulComponent;

        public ContainersInspector(SerializedObject serializedObject)
        {
            Property = serializedObject.FindProperty(ContainersTabName);
            _serializedObject = serializedObject;
            _statefulComponent = serializedObject.targetObject as StatefulComponent;
        }
        
        protected override void DrawTitle()
        {
            DrawTitleField(nameof(ContainerReference.Role));
            DrawTitleField(nameof(ContainerReference.Container));
            DrawTitleField(nameof(ContainerReference.Prefab));
            DrawTitleField("Add", ToggleTitleSize);
            DrawTitleField("Clear", ToggleTitleSize);
        }

        protected override void DrawElement(SerializedProperty element, int index)
        {
            var role = element.FindPropertyRelative(nameof(ContainerReference.Role));
            var container = element.FindPropertyRelative(nameof(ContainerReference.Container));

            var containerView = container.objectReferenceValue as ContainerView;
            var prefab = containerView != null ? containerView.Prefab : null;

            EditorGUILayout.PropertyField(role, GUIContent.none);
            EditCommentWindow.Button(_serializedObject, element, RoleUtils.GetName(RoleType, role.intValue));
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(container, GUIContent.none);
            EditorGUILayout.ObjectField(prefab, typeof(ContainerView), true);
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("+", GUILayout.Width(ToggleSize * 2)) && containerView != null)
            {
                containerView.AddTestItem();
            }

            if (GUILayout.Button("✖", GUILayout.Width(ToggleSize * 2)) && containerView != null)
            {
                containerView.ClearTransform();
            }
        }
        
        protected override string CreateItemAPI(string prefix, string name, SerializedProperty element)
        {
            return $"{prefix}GetContainer(ContainerRole.{name});\n";
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