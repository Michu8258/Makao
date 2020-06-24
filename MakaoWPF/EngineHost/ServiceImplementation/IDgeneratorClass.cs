using System;
using System.Collections.Generic;

namespace EngineHost.ServiceImplementation
{
    //class tha need to be raeted an instance of,
    //to get random string
    class IDgeneratorClass
    {
        public string GenerateID(int length)
        {
            string returnStriing = "";
            const string AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz#@$^*()";
            Random rng = new Random();

            foreach (string randomString in rng.NextStrings(AllowedChars, (length, length), 1))
            {
                returnStriing = randomString;
            }

            return returnStriing;
        }
    }

    //random extensions class
    public static class RandomExtensions
    {
        public static IEnumerable<string> NextStrings(
            this Random rnd,
            string allowedChars,
            (int Min, int Max) length,
            int count)
        {
            ISet<string> usedRandomStrings = new HashSet<string>();
            (int min, int max) = length;
            char[] chars = new char[max];
            int setLength = allowedChars.Length;

            while (count-- > 0)
            {
                int stringLength = rnd.Next(min, max + 1);

                for (int i = 0; i < stringLength; ++i)
                {
                    chars[i] = allowedChars[rnd.Next(setLength)];
                }

                string randomString = new string(chars, 0, stringLength);

                if (usedRandomStrings.Add(randomString))
                {
                    yield return randomString;
                }
                else
                {
                    count++;
                }
            }
        }
    }
}
