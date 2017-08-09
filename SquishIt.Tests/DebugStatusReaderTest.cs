using System.Web;
using Moq;
using NUnit.Framework;
using SquishIt.AspNet.Utilities;
using SquishIt.Framework.Utilities;
using SquishIt.Tests.Helpers;

namespace SquishIt.Tests
{
    [TestFixture]
    public class DebugStatusReaderTest
    {
        [Test]
        public void ForceRelease()
        {
            //shouldn't touch anything on these
            var httpContext = new Mock<HttpContextBase>(MockBehavior.Strict);
            var machineConfigReader = new Mock<IMachineConfigReader>(MockBehavior.Strict);

            using(new HttpContextScope(httpContext.Object))
            {
                var reader = new DebugStatusReader(machineConfigReader.Object);
                reader.ForceRelease();
                Assert.IsFalse(reader.IsDebuggingEnabled());
            }

            httpContext.VerifyAll();
            machineConfigReader.VerifyAll();
        }

        [Test]
        public void ForceDebug()
        {
            //shouldn't touch anything on these
            var httpContext = new Mock<HttpContextBase>(MockBehavior.Strict);
            var machineConfigReader = new Mock<IMachineConfigReader>(MockBehavior.Strict);

            using(new HttpContextScope(httpContext.Object))
            {
                var reader = new DebugStatusReader(machineConfigReader.Object);
                reader.ForceDebug();
                Assert.IsTrue(reader.IsDebuggingEnabled());
            }

            httpContext.VerifyAll();
            machineConfigReader.VerifyAll();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Predicate(bool predicateReturn)
        {
            //shouldn't touch anything on these
            var httpContext = new Mock<HttpContextBase>(MockBehavior.Strict);
            var machineConfigReader = new Mock<IMachineConfigReader>(MockBehavior.Strict);

            using (new HttpContextScope(httpContext.Object))
            {
                var reader = new DebugStatusReader(machineConfigReader.Object);
                reader.ForceRelease();
                Assert.AreEqual(predicateReturn, reader.IsDebuggingEnabled(() => predicateReturn));
            }

            httpContext.VerifyAll();
            machineConfigReader.VerifyAll();
        }

        [Test]
        public void NullHttpContext()
        {
            var machineConfigReader = new Mock<IMachineConfigReader>(MockBehavior.Strict);

            var reader = new DebugStatusReader(machineConfigReader.Object);
            Assert.IsFalse(reader.IsDebuggingEnabled());

            machineConfigReader.VerifyAll();
        }

        [Test]
        public void DebuggingExplicitlyDisabledInConfiguration()
        {
            var httpContext = new Mock<HttpContextBase>();
            var machineConfigReader = new Mock<IMachineConfigReader>(MockBehavior.Strict);

            httpContext.SetupGet(hc => hc.IsDebuggingEnabled).Returns(false);

            using(new HttpContextScope(httpContext.Object))
            {
                var reader = new DebugStatusReader(machineConfigReader.Object);
                Assert.IsFalse(reader.IsDebuggingEnabled());
            }

            machineConfigReader.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DebuggingExplicitlyEnabledInConfiguration_CheckMachineConfig(bool configReaderValue)
        {
            var httpContext = new Mock<HttpContextBase>();
            var machineConfigReader = new Mock<IMachineConfigReader>(MockBehavior.Strict);
            var trustLevel = new Mock<ITrustLevel>();

            httpContext.SetupGet(hc => hc.IsDebuggingEnabled).Returns(true);
            machineConfigReader.SetupGet(mcr => mcr.IsNotRetailDeployment).Returns(configReaderValue);
            trustLevel.Setup(tl => tl.IsHighOrUnrestrictedTrust).Returns(true);

            using(new TrustLevelScope(trustLevel.Object))
            using(new HttpContextScope(httpContext.Object))
            {
                var reader = new DebugStatusReader(machineConfigReader.Object);
                Assert.AreEqual(configReaderValue, reader.IsDebuggingEnabled());
            }

            machineConfigReader.VerifyAll();
        }

        [Test]
        public void IgnoreMachineConfigReader_Untrusted()
        {
            var httpContext = new Mock<HttpContextBase>(MockBehavior.Strict);
            var trustLevel = new Mock<ITrustLevel>(MockBehavior.Strict);

            httpContext.SetupGet(hc => hc.IsDebuggingEnabled).Returns(true);
            trustLevel.SetupGet(tl => tl.IsHighOrUnrestrictedTrust).Returns(false);

            //shouldn't touch anything on this
            var machineConfigReader = new Mock<IMachineConfigReader>(MockBehavior.Strict);

            using(new TrustLevelScope(trustLevel.Object))
            using(new HttpContextScope(httpContext.Object))
            {
                var reader = new DebugStatusReader(machineConfigReader.Object);
                Assert.IsTrue(reader.IsDebuggingEnabled());
            }

            machineConfigReader.VerifyAll();
        }

        [Test]
        public void UseMachineConfigReader_Trusted()
        {
            var httpContext = new Mock<HttpContextBase>(MockBehavior.Strict);
            var trustLevel = new Mock<ITrustLevel>(MockBehavior.Strict);
            var machineConfigReader = new Mock<IMachineConfigReader>(MockBehavior.Strict);

            httpContext.SetupGet(hc => hc.IsDebuggingEnabled).Returns(true);
            trustLevel.SetupGet(tl => tl.IsHighOrUnrestrictedTrust).Returns(true);
            machineConfigReader.SetupGet(tl => tl.IsNotRetailDeployment).Returns(false);

            using(new TrustLevelScope(trustLevel.Object))
            using(new HttpContextScope(httpContext.Object))
            {
                var reader = new DebugStatusReader(machineConfigReader.Object);
                Assert.IsFalse(reader.IsDebuggingEnabled());
            }

            machineConfigReader.VerifyAll();
        }
    }
}
