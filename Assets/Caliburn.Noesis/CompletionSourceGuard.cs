namespace Caliburn.Noesis
{
    using System;
    using Cysharp.Threading.Tasks;

    /// <summary>
    ///     Provides a helper method to automatically handle the setting of a status on a
    ///     <see cref="UniTaskCompletionSource" /> after a provided guard goes out of scope.
    /// </summary>
    public static class TaskCompletion
    {
        #region Public Methods

        /// <summary>Creates a guarded <see cref="UniTaskCompletionSource" />.</summary>
        /// A reference to the
        /// <see cref="UniTaskCompletionSource" />
        /// to initialize and
        /// handle. Intended for
        /// use in a using block or statement.
        /// <returns>
        ///     An <see cref="IDisposable" /> which will set the result of the
        ///     <see cref="UniTaskCompletionSource" /> when disposed.
        /// </returns>
        /// <example>
        ///     <code>
        ///         using var _ = TaskCompletion.CreateGuard(out var completionSource);
        ///     </code>
        /// </example>
        /// <seealso cref="System.IDisposable" />
        public static IDisposable CreateGuard(out UniTaskCompletionSource completionSource)
        {
            var source = new UniTaskCompletionSource();

            completionSource = source;

            return new DisposableAction(() => source.TrySetResult());
        }

        #endregion
    }
}