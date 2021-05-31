namespace Caliburn.Noesis
{
    using Microsoft.Extensions.Logging;

    /// <summary>Defines a type that supports logging.</summary>
    public interface IHaveLogger
    {
        /// <summary>Gets or sets the logger.</summary>
        ILogger Logger { get; set; }
    }
}