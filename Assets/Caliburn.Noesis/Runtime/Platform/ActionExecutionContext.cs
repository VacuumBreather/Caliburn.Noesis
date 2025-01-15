namespace Caliburn.Noesis
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
#endif

    /// <summary>
    /// The context used during the execution of an Action or its guard.
    /// </summary>
    public class ActionExecutionContext : IDisposable {
        private WeakReference _message;
        private WeakReference _source;
        private WeakReference _target;
        private WeakReference _view;
        private WeakReference _serviceLocator;
        private Dictionary<string, object> _values;

        /// <summary>
        /// Determines whether the action can execute.
        /// </summary>
        /// <remarks>Returns true if the action can execute, false otherwise.</remarks>
        public Func<bool> CanExecute;

        /// <summary>
        /// Any event arguments associated with the action's invocation.
        /// </summary>
        public object EventArgs;

        /// <summary>
        /// The actual method info to be invoked.
        /// </summary>
        public MethodInfo Method;

        /// <summary>
        /// The message being executed.
        /// </summary>
        public ActionMessage Message {
            get { return _message == null ? null : _message.Target as ActionMessage; }
            set { _message = new WeakReference(value); }
        }

        /// <summary>
        /// The source from which the message originates.
        /// </summary>
        public FrameworkElement Source {
            get { return _source == null ? null : _source.Target as FrameworkElement; }
            set { _source = new WeakReference(value); }
        }

        /// <summary>
        /// The instance on which the action is invoked.
        /// </summary>
        public object Target {
            get { return _target == null ? null : _target.Target; }
            set { _target = new WeakReference(value); }
        }

        /// <summary>
        /// The view associated with the target.
        /// </summary>
        public DependencyObject View {
            get { return _view == null ? null : _view.Target as DependencyObject; }
            set { _view = new WeakReference(value); }
        }

        /// <summary>
        /// The service locator associated with the target.
        /// </summary>
        public IServiceLocator ServiceLocator {
            get { return _serviceLocator == null ? null : _serviceLocator.Target as IServiceLocator; }
            set { _serviceLocator = new WeakReference(value); }
        }

        /// <summary>
        /// Gets or sets additional data needed to invoke the action.
        /// </summary>
        /// <param name="key">The data key.</param>
        /// <returns>Custom data associated with the context.</returns>
        public object this[string key] {
            get {
                if (_values == null)
                    _values = new Dictionary<string, object>();

                object result;
                _values.TryGetValue(key, out result);

                return result;
            }
            set {
                if (_values == null)
                    _values = new Dictionary<string, object>();

                _values[key] = value;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            Disposing(this, System.EventArgs.Empty);
        }

        /// <summary>
        /// Called when the execution context is disposed
        /// </summary>
        public event System.EventHandler Disposing = delegate { };
    }
}
