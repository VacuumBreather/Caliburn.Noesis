namespace Caliburn.Noesis
{
    using JetBrains.Annotations;

    /// <summary>A base class for a screen that is displayed as a draggable window.</summary>
    /// <seealso cref="Caliburn.Noesis.Screen" />
    public abstract class WindowScreen : Screen
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="WindowScreen" /> class.</summary>
        protected WindowScreen()
        {
            CloseWindowCommand = new AsyncRelayCommand<bool?>(TryCloseAsync);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the command to close the window.</summary>
        [UsedImplicitly]
        public IAsyncCommand<bool?> CloseWindowCommand { get; }

        #endregion
    }
}