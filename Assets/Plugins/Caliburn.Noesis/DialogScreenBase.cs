namespace Caliburn.Noesis
{
    #region Using Directives

    using System.Threading;
    using System.Windows.Input;
    using Cysharp.Threading.Tasks;

    #endregion

    /// <summary>
    ///     Base class for dialog screens.
    /// </summary>
    public abstract class DialogScreenBase : Screen
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Creates an instance of <see cref="DialogScreenBase" />.
        /// </summary>
        protected DialogScreenBase()
        {
            CloseDialogCommand = new RelayCommand<bool?>(result => TryCloseAsync(result).Forget());
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The command to close the dialog.
        /// </summary>
        public ICommand CloseDialogCommand { get; }

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