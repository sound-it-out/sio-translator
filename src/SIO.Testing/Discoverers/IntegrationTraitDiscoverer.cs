using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SIO.Testing.Discoverers
{
    public class IntegrationTraitDiscoverer : ITraitDiscoverer
    {
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            yield return new KeyValuePair<string, string>("Category", "Integration");
        }
    }
}
