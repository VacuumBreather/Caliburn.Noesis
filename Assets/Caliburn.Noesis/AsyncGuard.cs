namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using System.Threading;

    #endregion

    /// <summary>Helper class to keep track of ongoing asynchronous operations.</summary>
    /// <example>
    ///     <code>
    ///     await OperationAsync().Using(asyncGuard);
    ///     </code>
    /// </example>
    public sealed class AsyncGuard
    {
        #region Constants and Fields

        private int asyncCounter;

        #endregion

        #region Public Events and Delegates

        /// <summary>Occurs when the number of ongoing operations has changed.</summary>
        public event EventHandler IsOngoingChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether any asynchronous operations are still ongoing (i.e. using
        ///     tokens).
        /// </summary>
        /// <value><c>true</c> if any asynchronous operations are still ongoing; otherwise, <c>false</c>.</value>
        public bool IsOngoing => this.asyncCounter != 0;

        /// <summary>
        ///     Gets a new token to track an asynchronous operation. Use as the <see cref="IDisposable" />
        ///     with <see cref="Extensions.UniTaskExtensions.Using" />.
        /// </summary>
        /// <value>A new token to track an asynchronous operation.</value>
        public IDisposable Token => new AsyncToken(this);

        #endregion

        #region Private Methods

        private void RaiseIsOngoingChanged()
        {
            IsOngoingChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Nested Types

        private class AsyncToken : IDisposable
        {
            private readonly AsyncGuard context;

            public AsyncToken(AsyncGuard context)
            {
                this.context = context;
                Interlocked.Increment(ref this.context.asyncCounter);
                this.context.RaiseIsOngoingChanged();
            }

            public void Dispose()
            {
                Interlocked.Decrement(ref this.context.asyncCounter);
                this.context.RaiseIsOngoingChanged();
            }
        }

        #endregion
    }
}