using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.RoleAttributes;
using UnityEditor;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class ButtonsInspector : ReadOnlyTwoColumnsInspector
    {
        private const string ButtonsTabName = nameof(StatefulComponent.Buttons);
        
        protected override Type RoleType =>  typeof(ButtonRoleAttribute);
        protected override string FirstFieldName => nameof(ButtonReference.Role);
        protected override string SecondFieldName => nameof(ButtonReference.Button);

        public ButtonsInspector(SerializedObject serializedObject)
            : base(serializedObject, ButtonsTabName)
        {}
        
        protected override string CreateItemAPI(string prefix, string name, SerializedProperty element)
        {
            return $"{prefix}GetButton(ButtonRole.{name});\n";
        }
    }
}
