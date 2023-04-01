using System;
using System.Collections.Generic;
using UnityEngine;

namespace StatefulUI.Runtime.States
{
    [Serializable]
    public class NodeData : ISerializationCallbackReceiver
    {
        public Vector2 Position;
        public int ParentRole;
        public bool HasParent;
        public List<int> ParentRoles = new List<int>();

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (HasParent && !ParentRoles.Contains(ParentRole))
            {
                ParentRoles.Add(ParentRole);
            }
        }
    }
}