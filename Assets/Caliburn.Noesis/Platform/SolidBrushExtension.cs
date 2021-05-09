namespace Caliburn.Noesis
{
    using System;
    using System.Collections.Generic;
#if UNITY_5_5_OR_NEWER
    using Noesis;
#else
    using System.Windows.Markup;
    using System.Windows.Media;
#endif

    /// <summary>
    ///     Implements a markup extension that returns a SolidColorBrush corresponding to the
    ///     specified color.
    /// </summary>
    /// <remarks>The brushes are frozen and cached.</remarks>
    public class SolidBrushExtension : MarkupExtension
    {
        private static readonly IDictionary<Color, SolidColorBrush> CachedBrushes =
            new Dictionary<Color, SolidColorBrush>();

        /// <summary>Initializes a new instance of the <see cref="SolidBrushExtension" /> class.</summary>
        /// <param name="color">The color of the brush.</param>
        public SolidBrushExtension(Color color)
        {
            Color = color;
        }

        /// <summary>Gets the color of the brush.</summary>
        /// <value>The color of the brush.</value>
        public Color Color { get; }

        /// <summary>
        ///     Returns the <see cref="SolidColorBrush" /> corresponding to the color specified by this
        ///     extension.
        /// </summary>
        /// <param name="serviceProvider">
        ///     A service provider helper that can provide services for the markup
        ///     extension.
        /// </param>
        /// <returns>The <see cref="SolidColorBrush" /> corresponding to the color specified by this extension.</returns>
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