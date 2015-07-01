using System;
using System.IO;
using Xunit;
using Verify = Xunit.Assert;

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
        public void AreEquals<T>(T control, T @true, T @false)
        {
            Ensure.AreEqual(control, @true, "test");
            Verify.Throws<ArgumentException>(() => { Ensure.AreEqual(control, @false, "test"); });
        }

        [Fact]
        public void DirectoryExists()
        {
            DirectoryInfo positiveTest = new DirectoryInfo("ensure-foo");
            DirectoryInfo negativeTest = new DirectoryInfo("ensure-bar");

            if (!positiveTest.Exists)
            {
                positiveTest.Create();
            }
            if (negativeTest.Exists)
            {
                negativeTest.Delete(true);
            }

            Ensure.DirectoryExists(positiveTest.FullName, "test");
            Verify.Throws<ArgumentException>(() => { Ensure.DirectoryExists(negativeTest.FullName, "test"); });

            positiveTest.Delete();
        }

        [Fact]
        public void EnumDefined()
        {
            Ensure.EnumDefined(TestEnum.Value1, "test");
            Ensure.EnumDefined(TestEnum.Value2, "test");
            Ensure.EnumDefined((TestEnum)0, "test");
            Ensure.EnumDefined((TestEnum)1, "test");

            Verify.Throws<ArgumentException>(() => { Ensure.EnumDefined((TestEnum)2, "test"); });
        }
    }
}
