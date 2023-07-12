using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace IV.Core.Pools
{
    [Serializable]
    public class GameObjectPool<T> where T : Behaviour
    {
        [SerializeField] private T prefab;
        [SerializeField] private Transform parent;
        [SerializeField] [Min(0)] private int initialSize;

        [SerializeField] [HideInInspector] // better to make readonly and show in inspector
        private bool isInitialized;

        [SerializeField] [HideInInspector]
        private List<T> pool = new();

#if UNITY_EDITOR
        public void Initialize()
        {
            Assert.IsNotNull(parent, "Parent transform is not set");
            Assert.IsNotNull(prefab, "Prefab is not set");

            pool.Clear();
            pool.Capacity = initialSize;

            foreach (Transform child in parent)
            {
                if (child.gameObject.TryGetComponent<T>(out var component))
                {
                    pool.Add(component);
                }
            }

            for (var i = pool.Count; i < initialSize; i++)
            {
                CreateNewObject();
            }

            while (initialSize < pool.Count)
            {
                DestroyLastObject();
            }

            isInitialized = true;
        }
#endif

        private T CreateNewObject()
        {
            var instance = Object.Instantiate(prefab, parent);
            instance.gameObject.SetActive(false);
            pool.Add(instance);

            return instance;
        }

        private void DestroyLastObject()
        {
            var instance = pool[^1];
            pool.RemoveAt(pool.Count - 1);
            Object.Destroy(instance.gameObject);
        }

        public T Get()
        {
            T instance = null;
            for (var i = 0; i < pool.Count; i++)
            {
                if (!pool[i].gameObject.activeSelf)
                {
                    instance = pool[i];
                    break;
                }
            }

            if (instance == null)
            {
                instance = CreateNewObject();
            }

            instance.gameObject.SetActive(true);

            return instance;
        }

        public void Return(T instance)
        {
            instance.gameObject.SetActive(false);
            pool.Add(instance);
        }

        public void Recycle()
        {
            for (var i = 0; i < pool.Count; i++)
            {
                var go = pool[i].gameObject;
                if (go.activeSelf)
                {
                    go.SetActive(false);
                }
            }
        }
    }
}