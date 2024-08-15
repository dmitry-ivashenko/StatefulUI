using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using UnityEngine.UI;

namespace StatefulUI.Runtime.References
{
    [Serializable]
    public class ImageReference : BaseReference
    {
        [Role(typeof(ImageRoleAttribute), "Drop Link", "RemoveReference")]
        public int Role;

        [ChildOnly]
        public Image Image;
    }
}
