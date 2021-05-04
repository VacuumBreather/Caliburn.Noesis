namespace Caliburn.Noesis.Extensions
{
    #region Using Directives

    using System.ComponentModel;
    using System.Windows.Input;

    #endregion

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
        /// <remarks>
        ///     Calling this causes the command to subscribe to an event on the async guard. This can
        ///     cause a leak if the guard lives longer than the command. Both instances should ideally have the
        ///     same lifecycle, i.e. live on the same view-model. Check life cycles before using this.
        /// </remarks>
        /// <returns>The command.</returns>
        public static IRaisingCommand RaiseWith(this IRaisingCommand command, AsyncGuard asyncGuard)
        {
            asyncGuard.IsOngoingChanged += (_, __) => command.RaiseCanExecuteChanged();

            return command;
        }

        /// <summary>
        ///     Causes this command to raise its <see cref="ICommand.CanExecuteChanged" /> event whenever
        ///     the specified <see cref="IScreen" /> raises its
        ///     <see cref="INotifyPropertyChanged.PropertyChanged" /> event.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="screen">The <see cref="IScreen" /> to watch.</param>
        /// <remarks>
        ///     Calling this causes the command to subscribe to an event on the screen. This can cause a
        ///     leak if the screen lives longer than the command. Both instances should ideally have the same
        ///     lifecycle, i.e. the command should only subscribe to the screen it lives on.
        /// </remarks>
        /// <returns>The command.</returns>
        public static IRaisingCommand RaiseWith(this IRaisingCommand command, IScreen screen)
        {
            screen.PropertyChanged += (_, __) => command.RaiseCanExecuteChanged();

            return command;
        }

        #endregion
    }
}