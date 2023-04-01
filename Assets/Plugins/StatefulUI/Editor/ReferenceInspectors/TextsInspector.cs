using System;
using StatefulUI.Editor.Core;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.Localization;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.RoleAttributes;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class TextsInspector : ItemsReferenceInspector
    {
        private const string TextsTabName = nameof(StatefulComponent.Texts);
        
        protected override Type RoleType => typeof(TextRoleAttribute);
        protected override string TabName => TextsTabName;
        protected override SerializedProperty Property { get; }

        private readonly PhrasesFromStatesInspector _phrasesFromStatesInspector;
        private readonly StatefulComponent _statefulComponent;
        private readonly SerializedObject _serializedObject;

        public TextsInspector(SerializedObject serializedObject)
        {
            Property = serializedObject.FindProperty(TextsTabName);

            _serializedObject = serializedObject;
            _statefulComponent = (StatefulComponent) serializedObject.targetObject;
            _phrasesFromStatesInspector = new PhrasesFromStatesInspector(_statefulComponent.PhrasesFromStates);
        }
        
        protected override void DrawTitle()
        {
            DrawTitleField(nameof(ButtonReference.Role));
            DrawTitleField("Code");
            DrawTitleField("Object");
            DrawTitleField("Localize");
            DrawTitleField("Value");
            DrawTitleField("Localized");
        }

        protected override void DrawElement(SerializedProperty element, int index)
        {
            var role = element.FindPropertyRelative(nameof(TextReference.Role));
            var identificator = element.FindPropertyRelative(nameof(TextReference.Identificator));
            var isTextMeshPro = element.FindPropertyRelative(nameof(TextReference.IsTextMeshPro));
            var text = element.FindPropertyRelative(nameof(TextReference.Text));
            var textMeshPro = element.FindPropertyRelative(nameof(TextReference.TMP));
            var localization = element.FindPropertyRelative(nameof(TextReference.Localize));

            EditorGUILayout.PropertyField(role, GUIContent.none);
            
            EditCommentWindow.Button(_serializedObject, element, RoleUtils.GetName(RoleType, role.intValue));

            var textColor = EditorStyles.textField.normal.textColor;
            EditorStyles.textField.normal.textColor = GetIdentificatorColor(_statefulComponent.Texts[index]);
            EditorGUILayout.PropertyField(identificator, GUIContent.none);
            EditorStyles.textField.normal.textColor = textColor;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(isTextMeshPro.boolValue ? textMeshPro : text, GUIContent.none);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(localization, GUIContent.none, GUILayout.Width(ToggleSize));

            var value = GetValue(element);
            var localizedValue = LocalizationUtils.GetPhrase(identificator.stringValue, "---");

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(value);
            EditorGUILayout.TextField(localizedValue);
            EditorGUI.EndDisabledGroup();
        }
        
        private Color GetIdentificatorColor(TextReference textReference)
        {
            if (textReference.LocalizedTimes > 1)
            {
                return new Color(1f, 0.44f, 0.46f);
            }

            return textReference.ContainsInInnerComponent ? new Color(1f, 0.94f, 0.73f) : EditorStyles.label.normal.textColor;
        }
        
        protected override void DrawAdditionalItems()
        {
            _phrasesFromStatesInspector.OnInspectorGUI();
        }
        
        protected override void DrawExtraButtons()
        {
            base.DrawExtraButtons();
            
            if (GUILayout.Button("Update"))
            {
                _statefulComponent.OnValidate();
            }

            if (GUILayout.Button("Copy Localization"))
            {
                _statefulComponent.CopyLocalizationInspector();
            }
        }

        protected override string CreateItemAPI(string prefix, string name, SerializedProperty element)
        {
            var value = GetValue(element);
            return $"{prefix}SetText(TextRole.{name}, \"{value}\");\n";
        }

        private static string GetValue(SerializedProperty element)
        {
            var isTextMeshPro = element.FindPropertyRelative(nameof(TextReference.IsTextMeshPro));
            var text = element.FindPropertyRelative(nameof(TextReference.Text));
            var textMeshPro = element.FindPropertyRelative(nameof(TextReference.TMP));

            var textValue = text.objectReferenceValue as Text;
            var textMeshProValue = textMeshPro.objectReferenceValue as TMP_Text;

            return isTextMeshPro.boolValue && textMeshProValue != null ? textMeshProValue.text
                : textValue != null ? textValue.text : "[empty]";
        }
    }
}