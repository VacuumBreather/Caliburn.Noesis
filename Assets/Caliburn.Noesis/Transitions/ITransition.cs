namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    /// <summary>
    ///     Defines a transition effect for a <see cref="FrameworkElement" /> implementing
    ///     <see cref="ITransitionSubject" />.
    /// </summary>
    public interface ITransition
    {
        /// <summary>Gets or sets the <see cref="TimeSpan" /> by which the transition is delayed.</summary>
        /// <value>The <see cref="TimeSpan" /> by which the transition is delayed.</value>
        TimeSpan Delay { get; set; }

        /// <summary>Gets or sets the duration of the transition.</summary>
        /// <value>The duration of the transition.</value>
        TimeSpan Duration { get; set; }

        /// <summary>Gets or sets the easing function which is applied to the transition.</summary>
        /// <value>The easing function which is applied to the transition.</value>
        IEasingFunction EasingFunction { get; set; }

        /// <summary>>Gets or sets the origin point for the transition effect.</summary>
        /// <value>The origin point for the transition effect.</value>
        Point Origin { get; set; }

        /// <summary>Builds the transition effect timeline.</summary>
        /// <typeparam name="TSubject">The type of the <see cref="ITransitionSubject" />.</typeparam>
        /// <param name="effectSubject">The <see cref="ITransitionSubject" /> of the effect.</param>
        /// <returns>The built transition effect timeline.</returns>
        Timeline Build<TSubject>(TSubject effectSubject)
            where TSubject : FrameworkElement, ITransitionSubject;

        /// <summary>Cancels the transition effect.</summary>
        /// <remarks>
        ///     When implemented this should set the appropriate values which represent the end state of
        ///     the transition.
        /// </remarks>
        /// <typeparam name="TSubject">The type of the <see cref="ITransitionSubject" />.</typeparam>
        /// <param name="effectSubject">The <see cref="ITransitionSubject" /> of the effect.</param>
        void Cancel<TSubject>(TSubject effectSubject)
            where TSubject : FrameworkElement, ITransitionSubject;
    }
}