namespace Caliburn.Noesis
{
    /// <summary>Denotes an instance which maintains an active item.</summary>
    public interface IHaveActiveItem
    {
        /// <summary>The currently active item.</summary>
        object ActiveItem { get; set; }
    }
}