
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace XTypes
{
    internal static class Assert
    {
        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AreEqual<T>(T value, T expected)
        {
            NotNull(expected);
            NotNull(value);

            if (!value.Equals(expected))
                Fail($"Expected '{expected}', found '{value}'.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DirectoryExists(string path)
        {
            ValidString(path);

            if (!Directory.Exists(path))
                Fail($"'{path}' does not exist or is inaccessible.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnumDefined<T>(T value)
        {
            NotNull(value);

            if (!(typeof(T).IsEnum && Enum.IsDefined(typeof(T), value)))
                Fail($"'{value}' is not a member of '{typeof(T)}'.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fail(string message)
        {
            throw new AssertionException(message);
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FileExist(string path)
        {
            ValidString(path);

            if (!File.Exists(path))
                Fail($"'{path}' does not exist or is inaccessible.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Greater<T>(T value, T lesser)
            where T : IComparable<T>
        {
            NotNull(lesser);
            NotNull(value);

            if (value.CompareTo(lesser) <= 0)
                Fail($"Expected greater than '{lesser}', found '{value}'.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GreaterOrEqual<T>(T value, T lesser)
            where T : IComparable<T>
        {
            NotNull(value);

            if (value.CompareTo(lesser) < 0)
                Fail($"Expected greater than or equal to '{lesser}', found '{value}'.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsFalse(bool value)
        {
            if (value)
                Fail("Expected false, found true.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTrue(bool value)
        {
            if (!value)
                Fail("Expected true, found false.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsType<TType>(object value)
        {
            if (value.GetType() != typeof(TType))
                Fail($"Unexpected type encountered. Expected '{typeof(TType)}', found '{value.GetType()}'.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Less<T>(T value, T greater)
            where T : IComparable<T>
        {
            NotNull(value);

            if (value.CompareTo(greater) >= 0)
                Fail($"Invalid argument encountered. Expected less than '{greater}', found '{value}'.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LessOrEqual<T>(T value, T greater)
            where T : IComparable<T>
        {
            NotNull(greater);
            NotNull(value);

            if (value.CompareTo(greater) > 0)
                Fail($"Invalid argument encountered. Expected less then or equal to '{greater}', found '{value}'.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotEqual<T>(T value, T expected)
        {
            NotNull(expected);
            NotNull(value);

            if (value.Equals(expected))
                Fail($"Invalid argument encountered. Expected {value}, found '{expected}'.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull(object value)
        {
            if (value == null)
                Fail("Unexpected null value encountered.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidArrayIndex(int value, Array array)
        {
            NotNull(array);

            if (value < 0 || value >= array.Length)
                Fail($"Expected greater than or equal to 0 and less than {array.Length}, found {value}.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidArrayIndex(long value, Array array)
        {
            NotNull(array);

            if (value < 0 || value >= array.LongLength)
                Fail($"Expected greater than or equal to 0 and less than {array.LongLength}, found {value}.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidArrayIndex(int value, string array, string name)
        {
            NotNull(array);

            if (value < 0 || value >= array.Length)
                Fail($"Expected greater than or equal to 0 and less than {array.Length}, found {value}.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidString(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                Fail("Unexpected null or empty value encountered.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidUriString(string value)
        {
            ValidString(value);

            if (!Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute))
                Fail("Unexpected malformed URI value encountered.");
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WithinRange<T>(T value, T lowerLimit, T upperLimit)
            where T : IComparable, IComparable<T>
        {
            Greater(value, lowerLimit);
            Less(value, upperLimit);
        }
    }

    internal class AssertionException : Exception
    {
        public AssertionException(string message)
            : base(message)
        { }

        public AssertionException(string message, Exception innerException)
            : base(message, innerException)
        { }

        internal AssertionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        { }
    }
}
