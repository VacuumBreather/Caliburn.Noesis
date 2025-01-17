namespace Caliburn.Noesis
{
    /// <summary>
    /// An <see cref="ICollectionConductor{T}" /> that also implements <see cref="IConductActiveItem{T}" />.
    /// </summary>
    /// <typeparam name="T">The type of items to conduct.</typeparam>
    public interface ICollectionConductorWithActiveItem<T> : ICollectionConductor<T>, IConductActiveItem<T>
    {        
    }
}
