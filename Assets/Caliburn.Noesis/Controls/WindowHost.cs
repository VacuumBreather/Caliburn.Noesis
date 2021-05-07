namespace Caliburn.Noesis.Controls
{
    #region Using Directives

#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;

#endif

    #endregion

    /// <summary>Used to host windows.</summary>
    /// <seealso cref="ItemsControl" />
    public class WindowHost : ItemsControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes the <see cref="WindowHost" /> class.</summary>
        static WindowHost()
        {
            var defaultPanelTemplate = GetItemsPanelTemplate();
            ItemsPanelProperty.OverrideMetadata(
                typeof(WindowHost),
                new FrameworkPropertyMetadata(defaultPanelTemplate));
        }

        #endregion

#if !UNITY_5_5_OR_NEWER
        #region Protected Methods

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new Window();
        }

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return false;
        }

        #endregion
#endif

        #region Private Methods

        private static ItemsPanelTemplate GetItemsPanelTemplate()
        {
            var xaml =
                $@"<ItemsPanelTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                                       xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                                       xmlns:cal='clr-namespace:{typeof(WindowCanvas).Namespace};assembly={typeof(WindowCanvas).Assembly.GetName().Name}'>
                     <cal:{nameof(WindowCanvas)} />
                   </ItemsPanelTemplate>";

            return XamlReader.Parse(xaml) as ItemsPanelTemplate;
        }

        #endregion
    }
}