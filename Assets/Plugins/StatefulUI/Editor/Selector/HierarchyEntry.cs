using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace StatefulUI.Editor.Selector
{
    public class HierarchyEntry : IComparable<HierarchyEntry>
    {
        public HierarchyEntry Parent { get; set; }
        public string Name => Asset.name;
        public Object Asset { get; set;}
        public List<HierarchyEntry> Children { get; set;}
        public bool Expanded { get; set; } = true;
        public bool Hidden { get; set;}

        public int CompareTo(HierarchyEntry other)
        {
            if (Children == null && other.Children != null)
            {
                return 1;
            }

            if (Children != null && other.Children == null)
            {
                return -1;
            }

            return String.CompareOrdinal(Name, other.Name);
        }
    }
}