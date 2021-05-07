namespace Caliburn.Noesis
{
    using Cysharp.Threading.Tasks;

    /// <summary>Defines a command which executes asynchronously.</summary>
    public interface IAsyncCommand : IRaisingCommand
    {
        /// <summary>Defines the method that determines whether the command can execute in its current state.</summary>
        /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
        bool CanExecute();

        /// <summary>Defines the asynchronous method to be called when the command is invoked.</summary>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        UniTask ExecuteAsync();
    }

    /// <summary>Defines a command which executes asynchronously.</summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    public interface IAsyncCommand<in T> : IRaisingCommand
    {
        /// <summary>Defines the method that determines whether the command can execute in its current state.</summary>
        /// <param name="parameter">
        ///     Data used by the command. If the command does not require data to be
        ///     passed, this object can be set to <see langword="null" />.
        /// </param>
        /// <returns></returns>
        bool CanExecute(T parameter);

        /// <summary>Defines the asynchronous method to be called when the command is invoked.</summary>
        /// <param name="parameter">
        ///     Data used by the command. If the command does not require data to be
        ///     passed, this object can be set to <see langword="null" />.
        /// </param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        UniTask ExecuteAsync(T parameter);
    }
}