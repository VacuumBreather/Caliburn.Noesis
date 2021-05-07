namespace Caliburn.Noesis
{
    using System;
    using System.Linq;
    using Extensions;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows;
    using System.Windows.Markup;
#endif

    #endregion

    /// <summary>Creates data templates for view-models.</summary>
    [PublicAPI]
    public static class DataTemplateManager
    {
        #region Private Properties

        private static ILogger Logger => LogManager.FrameworkLogger;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates a data template for the specified view-model view pair and registers it in the
        ///     dictionary.
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
            using var _ = Logger.GetMethodTracer(viewModelType, viewType, dictionary, onRegister);

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
        ///     Creates data templates for all view-model types found in the specified
        ///     <see cref="AssemblySource" />.
        /// </summary>
        /// <param name="viewLocator">The view locator used to map view-model to view types.</param>
        /// <param name="resourceDictionary">
        ///     The <see cref="ResourceDictionary" /> to register the data
        ///     templates in.
        /// </param>
        /// <param name="onRegister">A callback to execute whenever a data template was registered.</param>
        public static void RegisterDataTemplates(ViewLocator viewLocator,
                                                 ResourceDictionary resourceDictionary,
                                                 Action<DataTemplate> onRegister)
        {
            using var _ = Logger.GetMethodTracer(viewLocator, resourceDictionary, onRegister);

            viewLocator.AssemblySource.ViewModelTypes
                       .Select(vmType => (vmType, viewLocator.LocateTypeForModelType(vmType)))
                       .ForEach(
                           ((Type vmType, Type vType) mapping) => RegisterDataTemplate(
                               mapping.vmType,
                               mapping.vType,
                               resourceDictionary,
                               onRegister));
        }

        #endregion

        #region Private Methods

        private static DataTemplate CreateTemplate(Type viewModelType, Type viewType)
        {
            using var _ = Logger.GetMethodTracer(viewModelType, viewType);

            string xamlTemplate;

            if (viewType != null)
            {
                Logger.LogDebug("Creating data template for {ViewModel}", viewModelType);

                xamlTemplate = "<DataTemplate\n" +
                               "  xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n" +
                               "  xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n" +
                               $"  xmlns:vm=\"clr-namespace:{viewModelType.Namespace};assembly={viewModelType.Assembly.GetName().Name}\"\n" +
                               $"  xmlns:cal=\"clr-namespace:{typeof(View).Namespace};assembly={typeof(View).Assembly.GetName().Name}\"\n" +
                               $"  DataType=\"{{x:Type vm:{viewModelType.Name}}}\">\n" +
                               "    <ContentPresenter cal:View.IsGenerated=\"True\" cal:View.Model=\"{Binding}\" />\n" +
                               "</DataTemplate>";
            }
            else if (viewModelType.IsDerivedFromOrImplements(typeof(DialogScreen)))
            {
                Logger.LogWarning("Creating placeholder dialog template for {ViewModel}", viewModelType);

                xamlTemplate = "<DataTemplate\n" +
                               "    xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n" +
                               $"    xmlns:vm=\"clr-namespace:{viewModelType.Namespace};assembly={viewModelType.Assembly.GetName().Name}\"\n" +
                               "    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n" +
                               "    xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"\n" +
                               "    xmlns:system=\"clr-namespace:System;assembly=mscorlib\"\n" +
                               $"    DataType=\"{{x:Type vm:{viewModelType.Name}}}\">\n" +
                               "    <Grid\n" +
                               "      Control.FontSize=\"20\"\n" +
                               "      Control.FontWeight=\"Bold\">\n" +
                               "        <Border\n" +
                               "            HorizontalAlignment=\"Center\"\n" +
                               "            VerticalAlignment=\"Center\"\n" +
                               "            Background=\"Magenta\"\n" +
                               "            BorderBrush=\"Black\"\n" +
                               "            BorderThickness=\"2\">\n" +
                               "            <StackPanel>\n" +
                               "                <Grid Height=\"100\">\n" +
                               "                    <TextBlock Foreground=\"#222222\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\">\n" +
                               $"                        Placeholder view for {viewModelType.Name}.\n" +
                               "                    </TextBlock>\n" +
                               "                </Grid>\n" +
                               "                <UniformGrid Columns=\"3\">\n" +
                               "                    <Button\n" +
                               "                        Width=\"200\"\n" +
                               "                        Height=\"50\"\n" +
                               "                        Margin=\"10\"\n" +
                               $"                        Command=\"{{Binding {nameof(DialogScreen.CloseDialogCommand)}, Mode=OneTime}}\">\n" +
                               "                        <Button.CommandParameter>\n" +
                               "                            <system:Boolean>True</system:Boolean>\n" +
                               "                        </Button.CommandParameter>\n" +
                               "                        Yes\n" +
                               "                    </Button>\n" +
                               "                    <Button\n" +
                               "                        Width=\"200\"\n" +
                               "                        Height=\"50\"\n" +
                               "                        Margin=\"10\"\n" +
                               $"                        Command=\"{{Binding {nameof(DialogScreen.CloseDialogCommand)}, Mode=OneTime}}\">\n" +
                               "                        <Button.CommandParameter>\n" +
                               "                            <system:Boolean>False</system:Boolean>\n" +
                               "                        </Button.CommandParameter>\n" +
                               "                        No\n" +
                               "                    </Button>\n" +
                               "                    <Button\n" +
                               "                        Width=\"200\"\n" +
                               "                        Height=\"50\"\n" +
                               "                        Margin=\"10\"\n" +
                               $"                        Command=\"{{Binding {nameof(DialogScreen.CloseDialogCommand)}, Mode=OneTime}}\">\n" +
                               "                        <Button.CommandParameter>\n" +
                               "                            <x:Null />\n" +
                               "                        </Button.CommandParameter>\n" +
                               "                        Cancel\n" +
                               "                    </Button>\n" +
                               "                </UniformGrid>\n" +
                               "            </StackPanel>\n" +
                               "        </Border>\n" +
                               "    </Grid>\n" +
                               "</DataTemplate>";
            }
            else
            {
                Logger.LogWarning("Creating placeholder data template for {ViewModel}", viewModelType);

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

        #endregion
    }
}