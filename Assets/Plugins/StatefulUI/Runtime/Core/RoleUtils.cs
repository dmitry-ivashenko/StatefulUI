using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StatefulUI.Runtime.RoleAttributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StatefulUI.Runtime.Core
{
    public static class RoleUtils
    {
        public static Type StateRoleType => typeof(StateRoleAttribute);
        public static Type AnimatorRoleType => typeof(AnimatorRoleAttribute);
        public static Type ButtonRoleType => typeof(ButtonRoleAttribute);
        public static Type ContainerRoleType => typeof(ContainerRoleAttribute);
        public static Type DropdownRoleType => typeof(DropdownRoleAttribute);
        public static Type ImageRoleType => typeof(ImageRoleAttribute);
        public static Type InnerComponentRoleType => typeof(InnerComponentRoleAttribute);
        public static Type ObjectRoleType => typeof(ObjectRoleAttribute);
        public static Type SliderRoleType => typeof(SliderRoleAttribute);
        public static Type TextInputRoleType => typeof(TextInputRoleAttribute);
        public static Type TextRoleType => typeof(TextRoleAttribute);
        public static Type ToggleRoleType => typeof(ToggleRoleAttribute);
        public static Type VideoPlayerRoleType => typeof(VideoPlayerRoleAttribute);
        
        private static readonly string[] _emptyOptions = {"Unknown"};
        private static readonly Dictionary<Type, Type> _types = new Dictionary<Type, Type>();
        private static readonly Dictionary<Type, string[]> _options = new Dictionary<Type, string[]>();
        private static readonly Dictionary<Type, List<int>> _values = new Dictionary<Type, List<int>>();
        private static readonly Dictionary<Type, Dictionary<int, int>> _indexes = new Dictionary<Type, Dictionary<int, int>>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Reset()
        {
            _types.Clear();
            _options.Clear();
            _values.Clear();
            _indexes.Clear();
        }

        public static Type GetRoleType(Type attributeType)
        {
            if (_types.TryGetValue(attributeType, out var type))
            {
                return type;
            }

            type = GetRoleTypeImpl(attributeType);

            if (type != null)
            {
                _types[attributeType] = type;
            }

            return type;
        }

        // Add fake selectable role until recompiling
        public static void AddTemporaryRole(Type attributeType, string name, int hashCodeValue)
        {
            var type = GetRoleType(attributeType);
            var list = GetOptions(attributeType).ToList();
            
            if (list.Contains(name)) return;
            
            list.Add(name);
            _options[attributeType] = list.ToArray();

            GetIndex(attributeType, hashCodeValue);
            var dictionary = _indexes[type];
            var index = dictionary.Values.Count;
            dictionary[hashCodeValue] = index;

            GetValue(attributeType, index);
            _values[type].Add(hashCodeValue);
        }

        public static string[] GetOptions(Type attributeType)
        {
            var type = GetRoleType(attributeType);

            if (type == null) return _emptyOptions;

            if (_options.TryGetValue(attributeType, out var result)) return result;

            return _options[attributeType] = type.GetFields()
                .Select(value => value.Name)
                .Where(value => !value.Equals("value__"))
                .ToArray();
        }

        public static int GetIndex(Type attributeType, int hashCodeValue)
        {
            var type = GetRoleType(attributeType);
            if (type == null) return 0;

            if (!_indexes.TryGetValue(type, out var indexes))
            {
                indexes = new Dictionary<int, int>();

                var index = 0;
                foreach (var field in type.GetFields())
                {
                    if (!field.Name.Equals("value__"))
                    {
                        indexes[(int) field.GetRawConstantValue()] = index;
                        index++;
                    }
                }

                _indexes[type] = indexes;
            }

            return indexes.TryGetValue(hashCodeValue, out var result) ? result : 0;
        }

        public static int GetValue(Type attributeType, int index)
        {
            var type = GetRoleType(attributeType);
            if (type == null) return 0;

            if (_values.TryGetValue(type, out var list))
            {
                return list.HasIndex(index) ? list[index] : 0;
            }

            list = new List<int>();
            _values[type] = list;
            
            foreach (var field in type.GetFields())
            {
                if (!field.Name.Equals("value__"))
                {
                    list.Add((int) field.GetRawConstantValue());
                }
            }
            
            return index < list.Count ? list[index] : 0;
        }

        public static string GetName(Type attributeType, int role)
        {
            var index = GetIndex(attributeType, role);
            return GetOptions(attributeType)[index];
        }

        private static Type GetRoleTypeImpl(Type attributeType)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attributes = type.GetCustomAttributes(false);

                    foreach (var attribute in attributes)
                    {
                        if (attribute.GetType() == attributeType)
                        {
                            return type;
                        }
                    }
                }
            }

            return null;
        }

        public static string ClearName(string str)
        {
            return Regex.Split(str, "\\s+")
                .ToList()
                .ConvertAll(input => input.CamelCase())
                .Join("")
                .Replace(" ", "");
        }
        
        public static string GetName(this Object obj)
        {
            return obj == null ? "null" : obj.name;
        }
    }
}