using System;
using System.Collections.Generic;
using System.Linq;
using StatefulUI.Editor.Selector;
using StatefulUI.Runtime.Core;
using UnityEditor;
using UnityEngine;

// ReSharper disable CheckNamespace

namespace Modules.UI.Selector
{
    public class ItemsSelector : EditorWindow
    {
        private const int rowHeight = 25;
        private const int windowHeight = 400;
        
        private string _filter;
        private string[] _options;
        private Vector2 _scroll;
        private Action<int> _callback;
        private Action<string> _createCallback;
        private string _selected;

        private static GUIStyle _horizontalLineStyle;
        private readonly SelectorStyles _styles = new SelectorStyles();
        private readonly List<string> _filteredList = new List<string>();
        private bool _readyToCreate;
        private bool _needFocusTextInput;
        private int _index;


        public static void Show(string[] options, Rect rect, int index, Action<int> callback, Action<string> createCallback = null)
        {
            var itemsSelector = CreateInstance<ItemsSelector>();
            var position = GUIUtility.GUIToScreenPoint(rect.position);
            itemsSelector.position = new Rect(position.x, position.y + rect.height, Mathf.Max(rect.width, 200), windowHeight);
            itemsSelector._options = options;
            itemsSelector._filteredList.AddRange(options);
            itemsSelector._filter = "";
            itemsSelector._callback = callback;
            itemsSelector._createCallback = createCallback;
            itemsSelector._selected = index >= 0 && index < options.Length ? options[index] : null;
            itemsSelector._index = index;
            itemsSelector._needFocusTextInput = true;
            itemsSelector.UpdateScrollPosition();
            itemsSelector.Search(itemsSelector._filter);
            itemsSelector.ShowPopup();
            itemsSelector.Focus();
        }

        public ItemsSelector()
        {
            Selection.selectionChanged += Repaint;
        }

        private void OnGUI()
        {
            _styles.InitStyles();
            
            var evnt = Event.current;
            var roleName = RoleUtils.ClearName(_filter);
            var showCreate = !_filteredList.Contains(roleName) && _createCallback != null && _filter.Length > 0;
            
            if (evnt.type == EventType.KeyDown)
            {
                if (evnt.keyCode == KeyCode.UpArrow || evnt.keyCode == KeyCode.DownArrow)
                {
                    var totalItems = _filteredList.Count + (showCreate ? 1 : 0);
                    
                    if (_filteredList.Count > 0 || showCreate)
                    {
                        _readyToCreate = false;
                        _index += evnt.keyCode == KeyCode.DownArrow ? 1 : -1;
                        if (_index < 0) _index = totalItems - 1; 
                        _index %= totalItems;

                        _selected = _filteredList.HasIndex(_index) ? _filteredList[_index] : "";
                        _callback?.Invoke(Array.IndexOf(_options, _selected));
                        UpdateScrollPosition();
                        evnt.Use();
                    }

                    _readyToCreate = showCreate && _index == totalItems - 1;
                }

                if (evnt.keyCode == KeyCode.Return)
                {
                    if (_readyToCreate)
                    {
                        CallCreateCallback();
                    }
                    
                    evnt.Use();
                    Close();
                }
                
                if (evnt.keyCode == KeyCode.Escape)
                {
                    evnt.Use();
                    Close();
                }
            }

            using (new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(false)))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUI.BeginChangeCheck();
                    
                    GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
                    {
                        GUILayout.Space(10);
                        GUI.SetNextControlName("NameFilter");
                        var style = GUI.skin.FindStyle("ToolbarSearchTextField");
                        if (style == null)
                        {
                            style = new GUIStyle(GUI.skin.FindStyle("SearchTextField"));
                        }
                        _filter = GUILayout.TextField(_filter ?? "", style);

                        var cancelStyle = GUI.skin.FindStyle("ToolbarSearchCancelButton");
                        if (cancelStyle == null)
                        {
                            cancelStyle = new GUIStyle(GUI.skin.FindStyle("SearchCancelButton"));
                        }
                        
                        if (GUILayout.Button("", cancelStyle))
                        {
                            // Remove focus if cleared
                            _filter = "";
                            GUI.FocusControl(null);
                        }
                        
                        if (_needFocusTextInput)
                        {
                            _needFocusTextInput = false;
                            GUI.FocusControl("NameFilter");
                        }
                    }
                    GUILayout.EndHorizontal();
    
                    if (EditorGUI.EndChangeCheck())
                    {
                        _index = -1;
                        _selected = "";

                        Search(_filter);
                        UpdateScrollPosition();
                    }
                }
            }

            DisplayList();
            
            if (showCreate)
            {
                var style = _readyToCreate ? _styles.SelectedRole : _styles.DeselectedRole;
                
                HorizontalLine(Color.gray);
                
                if (GUILayout.Button($"Create \"{roleName}\"", style))
                {
                    CallCreateCallback();
                    Close();
                }
            }
        }

        public static void HorizontalLine(Color color) 
        {
            if (_horizontalLineStyle == null)
            {
                _horizontalLineStyle = new GUIStyle();
                _horizontalLineStyle.normal.background = EditorGUIUtility.whiteTexture;
                _horizontalLineStyle.margin = new RectOffset( 0, 0, 4, 4 );
                _horizontalLineStyle.fixedHeight = 1;    
            }
            
            var guiColor = GUI.color;
            GUI.color = color;
            GUILayout.Box(GUIContent.none, _horizontalLineStyle);
            GUI.color = guiColor;
        }

        private void CallCreateCallback()
        {
            _createCallback?.Invoke(RoleUtils.ClearName(_filter));
        }

        private void UpdateScrollPosition()
        {
            _scroll.y = Mathf.Max(0, _index * rowHeight - windowHeight / 2);
        }

        private void DisplayList()
        {
            using var scope = new EditorGUILayout.ScrollViewScope(_scroll, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandHeight(true));
            
            _scroll = scope.scrollPosition;

            foreach (var item in _filteredList)
            {
                var style = _selected == item ? _styles.SelectedRole : _styles.DeselectedRole;
                
                if (GUILayout.Button(item.SplitPascalCase(), style))
                {
                    _callback?.Invoke(Array.IndexOf(_options, item));
                    Close();
                }
            }
        }

        private void Search(string filter)
        {
            _filteredList.Clear();
            filter = filter.ToLowerInvariant().Trim();

            var options = _options.ToList();
            if (filter.Length > 0)
            {
                options = options.OrderBy(s => s.DistanceTo(filter)).ToList();
            }
            
            foreach (var option in options)
            {
                var lowerName = option.ToLowerInvariant();
                var isFit =  string.IsNullOrEmpty(filter) 
                           || lowerName.ContainsIgnoreCase(filter);
                
                if (isFit)
                {
                    _filteredList.Add(option);
                }
            }
        }

        private void OnLostFocus()
        {
            Close();
        }
    }
}
