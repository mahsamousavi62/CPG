using System;

namespace CPG.Domain.SharedKernel;

public class Constants
{
    public const string Pattern = "(usr|pwd|merchantConfigurationId|key|iv)\\\"\\s*(:)\\s*\"([^\"]*)\"";
    public const string Replaceformat = "$1$2*****";
}
public static class EnumHelper
{
    public static T ToEnum<T>(this string s) where T : struct
    {
        T newValue;
        return Enum.TryParse(s, out newValue) ? newValue : default(T);
    }
}
