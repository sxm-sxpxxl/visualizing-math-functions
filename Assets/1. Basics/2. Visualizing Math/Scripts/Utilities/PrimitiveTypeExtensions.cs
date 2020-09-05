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

    // 1:00:00 [hours != 0] or 0:00 [hours == 0]
    public static string ToStringInRelevantFormat(this TimeSpan span, int totalHours)
    {
        var hoursFormat = totalHours == 0 ? string.Empty : @"h\:m";
        return span.ToString(hoursFormat + @"m\:ss");
    }
    
    public static string ToStringInRelevantFormat(this TimeSpan span) => span.ToStringInRelevantFormat(span.Hours);

    // 0:00:00 / 1:00:00 [hours != 0] or 0:00 / 1:00 [hours == 0]
    public static string BuildStringInRelevantFormatWithSpans(TimeSpan firstSpan, TimeSpan secondSpan)
    {
        return firstSpan.ToStringInRelevantFormat(firstSpan.Hours) + " / " + secondSpan.ToStringInRelevantFormat();
    }
}
