using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public static class DropdownUtility
{
    public static List<Dropdown.OptionData> GetOptionsForEnum<T>()
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(en => new Dropdown.OptionData(en.ToString().SplitByUppercaseLetters()))
            .ToList();
    }
}