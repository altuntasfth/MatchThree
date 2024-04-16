using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace _Game.Scripts.Utilities
{
    public class ObjectPool<T> : UnityEngine.Pool.ObjectPool<T> where T : class
    {
        protected readonly HashSet<T> activeObjects = new();

        public ObjectPool(Func<T> createFunc, Action<T> actionOnGet = null, Action<T> actionOnRelease = null,
            Action<T> actionOnReset = null, bool collectionCheck = false, int defaultCapacity = 10, int maxSize = 100)
            : base(createFunc, actionOnGet, actionOnRelease, actionOnReset, collectionCheck, defaultCapacity, maxSize)
        {
        }

        public new T Get()
        {
            var obj = base.Get();
            activeObjects.Add(obj);
            return obj;
        }

        public new void Release(T obj)
        {
            base.Release(obj);
            activeObjects.Remove(obj);
        }

        public void ReleaseAllActive()
        {
            foreach (var obj in activeObjects) base.Release(obj);
            activeObjects.Clear();
        }
    }
}