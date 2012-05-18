using System.Web;
using Moq;
using NUnit.Framework;
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

            httpContext.SetupGet(hc => hc.IsDebuggingEnabled).Returns(true);
            machineConfigReader.SetupGet(mcr => mcr.IsRetailDeployment).Returns(configReaderValue);

            using(new HttpContextScope(httpContext.Object))
            {
                var reader = new DebugStatusReader(machineConfigReader.Object);
                Assert.AreEqual(configReaderValue, reader.IsDebuggingEnabled());
            }

            machineConfigReader.VerifyAll();
        }

        [TestCase(AspNetHostingPermissionLevel.Medium)]
        [TestCase(AspNetHostingPermissionLevel.Low)]
        [TestCase(AspNetHostingPermissionLevel.Minimal)]
        [TestCase(AspNetHostingPermissionLevel.None)]
        public void IgnoreMachineConfigReader_Untrusted(AspNetHostingPermissionLevel permissionLevel)
        {
            var httpContext = new Mock<HttpContextBase>(MockBehavior.Strict);
            var trustLevel = new Mock<ITrustLevel>(MockBehavior.Strict);

            httpContext.SetupGet(hc => hc.IsDebuggingEnabled).Returns(true);
            trustLevel.SetupGet(tl => tl.CurrentTrustLevel).Returns(permissionLevel);

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

        [TestCase(AspNetHostingPermissionLevel.Unrestricted)]
        [TestCase(AspNetHostingPermissionLevel.High)]
        public void UseMachineConfigReader_Trusted(AspNetHostingPermissionLevel permissionLevel)
        {
            var httpContext = new Mock<HttpContextBase>(MockBehavior.Strict);
            var trustLevel = new Mock<ITrustLevel>(MockBehavior.Strict);
            var machineConfigReader = new Mock<IMachineConfigReader>(MockBehavior.Strict);

            httpContext.SetupGet(hc => hc.IsDebuggingEnabled).Returns(true);
            trustLevel.SetupGet(tl => tl.CurrentTrustLevel).Returns(permissionLevel);
            machineConfigReader.SetupGet(tl => tl.IsRetailDeployment).Returns(false);

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
