namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using Extensions;
    using global::Noesis;

    #endregion

    public static class DataTemplateManager
    {
        #region Public Methods

        public static void RegisterDataTemplate<TViewModel, TView>(ResourceDictionary dictionary)
            where TView : FrameworkElement
        {
            RegisterDataTemplate(typeof(TViewModel), typeof(TView), dictionary);
        }

        public static void RegisterDataTemplate(Type viewModelType, Type viewType, ResourceDictionary dictionary)
        {
            var dataTemplate = CreateTemplate(viewModelType, viewType);

            dictionary.Add(viewModelType, dataTemplate);
        }

        #endregion

        #region Private Methods

        private static DataTemplate CreateTemplate(Type viewModelType, Type viewType)
        {
            string xamlTemplate;

            if (viewType != null)
            {
                xamlTemplate = "<DataTemplate\n" +
                               "  xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n" +
                               "  xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n" +
                               $"  xmlns:vm=\"clr-namespace:{viewModelType.Namespace};assembly={viewModelType.Assembly.GetName().Name}\"\n" +
                               $"  xmlns:v=\"clr-namespace:{viewType.Namespace};assembly={viewType.Assembly.GetName().Name}\"\n" +
                               $"  DataType=\"{{x:Type vm:{viewModelType.Name}}}\">\n" +
                               $"    <v:{viewType.Name} />\n" +
                               "</DataTemplate>";
            }
            else if (viewModelType.IsDerivedFromOrImplements(typeof(DialogScreen)))
            {
                xamlTemplate = "<DataTemplate\n" +
                               "    xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n" +
                               $"    xmlns:vm=\"clr-namespace:{viewModelType.Namespace};assembly={viewModelType.Assembly.GetName().Name}\"\n" +
                               "    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n" +
                               "    xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"\n" +
                               "    xmlns:system=\"clr-namespace:System;assembly=mscorlib\"\n" +
                               "    DataType=\"{x:Type vm:DialogScreen}\">\n" +
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