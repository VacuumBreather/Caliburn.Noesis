namespace Caliburn.Noesis.Samples.FlipView.ViewModels
{
    using System.Windows.Media;

    /// <summary>A demonstration sub-screen view-model.</summary>
    /// <seealso cref="Caliburn.Noesis.Screen" />
    public class ChildViewModel : Screen
    {
        #region Constants and Fields

        private static readonly SolidColorBrush[] Brushes =
            {
                System.Windows.Media.Brushes.Brown,
                System.Windows.Media.Brushes.DarkSlateBlue,
                System.Windows.Media.Brushes.Chocolate,
                System.Windows.Media.Brushes.Crimson,
                System.Windows.Media.Brushes.DarkRed,
                System.Windows.Media.Brushes.DarkGreen,
                System.Windows.Media.Brushes.DarkCyan,
                System.Windows.Media.Brushes.DarkGoldenrod,
                System.Windows.Media.Brushes.DarkSlateGray,
                System.Windows.Media.Brushes.Purple
            };

        private static int index;
        private Brush color;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ChildViewModel" /> class.</summary>
        public ChildViewModel()
        {
            DisplayName = $"{GetType().Name} #{index++}";

            Color = Brushes[(index + Brushes.Length) % Brushes.Length];
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