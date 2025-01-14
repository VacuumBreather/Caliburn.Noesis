using System.Collections.Generic;
using System.Reflection;
using Caliburn.Noesis;

namespace Caliburn.Noesis.Samples.EarlySample
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
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
