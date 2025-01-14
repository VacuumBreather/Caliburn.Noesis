namespace Caliburn.Noesis
{
    using System;
    using System.ComponentModel;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
    using NoesisApp;
#else
    using System.Windows;
    using Microsoft.Xaml.Behaviors;
#endif

    /// <summary>
    /// Represents a parameter of an <see cref="ActionMessage"/>.
    /// </summary>
    public class Parameter : Freezable, IAttachedObject
    {
        /// <summary>
        /// A dependency property representing the parameter's value.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(object),
                typeof(Parameter),
                new PropertyMetadata(OnValueChanged)
                );

        DependencyObject associatedObject;
        WeakReference owner;

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        /// <value>The value.</value>
        [Category("Common Properties")]
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        DependencyObject IAttachedObject.AssociatedObject
        {
            get
            {
#if !UNITY_5_5_OR_NEWER
                ReadPreamble();
#endif
                return associatedObject;
            }
        }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        protected ActionMessage Owner
        {
            get { return owner == null ? null : owner.Target as ActionMessage; }
            set { owner = new WeakReference(value); }
        }

        void IAttachedObject.Attach(DependencyObject dependencyObject)
        {
#if !UNITY_5_5_OR_NEWER
            WritePreamble();
#endif
            associatedObject = dependencyObject;
#if !UNITY_5_5_OR_NEWER
            WritePostscript();
#endif
        }

        void IAttachedObject.Detach()
        {
#if !UNITY_5_5_OR_NEWER
            WritePreamble();
#endif
            associatedObject = null;
#if !UNITY_5_5_OR_NEWER
            WritePostscript();
#endif
        }
#if !UNITY_5_5_OR_NEWER
        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable"/> derived class.
        /// </summary>
        /// <returns>The new instance.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new Parameter();
        }
#endif

        /// <summary>
        /// Makes the parameter aware of the <see cref="ActionMessage"/> that it's attached to.
        /// </summary>
        /// <param name="actionMessageOwner">The action message.</param>
        internal void MakeAwareOf(ActionMessage actionMessageOwner)
        {
            Owner = actionMessageOwner;
        }

        static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var parameter = (Parameter)d;
            var owner = parameter.Owner;

            if (owner != null)
            {
                owner.UpdateAvailability();
            }
        }
    }
}
