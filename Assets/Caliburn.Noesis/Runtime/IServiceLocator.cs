using System;
using System.Collections.Generic;

namespace Caliburn.Noesis
{
    public interface IServiceLocator
    {
        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <param name="key">The key to locate.</param>
        /// <returns>The located service.</returns>
        object GetInstance(Type service, string key = null);

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <typeparam name="T">The service to locate.</typeparam>
        /// <returns>The located service.</returns>
        T GetInstance<T>(string key = null);

        /// <summary>
        /// Override this to provide an IoC specific implementation
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <returns>The located services.</returns>
        IEnumerable<object> GetAllInstances(Type service);

        /// <summary>
        /// Override this to provide an IoC specific implementation
        /// </summary>
        /// <typeparam name="T">The service to locate.</typeparam>
        /// <returns>The located services.</returns>
        IEnumerable<T> GetAllInstances<T>();

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="instance">The instance to perform injection on.</param>
        void BuildUp(object instance);
    }
}
