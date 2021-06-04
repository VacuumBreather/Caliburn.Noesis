// namespace Caliburn.Noesis.Extensions
// {
//     using System;
//
//     /// <summary>Provides extension methods for the <see cref="Container" /> type.</summary>
//     public static class ContainerExtensions
//     {
//         #region Public Methods
//
//         /// <summary>Registers an implementation type for the specified interface.</summary>
//         /// <typeparam name="T">The type of the interface to register.</typeparam>
//         /// <param name="container">This container instance.</param>
//         /// <param name="type">An implementing type.</param>
//         /// <returns>The type registration.</returns>
//         public static Container.IRegisteredType Register<T>(this Container container, Type type)
//         {
//             return container.Register(typeof(T), type);
//         }
//
//         /// <summary>Registers an implementation type for the specified interface.</summary>
//         /// <typeparam name="TInterface">The interface type to register.</typeparam>
//         /// <typeparam name="TImplementation">An implementing type.</typeparam>
//         /// <param name="container">This container instance.</param>
//         /// <returns>The type registration.</returns>
//         public static Container.IRegisteredType Register<TInterface, TImplementation>(
//             this Container container)
//             where TImplementation : TInterface
//         {
//             return container.Register(typeof(TInterface), typeof(TImplementation));
//         }
//
//         /// <summary>Registers a factory function which will be called to resolve the specified interface.</summary>
//         /// <typeparam name="T">The interface type to register.</typeparam>
//         /// <param name="container">This container instance.</param>
//         /// <param name="factory">A factory method.</param>
//         /// <returns>The type registration.</returns>
//         public static Container.IRegisteredType Register<T>(this Container container,
//                                                             Func<T> factory)
//         {
//             return container.Register(typeof(T), () => factory());
//         }
//
//         /// <summary>Registers a type.</summary>
//         /// <typeparam name="T">The interface type to register.</typeparam>
//         /// <param name="container">This container instance.</param>
//         /// <returns>The type registration.</returns>
//         public static Container.IRegisteredType Register<T>(this Container container)
//         {
//             return container.Register(typeof(T), typeof(T));
//         }
//
//         /// <summary>Returns an implementation of the specified interface.</summary>
//         /// <typeparam name="T">The interface type to resolve.</typeparam>
//         /// <param name="scope">This scope instance.</param>
//         /// <returns>An object implementing the interface.</returns>
//         public static T Resolve<T>(this Container.IScope scope)
//         {
//             return (T)scope.GetService(typeof(T));
//         }
//
//         #endregion
//     }
// }

