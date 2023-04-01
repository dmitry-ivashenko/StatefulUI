using StatefulUI.Editor.Core;
using StatefulUI.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors
{
    public class ToolsInspector : ReferenceInspector
    {
        private const string TextsTabName = "Tools";
        
        protected override string TabName => TextsTabName;

        private readonly SerializedProperty _localizeOnEnable;
        private readonly SerializedProperty _setIdOnEnable;
        private readonly SerializedProperty _applyInitialStateOnEnable;
        private readonly StateInspector _stateInspector;
        private readonly StatefulComponent _statefulComponent;
        private readonly IAPICreator _apiCreator;

        public ToolsInspector(SerializedObject serializedObject, IAPICreator apiCreator)
        {
            _statefulComponent = serializedObject.targetObject as StatefulComponent;
            _apiCreator = apiCreator;
            _localizeOnEnable = serializedObject.FindProperty(nameof(StatefulComponent.LocalizeOnEnable));
            _applyInitialStateOnEnable = serializedObject.FindProperty(nameof(StatefulComponent.ApplyInitialStateOnEnable));

            _stateInspector = new StateInspector(serializedObject.FindProperty("_initialState"));
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(_localizeOnEnable);
            EditorGUILayout.PropertyField(_applyInitialStateOnEnable);

            if (_applyInitialStateOnEnable.boolValue)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();

                var width = GUILayout.Width(StatesInspector.RoleFieldWidth);
                EditorGUILayout.DropdownButton(new GUIContent("Init"), FocusType.Passive, width);
                
                GUILayout.FlexibleSpace();
                
                if (GUILayout.Button("Test", GUILayout.Width(StatesInspector.TestButtonWidth)))
                {
                    _statefulComponent.ApplyState(_statefulComponent.InitialState);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;
                
                _stateInspector.OnInspectorGUI();
                
                EditorGUI.indentLevel--;
                
                EditorGUILayout.EndVertical();
            }
            
            DrawButtons();
            EditorGUILayout.EndVertical();
        }

        private void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal(BoxStyle);
            if (GUILayout.Button("Update")) Update();
            if (GUILayout.Button("Remove Empty")) RemoveEmpty();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal(BoxStyle);
            if (GUILayout.Button("Copy Full API")) CopyAPI();
            if (GUILayout.Button("Copy Full Docs")) CopyDocs();
            if (GUILayout.Button("Copy Localization")) CopyLocalization();
            EditorGUILayout.EndHorizontal();
        }
        
        private void Update()
        {
            _statefulComponent.OnValidate();
        }

        private void RemoveEmpty()
        {
            _statefulComponent.Buttons.RemoveAll(reference => reference.Button == null);
            _statefulComponent.Animators.RemoveAll(reference => reference.Animator == null);
            _statefulComponent.Containers.RemoveAll(reference => reference.Container == null);
            _statefulComponent.Texts.RemoveAll(reference => reference.IsEmpty);
            _statefulComponent.TextsInputs.RemoveAll(reference => reference.IsEmpty);
            _statefulComponent.Dropdowns.RemoveAll(reference => reference.IsEmpty);
            _statefulComponent.Sliders.RemoveAll(reference => reference.Slider == null);
            _statefulComponent.Toggles.RemoveAll(reference => reference.Toggle == null);
            _statefulComponent.InnerComponents.RemoveAll(reference => reference.InnerComponent == null || reference.InnerComponent == _statefulComponent);
            _statefulComponent.Images.RemoveAll(reference => reference.Image == null);
            _statefulComponent.Objects.RemoveAll(reference => reference.Object == null);
            
            foreach (var stateReference in _statefulComponent.States)
            {
                stateReference.Description?.RemoveAll(description => description.IsEmpty);
            }
        }

        private void CopyAPI()
        {
            EditorGUIUtility.systemCopyBuffer = _apiCreator.CreateAPI();
        }

        private void CopyDocs()
        {
            EditorGUIUtility.systemCopyBuffer = _apiCreator.CreateDocs("- ");
        }

        private void CopyLocalization()
        {
            _statefulComponent.CopyLocalizationInspector();
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