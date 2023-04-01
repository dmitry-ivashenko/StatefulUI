using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.RoleAttributes;
using UnityEditor;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class AnimatorsInspector : ReadOnlyTwoColumnsInspector
    {
        private const string AnimatorsTabName = nameof(StatefulComponent.Animators);

        protected override Type RoleType => typeof(AnimatorRoleAttribute);
        protected override string FirstFieldName => nameof(AnimatorReference.Role);
        protected override string SecondFieldName => nameof(AnimatorReference.Animator);

        public AnimatorsInspector(SerializedObject serializedObject)
            : base(serializedObject, AnimatorsTabName)
        {}

        protected override string CreateItemAPI(string prefix, string name, SerializedProperty element)
        {
            return $"{prefix}GetAnimator(AnimatorRoles.{name});\n";
        }
    }
}