using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace XTypes
{
    internal static class Ensure
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AreEqual<T>(T value, T expected, string name)
        {
            Assert.NotNull(name);
            Assert.NotNull(expected);
            NotNull(value, name);

            if (!value.Equals(expected))
                throw new ArgumentException($"Invalid argument encountered. Expected '{expected}', found '{value}'", name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DirectoryExists(string path, string name)
        {
            Assert.NotNull(name);
            ValidString(path, name);

            if (!Directory.Exists(path))
                throw new ArgumentException("Invalid argument encountered.", name, new DirectoryNotFoundException($"'{path}' does not exist or is inaccessible."));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnumDefined<T>(int value, string name)
        {
            Assert.NotNull(name);
            NotNull(value, name);

            if (!(typeof(T).IsEnum && Enum.IsDefined(typeof(T), value)))
                throw new InvalidEnumArgumentException(name, value, typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FileExist(string path, string name)
        {
            Assert.NotNull(name);
            ValidString(path, name);

            if (!File.Exists(path))
                throw new ArgumentException("Invalid argument encountered.", name, new FileNotFoundException($"'{path}' does not exist or is inaccessible."));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Greater<T>(T value, T lesser, string name)
            where T : IComparable<T>
        {
            Assert.NotNull(name);
            Assert.NotNull(lesser);
            NotNull(value, name);

            if (value.CompareTo(lesser) <= 0)
                throw new ArgumentOutOfRangeException(name, $"Invalid argument encountered. Expected greater than {lesser}, found {value}.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GreaterOrEqual<T>(T value, T lesser, string name)
            where T : IComparable<T>
        {
            Assert.NotNull(name);
            Assert.NotNull(lesser);
            NotNull(value, name);

            if (value.CompareTo(lesser) < 0)
                throw new ArgumentOutOfRangeException(name, $"Invalid argument encountered. Expected greater than or equal to {lesser}, found {value}.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsFalse(bool value, string name)
        {
            if (value)
                throw new ArgumentException("Invalid argument encountered. Expected false, found true.", name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTrue(bool value, string name)
        {
            if (!value)
                throw new ArgumentException("Invalid argument encountered. Expected true, found false.", name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsType<TType>(object value, string name)
        {
            Assert.NotNull(name);

            if (value.GetType() != typeof(TType))
                throw new ArgumentException($"Unexpected type encountered. Expected '{typeof(TType)}', found '{value.GetType()}'.", name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Less<T>(T value, T greater, string name)
            where T : IComparable<T>
        {
            Assert.NotNull(name);
            Assert.NotNull(greater);
            NotNull(value, name);

            if (value.CompareTo(greater) >= 0)
                throw new ArgumentOutOfRangeException(name, $"Invalid argument encountered. Expected less than {greater}, found {value}.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LessOrEqual<T>(T value, T greater, string name)
            where T : IComparable<T>
        {
            Assert.NotNull(name);
            Assert.NotNull(greater);
            NotNull(value, name);

            if (value.CompareTo(greater) > 0)
                throw new ArgumentOutOfRangeException(name, $"Invalid argument encountered. Expected less then or equal to {greater}, found {value}.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotEqual<T>(T value, T expected, string name)
        {
            Assert.NotNull(name);
            Assert.NotNull(expected);
            NotNull(value, name);

            if (value.Equals(expected))
                throw new ArgumentException($"Invalid argument encountered. Expected {value}, found '{expected}'.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull(object value, string name)
        {
            Assert.NotNull(name);

            if (value == null)
                throw new ArgumentNullException(name, "Unexpected null value encountered.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidArrayIndex(int value, Array array, string name)
        {
            Assert.NotNull(name);
            NotNull(array, name);

            if (value < 0 || value >= array.Length)
                throw new ArgumentOutOfRangeException(name, $"Expected greater than or equal to 0 and less than {array.Length}, found {value}.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidArrayIndex(long value, Array array, string name)
        {
            Assert.NotNull(name);
            NotNull(array, name);

            if (value < 0 || value >= array.LongLength)
                throw new ArgumentOutOfRangeException(name, $"Expected greater than or equal to 0 and less than {array.LongLength}, found {value}.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidArrayIndex(int value, string array, string name)
        {
            Assert.NotNull(name);
            NotNull(array, name);

            if (value < 0 || value >= array.Length)
                throw new ArgumentOutOfRangeException(name, $"Expected greater than or equal to 0 and less than {array.Length}, found {value}.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidString(string value, string name)
        {
            Assert.NotNull(name);

            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(name, "Unexpected null or empty value encountered.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidUriString(string value, string name)
        {
            Assert.NotNull(name);

            ValidString(value, name);

            if (!Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute))
                throw new ArgumentException("Unexpected malformed URI value encountered.", name, new UriFormatException($"'{value}' is invalid."));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WithinRange<T>(T value, T lowerLimit, T upperLimit, string name)
            where T : IComparable, IComparable<T>
        {
            Assert.NotNull(name);

            Greater(value, lowerLimit, name);
            Less(value, upperLimit, name);
        }
    }
}
