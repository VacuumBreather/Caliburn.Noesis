#if UNITY_5_3_OR_NEWER
    #define NOESIS
    using Noesis;
#else
    using System.Windows;
    using System.Windows.Controls;
#endif

namespace Testing
{
    public partial class Root: Noesis.UserControl
    {
        public Root()
        {
            InitializeComponent();
        }

    #if NOESIS
        private void InitializeComponent()
        {
            NoesisUnity.LoadComponent(this);
        }
    #endif
    };
}
