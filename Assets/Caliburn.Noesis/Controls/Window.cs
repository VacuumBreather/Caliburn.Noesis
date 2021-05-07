namespace Caliburn.Noesis.Controls
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows.Controls;
#endif
    using System.Windows.Input;

    /// <summary>A <see cref="ContentControl" /> used as a container for window content.</summary>
    /// <seealso cref="ContentControl" />
    public class Window : ContentControl
    {
        #region Constants and Fields

        private readonly WindowCanvas parentCanvas;

        #endregion

        #region Constructors and Destructors

        /// <inheritdoc />
        public Window(WindowCanvas parentCanvas)
        {
            this.parentCanvas = parentCanvas;
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.parentCanvas.BringToFront(this);
            base.OnMouseLeftButtonDown(e);
        }

        #endregion
    }
}