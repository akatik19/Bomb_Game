using UnityEngine;

public static class CharExtension
{
    public const string Vowels = "AEIOU";

    public static bool isVowel(this char c)
    {
        return Vowels.IndexOf(char.ToUpperInvariant(c)) >= 0;
    }
}