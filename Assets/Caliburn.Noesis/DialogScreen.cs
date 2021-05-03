namespace Caliburn.Noesis
{
    #region Using Directives

    using System.Threading;
    using Cysharp.Threading.Tasks;

    #endregion

    /// <summary>
    ///     A base class for dialog screens.
    /// </summary>
    public abstract class DialogScreen : Screen
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DialogScreen" /> class.
        /// </summary>
        protected DialogScreen()
        {
            CloseDialogCommand = new RelayCommand<bool?>(result => TryCloseAsync(result).Forget());
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the command to close the dialog.
        /// </summary>
        public RelayCommand<bool?> CloseDialogCommand { get; protected set; }

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
    }
}