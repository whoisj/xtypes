using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verify = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace XTypes.Tests
{
    /// <summary>
    /// Summary description for EnsureFixture
    /// </summary>
    [TestClass]
    public class EnsureFixture : ReflectionFixture
    {
        [TestMethod]
        public void IsMirrorOfAssert()
        {
            TypeCompareCallback callback = (Type testedType, Type controlType, MethodInfo missingMethod) =>
            {
                Verify.Fail($"'{missingMethod.Name}' exists in '{controlType.Name}' but not in '{testedType.Name}'");
            };

            Verify.IsTrue(TypesAreEqual(typeof(Ensure), typeof(Assert), callback), $"{nameof(Ensure)} is not a proper missor of {nameof(Assert)}");
        }
    }
}
