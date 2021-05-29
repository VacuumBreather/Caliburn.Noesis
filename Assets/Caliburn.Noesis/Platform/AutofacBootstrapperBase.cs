namespace Caliburn.Noesis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary>
    ///     Inherit from this class to configure and run the framework using an <see cref="Autofac" />
    ///     DI container.
    /// </summary>
    [PublicAPI]
    public abstract class AutofacBootstrapperBase<T> : BootstrapperBase<T>
        where T : Screen
    {
        #region Private Properties

        private IContainer Container { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override object GetService(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override void ConfigureIoCContainer(IEnumerable<Type> viewModelTypes)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(this).As<IServiceProvider>().SingleInstance();
            builder.RegisterType<ShellViewModel>().As<IWindowManager>().SingleInstance();

            foreach (var type in viewModelTypes.Where(type => type != typeof(ShellViewModel)))
            {
                builder.RegisterType(type)
                       .AsSelf()
                       .AsImplementedInterfaces()
                       .InstancePerDependency();
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