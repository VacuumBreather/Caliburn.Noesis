namespace Caliburn.Noesis
{
    #region Using Directives

    using System.Windows.Input;

    #endregion

    /// <summary>
    ///     Defines a command which can be told to raise its <see cref="ICommand.CanExecuteChanged" />
    ///     event.
    /// </summary>
    public interface IRaisingCommand : ICommand
    {
        /// <summary>Raises the <see cref="ICommand.CanExecuteChanged" /> event.</summary>
        void RaiseCanExecuteChanged();
    }
}