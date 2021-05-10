namespace Caliburn.Noesis.Samples.Conductors.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    ///     The <see cref="DecorativeBorder" /> decorator is used to draw a decorative double border
    ///     and/or background around another element.
    /// </summary>
    public class DecorativeBorder : Decorator
    {
        #region Constants and Fields

        /// <summary>DependencyProperty for <see cref="Padding" /> property.</summary>
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
            nameof(Padding),
            typeof(Thickness),
            typeof(DecorativeBorder),
            new FrameworkPropertyMetadata(
                new Thickness(),
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsRender),
            IsThicknessValid);

        /// <summary>DependencyProperty for <see cref="BorderBrush" /> property.</summary>
        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            nameof(BorderBrush),
            typeof(Brush),
            typeof(DecorativeBorder),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions
                    .SubPropertiesDoNotAffectRender,
                OnClearPenCache));

        /// <summary>DependencyProperty for <see cref="Background" /> property.</summary>
        public static readonly DependencyProperty BackgroundProperty =
            Panel.BackgroundProperty.AddOwner(
                typeof(DecorativeBorder),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        /// <summary>DependencyProperty for <see cref="BorderThickness" /> property.</summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                nameof(BorderThickness),
                typeof(double),
                typeof(DecorativeBorder),
                new FrameworkPropertyMetadata(
                    default(double),
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnClearPenCache),
                IsValid);

        #endregion

        #region Public Properties

        /// <summary>The Background property defines the brush used to fill the area within the border.</summary>
        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        /// <summary>The BorderBrush property defines the brush used to fill the border region.</summary>
        public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        /// <summary>
        ///     The BorderThickness property defined how thick a border to draw.  The property's value is
        ///     a double representing a uniform value for each of the Left, Top, Right, and Bottom sides.
        ///     Values of Auto are interpreted as zero.
        /// </summary>
        public double BorderThickness
        {
            get => (double)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        /// <summary>
        ///     The Padding property inflates the effective size of the child by the specified thickness.
        ///     This achieves the same effect as adding margin on the child, but is present here for
        ///     convenience.
        /// </summary>
        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        #endregion

        #region Private Properties

        private StreamGeometry InnerBorderGeometry { get; set; } = new StreamGeometry();

        private Pen BorderPen { get; set; } = new Pen();

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            //  Arrange child
            var boundRect = new Rect(finalSize);

            if (Child is { } child)
            {
                var borderThickness = BorderThickness;

                if (UseLayoutRounding)
                {
                    var dpi = GetDpi();
                    borderThickness = RoundLayoutValue(borderThickness, dpi.Width);
                }

                
                var innerRect = DeflateRect(boundRect, borderThickness);
                var childRect = DeflateRect(innerRect, Padding);

                child.Arrange(childRect);
            }

            var innerBorderGeometry = new StreamGeometry();
 
            using (var geometryContext = innerBorderGeometry.Open())
            {
                GenerateGeometry(geometryContext, boundRect);
            }
 
            innerBorderGeometry.Freeze();
            InnerBorderGeometry = innerBorderGeometry;

            return finalSize;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            var child = Child;
            var mySize = new Size();
            var borderThickness = BorderThickness;

            if (UseLayoutRounding)
            {
                var dpi = GetDpi();
                borderThickness = RoundLayoutValue(borderThickness, dpi.Width);
            }

            // Compute the chrome size added by the various elements
            borderThickness = CollapseBorderThickness(borderThickness);
            var padding = CollapseThickness(Padding);

            //If we have a child
            if (child != null)
            {
                // Combine into total decorating size
                var combined = new Size(
                    borderThickness + padding.Width,
                    borderThickness + padding.Height);

                // Remove size of border only from child's reference size.
                var childConstraint = new Size(
                    Math.Max(0.0, constraint.Width - combined.Width),
                    Math.Max(0.0, constraint.Height - combined.Height));

                child.Measure(childConstraint);

                var childSize = child.DesiredSize;

                // Now use the returned size to drive our size, by adding back the margins, etc.
                mySize.Width = childSize.Width + combined.Width;
                mySize.Height = childSize.Height + combined.Height;
            }
            else
            {
                // Combine into total decorating size
                mySize = new Size(borderThickness + padding.Width, borderThickness + padding.Height);
            }

            return mySize;
        }

        /// <inheritdoc />
        protected override void OnRender(DrawingContext dc)
        {
            var useLayoutRounding = UseLayoutRounding;
            var dpi = GetDpi();
            var borderThickness = BorderThickness;

            // If we have no brush with which to draw the border, bail here.
            if (AreClose(0.0, borderThickness) || !(BorderBrush is { } borderBrush))
            {
                return;
            }

            // Initialize the pen.
            if (!(BorderPen is { } borderPen))
            {
                borderPen = new Pen
                                {
                                    Brush = borderBrush,
                                    Thickness = useLayoutRounding
                                                    ? RoundLayoutValue(borderThickness, dpi.Width)
                                                    : borderThickness
                                };

                if (borderBrush.IsFrozen)
                {
                    borderPen.Freeze();
                }

                BorderPen = borderPen;
            }

            var halfThickness = borderPen.Thickness * 0.5;

            // Create rect with border thickness, and round if applying layout rounding.
            var rect = new Rect(
                new Point(halfThickness, halfThickness),
                new Point(RenderSize.Width - halfThickness, RenderSize.Height - halfThickness));

            dc.DrawRectangle(Brushes.Transparent, borderPen, rect);

            if (InnerBorderGeometry is { } borderGeometry)
            {
                dc.DrawGeometry(Background, borderPen, borderGeometry);
            }
        }

        #endregion

        #region Private Methods

        private static bool AreClose(double x, double y)
        {
            const double DoubleEpsilon = 2.2204460492503131e-016;

            // In case they are Infinities (then epsilon check does not work)
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (x == y)
            {
                return true;
            }

            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DoubleEpsilon
            var epsilon = (Math.Abs(x) + Math.Abs(y) + 10.0) * DoubleEpsilon;
            var delta = x - y;

            return (-epsilon < delta) && (epsilon > delta);
        }

        /// <summary>Generates a StreamGeometry.</summary>
        /// <param name="context">An already opened <see cref="StreamGeometryContext"/>.</param>
        /// <param name="rect">Rectangle for geometry conversion.</param>
        /// <returns>Result geometry.</returns>
        private void GenerateGeometry(StreamGeometryContext context, Rect rect)
        {
            var borderThickness = BorderThickness;
            var halfThickness = borderThickness * 0.5;

            // Compute the coordinates of the key points.
            var topLeftBottomLeft = new Point(2.0 * borderThickness, 4.0 * borderThickness);
            var topLeftBottomRight = new Point(4.0 * borderThickness, 4.0 * borderThickness);
            var topLeftTopRight = new Point(4.0 * borderThickness, 2.0 * borderThickness);
            
            var topRightTopLeft = new Point(rect.Width - 4.0 * borderThickness, 2.0 * borderThickness);
            var topRightBottomLeft = new Point(rect.Width - 4.0 * borderThickness, 4.0 * borderThickness);
            var topRightBottomRight = new Point(rect.Width - 2.0 * borderThickness, 4.0 * borderThickness);
            
            var bottomRightTopRight = new Point(rect.Width - 2.0 * borderThickness, rect.Height - 4.0 * borderThickness);
            var bottomRightTopLeft = new Point(rect.Width - 4.0 * borderThickness, rect.Height - 4.0 * borderThickness);
            var bottomRightBottomLeft = new Point(rect.Width - 4.0 * borderThickness, rect.Height - 2.0 * borderThickness);
            
            var bottomLeftBottomRight = new Point(4.0 * borderThickness, rect.Height - 2.0 * borderThickness);
            var bottomLeftTopRight = new Point(4.0 * borderThickness, rect.Height - 4.0 * borderThickness);
            var bottomLeftTopLeft = new Point(2.0 * borderThickness, rect.Height - 4.0 * borderThickness);

            // Check key points for overlap.
            if (topLeftBottomRight.X > topRightBottomLeft.X)
            {
                return;
            }

            if (topLeftBottomRight.Y > bottomLeftTopRight.Y)
            {
                return;
            }

            // Add on offsets.
            var topLeftOffset = new Vector(halfThickness, halfThickness);
            var topRightOffset = new Vector(-halfThickness, halfThickness);
            var bottomRightOffset = new Vector(-halfThickness, -halfThickness);
            var bottomLeftOffset = new Vector(halfThickness, -halfThickness);

            topLeftBottomLeft += topLeftOffset;
            topLeftBottomRight += topLeftOffset;
            topLeftTopRight += topLeftOffset;
            
            topRightTopLeft += topRightOffset;
            topRightBottomLeft += topRightOffset;
            topRightBottomRight += topRightOffset;
            
            bottomRightTopRight += bottomRightOffset;
            bottomRightTopLeft += bottomRightOffset;
            bottomRightBottomLeft += bottomRightOffset;
            
            bottomLeftBottomRight += bottomLeftOffset;
            bottomLeftTopRight += bottomLeftOffset;
            bottomLeftTopLeft += bottomLeftOffset;

            // Create the border geometry.
            context.BeginFigure(topLeftBottomLeft, true, true);
            context.LineTo(topLeftBottomRight, true, false);
            context.LineTo(topLeftTopRight, true, false);

            context.LineTo(topRightTopLeft, true, false);
            context.LineTo(topRightBottomLeft, true, false);
            context.LineTo(topRightBottomRight, true, false);

            context.LineTo(bottomRightTopRight, true, false);
            context.LineTo(bottomRightTopLeft, true, false);
            context.LineTo(bottomRightBottomLeft, true, false);

            context.LineTo(bottomLeftBottomRight, true, false);
            context.LineTo(bottomLeftTopRight, true, false);
            context.LineTo(bottomLeftTopLeft, true, false);
        }

        private static Size CollapseThickness(Thickness thickness)
        {
            return new Size(thickness.Left + thickness.Right, thickness.Top + thickness.Bottom);
        }

        private static double CollapseBorderThickness(double thickness)
        {
            // Outer line, double thickness gap and inner line.
            var totalThickness = thickness * 5.0;

            return 2.0 * totalThickness;
        }

        private static Rect DeflateRect(Rect rect, Thickness thickness)
        {
            return new Rect(
                rect.Left + thickness.Left,
                rect.Top + thickness.Top,
                Math.Max(0.0, rect.Width - thickness.Left - thickness.Right),
                Math.Max(0.0, rect.Height - thickness.Top - thickness.Bottom));
        }

        private static Rect DeflateRect(Rect rect, double thickness)
        {
            // Outer line, double thickness gap and inner line.
            var totalThickness = thickness * 5.0;

            return new Rect(
                rect.Left + totalThickness,
                rect.Top + totalThickness,
                Math.Max(0.0, rect.Width - (2.0 * totalThickness)),
                Math.Max(0.0, rect.Height - (2.0 * totalThickness)));
        }

        private static bool IsThicknessValid(object obj)
        {
            var thickness = (Thickness)obj;

            return IsValid(thickness.Left) && IsValid(thickness.Top) && IsValid(thickness.Right) &&
                   IsValid(thickness.Bottom);
        }

        private static bool IsValid(object obj)
        {
            var value = (double)obj;

            return (value >= 0) && !double.IsNaN(value) && !double.IsPositiveInfinity(value) &&
                   !double.IsNegativeInfinity(value);
        }

        private static void OnClearPenCache(DependencyObject d,
                                            DependencyPropertyChangedEventArgs args)
        {
            var border = (DecorativeBorder)d;

            border.BorderPen = null;
        }

        private static double RoundLayoutValue(double value, double dpiScale)
        {
            double newValue;

            // If DPI == 1, don't use DPI-aware rounding.
            if (!AreClose(dpiScale, 1.0))
            {
                newValue = Math.Round(value * dpiScale) / dpiScale;

                // If rounding produces a value unacceptable to layout (NaN, Infinity or MaxValue), use the original value.
                if (double.IsNaN(newValue) || double.IsInfinity(newValue) ||
                    AreClose(newValue, double.MaxValue))
                {
                    newValue = value;
                }
            }
            else
            {
                newValue = Math.Round(value);
            }

            return newValue;
        }

        private Size GetDpi()
        {
            var source = PresentationSource.FromVisual(this);

            var dpiX = 1.0;
            var dpiY = 1.0;

            if (source?.CompositionTarget != null)
            {
                dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            }

            return new Size(dpiX, dpiY);
        }

        #endregion
    }
}