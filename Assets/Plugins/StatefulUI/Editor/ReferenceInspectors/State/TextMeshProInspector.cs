using System;
using StatefulUI.Runtime.Localization;
using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors.State
{
    public class TextMeshProInspector : DescriptionInspector
    {
        protected override int LineCode => 3;
        
        protected override void OnInspectorGUI(SerializedProperty property, Rect position)
        {
            DrawProperty(property, ref position, nameof(StateDescription.TextMeshPro));
            DrawProperty(property, ref position, nameof(StateDescription.TextMeshProImpact), "Impact");
            
            var impact = property.FindPropertyRelative(nameof(StateDescription.TextMeshProImpact));

            switch ((TextMeshProImpactType)impact.intValue)
            {
                case TextMeshProImpactType.SetColor :
                    DrawProperty(property, ref position, nameof(StateDescription.TextMeshProColor), "Color");
                    break;
                case TextMeshProImpactType.SetPhrase:
                    DrawProperty(property, ref position, nameof(StateDescription.TextMeshProPhraseCode), "Phrase Code");
                    var phraseCode = property.FindPropertyRelative(nameof(StateDescription.TextMeshProPhraseCode));
                    var localizedValue = LocalizationUtils.GetPhrase(phraseCode.stringValue, "---");
                    EditorGUI.LabelField(position, new GUIContent("Value"), new GUIContent(localizedValue));
                    break;
                case TextMeshProImpactType.SetFontSize:
                    DrawProperty(property, ref position, nameof(StateDescription.TextMeshProFontSize), "Font size");
                    break;
                case TextMeshProImpactType.SetMaxFontSize:
                    DrawProperty(property, ref position, nameof(StateDescription.TextMeshProMaxFontSize), "Max font size");
                    break;
                case TextMeshProImpactType.SetFont :
                    DrawProperty(property, ref position, nameof(StateDescription.TextMeshProFont), "Font");
                    break;
                case TextMeshProImpactType.SetHorizontalAlignment :
                    DrawProperty(property, ref position, nameof(StateDescription.TextMeshProTextHorizontalAlignment), "Horizontal Alignment");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        protected override float GetLineCount(SerializedProperty property)
        {
            var result = base.GetLineCount(property);
            var impact = (TextMeshProImpactType)property
                .FindPropertyRelative(nameof(StateDescription.TextMeshProImpact)).intValue;

            if (impact == TextMeshProImpactType.SetPhrase)
            {
                result++;
            }

            return result;
        }
    }
}