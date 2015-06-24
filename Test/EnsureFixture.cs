using System;
using Verify = Xunit.Assert;
using Xunit;

namespace XTypes.Tests
{
    /// <summary>
    /// Summary description for EnsureFixture
    /// </summary>
    public class EnsureFixture : ReflectionFixture
    {
        [Fact]
        public void IsMirrorOfAssert()
        {
            TypesAreEqual(typeof(Ensure), typeof(Assert));
        }
    }
}
