using System;
using StatefulUI.Editor.Core;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.RoleAttributes;
using UnityEditor;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class InnerComponentsInspector : ReadOnlyTwoColumnsInspector
    {
        private const string InnerComponentsTabName = nameof(StatefulComponent.InnerComponents);
        
        protected override Type RoleType => typeof(InnerComponentRoleAttribute);
        protected override string FirstFieldName => nameof(InnerComponentReference.Role);
        protected override string SecondFieldName => nameof(InnerComponentReference.InnerComponent);

        public InnerComponentsInspector(SerializedObject serializedObject) : base(serializedObject, InnerComponentsTabName) { }

        protected override string CreateItemAPI(string prefix, string name, SerializedProperty element)
        {
            var InnerComponent = element.FindPropertyRelative(nameof(InnerComponentReference.InnerComponent));
            var innerPrefix = $"{prefix}GetInnerComponent(InnerComponentRole.{name}).";
            return new StatefulComponentInspector(InnerComponent).CreateAPI(innerPrefix);
        }

        public override string GetTabName()
        {
            var tabName ="Inner Comps";
            return Property.arraySize > 0 ? $"{tabName} ({Property.arraySize})" : tabName;
        }
    }
}
