namespace Caliburn.Noesis
{
    /// <summary>An <see cref="IConductor" /> that also implements <see cref="IHaveActiveItem" />.</summary>
    public interface IConductActiveItem : IConductor, IHaveActiveItem
    {
    }

    /// <summary>An <see cref="IConductor{T}" /> that also implements <see cref="IHaveActiveItem" />.</summary>
    public interface IConductActiveItem<T> : IConductor<T>, IConductActiveItem
    {
    }
}