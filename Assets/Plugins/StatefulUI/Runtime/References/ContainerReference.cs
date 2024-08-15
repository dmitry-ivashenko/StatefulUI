using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using UnityEngine;

namespace StatefulUI.Runtime.References
{
    [Serializable]
    public class ContainerReference : BaseReference
    {
        [Role(typeof(ContainerRoleAttribute), "Drop Link", "RemoveReference")]
        public int Role;

        [ChildOnly]
        public ContainerView Container;

        public GameObject Prefab => Container.Prefab;
    }
}