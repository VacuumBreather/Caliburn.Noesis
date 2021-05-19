namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows.Markup;

    [MarkupExtensionReturnType(typeof(TransitionEffectBase))]
    public class TransitionEffectExtension : MarkupExtension
    {
        #region Constructors and Destructors

        public TransitionEffectExtension()
        {
            Kind = TransitionEffectKind.None;
        }

        public TransitionEffectExtension(TransitionEffectKind kind)
        {
            Kind = kind;
        }

        public TransitionEffectExtension(TransitionEffectKind kind, TimeSpan duration)
        {
            Kind = kind;
            Duration = duration;
        }

        #endregion

        #region Public Properties

        [ConstructorArgument("duration")]
        public TimeSpan? Duration { get; set; }

        [ConstructorArgument("kind")]
        public TransitionEffectKind Kind { get; set; }

        #endregion

        #region Public Methods

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Duration.HasValue
                       ? new TransitionEffect(Kind, Duration.Value)
                       : new TransitionEffect(Kind);
        }

        #endregion
    }
}