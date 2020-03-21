using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


namespace GalaxyMap.Utils
{
    // TODO: unit tests?
    public class PrefabPool<T> where T : Component
    {
        private const int InitialPoolSize = 3;
        
        private List<T> _active = new List<T>();
        private Queue<T> _inactive = new Queue<T>();
        private T _prefab;
        private Transform _parent;
    
        private Action<T> _onInstantiate;
        private Action<T> _onBecomeActive;
        private Action<T> _onBecomeInactive;
        
        // Should instance.gameObject SetActive be bound to when it is active/inactive in pool?
        private bool _setActiveByState;
    
        private int Size => _active.Count + _inactive.Count;

        /// <summary>
        /// Create a new prefabPool
        /// </summary>
        /// <param name="prefab">The prefab that will be used for instantiations</param>
        /// <param name="name">The name of the GameObject that instances will be placed under</param>
        /// <param name="setActiveByState">If true - instantiated gameObjects will have SetActive called automatically when they're pulled from the pool or recycled</param>
        public PrefabPool(T prefab, string name, bool setActiveByState = true)
        {
            _prefab = prefab;
            _parent = new GameObject(name).transform;
            _setActiveByState = setActiveByState;
            
            Fill(InitialPoolSize); // Minimum
        }
    
        public T Get()
        {
            if (_inactive.Count == 0)
            {
                Fill(Size); // Double size
            }
    
            return GetInternal();
        }
    
        public void RecycleAll()
        {
            if (_active.Count == 0) return;
            
            // Copy active items to array because recycle mutates the _active list's state
            // NOTE: we maybe could've iterated on list backwards and all would be fine, 
            // But copying seems more durable to changes in Recycle logic
            var activeCopy = _active.ToArray();
            foreach (var item in activeCopy)
            {
                Recycle(item);
            }
        }
    
        public void Recycle(T instance)
        {
            if (instance == null)
            {
                Debug.LogError("Cannot Recycle null!");
                return;
            }
            if (!_active.Contains(instance))
            {
                Debug.LogError($"Pool's active objects do not include gameobject \"{instance.name}\"");
                return;
            }
    
            SetInactive(instance);
        }
    
        public void OnBecomeActive(Action<T> onActive) => _onBecomeActive = onActive;
    
        public void OnBecomeInactive(Action<T> onActive) => _onBecomeActive = onActive;
    
        private void SetInactive(T component)
        {
            _active.Remove(component);
            
            if (_setActiveByState) component.gameObject.SetActive(false);
            _onBecomeInactive?.Invoke(component);
            
            _inactive.Enqueue(component);
        }
        
        private T GetInternal()
        {
            /* Assumes pool size was checked BEFORE this and it's safe to retrieve from _inactive */
            
            var component = _inactive.Dequeue();
            
            if (_setActiveByState) component.gameObject.SetActive(true);
            _onBecomeActive?.Invoke(component);
            
            _active.Add(component);
            return component;
        }
    
        private void Fill(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var go = CreateInstance();
                SetInactive(go);
                go.transform.SetParent(_parent, false);
            }
        }
    
        private T CreateInstance()
        {
            return Object.Instantiate(_prefab);
        }
    }
}