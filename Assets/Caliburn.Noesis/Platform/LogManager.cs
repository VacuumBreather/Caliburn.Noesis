namespace Caliburn.Noesis
{
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

#if UNITY_5_5_OR_NEWER

#endif

    /// <summary>Responsible for creating <see cref="Microsoft.Extensions.Logging.ILogger" /> instances.</summary>
    [PublicAPI]
    public static class LogManager
    {
        #region Constants and Fields

        /// <summary>The category name for logs created by the framework itself.</summary>
        public const string FrameworkCategoryName = nameof(Caliburn);

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the global logger for the <see cref="Caliburn" />.
        ///     <see cref="Caliburn.Noesis" /> framework.
        /// </summary>
        public static ILogger FrameworkLogger { get; set; } = NullLogger.Instance;

        #endregion
    }
}