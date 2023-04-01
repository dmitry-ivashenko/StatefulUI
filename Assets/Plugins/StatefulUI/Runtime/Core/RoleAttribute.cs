using System;
using UnityEngine;

namespace StatefulUI.Runtime.Core
{
    public class RoleAttribute : PropertyAttribute
    {
        public Type Type { get; }

        public string Name { get; }
        public string Action { get; }

        public bool HasAction => !string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(Name); 

        public RoleAttribute(Type type)
        {
            Type = type;
        }
        
        public RoleAttribute(Type type, string name, string action)
        {
            Type = type;
            Name = name;
            Action = action;
        }
    }

    public class InnerRoleAttribute : RoleAttribute
    {
        public InnerRoleAttribute(Type type) : base(type)
        {
        }
    }
}