namespace Caliburn.Noesis
{
    using System;
    using System.Windows.Input;

    /// <summary>A command which relays its execution to a delegate.</summary>
    public class RelayCommand : IRaisingCommand
    {
        #region Constants and Fields

        /// <summary>A command which does nothing and can always be executed.</summary>
        public static RelayCommand DoNothing = new RelayCommand(() => { });

        private readonly Func<bool> canExecute;
        private readonly Action execute;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="RelayCommand" /> class.</summary>
        /// <param name="execute">The action to perform when the command is executed.</param>
        /// <param name="canExecute">(Optional) The predicate which checks if the command can be executed.</param>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion

        #region ICommand Implementation

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            return this.canExecute?.Invoke() ?? true;
        }

        /// <inheritdoc />
        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                this.execute?.Invoke();
            }
        }

        #endregion

        #region IRaisingCommand Implementation

        /// <summary>Raises the <see cref="CanExecuteChanged" /> event.</summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }

    /// <summary>A command which relays its execution to a delegate.</summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    public class RelayCommand<T> : ICommand<T>
    {
        #region Constants and Fields

        /// <summary>A command which does nothing and can always be executed.</summary>
        public static RelayCommand<T> DoNothing = new RelayCommand<T>(_ => { });

        private readonly Func<T, bool> canExecute;
        private readonly Action<T> execute;

        #endregion

        #region Constructors and Destructors

        /// <summary>Creates a new instance of <see cref="RelayCommand{T}" />.</summary>
        /// <param name="execute">The action to perform when the command is executed.</param>
        /// <param name="canExecute">(Optional) The predicate which checks if the command can be executed.</param>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
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
            Execute((T)parameter);
        }

        #endregion

        #region ICommand<T> Implementation

        /// <inheritdoc />
        public bool CanExecute(T parameter)
        {
            return this.canExecute?.Invoke(parameter) ?? true;
        }

        /// <inheritdoc />
        public void Execute(T parameter)
        {
            if (CanExecute(parameter))
            {
                this.execute?.Invoke(parameter);
            }
        }

        #endregion

        #region IRaisingCommand Implementation

        /// <summary>Raises the <see cref="CanExecuteChanged" /> event.</summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}