#if UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
#else
using System.Windows.Controls;
#endif

namespace Caliburn.Noesis
{
    public partial class RootView: UserControl
    {
        public RootView()
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
