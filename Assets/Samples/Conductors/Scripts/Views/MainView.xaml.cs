namespace Caliburn.Noesis.Samples.Conductors.Views
{
    using Extensions;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Controls;

#endif

    /// <summary />
    public partial class MainView : UserControl
    {
        #region Constructors and Destructors

        public MainView()
        {
            this.InitializeComponent();
        }

        #endregion
    }
}