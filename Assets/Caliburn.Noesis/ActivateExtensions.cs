using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>
    /// Extension methods for the <see cref="IActivate"/> instance.
    /// </summary>
    public static class ActivateExtensions
    {
        /// <summary>
        /// Activates this instance.
        /// </summary>
        /// <param name="activate">The instance to activate</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask ActivateAsync(this IActivate activate) => activate.ActivateAsync(default);
    }
}
