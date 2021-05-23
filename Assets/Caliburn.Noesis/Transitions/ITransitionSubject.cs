namespace Caliburn.Noesis.Transitions
{
    using System;

    /// <summary>Defines the subject of a transition effect.</summary>
    public interface ITransitionSubject
    {
        /// <summary>
        ///     Additional transition effects which will be run in combination with the
        ///     <see cref="TransitionEffect" />.
        /// </summary>
        IBindableCollection<ITransition> AdditionalTransitionEffects { get; }

        /// <summary>Gets the name of the matrix transform template part.</summary>
        /// <value>The name of the matrix transform template part.</value>
        string MatrixTransformName { get; }

        /// <summary>Gets the name of the rotate transform template part.</summary>
        /// <value>The name of the rotate transform template part.</value>
        string RotateTransformName { get; }

        /// <summary>Gets the name of the scale transform template part.</summary>
        /// <value>The name of the scale transform template part.</value>
        string ScaleTransformName { get; }

        /// <summary>Gets the name of the skew transform template part.</summary>
        /// <value>The name of the skew transform template part.</value>
        string SkewTransformName { get; }

        /// <summary>Gets the timespan by which the transition of this subject is delayed.</summary>
        /// <value>The timespan by which the transition of this subject is delayed.</value>
        TimeSpan TransitionDelay { get; }

        /// <summary>Gets or sets the effect to run when transitioning.</summary>
        ITransition TransitionEffect { get; set; }

        /// <summary>Gets the name of the translate transform template part.</summary>
        /// <value>The name of the translate transform template part.</value>
        string TranslateTransformName { get; }

        /// <summary>Cancels all running transition effects.</summary>
        void CancelTransition();

        /// <summary>Performs the transition with all specified effects.</summary>
        void PerformTransition();
    }
}