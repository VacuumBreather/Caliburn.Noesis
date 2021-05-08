namespace Caliburn.Noesis.Samples.Conductors.Controls
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
#endif
    using System;

    /// <summary>Control for navigating back and forth between its children.</summary>
    /// <seealso cref="Selector" />
    [TemplatePart(Name = BackButtonName, Type = typeof(Button))]
    [TemplatePart(Name = ForwardButtonName, Type = typeof(Button))]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(FlipViewItem))]
    public class FlipView : Selector
    {
        private const string BackButtonName = "PART_BackButton";
        private const string ForwardButtonName = "PART_ForwardButton";

        private Button backButton;
        private Button forwardButton;

        public static readonly DependencyProperty ContextItemTemplateProperty = DependencyProperty.Register(
            nameof(ContextItemTemplate),
            typeof(DataTemplate),
            typeof(FlipView),
            new PropertyMetadata(default(DataTemplate)));

        public DataTemplate ContextItemTemplate
        {
            get => (DataTemplate)GetValue(ContextItemTemplateProperty);
            set => SetValue(ContextItemTemplateProperty, value);
        }

        public static readonly DependencyProperty ContextItemContainerStyleProperty = DependencyProperty.Register(
            nameof(ContextItemContainerStyle),
            typeof(Style),
            typeof(FlipView),
            new PropertyMetadata(default(Style)));

        public Style ContextItemContainerStyle
        {
            get => (Style)GetValue(ContextItemContainerStyleProperty);
            set => SetValue(ContextItemContainerStyleProperty, value);
        }

        public static readonly DependencyProperty IsCyclingProperty = DependencyProperty.Register(
            nameof(IsCycling),
            typeof(bool),
            typeof(FlipView),
            new PropertyMetadata(default(bool)));

        public bool IsCycling
        {
            get => (bool)GetValue(IsCyclingProperty);
            set => SetValue(IsCyclingProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header),
            typeof(object),
            typeof(FlipView),
            new PropertyMetadata(default(object)));

        public object Header
        {
            get => (object)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            nameof(HeaderTemplate),
            typeof(DataTemplate),
            typeof(FlipView),
            new PropertyMetadata(default(DataTemplate)));

        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FlipViewItem();
        }

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return false;
        }

        /// <inheritdoc />
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            Header = SelectedItem;
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.backButton = GetTemplateChild(BackButtonName) as Button;
            this.forwardButton = GetTemplateChild(ForwardButtonName) as Button;

            if (this.backButton != null)
            {
                this.backButton.Click += OnBackButtonClick;
            }

            if (this.forwardButton != null)
            {
                this.forwardButton.Click += OnForwardButtonClick;
            }
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            if (IsCycling)
            {
                SelectedIndex = (--SelectedIndex + Items.Count) % Items.Count;
            }
            else
            {
                SelectedIndex = Math.Max(0, --SelectedIndex);
            }
        }

        private void OnForwardButtonClick(object sender, RoutedEventArgs e)
        {
            if (IsCycling)
            {
                SelectedIndex = (++SelectedIndex) % Items.Count;
            }
            else
            {
                SelectedIndex = Math.Min(Items.Count - 1, ++SelectedIndex);
            }
        }
    }
}