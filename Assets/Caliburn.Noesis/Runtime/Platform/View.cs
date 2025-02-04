namespace Caliburn.Noesis
{
    using System;
    using System.Linq;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
    using EventHandler = global::Noesis.EventHandler;
#else
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
#endif

    /// <summary>
    /// Hosts attached properties related to view models.
    /// </summary>
    public static class View
    {
        static readonly ILog Log = LogManager.GetLog(typeof(View));
        static readonly ContentPropertyAttribute DefaultContentProperty = new ContentPropertyAttribute("Content");

        /// <summary>
        /// A dependency property which allows the framework to track whether a certain element has already been loaded in certain scenarios.
        /// </summary>
        public static readonly DependencyProperty IsLoadedProperty =
            DependencyPropertyHelper.RegisterAttached(
                "IsLoaded",
                typeof(bool),
                typeof(View),
                false
                );

        /// <summary>
        /// A dependency property which marks an element as a name scope root.
        /// </summary>
        public static readonly DependencyProperty IsScopeRootProperty =
            DependencyPropertyHelper.RegisterAttached(
                "IsScopeRoot",
                typeof(bool),
                typeof(View),
                false
                );

        /// <summary>
        /// A dependency property which allows the override of convention application behavior.
        /// </summary>
        public static readonly DependencyProperty ApplyConventionsProperty =
            DependencyPropertyHelper.RegisterAttached(
                "ApplyConventions",
                typeof(bool?),
                typeof(View)
                );

        /// <summary>
        /// A dependency property for assigning a context to a particular portion of the UI.
        /// </summary>
        public static readonly DependencyProperty ContextProperty =
            DependencyPropertyHelper.RegisterAttached(
                "Context",
                typeof(object),
                typeof(View),
                null,
                OnContextChanged
                );

        /// <summary>
        /// A dependency property for attaching a model to the UI.
        /// </summary>
        public static DependencyProperty ModelProperty =
            DependencyPropertyHelper.RegisterAttached(
                "Model",
                typeof(object),
                typeof(View),
                null,
                OnModelChanged
                );

        /// <summary>
        /// Used by the framework to indicate that this element was generated.
        /// </summary>
        public static readonly DependencyProperty IsGeneratedProperty =
            DependencyPropertyHelper.RegisterAttached(
                "IsGenerated",
                typeof(bool),
                typeof(View),
                false
                );

        /// <summary>
        /// Executes the handler immediately if the element is loaded, otherwise wires it to the Loaded event.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>true if the handler was executed immediately; false otherwise</returns>
        public static bool ExecuteOnLoad(FrameworkElement element, RoutedEventHandler handler)
        {
            return ExecuteOnLoad(element, handler, out _);
        }

        internal static bool ExecuteOnLoad(FrameworkElement element, RoutedEventHandler handler, out RoutedEventHandler attachedHandler)
        {
            if (element.IsLoaded)
            {
#if UNITY_5_5_OR_NEWER
                handler(element, new RoutedEventArgs(FrameworkElement.LoadedEvent, element));
#else
                handler(element, new RoutedEventArgs());
#endif
                attachedHandler = null;
                
                return true;
            }

            RoutedEventHandler loaded = null;
            loaded = (s, e) => {
                element.Loaded -= loaded;
                handler(s, e);
            };
            element.Loaded += loaded;

            attachedHandler = loaded;

            return false;
        }

        /// <summary>
        /// Executes the handler when the element is unloaded.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void ExecuteOnUnload(FrameworkElement element, RoutedEventHandler handler)
        {

            RoutedEventHandler unloaded = null;
            unloaded = (s, e) => {
                element.Unloaded -= unloaded;
                handler(s, e);
            };
            element.Unloaded += unloaded;
        }

        /// <summary>
        /// Executes the handler the next time the elements's LayoutUpdated event fires.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void ExecuteOnLayoutUpdated(FrameworkElement element, EventHandler handler)
        {
            EventHandler onLayoutUpdate = null;

            onLayoutUpdate = (s, e) =>
            {
                element.LayoutUpdated -= onLayoutUpdate;
                handler(element, e);
            };
            element.LayoutUpdated += onLayoutUpdate;

        }

        /// <summary>
        /// Used to retrieve the root, non-framework-created view.
        /// </summary>
        /// <remarks>In certain instances the services create UI elements.
        /// For example, if you ask the window manager to show a UserControl as a dialog, it creates a window to host the UserControl in.
        /// The WindowManager marks that element as a framework-created element so that it can determine what it created vs. what was intended by the developer.
        /// Calling GetFirstNonGeneratedView allows the framework to discover what the original element was. 
        /// </remarks>
        public static Func<object, object> GetFirstNonGeneratedView = view =>
        {
            var dependencyObject = view as DependencyObject;
            if (dependencyObject == null)
            {
                return view;
            }

            if ((bool)dependencyObject.GetValue(IsGeneratedProperty))
            {
                if (dependencyObject is ContentControl)
                {
                    return ((ContentControl)dependencyObject).Content;
                }

                var type = dependencyObject.GetType();
                var contentProperty = type.GetCustomAttributes(typeof(ContentPropertyAttribute), true)
                                          .OfType<ContentPropertyAttribute>()
                                          .FirstOrDefault() ?? DefaultContentProperty;

                return type.GetProperty(contentProperty.Name)
                    .GetValue(dependencyObject, null);
            }

            return dependencyObject;
        };

        /// <summary>
        /// Gets the convention application behavior.
        /// </summary>
        /// <param name="d">The element the property is attached to.</param>
        /// <returns>Whether or not to apply conventions.</returns>
        public static bool? GetApplyConventions(DependencyObject d)
        {
            return (bool?)d.GetValue(ApplyConventionsProperty);
        }

        /// <summary>
        /// Sets the convention application behavior.
        /// </summary>
        /// <param name="d">The element to attach the property to.</param>
        /// <param name="value">Whether or not to apply conventions.</param>
        public static void SetApplyConventions(DependencyObject d, bool? value)
        {
            d.SetValue(ApplyConventionsProperty, value);
        }

        /// <summary>
        /// Sets the model.
        /// </summary>
        /// <param name="d">The element to attach the model to.</param>
        /// <param name="value">The model.</param>
        public static void SetModel(DependencyObject d, object value)
        {
            d.SetValue(ModelProperty, value);
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="d">The element the model is attached to.</param>
        /// <returns>The model.</returns>
        public static object GetModel(DependencyObject d)
        {
            return d.GetValue(ModelProperty);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="d">The element the context is attached to.</param>
        /// <returns>The context.</returns>
        public static object GetContext(DependencyObject d)
        {
            return d.GetValue(ContextProperty);
        }

        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <param name="d">The element to attach the context to.</param>
        /// <param name="value">The context.</param>
        public static void SetContext(DependencyObject d, object value)
        {
            d.SetValue(ContextProperty, value);
        }

        static void OnModelChanged(DependencyObject targetLocation, DependencyPropertyChangedEventArgs args)
        {
            if (object.ReferenceEquals(args.OldValue, args.NewValue))
            {
                return;
            }

            if (args.NewValue != null)
            {
                var context = GetContext(targetLocation);

                ViewLocator viewLocator = targetLocation.GetViewLocator();

                var view = viewLocator.LocateForModel(args.NewValue, targetLocation, context);

                if (view is FrameworkElement viewElement)
                {
                    Log.Info("Setting DC of {0} to {1}.", viewElement, args.NewValue);
                    viewElement.DataContext = args.NewValue;
                }
                
                ViewModelBinder.Bind(args.NewValue, view, context);
                
                if (!SetContentProperty(targetLocation, view))
                {

                    Log.Warn("SetContentProperty failed for ViewLocator.LocateForModel, falling back to LocateForModelType");

                    view = viewLocator.LocateForModelType(args.NewValue.GetType(), targetLocation, context);

                    SetContentProperty(targetLocation, view);
                }
            }
            else
            {
                SetContentProperty(targetLocation, args.NewValue);
            }
        }

        static void OnContextChanged(DependencyObject targetLocation, DependencyPropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(e.OldValue, e.NewValue))
            {
                return;
            }

            var model = GetModel(targetLocation);
            if (model == null)
            {
                return;
            }

            ViewLocator viewLocator = targetLocation.GetViewLocator();

            var view = viewLocator.LocateForModel(model, targetLocation, e.NewValue);
            ViewModelBinder.Bind(model, view, e.NewValue);

            if (!SetContentProperty(targetLocation, view))
            {

                Log.Warn("SetContentProperty failed for ViewLocator.LocateForModel, falling back to LocateForModelType");

                view = viewLocator.LocateForModelType(model.GetType(), targetLocation, e.NewValue);

                SetContentProperty(targetLocation, view);
            }
        }

        static bool SetContentProperty(object targetLocation, object view)
        {
            var fe = view as FrameworkElement;
            if (fe != null && fe.Parent != null)
            {
                SetContentPropertyCore(fe.Parent, null);
            }

            return SetContentPropertyCore(targetLocation, view);
        }

        static bool SetContentPropertyCore(object targetLocation, object view)
        {
            try
            {
                var type = targetLocation.GetType();
                var contentProperty = type.GetCustomAttributes(typeof(ContentPropertyAttribute), true)
                                          .OfType<ContentPropertyAttribute>()
                                          .FirstOrDefault() ?? DefaultContentProperty;

                type.GetProperty(contentProperty?.Name ?? DefaultContentProperty.Name)
                    .SetValue(targetLocation, view, null);

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);

                return false;
            }
        }

        private static bool? inDesignMode;

        /// <summary>
        /// Gets a value that indicates whether the process is running in design mode.
        /// </summary>
        public static bool InDesignMode
        {
            get
            {
                if (!inDesignMode.HasValue)
                {
#if UNITY_5_5_OR_NEWER
                    inDesignMode = false;
#else
                    var descriptor = DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement));
                    inDesignMode = (bool)descriptor.Metadata.DefaultValue;
#endif
                }

                return inDesignMode.GetValueOrDefault(false);
            }
        }
    }
}
