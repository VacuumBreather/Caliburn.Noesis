namespace Caliburn.Noesis.Samples.Conductors.MarkupExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Markup;
    using System.Windows.Media;

    public class SolidBrushExtension : MarkupExtension
    {
        private static readonly IDictionary<Color, SolidColorBrush> CachedBrushes =
            new Dictionary<Color, SolidColorBrush>();

        /// <inheritdoc />
        public SolidBrushExtension(Color color)
        {
            Color = color;
        }

        public Color Color { get; }

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!CachedBrushes.TryGetValue(Color, out var solidColorBrush))
            {
                solidColorBrush = new SolidColorBrush(Color);
                solidColorBrush.Freeze();
                CachedBrushes[Color] = solidColorBrush;
            }

            return solidColorBrush;
        }
    }
}