namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    /// <summary>
    ///     Defines a transition effect for a <see cref="FrameworkElement" /> implementing
    ///     <see cref="ITransitionEffectSubject" />.
    /// </summary>
    public interface ITransitionEffect
    {
        /// <summary>Gets or sets the <see cref="TimeSpan" /> by which the effect is delayed.</summary>
        /// <value>The <see cref="TimeSpan" /> by which the effect is delayed.</value>
        TimeSpan Delay { get; set; }

        /// <summary>Gets or sets the duration of the effect.</summary>
        /// <value>The duration of the effect.</value>
        TimeSpan Duration { get; set; }

        /// <summary>Gets or sets the easing function which is applied to the effect.</summary>
        /// <value>The easing function which is applied to the effect.</value>
        public IEasingFunction EasingFunction { get; set; }

        /// <summary>Builds the effect animation timeline to be applied to the specified subject.</summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <param name="effectSubject">The subject of the effect.</param>
        /// <returns>The built effect animation timeline.</returns>
        Timeline Build<TSubject>(TSubject effectSubject)
            where TSubject : FrameworkElement, ITransitionEffectSubject;
    }
}