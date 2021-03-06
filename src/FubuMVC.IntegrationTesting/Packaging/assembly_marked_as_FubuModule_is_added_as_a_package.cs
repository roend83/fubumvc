using System.Linq;
using FubuMVC.IntegrationTesting.Conneg;
using FubuTestingSupport;
using NUnit.Framework;
namespace FubuMVC.IntegrationTesting.Packaging
{
    [TestFixture]
    public class assembly_marked_as_FubuModule_is_added_as_a_package : FubuRegistryHarness
    {
        protected override void initializeBottles()
        {
            runBottles(@"link harness --clean-all");
            runFubu("packages harness --clean-all --remove-all");
        }

        [Test]
        public void has_endpoints_from_AssemblyPackage()
        {
            remote.All().EndpointsForAssembly("AssemblyPackage").Any().ShouldBeTrue();
        }
    }
}