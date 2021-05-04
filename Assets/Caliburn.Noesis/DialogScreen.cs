namespace Caliburn.Noesis
{
    #region Using Directives

    using System.Threading;
    using Cysharp.Threading.Tasks;

    #endregion

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
        public AsyncRelayCommand<bool?> CloseDialogCommand { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override async UniTask TryCloseAsync(bool? dialogResult = null)
        {
            if (Parent is DialogConductor conductor)
            {
                await conductor.CloseDialogAsync(this, dialogResult, CancellationToken.None);
            }
            else
            {
                Logger.Warn($"{this} should be hosted in a {nameof(DialogConductor)}.");
                await base.TryCloseAsync(dialogResult);
            }
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