﻿using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>
    /// Extension methods for the <see cref="IDeactivate"/> instance.
    /// </summary>
    public static class DeactivateExtensions
    {
        /// <summary>
        /// Deactivates this instance.
        /// </summary>
        /// <param name="deactivate">The instance to deactivate</param>
        /// <param name="close">Indicates whether or not this instance is being closed.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask DeactivateAsync(this IDeactivate deactivate, bool close) => deactivate.DeactivateAsync(close, default);
    }
}
