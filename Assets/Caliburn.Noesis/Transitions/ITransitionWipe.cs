namespace Caliburn.Noesis.Transitions
{
    using System;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
    using System.Windows.Media.Animation;
#endif

    /// <summary>
    ///     Defines a type of effect used by a <see cref="Transitioner" /> to transition from one
    ///     content to another using <see cref="ITransition" /> effects for both the old and new content.
    /// </summary>
    public interface ITransitionWipe
    {
        /// <summary>Gets or sets the <see cref="TimeSpan" /> by which the wipe transition is delayed.</summary>
        /// <value>The <see cref="TimeSpan" /> by which the wipe transition is delayed.</value>
        TimeSpan Delay { get; set; }

        /// <summary>Gets or sets the duration of the wipe transition.</summary>
        /// <value>The duration of the wipe transition.</value>
        TimeSpan Duration { get; set; }

#if UNITY_5_5_OR_NEWER
        /// <summary>Gets or sets the easing function which is applied to the wipe transition.</summary>
        /// <value>The easing function which is applied to the wipe transition.</value>
        public EasingFunctionBase EasingFunction { get; set; }
#else
        /// <summary>Gets or sets the easing function which is applied to the wipe transition.</summary>
        /// <value>The easing function which is applied to the wipe transition.</value>
        public IEasingFunction EasingFunction { get; set; }
#endif

        /// <summary>Performs a wipe transition from one content to another.</summary>
        /// <param name="fromItem">The content to transition from.</param>
        /// <param name="toItem">To content to transition to.</param>
        /// <param name="origin">The origin point for the wipe transition.</param>
        /// <param name="zIndexController">The controller in charge of the z-index of each item.</param>
        void Wipe(TransitionerItem fromItem,
                  TransitionerItem toItem,
                  Point origin,
                  IZIndexController zIndexController);
    }
}