using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.RoleAttributes;
using UnityEditor;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class SlidersInspector : ReadOnlyTwoColumnsInspector
    {
        private const string SlidersTabName = nameof(StatefulComponent.Sliders);
        
        protected override Type RoleType => typeof(SliderRoleAttribute);
        protected override string FirstFieldName => nameof(SliderReference.Role);
        protected override string SecondFieldName => nameof(SliderReference.Slider);

        public SlidersInspector(SerializedObject serializedObject)
            : base(serializedObject, SlidersTabName)
        {}

        protected override string CreateItemAPI(string prefix, string name, SerializedProperty element)
        {
            return $"{prefix}GetSlider(SliderRole.{name});\n";
        }
    }
}