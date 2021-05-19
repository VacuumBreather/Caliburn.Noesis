namespace Caliburn.Noesis.Transitions
{
    using System.Windows;
    using System.Windows.Media.Animation;

    /// <summary>
    ///     Defines a transition effect for a <see cref="FrameworkElement" /> implementing
    ///     <see cref="ITransitionEffectSubject" />.
    /// </summary>
    public interface ITransitionEffect
    {
        /// <summary>Builds the effect of the specified subject.</summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <param name="effectSubject">The effect subject.</param>
        /// <returns>The built effect timeline.</returns>
        Timeline Build<TSubject>(TSubject effectSubject)
            where TSubject : FrameworkElement, ITransitionEffectSubject;
    }
}