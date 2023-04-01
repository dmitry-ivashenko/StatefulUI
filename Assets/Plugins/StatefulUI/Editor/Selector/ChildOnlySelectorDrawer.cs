using System.Reflection;
using StatefulUI.Editor.Core;
using StatefulUI.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.Selector
{
	[CustomPropertyDrawer(typeof(ChildOnlyAttribute))]
	public class ChildOnlySelectorDrawer : PropertyDrawer
	{
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var assetType = fieldInfo.FieldType;
			
			var childOnlyAttribute = attribute as ChildOnlyAttribute;

			ChildOnlySelector.PropertyField(position, property, label, assetType, () =>
			{
				if (childOnlyAttribute.HasSelector)
				{
					CallAction(property, childOnlyAttribute.OnSelected);
				}
			});
		}
		
		private void CallAction(SerializedProperty property, string methodName)
		{
			var target = property.GetTargetObjectWithProperty();
			var method = target.GetType().GetMethod(methodName,
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			method?.Invoke(target, new object[]{});
		}
	}
}