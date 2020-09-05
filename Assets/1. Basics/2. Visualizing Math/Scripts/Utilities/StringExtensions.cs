using System.Linq;
using System.Text.RegularExpressions;

public static class StringExtensions
{
    public static string SplitByUppercaseLetters(this string str)
    {
        var regex = new Regex(@"([A-Z]|\d)([a-z]|[A-Z])[a-z]*(?=[A-Z]*)");
        var matches = regex.Matches(str);
		
        return string.Join(" ", matches.Cast<Match>().Select(m => m.Value));
    }
}
