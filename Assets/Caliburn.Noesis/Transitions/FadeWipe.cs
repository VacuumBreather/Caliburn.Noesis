namespace Caliburn.Noesis.Transitions
{
    using System.Windows;

    /// <seealso cref="TransitionWipeBase" />
    /// <seealso cref="ITransitionWipe" />
    public class FadeWipe : TransitionWipeBase, ITransitionWipe
    {
        #region Constants and Fields

        private readonly ITransition fromTransition = new FadeOutTransition();
        private readonly ITransition toTransition = new FadeInTransition();

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override void ConfigureItems(TransitionerItem fromItem,
                                               TransitionerItem toItem,
                                               Point origin)
        {
            fromItem.TransitionEffect = this.fromTransition;
            fromItem.TransitionEffect.Duration = Duration;
            fromItem.TransitionEffect.Delay = Delay;
            fromItem.TransitionEffect.EasingFunction = EasingFunction;

            toItem.TransitionEffect = this.toTransition;
            toItem.TransitionEffect.Duration = Duration;
            toItem.TransitionEffect.Delay = Delay;
            toItem.TransitionEffect.EasingFunction = EasingFunction;
        }

        #endregion
    }
}