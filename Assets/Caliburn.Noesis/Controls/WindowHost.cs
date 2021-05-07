namespace Caliburn.Noesis.Controls
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

#endif

    /// <summary>Used to host windows.</summary>
    /// <seealso cref="ItemsControl" />
    public class WindowHost : ItemsControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes the <see cref="WindowHost" /> class.</summary>
        static WindowHost()
        {
            ItemsPanelProperty.OverrideMetadata(
                typeof(WindowHost),
                new FrameworkPropertyMetadata(GetDefaultItemsPanelTemplate()));
        }

        #endregion

        #region Private Methods

        private static ItemsPanelTemplate GetDefaultItemsPanelTemplate()
        {
            var xaml =
                $@"<ItemsPanelTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                                       xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                                       xmlns:cal='clr-namespace:{typeof(WindowCanvas).Namespace};assembly={typeof(WindowCanvas).Assembly.GetName().Name}'>
                     <cal:{nameof(WindowCanvas)} />
                   </ItemsPanelTemplate>";

            var template = XamlReader.Parse(xaml) as ItemsPanelTemplate;
            template!.Seal();

            return template;
        }

        #endregion

#if !UNITY_5_5_OR_NEWER

        #region Protected Methods

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            var canvas = this.GetVisualChild<WindowCanvas>();

            return new Window(canvas);
        }

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return false;
        }

        #endregion

#endif
    }
}