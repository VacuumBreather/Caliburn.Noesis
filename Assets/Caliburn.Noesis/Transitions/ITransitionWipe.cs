namespace Caliburn.Noesis.Transitions
{
    using System.Windows;

    public interface ITransitionWipe
    {
        void Wipe(TransitionerItem fromItem,
                  TransitionerItem toItem,
                  Point origin,
                  IZIndexController zIndexController);
    }
}