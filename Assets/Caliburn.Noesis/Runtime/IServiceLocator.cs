using System;
using System.Collections.Generic;

namespace Caliburn.Noesis
{
    public interface IServiceLocator
    {
        /// <summary>
        /// Returns an implementation of a given service type from the container.
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <param name="key">The key to locate.</param>
        /// <returns>The located service.</returns>
        object GetInstance(Type service, string key = null);

        /// <summary>
        /// Returns an implementation of a given service type from the container.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <typeparam name="T">The service to locate.</typeparam>
        /// <returns>The located service.</returns>
        T GetInstance<T>(string key = null);

        /// <summary>
        /// Returns all instances of a given service type from the container.
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <returns>The located services.</returns>
        IEnumerable<object> GetAllInstances(Type service);

        /// <summary>
        /// Returns all instances of a given service type from the container.
        /// </summary>
        /// <typeparam name="T">The service to locate.</typeparam>
        /// <returns>The located services.</returns>
        IEnumerable<T> GetAllInstances<T>();

        /// <summary>
        /// Injects dependencies into an existing instance.
        /// </summary>
        /// <param name="instance">The instance to perform injection on.</param>
        void BuildUp(object instance);
    }
}
