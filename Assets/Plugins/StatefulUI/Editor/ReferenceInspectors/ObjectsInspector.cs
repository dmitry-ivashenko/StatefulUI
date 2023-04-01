using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.RoleAttributes;
using UnityEditor;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class ObjectsInspector : TwoColumnsInspector
    {
        private const string ObjectsTabName = nameof(StatefulComponent.Objects); 
            
        protected override Type RoleType => typeof(ObjectRoleAttribute);
        protected override string FirstFieldName => nameof(ObjectReference.Role);
        protected override string SecondFieldName => nameof(ObjectReference.Object);

        public ObjectsInspector(SerializedObject serializedObject)
            : base(serializedObject, ObjectsTabName)
        {
        }

        protected override string CreateItemAPI(string prefix, string name, SerializedProperty element)
        {
            return $"{prefix}GetObject(ObjectRole.{name});\n";
        }

        protected override void DrawExtraButtons()
        {
        }
    }
}