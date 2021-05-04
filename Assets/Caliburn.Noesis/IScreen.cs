namespace Caliburn.Noesis
{
    #region Using Directives

    #endregion

    /// <summary>
    ///     Denotes an instance which implements <see cref="IHaveDisplayName" />,
    ///     <see cref="IActivate" />, <see cref="IDeactivate" />, <see cref="IGuardClose" /> and
    ///     <see cref="INotifyPropertyChangedEx" />
    /// </summary>
    public interface IScreen : IHaveDisplayName,
                               IActivate,
                               IDeactivate,
                               IGuardClose,
                               INotifyPropertyChangedEx
    {
    }
}