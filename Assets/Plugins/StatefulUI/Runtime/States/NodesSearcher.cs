using System.Collections.Generic;
using StatefulUI.Runtime.Core;

namespace StatefulUI.Runtime.States
{
    public static class NodesSearcher
    {
        private static readonly List<StateReference> _tmpList = new List<StateReference>();
        private static readonly Queue<StateReference> _searchQueue = new Queue<StateReference>();
        private static readonly HashSet<StateReference> _markerSet = new HashSet<StateReference>();

        public static List<StateReference> GetParents(StateReference state, StatefulComponent view)
        {
            _tmpList.Clear();
            _searchQueue.Clear();
            _markerSet.Clear();
            
            _searchQueue.Enqueue(state);
            _markerSet.Add(state);

            while (_searchQueue.Count > 0)
            {
                var parentRoles = _searchQueue.Dequeue().NodeData.ParentRoles;

                foreach (var parentRole in parentRoles)
                {
                    if (parentRole == 0) continue;

                    var parentReference = view.States.Find(reference => reference.Role == (int) parentRole);
                    if (parentReference == null || _markerSet.Contains(parentReference)) continue;

                    _markerSet.Add(parentReference);
                    _tmpList.Add(parentReference);

                    _searchQueue.Enqueue(parentReference);
                }
            }

            _tmpList.Reverse();
            
            return _tmpList;
        }

        public static List<StateReference> GetChildren(StateReference state, StatefulComponent view)
        {
            _tmpList.Clear();
            _searchQueue.Clear();
            _markerSet.Clear();

            _searchQueue.Enqueue(state);
            _markerSet.Add(state);

            while (_searchQueue.Count > 0)
            {
                var current = _searchQueue.Dequeue();

                var children = view.States.FindAll(reference => reference.NodeData.ParentRoles
                    .Contains(current.Role));

                foreach (var child in children)
                {
                    if (child.Role == 0) continue;

                    if (_markerSet.Contains(child)) continue;

                    _markerSet.Add(child);
                    _tmpList.Add(child);

                    _searchQueue.Enqueue(child);
                }
            }

            return _tmpList;
        }
    }
}