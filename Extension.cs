namespace NACHAParser
{
    public static class Extension
    {
        public static string SubExt(this string str, int startIndex, int length)
        {
            return str.Substring(startIndex - 1, length);
        }
    }
}