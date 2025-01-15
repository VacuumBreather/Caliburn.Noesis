namespace Caliburn.Noesis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using Cysharp.Threading.Tasks;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
    using NoesisApp;
    using EventTrigger = NoesisApp.EventTrigger;
#else
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Markup;
    using Microsoft.Xaml.Behaviors;
    using EventTrigger = Microsoft.Xaml.Behaviors.EventTrigger;
#endif

#if NET5_0_WINDOWS || NET6_0_WINDOWS
    using System.IO;
    using System.Xml;
#endif


    /// <summary>
    /// Used to send a message from the UI to a presentation model class, indicating that a particular Action should be invoked.
    /// </summary>
    [ContentProperty("Parameters")]
#if !UNITY_5_5_OR_NEWER
    [DefaultTrigger(typeof(FrameworkElement), typeof(EventTrigger), "MouseLeftButtonDown")]
    [DefaultTrigger(typeof(ButtonBase), typeof(EventTrigger), "Click")]
    [TypeConstraint(typeof(FrameworkElement))]
#endif
    public class ActionMessage : TriggerAction<FrameworkElement>, IHaveParameters
    {
        static readonly ILog Log = LogManager.GetLog(typeof(ActionMessage));
        ActionExecutionContext _context;

        internal static readonly DependencyProperty HandlerProperty =
            DependencyProperty.RegisterAttached(
            "Handler",
            typeof(object),
            typeof(ActionMessage),
            new PropertyMetadata(null, HandlerPropertyChanged)
            );



        ///<summary>
        /// Causes the action invocation to "double check" if the action should be invoked by executing the guard immediately before hand.
        ///</summary>
        /// <remarks>This is disabled by default. If multiple actions are attached to the same element, you may want to enable this so that each individaul action checks its guard regardless of how the UI state appears.</remarks>
        public static bool EnforceGuardsDuringInvocation = false;

        ///<summary>
        /// Causes the action to throw if it cannot locate the target or the method at invocation time.
        ///</summary>
        /// <remarks>True by default.</remarks>
        public static bool ThrowsExceptions = true;

        /// <summary>
        /// Represents the method name of an action message.
        /// </summary>
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register(
                "MethodName",
                typeof(string),
                typeof(ActionMessage),
                null
                );

        /// <summary>
        /// Represents the parameters of an action message.
        /// </summary>
        public static readonly DependencyProperty ParametersProperty =
            DependencyProperty.Register(
            "Parameters",
            typeof(AttachedCollection<Parameter>),
            typeof(ActionMessage),
            null
            );

        /// <summary>
        /// Creates an instance of <see cref="ActionMessage"/>.
        /// </summary>
        public ActionMessage()
        {
            SetValue(ParametersProperty, new AttachedCollection<Parameter>());
        }

        /// <summary>
        /// Gets or sets the name of the method to be invoked on the presentation model class.
        /// </summary>
        /// <value>The name of the method.</value>
        [Category("Common Properties")]
        public string MethodName
        {
            get { return (string)GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }

        /// <summary>
        /// Gets the parameters to pass as part of the method invocation.
        /// </summary>
        /// <value>The parameters.</value>
        [Category("Common Properties")]
        public AttachedCollection<Parameter> Parameters
        {
            get { return (AttachedCollection<Parameter>)GetValue(ParametersProperty); }
        }

        /// <summary>
        /// Occurs before the message detaches from the associated object.
        /// </summary>
        public event System.EventHandler Detaching = delegate { };
        
        protected override void OnAttached()
        {
            if (!Noesis.View.InDesignMode)
            {
                Parameters.Attach(AssociatedObject);
                Parameters.Apply(x => x.MakeAwareOf(this));

                if (Noesis.View.ExecuteOnLoad(AssociatedObject, ElementLoaded))
                {
                    string eventName = "Loaded";
                    var trigger = Interaction.GetTriggers(AssociatedObject)
                     .FirstOrDefault(t => t.Actions.Contains(this)) as EventTrigger;

                    Log.Info($"Trigger EventName '{trigger?.EventName}' event name '{eventName}'");
                    if (trigger != null && trigger.EventName == eventName)
                    {
                        Invoke(new RoutedEventArgs(trigger.SourceObject.GetRoutedEvent(trigger.EventName), trigger.SourceObject));
                    }
                }
            }

            base.OnAttached();
        }

        static void HandlerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Log.Info($"Handler property changed {d}");
            ((ActionMessage)d).UpdateContext();
        }

        /// <summary>
        /// Called when the action is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            if (!Noesis.View.InDesignMode)
            {
                Detaching(this, System.EventArgs.Empty);
                //TODO: Fix this: cannot remove ElementLoaded here because a wrapper handler was added instead (in View.ExecuteOnLoad() called from this.OnAttached())
                AssociatedObject.Loaded -= ElementLoaded;
                Parameters.Detach();
            }

            base.OnDetaching();
        }

        void ElementLoaded(object sender, RoutedEventArgs e)
        {
            UpdateContext();

            DependencyObject currentElement;
            if (_context.View == null)
            {
                currentElement = AssociatedObject;
                while (currentElement != null)
                {
                    if (Action.HasTargetSet(currentElement))
                        break;

                    currentElement = BindingScope.GetVisualParent(currentElement);
                }
            }
            else
            {
                currentElement = _context.View;
            }

            var binding = new Binding {
                Path = new PropertyPath(Message.HandlerProperty), 
                Source = currentElement
            };

            BindingOperations.SetBinding(this, HandlerProperty, binding);
        }

        void UpdateContext()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
            _context = new ActionExecutionContext
            {
                Message = this,
                Source = AssociatedObject
            };

            PrepareContext(_context);
            UpdateAvailabilityCore();
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="eventArgs">The parameter to the action. If the action does not require a parameter, the parameter may be set to a null reference.</param>
        protected override void Invoke(object eventArgs)
        {
            Log.Info("Invoking {0}.", this);

            if (_context == null)
            {
                UpdateContext();
            }

            if (_context.Target == null || _context.View == null)
            {
                PrepareContext(_context);
                if (_context.Target == null)
                {
                    var ex = new Exception(string.Format("No target found for method {0}.", _context.Message.MethodName));
                    Log.Error(ex);

                    if (!ThrowsExceptions)
                        return;
                    throw ex;
                }

                if (!UpdateAvailabilityCore())
                {
                    return;
                }
            }

            if (_context.Method == null)
            {
                var ex = new Exception(string.Format("Method {0} not found on target of type {1}.", _context.Message.MethodName, _context.Target.GetType()));
                Log.Error(ex);

                if (!ThrowsExceptions)
                    return;
                throw ex;
            }

            _context.EventArgs = eventArgs;

            if (EnforceGuardsDuringInvocation && (_context?.CanExecute() != true))
            {
                return;
            }

            InvokeAction(_context);
            _context.EventArgs = null;
        }

        /// <summary>
        /// Forces an update of the UI's Enabled/Disabled state based on the the preconditions associated with the method.
        /// </summary>
        public virtual void UpdateAvailability()
        {
            if (_context == null)
            {
                Log.Info("{0} availability update. Context is null", this);
                return;
            }
            if (_context.Target == null || _context.View == null)
                PrepareContext(_context);

            UpdateAvailabilityCore();
        }

        bool UpdateAvailabilityCore()
        {
            Log.Info("{0} availability update.", this);
            return ApplyAvailabilityEffect(_context);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "Action: " + MethodName;
        }

        /// <summary>
        /// Invokes the action using the specified <see cref="ActionExecutionContext"/>
        /// </summary>
        public static Action<ActionExecutionContext> InvokeAction = context =>
        {
            var values = MessageBinder.DetermineParameters(context, context.Method.GetParameters());
            var returnValue = context.Method.Invoke(context.Target, values);

            if (returnValue is UniTask task)
            {
                returnValue = task.AsResult();
            }

            var result = returnValue as IResult;
            if (result != null)
            {
                returnValue = new[] { result };
            }

            var enumerable = returnValue as IEnumerable<IResult>;
            if (enumerable != null)
            {
                returnValue = enumerable.GetEnumerator();
            }

            var enumerator = returnValue as IEnumerator<IResult>;
            if (enumerator != null)
            {
                Coroutine.BeginExecute(enumerator,
                    new CoroutineExecutionContext
                    {
                        Source = context.Source,
                        View = context.View,
                        Target = context.Target
                    });
            }
        };

        /// <summary>
        /// Applies an availability effect, such as IsEnabled, to an element.
        /// </summary>
        /// <remarks>Returns a value indicating whether or not the action is available.</remarks>
        public static Func<ActionExecutionContext, bool> ApplyAvailabilityEffect = context =>
        {
            Log.Info("ApplyAvailabilityEffect");

            var source = context.Source;

            if (source == null)
            {
                Log.Info("ApplyAvailabilityEffect source is null");
                return true;
            }

            Log.Info($"HasBinding source {source.Name}");
            var hasBinding = ConventionManager.HasBinding(source, UIElement.IsEnabledProperty);
            Log.Info($"ApplyAvailabilityEffect hasBinding {hasBinding}");

            if (!hasBinding && context.CanExecute != null)
            {
                Log.Info($"ApplyAvailabilityEffect CanExecute {context.CanExecute()} - {context.Method.Name}");
                source.IsEnabled = context.CanExecute();
            }

            Log.Info($"ApplyAvailabilityEffect source enabled {source.IsEnabled}");
            return source.IsEnabled;
        };

        /// <summary>
        /// Finds the method on the target matching the specified message.
        /// </summary>
        /// <returns>The matching method, if available.</returns>
        public static Func<ActionMessage, object, MethodInfo> GetTargetMethod = (message, target) =>
        {
            return (from method in target.GetType().GetMethods()
                    where method.Name == message.MethodName
                    let methodParameters = method.GetParameters()
                    where message.Parameters.Count == methodParameters.Length
                    select method).FirstOrDefault();
        };

        /// <summary>
        /// Sets the target, method and view on the context. Uses a bubbling strategy by default.
        /// </summary>
        public static Action<ActionExecutionContext> SetMethodBinding = context =>
        {
            Log.Info("SetMethodBinding");
            var source = context.Source;

            DependencyObject currentElement = source;
            while (currentElement != null)
            {
                if (Action.HasTargetSet(currentElement))
                {
                    var target = Message.GetHandler(currentElement);
                    Log.Info("SetMethodBinding target is null {0}", target == null);
                    if (target != null)
                    {
                        var method = GetTargetMethod(context.Message, target);
                        if (method != null)
                        {
                            context.Method = method;
                            context.Target = target;
                            context.View = currentElement;
                            return;
                        }
                    }
                    else
                    {
                        context.View = currentElement;
                        return;
                    }
                }

                currentElement = BindingScope.GetVisualParent(currentElement);
            }

            if (source != null && source.DataContext != null)
            {
                var target = source.DataContext;
                var method = GetTargetMethod(context.Message, target);

                if (method != null)
                {
                    context.Target = target;
                    context.Method = method;
                    context.View = source;
                }
            }
        };

        /// <summary>
        /// Prepares the action execution context for use.
        /// </summary>
        public static Action<ActionExecutionContext> PrepareContext = context =>
        {
            Log.Info("PrepareContext");
            SetMethodBinding(context);
            Log.Info("PrepareContext Context is  {0}", context.View);
            Log.Info("PrepareContext method is null {0}", context.Method == null);
            Log.Info("PrepareContext target is null {0}", context.Target == null);
            if (context.Target == null || context.Method == null)
            {
                return;
            }
            var possibleGuardNames = BuildPossibleGuardNames(context.Method).ToList();
            Log.Info($"PrepareContext {possibleGuardNames.Count}");
            foreach (var methodName in possibleGuardNames)
            {
                Log.Info($"PrepareContext {methodName}");
            }
            var guard = TryFindGuardMethod(context, possibleGuardNames);

            if (guard == null)
            {
                Log.Info("Guard not found");
                var inpc = context.Target as INotifyPropertyChanged;
                if (inpc == null)
                    return;

                var targetType = context.Target.GetType();
                string matchingGuardName = null;
                foreach (string possibleGuardName in possibleGuardNames)
                {
                    matchingGuardName = possibleGuardName;
                    guard = GetMethodInfo(targetType, "get_" + matchingGuardName);
                    if (guard != null)
                        break;
                }

                if (guard == null)
                    return;
                Log.Info("Found guard 2 try");
                PropertyChangedEventHandler handler = null;
                handler = (s, e) =>
                {
                    if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == matchingGuardName)
                    {
                        Log.Info($"UpdateAvailablilty  {e.PropertyName}");
                        Caliburn.Noesis.Execute.OnUIThread(() =>
                        {
                            var message = context.Message;
                            if (message == null)
                            {
                                inpc.PropertyChanged -= handler;
                                return;
                            }
                            message.UpdateAvailability();
                        });
                    }
                };

                inpc.PropertyChanged += handler;
                context.Disposing += delegate
                { inpc.PropertyChanged -= handler; };
                context.Message.Detaching += delegate
                { inpc.PropertyChanged -= handler; };
            }
            Log.Info("guard create method {0}", guard.Name);
            context.CanExecute = () => (bool)guard.Invoke(
                context.Target,
                MessageBinder.DetermineParameters(context, guard.GetParameters()));
        };

        /// <summary>
        /// Try to find a candidate for guard function, having: 
        ///    - a name matching any of <paramref name="possibleGuardNames"/>
        ///    - no generic parameters
        ///    - a bool return type
        ///    - no parameters or a set of parameters corresponding to the action method
        /// </summary>
        /// <param name="context">The execution context</param>
        /// <param name="possibleGuardNames">Method names to look for.</param>
        ///<returns>A MethodInfo, if found; null otherwise</returns>
        static MethodInfo TryFindGuardMethod(ActionExecutionContext context, IEnumerable<string> possibleGuardNames)
        {
            var targetType = context.Target.GetType();
            MethodInfo guard = null;
            foreach (string possibleGuardName in possibleGuardNames)
            {
                guard = GetMethodInfo(targetType, possibleGuardName);
                if (guard != null)
                    break;
            }

            if (guard == null)
                return null;
            if (guard.ContainsGenericParameters)
                return null;
            if (!typeof(bool).Equals(guard.ReturnType))
                return null;

            var guardPars = guard.GetParameters();
            var actionPars = context.Method.GetParameters();
            if (guardPars.Length == 0)
                return guard;
            if (guardPars.Length != actionPars.Length)
                return null;

            var comparisons = guardPars.Zip(
                context.Method.GetParameters(),
                (x, y) => x.ParameterType == y.ParameterType
                );

            if (comparisons.Any(x => !x))
            {
                return null;
            }
            Log.Info($"TryFindGuardMethod {guard.Name}");
            return guard;
        }

        /// <summary>
        /// Returns the list of possible names of guard methods / properties for the given method.
        /// </summary>
        public static Func<MethodInfo, IEnumerable<string>> BuildPossibleGuardNames = method =>
        {

            var guardNames = new List<string>();

            const string GuardPrefix = "Can";

            var methodName = method.Name;

            guardNames.Add(GuardPrefix + methodName);

            const string AsyncMethodSuffix = "Async";

            if (methodName.EndsWith(AsyncMethodSuffix, StringComparison.OrdinalIgnoreCase))
            {
                guardNames.Add(GuardPrefix + methodName.Substring(0, methodName.Length - AsyncMethodSuffix.Length));
            }

            return guardNames;
        };

        static MethodInfo GetMethodInfo(Type t, string methodName)
        {
            return t.GetMethod(methodName);
        }
    }
}
