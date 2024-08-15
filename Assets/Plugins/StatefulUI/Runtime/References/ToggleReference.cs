using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using UnityEngine.UI;

namespace StatefulUI.Runtime.References
{
    [Serializable]
    public class ToggleReference : BaseReference
    {
        [Role(typeof(ToggleRoleAttribute), "Drop Link", "RemoveReference")]
        public int Role;

        [ChildOnly]
        public Toggle Toggle;
    }
}
