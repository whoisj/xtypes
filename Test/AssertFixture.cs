using System;
using System.Reflection;
using Xunit;
using Verify = Xunit.Assert;

namespace XTypes.Tests
{
    /// <summary>
    /// Summary description for AssertFixture
    /// </summary>
    public class AssertFixture : ReflectionFixture
    {
        [Fact]
        public void IsMirrorOfEnsure()
        {
            TypesAreEqual(typeof(Ensure), typeof(Assert));
        }

        [Theory]
        [InlineData(true, true, false)]
        [InlineData(0, 0, 1)]
        [InlineData(0U, 0U, 1U)]
        [InlineData(0L, 0L, 1L)]
        [InlineData(0UL, 0UL, 1UL)]
        [InlineData((byte)0, (byte)0, (byte)1)]
        [InlineData(0.0, 0.0, 1.0)]
        [InlineData(0.0f, 0.0f, 1.0f)]
        [InlineData("0", "0", "1")]
        [InlineData(0, 0, null)]
        public void AreEquals<T>(T control, T @true, T @false)
        {
            Assert.AreEqual(control, @true);
            Verify.Throws<Assert.Assertion>(() => { Assert.AreEqual(control, @false); });
        }
    }
}
