using System;
#if UNITY_5_5_OR_NEWER
using global::Noesis;
#else
using System.Windows;
#endif

namespace Caliburn.Noesis
{
    /// <summary>
    /// Class that abstracts the differences in creating a DepedencyProperty / BindableProperty on the different platforms.
    /// </summary>
    public static class DependencyPropertyHelper
    {
        /// <summary>
        /// Register an attached dependency / bindable property
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="propertyType">The property type</param>
        /// <param name="ownerType">The owner type</param>
        /// <param name="defaultValue">The default value</param>
        /// <param name="propertyChangedCallback">Callback to executed on property changed</param>
        /// <returns>The registred attached dependecy property</returns>
        public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType, object defaultValue = null, PropertyChangedCallback propertyChangedCallback = null) {
            return DependencyProperty.RegisterAttached(name, propertyType, ownerType, new PropertyMetadata(defaultValue, propertyChangedCallback));
        }

        /// <summary>
        /// Register a dependency / bindable property
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="propertyType">The property type</param>
        /// <param name="ownerType">The owner type</param>
        /// <param name="defaultValue">The default value</param>
        /// <param name="propertyChangedCallback">Callback to executed on property changed</param>
        /// <returns>The registred dependecy property</returns>
        public static DependencyProperty Register(string name, Type propertyType, Type ownerType, object defaultValue = null, PropertyChangedCallback propertyChangedCallback = null)
        {
            return DependencyProperty.Register(name, propertyType, ownerType, new PropertyMetadata(defaultValue, propertyChangedCallback));
        }
    }
}
