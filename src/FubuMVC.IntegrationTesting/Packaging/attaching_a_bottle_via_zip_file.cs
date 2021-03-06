using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using FubuMVC.IntegrationTesting.Conneg;
using FubuTestingSupport;
using NUnit.Framework;
using TestPackage1;
using FubuCore;

namespace FubuMVC.IntegrationTesting.Packaging
{
    [TestFixture]
    public class attaching_a_bottle_via_zip_file : FubuRegistryHarness
    {
        protected override void initializeBottles()
        {
            runFubu("packages harness --clean-all");

            runBottles(@"
link harness --clean-all
init src/TestPackage1 pak1
create pak1 -o pak1.zip
");

            runFubu("install-pak pak1.zip harness");


        }





        [Test]
        public void read_image_from_a_package()
        {
            endpoints.GetAsset(AssetFolder.images, "icon-add-alt.png")
                .LengthShouldBe(3517)
                .ContentTypeShouldBe(MimeType.Png)
                .StatusCodeShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void read_asset_from_a_bottle()
        {
            endpoints.GetAsset(AssetFolder.scripts, "Pak1-A.js")
                .ContentTypeShouldBe(MimeType.Javascript)
                .ReadAsText().ShouldContain("var name = 'Pak1-A.js';");
        }

        [Test]
        public void load_actions_from_a_bottle()
        {
            IEnumerable<string> names = remote.All().EndpointsForAssembly("TestPackage1").Select(x => x.FirstActionDescription);

            var expectation =
                @"
StringController.SayHello()
JsonController.SendMessage()
ViewController.ShowView()
OneController.Report()
OneController.Query()
TwoController.Report()
TwoController.Query()
ThreeController.Report()
ThreeController.Query()
"
                    .ReadLines().Where(x => x.IsNotEmpty()).OrderBy(x => x);

            names.OrderBy(x => x)
                .ShouldHaveTheSameElementsAs(expectation);
        }

        [Test]
        public void can_invoke_a_json_endpoint_in_a_package()
        {
            // This endpoint is in the TestPackage1 package
            endpoints.PostJson(new JsonSerializedMessage
            {
                Name = "Jeremy"
            })
                .ReadAsJson<JsonSerializedMessage>()
                .Name.ShouldEqual("Jeremy");
        }

        [Test]
        public void can_invoke_string_endpoint_from_a_package()
        {
            endpoints.Get<TestPackage1.StringController>(x => x.SayHello())
                .ReadAsText()
                .ShouldEqual("Hello");
        }
    }
}