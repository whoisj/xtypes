using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace XTypes
{
    internal static class Ensure
    {
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AreEqual<T>(T value, T expected)
        {
            Assert.NotNull(expected);
            NotNull(value);

            if (!value.Equals(expected))
                throw new EnsureException($"Invalid argument encountered. Expected '{expected}', found '{value}'.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AreEqual<T>(T value, T expected, string name)
        {
            Assert.NotNull(name);
            Assert.NotNull(expected);
            NotNull(value, name);

            if (!value.Equals(expected))
                throw new ArgumentException($"Invalid argument encountered. Expected '{expected}', found '{value}'.", name);
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DirectoryExists(string path)
        {
            ValidString(path);

            if (!Directory.Exists(path))
                throw new EnsureException("Invalid argument encountered.", new DirectoryNotFoundException($"'{path}' does not exist or is inaccessible."));
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DirectoryExists(string path, string name)
        {
            Assert.NotNull(name);
            ValidString(path, name);

            if (!Directory.Exists(path))
                throw new ArgumentException("Invalid argument encountered.", name, new DirectoryNotFoundException($"'{path}' does not exist or is inaccessible."));
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnumDefined<T>(T value)
        {
            NotNull(value);

            if (!(typeof(T).IsEnum && Enum.IsDefined(typeof(T), value)))
                throw new EnsureException("Invalid argument encountered.", new InvalidEnumArgumentException($"'{value}' is not a member of '{typeof(T)}'."));
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnumDefined<T>(T value, string name)
        {
            Assert.NotNull(name);
            NotNull(value, name);

            if (!(typeof(T).IsEnum && Enum.IsDefined(typeof(T), value)))
                throw new ArgumentException("Invalid argument encountered.", name, new InvalidEnumArgumentException($"'{value}' is not a member of '{typeof(T)}'."));
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FileExist(string path)
        {
            ValidString(path);

            if (!File.Exists(path))
                throw new EnsureException("Invalid argument encountered.", new FileNotFoundException($"'{path}' does not exist or is inaccessible."));
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FileExist(string path, string name)
        {
            Assert.NotNull(name);
            ValidString(path, name);

            if (!File.Exists(path))
                throw new ArgumentException("Invalid argument encountered.", name, new FileNotFoundException($"'{path}' does not exist or is inaccessible."));
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Greater<T>(T value, T lesser)
            where T : IComparable<T>
        {
            Assert.NotNull(lesser);
            NotNull(value);

            if (value.CompareTo(lesser) <= 0)
                throw new EnsureException($"Invalid argument encountered. Expected greater than '{lesser}', found '{value}'.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Greater<T>(T value, T lesser, string name)
            where T : IComparable<T>
        {
            Assert.NotNull(name);
            Assert.NotNull(lesser);
            NotNull(value, name);

            if (value.CompareTo(lesser) <= 0)
                throw new ArgumentOutOfRangeException(name, $"Invalid argument encountered. Expected greater than '{lesser}', found '{value}'.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GreaterOrEqual<T>(T value, T lesser)
            where T : IComparable<T>
        {
            Assert.NotNull(lesser);
            NotNull(value);

            if (value.CompareTo(lesser) < 0)
                throw new EnsureException($"Invalid argument encountered. Expected greater than or equal to '{lesser}', found '{value}'.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GreaterOrEqual<T>(T value, T lesser, string name)
            where T : IComparable<T>
        {
            Assert.NotNull(name);
            Assert.NotNull(lesser);
            NotNull(value, name);

            if (value.CompareTo(lesser) < 0)
                throw new ArgumentOutOfRangeException(name, $"Invalid argument encountered. Expected greater than or equal to '{lesser}', found '{value}'.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsFalse(bool value)
        {
            if (value)
                throw new EnsureException("Invalid argument encountered. Expected false, found true.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsFalse(bool value, string name)
        {
            if (value)
                throw new ArgumentException("Invalid argument encountered. Expected false, found true.", name);
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTrue(bool value)
        {
            if (!value)
                throw new EnsureException("Invalid argument encountered. Expected true, found false.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTrue(bool value, string name)
        {
            if (!value)
                throw new ArgumentException("Invalid argument encountered. Expected true, found false.", name);
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsType<TType>(object value)
        {
            if (value.GetType() != typeof(TType))
                throw new EnsureException($"Unexpected type encountered. Expected '{typeof(TType)}', found '{value.GetType()}'.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsType<TType>(object value, string name)
        {
            Assert.NotNull(name);

            if (value.GetType() != typeof(TType))
                throw new ArgumentException($"Unexpected type encountered. Expected '{typeof(TType)}', found '{value.GetType()}'.", name);
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Less<T>(T value, T greater)
            where T : IComparable<T>
        {
            Assert.NotNull(greater);
            NotNull(value);

            if (value.CompareTo(greater) >= 0)
                throw new EnsureException($"Invalid argument encountered. Expected less than '{greater}', found '{value}'.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Less<T>(T value, T greater, string name)
            where T : IComparable<T>
        {
            Assert.NotNull(name);
            Assert.NotNull(greater);
            NotNull(value, name);

            if (value.CompareTo(greater) >= 0)
                throw new ArgumentOutOfRangeException(name, $"Invalid argument encountered. Expected less than '{greater}', found '{value}'.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LessOrEqual<T>(T value, T greater)
            where T : IComparable<T>
        {
            Assert.NotNull(greater);
            NotNull(value);

            if (value.CompareTo(greater) > 0)
                throw new EnsureException($"Invalid argument encountered. Expected less then or equal to '{greater}', found '{value}'.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LessOrEqual<T>(T value, T greater, string name)
            where T : IComparable<T>
        {
            Assert.NotNull(name);
            Assert.NotNull(greater);
            NotNull(value, name);

            if (value.CompareTo(greater) > 0)
                throw new ArgumentOutOfRangeException(name, $"Invalid argument encountered. Expected less then or equal to '{greater}', found '{value}'.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotEqual<T>(T value, T expected)
        {
            Assert.NotNull(expected);
            NotNull(value);

            if (value.Equals(expected))
                throw new EnsureException($"Invalid argument encountered. Expected {value}, found '{expected}'.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotEqual<T>(T value, T expected, string name)
        {
            Assert.NotNull(name);
            Assert.NotNull(expected);
            NotNull(value, name);

            if (value.Equals(expected))
                throw new ArgumentException($"Invalid argument encountered. Expected {value}, found '{expected}'.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull(object value)
        {
            if (value == null)
                throw new EnsureException("Unexpected null value encountered.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull(object value, string name)
        {
            Assert.NotNull(name);

            if (value == null)
                throw new ArgumentNullException(name, "Unexpected null value encountered.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidArrayIndex(int value, Array array)
        {
            NotNull(array);

            if (value < 0 || value >= array.Length)
                throw new EnsureException($"Expected greater than or equal to 0 and less than {array.Length}, found {value}.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidArrayIndex(long value, Array array, string name)
        {
            Assert.NotNull(name);
            NotNull(array, name);

            if (value < 0 || value >= array.LongLength)
                throw new ArgumentOutOfRangeException(name, $"Expected greater than or equal to 0 and less than {array.LongLength}, found {value}.");
        }


        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidArrayIndex(int value, string array)
        {
            NotNull(array);

            if (value < 0 || value >= array.Length)
                throw new EnsureException($"Expected greater than or equal to 0 and less than {array.Length}, found {value}.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidArrayIndex(int value, string array, string name)
        {
            Assert.NotNull(name);
            NotNull(array, name);

            if (value < 0 || value >= array.Length)
                throw new ArgumentOutOfRangeException(name, $"Expected greater than or equal to 0 and less than {array.Length}, found {value}.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidString(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new EnsureException("Unexpected null or empty value encountered.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidString(string value, string name)
        {
            Assert.NotNull(name);

            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(name, "Unexpected null or empty value encountered.");
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidUriString(string value)
        {
            ValidString(value);

            if (!Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute))
                throw new EnsureException("Unexpected malformed URI value encountered.", new UriFormatException($"'{value}' is invalid."));
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidUriString(string value, string name)
        {
            Assert.NotNull(name);

            ValidString(value, name);

            if (!Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute))
                throw new ArgumentException("Unexpected malformed URI value encountered.", name, new UriFormatException($"'{value}' is invalid."));
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WithinRange<T>(T value, T lowerLimit, T upperLimit)
            where T : IComparable, IComparable<T>
        {
            Greater(value, lowerLimit);
            Less(value, upperLimit);
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WithinRange<T>(T value, T lowerLimit, T upperLimit, string name)
            where T : IComparable, IComparable<T>
        {
            Assert.NotNull(name);

            Greater(value, lowerLimit, name);
            Less(value, upperLimit, name);
        }
    }

    internal sealed class EnsureException : Exception
    {
        public EnsureException(string message)
            : base(message)
        { }

        public EnsureException(string message, Exception innerException)
            : base(message, innerException)
        { }

        internal EnsureException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        { }
    }
}
