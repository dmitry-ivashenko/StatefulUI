using StatefulUI.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.Selector
{
    [CustomPropertyDrawer(typeof(ProjectAssetOnlyAttribute))]
    public class ProjectAssetOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var component = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(GameObject), false);

            if (property.objectReferenceValue != component)
            {
                property.serializedObject.Update();
                property.objectReferenceValue = component;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}