using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using UnityEngine;

namespace StatefulUI.Runtime.References
{
    // ReSharper disable CheckNamespace


    [Serializable]
    public class ObjectReference : BaseReference
    {
        [Role(typeof(ObjectRoleAttribute), "Copy Universal Link", "CopyUniversalLink")]
        public int Role;


        [ChildOnly]
        public GameObject Object;
    }
}