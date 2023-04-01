using System.Collections.Generic;
using StatefulUI.Editor.ReferenceInspectors;
using StatefulUI.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.Core
{
    public class StatefulComponentInspector : IAPICreator
    {
        private readonly SerializedObject _serializedObject;
        private readonly List<ReferenceInspector> _inspectors;
        private readonly string[] _tabs;

        private static int _index;

        public StatefulComponentInspector(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;
            _inspectors = new List<ReferenceInspector>
            {
                new ButtonsInspector(serializedObject),
                new TextsInspector(serializedObject),
                new AnimatorsInspector(serializedObject),
                new ContainersInspector(serializedObject),
                new SlidersInspector(serializedObject),
                new TogglesInspector(serializedObject),
                new InnerComponentsInspector(serializedObject),
                new ImagesInspector(serializedObject),
                new VideoPlayerInspector(serializedObject),
                new DropdownsInspector(serializedObject),
                new TextInputsInspector(serializedObject),
                new ObjectsInspector(serializedObject),
                new StatesInspector(serializedObject),
                new ToolsInspector(serializedObject, this)
            };

            _tabs = new string[_inspectors.Count];
        }
        
        public StatefulComponentInspector(SerializedProperty serializedProperty)
            : this(new SerializedObject(serializedProperty.objectReferenceValue))
        { }

        public void OnInspectorGUI()
        {
            UpdateTabs();
            _index = GUILayout.SelectionGrid(_index, _tabs, _tabs.Length / 3);
            _inspectors[_index].OnInspectorGUI();
        }

        private void UpdateTabs()
        {
            for (var index = 0; index < _tabs.Length; index++)
            {
                _tabs[index] = _inspectors[index].GetTabName();
            }
        }

        public string CreateAPI(string prefix = "")
        {
            var result = "";

            foreach (var inspector in _inspectors)
            {
                result += inspector.CreateAPI(prefix);
            }

            return result;
        }

        public string CreateDocs(string prefix = "")
        {
            var result = $"{_serializedObject.targetObject.name}:\n\n";

            foreach (var inspector in _inspectors)
            {
                var docs = inspector.CreateDocs(prefix);
                if (docs.IsNonEmpty())
                {
                    result += docs + "\n";    
                }
            }

            return result;
        }
    }
}