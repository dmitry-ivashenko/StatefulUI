using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using UnityEngine.UI;

namespace StatefulUI.Runtime.References
{
    [Serializable]
    public class ButtonReference : BaseReference
    {
        [Role(typeof(ButtonRoleAttribute), "Drop Link", "RemoveReference")]
        public int Role;

        [ChildOnly]
        public Button Button;
    }
}
