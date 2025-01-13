﻿namespace Caliburn.Noesis
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using Cysharp.Threading.Tasks;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
    using Microsoft.Xaml.Behaviors;
#endif

    /// <summary>
    /// Binds a view to a view model.
    /// </summary>
    public static class ViewModelBinder {
        const string AsyncSuffix = "Async";

        static readonly ILog Log = LogManager.GetLog(typeof(ViewModelBinder));

        /// <summary>
        /// Gets or sets a value indicating whether to apply conventions by default.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if conventions should be applied by default; otherwise, <c>false</c>.
        /// </value>
        public static bool ApplyConventionsByDefault = true;

        /// <summary>
        /// Indicates whether or not the conventions have already been applied to the view.
        /// </summary>
        public static readonly DependencyProperty ConventionsAppliedProperty =
            DependencyPropertyHelper.RegisterAttached(
                "ConventionsApplied",
                typeof(bool),
                typeof(ViewModelBinder),
                false
                );

        /// <summary>
        /// Determines whether a view should have conventions applied to it.
        /// </summary>
        /// <param name="view">The view to check.</param>
        /// <returns>Whether or not conventions should be applied to the view.</returns>
        public static bool ShouldApplyConventions(FrameworkElement view) {
            var overriden = View.GetApplyConventions(view);
            return overriden.GetValueOrDefault(ApplyConventionsByDefault);
        }

        /// <summary>
        /// Creates data bindings on the view's controls based on the provided properties.
        /// </summary>
        /// <remarks>Parameters include named Elements to search through and the type of view model to determine conventions for. Returns unmatched elements.</remarks>
        public static Func<IEnumerable<FrameworkElement>, Type, IEnumerable<FrameworkElement>> BindProperties = (namedElements, viewModelType) => {

            var unmatchedElements = new List<FrameworkElement>();

            foreach (var element in namedElements) {
                var cleanName = element.Name.Trim('_');
                var parts = cleanName.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

                var property = viewModelType.GetPropertyCaseInsensitive(parts[0]);
                var interpretedViewModelType = viewModelType;

                for (int i = 1; i < parts.Length && property != null; i++) {
                    interpretedViewModelType = property.PropertyType;
                    property = interpretedViewModelType.GetPropertyCaseInsensitive(parts[i]);
                }

                if (property == null) {
                    unmatchedElements.Add(element);
                    Log.Info("Binding Convention Not Applied: Element {0} did not match a property.", element.Name);
                    continue;
                }

                var convention = ConventionManager.GetElementConvention(element.GetType());
                if (convention == null) {
                    unmatchedElements.Add(element);
                    Log.Warn("Binding Convention Not Applied: No conventions configured for {0}.", element.GetType());
                    continue;
                }

                var applied = convention.ApplyBinding(
                    interpretedViewModelType,
                    cleanName.Replace('_', '.'),
                    property,
                    element,
                    convention
                    );

                if (applied) {
                    Log.Info("Binding Convention Applied: Element {0}.", element.Name);
                }
                else {
                    Log.Info("Binding Convention Not Applied: Element {0} has existing binding.", element.Name);
                    unmatchedElements.Add(element);
                }
            }

            return unmatchedElements;

        };

        /// <summary>
        /// Attaches instances of <see cref="ActionMessage"/> to the view's controls based on the provided methods.
        /// </summary>
        /// <remarks>Parameters include the named elements to search through and the type of view model to determine conventions for. Returns unmatched elements.</remarks>
        public static Func<IEnumerable<FrameworkElement>, Type, IEnumerable<FrameworkElement>> BindActions = (namedElements, viewModelType) => {
            var unmatchedElements = namedElements.ToList();
            var methods = viewModelType.GetMethods();

            foreach (var method in methods) {
            Log.Info($"Searching for methods control {method.Name} unmatchedElements count {unmatchedElements.Count}");
                var foundControl = unmatchedElements.FindName(method.Name);
                if (foundControl == null && IsAsyncMethod(method)) {
                    var methodNameWithoutAsyncSuffix = method.Name.Substring(0, method.Name.Length - AsyncSuffix.Length);
                    foundControl = unmatchedElements.FindName(methodNameWithoutAsyncSuffix);
                }

                if(foundControl == null) {
                    Log.Info("Action Convention Not Applied: No actionable element for {0}. {1}", method.Name, unmatchedElements.Count);
                    foreach(var element in unmatchedElements)
                    {
                    Log.Info($"Unnamed element {element.Name}");
                    }
                    continue;
                }

                unmatchedElements.Remove(foundControl);

                var message = method.Name;
                var parameters = method.GetParameters();

                if (parameters.Length > 0) {
                    message += "(";

                    foreach (var parameter in parameters) {
                        var paramName = parameter.Name;
                        var specialValue = "$" + paramName.ToLower();

                        if (MessageBinder.SpecialValues.ContainsKey(specialValue))
                            paramName = specialValue;

                        message += paramName + ",";
                    }

                    message = message.Remove(message.Length - 1, 1);
                    message += ")";
                }

                Log.Info("Action Convention Applied: Action {0} on element {1}.", method.Name, message);
                Message.SetAttach(foundControl, message);
            }

            return unmatchedElements;
        };

        static bool IsAsyncMethod(MethodInfo method) {
            return IntrospectionExtensions.GetTypeInfo(typeof(UniTask)).IsAssignableFrom(IntrospectionExtensions.GetTypeInfo(method.ReturnType)) &&
                   method.Name.EndsWith(AsyncSuffix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Allows the developer to add custom handling of named elements which were not matched by any default conventions.
        /// </summary>
        public static Action<IEnumerable<FrameworkElement>, Type> HandleUnmatchedElements = (elements, viewModelType) => { };

        /// <summary>
        /// Binds the specified viewModel to the view.
        /// </summary>
        ///<remarks>Passes the the view model, view and creation context (or null for default) to use in applying binding.</remarks>
        public static Action<object, DependencyObject, object> Bind = (viewModel, view, context) => {

            // when using d:DesignInstance, Blend tries to assign the DesignInstanceExtension class as the DataContext,
            // so here we get the actual ViewModel which is in the Instance property of DesignInstanceExtension
            if (View.InDesignMode) {
                var vmType = viewModel.GetType();
                if (vmType.FullName == "Microsoft.Expression.DesignModel.InstanceBuilders.DesignInstanceExtension") {
                    var propInfo = vmType.GetProperty("Instance", BindingFlags.Instance | BindingFlags.NonPublic);
                    viewModel = propInfo.GetValue(viewModel, null);
                }
            }

            Log.Info("Binding {0} and {1}.", view, viewModel);


            var noContext = Caliburn.Noesis.Bind.NoContextProperty;

            if ((bool)view.GetValue(noContext)) {
                Action.SetTargetWithoutContext(view, viewModel);
            }
            else {
                Action.SetTarget(view, viewModel);
            }

            var viewAware = viewModel as IViewAware;
            if (viewAware != null) {
                Log.Info("Attaching {0} to {1}.", view, viewAware);
                viewAware.AttachView(view, context);
            }

            if ((bool)view.GetValue(ConventionsAppliedProperty)) {
                return;
            }

            var element = View.GetFirstNonGeneratedView(view) as FrameworkElement;
            if (element == null) {
                return;
            }

            if (!ShouldApplyConventions(element)) {
                Log.Info("Skipping conventions for {0} and {1}.", element, viewModel);
                return;
            }

            var viewModelType = viewModel.GetType();
            var namedElements = BindingScope.GetNamedElements(element);

            namedElements = BindActions(namedElements, viewModelType);
            namedElements = BindProperties(namedElements, viewModelType);
            HandleUnmatchedElements(namedElements, viewModelType);

            view.SetValue(ConventionsAppliedProperty, true);
        };
    }
}
