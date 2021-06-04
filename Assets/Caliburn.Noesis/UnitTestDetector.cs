namespace Caliburn.Noesis
{
    using System;
    using System.Linq;
    using JetBrains.Annotations;

    /// <summary>Responsible for checking if the code is framework is executing inside a unit test.</summary>
    [PublicAPI]
    public static class UnitTestDetector
    {
        #region Constants and Fields

        private const string TestAssemblyName = "Microsoft.VisualStudio.QualityTools.UnitTestFramework";

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes static members of the <see cref="UnitTestDetector" /> class.</summary>
        static UnitTestDetector()
        {
            IsInUnitTest = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.StartsWith(TestAssemblyName));
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating if the framework is executing inside a unit test.</summary>
        public static bool IsInUnitTest { get; }

        #endregion
    }
}