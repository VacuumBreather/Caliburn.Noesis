namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

#endif

    #endregion

    /// <summary>Contains attached properties used to drag elements hosted inside a canvas.</summary>
    public static class Draggable
    {
        #region Constants and Fields

        /// <summary>
        ///     IsDragHandle property. This is an attached property. Draggable defines the property, so
        ///     that it can be set on any <see cref="FrameworkElement" /> that is supposed to be the handle of
        ///     a draggable <see cref="FrameworkElement" /> hosted inside a canvas.
        /// </summary>
        public static readonly DependencyProperty IsDragHandleProperty =
            DependencyProperty.RegisterAttached(
                PropertyNameHelper.GetName(nameof(IsDragHandleProperty)),
                typeof(bool),
                typeof(Draggable),
                new PropertyMetadata(default(bool), OnIsDragHandleChanged));

        private static readonly DependencyProperty DraggableElementProperty =
            DependencyProperty.RegisterAttached(
                PropertyNameHelper.GetName(nameof(DraggableElementProperty)),
                typeof(FrameworkElement),
                typeof(Draggable),
                new PropertyMetadata(default(FrameworkElement)));

        private static readonly DependencyProperty InitialDragMousePositionProperty =
            DependencyProperty.RegisterAttached(
                PropertyNameHelper.GetName(nameof(InitialDragMousePositionProperty)),
                typeof(Point),
                typeof(Draggable),
                new PropertyMetadata(default(Point)));

        private static readonly DependencyProperty ParentCanvasProperty =
            DependencyProperty.RegisterAttached(
                PropertyNameHelper.GetName(nameof(ParentCanvasProperty)),
                typeof(Canvas),
                typeof(Draggable),
                new PropertyMetadata(default(Canvas)));

        #endregion

        #region Public Methods

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

        private static void OnElementPreviewMouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs args)
        {
            var element = (FrameworkElement)sender;

            element.PreviewMouseLeftButtonUp += OnElementPreviewMouseLeftButtonUp;
            element.PreviewMouseMove += OnElementPreviewMouseMove;
            element.CaptureMouse();

            var draggableElement = GetDraggableElement(element);

            SetInitialDragMousePosition(element, args.GetPosition(draggableElement));
        }

        private static void OnElementPreviewMouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs args)
        {
            var element = (FrameworkElement)sender;

            element.PreviewMouseMove -= OnElementPreviewMouseMove;
            element.PreviewMouseLeftButtonUp -= OnElementPreviewMouseLeftButtonUp;

            element.ReleaseMouseCapture();
        }

        private static void OnElementPreviewMouseMove(object sender, MouseEventArgs args)
        {
            var element = (FrameworkElement)sender;

            var canvas = GetParentCanvas(element);
            var draggableElement = GetDraggableElement(element);
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

        private static (Canvas canvas, FrameworkElement draggableElement) FindRelevantElements(
            FrameworkElement element)
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

        private static FrameworkElement GetDraggableElement(DependencyObject element)
        {
            return (FrameworkElement)element.GetValue(DraggableElementProperty);
        }

        private static Point GetInitialDragMousePosition(DependencyObject element)
        {
            return (Point)element.GetValue(InitialDragMousePositionProperty);
        }

        private static Canvas GetParentCanvas(DependencyObject element)
        {
            return (Canvas)element.GetValue(ParentCanvasProperty);
        }

        private static void OnIsDragHandleChanged(DependencyObject d,
                                                  DependencyPropertyChangedEventArgs e)
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
                SetDraggableElement(element, draggableElement);

                element.PreviewMouseLeftButtonDown += OnElementPreviewMouseLeftButtonDown;
            }
            else
            {
                element.PreviewMouseLeftButtonDown -= OnElementPreviewMouseLeftButtonDown;
            }
        }

        private static void SetDraggableElement(DependencyObject element, FrameworkElement value)
        {
            element.SetValue(DraggableElementProperty, value);
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