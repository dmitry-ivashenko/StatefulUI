using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;

namespace StatefulUI.Editor.Core
{
    public static class RoleGenerator
    {
        public const string ComponentsDirectory = "Assets/StatefulUISupport/Scripts/Components";
        public const string RolesDirectory = "Assets/StatefulUISupport/Scripts/Roles";

        private static readonly List<Type> _attributeTypes = new List<Type>
        {
            typeof(AnimatorRoleAttribute),
            typeof(ButtonRoleAttribute),
            typeof(ContainerRoleAttribute),
            typeof(DropdownRoleAttribute),
            typeof(ImageRoleAttribute),
            typeof(InnerComponentRoleAttribute),
            typeof(ObjectRoleAttribute),
            typeof(ScrollerItemRoleAttribute),
            typeof(ScrollerRoleAttribute),
            typeof(SliderRoleAttribute),
            typeof(StateRoleAttribute),
            typeof(TextInputRoleAttribute),
            typeof(TextRoleAttribute),
            typeof(ToggleRoleAttribute),
            typeof(VideoPlayerRoleAttribute),
        };

        private static readonly Dictionary<string, string> _components = new Dictionary<string, string>
        {
            { "StatefulView", ExtensionGenerator.GenerateStatefulView() },
            { "StatefulComponentExtensions", ExtensionGenerator.GenerateStatefulComponentExtensions() },
        };

        public static void GenerateAll()
        {
            CreateNotExistsRoles();
            GenerateNotExistsComponents();
        }

        private static void GenerateNotExistsComponents()
        {
            if (!Directory.Exists(ComponentsDirectory))
            {
                Directory.CreateDirectory(ComponentsDirectory);
            }

            foreach (var component in _components)
            {
                var path = Path.Combine(ComponentsDirectory, $"{component.Key}.cs");
                if (File.Exists(path)) continue;
                WriteFile(path, component.Value);
            }
        }

        public static void CreateNotExistsRoles()
        {
            if (!Directory.Exists(RolesDirectory))
            {
                Directory.CreateDirectory(RolesDirectory);
            }

            foreach (var attributeType in _attributeTypes)
            {
                var enumName = GetEnumName(attributeType.Name);
                if (File.Exists(Path.Combine(RolesDirectory, $"{enumName}.cs"))) continue;
                Generate(attributeType, null);
            }
        }
        
        public static void Generate(Type attributeType, Type type, List<string> newRoles = null)
        {
            if (!Directory.Exists(RolesDirectory))
            {
                Directory.CreateDirectory(RolesDirectory);
            }

            var enumName = GetEnumName(attributeType.Name);
            var typeGenerator = new TypeGenerator()
                .AddAttribute("StatefulUI.Runtime.RoleAttributes." + attributeType.Name)
                .SetEnum(enumName);

            var path = Path.Combine(RolesDirectory, $"{enumName}.cs");
            AddFields(typeGenerator, type, path);

            if (newRoles != null)
            {
                foreach (var newRole in newRoles)
                {
                    if (!string.IsNullOrEmpty(newRole))
                    {
                        var hashCode = newRole.GetHashCode();
                        typeGenerator.AddEnumField(newRole, hashCode);
                        RoleUtils.AddTemporaryRole(attributeType, newRole, hashCode);
                    }
                }
            }

            typeGenerator.AddBottom();
            
            var result = typeGenerator.ToString();
            WriteFile(path, result);
        }

        private static string GetEnumName(string attributeName)
        {
            return attributeName.Replace("Attribute", "");
        }

        private static void AddFields(TypeGenerator typeGenerator, Type type, string path)
        {
            if (type != null)
            {
                var items = File.ReadAllLines(path)
                    .ToList()
                    .FindAll(str => Regex.IsMatch(str, "^\\s*\\S+\\s*=\\s*\\S+,?\\s*$"))
                    .ToDictionary(str => Regex.Match(str, "^\\s*(\\S+?)\\s*=.*$").Groups[1].Value, 
                        str => Regex.Match(str, "=\\s*([^,\\s]+),?\\s*").Groups[1].Value);

                foreach (var field in items)
                {
                    if (field.Key.Equals("value__") && field.Key != "Unknown") continue;
                    typeGenerator.AddEnumField(field.Key, field.Value);
                }
            }
            else
            {
                typeGenerator.AddEnumField("Unknown", 0);
            }
        }
        
        private static void WriteFile(string path, string content)
        {
            using var streamWriter = new StreamWriter(path, false);
            streamWriter.Write(content.Replace("\r\n", "\n"));
        }
    }
}
