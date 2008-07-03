using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceTests
{
    /// <summary>
    /// Class to generate random test data
    /// </summary>
    public class RandomGenerator
    {
        private static string WordList = @"Lorem ipsum dolor sit amet consectetur adipisicing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua Ut enim ad minim veniam quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur Excepteur sint occaecat cupidatat non proident sunt in culpa qui officia deserunt mollit anim id est laborum";
        private static string[] words;

        private int _wordIndex = 0;
        private Random _random;

        static RandomGenerator()
        {
            words = WordList.Split(' ');            
        }

        public RandomGenerator()
        {
            _wordIndex = 0;
            _random = new Random(99);
        }

        public int RandomInt(int min, int max)
        {
            return _random.Next(min, max);
        }

        public int RandomInt()
        {
            return _random.Next();
        }

        public string RandomWord()
        {
            return NextWord();
        }

        public string RandomWord(int count)
        {
            StringBuilder sb = new StringBuilder();
            while (--count != 0)
            {
                sb.Append(NextWord());
                sb.Append(' ');
            }
            sb.Append(NextWord());
            return sb.ToString();
        }
        private string NextWord()
        {
            if (_wordIndex >= words.Length)
                _wordIndex = 0;
            return words[_wordIndex++];
        }
    }
}
