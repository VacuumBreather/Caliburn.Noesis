namespace Caliburn.Noesis
{
    /// <summary>Helper class to get the name of a dependency property without using hardcoded strings.</summary>
    public static class PropertyNameHelper
    {
        #region Public Methods

        /// <summary>Gets the name of a dependency property minus the "Property" part.</summary>
        /// <param name="dependencyPropertyName">The full name of the dependency property.</param>
        /// <returns>The name of a dependency property minus the "Property" part.</returns>
        public static string GetName(string dependencyPropertyName)
        {
            return dependencyPropertyName.Replace("Property", string.Empty);
        }

        #endregion
    }
}