namespace Caliburn.Noesis.Transitions
{
    /// <summary>A fade-in <see cref="ITransition" /> effect.</summary>
    /// <seealso cref="TransitionBase" />
    public class FadeInTransition : FadeTransitionBase
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="FadeInTransition" /> class.</summary>
        public FadeInTransition()
            : base(FadeTransitionType.FadeIn)
        {
        }

        #endregion
    }
}