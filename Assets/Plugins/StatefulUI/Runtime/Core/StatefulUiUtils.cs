using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using StatefulUI.Runtime.References;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable UnusedMember.Local

namespace StatefulUI.Runtime.Core
{
    public static class StatefulUiUtils
    {
        private static void RemoveReference(this TextReference @ref, StatefulComponent obj) => obj.DropItem(@ref);
        private static void RemoveReference(this ToggleReference @ref, StatefulComponent obj) => obj.DropItem(@ref);
        private static void RemoveReference(this SliderReference @ref, StatefulComponent obj) => obj.DropItem(@ref);
        private static void RemoveReference(this ButtonReference @ref, StatefulComponent obj) => obj.DropItem(@ref);
        private static void RemoveReference(this ImageReference @ref, StatefulComponent obj) => obj.DropItem(@ref);
        private static void RemoveReference(this TextInputReference @ref, StatefulComponent obj) => obj.DropItem(@ref);
        private static void RemoveReference(this DropdownReference @ref, StatefulComponent obj) => obj.DropItem(@ref);
        private static void RemoveReference(this ContainerReference @ref, StatefulComponent obj) => obj.DropItem(@ref);
        private static void RemoveReference(this InnerComponentReference @ref, StatefulComponent obj) => obj.DropItem(@ref);
        private static void RemoveReference(this VideoPlayerReference @ref, StatefulComponent obj) => obj.DropItem(@ref);
        private static void RemoveReference(this ObjectReference @ref, StatefulComponent obj) => obj.DropItem(@ref);

        public static int IndexOf<T>(this IList<T> list, T item)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (Equals(list[i], item))
                {
                    return i;
                }
            }

            return -1;
        }

        public static string Join<T>(this IEnumerable<T> sequence, string delimeter = ", ")
        {
            return string.Join(delimeter, sequence);
        }

        public static Color ChangeR(this Color input, float value)
        {
            input.r = value;
            return input;
        }

        public static Color ChangeG(this Color input, float value)
        {
            input.g = value;
            return input;
        }

        public static Color ChangeB(this Color input, float value)
        {
            input.b = value;
            return input;
        }

        public static Color ChangeA(this Color input, float value)
        {
            input.a = value;
            return input;
        }

        public static Vector2 ChangeX(this Vector2 input, float newX)
        {
            input.x = newX;
            return input;
        }

        public static Vector2 ChangeY(this Vector2 input, float newY)
        {
            input.y = newY;
            return input;
        }

        public static Vector3 ChangeX(this Vector3 input, float newX)
        {
            input.x = newX;
            return input;
        }

        public static Vector3 ChangeY(this Vector3 input, float newY)
        {
            input.y = newY;
            return input;
        }

        public static Vector3 ChangeZ(this Vector3 input, float newZ)
        {
            input.z = newZ;
            return input;
        }

        public static Vector3 ModifyX(this Vector3 input, float newX)
        {
            var output = input;
            output.x += newX;
            return output;
        }

        public static Vector2 ModifyX(this Vector2 input, float newX)
        {
            var output = input;
            output.x += newX;
            return output;
        }

        public static Vector3 ModifyY(this Vector3 input, float newY)
        {
            var output = input;
            output.y += newY;
            return output;
        }

        public static Vector2 ModifyY(this Vector2 input, float newY)
        {
            var output = input;
            output.y += newY;
            return output;
        }

        public static Vector3 ModifyZ(this Vector3 input, float newZ)
        {
            var output = input;
            output.z += newZ;
            return output;
        }

        public static float DistanceTo(this Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }

        public static float DistanceTo(this Vector3 a, Component b)
        {
            return Vector3.Distance(a, b.transform.position);
        }

        public static float DistanceTo(this float a, float b)
        {
            return Mathf.Max(a, b) - Mathf.Min(a, b);
        }

        public static int DistanceTo(this int a, int b)
        {
            return Mathf.Max(a, b) - Mathf.Min(a, b);
        }

        public static float DistanceTo(this Transform a, Transform b)
        {
            return Vector3.Distance(a.position, b.position);
        }

        public static float DistanceTo(this GameObject a, GameObject b)
        {
            return Vector3.Distance(a.transform.position, b.transform.position);
        }

        public static float DistanceTo(this Component a, Component b)
        {
            return Vector3.Distance(a.transform.position, b.transform.position);
        }

        public static float DistanceTo(this Component a, Vector3 b)
        {
            return Vector3.Distance(a.transform.position, b);
        }

        
        public static bool ContainsIgnoreCase(this string source, string toCheck, StringComparison comp = StringComparison.InvariantCultureIgnoreCase)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        
        public static string SplitPascalCase(this string input)
        {
            switch (input)
            {
                case "":
                case null:
                    return input;
                default:
                    var stringBuilder = new StringBuilder(input.Length);
                    if (char.IsLetter(input[0]))
                        stringBuilder.Append(char.ToUpper(input[0]));
                    else
                        stringBuilder.Append(input[0]);
                    for (var index = 1; index < input.Length; ++index)
                    {
                        var c = input[index];
                        if (char.IsUpper(c) && !char.IsUpper(input[index - 1]))
                            stringBuilder.Append(' ');
                        stringBuilder.Append(c);
                    }
                    return stringBuilder.ToString();
            }
        }

        
        public static string GetScenePath(this GameObject obj)
        {
            var path = "/" + obj.name;
            
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            
            return path;
        }

        
        public static void ForceRebuildLayout(this GameObject gameObject)
        {
            foreach (var group in gameObject.GetComponentsInChildren<LayoutGroup>())
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)group.transform);
            }
        }

        
        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }

        public static bool HasIndex<T>(this IEnumerable<T> list, int index)
        {
            return list != null && index >= 0 && index < list.Count();
        }
        
        public static bool TryFindValue<T>(this IList<T> list, Predicate<T> match, out T result)
        {
            foreach (var item in list)
            {
                if (match(item))
                {
                    result = item;
                    return true;
                }
            }

            result = default;
            return false;
        }

        private static readonly char[] Separators =
        {
            '_', ' ', '(', ')', '[', ']', '{', '}', '<', '>', ',', '.', ';', ':', '/', '\\', '\'', '\"', '`', '~', 
            '!', '@', '#', '$', '%', '^', '&', '*', '-', '+', '=', '?', '|', '\t', '\n', '\r', '\f', '\v', '\0', 
            '\a', '\b',
        };

        public static string TitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var result = new StringBuilder();
            var newWord = true;
            var previousUpper = false;

            foreach (var c in input)
            {
                if (c == '_' || (char.IsUpper(c) && !previousUpper))
                {
                    if (!newWord)
                    {
                        result.Append(' ');
                    }

                    if (c != '_')
                    {
                        result.Append(newWord ? char.ToUpper(c) : char.ToLower(c));
                        previousUpper = true;
                    }
                    newWord = true;
                }
                else
                {
                    result.Append(newWord ? char.ToUpper(c) : c);
                    newWord = false;
                    previousUpper = char.IsUpper(c);
                }
            }
            
            var ti = CultureInfo.CurrentCulture.TextInfo;
            return ti.ToTitleCase(result.ToString());
        }

        public static string CamelCase(this string format)
        {
            if (string.IsNullOrEmpty(format)) return "";
            var strings = format.Split(Separators);
            var result = "";

            Array.ForEach(strings, str =>
            {
                if (str.Length <= 0) return;
                result += str.Substring(0, 1).ToUpper() + str.Substring(1);
            });

            return result;
        }


        public static T GetComponentAlways<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.TryGetComponent<T>(out var component) ? component : gameObject.AddComponent<T>();
        }

        public static bool IsNonEmpty(this string s)
        {
            if (s != null)
            {
                return !string.IsNullOrEmpty(s);
            }

            return false;
        }

        public static bool IsEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static T FindImplementation<T>(params Type[] excludes)
        {
            var baseType = typeof(T);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (excludes.Length > 0 && excludes.Any(excludeType => type == excludeType)) continue;
                    
                    if (baseType.IsAssignableFrom(type) && type != baseType)
                    {
                        return (T) Activator.CreateInstance(type);
                    }
                }
            }

            return default;
        }

        public static bool IsImplementationExists<T>(params Type[] excludes)
        {
            var baseType = typeof(T);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (excludes.Length > 0 && excludes.Any(excludeType => type == excludeType)) continue;
                    
                    if (baseType.IsAssignableFrom(type) && type != baseType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static float _updateFinishTime;

        public static void StartEditorUpdateLoop(float duration)
        {
#if UNITY_EDITOR
            if (Application.isPlaying || duration <= 0f) return;
            
            if (_updateFinishTime > Time.realtimeSinceStartup)
            {
                _updateFinishTime = Mathf.Max(_updateFinishTime, Time.realtimeSinceStartup + duration);    
                return;
            }
            
            _updateFinishTime = Time.realtimeSinceStartup + duration;

            UnityEditor.EditorApplication.CallbackFunction callback = null;
            callback = () =>
            {
                if (Time.realtimeSinceStartup > _updateFinishTime)
                {
                    UnityEditor.EditorApplication.update -= callback;
                    Debug.Log("Editor update loop finished");
                    return;
                }

                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
            };
            
            UnityEditor.EditorApplication.update += callback;
            
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
#endif
        }
    }
}
