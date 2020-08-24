using System;
using Xunit;
using Xunit.Sdk;

namespace SIO.Testing.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [TraitDiscoverer("SIO.Testing.Discoverers.IntegrationTraitDiscoverer", "SIO.Testing")]
    public class IntegrationAttribute : FactAttribute, ITraitAttribute { }
}
