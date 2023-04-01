using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.Core
{
    public static class SerializedPropertyExtension
    {
        public static void DrawPropertyField(this SerializedProperty property, string name, string caption)
        {
            var propertyRelative = property.FindPropertyRelative(name);
            EditorGUILayout.PropertyField(propertyRelative, new GUIContent(caption));
        }
        
        public static void DrawPropertyField(this SerializedProperty property, string name)
        {
            var propertyRelative = property.FindPropertyRelative(name);
            EditorGUILayout.PropertyField(propertyRelative);
        }
        
        public static object GetTargetObjectWithProperty(this SerializedProperty property)
        {
            var path = property.propertyPath.Replace(".Array.data[", "[");
			
            object targetObject = property.serializedObject.targetObject;
            var elements = path.Split('.');
			
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("["))
                        .Replace("[", "").Replace("]", ""));

                    targetObject = GetValue(targetObject, elementName, index);
                }
                else
                {
                    targetObject = GetValue(targetObject, element);
                }
            }
            return targetObject;
        }
		
        private static object GetValue(object source, string name)
        {
            if (source == null) return null;
            var type = source.GetType();

            while (type != null)
            {
                var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (field != null) return field.GetValue(source);

                var property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null) return property.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        private static object GetValue(object source, string name, int sourceIndex)
        {
            var enumerable = GetValue(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enumerator = enumerable.GetEnumerator();

            for (var index = 0; index <= sourceIndex; index++)
            {
                if (!enumerator.MoveNext()) return null;
            }

            return enumerator.Current;
        }
    }
}