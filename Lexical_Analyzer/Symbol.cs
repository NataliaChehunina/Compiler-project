using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexical_Analyzer
{
    static class Symbol
    {
        internal  enum Category { Whitespace, Digital, Letter, Delimiter, Error };
        static List<int> Del = new List<int>() {40,46,58,59,91,93};
        static List<int> DigitDelimiters = new List<int>() {46,59,93};
        static List<int> LetterDelimiters = new List<int>() {46,58,59,91,93};

        internal static Category GetASCIICategory(int c)
        {
            if (c <= 32)
                return Category.Whitespace ;
            if ((c >= 48) && (c <= 57)) 
                return Category.Digital;
            if (((c>=65)&&(c<=90))||((c>=97)&&(c<=122)))
                return Category.Letter;
            if (Del.BinarySearch(c) >= 0)
                return Category.Delimiter;
            return Category.Error;
        }

        internal static bool IsDecimal(int c)
        {
            return ((c>=48)&&(c<= 57));
        }

        internal static bool IsLetter(int c)
        {
            return ((c>=65)&&(c<=90))||((c>=97)&&(c<=122));
        }

        internal static bool IsWhiteSpace(int c)
        {
            return (c <= 32);
        }

        internal static bool IsDelimiter(int c)
        {
            return Del.BinarySearch(c)>=0;
        }

        internal static bool IsDigitDelimiter(int c)
        {
            return DigitDelimiters.BinarySearch(c) >= 0;
        }

        internal static bool IsLetterDelimiter(int c)
        {
            return LetterDelimiters.BinarySearch(c) >= 0;
        }
    }
}
