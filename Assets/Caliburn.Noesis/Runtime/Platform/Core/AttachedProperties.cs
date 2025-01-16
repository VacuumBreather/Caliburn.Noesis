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

        public static readonly DependencyProperty ViewLocatorProperty = DependencyProperty.RegisterAttached(
            "ViewLocator",
            typeof(ViewLocator),
            typeof(AttachedProperties),
            new FrameworkPropertyMetadata(default(ViewLocator), FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty ViewModelLocatorProperty = DependencyProperty.RegisterAttached(
            "ViewModelLocator",
            typeof(ViewModelLocator),
            typeof(AttachedProperties),
            new FrameworkPropertyMetadata(default(ViewModelLocator), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetServiceLocator(DependencyObject element, IServiceLocator value)
        {
            element.SetValue(ServiceLocatorProperty, value);
        }

        public static IServiceLocator GetServiceLocator(DependencyObject element)
        {
            return (IServiceLocator)element.GetValue(ServiceLocatorProperty);
        }

        public static void SetViewLocator(DependencyObject element, ViewLocator value)
        {
            element.SetValue(ViewLocatorProperty, value);
        }

        public static ViewLocator GetViewLocator(DependencyObject element)
        {
            return (ViewLocator)element.GetValue(ViewLocatorProperty);
        }

        public static void SetViewModelLocator(DependencyObject element, ViewModelLocator value)
        {
            element.SetValue(ViewModelLocatorProperty, value);
        }

        public static ViewModelLocator GetViewModelLocator(DependencyObject element)
        {
            return (ViewModelLocator)element.GetValue(ViewModelLocatorProperty);
        }
    }
}
