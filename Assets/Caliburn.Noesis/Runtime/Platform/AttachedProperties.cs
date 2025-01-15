#if UNITY_5_5_OR_NEWER
using global::Noesis;
#else
using System.Windows;
#endif

namespace Caliburn.Noesis
{
    public static class AttachedProperties
    {
        public static readonly DependencyProperty ServiceLocatorProperty = DependencyProperty.RegisterAttached(
            "ServiceLocator",
            typeof(IServiceLocator),
            typeof(AttachedProperties),
            new FrameworkPropertyMetadata(default(IServiceLocator), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetServiceLocator(DependencyObject element, IServiceLocator value)
        {
            element.SetValue(ServiceLocatorProperty, value);
        }

        public static IServiceLocator GetServiceLocator(DependencyObject element)
        {
            return (IServiceLocator)element.GetValue(ServiceLocatorProperty);
        }
    }
}
