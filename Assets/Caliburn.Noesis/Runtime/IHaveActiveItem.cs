namespace Caliburn.Noesis
{
    /// <summary>
    /// Denotes an instance which maintains an active item.
    /// </summary>
    public interface IHaveActiveItem : IHaveReadOnlyActiveItem
    {
        /// <summary>
        /// The currently active item.
        /// </summary>
        new object ActiveItem { get; set; }
    }

    /// <summary>
    /// Denotes an instance which maintains an active item.
    /// </summary>
    /// <typeparam name="T">The type of the active item.</typeparam>
    public interface IHaveActiveItem<T> : IHaveReadOnlyActiveItem<T>, IHaveActiveItem
    {
        /// <summary>
        /// The currently active item.
        /// </summary>
        new T ActiveItem { get; set; }
    }
}
