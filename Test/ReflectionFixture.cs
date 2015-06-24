using System;
using System.Linq;
using System.Reflection;
using Verify = Xunit.Assert;

namespace XTypes.Tests
{
    public class ReflectionFixture:BaseFixture
    {
        protected MethodInfo[] GetMethodsFromType(Type type)
        {
            return type.GetMethods();
        }

        protected bool MethodsAreEqual(MethodInfo method1, MethodInfo method2)
        {
            if (ReferenceEquals(method1, method2))
                return true;
            if (ReferenceEquals(method1, null) || ReferenceEquals(method2, null))
                return false;

            if (method1.Name != method2.Name)
                return false;

            var params1 = method1.GetParameters();
            var params2 = method2.GetParameters();

            return ParametersAreEqual(params1, params2, Math.Min(params1.Length, params2.Length));
        }

        protected bool ParametersAreEqual(ParameterInfo[] parameters1, ParameterInfo[] parameters2, int length)
        {
            if (parameters1.Length < length || parameters2.Length < length)
                return false;

            for (int i = 0; i < length; i++)
            {
                if (!ParametersAreEqual(parameters1[i], parameters2[i]))
                    return false;
            }

            return true;
        }

        protected bool ParametersAreEqual(ParameterInfo parameter1, ParameterInfo parameter2)
        {
            return parameter1.Name == parameter2.Name
                && parameter1.Position == parameter2.Position
                && parameter1.GetType() == parameter2.GetType()
                && parameter1.IsOut == parameter2.IsOut
                && parameter1.IsOptional == parameter2.IsOptional;
        }

        protected void TypesAreEqual(Type testedType, Type controlType)
        {
            var testedMethods = GetMethodsFromType(testedType);
            var controlMethods = GetMethodsFromType(controlType);

            foreach (var testedMethod in testedMethods)
            {
                Verify.True(controlMethods.Any((MethodInfo controlMethod) => { return MethodsAreEqual(controlMethod, testedMethod); }),
                            $"'{testedMethod.Name}' exists in '{controlType.Name}' but not in '{testedType.Name}'");
            }
        }

    }
}
