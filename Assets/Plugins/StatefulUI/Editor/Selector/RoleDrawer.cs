using System;
using System.Collections.Generic;
using Modules.UI.Selector;
using StatefulUI.Editor.Core;
using StatefulUI.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.Selector
{
    [CustomPropertyDrawer(typeof(RoleAttribute))]
    public class RoleDrawer : PropertyDrawer
    {
        private ExtensionMethodWrapper _extensionMethodWrapper;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var roleAttribute = (RoleAttribute) attribute;

            DrawRole(position, roleAttribute.Type, property);

            var currentEvent = Event.current;

            if (roleAttribute.HasAction && currentEvent.type == EventType.MouseDown && currentEvent.button == 1 
                && position.Contains(currentEvent.mousePosition)) 
            {
                if (_extensionMethodWrapper == null)
                {
                    var statefulComponent = property.serializedObject.targetObject as StatefulComponent;
                    var target = property.GetTargetObjectWithProperty();
                    _extensionMethodWrapper = new ExtensionMethodWrapper(target, statefulComponent, roleAttribute.Action);
                }

                var context = new GenericMenu();
                context.AddItem(new GUIContent(roleAttribute.Name), false, _extensionMethodWrapper.Invoke);
                context.ShowAsContext();
            }
        }
        
        protected virtual void DrawRole(Rect position, Type attributeType, SerializedProperty property)
        {
            var options = RoleUtils.GetOptions(attributeType);
            var index = RoleUtils.GetIndex(attributeType, property.intValue);
            var title = options[index];

            if (EditorGUI.DropdownButton(position, new GUIContent(title), FocusType.Passive))
            {
                ItemsSelector.Show(options, position, index, selectedIndex =>
                {
                    if (selectedIndex >= 0 && selectedIndex < options.Length)
                    {
                        SaveValue(property, RoleUtils.GetValue(attributeType, selectedIndex));
                    }
                },
                newRoleName =>
                {
                    RoleGenerator.CreateNotExistsRoles();
                    RoleGenerator.Generate(attributeType, RoleUtils.GetRoleType(attributeType), new List<string>{ newRoleName });
                    var newIndex = RoleUtils.GetOptions(attributeType).IndexOf(newRoleName);
                    SaveValue(property, RoleUtils.GetValue(attributeType, newIndex));
                });
            }
        }

        protected static void SaveValue(SerializedProperty property, int value)
        {
            if (property.intValue != value)
            {
                property.serializedObject.Update();
                property.intValue = value;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
