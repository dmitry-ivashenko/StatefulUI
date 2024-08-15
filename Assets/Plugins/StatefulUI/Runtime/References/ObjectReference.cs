using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using UnityEngine;

namespace StatefulUI.Runtime.References
{
    [Serializable]
    public class ObjectReference : BaseReference
    {
        [Role(typeof(ObjectRoleAttribute), "Drop Link", "RemoveReference")]
        public int Role;


        [ChildOnly]
        public GameObject Object;
    }
}
