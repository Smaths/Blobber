using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Utility
{
    public static class BadWordFilter
    {
        private static List<string> _badWords;

        static BadWordFilter()
        {
            // Load bad words from a file or define them directly in code
            _badWords = LoadBadWordsFromFile();
        }

        public static void Load()
        {
            _badWords = LoadBadWordsFromFile();
        }

        public static bool ContainsBadWord(string playerName)
        {
            // Convert the player name to lowercase for case-insensitive comparison
            string lowerPlayerName = playerName.ToLower();

            // Check if the player name contains any bad words
            return _badWords.Any(badWord => lowerPlayerName.Contains(badWord));
        }

        // This doesn't work...
        /*public static string Filter(string input)
        {
            const string CensoredText = "Blob";
            const string PatternTemplate = @"\b({0})(s?)\b";
            const RegexOptions Options = RegexOptions.IgnoreCase;

            IEnumerable<Regex> badWordMatchers = _badWords.Select(x => new Regex(string.Format(PatternTemplate, x), Options));

            string output = badWordMatchers.Aggregate(input, (current, matcher) => matcher.Replace(current, CensoredText));

            Debug.Log($"Filtered: {output}");
            Console.WriteLine(output);

            return output;
        }*/

        private static List<string> LoadBadWordsFromFile()
        {
            var words = new List<string>();

            // Specify the path to your CSV file
            string filePath = Application.dataPath + "/Data/badWords-en.txt";
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        // Split the line into individual words using the CSV delimiter (e.g., comma)
                        string[] wordArray = line.Split(',');

                        foreach (string word in wordArray)
                        {
                            // Trim any whitespace and add the word to the list
                            words.Add(word.Trim());
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to read the CSV file: {e.Message}");
            }

            return words;
        }
    }
}