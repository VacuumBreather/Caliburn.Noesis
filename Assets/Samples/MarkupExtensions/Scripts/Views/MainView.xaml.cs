namespace Caliburn.Noesis.Samples.MarkupExtensions.Views
{
    using Extensions;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// The main view-model of the MarkupExtensions sample.
    /// </summary>
    /// <seealso cref="UserControl" />
    public partial class MainView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            this.InitializeComponent();
        }
    }
}