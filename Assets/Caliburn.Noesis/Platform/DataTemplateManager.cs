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
        private static readonly ILog Log = LogManager.GetLog(typeof(DataTemplateManager));

        /// <summary>
        /// Creates a data template for the specified view-model view pair and registers it in the dictionary.
        /// </summary>
        /// <param name="viewModelType">The view-model type.</param>
        /// <param name="dictionary">The <see cref="ResourceDictionary" /> to register the data template in.</param>
        /// <param name="onRegister">A callback to execute when the data template was registered.</param>
        public static void RegisterDataTemplate(Type viewModelType,
                                                ResourceDictionary dictionary,
                                                Action<DataTemplate> onRegister)
        {
            var dataTemplate = CreateTemplate(viewModelType);
            onRegister(dataTemplate);
            dataTemplate.Seal();

#if UNITY_5_5_OR_NEWER
            dictionary[typeof(PropertyChangedBase)] = dataTemplate;
#else
            dictionary[dataTemplate.DataTemplateKey!] = dataTemplate;
#endif
        }

        private static DataTemplate CreateTemplate(Type viewModelType)
        {
            string xamlTemplate = "<DataTemplate\n" +
                                  "  xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n" +
                                  "  xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n" +
                                  $"  xmlns:vm=\"clr-namespace:{viewModelType.Namespace};assembly={viewModelType.Assembly.GetName().Name}\"\n" +
                                  $"  xmlns:cal=\"clr-namespace:{typeof(View).Namespace};assembly={typeof(View).Assembly.GetName().Name}\"\n" +
                                  $"  DataType=\"{{x:Type vm:{viewModelType.Name}}}\">\n" +
                                  "    <ContentPresenter cal:View.IsGenerated=\"True\" cal:View.Model=\"{Binding}\" />\n" +
                                  "</DataTemplate>";

            var template = (DataTemplate)XamlReader.Parse(xamlTemplate);

            return template;
        }
    }
}
