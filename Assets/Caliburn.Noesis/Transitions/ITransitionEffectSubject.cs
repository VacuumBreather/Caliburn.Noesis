namespace Caliburn.Noesis.Transitions
{
    using System;

    /// <summary>Defines the subject of a transition effect.</summary>
    public interface ITransitionEffectSubject
    {
        /// <summary>Gets the name of the matrix transform template part.</summary>
        /// <value>The name of the matrix transform template part.</value>
        string MatrixTransformName { get; }

        /// <summary>Gets the offset timespan.</summary>
        /// <value>The offset timespan.</value>
        TimeSpan Offset { get; }

        /// <summary>Gets the name of the rotate transform template part.</summary>
        /// <value>The name of the rotate transform template part.</value>
        string RotateTransformName { get; }

        /// <summary>Gets the name of the scale transform template part.</summary>
        /// <value>The name of the scale transform template part.</value>
        string ScaleTransformName { get; }

        /// <summary>Gets the name of the skew transform template part.</summary>
        /// <value>The name of the skew transform template part.</value>
        string SkewTransformName { get; }

        /// <summary>Gets the name of the translate transform template part.</summary>
        /// <value>The name of the translate transform template part.</value>
        string TranslateTransformName { get; }
    }
}