namespace Caliburn.Noesis
{
    /// <summary>
    ///     Denotes an instance which implements <see cref="IHaveDisplayName" />,
    ///     <see cref="IActivate" />, <see cref="IDeactivate" />, <see cref="IGuardClose" /> and
    ///     <see cref="INotifyPropertyChangedEx" />
    /// </summary>
    public interface IScreen : IHaveDisplayName,
                               IActivate,
                               IDeactivate,
                               IGuardClose,
                               IHaveLogger,
                               INotifyPropertyChangedEx
    {
    }
}