using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
#if UNITY_5_5_OR_NEWER
using global::Noesis;
#else
    using System.Windows;
    using System.Windows.Controls;
#endif

namespace Caliburn.Noesis
{
    /// <summary>
    ///   A strategy for determining which view to use for a given model.
    /// </summary>
    public class ViewLocator
    {
        static readonly ILog Log = LogManager.GetLog(typeof(ViewLocator));

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
        public NameTransformer NameTransformer { get; set; } = new NameTransformer();

        /// <summary>
        ///   Separator used when resolving View names for context instances.
        /// </summary>
        public string ContextSeparator = ".";

        /// <summary>
        /// Gets the source of assemblies that contain view and view-model types relevant to this instance.
        /// </summary>
        public AssemblySource AssemblySource { get; }

        /// <summary>
        /// The service provider used to resolve views.
        /// </summary>
        private IServiceLocator ServiceLocator { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLocator" /> class.
        /// </summary>
        /// <param name="assemblySource">The source of assemblies that contain view and view-model types relevant to this instance.</param>
        /// <param name="serviceProvider">The service provider used to resolve views.</param>
        public ViewLocator(AssemblySource assemblySource, IServiceLocator serviceProvider)
        {
            GetOrCreateViewType = InternalGetOrCreateViewType;
            ModifyModelTypeAtDesignTime = InternalModifyModelTypeAtDesignTime;
            TransformName = InternalTransformName;
            LocateTypeForModelType = InternalLocateTypeForModelType;
            LocateForModelType = InternalLocateForModelType;
            LocateForModel = InternalLocateForModel;
            DeterminePackUriFromType = InternalDeterminePackUriFromType;

            AssemblySource = assemblySource;
            ServiceLocator = serviceProvider;
            
            ConfigureTypeMappings(new TypeMappingConfiguration());
        }

        /// <summary>
        /// Specifies how type mappings are created, including default type mappings. Calling this method will
        /// clear all existing name transformation rules and create new default type mappings according to the
        /// configuration.
        /// </summary>
        /// <param name="config">An instance of TypeMappingConfiguration that provides the settings for configuration</param>
        public void ConfigureTypeMappings(TypeMappingConfiguration config)
        {
            if (string.IsNullOrEmpty(config.DefaultSubNamespaceForViews))
            {
                throw new ArgumentException("DefaultSubNamespaceForViews field cannot be blank.");
            }

            if (string.IsNullOrEmpty(config.DefaultSubNamespaceForViewModels))
            {
                throw new ArgumentException("DefaultSubNamespaceForViewModels field cannot be blank.");
            }

            if (string.IsNullOrEmpty(config.NameFormat))
            {
                throw new ArgumentException("NameFormat field cannot be blank.");
            }
            if (!IsNameFormatValidFormat(config.NameFormat))
            {
                throw new ArgumentException("NameFormat field must contain {0} and {1} placeholders.");
            }

            if (NameTransformer.Any())
            {
                NameTransformer.Clear();
            }
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


        private void SetAllDefaults()
        {
            if (useNameSuffixesInMappings)
            {
                //Add support for all view suffixes
                ViewSuffixList.Apply(AddDefaultTypeMapping);
            }
            else
            {
                AddSubNamespaceMapping(defaultSubNsViewModels, defaultSubNsViews);
            }
        }

        /// <summary>
        /// Adds a default type mapping using the standard namespace mapping convention
        /// </summary>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddDefaultTypeMapping(string viewSuffix = "View")
        {
            if (!useNameSuffixesInMappings)
            {
                return;
            }

            //Check for <Namespace>.<BaseName><ViewSuffix> construct
            AddNamespaceMapping(String.Empty, String.Empty, viewSuffix);

            //Check for <Namespace>.ViewModels.<NameSpace>.<BaseName><ViewSuffix> construct
            AddSubNamespaceMapping(defaultSubNsViewModels, defaultSubNsViews, viewSuffix);
        }

        /// <summary>
        /// This method registers a View suffix or synonym so that View Context resolution works properly.
        /// It is automatically called internally when calling AddNamespaceMapping(), AddDefaultTypeMapping(),
        /// or AddTypeMapping(). It should not need to be called explicitly unless a rule that handles synonyms
        /// is added directly through the NameTransformer.
        /// </summary>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View".</param>
        public void RegisterViewSuffix(string viewSuffix)
        {
            if (ViewSuffixList.Count(s => s == viewSuffix) == 0)
            {
                ViewSuffixList.Add(viewSuffix);
            }
        }

        /// <summary>
        /// Adds a standard type mapping based on namespace RegEx replace and filter patterns
        /// </summary>
        /// <param name="nsSourceReplaceRegEx">RegEx replace pattern for source namespace</param>
        /// <param name="nsSourceFilterRegEx">RegEx filter pattern for source namespace</param>
        /// <param name="nsTargetsRegEx">Array of RegEx replace values for target namespaces</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddTypeMapping(string nsSourceReplaceRegEx, string nsSourceFilterRegEx,
            string[] nsTargetsRegEx, string viewSuffix = "View")
        {
            RegisterViewSuffix(viewSuffix);

            var replist = new List<string>();
            var repsuffix = useNameSuffixesInMappings ? viewSuffix : String.Empty;
            const string basegrp = "${basename}";

            foreach (var t in nsTargetsRegEx)
            {
                replist.Add(t + string.Format(nameFormat, basegrp, repsuffix));
            }

            var rxbase = RegExHelper.GetNameCaptureGroup("basename");
            var suffix = String.Empty;
            if (useNameSuffixesInMappings)
            {
                suffix = viewModelSuffix;
                if (!viewModelSuffix.Contains(viewSuffix) && includeViewSuffixInVmNames)
                {
                    suffix = viewSuffix + suffix;
                }
            }

            var rxsrcfilter = string.IsNullOrEmpty(nsSourceFilterRegEx)
                ? null
                : string.Concat(nsSourceFilterRegEx, string.Format(nameFormat, RegExHelper.NameRegEx, suffix), "$");
            var rxsuffix = RegExHelper.GetCaptureGroup("suffix", suffix);

            NameTransformer.AddRule(
                String.Concat(nsSourceReplaceRegEx, string.Format(nameFormat, rxbase, rxsuffix), "$"),
                replist.ToArray(),
                rxsrcfilter
            );
        }

        internal static bool IsNameFormatValidFormat(string formatToValidate)
        {
            return formatToValidate.Contains("{0}") && formatToValidate.Contains("{1}");
        }

        /// <summary>
        /// Adds a standard type mapping based on namespace RegEx replace and filter patterns
        /// </summary>
        /// <param name="nsSourceReplaceRegEx">RegEx replace pattern for source namespace</param>
        /// <param name="nsSourceFilterRegEx">RegEx filter pattern for source namespace</param>
        /// <param name="nsTargetRegEx">RegEx replace value for target namespace</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddTypeMapping(string nsSourceReplaceRegEx, string nsSourceFilterRegEx, string nsTargetRegEx,
            string viewSuffix = "View")
        {
            AddTypeMapping(nsSourceReplaceRegEx, nsSourceFilterRegEx, new[] { nsTargetRegEx }, viewSuffix);
        }

        /// <summary>
        /// Adds a standard type mapping based on simple namespace mapping
        /// </summary>
        /// <param name="nsSource">Namespace of source type</param>
        /// <param name="nsTargets">Namespaces of target type as an array</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddNamespaceMapping(string nsSource, string[] nsTargets, string viewSuffix = "View")
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
        public void AddNamespaceMapping(string nsSource, string nsTarget, string viewSuffix = "View")
        {
            AddNamespaceMapping(nsSource, new[] { nsTarget }, viewSuffix);
        }

        /// <summary>
        /// Adds a standard type mapping by substituting one subnamespace for another
        /// </summary>
        /// <param name="nsSource">Subnamespace of source type</param>
        /// <param name="nsTargets">Subnamespaces of target type as an array</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public void AddSubNamespaceMapping(string nsSource, string[] nsTargets, string viewSuffix = "View")
        {
            //need to terminate with "." in order to concatenate with type name later
            var nsencoded = RegExHelper.NamespaceToRegEx(nsSource + ".");

            string rxbeforetgt, rxaftersrc, rxaftertgt;
            var rxbeforesrc = rxbeforetgt = rxaftersrc = rxaftertgt = String.Empty;

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
        public void AddSubNamespaceMapping(string nsSource, string nsTarget, string viewSuffix = "View")
        {
            AddSubNamespaceMapping(nsSource, new[] { nsTarget }, viewSuffix);
        }

        /// <summary>
        ///   Retrieves the view from the IoC container or tries to create it if not found.
        /// </summary>
        /// <remarks>
        ///   Pass the type of view as a parameter and recieve an instance of the view.
        /// </remarks>
        public Func<Type, UIElement> GetOrCreateViewType { get; set; }
        
        private UIElement InternalGetOrCreateViewType(Type viewType)
        {
            var view = ServiceLocator.GetAllInstances(viewType)
                .FirstOrDefault() as UIElement;

            if (view == null)
            {
                if (viewType.IsInterface || viewType.IsAbstract || !typeof(UIElement).IsAssignableFrom(viewType))
                {
                    return new TextBlock { Text = string.Format("Cannot create {0}.", viewType.FullName) };
                }
                
                view = (UIElement)System.Activator.CreateInstance(viewType);
            }

            view.SetValue(AttachedProperties.ServiceLocatorProperty, ServiceLocator);
            view.SetValue(AttachedProperties.ViewLocatorProperty, this);

            return view;
        }

        /// <summary>
        /// Modifies the name of the type to be used at design time.
        /// </summary>
        public Func<string, string> ModifyModelTypeAtDesignTime { get; set; }

        private static string InternalModifyModelTypeAtDesignTime(string modelTypeName)
        {
            if (modelTypeName.StartsWith("_"))
            {
                var index = modelTypeName.IndexOf('.');
                modelTypeName = modelTypeName.Substring(index + 1);
                index = modelTypeName.IndexOf('.');
                modelTypeName = modelTypeName.Substring(index + 1);
            }

            return modelTypeName;
        }

        /// <summary>
        /// Transforms a ViewModel type name into all of its possible View type names. Optionally accepts an instance
        /// of context object
        /// </summary>
        /// <returns>Enumeration of transformed names</returns>
        /// <remarks>Arguments:
        /// typeName = The name of the ViewModel type being resolved to its companion View.
        /// context = An instance of the context or null.
        /// </remarks>
        public Func<string, object, IEnumerable<string>> TransformName { get; set; }
        
        private IEnumerable<string> InternalTransformName(string typeName, object context)
        {
            Func<string, string> getReplaceString;
            if (context == null)
            {
                getReplaceString = r => r;
                return NameTransformer.Transform(typeName, getReplaceString);
            }

            var contextstr = ContextSeparator + context;
            string grpsuffix = String.Empty;
            if (useNameSuffixesInMappings)
            {
                //Create RegEx for matching any of the synonyms registered
                var synonymregex = "(" + String.Join("|", ViewSuffixList.ToArray()) + ")";
                grpsuffix = RegExHelper.GetCaptureGroup("suffix", synonymregex);
            }

            const string grpbase = @"\${basename}";
            var patternregex = String.Format(nameFormat, grpbase, grpsuffix) + "$";

            //Strip out any synonym by just using contents of base capture group with context string
            var replaceregex = "${basename}" + contextstr;

            //Strip out the synonym
            getReplaceString = r => Regex.Replace(r, patternregex, replaceregex);

            //Return only the names for the context
            return NameTransformer.Transform(typeName, getReplaceString).Where(n => n.EndsWith(contextstr));
        }

        /// <summary>
        ///   Locates the view type based on the specified model type.
        /// </summary>
        /// <returns>The view.</returns>
        /// <remarks>
        ///   Pass the model type, display location (or null) and the context instance (or null) as parameters and receive a view type.
        /// </remarks>
        public Func<Type, DependencyObject, object, Type> LocateTypeForModelType { get; set; }
            
        private Type InternalLocateTypeForModelType(Type modelType, DependencyObject displayLocation, object context)
        {
            var viewTypeName = modelType.FullName;

            if (View.InDesignMode)
            {
                viewTypeName = ModifyModelTypeAtDesignTime(viewTypeName);
            }

            viewTypeName = viewTypeName.Substring(
                0,
                viewTypeName.IndexOf('`') < 0
                    ? viewTypeName.Length
                    : viewTypeName.IndexOf('`')
            );

            var viewTypeList = TransformName(viewTypeName, context);
            var viewType = AssemblySource.FindTypeByNames(viewTypeList);

            if (viewType == null)
            {
                Log.Warn("View not found. Searched: {0}.", string.Join(", ", viewTypeList.ToArray()));
            }

            return viewType;
        }

        /// <summary>
        ///   Locates the view for the specified model type.
        /// </summary>
        /// <returns>The view.</returns>
        /// <remarks>
        ///   Pass the model type, display location (or null) and the context instance (or null) as parameters and receive a view instance.
        /// </remarks>
        public Func<Type, DependencyObject, object, UIElement> LocateForModelType { get; set; }
                
        private UIElement InternalLocateForModelType(Type modelType, DependencyObject displayLocation, object context)
        {
            var viewType = LocateTypeForModelType(modelType, displayLocation, context);

            return viewType == null
                ? new TextBlock { Text = string.Format("Cannot find view for {0}.", modelType) }
                : GetOrCreateViewType(viewType);
        }

        /// <summary>
        ///   Locates the view for the specified model instance.
        /// </summary>
        /// <returns>The view.</returns>
        /// <remarks>
        ///   Pass the model instance, display location (or null) and the context (or null) as parameters and receive a view instance.
        /// </remarks>
        public Func<object, DependencyObject, object, UIElement> LocateForModel { get; set; }

        private UIElement InternalLocateForModel(object model, DependencyObject displayLocation, object context)
        {
            var viewAware = model as IViewAware;
            if (viewAware != null)
            {
                var view = viewAware.GetView(context) as UIElement;
                if (view != null)
                {
                    Log.Info("Using cached view for {0}.", model);
                    return view;
                }
            }

            return LocateForModelType(model.GetType(), displayLocation, context);
        }



        /// <summary>
        /// Transforms a view type into a pack uri.
        /// </summary>
        public Func<Type, Type, string> DeterminePackUriFromType { get; set; }
            
        private string InternalDeterminePackUriFromType(Type viewModelType, Type viewType)
        {
            var assemblyName = viewType.Assembly.GetAssemblyName();
            var applicationAssemblyName = ServiceLocator.GetType().Assembly.GetAssemblyName();
            var viewTypeName = viewType.FullName;

            if (viewTypeName.StartsWith(assemblyName))
                viewTypeName = viewTypeName.Substring(assemblyName.Length);

            var uri = viewTypeName.Replace(".", "/") + ".xaml";

            if (!applicationAssemblyName.Equals(assemblyName))
            {
                return "/" + assemblyName + ";component" + uri;
            }

            return uri;
        }
    }
}
