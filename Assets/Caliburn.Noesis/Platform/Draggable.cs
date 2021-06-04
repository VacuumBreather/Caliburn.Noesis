namespace Caliburn.Noesis
{
    using System;
    using System.Linq;
    using Extensions;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
#endif

    /// <summary>Contains attached properties used to drag elements hosted inside a canvas.</summary>
    public static class Draggable
    {
        #region Constants and Fields

        /// <summary>
        ///     IsDragHandle property. This is an attached property. Draggable defines the property, so
        ///     that it can be set on any <see cref="FrameworkElement" /> that is supposed to be the handle of
        ///     a draggable <see cref="FrameworkElement" /> hosted inside a canvas.
        /// </summary>
        public static readonly DependencyProperty IsDragHandleProperty = DependencyProperty.RegisterAttached(
            PropertyNameHelper.GetName(nameof(IsDragHandleProperty)),
            typeof(bool),
            typeof(Draggable),
            new PropertyMetadata(default(bool), OnPropertyChanged));

        /// <summary>
        ///     BringToFrontOnClick property. This is an attached property. Draggable defines the
        ///     property, so that it can be set on any <see cref="FrameworkElement" /> that is supposed to be
        ///     brought to the front of a canvas whenever it is left-clicked.
        /// </summary>
        public static readonly DependencyProperty BringToFrontOnClickProperty = DependencyProperty.RegisterAttached(
            PropertyNameHelper.GetName(nameof(BringToFrontOnClickProperty)),
            typeof(bool),
            typeof(Draggable),
            new PropertyMetadata(default(bool), OnPropertyChanged));

        private static readonly DependencyProperty AttachedElementProperty = DependencyProperty.RegisterAttached(
            PropertyNameHelper.GetName(nameof(AttachedElementProperty)),
            typeof(FrameworkElement),
            typeof(Draggable),
            new PropertyMetadata(default(FrameworkElement)));

        private static readonly DependencyProperty InitialDragMousePositionProperty =
            DependencyProperty.RegisterAttached(
                PropertyNameHelper.GetName(nameof(InitialDragMousePositionProperty)),
                typeof(Point),
                typeof(Draggable),
                new PropertyMetadata(default(Point)));

        private static readonly DependencyProperty ParentCanvasProperty = DependencyProperty.RegisterAttached(
            PropertyNameHelper.GetName(nameof(ParentCanvasProperty)),
            typeof(Canvas),
            typeof(Draggable),
            new PropertyMetadata(default(Canvas)));

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets a value indicating whether the specified element will be brought to the from of its
        ///     hosting canvas whenever it is left-clicked.
        /// </summary>
        /// <param name="element">The element in the host canvas.</param>
        /// <returns>
        ///     <c>true</c> if the specified element will be brought to the from of its hosting canvas
        ///     whenever it is left-clicked; otherwise, <c>false</c>.
        /// </returns>
        public static bool GetBringToFrontOnClick(FrameworkElement element)
        {
            return (bool)element.GetValue(BringToFrontOnClickProperty);
        }

        /// <summary>
        ///     Gets a value indicating whether the specified element is the handle of a draggable element
        ///     inside a canvas.
        /// </summary>
        /// <param name="element">The potential drag handle element.</param>
        /// <returns>
        ///     <c>true</c> if the specified element is is the handle of a draggable element inside a
        ///     canvas; otherwise, <c>false</c>.
        /// </returns>
        public static bool GetIsDragHandle(FrameworkElement element)
        {
            return (bool)element.GetValue(IsDragHandleProperty);
        }

        /// <summary>
        ///     Sets a value indicating whether the specified element should be brought to the from of its
        ///     hosting canvas whenever it is left-clicked.
        /// </summary>
        /// <param name="element">The element to bring to the front of its host canvas.</param>
        /// <param name="value">
        ///     If this is set to <c>true</c>, the specified element will be brought to the
        ///     from of its hosting canvas whenever it is left-clicked.
        /// </param>
        /// s>
        public static void SetBringToFrontOnClick(FrameworkElement element, bool value)
        {
            element.SetValue(BringToFrontOnClickProperty, value);
        }

        /// <summary>
        ///     Sets a value indicating whether the specified element should be the handle of a draggable
        ///     element inside a canvas.
        /// </summary>
        /// <param name="element">The potential drag handle element.</param>
        /// <param name="value">
        ///     If this is set to <c>true</c>, the specified element will be the handle of a
        ///     draggable element inside a canvas.
        /// </param>
        /// s>
        public static void SetIsDragHandle(FrameworkElement element, bool value)
        {
            element.SetValue(IsDragHandleProperty, value);
        }

        #endregion

        #region Event Handlers

        private static void OnElementMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            var element = (FrameworkElement)sender;

            if (GetIsDragHandle(element))
            {
                element.MouseLeftButtonUp += OnElementMouseLeftButtonUp;
                element.MouseMove += OnElementMouseMove;
                element.CaptureMouse();

                var draggableElement = GetAttachedElement(element);

                SetInitialDragMousePosition(element, args.GetPosition(draggableElement));
            }

            if (GetBringToFrontOnClick(element))
            {
                var canvas = GetParentCanvas(element);
                var hostedElement = GetAttachedElement(element);

                var zIndex = 0;

                // Cast is necessary in WPF.
                var children = canvas.Children.Cast<UIElement>();

                children.Where(child => !ReferenceEquals(child, hostedElement))
                        .OrderBy(child => child.GetValue(Panel.ZIndexProperty))
                        .ThenBy(child => canvas.Children.IndexOf(child))
                        .Append(hostedElement)
                        .ForEach(child => { child.SetValue(Panel.ZIndexProperty, zIndex++); });
            }
        }

        private static void OnElementMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            var element = (FrameworkElement)sender;

            element.MouseMove -= OnElementMouseMove;
            element.MouseLeftButtonUp -= OnElementMouseLeftButtonUp;

            element.ReleaseMouseCapture();
        }

        private static void OnElementMouseMove(object sender, MouseEventArgs args)
        {
            var element = (FrameworkElement)sender;

            var canvas = GetParentCanvas(element);
            var draggableElement = GetAttachedElement(element);
            var initialPosition = GetInitialDragMousePosition(element);
            var position = args.GetPosition(canvas);

            var x = position.X - initialPosition.X;
            var y = position.Y - initialPosition.Y;

            x = Math.Min(Math.Max(0, x), canvas.ActualWidth - draggableElement.ActualWidth);
            y = Math.Min(Math.Max(0, y), canvas.ActualHeight - draggableElement.ActualHeight);

            draggableElement.SetValue(Canvas.LeftProperty, x);
            draggableElement.SetValue(Canvas.TopProperty, y);
        }

        #endregion

        #region Private Methods

        private static (Canvas canvas, FrameworkElement draggableElement) FindRelevantElements(FrameworkElement element)
        {
            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;

            while (parent != null)
            {
                if (parent is Canvas canvas)
                {
                    return (canvas, element);
                }

                element = parent;
                parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
            }

            return default;
        }

        private static FrameworkElement GetAttachedElement(DependencyObject element)
        {
            return (FrameworkElement)element.GetValue(AttachedElementProperty);
        }

        private static Point GetInitialDragMousePosition(DependencyObject element)
        {
            return (Point)element.GetValue(InitialDragMousePositionProperty);
        }

        private static Canvas GetParentCanvas(DependencyObject element)
        {
            return (Canvas)element.GetValue(ParentCanvasProperty);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement element) || (e.NewValue == e.OldValue))
            {
                return;
            }

            if (Equals(e.NewValue, true))
            {
                var (canvas, draggableElement) = FindRelevantElements(element);

                if (canvas is null || draggableElement is null)
                {
                    return;
                }

                SetParentCanvas(element, canvas);
                SetAttachedElement(element, draggableElement);

                element.MouseLeftButtonDown += OnElementMouseLeftButtonDown;
            }
            else
            {
                element.MouseLeftButtonDown -= OnElementMouseLeftButtonDown;
            }
        }

        private static void SetAttachedElement(DependencyObject element, FrameworkElement value)
        {
            element.SetValue(AttachedElementProperty, value);
        }

        private static void SetInitialDragMousePosition(DependencyObject element, Point value)
        {
            element.SetValue(InitialDragMousePositionProperty, value);
        }

        private static void SetParentCanvas(DependencyObject element, Canvas value)
        {
            element.SetValue(ParentCanvasProperty, value);
        }

        #endregion
    }
}