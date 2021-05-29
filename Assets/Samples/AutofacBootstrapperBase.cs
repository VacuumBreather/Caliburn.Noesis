namespace Caliburn.Noesis.Samples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            return Container.Resolve(serviceType);
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override void ConfigureIoCContainer(IEnumerable<Type> viewModelTypes)
        {
            var builder = new ContainerBuilder();

#if UNITY_5_5_OR_NEWER
            builder.Register(_ => new DebugLoggerFactory(this, LogLevel.Trace))
#else
            builder.Register(_ => new DebugLoggerFactory(LogLevel.Trace))
#endif
                   .As<ILoggerFactory>().SingleInstance();

            builder.ComponentRegistryBuilder.Registered += (_, args) =>
                {
                    args.ComponentRegistration.PipelineBuilding += (_, pipeline) =>
                        {
                            pipeline.Use(attachLoggerMiddleware);
                        };
                };

            builder.RegisterInstance(this).As<IServiceProvider>().SingleInstance();
            builder.RegisterType<ShellViewModel>().As<IWindowManager>().SingleInstance();
            builder.RegisterType<ViewLocator>().AsSelf().SingleInstance();
            builder.RegisterType<NameTransformer>().AsSelf().SingleInstance();

            foreach (var type in viewModelTypes.Where(type => type != typeof(ShellViewModel)))
            {
                builder.RegisterType(type)
                       .AsSelf()
                       .AsImplementedInterfaces()
                       .InstancePerDependency();
            }

            Container = builder.Build();
            
            LogManager.SetLoggerFactory(Container.Resolve<ILoggerFactory>());
            Logger = Container.Resolve<ILoggerFactory>().CreateLogger(GetType().Name);
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
                                              .CreateLogger(hasLogger.GetType().Name);
                }
            }
        }

        #endregion
    }
}