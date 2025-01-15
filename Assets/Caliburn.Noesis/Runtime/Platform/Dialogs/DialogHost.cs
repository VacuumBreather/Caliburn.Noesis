
using UnityEditor.Experimental.GraphView;
#if UNITY_5_5_OR_NEWER
using global::Noesis;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
#endif

namespace Caliburn.Noesis
{
    /// <summary>Represents a control that contains opened <see cref="DialogItem"/> items.</summary>
    /// <seealso cref="ContentControl"/>
    [TemplatePart(Name = RootGridPartName, Type = typeof(Grid))]
    [StyleTypedProperty(Property = nameof(DialogContainerStyle), StyleTargetType = typeof(DialogItem))]
    public class DialogHost : ContentControl
    {
        /// <summary>The name of the root grid template part.</summary>
        public const string RootGridPartName = "PART_RootGrid";

        /// <summary>Identifies the <see cref="DialogContainerStyle"/> dependency property.</summary>
        public static readonly DependencyProperty DialogContainerStyleProperty =
            DependencyProperty.Register(nameof(DialogContainerStyle),
                typeof(Style),
                typeof(DialogHost),
                new PropertyMetadata(default(Style)));

        /// <summary>Identifies the <see cref="OverlayBackgroundBrush"/> dependency property.</summary>
        public static readonly DependencyProperty OverlayBackgroundBrushProperty =
            DependencyProperty.Register(nameof(OverlayBackgroundBrush),
                typeof(Brush),
                typeof(DialogHost),
                new PropertyMetadata(Brushes.Transparent));
        
        public static readonly DependencyProperty PrivateContentProperty = DependencyProperty.Register(
            "PrivateContent",
            typeof(object),
            typeof(DialogHost),
            new PropertyMetadata(default(object), OnPrivateContentChanged));

        private Grid _rootGrid;

        /// <summary>Initializes static members of the <see cref="DialogHost"/> class.</summary>
        static DialogHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogHost), new FrameworkPropertyMetadata(typeof(DialogHost)));
        }

        /// <summary>Initializes a new instance of the <see cref="DialogHost"/> class.</summary>
        public DialogHost()
        {
            Loaded += OnLoaded;
        }

        /// <summary>Gets or sets the <see cref="Style"/> to be used on the <see cref="DialogItem"/> container.</summary>
        public Style DialogContainerStyle
        {
            get => (Style)GetValue(DialogContainerStyleProperty);
            set => SetValue(DialogContainerStyleProperty, value);
        }

        /// <summary>Gets or sets the brush used for the background which overlays the regular UI while a dialog is open.</summary>
        public Brush OverlayBackgroundBrush
        {
            get => (Brush)GetValue(OverlayBackgroundBrushProperty);
            set => SetValue(OverlayBackgroundBrushProperty, value);
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            _rootGrid = GetTemplateChild(RootGridPartName) as Grid;

            base.OnApplyTemplate();
        }

        private static void OnPrivateContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DialogHost host)
            {
                return;
            }
            
            host.SetCurrentValue(ContentProperty, e.NewValue);
            host.UpdateOverlayVisibility();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var serviceLocator = this.GetServiceLocator();
            var dialogService = serviceLocator?.GetInstance<IDialogService>();


            if (dialogService is { } service)
            {
                DataContext = service;
                var contentBinding = new Binding(nameof(IDialogService.ActiveItem)) { Mode = BindingMode.OneWay };
                SetBinding(PrivateContentProperty, contentBinding);
            }

            UpdateOverlayVisibility();
        }

        private void UpdateOverlayVisibility()
        {
            _rootGrid?.SetCurrentValue(VisibilityProperty, Content is null ? Visibility.Collapsed : Visibility.Visible);
        }
    }
}
