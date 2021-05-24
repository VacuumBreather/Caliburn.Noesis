namespace Caliburn.Noesis.Transitions
{
    using System;

    /// <summary>
    ///     Represents the possible conditions which trigger the transition of a
    ///     <see cref="TransitioningContentControl" />.
    /// </summary>
    [Flags]
    public enum TransitionTriggers
    {
        /// <summary>No transitions will happen.</summary>
        None = 0b0000,

        /// <summary>
        ///     The transition is triggered when the <see cref="TransitioningContentControl" /> is first
        ///     loaded.
        /// </summary>
        Loaded = 0b0001,

        /// <summary>
        ///     The transition is triggered when the <see cref="TransitioningContentControl" /> becomes
        ///     visible.
        /// </summary>
        IsVisible = 0b0010,

        /// <summary>
        ///     The transition is triggered when the <see cref="TransitioningContentControl" /> is
        ///     enabled.
        /// </summary>
        IsEnabled = 0b0100,

        /// <summary>
        ///     The transition is triggered when the content of the
        ///     <see cref="TransitioningContentControl" /> changes.
        /// </summary>
        ContentChanged = 0b1000,

        /// <summary>
        ///     The transition is triggered when the content of the
        ///     <see cref="TransitioningContentControl" /> is first loaded or when its content changes. This is
        ///     the default.
        /// </summary>
        Default = Loaded | ContentChanged,

        /// <summary>
        ///     The transition is triggered when the <see cref="TransitioningContentControl" /> is first
        ///     loaded or when its content changes, it becomes visible or is enabled. This is the default.
        /// </summary>
        All = Loaded | IsVisible | IsEnabled | ContentChanged
    }
}