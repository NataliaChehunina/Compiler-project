using System;
using System.Collections.Generic;


namespace Lexical_Analyzer
{
    static class ASM
    {
        static internal List<string> Reg32 = new List<string> (){"ecx","esi",
            "edi","ebp","esp","eip" };
        //static internal List<string> Reg16 = new List<string>(){"ax","bx","cx","dx","si",
            //"di","bp","sp","ip"};
        static internal uint LoopQuantCounter = 0;

        static internal string BegProgr(string Ident)
        {
            return $";Program {Ident}\n";
        }

        static internal string Mov<T> (int id,T Value)
        {
            if (Value is UInt32 || Value is String)
                return $"Mov {Reg32[id % Reg32.Capacity]},{Value}";
            return "Error!";
        }

        static internal string Add<T>(string Name, T Value)
        {
            if (Value is UInt32 || Value is String)
                return $"Add {Name},{Value}";
            return "Error!";
        }

        static internal string Sub<T>(string Name, T Value)
        {
            if (Value is UInt32 || Value is String)
                return $"Sub {Name},{Value}";
            return "Error!";
        }

        static internal string Begin()
        { return ".Code \r\n Start:"; }

        static internal string End()
        { return "End Start"; }

        static internal string Index(uint index,string ident)
        { return $"{ident}+{index}"; }
            
        static uint GetLength(uint low,uint high)
        {
            if (high < low)
                Swap(ref low, ref high);
            return high - low + 1;
        }

        static internal uint GetOffset(uint low,uint CurrentIndex)
        {
            if (CurrentIndex < low)
                Swap(ref CurrentIndex, ref low);
            return CurrentIndex - low;
        }

        static void Swap(ref uint a,ref uint b)
        {
            uint c = a;
            a = b;
            b = c;
        }

        static internal string BeginLoop(uint LoopQuantCounter)
        {
            return $"?L{LoopQuantCounter}:";
        }

        static internal string EndLoop(uint LoopQuantCounter)
        {
            return $"Jmp ?L{LoopQuantCounter}";
        }

        static internal string Data()
        { return ".Data"; }

        static internal string DD(string Identifier,string Value = "?")
        {
            return $"{Identifier} dd {Value}";
        }

        static internal string DW(string Identifier, string Value = "?")
        {
            return $"{Identifier} dw {Value}";
        }

        static internal string Array(string Identifier, uint low, uint high)
        {
            return $"{Identifier} dw {GetLength(low,high)} dup (?)";
        }
    }
}
