using System;
using System.Collections.Generic;
using UnityEngine;

namespace StatefulUI.Runtime.Core
{
    public class ContainerView : MonoBehaviour
    {
        public RectTransform RectTransform => transform as RectTransform;

        [ProjectAssetOnly] public GameObject Prefab;

        [SerializeField] private Transform _root;

        public Transform Root => _root ? _root : transform;

        public List<GameObject> Instances { get; } = new List<GameObject>();

        public event Action OnAddTestItem = delegate { };
        public event Action OnClearTestItems = delegate { };

        public void AddTestItem()
        {
            if (Application.isPlaying)
            {
                AddInstance();
            }
            else
            {
                Instances.Add(Instantiate(Prefab, Root));
            }

            OnAddTestItem?.Invoke();
        }

        public void ClearTransform()
        {
            foreach (var instance in Instances)
            {
                DestroyImmediate(instance.gameObject);
            }

            Instances.Clear();
            OnClearTestItems?.Invoke();
        }

        public void FillWithItems<TL>(IEnumerable<TL> items, Action<StatefulComponent, TL> action,
            bool keepItems = false)
        {
            if (!keepItems)
            {
                Clear();
            }

            foreach (var item in items)
            {
                var view = AddInstance().GetComponentAlways<StatefulComponent>();
                view.Localize();

                foreach (var InnerComponent in view.InnerComponents)
                {
                    InnerComponent.InnerComponent.Localize();
                }

                action(view, item);
            }
        }

        public void FillWithItems<TL>(Action<StatefulComponent, TL> action, bool keepItems = false, params TL[] items)
        {
            FillWithItems(items, action, keepItems);
        }

        public GameObject AddInstance()
        {
            var instance = Instances.Find(go => !go.activeSelf);
            if (instance == null)
            {
                instance = StatefulUiManager.Instance.InstantiatePrefab(Prefab);
                Instances.Add(instance);
            }

            instance.SetActive(true);
            var instanceTransform = instance.transform;
            instanceTransform.SetParent(Root);
            instanceTransform.localScale = Vector3.one;

            return instance;
        }

        public T AddInstance<T>()
        {
            return AddInstance().GetComponent<T>();
        }

        public StatefulComponent AddStatefulComponent()
        {
            var view = AddInstance().GetComponent<StatefulComponent>();
            view.Localize();

            foreach (var InnerComponent in view.InnerComponents)
            {
                InnerComponent.InnerComponent.Localize();
            }

            return view;
        }

        public void Clear()
        {
            Instances.RemoveAll(go => go == null);

            foreach (var instance in Instances)
            {
                instance.SetActive(false);
            }
        }

        public void Remove(GameObject go, bool withDestroy = true)
        {
            if (withDestroy) Instances.RemoveAll(obj => go == obj);
            go.SetActive(false);
        }

        public void Restore(GameObject go)
        {
            Instances.Add(go);

            go.SetActive(true);
        }
    }
}