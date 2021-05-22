namespace Caliburn.Noesis.Transitions
{
    /// <seealso cref="TransitionWipeBase" />
    /// <seealso cref="ITransitionWipe" />
    public class FadeWipe : TransitionWipeBase, ITransitionWipe
    {
        #region Constructors and Destructors

        /// <inheritdoc />
        public FadeWipe()
        {
            FromEffect = new FadeOutEffect();
            ToEffect = new FadeInEffect();
        }

        #endregion
    }
}