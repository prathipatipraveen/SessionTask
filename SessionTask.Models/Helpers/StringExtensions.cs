namespace SessionTask.Models.Helpers
{
    public static class StringExtensions
    {
        public static int ToInt(this string value)
        {
            return int.TryParse(value, out int convertedValue) ? convertedValue : 0;
        }
    }
}
