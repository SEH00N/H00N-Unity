namespace H00N.Extensions
{
    public static class NumberExtensions
    {
        public static string ToNumberString(this float value) => value.ToString("N0");
        public static string ToNumberString(this double value) => value.ToString("N0");
        public static string ToNumberString(this int value) => value.ToString("N0");
        public static string ToNumberString(this long value) => value.ToString("N0");
    }
}