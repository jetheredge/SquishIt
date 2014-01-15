using System;
using NUnit.Framework;
using SquishIt.Framework.CSS;

namespace SquishIt.Tests
{
    [TestFixture]
    public class RelativePathAdapterTest
    {
        [Test]
        public void Between_Throws_If_No_Common_Root()
        {
            var from = "C:\\asdfasdfasdf\\asdfasdrqwettadsf";
            var to = "D:\\asdfasdfasewtertwasdf\\eewtyeryredag";

            var ex = Assert.Throws<InvalidOperationException>(() => RelativePathAdapter.Between(from, to));
            Assert.NotNull(ex);
            Assert.AreEqual("Can't calculate relative distance between '" + from + "' and '" + to + "' because they do not have a shared base.", ex.Message);
        }

        [Test, Platform(Exclude = "Unix, Linux, Mono")]
        public void DoesNotErrorOnNetworkSharePath()
        {
            var from = @"\\network\website\assets\css\main\";
            var to = @"\\network\website\Content\style.css";

            var adapter = RelativePathAdapter.Between(from, to);

            Assert.IsNotNull(adapter);
        }

        [Test, Platform(Exclude = "Unix, Linux, Mono")]
        public void DoesNotErrorOnDefaultDriveShare()
        {
            //variant of share path that is a default drive share (e.g. \\server\d$\style.css)
            var from = @"\\network\d$\website\assets\css\main\";
            var to = @"\\network\d$\website\Content\style.css";

            var adapter = RelativePathAdapter.Between(from, to);

            Assert.IsNotNull(adapter);
        }

    }
}
