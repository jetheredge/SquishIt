using NUnit.Framework;

namespace SquishIt.Tests
{
    //TODO: this is a hack until issues with dependencies in SquishIt.AspNet are addressed
    public abstract class ConfigurationEstablishingTest
    {
        [TestFixtureSetUp]
        public void EstablishConfiguration()
        {
            SquishIt.Framework.Configuration.Apply(SquishIt.AspNet.ConfigurationLoader.RegisterPlatform);
        }
    }
}
