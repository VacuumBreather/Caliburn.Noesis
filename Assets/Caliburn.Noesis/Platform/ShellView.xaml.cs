namespace Caliburn.Noesis
{
#if UNITY_5_5_OR_NEWER
    using Extensions;
    using global::Noesis;

#else
    using System.Windows.Controls;

#endif

    /// <summary>Interaction logic for ShellView.xaml</summary>
    public partial class ShellView : UserControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ShellView" /> class.</summary>
        public ShellView()
        {
            this.InitializeComponent();
        }

        #endregion
    }
}