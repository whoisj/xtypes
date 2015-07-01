using System.IO;
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
        public void AreEquals<T>(T control, T @true, T @false)
        {
            Assert.AreEqual(control, @true);
            Verify.Throws<AssertionException>(() => { Assert.AreEqual(control, @false); });
        }

        [Fact]
        public void DirectoryExists()
        {
            DirectoryInfo positiveTest = new DirectoryInfo("assert-foo");
            DirectoryInfo negativeTest = new DirectoryInfo("assert-bar");

            if (!positiveTest.Exists)
            {
                positiveTest.Create();
            }
            if (negativeTest.Exists)
            {
                negativeTest.Delete(true);
            }

            Assert.DirectoryExists(positiveTest.FullName);
            Verify.Throws<AssertionException>(() => { Assert.DirectoryExists(negativeTest.FullName); });

            positiveTest.Delete();
        }

        [Fact]
        public void EnumDefined()
        {
            Assert.EnumDefined(TestEnum.Value1);
            Assert.EnumDefined(TestEnum.Value2);
            Assert.EnumDefined((TestEnum)0);
            Assert.EnumDefined((TestEnum)1);

            Verify.Throws<AssertionException>(() => { Assert.EnumDefined((TestEnum)2); });
        }
    }
}
