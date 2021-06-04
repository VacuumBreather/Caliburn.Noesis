namespace Caliburn.Noesis
{
    /// <summary>
    ///     Denotes an instance which implements <see cref="IHaveDisplayName" />,
    ///     <see cref="IActivate" />, <see cref="IDeactivate" />, <see cref="IGuardClose" /> and
    ///     <see cref="IBindableObject" />
    /// </summary>
    public interface IScreen : IHaveDisplayName,
                               IActivate,
                               IDeactivate,
                               IGuardClose,
                               IBindableObject
    {
    }
}