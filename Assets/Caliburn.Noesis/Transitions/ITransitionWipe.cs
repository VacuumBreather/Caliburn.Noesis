namespace Caliburn.Noesis.Transitions
{
    using System.Windows;

    public interface ITransitionWipe
    {
        void Wipe(TransitionerSlide fromSlide,
                  TransitionerSlide toSlide,
                  Point origin,
                  IZIndexController zIndexController);
    }
}