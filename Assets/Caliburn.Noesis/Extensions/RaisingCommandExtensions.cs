namespace Caliburn.Noesis.Extensions
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    /// <summary>Provides extension methods for the <see cref="IRaisingCommand" /> type.</summary>
    public static class RaisingCommandExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Causes this command to raise its <see cref="ICommand.CanExecuteChanged" /> event whenever
        ///     the specified <see cref="AsyncGuard" /> raises its <see cref="AsyncGuard.IsOngoingChanged" />
        ///     event.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="asyncGuard">The <see cref="AsyncGuard" /> to subscribe to.</param>
        /// <returns>The command.</returns>
        public static IRaisingCommand RaiseWith(this IRaisingCommand command, AsyncGuard asyncGuard)
        {
            var weakReference = new WeakReference(command);
            asyncGuard.IsOngoingChanged += (_, __) => ((IRaisingCommand)weakReference.Target)?.RaiseCanExecuteChanged();

            return command;
        }

        /// <summary>
        ///     Causes this command to raise its <see cref="ICommand.CanExecuteChanged" /> event whenever
        ///     the specified <see cref="INotifyPropertyChanged" /> instance raises its
        ///     <see cref="INotifyPropertyChanged.PropertyChanged" /> event.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="notify">The <see cref="INotifyPropertyChanged" /> instance to watch.</param>
        /// <returns>The command.</returns>
        public static IRaisingCommand RaiseWith(this IRaisingCommand command, INotifyPropertyChanged notify)
        {
            var weakReference = new WeakReference(command);
            notify.PropertyChanged += (_, __) => ((IRaisingCommand)weakReference.Target)?.RaiseCanExecuteChanged();

            return command;
        }

        #endregion
    }
}