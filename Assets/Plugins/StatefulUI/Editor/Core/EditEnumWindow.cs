#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StatefulUI.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.Core
{
    public class EditEnumWindow : EditorWindow
    {
        private const int width = 250;
        private const int height = 50;
        private const int maxHeight = 500;
        private const int rowHeight = 30;
        public const string DIR = "Assets/Scripts/Modules/UI/Runtime/Roles";
        private Type _type;
        private string _dir;
        private readonly List<string> _newNames = new List<string>();
        private float _mousePosX;
        private float _mousePosY;
        private Vector2 _scroll = Vector2.zero;

        public void Init(Vector2 mousePos, Type type, string dir)
        {
            _type = type;
            _dir = dir ?? DIR;
            _newNames.Clear();
            _newNames.Add("");
            _newNames.Add("");
            _newNames.Add("");
            _mousePosX = mousePos.x - width / 2f;
            _mousePosY = mousePos.y;
            _scroll = new Vector2(0, 0);
        }
        
        private void CreateEnumItem()
        {
            var result = $"// ReSharper disable CheckNamespace\npublic enum {_type.Name}\n{{\n";
            var fields = _type.GetFields();

            foreach (var field in fields)
            {
                if (field.Name.Equals("value__")) continue;
                var value = field.GetRawConstantValue();
                result += $"    {field.Name} = {value},\n";
            }

            foreach (var item in _newNames.FindAll(s => s.IsNonEmpty()))
            {
                var itemName = item.CamelCase().Trim();
                if (!fields.TryFindValue(info => info.Name == itemName, out _))
                {
                    result += $"    {itemName} = {item.GetHashCode()},\n";    
                }
            }

            result += "}\n";

            WriteFile(Path.Combine(_dir, $"{_type.Name}.cs"), result);

            AssetDatabase.Refresh();
        }
        
        
        private void OnGUI()
        {
            var newHeight = Mathf.Min(maxHeight, (_type.GetEnumNames().Length + _newNames.Count) * rowHeight + height);
            position = new Rect(_mousePosX, _mousePosY, width, newHeight);
            
            GUILayout.BeginHorizontal("toolbar");
            {
                EditorGUILayout.LabelField("Enter role name in CamelCaseStyle", EditorStyles.wordWrappedLabel);
            
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(" x ", EditorStyles.toolbarButton))
                {
                    Close();
                }
            }
            GUILayout.EndHorizontal();
        
            GUILayout.Space(5);
            
            var enumNames = _type.GetEnumNames();
            
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                
                if (GUILayout.Button("Save", GUILayout.Width(50)))
                {
                    if (_newNames.Count > 0 && _newNames.Count(item => item.IsNonEmpty()) > 0)
                    {
                        CreateEnumItem();
                    }

                    Close();
                }

                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    _newNames.Add("");
                    _scroll = new Vector2(0, 0);
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);

            _scroll = GUILayout.BeginScrollView(_scroll);
            {
                GUILayout.BeginVertical();
                {
                    for (var index = 0; index < _newNames.Count; index++)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            var newName = _newNames[index];
                            newName = newName.Replace(" ", "");
                            newName = GUILayout.TextField(newName, GUILayout.MaxWidth(200));
                            _newNames[index] = newName;
                            
                            GUILayout.FlexibleSpace();
                        
                            if (GUILayout.Button("-", GUILayout.Width(20)))
                            {
                                _newNames.RemoveAt(index);
                                break;
                            }
                        }
                        GUILayout.EndHorizontal();
                    }

                    foreach (var enumName in enumNames)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(enumName);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();

            }
            GUILayout.EndScrollView();
        }
        
        public static void WriteFile(string path, string content)
        {
            using (var streamWriter = new StreamWriter(path, false))
            {
                streamWriter.Write(content.Replace("\r\n", "\n"));
            }
        }
    }
}

#endif
