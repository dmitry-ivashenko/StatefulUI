using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public abstract class ItemsReferenceInspector : ReferenceInspector
    {
        protected const string RoleFieldName = "Role";
        
        protected abstract SerializedProperty Property { get; }
        protected abstract Type RoleType { get; }

        public override string GetTabName()
        {
            var tabName = TabName.TitleCase();
            return Property.arraySize > 0 ? $"{tabName} ({Property.arraySize})" : tabName;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                EditorGUILayout.BeginHorizontal(BoxStyle);
                DrawTitle();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(2);

                for (var index = 0; index < Property.arraySize; index++)
                {
                    var element = Property.GetArrayElementAtIndex(index);

                    EditorGUILayout.BeginHorizontal(BoxStyle);
                    DrawElement(element, index);
                    EditorGUILayout.EndHorizontal();

                    if (index < Property.arraySize)
                    {
                        AfterDrawElement(element, index);
                    }
                }

                DrawAdditionalItems();

                EditorGUILayout.BeginHorizontal(BoxStyle);
                DrawButtons();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawAdditionalItems()
        {
        }

        protected virtual void DrawButtons()
        {
            DrawExtraButtons();

            if (GUILayout.Button(CopyAPIButtonLabel))
            {
                EditorGUIUtility.systemCopyBuffer = CreateAPI();
            }
            
            if (GUILayout.Button(CopyDocsButtonLabel))
            {
                EditorGUIUtility.systemCopyBuffer = CreateDocs("- ");
            }
        }

        protected abstract void DrawTitle();

        protected abstract void DrawElement(SerializedProperty element, int index);

        protected virtual void AfterDrawElement(SerializedProperty element, int index) 
        { }

        public override string CreateAPI(string prefix = "")
        {
            var result = "";
            
            for (var index = 0; index < Property.arraySize; index ++)
            {
                var element = Property.GetArrayElementAtIndex(index);
                
                var role = element.FindPropertyRelative(RoleFieldName).intValue;
                var name = RoleUtils.GetName(RoleType, role);
                result += role != 0 ? CreateItemAPI(prefix, name, element) : "";
            }

            return result;
        }

        protected abstract string CreateItemAPI(string prefix, string name, SerializedProperty element);

        public override string CreateDocs(string prefix = "")
        {
            if (Property.arraySize == 0) return "";
            
            var result = "";
            
            for (var index = 0; index < Property.arraySize; index ++)
            {
                var element = Property.GetArrayElementAtIndex(index);
                
                var role = element.FindPropertyRelative(RoleFieldName).intValue;
                var name = RoleUtils.GetName(RoleType, role);
                result += role != 0 ? CreateItemDocs($"    {prefix}", name, element) : "";
            }
            
            if (result.IsNonEmpty())
            {
                result = $"{TabName}:\n{result}";
            }

            return result;
        }

        protected virtual string CreateItemDocs(string prefix, string name, SerializedProperty element)
        {
            var property = element.FindPropertyRelative(nameof(BaseReference.Comment));
            var comment = property.stringValue;
            if (comment.IsNonEmpty())
            {
                comment = $" - {comment}";
            }

            return $"{prefix}{name}{comment}\n";
        }

        protected void DrawTitleField(string title)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextArea(title, TitleStyle.Center);
            EditorGUI.EndDisabledGroup();
        }
        
        protected void DrawTitleField(string title, int labelWidth)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextArea(title, TitleStyle.Center, GUILayout.Width(labelWidth));
            EditorGUI.EndDisabledGroup();
        }

        protected virtual void DrawExtraButtons()
        { }
    }
}