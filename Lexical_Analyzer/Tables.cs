using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Lexical_Analyzer
{
    static class Tables
    {
        static string FileRW, FileDl, FileMDl;
        internal static Dictionary<string, uint> ResWor;
        internal static Dictionary<string, uint> Delimiters;
        internal static Dictionary<string, uint> MSDel;

        static Tables()
        {
            FileRW = "reswords.txt";
            FileDl = "delimiters.txt";
            FileMDl = "msdel.txt";

            ResWor = new Dictionary<string, uint>();
            Delimiters = new Dictionary<string, uint>();
            MSDel = new Dictionary<string, uint>();
        }

        static void LoadFromFile(string filename, Dictionary<string, uint> dict)
        {
            if (File.Exists(filename))
            {
                string buf, key;
                StreamReader reader = new StreamReader(filename);
                while (((buf = reader.ReadLine()) != null) && (buf != ""))
                {
                    char del = ' ';
                    String[] substr = buf.Split(del);
                    dict.Add(key = substr[0], UInt32.Parse(substr[1]));
                    //Console.WriteLine("{0}-{1}", dict[key], key);
                }
                reader.Close();
            }
            else throw new FileNotFoundException();
        }

        static internal void CreateTables()
        {
            LoadFromFile(FileDl, Delimiters);
            LoadFromFile(FileRW, ResWor);
            LoadFromFile(FileMDl, MSDel);
        }

    }
}
