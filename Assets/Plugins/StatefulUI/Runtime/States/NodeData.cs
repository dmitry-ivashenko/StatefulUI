using System;
using System.Collections.Generic;
using UnityEngine;

namespace StatefulUI.Runtime.States
{
    [Serializable]
    public class NodeData
    {
        public Vector2 Position;
        public List<int> ParentRoles = new List<int>();
    }
}