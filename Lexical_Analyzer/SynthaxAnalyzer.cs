using System;

namespace Lexical_Analyzer
{
    class SynthaxAnalyzer
    {
        LexicalAnalizer.LA Input;
        int ProgramLength;
        string key = "KeyWord", del = "Delimiter";


        public SynthaxAnalyzer() { }
        
        public SynthaxAnalyzer(LexicalAnalizer.LA input)
        {
            Input = input;
            ProgramLength = Input.Lexems.Count;
        }

        public TreeAnalysis Parser()
        {
            TreeAnalysis tree = new TreeAnalysis (new OneParentNode("Tree", Program()));
            //tree.RootNode.PrintNode();
            return tree;
        }

        
        ParentNode Program()
        {
           ParentNode node = new ParentNode("Program");
           uint elem = Input.Lexems.GetAndRemove();
           if (elem != Tables.ResWor["PROGRAM"])
                Error();
            else
            {
                node.AddChildren(new ContainerNode(key, elem));
                node.AddChildren(ProcedureIdentifier());
                if ((elem = Input.Lexems.GetAndRemove()) != Tables.Delimiters[";"])
                    Error();
                else
                {
                    node.AddChildren(new ContainerNode(del, elem));
                    node.AddChildren(Block());
                    if ((elem = Input.Lexems.GetAndRemove()) != Tables.Delimiters["."])
                        Error();
                    else
                        node.AddChildren(new ContainerNode(del, elem));  
                }
            }
            return node;
        }

        ParentNode Block()
        {
            ParentNode node = new ParentNode("Block", VariableDeclarations());
            uint elem = Input.Lexems.GetAndRemove();
            if (elem  != Tables.ResWor["BEGIN"])
                Error();
            else
            {

                node.AddChildren(new ContainerNode(key, elem));
                node.AddChildren(StatementsList());

                if ((elem = Input.Lexems.GetAndRemove()) != Tables.ResWor["END"])
                    Error();
                else
                    node.AddChildren(new ContainerNode(key, elem));
            }
            return node;
        }

        ParentNode VariableDeclarations()
        {
            ParentNode node = new ParentNode("VariableDeclaration");
            uint elem = Input.Lexems[0]; 
            if (elem != Tables.ResWor["VAR"])
            {
                Error();
                return null;
            }            
            else
            {
                node.AddChildren(new ContainerNode(key, elem));
                elem = Input.Lexems.GetAndRemove();
                node.AddChildren(DeclarationsList());
            }
            return node;
        }

        ParentNode DeclarationsList()
        {
            uint elem = Input.Lexems[0];
            if (elem != Tables.ResWor["BEGIN"])
            {
                ParentNode node = Declaration();
                return DeclarationsList().PushChildren(node);
            }
            else
                return new ParentNode("DeclarationsList");
        }

        ParentNode Declaration()
        {
            ParentNode node = new ParentNode("Declaration", VariableIdentifier());
            uint elem = Input.Lexems.GetAndRemove();
            if (elem != Tables.Delimiters[":"])
                Error();
            else
            {
                node.AddChildren(new ContainerNode(del, elem));
                node.AddChildren(Attribute());
                node.AddChildren(AttributesList());
                elem = Input.Lexems.GetAndRemove();
                if (elem != Tables.Delimiters[";"])
                    Error();
                else
                    node.AddChildren(new ContainerNode(del, elem));
            }
            return node;
        }

        OneParentNode VariableIdentifier()
        {
            return new OneParentNode("VariableIdentifier",Identifier());
        }

        OneLineParentNode Attribute()
        {
            OneLineParentNode node = new OneLineParentNode("Attribute");
            uint elem = Input.Lexems.GetAndRemove();

            if (elem == Tables.ResWor["INTEGER"])
            {
                node.AddChildren(new ContainerNode(key, elem));
                return node;
            }
            if (elem == Tables.ResWor["FLOAT"])
            {
                node.AddChildren(new ContainerNode(key, elem));
                return node;
            }
            if (elem == Tables.Delimiters["["])
            {
                node.AddChildren(new ContainerNode(del, elem));
                node.AddChildren(Range());
                elem = Input.Lexems.GetAndRemove();
                if (elem != Tables.Delimiters["]"])
                    Error();
                else
                    node.AddChildren(new ContainerNode(del, elem));
                return node;
            }
            Error();
            return null;
        }

        OneLineParentNode Range()
        {
            OneLineParentNode node = new OneLineParentNode("Range", UnsignedInteger());
            uint elem = Input.Lexems.GetAndRemove();
            if (elem != Tables.MSDel[".."])
                Error();
            else
            {
                node.AddChildren(new ContainerNode(del, elem));
                node.AddChildren(UnsignedInteger());
            }
            return node;
        }

        ContainerNode UnsignedInteger()
        {
            uint elem = Input.Lexems.GetAndRemove();
            if (!Input.Constants.ContainsKey(elem))
            {
                Error();
                return null;
            }
            else
                return new ContainerNode("UInteger", elem);
        }

        OneLineParentNode AttributesList()
        {
            if (Input.Lexems[0] != Tables.Delimiters[";"])
            {
                OneLineParentNode node = Attribute();
                return AttributesList().AddChildren(node);
            }
            else
                return new OneLineParentNode("AttrList");
        }

        ParentNode StatementsList()
        {
            uint elem = Input.Lexems[0];
            if (elem != Tables.ResWor["END"] && elem != Tables.ResWor["ENDLOOP"])
            {
                ParentNode node = Statement();
                return StatementsList().PushChildren(node);
            }
            else
                return new ParentNode("Stmt-list");
        }

        ParentNode Statement()
        {
            ParentNode node = new ParentNode("Stmt");
            uint elem = Input.Lexems[0];
            if (elem == Tables.ResWor["LOOP"])
            {
                node.AddChildren(new ContainerNode(key, Input.Lexems.GetAndRemove()));
                node.AddChildren(StatementsList());
                elem = Input.Lexems.GetAndRemove();
                if (elem != Tables.ResWor["ENDLOOP"])
                    Error();
                else
                    node.AddChildren(new ContainerNode(key, elem));
                elem = Input.Lexems.GetAndRemove();
                if (elem != Tables.Delimiters[";"])
                    Error();
                else
                    node.AddChildren(new ContainerNode(del, elem));
            }
            else
            {
                node.AddChildren(Variable());
                elem = Input.Lexems.GetAndRemove();
                if (elem != Tables.MSDel[":="])
                    Error();
                else
                {
                    node.AddChildren(del, elem);
                    node.AddChildren(Expression());
                    elem = Input.Lexems.GetAndRemove();
                    if (elem != Tables.Delimiters[";"])
                        Error();
                    else
                        node.AddChildren(del, elem);
                }
            }
            return node;
        }

        OneLineParentNode Variable()
        {
            return new OneLineParentNode("Variable", VariableIdentifier(), Dimension());           
        }

        OneLineParentNode Dimension()
        {
            OneLineParentNode node = new OneLineParentNode("Dimension");
            if (Input.Lexems[0] == Tables.Delimiters["["])
            {
                node.AddChildren(del, Input.Lexems.GetAndRemove());
                node.AddChildren(Expression());
                if (Input.Lexems[0] != Tables.Delimiters["]"])
                    Error();
                else
                    node.AddChildren(del, Input.Lexems.GetAndRemove());
            }
            return node;
        }

        OneParentNode Expression()
        {
            if (Input.Constants.ContainsKey(Input.Lexems[0]))
                return new OneParentNode("Expr", UnsignedInteger());
            else
                return new OneParentNode("Expr", Variable());
        }

        OneParentNode ProcedureIdentifier()
        {
            return new OneParentNode("ProcedureIdentifier", Identifier());
        }

        ContainerNode Identifier ()
        {
            uint elem = Input.Lexems.GetAndRemove();
            if (!Input.Identifiers.ContainsKey(elem))
            {
                Error();
                return null;
            }
            else
                return new ContainerNode("Identifier", elem);
        }

        void Error()
        {
            Console.WriteLine($"Synthax Error in this token : {Input.Lexems.GetIndex(ProgramLength)}!!!");
            throw new Exception("SynthaxError!!!");
        }
     
    }
}
