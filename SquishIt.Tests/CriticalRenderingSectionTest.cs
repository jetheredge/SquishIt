using System;
using System.Threading;
using System.Web;
using Moq;
using NUnit.Framework;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Helpers;

namespace SquishIt.Tests
{
    [TestFixture]
    public class CriticalRenderingSectionTest
    {
        [Test]
        public void UseMutex(AspNetHostingPermissionLevel level)
        {
            string testDir = Guid.NewGuid().ToString();

            var trustLevel = new Mock<ITrustLevel>();
            trustLevel.SetupGet(tl => tl.IsFullTrust).Returns(true);

            var filePathMutextProvider = new Mock<IFilePathMutexProvider>();
            filePathMutextProvider.Setup(mp => mp.GetMutexForPath(testDir)).Returns(new Mutex());

            using(new TrustLevelScope(trustLevel.Object))
            using(new FilePathMutexProviderScope(filePathMutextProvider.Object))
            {
                using(new CriticalRenderingSection(testDir))
                {
                    //do something
                }
            }

            trustLevel.VerifyAll();
            filePathMutextProvider.VerifyAll();
        }

        [Test]
        public void DontUseMutex()
        {
            string testDir = Guid.NewGuid().ToString();

            var trustLevel = new Mock<ITrustLevel>();
            trustLevel.SetupGet(tl => tl.IsFullTrust).Returns(false);

            //just want to be sure nothing is called on this
            var filePathMutextProvider = new Mock<IFilePathMutexProvider>(MockBehavior.Strict);

            using(new TrustLevelScope(trustLevel.Object))
            using(new FilePathMutexProviderScope(filePathMutextProvider.Object))
            {
                using(new CriticalRenderingSection(testDir))
                {
                    //do something
                }
            }

            trustLevel.VerifyAll();
            filePathMutextProvider.VerifyAll();
        }
    }
}
