using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using UnityEngine.UI;

namespace StatefulUI.Runtime.References
{
    [Serializable]
    public class ButtonReference : BaseReference
    {
        [Role(typeof(ButtonRoleAttribute), "Copy Universal Link", "CopyUniversalLink")]
        public int Role;

        [ChildOnly]
        public Button Button;
    }
}
