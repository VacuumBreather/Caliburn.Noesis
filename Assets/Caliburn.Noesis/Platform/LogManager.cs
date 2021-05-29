namespace Caliburn.Noesis
{
    using System;
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

        private const string FrameworkCategoryName = nameof(Caliburn);
        private static ILoggerFactory loggerFactory = NullLoggerFactory.Instance;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the global logger for the <see cref="Caliburn" />.<see cref="Caliburn.Noesis" />
        ///     framework.
        /// </summary>
        public static ILogger FrameworkLogger { get; private set; } = NullLogger.Instance;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates a new <see cref="ILogger" /> instance using the full name of the given
        ///     <paramref name="type" />.
        /// </summary>
        /// <param name="type">The type defining the category.</param>
        /// <returns>A new <see cref="ILogger" /> instance.</returns>
        public static ILogger GetLogger(Type type)
        {
            return loggerFactory.CreateLogger(type);
        }

        /// <summary>Creates a new <see cref="ILogger" /> instance using the full name of the given type.</summary>
        /// <typeparam name="T">The type defining the category.</typeparam>
        /// <returns>A new <see cref="ILogger" /> instance.</returns>
        public static ILogger<T> GetLogger<T>()
            where T : class
        {
            return loggerFactory.CreateLogger<T>();
        }

        /// <summary>Creates a new <see cref="ILogger" /> instance.</summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>A new <see cref="ILogger" /> instance.</returns>
        public static ILogger GetLogger(string categoryName)
        {
            return loggerFactory.CreateLogger(categoryName);
        }

        /// <summary>Sets the <see cref="ILoggerFactory" /> used by this manager to create loggers.</summary>
        /// <param name="factory">The <see cref="ILoggerFactory" /> this manager should use.</param>
        public static void SetLoggerFactory(ILoggerFactory factory)
        {
            loggerFactory = factory;
            FrameworkLogger = loggerFactory.CreateLogger(FrameworkCategoryName);
        }

        #endregion
    }
}