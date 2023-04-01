using StatefulUI.Editor.Core;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.States;
using UnityEditor;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class StateInspector : ReferenceInspector
    {
        private readonly SerializedProperty _descriptions;
        private readonly StateReference _state;
        private readonly StatefulComponent _statefulComponent;

        protected override string TabName => string.Empty;

        public StateInspector(SerializedProperty serializedProperty)
        {
            _statefulComponent = serializedProperty.serializedObject.targetObject as StatefulComponent;
            _descriptions = serializedProperty.FindPropertyRelative(nameof(StateReference.Description));
            _state = _descriptions.GetTargetObjectWithProperty() as StateReference;
        }

        public void Apply()
        {
            _statefulComponent.ApplyState(_state);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_descriptions);
        }

        public override string CreateAPI(string prefix = "")
        {
            return "";
        }

        public override string CreateDocs(string prefix = "")
        {
            return "";
        }
    }
}