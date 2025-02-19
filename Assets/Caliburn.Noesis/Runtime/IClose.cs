﻿using Cysharp.Threading.Tasks;
    
namespace Caliburn.Noesis
{
    /// <summary>
    /// Denotes an object that can be closed.
    /// </summary>
    public interface IClose
    {
        /// <summary>
        /// Tries to close this instance.
        /// Also provides an opportunity to pass a dialog result to it's corresponding view.
        /// </summary>
        /// <param name="dialogResult">The dialog result.</param>
        UniTask TryCloseAsync(bool? dialogResult = null);
    }
}
