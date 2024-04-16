using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Game.Scripts.Utilities
{
    public static class Pool
    {
        private static readonly Dictionary<Type, object> objectPools = new();

        public static ObjectPool<T> GeneratePool<T>(GameObject prefab, Transform parent, Action<T> onGet = null,
            Action<T> onRelease = null, Action<T> onReset = null, int initialSize = 10) where T : class
        {
            var pool = new ObjectPool<T>(() => CreateNewInstance<T>(prefab, parent), onGet, onRelease, onReset,
                defaultCapacity: initialSize);

            for (var i = 0; i < initialSize; i++)
            {
                var obj = CreateNewInstance<T>(prefab, parent);
                pool.Release(obj);
            }

            objectPools[typeof(T)] = pool;

            return pool;
        }

        private static T CreateNewInstance<T>(GameObject prefab, Transform parent)
        {
            var newObj = Object.Instantiate(prefab, parent);
            newObj.SetActive(false);
            return newObj.GetComponent<T>();
        }
    }
}