namespace Caliburn.Noesis
{
    using System;
    using Cysharp.Threading.Tasks;

    /// <summary>
    ///     Automatically handles the setting of a status on a <see cref="UniTaskCompletionSource" />
    ///     after it goes out of scope. Intended for use in a using block or statement.
    /// </summary>
    /// <example>
    ///     <code>
    /// using var guard = new CompletionSourceGuard(out this.tcs);
    /// </code>
    /// </example>
    /// <seealso cref="System.IDisposable" />
    public class CompletionSourceGuard : IDisposable
    {
        #region Constants and Fields

        private readonly UniTaskCompletionSource source;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="CompletionSourceGuard" /> class.</summary>
        /// <param name="source">
        ///     A reference to the <see cref="UniTaskCompletionSource" /> to initialize and
        ///     handle.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is <c>null</c>.</exception>
        public CompletionSourceGuard(out UniTaskCompletionSource source)
        {
            source = new UniTaskCompletionSource();
            this.source = source;
        }

        #endregion

        #region IDisposable Implementation

        /// <inheritdoc />
        public void Dispose()
        {
            this.source.TrySetResult();
        }

        #endregion
    }
}