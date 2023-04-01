using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.Selector
{
    public class SelectorStyles
    {
        public GUIStyle Selected => _selectedStyle;
        public GUIStyle Object => _objectStyle;
        public GUIStyle Folder => _folderStyle;
        public GUIStyle SelectedRole => _selectedRole; 
        public GUIStyle DeselectedRole => _deselectedRole; 

        private static GUIStyle _selectedRole;
        private static GUIStyle _deselectedRole;
        private static GUIStyle _selectedStyle;
        private static GUIStyle _objectStyle;
        private static GUIStyle _folderStyle;
		
        public void InitStyles()
        {
            if (_selectedStyle == null)
            {
                _selectedStyle = new GUIStyle(EditorStyles.label);
                _selectedStyle.normal = _selectedStyle.focused;
                _selectedStyle.active = _selectedStyle.focused;
                _selectedStyle.onNormal = _selectedStyle.focused;
                _selectedStyle.onActive = _selectedStyle.focused;
            }
            
            if (_selectedRole == null)
            {
                var style = new GUIStyle(EditorStyles.label)
                {
                    border = new RectOffset(0, 0, 0, 1),
                    padding = new RectOffset(20, 0, 5, 5),
                    alignment = TextAnchor.MiddleLeft,
                    margin = new RectOffset(0, 0, 0, 0),
                    overflow = new RectOffset(0, 50, 0, 0)
                };

                style.normal = new GUIStyleState
                {
                    textColor = EditorStyles.whiteLabel.normal.textColor,
                    background = CreateColoredTexture(new Color(0.24f, 0.49f, 0.9f)),
                };
                
                style.normal = style.normal;
                style.active = style.normal;
                style.hover = style.normal;
                style.focused = style.normal;
                
                style.onNormal = style.normal;
                style.onActive = style.normal;
                style.onHover = style.normal;
                style.onFocused = style.normal;
                
                _selectedRole = style;
            }
            
            if (_deselectedRole == null)
            {
                var style = new GUIStyle(EditorStyles.label)
                {
                    border = new RectOffset(0, 0, 0, 1),
                    padding = new RectOffset(20, 0, 5, 5),
                    alignment = TextAnchor.MiddleLeft,
                    margin = new RectOffset(0, 0, 0, 0),
                    overflow = new RectOffset(0, 50, 0, 0)
                };
                
                _deselectedRole = style;
            }

            if (_objectStyle == null)
            {
                _objectStyle = new GUIStyle(EditorStyles.label);
                _objectStyle.active = _objectStyle.normal;
                _objectStyle.focused = _objectStyle.normal;
                _objectStyle.onActive = _objectStyle.normal;
                _objectStyle.onFocused = _objectStyle.normal;
            }

            if (_folderStyle == null)
            {
                _folderStyle = new GUIStyle(EditorStyles.foldout);
                _folderStyle.focused = _folderStyle.normal;
                _folderStyle.active = _folderStyle.normal;
                _folderStyle.hover = _folderStyle.normal;
                _folderStyle.onFocused = _folderStyle.normal;
                _folderStyle.onActive = _folderStyle.normal;
                _folderStyle.onHover = _folderStyle.normal;
            }
        }

        private static Texture2D CreateColoredTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}