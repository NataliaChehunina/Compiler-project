
using System.Collections.Generic;

namespace Lexical_Analyzer
{
    static class ListTHelper
    {
        internal static uint GetAndRemove(this List<uint> lst,int index = 0)
        {
            uint token;
            if (lst.Count > 0)
            {
                token = lst[index];
                lst.RemoveAt(index);
                return token;
            }
            else
                token = 0;
            return token;
        }

        internal static int GetIndex(this List<uint> lst, int length)
        {
            return length - lst.Count;
        }
    }
}
