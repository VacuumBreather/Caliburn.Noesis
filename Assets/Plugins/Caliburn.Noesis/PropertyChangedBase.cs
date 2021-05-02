namespace Caliburn.Noesis
{
    #region Using Directives

    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    #endregion

    /// <summary>
    ///     A base class that implements the infrastructure for property change notification.
    /// </summary>
    [DataContract]
    public abstract class PropertyChangedBase : INotifyPropertyChangedEx
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Creates an instance of <see cref="PropertyChangedBase" />.
        /// </summary>
        protected PropertyChangedBase()
        {
            IsNotifying = true;
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region INotifyPropertyChangedEx Implementation

        /// <summary>
        ///     Enables/Disables property change notification.
        /// </summary>
        public bool IsNotifying { get; set; }

        /// <summary>
        ///     Notifies subscribers of the property change.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public virtual void NotifyOfPropertyChange([CallerMemberName] string propertyName = null)
        {
            if (IsNotifying && (PropertyChanged != null))
            {
                Execute.OnUIThread(() => OnPropertyChanged(new PropertyChangedEventArgs(propertyName)));
            }
        }

        /// <summary>
        ///     Raises a change notification indicating that all bindings should be refreshed.
        /// </summary>
        public virtual void Refresh()
        {
            NotifyOfPropertyChange(string.Empty);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Raises the <see cref="PropertyChanged" /> event directly.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        ///     Sets a backing field value and if it's changed raise a notification.
        /// </summary>
        /// <typeparam name="T">The type of the value being set.</typeparam>
        /// <param name="oldValue">A reference to the field to update.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">The name of the property for change notifications.</param>
        /// <returns>
        ///     Returns <c>true</c> if the new value differed from the old one and raised a change notification; otherwise
        ///     <c>false</c>.
        /// </returns>
        protected bool Set<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return false;
            }

            oldValue = newValue;

            NotifyOfPropertyChange(propertyName ?? string.Empty);

            return true;
        }

        #endregion
    }
}