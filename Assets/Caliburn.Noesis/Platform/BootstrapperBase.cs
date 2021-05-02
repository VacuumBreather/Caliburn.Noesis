namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    #endregion

    /// <summary>
    ///     Inherit from this class in order to customize the configuration of the framework.
    /// </summary>
    public abstract class BootstrapperBase : MonoBehaviour
    {
        #region Constants and Fields

        private bool isInitialized;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Initialize the framework.
        /// </summary>
        public void Initialize()
        {
            if (this.isInitialized)
            {
                return;
            }

            this.isInitialized = true;

            var baseExtractTypes = AssemblySourceCache.ExtractTypes;

            AssemblySourceCache.ExtractTypes = assembly =>
                {
                    var baseTypes = baseExtractTypes(assembly);
                    /*var elementTypes = assembly.GetExportedTypes()
                        .Where(t => typeof(UIElement).IsAssignableFrom(t));*/

                    return baseTypes /*.Union(elementTypes)*/;
                };

            AssemblySource.Instance.Refresh();

            StartRuntime();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="instance">The instance to perform injection on.</param>
        protected virtual void BuildUp(object instance)
        {
        }

        /// <summary>
        ///     Override to configure the framework and setup your IoC container.
        /// </summary>
        protected virtual void Configure()
        {
        }

        /// <summary>
        ///     Override this to provide an IoC specific implementation
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <returns>The located services.</returns>
        protected virtual IEnumerable<object> GetAllInstances(Type service)
        {
            return new[]
                       {
                           Activator.CreateInstance(service)
                       };
        }

        /// <summary>
        ///     Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <returns>The located service.</returns>
        protected virtual object GetInstance(Type service)
        {
            // if (service == typeof(IWindowManager))
            //     service = typeof(WindowManager);

            return Activator.CreateInstance(service);
        }

        /// <summary>
        ///     Override this to add custom behavior on exit.
        /// </summary>
        protected virtual void OnExit()
        {
        }

        /// <summary>
        ///     Override this to add custom behavior to execute after the application starts.
        /// </summary>
        protected virtual void OnStartup()
        {
        }

        /// <summary>
        ///     Override this to add custom behavior for unhandled exceptions.
        /// </summary>
        protected virtual void OnUnhandledException()
        {
        }

        /// <summary>
        ///     Provides an opportunity to hook into the application object.
        /// </summary>
        protected virtual void PrepareApplication()
        {
            // Application.Startup += OnStartup;
            // Application.DispatcherUnhandledException += OnUnhandledException;
            // Application.Exit += OnExit;
        }

        /// <summary>
        ///     Override to tell the framework where to find assemblies to inspect for views, etc.
        /// </summary>
        /// <returns>A list of assemblies to inspect.</returns>
        protected virtual IEnumerable<Assembly> SelectAssemblies()
        {
            return new[]
                       {
                           GetType().Assembly
                       };
        }

        /// <summary>
        ///     Called by the bootstrapper's constructor at runtime to start the framework.
        /// </summary>
        protected virtual void StartRuntime()
        {
            AssemblySourceCache.Install();
            AssemblySource.Instance.AddRange(SelectAssemblies());

            PrepareApplication();

            Configure();
        }

        /// <summary>
        ///     Locates the view model, locates the associate view, binds them and shows it as the root view.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <param name="settings">The optional window settings.</param>
        protected void DisplayRootViewFor(Type viewModelType, IDictionary<string, object> settings = null)
        {
            // var windowManager = IoC.Get<IWindowManager>();
            // windowManager.ShowWindow(IoC.GetInstance(viewModelType, null), null, settings);
        }

        /// <summary>
        ///     Locates the view model, locates the associate view, binds them and shows it as the root view.
        /// </summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="settings">The optional window settings.</param>
        protected void DisplayRootViewFor<TViewModel>(IDictionary<string, object> settings = null)
        {
            DisplayRootViewFor(typeof(TViewModel), settings);
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            OnStartup();
        }

        private void OnDestroy()
        {
            OnExit();
        }

        #endregion
    }
}