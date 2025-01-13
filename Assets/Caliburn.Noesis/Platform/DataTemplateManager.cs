namespace Caliburn.Noesis
{
    using System;
    using System.Linq;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows;
    using System.Windows.Markup;
#endif

    /// <summary>
    /// Creates data templates for view-models.
    /// </summary>
    public static class DataTemplateManager
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(Screen));

        /// <summary>
        /// Creates a data template for the specified view-model view pair and registers it in the dictionary.
        /// </summary>
        /// <param name="viewModelType">The view-model type.</param>
        /// <param name="viewType">The view type.</param>
        /// <param name="dictionary">The <see cref="ResourceDictionary" /> to register the data template in.</param>
        /// <param name="onRegister">A callback to execute when the data template was registered.</param>
        public static void RegisterDataTemplate(Type viewModelType,
                                                Type viewType,
                                                ResourceDictionary dictionary,
                                                Action<DataTemplate> onRegister)
        {
            var dataTemplate = CreateTemplate(viewModelType, viewType);
            onRegister(dataTemplate);
            dataTemplate.Seal();

#if UNITY_5_5_OR_NEWER
            dictionary[viewModelType] = dataTemplate;
#else
            dictionary[dataTemplate.DataTemplateKey!] = dataTemplate;
#endif
        }

        /// <summary>
        /// Creates data templates for all view-model types found in the specified <see cref="AssemblySource" />.
        /// </summary>
        /// <param name="viewLocator">The view locator used to map view-model to view types.</param>
        /// <param name="resourceDictionary">The <see cref="ResourceDictionary" /> to register the data templates in.</param>
        /// <param name="onRegister">A callback to execute whenever a data template was registered.</param>
        public static void RegisterDataTemplates(ViewLocator viewLocator,
                                                 ResourceDictionary resourceDictionary,
                                                 Action<DataTemplate> onRegister)
        {
            viewLocator.AssemblySource.ViewModelTypes
                       .Select(vmType => (vmType, viewLocator.LocateTypeForModelType(vmType)))
                       .Apply(
                           ((Type vmType, Type vType) mapping) => RegisterDataTemplate(
                               mapping.vmType,
                               mapping.vType,
                               resourceDictionary,
                               onRegister));
        }

        private static DataTemplate CreateTemplate(Type viewModelType, Type viewType)
        {
            string xamlTemplate;

            if (viewType != null)
            {
                Log.Info("Creating data template for {ViewModel}", viewModelType);

                xamlTemplate = "<DataTemplate\n" +
                               "  xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n" +
                               "  xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n" +
                               $"  xmlns:vm=\"clr-namespace:{viewModelType.Namespace};assembly={viewModelType.Assembly.GetName().Name}\"\n" +
                               $"  xmlns:cal=\"clr-namespace:{typeof(View).Namespace};assembly={typeof(View).Assembly.GetName().Name}\"\n" +
                               $"  DataType=\"{{x:Type vm:{viewModelType.Name}}}\">\n" +
                               "    <ContentPresenter cal:View.IsGenerated=\"True\" cal:View.Model=\"{Binding}\" />\n" +
                               "</DataTemplate>";
            }
            else
            {
                Log.Warn("Creating placeholder data template for {ViewModel}", viewModelType);

                xamlTemplate = "<DataTemplate\n" +
                               "  xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n" +
                               "  xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n" +
                               $"  xmlns:vm=\"clr-namespace:{viewModelType.Namespace};assembly={viewModelType.Assembly.GetName().Name}\"\n" +
                               $"  DataType=\"{{x:Type vm:{viewModelType.Name}}}\">\n" +
                               "  <Border Background=\"Magenta\">\n" +
                               $"    <TextBlock Margin=\"5\" Foreground=\"#222222\" FontWeight=\"Bold\" FontSize=\"20\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\" Text=\"Placeholder view for {viewModelType.Name}.\" />\n" +
                               "  </Border>\n</DataTemplate>";
            }

            var template = (DataTemplate)XamlReader.Parse(xamlTemplate);

            return template;
        }
    }
}
