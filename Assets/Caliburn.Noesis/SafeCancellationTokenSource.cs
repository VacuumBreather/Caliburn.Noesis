namespace Caliburn.Noesis
{
    using System.Threading;

    /// <summary>A <see cref="CancellationTokenSource" /> which can be checked for its disposed state.</summary>
    public class SafeCancellationTokenSource : CancellationTokenSource
    {
        #region Public Properties

        /// <summary>Gets a value indicating if this instance has been disposed.</summary>
        public bool IsDisposed { get; private set; }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            IsDisposed = true;
        }

        #endregion
    }
}