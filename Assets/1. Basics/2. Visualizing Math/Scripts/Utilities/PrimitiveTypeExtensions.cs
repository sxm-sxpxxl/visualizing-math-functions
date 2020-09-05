using System;
using System.Linq;
using System.Text.RegularExpressions;

public static class PrimitiveTypeExtensions
{
    public static string SplitByUppercaseLetters(this string str)
    {
        var regex = new Regex(@"([A-Z]|\d)([a-z]|[A-Z])[a-z]*(?=[A-Z]*)");
        var matches = regex.Matches(str);
		
        return string.Join(" ", matches.Cast<Match>().Select(m => m.Value));
    }

    public static bool ToBoolean(this char ch)
    {
        if (char.IsNumber(ch) == false)
            throw new ArgumentException($"{ch} char is not a number and cannot be converted to boolean.");
        
        return Convert.ToBoolean(Convert.ToInt32(Convert.ToString(ch)));
    }
}
