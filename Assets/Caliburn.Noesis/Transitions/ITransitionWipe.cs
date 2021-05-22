namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    /// <summary>
    ///     Defines a type of transition where one content replaces another by traveling from one side
    ///     of the view to another or with a special shape.
    /// </summary>
    public interface ITransitionWipe
    {
        /// <summary>Gets or sets the <see cref="TimeSpan" /> by which the wipe transition is delayed.</summary>
        /// <value>The <see cref="TimeSpan" /> by which the wipe transition is delayed.</value>
        TimeSpan Delay { get; set; }

        /// <summary>Gets or sets the duration of the wipe transition.</summary>
        /// <value>The duration of the wipe transition.</value>
        TimeSpan Duration { get; set; }

        /// <summary>Gets or sets the easing function which is applied to the wipe transition.</summary>
        /// <value>The easing function which is applied to the wipe transition.</value>
        public IEasingFunction EasingFunction { get; set; }

        /// <summary>Performs a wipe transition from one item to another.</summary>
        /// <param name="fromItem">The item to transition from.</param>
        /// <param name="toItem">To item to transition to.</param>
        /// <param name="origin">The origin point for the wipe transition.</param>
        /// <param name="zIndexController">The controller in charge of the z-index of each item.</param>
        void Wipe(TransitionerItem fromItem,
                  TransitionerItem toItem,
                  Point origin,
                  IZIndexController zIndexController);
    }
}