using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GalaxyMap.DependencyInjection
{
    /*
     * Class Originally created to decouple MapSelectionEffectsManager from GalaxyMap
     * (EffectManager requires IGalaxyMap for events etc.).
     * Didn't want to use Zenject or a larger framework cause of unneeded complexity.
     * Another solution considered was using an event bus, may implement in the future. 
     */
    
    // TODO: Tests
    /// <summary>
    /// A simple Dependency Injection Container to decouple classes in WorldMap. 
    /// </summary>
    public static class MapDependencyContainer
    {
        private static readonly Dictionary<Type, object> _registeredTypes = new Dictionary<Type, object>();

        public static void RegisterSingleton<T>(T instance) where T : class
        {
            var type = typeof(T);
            if (instance == null)
            {
                Debug.LogError($"Can't register instance of type {type.Name} to null!");
                return;
            }
            
            if (_registeredTypes.ContainsKey(type))
            {
                if (ReferenceEquals(_registeredTypes[type], instance))
                {
                    Debug.LogWarning($"Setting type {type.Name} to same instance");
                }
                else
                {
                    Debug.LogError($"Trying to set different singleton to type {type.Name}! This is not allowed. " +
                                   $"\nCall {nameof(UnregisterSingleton)} beforehand if you'd like to register another singleton");
                }
                return;
            }

            _registeredTypes[type] = instance;
        }

        public static void UnregisterSingleton<T>(T instance) where T : class
        {
            var type = typeof(T);
            if (!_registeredTypes.ContainsKey(type))
            {
                Debug.LogWarning($"Trying to unregister type {type.Name} but no instance was registered!");
                return;
            }

            var registeredInstance = _registeredTypes[type];
            if (!ReferenceEquals(instance, registeredInstance))
            {
                Debug.LogError($"Trying to unregister singleton of type {type.Name}, but the instance given was not the one registered!");
                return;
            }

            _registeredTypes.Remove(type);
        }

        /// <summary>
        /// Resolve the Type given if it was registered, else return null
        /// </summary>
        public static T ResolveImmediate<T>() where T : class
        {
            var type = typeof(T);

            if (!_registeredTypes.ContainsKey(type))
            {
                Debug.LogError($"Type {type.Name} has no registered instance!");
                return null;
            }

            return (T) _registeredTypes[type];
        }

        /// <summary>
        /// Wait until the given type has been registered, and return it's instance <br />
        /// If timed out, return null 
        /// </summary>
        /// <param name="timeoutSecs">The maximum time to await resolution</param>
        /// <param name="pollDeltaMs"> Milliseconds interval between checking whether the dependency exists</param>
        public static async Task<T> ResolveAsync<T>(float timeoutSecs, float pollDeltaMs = 50f) where T : class
        {
            var type = typeof(T);
            
            var startTime = Time.unscaledTime;
            var timeoutTime = startTime + timeoutSecs;
            
            while (Time.unscaledTime < timeoutTime)
            {
                if (_registeredTypes.ContainsKey(type))
                {
                    return (T) _registeredTypes[type];
                }

                await Task.Delay(TimeSpan.FromMilliseconds(pollDeltaMs));
            }
            
            if (!_registeredTypes.ContainsKey(type))
            {
                Debug.LogError($"{nameof(ResolveAsync)} Timed Out when trying to resolve type {type.Name}");
                return null;
            }
            
            return (T) _registeredTypes[type];
        }
    }
}