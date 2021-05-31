namespace Caliburn.Noesis
{
    using System;
    using System.Windows.Input;
    using Cysharp.Threading.Tasks;
    using Extensions;
    using JetBrains.Annotations;

    /// <summary>A command which relays its execution to an asynchronous delegate.</summary>
    [PublicAPI]
    public class AsyncRelayCommand : IAsyncCommand
    {
        #region Constants and Fields

        /// <summary>A command which does nothing and can always be executed.</summary>
        public static AsyncRelayCommand DoNothing =
            new AsyncRelayCommand(() => UniTask.CompletedTask);

        private readonly AsyncGuard asyncGuard = new AsyncGuard();

        private readonly Func<bool> canExecute;
        private readonly Func<UniTask> execute;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="AsyncRelayCommand" /> class.</summary>
        /// <param name="execute">The asynchronous action to perform when the command is executed.</param>
        /// <param name="canExecute">(Optional) The predicate which checks if the command can be executed.</param>
        public AsyncRelayCommand(Func<UniTask> execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;

            this.asyncGuard.IsOngoingChanged += (_, __) => RaiseCanExecuteChanged();
        }

        #endregion

        #region IAsyncCommand Implementation

        /// <inheritdoc />
        public bool CanExecute()
        {
            return !this.asyncGuard.IsOngoing && (this.canExecute?.Invoke() ?? true);
        }

        /// <inheritdoc />
        public async UniTask ExecuteAsync()
        {
            if (CanExecute())
            {
                await this.execute().Using(this.asyncGuard);
            }
        }

        #endregion

        #region ICommand Implementation

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;

        /// <inheritdoc />
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        /// <inheritdoc />
        void ICommand.Execute(object parameter)
        {
            ExecuteAsync().Forget();
        }

        #endregion

        #region IRaisingCommand Implementation

        /// <summary>Raises the <see cref="CanExecuteChanged" /> event.</summary>
        public void RaiseCanExecuteChanged()
        {
            Execute.OnUIThread(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }

        #endregion
    }

    /// <summary>A command which relays its execution to an asynchronous delegate.</summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    [PublicAPI]
    public class AsyncRelayCommand<T> : IAsyncCommand<T>
    {
        #region Constants and Fields

        /// <summary>A command which does nothing and can always be executed.</summary>
        public static AsyncRelayCommand<T> DoNothing =
            new AsyncRelayCommand<T>(_ => UniTask.CompletedTask);

        private readonly AsyncGuard asyncGuard = new AsyncGuard();

        private readonly Func<T, bool> canExecute;
        private readonly Func<T, UniTask> execute;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="AsyncRelayCommand{T}" /> class.</summary>
        /// <param name="execute">The asynchronous action to perform when the command is executed.</param>
        /// <param name="canExecute">(Optional) The predicate which checks if the command can be executed.</param>
        public AsyncRelayCommand(Func<T, UniTask> execute, Func<T, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;

            this.asyncGuard.IsOngoingChanged += (_, __) => RaiseCanExecuteChanged();
        }

        #endregion

        #region IAsyncCommand<T> Implementation

        /// <inheritdoc />
        public bool CanExecute(T parameter)
        {
            return !this.asyncGuard.IsOngoing && (this.canExecute?.Invoke(parameter) ?? true);
        }

        /// <inheritdoc />
        public async UniTask ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter))
            {
                await this.execute(parameter).Using(this.asyncGuard);
            }
        }

        #endregion

        #region ICommand Implementation

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;

        /// <inheritdoc />
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }

        /// <inheritdoc />
        void ICommand.Execute(object parameter)
        {
            ExecuteAsync((T)parameter).Forget();
        }

        #endregion

        #region IRaisingCommand Implementation

        /// <summary>Raises the <see cref="CanExecuteChanged" /> event.</summary>
        public void RaiseCanExecuteChanged()
        {
            Execute.OnUIThread(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }

        #endregion
    }
}