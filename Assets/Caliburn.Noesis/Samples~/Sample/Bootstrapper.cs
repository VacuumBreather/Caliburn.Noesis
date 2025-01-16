using System;
using System.Collections.Generic;
using System.Reflection;

namespace Caliburn.Noesis.Samples
{
    public class Bootstrapper : BootstrapperBase
    {
        static Bootstrapper()
        {
            LogManager.GetLog = type => new DebugLog(type);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] { GetType().Assembly };
        }
    }
}
