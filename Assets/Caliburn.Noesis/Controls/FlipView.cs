namespace Caliburn.Noesis.Controls
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
#endif
    using System;

    /// <summary>
    ///     Represents an items control that displays one item at a time, and can allows forwards and
    ///     backwards traversing its collection of items.
    /// </summary>
    /// <seealso cref="Selector" />
    [TemplatePart(Name = TemplatePartNameBackButton, Type = typeof(Button))]
    [TemplatePart(Name = TemplatePartNameForwardButton, Type = typeof(Button))]
    [StyleTypedProperty(
        Property = nameof(ItemContainerStyle),
        StyleTargetType = typeof(FlipViewItem))]
    public class FlipView : Selector
    {
        #region Constants and Fields

        private const string TemplatePartNameBackButton = "PART_BackButton";
        private const string TemplatePartNameForwardButton = "PART_ForwardButton";

        /// <summary>The ButtonStyle property.</summary>
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(
            nameof(ButtonStyle),
            typeof(Style),
            typeof(FlipView),
            new PropertyMetadata(default(Style)));

        /// <summary>The Header property.</summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header),
            typeof(object),
            typeof(FlipView),
            new PropertyMetadata(default(object)));

        /// <summary>The HeaderTemplate property.</summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                nameof(HeaderTemplate),
                typeof(DataTemplate),
                typeof(FlipView),
                new PropertyMetadata(default(DataTemplate)));

        /// <summary>The IsCycling property.</summary>
        public static readonly DependencyProperty IsCyclingProperty = DependencyProperty.Register(
            nameof(IsCycling),
            typeof(bool),
            typeof(FlipView),
            new PropertyMetadata(default(bool)));

        /// <summary>The ItemsListItemContainerStyle property.</summary>
        public static readonly DependencyProperty ItemsListItemContainerStyleProperty =
            DependencyProperty.Register(
                nameof(ItemsListItemContainerStyle),
                typeof(Style),
                typeof(FlipView),
                new PropertyMetadata(default(Style)));

        /// <summary>The ItemsListItemsPanel property.</summary>
        public static readonly DependencyProperty ItemsListItemsPanelProperty =
            DependencyProperty.Register(
                nameof(ItemsListItemsPanel),
                typeof(ItemsPanelTemplate),
                typeof(FlipView),
                new PropertyMetadata(default(ItemsPanelTemplate)));

        /// <summary>The ItemsListItemTemplate property.</summary>
        public static readonly DependencyProperty ItemsListItemTemplateProperty =
            DependencyProperty.Register(
                nameof(ItemsListItemTemplate),
                typeof(DataTemplate),
                typeof(FlipView),
                new PropertyMetadata(default(DataTemplate)));

        /// <summary>The NextItem property.</summary>
        public static readonly DependencyProperty NextItemProperty = DependencyProperty.Register(
            nameof(NextItem),
            typeof(object),
            typeof(FlipView),
            new PropertyMetadata(default(object)));

        /// <summary>The PreviewItemTemplate property.</summary>
        public static readonly DependencyProperty PreviewItemTemplateProperty =
            DependencyProperty.Register(
                nameof(PreviewItemTemplate),
                typeof(DataTemplate),
                typeof(FlipView),
                new PropertyMetadata(default(DataTemplate)));

        /// <summary>The PreviousItem property.</summary>
        public static readonly DependencyProperty PreviousItemProperty =
            DependencyProperty.Register(
                nameof(PreviousItem),
                typeof(object),
                typeof(FlipView),
                new PropertyMetadata(default(object)));

        private Button backButton;
        private Button forwardButton;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes the <see cref="FlipView" /> class.</summary>
        static FlipView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(FlipView),
                new FrameworkPropertyMetadata(typeof(FlipView)));
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the style of the Back and Forwards buttons.</summary>
        /// <value>The style of the Back and Forwards buttons.</value>
        public Style ButtonStyle
        {
            get => (Style)GetValue(ButtonStyleProperty);
            set => SetValue(ButtonStyleProperty, value);
        }

        /// <summary>Gets or sets the header.</summary>
        /// <value>The header.</value>
        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>Gets or sets the <see cref="DataTemplate" /> for the header.</summary>
        /// <value>The <see cref="DataTemplate" /> for the header.</value>
        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the <see cref="FlipView" /> should go back to the
        ///     first item after hitting the last one, and vice-versa.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="FlipView" /> should go back to the first item after hitting
        ///     the last one; otherwise, <c>false</c>.
        /// </value>
        public bool IsCycling
        {
            get => (bool)GetValue(IsCyclingProperty);
            set => SetValue(IsCyclingProperty, value);
        }

        /// <summary>Gets or sets the container style of the items shown in the list of all items.</summary>
        /// <value>The container style of the items shown in the list of all items.</value>
        public Style ItemsListItemContainerStyle
        {
            get => (Style)GetValue(ItemsListItemContainerStyleProperty);
            set => SetValue(ItemsListItemContainerStyleProperty, value);
        }

        /// <summary>Gets or sets the <see cref="ItemsPanelTemplate" /> for the list of all items.</summary>
        /// <value>The <see cref="ItemsPanelTemplate" /> for the list of all items.</value>
        public ItemsPanelTemplate ItemsListItemsPanel
        {
            get => (ItemsPanelTemplate)GetValue(ItemsListItemsPanelProperty);
            set => SetValue(ItemsListItemsPanelProperty, value);
        }

        /// <summary>Gets or sets the <see cref="DataTemplate" /> for items in the list of all items.</summary>
        /// <value>The <see cref="DataTemplate" /> for items in the list of all items.</value>
        public DataTemplate ItemsListItemTemplate
        {
            get => (DataTemplate)GetValue(ItemsListItemTemplateProperty);
            set => SetValue(ItemsListItemTemplateProperty, value);
        }

        /// <summary>Gets or sets the next item in the list after the selected one.</summary>
        /// <value>The next item in the list after the selected one.</value>
        public object NextItem
        {
            get => GetValue(NextItemProperty);
            set => SetValue(NextItemProperty, value);
        }

        /// <summary>
        ///     Gets or sets the <see cref="DataTemplate" /> for the preview of the next and previous
        ///     item.
        /// </summary>
        /// <value>The <see cref="DataTemplate" /> for the preview of the next and previous item.</value>
        public DataTemplate PreviewItemTemplate
        {
            get => (DataTemplate)GetValue(PreviewItemTemplateProperty);
            set => SetValue(PreviewItemTemplateProperty, value);
        }

        /// <summary>
        ///     Gets or sets the previous item in the list before the selected one. Can be <c>null</c> if
        ///     <see cref="IsCycling" /> is set to <c>false</c>.
        /// </summary>
        /// <value>
        ///     The previous item in the list before the selected one. Can be <c>null</c> if
        ///     <see cref="IsCycling" /> is set to <c>false</c>.
        /// </value>
        public object PreviousItem
        {
            get => GetValue(PreviousItemProperty);
            set => SetValue(PreviousItemProperty, value);
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.backButton = GetTemplateChild(TemplatePartNameBackButton) as Button;
            this.forwardButton = GetTemplateChild(TemplatePartNameForwardButton) as Button;

            if (this.backButton != null)
            {
                this.backButton.Click += OnBackButtonClick;
            }

            if (this.forwardButton != null)
            {
                this.forwardButton.Click += OnForwardButtonClick;
            }
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FlipViewItem();
        }

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FlipViewItem;
        }

        /// <inheritdoc />
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (Items.Count == 0)
            {
                PreviousItem = null;
                NextItem = null;

                return;
            }

            var previousIndex = SelectedIndex - 1;
            var nextIndex = SelectedIndex + 1;

            if (IsCycling)
            {
                previousIndex = (previousIndex + Items.Count) % Items.Count;
                nextIndex = (nextIndex + Items.Count) % Items.Count;
            }

            PreviousItem = previousIndex >= 0 ? Items[previousIndex] : null;
            NextItem = nextIndex < Items.Count ? Items[nextIndex] : null;
        }

        #endregion

        #region Event Handlers

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
                SelectedIndex = ++SelectedIndex % Items.Count;
            }
            else
            {
                SelectedIndex = Math.Min(++SelectedIndex, Items.Count - 1);
            }
        }

        #endregion
    }
}