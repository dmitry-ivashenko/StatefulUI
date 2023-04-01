using System;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.States;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StatefulUI.Runtime.Core
{
    public class StatefulUiManager
    {
        private static StatefulUiManager _instance;

        public static StatefulUiManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StatefulUiManager();
                }

                return _instance;
            }
        }

        public Type AnimatorReferenceType { get; }
        public Type ButtonReferenceType { get; }
        public Type ContainerReferenceType { get; }
        public Type DropdownReferenceType { get; }
        public Type ImageReferenceType { get; }
        public Type InnerComponentReferenceType { get; }
        public Type ObjectReferenceType { get; }
        public Type SliderReferenceType { get; }
        public Type StateReferenceType { get; }
        public Type TextInputReferenceType { get; }
        public Type TextReferenceType { get; }
        public Type ToggleReferenceType { get; }
        public Type VideoPlayerReferenceType { get; }
        
        public Action<StatefulComponent> ValidateStatefulComponent;

        public StatefulUiManager()
        {
            AnimatorReferenceType = typeof(AnimatorReference);
            ButtonReferenceType = typeof(ButtonReference);
            ContainerReferenceType = typeof(ContainerReference);
            DropdownReferenceType = typeof(DropdownReference);
            ImageReferenceType = typeof(ImageReference);
            InnerComponentReferenceType = typeof(InnerComponentReference);
            ObjectReferenceType = typeof(ObjectReference);
            SliderReferenceType = typeof(SliderReference);
            StateReferenceType = typeof(StateReference);
            TextInputReferenceType = typeof(TextInputReference);
            TextReferenceType = typeof(TextReference);
            ToggleReferenceType = typeof(ToggleReference);
            VideoPlayerReferenceType = typeof(VideoPlayerReference);
        }

        public GameObject InstantiatePrefab(GameObject prefab)
        {
            var instantiateMethod = CustomInstantiateMethod ?? DefaultInstantiateMethod;
            return instantiateMethod?.Invoke(prefab);
        }

        private static GameObject DefaultInstantiateMethod(GameObject arg)
        {
            return Object.Instantiate(arg);
        }

        public Func<GameObject, GameObject> CustomInstantiateMethod;
    }
}