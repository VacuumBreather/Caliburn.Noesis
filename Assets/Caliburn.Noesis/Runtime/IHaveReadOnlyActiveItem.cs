namespace Caliburn.Noesis
{
    /// <summary>Denotes an instance which maintains an active item.</summary>
    public interface IHaveReadOnlyActiveItem
    {
        /// <summary>Gets the currently active item.</summary>
        object ActiveItem { get; }
    }

    /// <summary>Denotes an instance which maintains an active item.</summary>
    /// <typeparam name="T">The type of the active item.</typeparam>
    public interface IHaveReadOnlyActiveItem<out T> : IHaveReadOnlyActiveItem
    {
        /// <summary>Gets the currently active item.</summary>
        new T ActiveItem { get; }
    }
}
