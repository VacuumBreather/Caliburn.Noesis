namespace Caliburn.Noesis
{
    /// <summary>
    /// Defines a command.
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    public interface ICommand<in T> : IRaisingCommand
    {
        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
        bool CanExecute(T parameter);

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        void Execute(T parameter);
    }
}