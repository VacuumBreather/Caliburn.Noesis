namespace Caliburn.Noesis
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Markup;
    using System.Windows.Media;
#endif
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Implements a markup extension that returns a SolidColorBrush corresponding to the
    ///     specified color.
    /// </summary>
    /// <remarks>The brushes are frozen and cached.</remarks>
    public class SolidBrushExtension : MarkupExtension
    {
        #region Constants and Fields

        private static readonly IDictionary<(Color, double), SolidColorBrush> CachedBrushes =
            new Dictionary<(Color, double), SolidColorBrush>();

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SolidBrushExtension" /> class.</summary>
        public SolidBrushExtension()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SolidBrushExtension" /> class.</summary>
        /// <param name="color">The color of the brush.</param>
        public SolidBrushExtension(Color color)
        {
            Color = color;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the color of the brush.</summary>
        /// <value>The color of the brush.</value>
        public Color Color { get; set; } = Colors.Transparent;

        /// <summary>Gets or sets the opacity.</summary>
        /// <value>The opacity.</value>
        public double Opacity { get; set; } = 1.0;

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