using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Lexical_Analyzer
{
    class LexicalAnalizer
    {
        public struct LA
        {
             public Dictionary<uint, string> Identifiers;
             public Dictionary<uint, string> Constants;
             public List<uint> Lexems;

            public LA(List<uint>lst,params Dictionary<uint, string> [] hashtables)
            {
                this.Lexems = lst;
                this.Constants = hashtables[0];
                this.Identifiers = hashtables[1];
            }
        }
        string FileName;
        Dictionary<string, uint> Identifiers;
        Dictionary<string, uint> Constants;
        Dictionary<uint, string> ReverseIdentifiers;
        Dictionary<uint, string> ReverseConstants;
        List<uint> lexemslist = new List<uint>();

        string FileResult;
        //bool FLSquareBrackets;
        int SBCounter;

        public LexicalAnalizer(string filename)
        {
            FileName = filename;
            FileResult = "lexems.txt";
            //FLSquareBrackets = true;
            SBCounter = 0;
            ReverseConstants = new Dictionary<uint,string>();
            ReverseIdentifiers = new Dictionary<uint, string>();
            Identifiers = new Dictionary<string, uint>();
            Constants = new Dictionary<string, uint>();
        }

        void WriteTable(string filename,string resultf)
        {
            if (File.Exists(filename))
            {
                StreamReader reader = new StreamReader(filename,Encoding.ASCII);
                StreamWriter wrConst = new StreamWriter("constants.txt", false, Encoding.ASCII);
                StreamWriter wrRes = new StreamWriter(resultf, false, Encoding.ASCII);
                StreamWriter wrIdent = new StreamWriter("identifiers.txt", false, Encoding.ASCII);
                int line = 1,buf = reader.Read();
                uint iconst = 500, iident = 1000;
                char c;
                string lexem;
                StringBuilder str = new StringBuilder();
                while (buf >= 0)
                {
                    switch (Symbol.GetASCIICategory(buf))
                    {
                        case Symbol.Category.Letter:
                            while ((Symbol.IsLetter(buf) || (Symbol.IsDecimal(buf))))
                            {
                                c = (char)buf;
                                str.Append(c);
                                buf = reader.Read();
                            }
                            if (!Symbol.IsLetterDelimiter(buf) && !Symbol.IsWhiteSpace(buf))
                            {
                                wrRes.Write("LE ");
                                Console.WriteLine("Lexical Error : line{0}!!!", line);
                                while (!Symbol.IsWhiteSpace(buf) && !Symbol.IsDelimiter(buf))
                                    buf = reader.Read();
                            }
                            else
                            {
                                lexem = str.ToString().ToUpper();
                                bool flRW = false;
                                if ((Identifiers.ContainsKey(lexem)) || (flRW = Tables.ResWor.ContainsKey(lexem)))
                                {
                                    if (flRW) {
                                        lexemslist.Add(Tables.ResWor[lexem]);
                                        wrRes.Write("{0} ", Tables.ResWor[lexem]); }
                                    else {
                                        lexemslist.Add(Identifiers[lexem]);
                                        wrRes.Write("{0} ", Identifiers[lexem]);}
                                }
                                else
                                {
                                    iident++;
                                    ReverseIdentifiers.Add(iident, lexem);
                                    Identifiers.Add(lexem, iident);
                                    wrIdent.WriteLine("{0} {1}", lexem, iident);
                                    wrRes.Write("{0} ", Identifiers[lexem]);
                                    lexemslist.Add(Identifiers[lexem]);
                                }   
                            }
                            break;

                        case Symbol.Category.Digital:
                            while (Symbol.IsDecimal(buf))
                            {
                                c = (char)buf;
                                str.Append(c);
                                buf = reader.Read();
                            }
                            if (!Symbol.IsDigitDelimiter(buf) && !Symbol.IsWhiteSpace(buf))
                            {
                                wrRes.Write("LE ");
                                Console.WriteLine("Lexical Error : line{0}!!!", line);
                                while (!Symbol.IsWhiteSpace(buf) && !Symbol.IsDelimiter(buf))
                                    buf = reader.Read();
                            }
                            else
                            {
                                lexem = str.ToString();
                                if (!Constants.ContainsKey(lexem))
                                {
                                    iconst++;
                                    if (iconst > 1001) { throw new ArgumentOutOfRangeException(); }
                                    Constants.Add(lexem, iconst);
                                    ReverseConstants.Add(iconst, lexem);
                                    wrConst.WriteLine("{0} {1}", lexem, iconst);
                                }
                                wrRes.Write("{0} ", Constants[lexem]);
                                lexemslist.Add(Constants[lexem]);
                            }
                            break;

                        case Symbol.Category.Whitespace:
                            if (buf == 13) { line++; wrRes.WriteLine(); }
                            buf = reader.Read();
                            while ((Symbol.IsWhiteSpace(buf))&&(buf>=0))
                            {
                                if (buf == 13) { line++; wrRes.WriteLine(); }
                                buf = reader.Read();
                            }
                            break;

                        case Symbol.Category.Delimiter:
                            string key = String.Concat((char)buf);
                            if (Tables.Delimiters.ContainsKey(key))
                            {
                                bool sbflag = (buf == 93);
                                if ((buf == 91) || (sbflag))//'[' or ']'  
                                {
                                    if (sbflag)//']'
                                    {
                                        if (SBCounter > 0)
                                        {
                                            wrRes.Write("{0} ",Tables.Delimiters[key]);
                                            lexemslist.Add(Tables.Delimiters[key]);
                                            //FLSquareBrackets = !FLSquareBrackets;
                                            SBCounter--;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Lexical Error : line{0}!!!", line);
                                            wrRes.Write("LE ");
                                        }
                                        buf = reader.Read();
                                    }
                                    else
                                    {
                                        /*
                                        if (SBCounter == 0)
                                        {
                                            lexemslist.Add(Tables.Delimiters[key]);
                                            wrRes.Write("{0} ", Tables.Delimiters[key]);
                                            FLSquareBrackets = !FLSquareBrackets;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Lexical Error : line{0}!!!", line);
                                            wrRes.Write("LE ");
                                        }*/
                                        lexemslist.Add(Tables.Delimiters[key]);
                                        wrRes.Write("{0} ", Tables.Delimiters[key]);
                                        //FLSquareBrackets = !FLSquareBrackets;
                                        SBCounter++;
                                        buf = reader.Read();
                                    }
                                }
                                else
                                {
                                    buf = reader.Read();
                                    if (Tables.MSDel.ContainsKey(String.Concat(key, (char)buf)))
                                    {
                                        lexemslist.Add(Tables.MSDel[String.Concat(key, (char)buf)]);
                                        wrRes.Write("{0} ", Tables.MSDel[String.Concat(key, (char)buf)]);
                                        buf = reader.Read();
                                    }
                                    else
                                    {
                                        lexemslist.Add(Tables.Delimiters[key]);
                                        wrRes.Write("{0} ", Tables.Delimiters[key]);
                                    }
                                }
                            }
                            else//comments
                            {
                                buf = reader.Read();
                                key = String.Concat(key,(char)buf);
                                if (Tables.MSDel.ContainsKey(key))
                                {
                                    bool cflag = true;
                                    while(cflag)
                                    {
                                        buf = reader.Read();
                                        if (buf == 42)//*
                                        {
                                            buf = reader.Read();
                                            if (buf == 41)//)
                                            {
                                                cflag = false;
                                            }
                                        }
                                           if (buf == -1)//eof
                                           {
                                               Console.WriteLine("Lexical Error : line{0}!!!", line);
                                               wrRes.Write("LE ");
                                               break;
                                           }
                                    }
                                    buf = reader.Read();
                                }
                                else
                                {
                                    Console.WriteLine("Lexical Error : line{0}!!!", line);
                                    wrRes.Write("LE ");
                                    buf = reader.Read();
                                }
                            }
                          break;   

                        case Symbol.Category.Error:
                            Console.WriteLine("Lexical Error : line{0}!!!",line);
                            wrRes.Write("LE ");
                            buf = reader.Read();
                            while (Symbol.GetASCIICategory(buf) == Symbol.Category.Error)
                            {
                                buf = reader.Read();
                            }
                            break;
                    }
                    str.Remove(0,str.Length);
                }
                if (SBCounter != 0)
                {
                    Console.WriteLine("Lexical Error : line{0}!!!", line);
                    wrRes.Write("LE "); 
                }
                wrConst.Close();
                reader.Close();
                wrRes.Close();
                wrIdent.Close();
            }
            else throw new FileNotFoundException();            
        }
        
        public LA Out()
        {
            Tables.CreateTables();
            this.WriteTable(FileName, FileResult);
            LA resulstruct = new LA(lexemslist, ReverseConstants, ReverseIdentifiers);
            return resulstruct;
        }


    }
}
