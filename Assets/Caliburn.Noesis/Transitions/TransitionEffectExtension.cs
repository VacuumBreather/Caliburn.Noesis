namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows.Markup;

    /// <summary>Provides a <see cref="TransitionEffectBase" /> instance.</summary>
    /// <seealso cref="System.Windows.Markup.MarkupExtension" />
    [MarkupExtensionReturnType(typeof(TransitionEffectBase))]
    public class TransitionEffectExtension : MarkupExtension
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="TransitionEffectExtension" /> class.</summary>
        public TransitionEffectExtension()
        {
            Kind = TransitionEffectKind.None;
        }

        /// <summary>Initializes a new instance of the <see cref="TransitionEffectExtension" /> class.</summary>
        /// <param name="kind">The kind of effect to return.</param>
        public TransitionEffectExtension(TransitionEffectKind kind)
        {
            Kind = kind;
        }

        /// <summary>Initializes a new instance of the <see cref="TransitionEffectExtension" /> class.</summary>
        /// <param name="kind">The kind of effect to return.</param>
        /// <param name="duration">The duration of the effect.</param>
        public TransitionEffectExtension(TransitionEffectKind kind, TimeSpan duration)
        {
            Kind = kind;
            Duration = duration;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the duration of the effect.</summary>
        /// <value>The duration of the effect.</value>
        [ConstructorArgument("duration")]
        public TimeSpan? Duration { get; set; }

        /// <summary>Gets or sets the kind of effect to return.</summary>
        /// <value>The kind of effect to return.</value>
        [ConstructorArgument("kind")]
        public TransitionEffectKind Kind { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Duration.HasValue
                       ? new TransitionEffect(Kind, Duration.Value)
                       : new TransitionEffect(Kind);
        }

        #endregion
    }
}