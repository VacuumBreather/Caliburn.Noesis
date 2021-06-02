namespace Caliburn.Noesis
{
    using System;
    using System.Linq;
    using Extensions;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
#endif

    /// <summary>Hosts attached properties related to view-models.</summary>
    [PublicAPI]
    public static class View
    {
        #region Constants and Fields

        private static readonly ContentPropertyAttribute DefaultContentProperty =
            new ContentPropertyAttribute(nameof(ContentControl.Content));

        /// <summary>A dependency property for attaching a model to the UI.</summary>
        public static DependencyProperty ModelProperty = DependencyProperty.RegisterAttached(
            PropertyNameHelper.GetName(nameof(ModelProperty)),
            typeof(object),
            typeof(View),
            new PropertyMetadata(default, OnModelChanged));

        private static ILogger logger;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the <see cref="ILogger" /> for this type.</summary>
        public static ILogger Logger
        {
            get => logger ??= LogManager.FrameworkLogger;
            set => logger = value;
        }

        #endregion

        #region Public Methods

        /// <summary>Gets the model.</summary>
        /// <param name="d">The element the model is attached to.</param>
        /// <returns>The model.</returns>
        public static object GetModel(DependencyObject d)
        {
            return d.GetValue(ModelProperty);
        }

        /// <summary>Sets the model.</summary>
        /// <param name="d">The element to attach the model to.</param>
        /// <param name="value">The model.</param>
        public static void SetModel(DependencyObject d, object value)
        {
            d.SetValue(ModelProperty, value);
        }

        #endregion

        #region Private Methods

        private static bool CanCreateView(object viewModel,
                                          DependencyObject targetLocation,
                                          out IServiceProvider serviceProvider,
                                          out AssemblySource assemblySource,
                                          out ViewLocator viewLocator)
        {
            using var _ = Logger.GetMethodTracer(
                viewModel,
                targetLocation,
                null,
                null,
                null);

            var frameworkElement = targetLocation.FindVisualAncestor<FrameworkElement>();
            serviceProvider =
                frameworkElement?.TryFindResource(nameof(IServiceProvider)) as IServiceProvider;
            assemblySource =
                frameworkElement?.TryFindResource(nameof(AssemblySource)) as AssemblySource;
            viewLocator = frameworkElement?.TryFindResource(nameof(ViewLocator)) as ViewLocator;

            return (viewModel != null) &&
                   (serviceProvider != null) &&
                   (assemblySource != null) &&
                   (viewLocator != null);
        }

        private static void OnModelChanged(DependencyObject targetLocation,
                                           DependencyPropertyChangedEventArgs args)
        {
            using var _ = Logger.GetMethodTracer(targetLocation, args);

            if (args.OldValue == args.NewValue)
            {
                return;
            }

            var viewModel = args.NewValue;

            if (viewModel is null)
            {
                SetContentProperty(targetLocation, null);

                return;
            }

            if (viewModel is IViewAware { View: UIElement cachedView })
            {
                Logger.LogDebug("Using cached view for {ViewModel}", viewModel);

                SetContentProperty(targetLocation, cachedView);

                return;
            }

            if (CanCreateView(
                viewModel,
                targetLocation,
                out var serviceProvider,
                out var assemblySource,
                out var viewLocator))
            {
                var view = viewLocator.LocateForModel(viewModel, serviceProvider, assemblySource);

                if (viewModel is IViewAware viewAware && !ReferenceEquals(viewAware.View, view))
                {
                    Logger.LogDebug("Attaching {@View} to {ViewModel}", view, viewAware);

                    viewAware.AttachView(view);
                }

                if (!SetContentProperty(targetLocation, view))
                {
                    Logger.LogWarning(
                        "{SetContentProperty}() failed for {ViewLocator}.{LocateForModel}(), falling back to {LocateForModelType}()",
                        nameof(SetContentProperty),
                        nameof(ViewLocator),
                        nameof(ViewLocator.LocateForModel),
                        nameof(ViewLocator.LocateForModelType));

                    view = viewLocator.LocateForModelType(
                        viewModel.GetType(),
                        serviceProvider,
                        assemblySource);

                    SetContentProperty(targetLocation, view);
                }
            }
            else
            {
                SetContentProperty(targetLocation, null);
            }
        }

        private static bool SetContentProperty(object targetLocation, object view)
        {
            if (view is FrameworkElement { Parent: { } } frameworkElement)
            {
                SetContentPropertyCore(frameworkElement.Parent, null);
            }

            return SetContentPropertyCore(targetLocation, view);
        }

        private static bool SetContentPropertyCore(object targetLocation, object view)
        {
            try
            {
                var type = targetLocation.GetType();
                var contentProperty =
                    type.GetCustomAttributes(typeof(ContentPropertyAttribute), true)
                        .OfType<ContentPropertyAttribute>()
                        .FirstOrDefault() ??
                    DefaultContentProperty;

                var propertyInfo =
                    type.GetProperty(contentProperty?.Name ?? DefaultContentProperty.Name);

                if (propertyInfo == null)
                {
                    return false;
                }

                propertyInfo.SetValue(targetLocation, view, null);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}