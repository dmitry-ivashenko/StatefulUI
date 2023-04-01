using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;

namespace StatefulUI.Runtime.References
{
    [Serializable]
    public class InnerComponentReference : BaseReference
    {
        [Role(typeof(InnerComponentRoleAttribute))]
        public int Role;

        [ChildOnly]
        public StatefulComponent InnerComponent;
    }
}