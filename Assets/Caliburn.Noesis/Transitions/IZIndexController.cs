namespace Caliburn.Noesis.Transitions
{
    /// <summary>
    ///     Defines a class which can stack the z-index of a series of
    ///     <see cref="TransitionerItem" /> objects.
    /// </summary>
    public interface IZIndexController
    {
        void Stack(params TransitionerItem[] highestToLowest);
    }
}