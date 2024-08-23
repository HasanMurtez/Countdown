using System;
using System.Collections.Generic;
using System.Linq;

namespace Countdown;

public class CountdownGame
{
    private List<char> vowels = new List<char>();
    private List<char> consonants = new List<char>();
    private List<char> selectedLetters = new List<char>();

    public CountdownGame()
    {
        InitializeLetters();
    }

    private void InitializeLetters()
    {
        // Initialize vowels
        vowels.AddRange(Enumerable.Repeat('A', 15));
        vowels.AddRange(Enumerable.Repeat('E', 21));
        vowels.AddRange(Enumerable.Repeat('I', 13));
        vowels.AddRange(Enumerable.Repeat('O', 13));
        vowels.AddRange(Enumerable.Repeat('U', 5));

        // Initialize consonants
        consonants.AddRange(Enumerable.Repeat('B', 2));
        consonants.AddRange(Enumerable.Repeat('C', 3));
        consonants.AddRange(Enumerable.Repeat('D', 6));
        consonants.AddRange(Enumerable.Repeat('F', 2));
        consonants.AddRange(Enumerable.Repeat('G', 3));
        consonants.AddRange(Enumerable.Repeat('H', 2));
        consonants.AddRange(Enumerable.Repeat('J', 1));
        consonants.AddRange(Enumerable.Repeat('K', 1));
        consonants.AddRange(Enumerable.Repeat('L', 5));
        consonants.AddRange(Enumerable.Repeat('M', 4));
        consonants.AddRange(Enumerable.Repeat('N', 8));
        consonants.AddRange(Enumerable.Repeat('P', 4));
        consonants.AddRange(Enumerable.Repeat('Q', 1));
        consonants.AddRange(Enumerable.Repeat('R', 9));
        consonants.AddRange(Enumerable.Repeat('S', 9));
        consonants.AddRange(Enumerable.Repeat('T', 9));
        consonants.AddRange(Enumerable.Repeat('V', 1));
        consonants.AddRange(Enumerable.Repeat('W', 1));
        consonants.AddRange(Enumerable.Repeat('X', 1));
        consonants.AddRange(Enumerable.Repeat('Y', 1));
        consonants.AddRange(Enumerable.Repeat('Z', 1));

        // Shuffle both lists
        ShuffleList(vowels);
        ShuffleList(consonants);
    }

    private void ShuffleList(List<char> list)
    {
        Random rng = new Random();
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            char value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public char PickLetter(bool isVowel)
    {
        if (isVowel)
        {
            if (vowels.Count > 0)
            {
                char picked = vowels[0];
                vowels.RemoveAt(0);
                selectedLetters.Add(picked);
                return picked;
            }
        }
        else
        {
            if (consonants.Count > 0)
            {
                char picked = consonants[0];
                consonants.RemoveAt(0);
                selectedLetters.Add(picked);
                return picked;
            }
        }
        throw new InvalidOperationException("No more letters available.");
    }

    public List<char> GetSelectedLetters()
    {
        return new List<char>(selectedLetters);
    }
}
