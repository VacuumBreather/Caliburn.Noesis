namespace Caliburn.Noesis
{
    using System;
    using JetBrains.Annotations;

    /// <summary>Provides a disposable that does nothing when disposed.</summary>
    [PublicAPI]
    public static class Disposable
    {
        #region Public Properties

        /// <summary>Gets a disposable that does nothing when disposed.</summary>
        public static IDisposable Empty => EmptyDisposable.Instance;

        #endregion

        #region Nested Types

        private class EmptyDisposable : IDisposable
        {
            public static readonly IDisposable Instance = new EmptyDisposable();

            public void Dispose()
            {
            }
        }

        #endregion
    }
}