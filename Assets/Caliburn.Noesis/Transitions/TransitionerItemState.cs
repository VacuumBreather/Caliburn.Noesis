namespace Caliburn.Noesis.Transitions
{
    /// <summary>Represents the state of a <see cref="TransitionerItem" />.</summary>
    public enum TransitionerItemState
    {
        /// <summary>The item is neither the previously nor currently displayed content.</summary>
        None,

        /// <summary>The item is the currently displayed content.</summary>
        Current,

        /// <summary>The item is the previously displayed content during the transition to the new content.</summary>
        Previous,
    }
}