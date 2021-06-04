namespace Caliburn.Noesis
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Threading;
    using JetBrains.Annotations;

    /// <summary>A base class that implements the infrastructure for property change notification.</summary>
    [PublicAPI]
    [DataContract]
    public abstract class BindableObject : IBindableObject, INotifyPropertyChanging
    {
        #region Constants and Fields

        private int suspensionCount;

        #endregion

        #region IBindableObject Implementation

        /// <summary>Raises a change notification indicating that all bindings should be refreshed.</summary>
        public virtual void Refresh()
        {
            RaisePropertyChanging(string.Empty);
            RaisePropertyChanged(string.Empty);
        }

        /// <summary>Suspends the change notifications.</summary>
        /// <returns>A guard resuming the notifications when it goes out of scope.</returns>
        /// <remarks>Use the guard in a using statement.</remarks>
        public IDisposable SuspendNotifications()
        {
            Interlocked.Increment(ref this.suspensionCount);

            return new DisposableAction(ResumeNotifications);
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region INotifyPropertyChanging Implementation

        /// <inheritdoc />
        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        #region Protected Methods

        /// <summary>Assigns a new value to the property. Then, raises the PropertyChanged event if needed.</summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <returns>
        ///     <c>true</c> if the PropertyChanged event has been raised, otherwise, <c>false</c>. The
        ///     event is not raised if the old value is equal to the new value.
        /// </returns>
        protected virtual bool Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            RaisePropertyChanging(propertyName);
            field = newValue;
            RaisePropertyChanged(propertyName);

            return true;
        }

        /// <summary>Determines whether notifications are suspended.</summary>
        /// <returns></returns>
        protected bool AreNotificationsSuspended()
        {
            return this.suspensionCount > 0;
        }

        /// <summary>Raises the PropertyChanged event if needed.</summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (AreNotificationsSuspended())
            {
                return;
            }

            Execute.OnUIThread(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
        }

        /// <summary>Raises the PropertyChanging event if needed.</summary>
        /// <param name="propertyName">The name of the property that is changing.</param>
        protected void RaisePropertyChanging([CallerMemberName] string propertyName = null)
        {
            if (AreNotificationsSuspended())
            {
                return;
            }

            Execute.OnUIThread(() => PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName)));
        }

        #endregion

        #region Private Methods

        private void ResumeNotifications()
        {
            Interlocked.Decrement(ref this.suspensionCount);
        }

        #endregion
    }
}