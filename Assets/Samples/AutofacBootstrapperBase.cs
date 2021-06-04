namespace Caliburn.Noesis.Samples
{
    using System;
    using System.Collections.Generic;
    using Autofac;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     Inherit from this class to configure and run the framework using an <see cref="Autofac" />
    ///     DI container.
    /// </summary>
    [PublicAPI]
    public abstract class AutofacBootstrapperBase<T> : BootstrapperBase<T>
        where T : Screen
    {
        #region Protected Properties

        /// <summary>Gets the DI container.</summary>
        protected IContainer Container { get; private set; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override object GetService(Type serviceType)
        {
            try
            {
                return Container.Resolve(serviceType);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override void ConfigureIoCContainer(IEnumerable<Type> viewModelTypes, IEnumerable<Type> viewTypes)
        {
            var builder = new ContainerBuilder();

#if UNITY_5_5_OR_NEWER
            builder.Register(_ => new DebugLoggerFactory(this, LogLevel.Information))
#else
            builder.Register(_ => new DebugLoggerFactory(LogLevel.Information))
#endif
                   .As<ILoggerFactory>()
                   .SingleInstance();

            builder.RegisterInstance(this).As<IServiceProvider>().SingleInstance();
            builder.RegisterType<ShellViewModel>().As<IWindowManager>().SingleInstance();

            foreach (var type in viewTypes)
            {
                builder.RegisterType(type).AsSelf().InstancePerDependency();
            }

            foreach (var type in viewModelTypes)
            {
                builder.RegisterType(type).AsSelf().AsImplementedInterfaces().InstancePerDependency();
            }

            Container = builder.Build();
        }

        /// <inheritdoc />
        protected override UniTask OnShutdown()
        {
            Container.Dispose();

            return base.OnShutdown();
        }

        #endregion
    }
}