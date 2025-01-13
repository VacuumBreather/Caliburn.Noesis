using System.Threading;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>
    /// Extension methods for the <see cref="IConductor"/> instance.
    /// </summary>
    public static class ConductorExtensions
    {
        /// <summary>
        /// Activates the specified item.
        /// </summary>
        /// <param name="conductor">The conductor to activate the item with.</param>
        /// <param name="item">The item to activate.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask ActivateItemAsync(this IConductor conductor, object item) => conductor.ActivateItemAsync(item, default);
    }
}
