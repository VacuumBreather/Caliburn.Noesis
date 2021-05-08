namespace Caliburn.Noesis
{
    /// <summary>Represents a configuration for the framework.</summary>
    public readonly struct CaliburnConfiguration
    {
        #region Constructors and Destructors

        private CaliburnConfiguration(AssemblySource assemblySource, ViewLocator viewLocator)
        {
            AssemblySource = assemblySource;
            ViewLocator = viewLocator;
        }

        #endregion

        #region Public Properties

        /// <summary>The default configuration. Use this a base to add your modifications.</summary>
        public static CaliburnConfiguration Default => new CaliburnConfiguration(
            new AssemblySource(),
            new ViewLocator());

        /// <summary>Gets the collection of assemblies containing relevant view-model and view types.</summary>
        public AssemblySource AssemblySource { get; }

        /// <summary>
        ///     Gets the <see cref="ViewLocator" /> responsible for mapping view-model types to view
        ///     types.
        /// </summary>
        public ViewLocator ViewLocator { get; }

        #endregion
    }
}