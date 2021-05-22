namespace Caliburn.Noesis.Transitions
{
    using System.Windows;

    /// <summary>
    ///     Defines a type of transition where one content replaces another by traveling from one side
    ///     of the view to another or with a special shape.
    /// </summary>
    public interface ITransitionWipe
    {
        /// <summary>Performs a wipe transition from one content item to another.</summary>
        /// <param name="fromItem">The content to transition from.</param>
        /// <param name="toItem">To content to transition to.</param>
        /// <param name="origin">The origin point for the wipe transition.</param>
        /// <param name="zIndexController">The controller in charge of each content's z-index.</param>
        void Wipe(TransitionerItem fromItem,
                  TransitionerItem toItem,
                  Point origin,
                  IZIndexController zIndexController);
    }
}