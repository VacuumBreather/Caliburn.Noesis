namespace Caliburn.Noesis.Transitions
{
    using System;

    /// <summary>Defines the subject of a <see cref="ITransition" /> effect.</summary>
    public interface ITransitionSubject
    {
        /// <summary>
        ///     Additional transition effects which will be run in combination with the main
        ///     <see cref="TransitionEffect" />.
        /// </summary>
        IBindableCollection<ITransition> AdditionalTransitionEffects { get; }

        /// <summary>Gets the name of the matrix transform part of the template.</summary>
        /// <value>The name of the matrix transform part of the template.</value>
        string MatrixTransformName { get; }

        /// <summary>Gets the name of the rotate transform part of the template.</summary>
        /// <value>The name of the rotate transform part of the template.</value>
        string RotateTransformName { get; }

        /// <summary>Gets the name of the scale transform part of the template.</summary>
        /// <value>The name of the scale transform part of the template.</value>
        string ScaleTransformName { get; }

        /// <summary>Gets the name of the skew transform part of the template.</summary>
        /// <value>The name of the skew transform part of the template.</value>
        string SkewTransformName { get; }

        /// <summary>Gets the timespan by which the transition of this subject is delayed.</summary>
        /// <value>The timespan by which the transition of this subject is delayed.</value>
        TimeSpan TransitionDelay { get; }

        /// <summary>Gets or sets the effect to run when transitioning.</summary>
        ITransition TransitionEffect { get; set; }

        /// <summary>Gets the name of the translate transform part of the template.</summary>
        /// <value>The name of the translate transform part of the template.</value>
        string TranslateTransformName { get; }

        /// <summary>Cancels all running transition effects.</summary>
        void CancelTransition();

        /// <summary>Performs the transition.</summary>
        /// <param name="includeAdditionalEffects">
        ///     (Optional) If set to <c>true</c> the additional effects will
        ///     be executed in addition to the main <see cref="TransitionEffect" />. The default is <c>true</c>
        ///     .
        /// </param>
        void PerformTransition(bool includeAdditionalEffects = true);
    }
}