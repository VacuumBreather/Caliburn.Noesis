using System.Collections.Generic;
using System.Reflection;
using Caliburn.Noesis;
using Testing;
using UnityEngine;

public class Bootstrapper : BootstrapperBase<ShellViewModel>
{
    protected override IEnumerable<Assembly> SelectAssemblies()
    {
        return new[] { GetType().Assembly };
    }
}
