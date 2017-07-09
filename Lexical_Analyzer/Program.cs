using System;
using System.Text;

namespace Lexical_Analyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("<<File Name>>:");
            string str = Console.ReadLine();
            if (!str.EndsWith(".txt"))
            {
                StringBuilder FName = new StringBuilder();
                FName.Append(str).Append(".txt");
                str = FName.ToString();
                Console.WriteLine(str);
            }
            LexicalAnalizer LexAnalizer = new LexicalAnalizer(str);
            LexicalAnalizer.LA lexstruct = LexAnalizer.Out();
            SynthaxAnalyzer SA = new SynthaxAnalyzer(lexstruct);
            CodeGen g = new CodeGen(lexstruct);
            TreeAnalysis tree = SA.Parser();
            tree.RootNode.PrintNode();
            Console.WriteLine("\n\n\n");
            g.Generator(tree);
        }
    }
}
