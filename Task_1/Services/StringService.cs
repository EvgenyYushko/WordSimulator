using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_1.Services
{
    public static class StringService
    {
        private static Random random = new Random();

        /// <summary>
        /// Получить рандомную строку.
        /// </summary>
        /// <param name="countWord">колличесво слов в строке</param>
        /// <param name="isCaseSensitive">в том числе верхний регистр</param>
        /// <returns></returns>
        public static string GetRandomString(int countWord, bool isCaseSensitive)
        {
            string resultString = string.Empty;

            for (int item = 0; item < countWord; item++)
            {
                var chars = new char[random.Next(3, 15)];

                var possibleLetters = "abcdefghijklmnopqrstuvwxyz";

                for (int i = 0; i < chars.Length; i++)
                {
                    var defaultChar = possibleLetters[random.Next(0, possibleLetters.Length - 1)];
                    char finishChar = defaultChar;

                    if (isCaseSensitive)
                    {
                        bool isUpperCase = random.Next(0, 2) == 1;

                        finishChar = isUpperCase ? defaultChar.ToString().ToUpper()[0] : defaultChar;
                    }

                    chars[i] = finishChar;
                }

                string space = item != 0 ? " " : "";
                resultString += space + new string(chars);
            }

            return resultString;
        }
    }
}
