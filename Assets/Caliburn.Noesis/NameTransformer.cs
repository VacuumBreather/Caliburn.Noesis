namespace Caliburn.Noesis
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Extensions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     Class for managing the list of rules for transforming view-model type names into view type
    ///     names.
    /// </summary>
    public class NameTransformer : BindableCollection<NameTransformer.Rule>
    {
        #region Constants and Fields

        private const RegexOptions Options = RegexOptions.Compiled;

        private bool useEagerRuleSelection = true;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Flag to indicate if transformations from all matched rules are returned. Otherwise,
        ///     transformations from only the first matched rule are returned.
        /// </summary>
        public bool UseEagerRuleSelection
        {
            get => this.useEagerRuleSelection;
            set => this.useEagerRuleSelection = value;
        }

        #endregion

        #region Private Properties

        private static ILogger Logger => LogManager.FrameworkLogger;

        #endregion

        #region Public Methods

        /// <summary>Adds a transform using a single replacement value and a global filter pattern.</summary>
        /// <param name="replacePattern">Regular expression pattern for replacing text.</param>
        /// <param name="replaceValue">The replacement value.</param>
        /// <param name="globalFilterPattern">Regular expression pattern for global filtering.</param>
        /// <example>
        ///     <code>
        ///         NameTransformer.AddRule("Model$", string.Empty);
        ///     </code>
        ///     This transformation rule looks for the substring “Model” terminating the ViewModel name and
        ///     strips out that substring (i.e. replace with string.Empty or “null string”).<br /> The “$” in
        ///     the first argument indicates that the pattern must match at the end of the source string. If
        ///     “Model” exists anywhere else, the pattern is not matched. Because this call did not include the
        ///     optional “globalFilterPattern” argument, this rule applies to all ViewModel names.<br /> This
        ///     rule yields the following results:
        ///     <list type="bullet">
        ///         <item>MainViewModel => MainView</item>
        ///         <item>ModelAirplaneViewModel => ModelAirplaneView</item>
        ///         <item>CustomerViewModelBase => CustomerViewModelBase</item>
        ///     </list>
        ///     For examples of the use of the global filter pattern check the defaults used in
        ///     <see cref="ViewLocator" />.
        /// </example>
        public void AddRule(string replacePattern,
                            string replaceValue,
                            string globalFilterPattern = null)
        {
            using var _ = Logger.GetMethodTracer(replacePattern, replaceValue, globalFilterPattern);

            AddRule(
                replacePattern,
                new[]
                    {
                        replaceValue
                    },
                globalFilterPattern);
        }

        /// <summary>Adds a transform using a list of replacement values and a global filter pattern.</summary>
        /// <param name="replacePattern">Regular expression pattern for replacing text.</param>
        /// <param name="replaceValueList">The list of replacement values.</param>
        /// <param name="globalFilterPattern">Regular expression pattern for global filtering.</param>
        /// <example>
        ///     <code>
        ///         NameTransformer.AddRule("Model$", new string[] { string.Empty });
        ///     </code>
        ///     This transformation rule looks for the substring “Model” terminating the ViewModel name and
        ///     strips out that substring (i.e. replace with string.Empty or “null string”).<br /> The “$” in
        ///     the first argument indicates that the pattern must match at the end of the source string. If
        ///     “Model” exists anywhere else, the pattern is not matched. Because this call did not include the
        ///     optional “globalFilterPattern” argument, this rule applies to all ViewModel names.<br /> This
        ///     rule yields the following results:
        ///     <list type="bullet">
        ///         <item>MainViewModel => MainView</item>
        ///         <item>ModelAirplaneViewModel => ModelAirplaneView</item>
        ///         <item>CustomerViewModelBase => CustomerViewModelBase</item>
        ///     </list>
        ///     For examples of the use of the global filter pattern check the defaults used in
        ///     <see cref="ViewLocator" />.
        /// </example>
        public void AddRule(string replacePattern,
                            IEnumerable<string> replaceValueList,
                            string globalFilterPattern = null)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            using var _ = Logger.GetMethodTracer(
                replacePattern,
                replaceValueList,
                globalFilterPattern);

            Add(
                new Rule
                    {
                        ReplacePattern = replacePattern,

                        // ReSharper disable once PossibleMultipleEnumeration
                        ReplacementValues = replaceValueList,
                        GlobalFilterPattern = globalFilterPattern
                    });
        }

        /// <summary>Gets the list of transformations for a given name based on the currently rule set.</summary>
        /// <param name="source">The name to transform into the resolved name list.</param>
        /// <returns>The transformed names.</returns>
        public IEnumerable<string> Transform(string source)
        {
            using var _ = Logger.GetMethodTracer(source);

            var nameList = new List<string>();
            var rules = this.Reverse();

            foreach (var rule in rules)
            {
                if (!string.IsNullOrEmpty(rule.GlobalFilterPattern) &&
                    !rule.GlobalFilterPatternRegex.IsMatch(source))
                {
                    continue;
                }

                if (!rule.ReplacePatternRegex.IsMatch(source))
                {
                    continue;
                }

                nameList.AddRange(
                    rule.ReplacementValues.Select(
                        repString => rule.ReplacePatternRegex.Replace(source, repString)));

                if (!this.useEagerRuleSelection)
                {
                    break;
                }
            }

            return nameList;
        }

        #endregion

        #region Nested Types

        /// <summary>A rule that describes a name transform.</summary>
        public class Rule
        {
            #region Constants and Fields

            /// <summary>Regular expression pattern for global filtering.</summary>
            public string GlobalFilterPattern;

            /// <summary>The list of replacement values</summary>
            public IEnumerable<string> ReplacementValues;

            /// <summary>Regular expression pattern for replacing text.</summary>
            public string ReplacePattern;

            private Regex globalFilterPatternRegex;
            private Regex replacePatternRegex;

            #endregion

            #region Public Properties

            /// <summary>Regular expression for global filtering.</summary>
            public Regex GlobalFilterPatternRegex => this.globalFilterPatternRegex ??
                                                     (this.globalFilterPatternRegex = new Regex(
                                                          this.GlobalFilterPattern,
                                                          Options));

            /// <summary>Regular expression for replacing text.</summary>
            public Regex ReplacePatternRegex => this.replacePatternRegex ??
                                                (this.replacePatternRegex = new Regex(
                                                     this.ReplacePattern,
                                                     Options));

            #endregion
        }

        #endregion
    }
}