using StatefulUI.Runtime.Core;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public abstract class ReferenceInspector
    {
        public static readonly GUIStyle BoxStyle = new GUIStyle();

        protected const string CopyAPIButtonLabel = "Copy API";
        protected const string CopyDocsButtonLabel = "Copy Docs";
        
        protected const int ToggleSize = 16;
        protected const int ToggleTitleSize = 32;

        protected abstract string TabName { get; }

        public abstract void OnInspectorGUI();
        
        public abstract string CreateAPI(string prefix = "");
        
        public abstract string CreateDocs(string prefix = "");

        public virtual string GetTabName()
        {
            return TabName.TitleCase();
        }
    }
}