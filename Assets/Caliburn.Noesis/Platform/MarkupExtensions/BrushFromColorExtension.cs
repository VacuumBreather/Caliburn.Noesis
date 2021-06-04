namespace Caliburn.Noesis.MarkupExtensions
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows.Markup;
    using System.Windows.Media;
#endif

    /// <summary>
    ///     Implements a markup extension that returns a <see cref="SolidColorBrush" /> corresponding
    ///     to the specified <see cref="Color" />.
    /// </summary>
    /// <remarks>The brushes are frozen and cached.</remarks>
    [PublicAPI]
    public class BrushFromColorExtension : MarkupExtension
    {
        #region Constants and Fields

        private static readonly IDictionary<(Color, double), SolidColorBrush> CachedBrushes =
            new Dictionary<(Color, double), SolidColorBrush>();

        #endregion

        #region Public Properties

        /// <summary>Gets the color of the brush.</summary>
        /// <value>The color of the brush.</value>
        public Color Color { get; set; }

        /// <summary>Gets the opacity of the brush.</summary>
        /// <value>The opacity of the brush.</value>
        public float Opacity { get; set; } = 1.0f;

        #endregion

        #region Public Methods

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
            if (!CachedBrushes.TryGetValue((Color, Opacity), out var solidColorBrush))
            {
                var color = Color.FromArgb((byte)(Color.A * Opacity), Color.R, Color.G, Color.B);
                solidColorBrush = new SolidColorBrush(color);
                solidColorBrush.Freeze();
                CachedBrushes[(Color, Opacity)] = solidColorBrush;
            }

            return solidColorBrush;
        }

        #endregion
    }
}