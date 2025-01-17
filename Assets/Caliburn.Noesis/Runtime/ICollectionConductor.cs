namespace Caliburn.Noesis
{
    /// <summary>
    /// Denotes an interface which conducts a collection of other objects by maintaining a strict lifecycle.
    /// </summary>
    /// <typeparam name="T">The type of items to conduct.</typeparam>
    public interface ICollectionConductor<T> : IConductor<T>
    {
        IObservableCollection<T> Items { get; }
    }
}
