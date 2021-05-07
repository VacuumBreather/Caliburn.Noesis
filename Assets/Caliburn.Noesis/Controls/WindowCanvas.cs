namespace Caliburn.Noesis.Controls
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Controls;
#endif
    using System.Linq;
    using Extensions;

    /// <summary>Used to host windows.</summary>
    /// <seealso cref="Canvas" />
    public class WindowCanvas : Canvas
    {
        #region Public Methods

        /// <summary>Brings the specified <see cref="Window" /> to the front of the canvas.</summary>
        /// <param name="window">The <see cref="Window" /> to bring to the front.</param>
        public void BringToFront(Window window)
        {
            if (window is null)
            {
                return;
            }

            var maxZIndex = Children.Cast<Window>()
                                    .Max(child => (int)child.GetValue(ZIndexProperty));

            if (maxZIndex != int.MaxValue)
            {
                SetZIndex(window, ++maxZIndex);
            }
            else
            {
                // The zIndex is probably never going to get this high but let's make sure.
                var zIndex = 0;

                Children.Cast<Window>()
                        .OrderBy(GetZIndex)
                        .ForEach(child => SetZIndex(child, zIndex++));

                window.SetValue(ZIndexProperty, zIndex);
            }
        }

        #endregion
    }
}