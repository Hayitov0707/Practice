using System.Linq;

namespace task01;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        string cleaned = new string(
            input
                .ToLower()
                .Where(c => !char.IsWhiteSpace(c) && !char.IsPunctuation(c))
                .ToArray());
        if (cleaned.Length == 0)
            return false;

        string reversed = new string(cleaned.Reverse().ToArray());

        return cleaned == reversed;
    }
}
