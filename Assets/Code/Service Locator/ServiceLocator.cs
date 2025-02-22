using UnityEngine;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace AndreaFrigerio.Framework
{
    [HideMonoScript]
    public class ServiceLocator : Singleton<ServiceLocator>
    {
        #region Fields

        [SerializeField]
        private bool m_useFixedUpdate = true;

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
        private List<UnityEngine.Object> services = new();
#endif

        #endregion

        #region Variables

        private static readonly Dictionary<Type, object> Services = new();

        #endregion

        #region Callbacks Methods

        private void Start()
        {
#if UNITY_EDITOR
            PopulateSerializedServices();
#endif
        }

        private void Update()
        {
            if (m_useFixedUpdate)
            {
                return;
            }

            OnUpdate();
        }

        private void FixedUpdate()
        {
            if (!m_useFixedUpdate)
            {
                return;
            }

            OnUpdate();
        }

        #endregion

        #region Private Methods

        private void OnUpdate()
        {
            foreach (var service in GetAllServices())
            {
                // Check if the service implements IUpdateService
                if (service is IUpdateService updateService)
                {
                    updateService.OnUpdate();
                }
            }
        }

        #endregion

        #region Registration & Unregistration

        /// <summary>
        /// Registers a service of type T in the Service Locator.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        /// <param name="service">The instance of the service to register.</param>
        public static void Register<T>(T service)
        {
            Services[typeof(T)] = service;
        }

        /// <summary>
        /// Unregisters a service of type T from the Service Locator.
        /// </summary>
        /// <typeparam name="T">The type of the service to unregister.</typeparam>
        public static void Unregister<T>()
        {
            Services.Remove(typeof(T));
        }

        #endregion

        #region Getters

        /// <summary>
        /// Returns the service of type T from the Service Locator.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <returns>The service of type T.</returns>
        public static T Get<T>()
        {
            // Check if the service is registered
            if (Services.ContainsKey(typeof(T)))
            {
                return (T)Services[typeof(T)];
            }
            else
            {
                // Service not found
                throw new KeyNotFoundException($"Service of type {typeof(T)} not found.");
            }
        }

        /// <summary>
        /// Returns all registered Services in the Service Locator.
        /// </summary>
        /// <returns>An IEnumerable containing all the Services.</returns>
        public static IEnumerable<object> GetAllServices()
        {
            // Return all registered services
            return Services.Values;
        }

        #endregion

#if UNITY_EDITOR
        #region Only in Editor

        private void PopulateSerializedServices()
        {
            services.Clear();

            foreach (var entry in Services)
            {
                services.Add(entry.Value as UnityEngine.Object);
            }
        }

        #endregion
#endif

    }
}