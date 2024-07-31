using System.Collections.Generic;
using StatefulUI.Editor.ReferenceInspectors.State;
using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.ReferenceInspectors
{
    [CustomPropertyDrawer(typeof(StateDescription))]
    public class StateEditor : PropertyDrawer
    {
        private static readonly Dictionary<StateDescriptionTargetType, DescriptionInspector> _stateDescriptionInspectors
            = new Dictionary<StateDescriptionTargetType, DescriptionInspector>
            {
                {StateDescriptionTargetType.GameObject, new GameObjectInspector()},
                {StateDescriptionTargetType.Animator, new AnimatorInspector()},
                {StateDescriptionTargetType.Image, new ImageInspector()},
                {StateDescriptionTargetType.TextMeshPro, new TextMeshProInspector()},
                {StateDescriptionTargetType.RectTransform, new RectTransformInspector()},
                {StateDescriptionTargetType.Button, new ButtonInspector()},
                {StateDescriptionTargetType.CanvasGroup, new CanvasGroupInspector()},
                {StateDescriptionTargetType.Animation, new AnimationInspector()},
                {StateDescriptionTargetType.LayoutGroup, new LayoutGroupInspector()},
                {StateDescriptionTargetType.State, new InnerStateInspector()},
                {StateDescriptionTargetType.LayoutElement, new LayoutElementInspector()},
                {StateDescriptionTargetType.Component, new ComponentInspector()},
                {StateDescriptionTargetType.Graphic, new GraphicInspector()},
                {StateDescriptionTargetType.TextMeshProInputField, new TextMeshProInputFieldInspector()},
            };
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _stateDescriptionInspectors[GetDescriptionType(property)].GetPropertyHeight(property);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            _stateDescriptionInspectors[GetDescriptionType(property)].OnGUI(rect, property);
            EditorGUI.EndProperty();
        }

        private StateDescriptionTargetType GetDescriptionType(SerializedProperty property)
        {
            return (StateDescriptionTargetType) property.FindPropertyRelative(nameof(StateDescription.Type)).intValue;
        }
    }
}
