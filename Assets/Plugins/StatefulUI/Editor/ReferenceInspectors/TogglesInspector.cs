using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.RoleAttributes;
using UnityEditor;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class TogglesInspector : ReadOnlyTwoColumnsInspector
    {
        private const string TogglesTabName = nameof(StatefulComponent.Toggles);
        
        protected override Type RoleType => typeof(ToggleRoleAttribute);
        protected override string FirstFieldName => nameof(ToggleReference.Role);
        protected override string SecondFieldName => nameof(ToggleReference.Toggle);

        public TogglesInspector(SerializedObject serializedObject)
            : base(serializedObject, TogglesTabName)
        {}

        protected override string CreateItemAPI(string prefix, string name, SerializedProperty element)
        {
            return $"{prefix}GetVideoPlayer(VideoPlayerRole.{name});\n";
        }
    }
}