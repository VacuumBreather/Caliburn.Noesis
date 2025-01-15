namespace Caliburn.Noesis
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
#endif
    using System.Collections.Generic;

    /// <summary>
    ///   A strategy for determining which view model to use for a given view.
    /// </summary>
    public class ViewModelLocator
    {
        const string DefaultViewSuffix = "View";

        static readonly ILog Log = LogManager.GetLog(typeof(ViewModelLocator));
        //These fields are used for configuring the default type mappings. They can be changed using ConfigureTypeMappings().
        string defaultSubNsViews;
        string defaultSubNsViewModels;
        bool useNameSuffixesInMappings;
        string nameFormat;
        string viewModelSuffix;
        readonly List<string> ViewSuffixList = new List<string>();
        bool includeViewSuffixInVmNames;

        ///<summary>
        /// Used to transform names.
        ///</summary>
        public readonly NameTransformer NameTransformer = new NameTransformer();

        /// <summary>
        /// The name of the capture group used as a marker for rules that return interface types
        /// </summary>
        public static string InterfaceCaptureGroupName = "isinterface";

        /// <summary>
        /// Gets the source of assemblies that contain view and view-model types relevant to this instance.
        /// </summary>
        public AssemblySource AssemblySource { get; }

        /// <summary>
        /// The service provider used to resolve views.
        /// </summary>
        private IServiceLocator ServiceProvider { get; }

        public ViewModelLocator(AssemblySource assemblySource, IServiceLocator serviceProvider)
        {
            TransformName = InternalTransformName;
            LocateTypeForViewType = InternalLocateTypeForViewType;
            LocateForViewType = InternalLocateForViewType;
            LocateForView = InternalLocateForView;
            
            AssemblySource = assemblySource;
            ServiceProvider = serviceProvider;

            var configuration = new TypeMappingConfiguration();

            ConfigureTypeMappings(configuration);
        }

        /// <summary>
        /// Specifies how type mappings are created, including default type mappings. Calling this method will
        /// clear all existing name transformation rules and create new default type mappings according to the
        /// configuration.
        /// </summary>
        /// <param name="config">An instance of TypeMappingConfiguration that provides the settings for configuration</param>
        public void ConfigureTypeMappings(TypeMappingConfiguration config)
        {
            if (String.IsNullOrEmpty(config.DefaultSubNamespaceForViews))
            {
                throw new ArgumentException("DefaultSubNamespaceForViews field cannot be blank.");
            }

            if (String.IsNullOrEmpty(config.DefaultSubNamespaceForViewModels))
            {
                throw new ArgumentException("DefaultSubNamespaceForViewModels field cannot be blank.");
            }

            if (String.IsNullOrEmpty(config.NameFormat))
            {
                throw new ArgumentException("NameFormat field cannot be blank.");
            }

            if (!IsNameFormatValidFormat(config.NameFormat))
            {
                throw new ArgumentException("NameFormat field must contain {0} and {1} placeholders.");
            }
            NameTransformer.Clear();
            ViewSuffixList.Clear();

            defaultSubNsViews = config.DefaultSubNamespaceForViews;
            defaultSubNsViewModels = config.DefaultSubNamespaceForViewModels;
            nameFormat = config.NameFormat;
            useNameSuffixesInMappings = config.UseNameSuffixesInMappings;
            viewModelSuffix = config.ViewModelSuffix;
            ViewSuffixList.AddRange(config.ViewSuffixList);
            includeViewSuffixInVmNames = config.IncludeViewSuffixInViewModelNames;

            SetAllDefaults();
        }


        internal static bool IsNameFormatValidFormat(string formatToValidate)
        {
            return formatToValidate.Contains("{0}") && formatToValidate.Contains("{1}");
        }

        private void SetAllDefaults()
        {
            if (useNameSuffixesInMappings)
            {
                //Add support for all view suffixes
                ViewSuffixList.Apply(AddDefaultTypeMapping);
            }
            else
            {
                AddSubNamespaceMapping(defaultSubNsViews, defaultSubNsViewModels);
            }
        }

        /// <summary>
        /// Adds a default type mapping using the standard namespace mapping convention
        /// </summary>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddDefaultTypeMapping(string viewSuffix = DefaultViewSuffix)
        {
            if (!useNameSuffixesInMappings)
            {
                return;
            }

            //Check for <Namespace>.<BaseName><ViewSuffix> construct
            AddNamespaceMapping(String.Empty, String.Empty, viewSuffix);

            //Check for <Namespace>.Views.<NameSpace>.<BaseName><ViewSuffix> construct
            AddSubNamespaceMapping(defaultSubNsViews, defaultSubNsViewModels, viewSuffix);
        }

        /// <summary>
        /// Adds a standard type mapping based on namespace RegEx replace and filter patterns
        /// </summary>
        /// <param name="nsSourceReplaceRegEx">RegEx replace pattern for source namespace</param>
        /// <param name="nsSourceFilterRegEx">RegEx filter pattern for source namespace</param>
        /// <param name="nsTargetsRegEx">Array of RegEx replace values for target namespaces</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddTypeMapping(string nsSourceReplaceRegEx, string nsSourceFilterRegEx, string[] nsTargetsRegEx, string viewSuffix = DefaultViewSuffix)
        {
            var replist = new List<string>();
            Action<string> func;

            const string basegrp = "${basename}";
            var interfacegrp = "${" + InterfaceCaptureGroupName + "}";

            if (useNameSuffixesInMappings)
            {
                if (viewModelSuffix.Contains(viewSuffix) || !includeViewSuffixInVmNames)
                {
                    var nameregex = String.Format(nameFormat, basegrp, viewModelSuffix);
                    func = t =>
                    {
                        replist.Add(t + "I" + nameregex + interfacegrp);
                        replist.Add(t + "I" + basegrp + interfacegrp);
                        replist.Add(t + nameregex);
                        replist.Add(t + basegrp);
                    };
                }
                else
                {
                    var nameregex = String.Format(nameFormat, basegrp, "${suffix}" + viewModelSuffix);
                    func = t =>
                    {
                        replist.Add(t + "I" + nameregex + interfacegrp);
                        replist.Add(t + nameregex);
                    };
                }
            }
            else
            {
                func = t =>
                {
                    replist.Add(t + "I" + basegrp + interfacegrp);
                    replist.Add(t + basegrp);
                };
            }

            nsTargetsRegEx.ToList().Apply(t => func(t));

            string suffix = useNameSuffixesInMappings ? viewSuffix : String.Empty;

            var srcfilterregx = String.IsNullOrEmpty(nsSourceFilterRegEx)
                ? null
                : String.Concat(nsSourceFilterRegEx, String.Format(nameFormat, RegExHelper.NameRegEx, suffix), "$");
            var rxbase = RegExHelper.GetNameCaptureGroup("basename");
            var rxsuffix = RegExHelper.GetCaptureGroup("suffix", suffix);

            //Add a dummy capture group -- place after the "$" so it can never capture anything
            var rxinterface = RegExHelper.GetCaptureGroup(InterfaceCaptureGroupName, String.Empty);
            NameTransformer.AddRule(
                String.Concat(nsSourceReplaceRegEx, String.Format(nameFormat, rxbase, rxsuffix), "$", rxinterface),
                replist.ToArray(),
                srcfilterregx
            );
        }

        /// <summary>
        /// Adds a standard type mapping based on namespace RegEx replace and filter patterns
        /// </summary>
        /// <param name="nsSourceReplaceRegEx">RegEx replace pattern for source namespace</param>
        /// <param name="nsSourceFilterRegEx">RegEx filter pattern for source namespace</param>
        /// <param name="nsTargetRegEx">RegEx replace value for target namespace</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddTypeMapping(string nsSourceReplaceRegEx, string nsSourceFilterRegEx, string nsTargetRegEx, string viewSuffix = DefaultViewSuffix)
        {
            AddTypeMapping(nsSourceReplaceRegEx, nsSourceFilterRegEx, new[] { nsTargetRegEx }, viewSuffix);
        }

        /// <summary>
        /// Adds a standard type mapping based on simple namespace mapping
        /// </summary>
        /// <param name="nsSource">Namespace of source type</param>
        /// <param name="nsTargets">Namespaces of target type as an array</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddNamespaceMapping(string nsSource, string[] nsTargets, string viewSuffix = DefaultViewSuffix)
        {
            //need to terminate with "." in order to concatenate with type name later
            var nsencoded = RegExHelper.NamespaceToRegEx(nsSource + ".");

            //Start pattern search from beginning of string ("^")
            //unless original string was blank (i.e. special case to indicate "append target to source")
            if (!String.IsNullOrEmpty(nsSource))
            {
                nsencoded = "^" + nsencoded;
            }

            //Capture namespace as "origns" in case we need to use it in the output in the future
            var nsreplace = RegExHelper.GetCaptureGroup("origns", nsencoded);

            var nsTargetsRegEx = nsTargets.Select(t => t + ".").ToArray();
            AddTypeMapping(nsreplace, null, nsTargetsRegEx, viewSuffix);
        }

        /// <summary>
        /// Adds a standard type mapping based on simple namespace mapping
        /// </summary>
        /// <param name="nsSource">Namespace of source type</param>
        /// <param name="nsTarget">Namespace of target type</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddNamespaceMapping(string nsSource, string nsTarget, string viewSuffix = DefaultViewSuffix)
        {
            AddNamespaceMapping(nsSource, new[] { nsTarget }, viewSuffix);
        }

        /// <summary>
        /// Adds a standard type mapping by substituting one subnamespace for another
        /// </summary>
        /// <param name="nsSource">Subnamespace of source type</param>
        /// <param name="nsTargets">Subnamespaces of target type as an array</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddSubNamespaceMapping(string nsSource, string[] nsTargets, string viewSuffix = DefaultViewSuffix)
        {
            //need to terminate with "." in order to concatenate with type name later
            var nsencoded = RegExHelper.NamespaceToRegEx(nsSource + ".");

            string rxbeforetgt, rxaftersrc, rxaftertgt;
            string rxbeforesrc = rxbeforetgt = rxaftersrc = rxaftertgt = String.Empty;

            if (!String.IsNullOrEmpty(nsSource))
            {
                if (!nsSource.StartsWith("*"))
                {
                    rxbeforesrc = RegExHelper.GetNamespaceCaptureGroup("nsbefore");
                    rxbeforetgt = @"${nsbefore}";
                }

                if (!nsSource.EndsWith("*"))
                {
                    rxaftersrc = RegExHelper.GetNamespaceCaptureGroup("nsafter");
                    rxaftertgt = "${nsafter}";
                }
            }

            var rxmid = RegExHelper.GetCaptureGroup("subns", nsencoded);
            var nsreplace = String.Concat(rxbeforesrc, rxmid, rxaftersrc);

            var nsTargetsRegEx = nsTargets.Select(t => String.Concat(rxbeforetgt, t, ".", rxaftertgt)).ToArray();
            AddTypeMapping(nsreplace, null, nsTargetsRegEx, viewSuffix);
        }

        /// <summary>
        /// Adds a standard type mapping by substituting one subnamespace for another
        /// </summary>
        /// <param name="nsSource">Subnamespace of source type</param>
        /// <param name="nsTarget">Subnamespace of target type</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddSubNamespaceMapping(string nsSource, string nsTarget, string viewSuffix = DefaultViewSuffix)
        {
            AddSubNamespaceMapping(nsSource, new[] { nsTarget }, viewSuffix);
        }

        /// <summary>
        ///   Makes a type name into an interface name.
        /// </summary>
        /// <param name = "typeName">The part.</param>
        /// <returns></returns>
        public static string MakeInterface(string typeName)
        {
            var suffix = string.Empty;
            if (typeName.Contains("[["))
            {
                //generic type
                var genericParStart = typeName.IndexOf("[[");
                suffix = typeName.Substring(genericParStart);
                typeName = typeName.Remove(genericParStart);
            }

            var index = typeName.LastIndexOf(".");
            return typeName.Insert(index + 1, "I") + suffix;
        }

        /// <summary>
        /// Transforms a View type name into all of its possible ViewModel type names. Accepts a flag
        /// to include or exclude interface types.
        /// </summary>
        /// <returns>Enumeration of transformed names</returns>
        /// <remarks>Arguments:
        /// typeName = The name of the View type being resolved to its companion ViewModel.
        /// includeInterfaces = Flag to indicate if interface types are included
        /// </remarks>
        public Func<string, bool, IEnumerable<string>> TransformName { get; set; }
            
        private IEnumerable<string> InternalTransformName(string typeName, bool includeInterfaces)
        {
            Func<string, string> getReplaceString;
            if (includeInterfaces)
            {
                getReplaceString = r => r;
            }
            else
            {
                var interfacegrpregex = @"\${" + InterfaceCaptureGroupName + @"}$";
                getReplaceString = r => Regex.IsMatch(r, interfacegrpregex) ? String.Empty : r;
            }
            return NameTransformer.Transform(typeName, getReplaceString).Where(n => n != String.Empty);
        }

        /// <summary>
        ///   Determines the view model type based on the specified view type.
        /// </summary>
        /// <returns>The view model type.</returns>
        /// <remarks>
        ///   Pass the view type and receive a view model type. Pass true for the second parameter to search for interfaces.
        /// </remarks>
        public Func<Type, bool, Type> LocateTypeForViewType { get; set; }

        private Type InternalLocateTypeForViewType(Type viewType, bool searchForInterface)
        {
            var typeName = viewType.FullName;

            var viewModelTypeList = TransformName(typeName, searchForInterface).ToList();

            var viewModelType = AssemblySource.FindTypeByNames(viewModelTypeList);

            if (viewModelType == null)
            {
                Log.Warn("View Model not found. Searched: {0}.", string.Join(", ", viewModelTypeList.ToArray()));
            }

            return viewModelType;
        }

        /// <summary>
        ///   Locates the view model for the specified view type.
        /// </summary>
        /// <returns>The view model.</returns>
        /// <remarks>
        ///   Pass the view type as a parameter and receive a view model instance.
        /// </remarks>
        public Func<Type, object> LocateForViewType { get; set; }

        private object InternalLocateForViewType(Type viewType)
        {
            var viewModelType = LocateTypeForViewType(viewType, false);

            if (viewModelType != null)
            {
                var viewModel = ServiceProvider.GetInstance(viewModelType, null);
                if (viewModel != null)
                {
                    return viewModel;
                }
            }

            viewModelType = LocateTypeForViewType(viewType, true);

            return viewModelType != null
                       ? ServiceProvider.GetInstance(viewModelType, null)
                       : null;
        }

        /// <summary>
        ///   Locates the view model for the specified view instance.
        /// </summary>
        /// <returns>The view model.</returns>
        /// <remarks>
        ///   Pass the view instance as a parameters and receive a view model instance.
        /// </remarks>
        public Func<object, object> LocateForView { get; set; }

        private object InternalLocateForView(object view)
        {
            if (view == null)
            {
                return null;
            }

            var frameworkElement = view as FrameworkElement;
            if (frameworkElement != null && frameworkElement.DataContext != null)
            {
                return frameworkElement.DataContext;
            }

            return LocateForViewType(view.GetType());
        }
    }
}
