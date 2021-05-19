namespace Caliburn.Noesis.Transitions
{
    using System;

    /// <summary>Defines the subject of a transition effect.</summary>
    public interface ITransitionEffectSubject
    {
        /// <summary>Gets the name of the matrix transform.</summary>
        /// <value>The name of the matrix transform.</value>
        string MatrixTransformName { get; }

        /// <summary>Gets the offset timespan.</summary>
        /// <value>The offset timespan.</value>
        TimeSpan Offset { get; }

        /// <summary>Gets the name of the rotate transform.</summary>
        /// <value>The name of the rotate transform.</value>
        string RotateTransformName { get; }

        /// <summary>Gets the name of the scale transform.</summary>
        /// <value>The name of the scale transform.</value>
        string ScaleTransformName { get; }

        /// <summary>Gets the name of the skew transform.</summary>
        /// <value>The name of the skew transform.</value>
        string SkewTransformName { get; }

        /// <summary>Gets the name of the translate transform.</summary>
        /// <value>The name of the translate transform.</value>
        string TranslateTransformName { get; }
    }
}