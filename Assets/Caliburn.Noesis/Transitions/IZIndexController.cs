namespace Caliburn.Noesis.Transitions
{
    /// <summary>
    ///     Defines a class which can stack the z-index of a series of
    ///     <see cref="TransitionerSlide" /> objects.
    /// </summary>
    public interface IZIndexController
    {
        void Stack(params TransitionerSlide[] highestToLowest);
    }
}