namespace Caliburn.Noesis
{
    using System;

    /// <summary>Executes an action when disposed.</summary>
    public sealed class DisposableAction : IDisposable
    {
        #region Constants and Fields

        private Action action;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="DisposableAction" /> class.</summary>
        /// <param name="action">The action to execute on dispose.</param>
        public DisposableAction(Action action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>Executes the supplied action.</summary>
        public void Dispose()
        {
            if (this.action is null)
            {
                return;
            }

            this.action();
            this.action = null;
        }

        #endregion
    }
}