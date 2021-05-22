namespace Caliburn.Noesis.Transitions
{
    /// <summary>A fade-out transition effect.</summary>
    /// <seealso cref="TransitionEffectBase{FadeOutEffect}" />
    public class FadeOutEffect : FadeEffectBase<FadeOutEffect>
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="FadeOutEffect" /> class.</summary>
        public FadeOutEffect()
            : base(FadeEffectType.FadeOut)
        {
        }

        #endregion
    }
}