namespace Caliburn.Noesis.Samples.FlipView.ViewModels
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Media;
#endif

    /// <summary>A demonstration sub-screen view-model.</summary>
    /// <seealso cref="Caliburn.Noesis.Screen" />
    public class ChildViewModel : Screen
    {
        #region Constants and Fields

        private static readonly SolidColorBrush[] BrushesList =
            {
                Brushes.Brown,
                Brushes.DarkSlateBlue,
                Brushes.Chocolate,
                Brushes.Crimson,
                Brushes.DarkRed,
                Brushes.DarkGreen,
                Brushes.DarkCyan,
                Brushes.DarkGoldenrod,
                Brushes.DarkSlateGray,
                Brushes.Purple
            };

        private static int index;
        private Brush color;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ChildViewModel" /> class.</summary>
        public ChildViewModel()
        {
            DisplayName = $"{GetType().Name} #{index++}";

            Color = BrushesList[(index + BrushesList.Length) % BrushesList.Length];
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the color of this screen.</summary>
        /// <value>The color of this screen.</value>
        public Brush Color
        {
            get => this.color;
            set => Set(ref this.color, value);
        }

        #endregion
    }
}