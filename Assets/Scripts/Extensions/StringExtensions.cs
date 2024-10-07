using System.Text.RegularExpressions;

public static class StringExtensions
{
    public static string PascalCaseToSentence(this string text)
    {
        return Regex.Replace(text, "(?<!^)([A-Z])", " $1");
    }
}