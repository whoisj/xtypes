
using System;
using System.Diagnostics;
using System.IO;

namespace XTypes
{
    internal static class Assert
    {
        [Conditional("DEBUG")]
        public static void AreEqual<T>(T value, T expected)
        {
            InternalAssert(value.Equals(expected));
        }

        [Conditional("DEBUG")]
        public static void DirectoryExists(string path)
        {
            ValidString(path);
            InternalAssert(Directory.Exists(path));
        }

        [Conditional("DEBUG")]
        public static void EnumDefined<T>(T value)
        {
            NotNull(value);
            InternalAssert(typeof(T).IsEnum && Enum.IsDefined(typeof(T), value));
        }

        [Conditional("DEBUG")]
        public static void FileExist(string path)
        {
            ValidString(path);
            InternalAssert(File.Exists(path));
        }

        [Conditional("DEBUG")]
        public static void Greater<T>(T value, T lesser)
            where T : IComparable, IComparable<T>
        {
            NotNull(value);
            NotNull(lesser);
            InternalAssert(value.CompareTo(lesser) > 0);
        }

        [Conditional("DEBUG")]
        public static void GreaterOrEqual<T>(T value, T lesser)
            where T : IComparable, IComparable<T>
        {
            NotNull(value);
            NotNull(lesser);
            InternalAssert(value.CompareTo(lesser) >= 0);
        }

        [Conditional("DEBUG")]
        public static void IsFalse(bool value)
        {
            InternalAssert(!value);
        }

        [Conditional("DEBUG")]
        public static void IsTrue(bool value)
        {
            InternalAssert(value);
        }

        [Conditional("DEBUG")]
        public static void IsType<TType>(object value)
        {
            NotNull(value);

            InternalAssert(value.GetType() == typeof(TType));
        }

        [Conditional("DEBUG")]
        public static void Less<T>(T value, T greater)
            where T : IComparable, IComparable<T>
        {
            NotNull(value);
            NotNull(greater);
            InternalAssert(value.CompareTo(greater) < 0);
        }

        [Conditional("DEBUG")]
        public static void LessOrEqual<T>(T value, T greater)
            where T : IComparable, IComparable<T>
        {
            NotNull(value);
            NotNull(greater);
            InternalAssert(value.CompareTo(greater) <= 0);
        }

        [Conditional("DEBUG")]
        public static void NotEqual<T>(T value, T expected)
        {
            InternalAssert(!value.Equals(expected));
        }

        [Conditional("DEBUG")]
        public static void NotNull(object value)
        {
            InternalAssert(value != null);
        }

        [Conditional("DEBUG")]
        public static void ValidArrayIndex(int value, Array array)
        {
            NotNull(array);
            InternalAssert(value >= 0 && value < array.Length);
        }

        [Conditional("DEBUG")]
        public static void ValidArrayIndex(long value, Array array)
        {
            NotNull(array);
            InternalAssert(value >= 0 && value < array.LongLength);
        }

        [Conditional("DEBUG")]
        public static void ValidArrayIndex(int value, string array)
        {
            NotNull(array);
            InternalAssert(value >= 0 && value < array.Length);
        }

        [Conditional("DEBUG")]
        public static void ValidString(string value)
        {
            InternalAssert(!String.IsNullOrWhiteSpace(value));
        }

        [Conditional("DEBUG")]
        public static void ValidUriString(string value)
        {
            ValidString(value);

            InternalAssert(Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute));
        }

        [Conditional("DEBUG")]
        public static void WithinRange<T>(T value, T lowerLimit, T upperLimit)
            where T : IComparable, IComparable<T>
        {
            Greater(value, lowerLimit);
            Less(value, upperLimit);
        }

        [Conditional("DEBUG")]
        private static void InternalAssert(bool condition)
        {
            if (!condition)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                else
                {
                    throw new Assertion("Internal assertion failed.");
                }
            }
        }

        public class Assertion : Exception
        {
            public Assertion(string message) : base(message) { }
        }
    }
}
