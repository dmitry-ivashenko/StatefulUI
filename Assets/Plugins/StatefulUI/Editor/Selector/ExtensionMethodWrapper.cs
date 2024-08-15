using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using StatefulUI.Runtime.Core;

namespace StatefulUI.Editor.Selector
{
    public class ExtensionMethodWrapper
    {
        private readonly object[] _parameters;
        private readonly MethodInfo _methodInfo;
        
        public ExtensionMethodWrapper(object target, StatefulComponent statefulComponent,  string name)
        {
            _parameters = new []{target, statefulComponent};
            _methodInfo = GetExtensionMethods(target.GetType(), name);
        }

        public void Invoke()
        {
            _methodInfo?.Invoke(null, _parameters);
        }

        private MethodInfo GetExtensionMethods(Type extendedType, string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSealed && !type.IsGenericType && !type.IsNested)
                    {
                        var methods = type.GetMethods(BindingFlags.Static
                                                      | BindingFlags.Public | BindingFlags.NonPublic);
                        
                        foreach (var method in methods)
                        {
                            if (method.IsDefined(typeof(ExtensionAttribute), false)
                                && method.GetParameters()[0].ParameterType == extendedType
                                && method.Name == name)
                            {
                                return method;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
