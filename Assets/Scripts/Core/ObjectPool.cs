using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter.Core
{
    public class ObjectPool<T> where T : Component
    {
        private readonly Stack<T> _stack = new();
        private readonly T _prefab;
        private readonly Transform _parent;

        public ObjectPool(T prefab, int preload, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            for (int i = 0; i < preload; i++)
            {
                var inst = GameObject.Instantiate(_prefab, _parent);
                inst.gameObject.SetActive(false);
                _stack.Push(inst);
            }
        }

        public T Get()
        {
            if (_stack.Count > 0)
            {
                var obj = _stack.Pop();
                obj.gameObject.SetActive(true);
                return obj;
            }
            return GameObject.Instantiate(_prefab, _parent);
        }

        public void Release(T obj)
        {
            obj.gameObject.SetActive(false);
            _stack.Push(obj);
        }
    }
}
