using System;
using System.Collections.Generic;
using StatefulUI.Editor.Core;
using StatefulUI.Editor.Tree;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class StatesInspector : ItemsReferenceInspector
    {
        public const int RoleFieldWidth = 180;
        public const int TestButtonWidth = 100;
        
        private const string StatesTabName = nameof(StatefulComponent.States);
        
        protected override Type RoleType => typeof(StateRoleAttribute);
        protected override string TabName => StatesTabName;
        protected override SerializedProperty Property { get; }
        
        private readonly Dictionary<SerializedProperty, StateInspector> _items = new Dictionary<SerializedProperty, StateInspector>();
        private readonly StatefulComponent _statefulComponent;
        private readonly SerializedObject _serializedObject;

        public StatesInspector(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;
            Property = serializedObject.FindProperty(StatesTabName);
            _statefulComponent = serializedObject.targetObject as StatefulComponent;
        }
        
        protected override void DrawTitle()
        {
            DrawTitleField(StatesTabName);

            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(ToggleTitleSize)))
            {
                Property.arraySize++;

                var newState = Property.GetArrayElementAtIndex(Property.arraySize - 1);
                var newStateType = newState.FindPropertyRelative(nameof(StateReference.Role));
                newStateType.intValue = 0;
                var newStateDescription = newState.FindPropertyRelative(nameof(StateReference.Description));
                newStateDescription.arraySize = 0;
                var nodeData = newState.FindPropertyRelative(nameof(StateReference.NodeData));

                _serializedObject.ApplyModifiedProperties();

                if (nodeData.GetTargetObjectWithProperty() is StateReference stateReference)
                {
                    _serializedObject.UpdateIfRequiredOrScript();
                    stateReference.NodeData = new NodeData();
                    _serializedObject.ApplyModifiedProperties();
                    
                    _statefulComponent.OnValidate();
                }
            }
        }

        protected override void DrawElement(SerializedProperty element, int index)
        {
            var role = element.FindPropertyRelative(nameof(StateReference.Role));

            var stateInspector = GetStateInspector(element);
            var state = index < _statefulComponent.States.Count ? _statefulComponent.States[index] : null;
            var color = GetGuiColor(state);

            var textColor = EditorStyles.miniPullDown.normal.textColor;
            EditorStyles.miniPullDown.normal.textColor = color;
            EditorGUILayout.PropertyField(role, GUIContent.none, GUILayout.Width(RoleFieldWidth));
            EditorStyles.miniPullDown.normal.textColor = textColor;

            if (state != null && state.Description.Count == 0)
            {
                textColor = GUI.skin.label.normal.textColor;
                GUI.skin.label.normal.textColor = new Color(0.86f, 0.62f,0.67f);
                GUILayout.Label(new GUIContent("★"), GUILayout.Width(TestButtonWidth));
                GUI.skin.label.normal.textColor = textColor;
            }
            
            EditCommentWindow.Button(_serializedObject, element, RoleUtils.GetName(RoleType, role.intValue));

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Test", EditorStyles.miniButton, GUILayout.Width(TestButtonWidth)))
            {
                stateInspector?.Apply();
                SceneView.RepaintAll();
            }
            
            GUILayout.Space(10);

            if (GUILayout.Button("✖", EditorStyles.miniButton, GUILayout.Width(ToggleTitleSize)))
            {
                Property.DeleteArrayElementAtIndex(index);
            }
        }

        private Color GetGuiColor(StateReference stateReference)
        {
            if (stateReference == null) return EditorStyles.miniPullDown.normal.textColor;

            var redColor = new Color(1f, 0.44f, 0.46f);
            foreach (var description in stateReference.Description)
            {
                if (description.IsEmpty) return redColor;
            }

            return stateReference.HasDuplicateRole ? redColor : EditorStyles.miniPullDown.normal.textColor;
        }
        
        protected override void AfterDrawElement(SerializedProperty element, int index)
        {
            base.AfterDrawElement(element, index);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(BoxStyle);
            GetStateInspector(element).OnInspectorGUI();
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }

        private StateInspector GetStateInspector(SerializedProperty element)
        {
            if (!_items.TryGetValue(element, out var items))
            {
                items = new StateInspector(element);
                _items[element] = items;
            }

            return items;
        }

        protected override string CreateItemAPI(string prefix, string name, SerializedProperty element)
        {
            return $"{prefix}SetState(StateRole.{name});\n";
        }

        protected override void DrawExtraButtons()
        {
            if (GUILayout.Button("Tree view"))
            {
                StateTreeEditor.OpenWindow(_statefulComponent);
            }
        }
    }
}
