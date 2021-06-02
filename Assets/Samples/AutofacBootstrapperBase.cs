namespace Caliburn.Noesis.Samples
{
    using System;
    using System.Collections.Generic;
    using Autofac;
    using Autofac.Core.Resolving.Pipeline;
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
        #region Constants and Fields

        private static IResolveMiddleware attachLoggerMiddleware = new AttachLoggerMiddleware();

        #endregion

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
        protected override void ConfigureIoCContainer(IEnumerable<Type> viewModelTypes,
                                                      IEnumerable<Type> viewTypes)
        {
            var builder = new ContainerBuilder();

#if UNITY_5_5_OR_NEWER
            builder.Register(_ => new DebugLoggerFactory(this, LogLevel.Information))
#else
            builder.Register(_ => new DebugLoggerFactory(LogLevel.Information))
#endif
                   .As<ILoggerFactory>()
                   .SingleInstance();

            builder.ComponentRegistryBuilder.Registered += (_, args) =>
                {
                    args.ComponentRegistration.PipelineBuilding += (_, pipeline) =>
                        {
                            pipeline.Use(attachLoggerMiddleware);
                        };
                };

            builder.RegisterInstance(this).As<IServiceProvider>().SingleInstance();
            builder.RegisterType<ShellViewModel>().As<IWindowManager>().SingleInstance();

            foreach (var type in viewTypes)
            {
                builder.RegisterType(type).AsSelf().InstancePerDependency();
            }

            foreach (var type in viewModelTypes)
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

        #region Nested Types

        private class AttachLoggerMiddleware : IResolveMiddleware
        {
            public PipelinePhase Phase => PipelinePhase.RegistrationPipelineStart;

            public void Execute(ResolveRequestContext context, Action<ResolveRequestContext> next)
            {
                // Call the next middleware in the pipeline.
                next(context);

                if (context.Instance is IHaveLogger hasLogger)
                {
                    // Resolve and attach the logger.
                    hasLogger.Logger = context.Resolve<ILoggerFactory>()
                                              .CreateLogger(LogManager.FrameworkCategoryName);
                }
            }
        }

        #endregion
    }
}