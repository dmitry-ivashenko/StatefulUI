using System.Collections.Generic;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.Localization;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.RoleAttributes;
using StatefulUI.Runtime.States;
using TMPro;
using UnityEditor;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class PhrasesFromStatesInspector
    {
        private readonly List<StatePhraseReference> _phrasesFromStates;

        public PhrasesFromStatesInspector(List<StatePhraseReference> phrasesFromStates)
        {
            _phrasesFromStates = phrasesFromStates;
        }

        public void OnInspectorGUI()
        {
            if (_phrasesFromStates.Count == 0) return;
            
            EditorGUILayout.BeginVertical(ReferenceInspector.BoxStyle);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(ReferenceInspector.BoxStyle);
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(nameof(ButtonReference.Role), TitleStyle.Center);
            EditorGUILayout.TextField("Code", TitleStyle.Center);
            EditorGUILayout.TextField("Object", TitleStyle.Center);
            EditorGUILayout.TextField("Value", TitleStyle.Center);
            EditorGUILayout.TextField("Localized", TitleStyle.Center);
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();


            foreach (var phraseReference in _phrasesFromStates)
            {
                EditorGUILayout.BeginHorizontal(ReferenceInspector.BoxStyle);
                var roleName = RoleUtils.GetName(typeof(StateRoleAttribute), phraseReference.Role);
                EditorGUILayout.TextField(roleName, TitleStyle.Center);
                EditorGUILayout.TextField(phraseReference.Identificator);
                EditorGUILayout.ObjectField(phraseReference.TMP, typeof(TextMeshProUGUI), true);

                var value = phraseReference.TMP != null ? phraseReference.TMP.text : "[empty]";
                EditorGUILayout.TextField(value, TitleStyle.Center);
                
                var localizedValue = LocalizationUtils.GetPhrase(phraseReference.Identificator, "---");
                EditorGUILayout.TextField(localizedValue, TitleStyle.Center);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            
            EditorGUI.EndDisabledGroup();
        }
    }
}