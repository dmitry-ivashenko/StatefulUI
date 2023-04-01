using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public static class TitleStyle
    {
        private static GUIStyle _titleStyle;
        private static GUIStyle _defaultStyle;

        public static GUIStyle Center => _titleStyle ??= CreateTitleStyle();
        public static GUIStyle DefaultStyle => _defaultStyle ??= CreateDefaultStyle();
        
        private static GUIStyle CreateTitleStyle()
        {
            return new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
        }
        
        private static GUIStyle CreateDefaultStyle()
        {
            return new GUIStyle(GUI.skin.label);
        }
    }
}