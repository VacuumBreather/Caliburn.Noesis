﻿namespace Caliburn.Noesis {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    ///  Class for managing the list of rules for doing name transformation.
    /// </summary>
    public class NameTransformer : BindableCollection<NameTransformer.Rule> {

#if NET
        private const RegexOptions options = RegexOptions.Compiled;
#else
        private const RegexOptions options = RegexOptions.None;
#endif

        bool useEagerRuleSelection = true;

        /// <summary>
        /// Flag to indicate if transformations from all matched rules are returned. Otherwise, transformations from only the first matched rule are returned.
        /// </summary>
        public bool UseEagerRuleSelection {
            get { return this.useEagerRuleSelection; }
            set { this.useEagerRuleSelection = value; }
        }

        /// <summary>
        ///  Adds a transform using a single replacement value and a global filter pattern.
        /// </summary>
        /// <param name = "replacePattern">Regular expression pattern for replacing text</param>
        /// <param name = "replaceValue">The replacement value.</param>
        /// <param name = "globalFilterPattern">Regular expression pattern for global filtering</param>
        public void AddRule(string replacePattern, string replaceValue, string globalFilterPattern = null) {
            AddRule(replacePattern, new[] { replaceValue }, globalFilterPattern);
        }

        /// <summary>
        ///  Adds a transform using a list of replacement values and a global filter pattern.
        /// </summary>
        /// <param name = "replacePattern">Regular expression pattern for replacing text</param>
        /// <param name = "replaceValueList">The list of replacement values</param>
        /// <param name = "globalFilterPattern">Regular expression pattern for global filtering</param>
        public void AddRule(string replacePattern, IEnumerable<string> replaceValueList, string globalFilterPattern = null) {
            Add(new Rule {
                ReplacePattern = replacePattern,
                ReplacementValues = replaceValueList,
                GlobalFilterPattern = globalFilterPattern
            });
        }

        /// <summary>
        /// Gets the list of transformations for a given name.
        /// </summary>
        /// <param name = "source">The name to transform into the resolved name list</param>
        /// <returns>The transformed names.</returns>
        public IEnumerable<string> Transform(string source) {
            var nameList = new List<string>();
            var rules = this.Reverse();

            foreach(var rule in rules) {
                if(!string.IsNullOrEmpty(rule.GlobalFilterPattern) && !rule.GlobalFilterPatternRegex.IsMatch(source)) {
                    continue;
                }

                if(!rule.ReplacePatternRegex.IsMatch(source)) {
                    continue;
                }

                nameList.AddRange(
                    rule.ReplacementValues
                        .Select(repString => rule.ReplacePatternRegex.Replace(source, repString))
                    );

                if (!this.useEagerRuleSelection) {
                    break;
                }
            }

            return nameList;
        }

        ///<summary>
        /// A rule that describes a name transform.
        ///</summary>
        public class Rule {
            private Regex replacePatternRegex;
            private Regex globalFilterPatternRegex;

            /// <summary>
            /// Regular expression pattern for global filtering
            /// </summary>
            public string GlobalFilterPattern;

            /// <summary>
            /// Regular expression pattern for replacing text
            /// </summary>
            public string ReplacePattern;

            /// <summary>
            /// The list of replacement values
            /// </summary>
            public IEnumerable<string> ReplacementValues;

            /// <summary>
            /// Regular expression for global filtering
            /// </summary>
            public Regex GlobalFilterPatternRegex {
                get {
                    return this.globalFilterPatternRegex ?? (this.globalFilterPatternRegex = new Regex(this.GlobalFilterPattern, options));
                }
            }

            /// <summary>
            /// Regular expression for replacing text
            /// </summary>
            public Regex ReplacePatternRegex {
                get {
                    return this.replacePatternRegex ?? (this.replacePatternRegex = new Regex(this.ReplacePattern, options));
                }
            }
        }
    }
}