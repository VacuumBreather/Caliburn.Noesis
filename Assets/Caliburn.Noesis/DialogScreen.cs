namespace Caliburn.Noesis
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Extensions;

    /// <summary>A base class for dialog screens.</summary>
    public abstract class DialogScreen : Screen
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="DialogScreen" /> class.</summary>
        protected DialogScreen()
        {
            CloseDialogCommand = new AsyncRelayCommand<bool?>(TryCloseAsync, CanCloseDialog);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the command to close the dialog.</summary>
        public IAsyncCommand<bool?> CloseDialogCommand { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override sealed UniTask<bool> CanCloseAsync(CancellationToken cancellationToken =
                                                               default)
        {
            return UniTask.FromResult(true);
        }

        /// <inheritdoc />
        public override sealed async UniTask TryCloseAsync(bool? dialogResult = null)
        {
            using var _ = Logger.GetMethodTracer(dialogResult);

            if (!(Parent is DialogConductor dialogConductor))
            {
                throw new InvalidOperationException(
                    $"{this} must be conducted by a {nameof(DialogConductor)}.");
            }

            await dialogConductor.CloseDialogAsync(this, dialogResult, CancellationToken.None);
        }

        #endregion

        #region Protected Methods

        /// <summary>Override this to define when the <see cref="CloseDialogCommand" /> can be executed.</summary>
        /// <param name="dialogResult">The dialog result parameter of the <see cref="CloseDialogCommand" />.</param>
        /// <returns>
        ///     <c>true</c> if the dialog can be closed with the specified result; otherwise, <c>false</c>
        ///     .
        /// </returns>
        protected virtual bool CanCloseDialog(bool? dialogResult)
        {
            return true;
        }

        #endregion
    }
}