namespace Caliburn.Noesis
{
    using System.Collections.Generic;

    /// <summary>Results from the close strategy.</summary>
    /// <typeparam name="T">The type of child element.</typeparam>
    public interface ICloseResult<out T>
    {
        /// <summary>Indicates which children should close if the parent cannot.</summary>
        IEnumerable<T> Children { get; }

        /// <summary>Indicates whether a close can occur</summary>
        bool CloseCanOccur { get; }
    }
}