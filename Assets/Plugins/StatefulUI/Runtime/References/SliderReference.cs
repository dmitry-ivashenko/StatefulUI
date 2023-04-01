using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using UnityEngine.UI;

namespace StatefulUI.Runtime.References
{
    [Serializable]
    public class SliderReference : BaseReference
    {
        [Role(typeof(SliderRoleAttribute), "Copy Universal Link", "CopyUniversalLink")]
        public int Role;

        [ChildOnly]
        public Slider Slider;
    }
}
