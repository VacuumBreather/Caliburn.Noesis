namespace Caliburn.Noesis.Transitions
{
    /// <summary>A fade-out transition effect.</summary>
    /// <seealso cref="TransitionBase" />
    public class FadeOutTransition : FadeTransitionBase
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="FadeOutTransition" /> class.</summary>
        public FadeOutTransition()
            : base(FadeTransitionType.FadeOut)
        {
        }

        #endregion
    }
}