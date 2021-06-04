namespace Caliburn.Noesis
{
    using System;
    using System.ComponentModel;
    using JetBrains.Annotations;

    /// <summary>
    ///     Extends <see cref="INotifyPropertyChanged" />  such that the change event can be
    ///     suspended.
    /// </summary>
    [PublicAPI]
    public interface IBindableObject : INotifyPropertyChanged
    {
        /// <summary>Raises a change notification indicating that all bindings should be refreshed.</summary>
        void Refresh();

        /// <summary>Suspends property change notifications.</summary>
        IDisposable SuspendNotifications();
    }
}