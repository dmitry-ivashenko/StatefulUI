#if UNITY_EDITOR

using System;
using StatefulUI.Runtime.References;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.Core
{
    public class EditCommentWindow : EditorWindow
    {
        private const int width = 250;
        private const int height = 130;
        private const int toolbarHeight = 25;
        private float _mousePosX;
        private float _mousePosY;
        private SerializedProperty _property;
        private SerializedObject _serializedObject;
        private string _name;
        private GUIStyle _style;

        public static void Button(SerializedObject serializedObject, SerializedProperty property, string name = "")
        {
            if (GUILayout.Button(EditorGUIUtility.IconContent("editicon.sml", "Edit comment"), EditorStyles.label, GUILayout.Width(18)))
            {
                var commentWindow = GetWindow<EditCommentWindow>();

                if (commentWindow != null)
                {
                    commentWindow.Close();
                }
                
                var window = CreateInstance<EditCommentWindow>();
                var mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);

                window.Init(mousePos, property.FindPropertyRelative(nameof(BaseReference.Comment)), serializedObject, name);
                window.ShowPopup();
                window.Focus();
            }
        }

        private void Init(Vector2 mousePos, SerializedProperty property, SerializedObject serializedObject, string name)
        {
            _name = name;
            _serializedObject = serializedObject;
            _property = property;
            _mousePosX = mousePos.x - width / 2f;
            _mousePosY = mousePos.y;
            _style = new GUIStyle(GUI.skin.label);
            _style.richText = true;
        }

        private void OnGUI()
        {
            if (_property == null
                || _serializedObject == null
                || Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                Close();
            }

            position = new Rect(_mousePosX, _mousePosY, width, height);

            GUILayout.BeginHorizontal("toolbar");
            {
                EditorGUILayout.LabelField($"Edit comment for role <b>{_name}</b>", _style);
            
                GUILayout.FlexibleSpace();
            
                if (GUILayout.Button(" x ", EditorStyles.toolbarButton))
                {
                    Close();
                }
            }
            GUILayout.EndHorizontal();

            GUI.SetNextControlName("focusElement");
            _property.stringValue = EditorGUILayout.TextArea(_property.stringValue, GUILayout.Height(height - toolbarHeight), GUILayout.ExpandWidth(true));
            EditorGUI.FocusTextInControl("focusElement");

            _serializedObject.ApplyModifiedProperties();            
        }

        private void OnLostFocus()
        {
            try
            {
                Close();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}

#endif
