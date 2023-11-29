using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;

namespace CPG.Tests.Base;

public static class TestExtensions
{
    public static IEnumerable<object[]> ToMemberData<T>(this IEnumerable<T> data) =>
        data.Select(item => new object[] { item });

    public static IEnumerable<object[]> GetEnumValues<T>(params T[] exceptValues) where T : Enum =>
        Enum.GetValues(typeof(T))
            .OfType<T>()
            .Where(v => !exceptValues.Contains(v))
            .Select(value => new object[] { value })
            .ToList();

    public static IEnumerable<object[]> GetEnumValues<T>() where T : Enum =>
        Enum.GetValues(typeof(T))
            .OfType<T>()
            .Select(value => new object[] { value })
            .ToList();

    public static List<T> ToSingleElementList<T>(this T entity) where T : class =>
        new List<T> { entity };

    public static void ShouldBeGuid(this string text)
    {
        Guid.TryParse(text, out _)
            .Should()
            .BeTrue();
    }

    public static string GenerateCustomizedPersianText(int length)
    {
        var persianCharacters = new List<string> { "ا", "ب", "پ", "ت", "ث", "ج", "چ", "ح",
                                                       "خ", "د", "ذ", "ر", "ز", "ژ", "س", "ش",
                                                       "ص", "ض", "ط", "ظ", "ع", "غ", "ف", "ق",
                                                       "ک", "گ", "ل", "م", "ن", "و", "ه", "ی"};
        var rnd = new Random();
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            int x = rnd.Next(persianCharacters.Count);
            result.Append(persianCharacters[x]);
        }

        return result.ToString();
    }
}