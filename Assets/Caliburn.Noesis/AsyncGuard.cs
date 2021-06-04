namespace Caliburn.Noesis
{
    using System;
    using System.Threading;

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

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets a new token to track an asynchronous operation. Use as the <see cref="IDisposable" />
        ///     with <see cref="Extensions.UniTaskExtensions.Using" />.
        /// </summary>
        /// <returns>A new token to track an asynchronous operation.</returns>
        public IDisposable GetToken()
        {
            IncrementCounter();

            return new DisposableAction(DecrementCounter);
        }

        #endregion

        #region Private Methods

        private void DecrementCounter()
        {
            Interlocked.Decrement(ref this.asyncCounter);
            RaiseIsOngoingChanged();
        }

        private void IncrementCounter()
        {
            Interlocked.Increment(ref this.asyncCounter);
            RaiseIsOngoingChanged();
        }

        private void RaiseIsOngoingChanged()
        {
            IsOngoingChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}