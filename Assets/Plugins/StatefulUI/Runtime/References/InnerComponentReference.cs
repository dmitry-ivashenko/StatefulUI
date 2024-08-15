using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;

namespace StatefulUI.Runtime.References
{
    [Serializable]
    public class InnerComponentReference : BaseReference
    {
        [Role(typeof(InnerComponentRoleAttribute), "Drop Link", "RemoveReference")]
        public int Role;

        [ChildOnly]
        public StatefulComponent InnerComponent;
    }
}
