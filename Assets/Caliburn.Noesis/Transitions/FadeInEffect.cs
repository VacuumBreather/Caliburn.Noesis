namespace Caliburn.Noesis.Transitions
{
    /// <summary>A fade-in transition effect.</summary>
    /// <seealso cref="TransitionEffectBase{FadeInEffect}" />
    public class FadeInEffect : FadeEffectBase<FadeInEffect>
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="FadeInEffect" /> class.</summary>
        public FadeInEffect()
            : base(FadeEffectType.FadeIn)
        {
        }

        #endregion
    }
}