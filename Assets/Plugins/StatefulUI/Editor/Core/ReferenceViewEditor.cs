using System.Collections.Generic;
using System.Linq;
using StatefulUI.Editor.ReferenceInspectors;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.Core
{
    [CustomEditor(typeof(StatefulComponent))]
    public class StatefulComponentEditor : UnityEditor.Editor
    {
        private StatefulComponentInspector _statefulComponentInspector;
        private StatefulComponent _statefulComponent;

        private bool _isHistoryFoldout;
        
        private void OnEnable()
        {
            if (target == null) return;

            _statefulComponentInspector = new StatefulComponentInspector(serializedObject);
            _statefulComponent = serializedObject.targetObject as StatefulComponent;
        }

        public override void OnInspectorGUI()
        {
            if (target == null) return;

            serializedObject.UpdateIfRequiredOrScript();
            _statefulComponentInspector.OnInspectorGUI();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.BeginVertical();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.EndDisabledGroup();

            if (Application.isPlaying)
            {
                var history = _statefulComponent.StateHistory
                    .ConvertAll(state => RoleUtils.GetName(typeof(StateRoleAttribute), state))
                    .ToList();

                _isHistoryFoldout = DrawList(_isHistoryFoldout, nameof(StatefulComponent.StateHistory), history);    
            }

            EditorGUILayout.EndVertical();
        }

        private bool DrawList(bool isFoldout, string title, List<string> list)
        {
            isFoldout = EditorGUILayout.Foldout(isFoldout, title, true);
            if (isFoldout)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.indentLevel++;
                foreach (var item in list)
                {
                    EditorGUILayout.TextArea(item, TitleStyle.DefaultStyle);
                }

                EditorGUI.indentLevel--;
                EditorGUI.EndDisabledGroup();
            }

            return isFoldout;
        }
    }
}