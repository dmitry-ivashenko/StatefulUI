using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using UnityEngine.UI;

namespace StatefulUI.Runtime.References
{
    [Serializable]
    public class SliderReference : BaseReference
    {
        [Role(typeof(SliderRoleAttribute), "Drop Link", "RemoveReference")]
        public int Role;

        [ChildOnly]
        public Slider Slider;
    }
}
