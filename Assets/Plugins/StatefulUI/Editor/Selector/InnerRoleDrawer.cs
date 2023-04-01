using System;
using Modules.UI.Selector;
using StatefulUI.Editor.Core;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.Selector
{
    [CustomPropertyDrawer(typeof(InnerRoleAttribute))]
    public class InnerRoleDrawer : RoleDrawer
    {
        protected override void DrawRole(Rect position, Type attributeType, SerializedProperty property)
        {
            if (property.GetTargetObjectWithProperty() is StateDescription stateDescription && stateDescription.InnerStatefulComponent != null)
            {
                var roles = stateDescription.InnerStatefulComponent.States.ConvertAll(state => state.Role);
                var options = roles.ConvertAll(role => RoleUtils.GetName(attributeType, role).ToString())
                    .ToArray();
                var index = roles.IndexOf(stateDescription.StateRole);
                var title = index >= 0 ? options[index] : "";
                
                if (EditorGUI.DropdownButton(position, new GUIContent(title), FocusType.Passive))
                {
                    ItemsSelector.Show(options, position, index, selectedIndex =>
                    {
                        if (selectedIndex >= 0 && selectedIndex < options.Length)
                        {
                            SaveValue(property, roles[selectedIndex]);
                        }
                    });
                }
            }
            else
            {
                base.DrawRole(position, attributeType, property);
            }
        }
    }
}