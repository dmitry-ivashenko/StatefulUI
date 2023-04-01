using System;
using System.Collections.Generic;
using StatefulUI.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.Core
{
    [InitializeOnLoad]
    internal static class HierarchyIcons
    {
        private const int MAX_SELECTION_UPDATE_COUNT = 3;

        private static readonly Dictionary<int, GUIContent> labeledObjects = new Dictionary<int, GUIContent>();
        private static readonly HashSet<int> unlabeledObjects = new HashSet<int>();
        private static GameObject[] previousSelection;
        
        private static readonly Dictionary<Type, GUIContent> typeIcons = new Dictionary<Type, GUIContent>
        {
            { typeof(StatefulComponent), EditorGUIUtility.IconContent("SceneViewOrtho") },
        };

        static HierarchyIcons()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
            ObjectFactory.componentWasAdded += c => UpdateObject(c.gameObject.GetInstanceID());
            Selection.selectionChanged += OnSelectionChanged;
        }

        private static void OnHierarchyGUI(int id, Rect rect)
        {
            if (unlabeledObjects.Contains(id)) return;

            if (ShouldDrawObject(id, out var icon))
            {
                rect.xMin = rect.xMax - 20;
                GUI.Label(rect, icon);
            }
        }

        private static bool ShouldDrawObject(int id, out GUIContent icon)
        {
            return labeledObjects.TryGetValue(id, out icon) || SortObject(id, out icon);
        }

        private static bool SortObject(int id, out GUIContent icon)
        {
            var go = EditorUtility.InstanceIDToObject(id) as GameObject;
            
            if (go != null)
            {
                foreach (var pair in typeIcons)
                {
                    if (go.GetComponent(pair.Key))
                    {
                        labeledObjects.Add(id, icon = pair.Value);
                        return true;
                    }
                }
            }

            unlabeledObjects.Add(id);
            icon = default;
            
            return false;
        }

        private static void UpdateObject(int id)
        {
            unlabeledObjects.Remove(id);
            labeledObjects.Remove(id);
            SortObject(id, out _);
        }

        private static void OnSelectionChanged()
        {
            TryUpdateObjects(previousSelection);
            TryUpdateObjects(previousSelection = Selection.gameObjects);
        }

        private static void TryUpdateObjects(GameObject[] objects)
        {
            if (objects != null && objects.Length > 0 && objects.Length <= MAX_SELECTION_UPDATE_COUNT)
            {
                foreach (var go in objects)
                {
                    UpdateObject(go.GetInstanceID());
                }
            }
        }
    }
}
