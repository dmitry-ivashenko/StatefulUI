using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StatefulUI.Editor.Selector
{
    public class Hierarchy
    {
        private readonly List<HierarchyEntry> _entries = new List<HierarchyEntry>();
        private readonly List<HierarchyEntry> _entriesFlat = new List<HierarchyEntry>();
        private readonly HierarchyFilter _filter = new HierarchyFilter();
        
        private HierarchyEntry _selectedAssetEntry;

        public HierarchyEntry SelectedAssetEntry
        {
            get => _selectedAssetEntry;
            set
            {
                var entry = value;
				
                while (entry != null)
                {
                    entry.Expanded = true;
                    entry = entry.Parent;
                }
                _selectedAssetEntry = value; 				
            }
        }

        public List<HierarchyEntry> GetEntries(bool isFlatMode)
        {
            return isFlatMode ? _entriesFlat : _entries;
        }
		
        public void UpdateAssetList(SerializedProperty serializedProperty, Type type)
        {
            _entries.Clear();
            _entriesFlat.Clear();

            var target = serializedProperty.serializedObject.targetObject as MonoBehaviour;

            var objects = type != typeof(GameObject) ? target.GetComponentsInChildren(type, true)
                : GetAllChildren(target).ToArray();

            foreach (var element in objects)
            {
                var entry = new HierarchyEntry
                {
                    Asset = element,
                };
                _entries.Add(entry);
            }

            foreach (var entry in _entries)
            {
                SortChildren(entry);
            }

            BuildFlatAssets(_entries);
            UpdateFilter("");
        }

        private List<Object> GetAllChildren(MonoBehaviour target)
        {
            var result = new List<Object>();
            FillChildrenList(target.transform, result);
            return result;
        }
        
        private void FillChildrenList(Transform transform, List<Object> result)
        {
            result.Add(transform.gameObject);

            for (var index = 0; index < transform.childCount; index ++)
            {
                FillChildrenList(transform.GetChild(index), result);
            }
        }
        

        private void SortChildren(HierarchyEntry entry)
        {
            if (entry.Children == null)
            {
                return;
            }

            entry.Children.Sort();
            foreach (var child in entry.Children)
            {
                SortChildren(child);
            }
        }

        private void BuildFlatAssets(List<HierarchyEntry> entries)
        {
            foreach (var entry in entries)
            {
                if (entry.Children != null)
                {
                    BuildFlatAssets(entry.Children);
                }
                else
                {
                    _entriesFlat.Add(entry);
                }
            }
        }

        public void UpdateFilter(string nameFilter)
        {
            _filter.UpdateFilterRecursive(_entries, nameFilter);
        }
    }
}