using System;
using Xunit;
using Xunit.Sdk;

namespace SIO.Testing.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [TraitDiscoverer("SIO.Testing.Discoverers.ThenTraitDiscoverer", "SIO.Testing")]
    public class ThenAttribute : FactAttribute, ITraitAttribute { }
}
