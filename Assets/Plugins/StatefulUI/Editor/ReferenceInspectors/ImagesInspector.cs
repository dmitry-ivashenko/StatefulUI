using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.RoleAttributes;
using UnityEditor;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class ImagesInspector : TwoColumnsInspector
    {
        private const string ImagesTabName = nameof(StatefulComponent.Images);
        
        protected override Type RoleType => typeof(ImageRoleAttribute);
        protected override string FirstFieldName => nameof(ImageReference.Role);
        protected override string SecondFieldName => nameof(ImageReference.Image);

        public ImagesInspector(SerializedObject serializedObject)
            : base(serializedObject, ImagesTabName)
        {
        }
        
        protected override string CreateItemAPI(string prefix, string name, SerializedProperty element)
        {
            return $"{prefix}GetImage(ImageRole.{name});\n";
        }

        protected override void DrawExtraButtons()
        {
        }
    }
}