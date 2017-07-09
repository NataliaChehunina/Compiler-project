using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Lexical_Analyzer
{
    public struct variable
    {
        public uint size;
        public bool flag;
        public string reg;
        public uint low;
    }
    class CodeGen
    {
        LexicalAnalizer.LA Input;
        uint ValueBuffer = 0;
        uint IdBuffer = 0;
        Queue<uint> queue;
        Stack<uint> stack;
        Dictionary<uint, variable> vars;
        Dictionary<uint, string> varRegs;
        uint LoopCounter = 0;
        int TargetCounter = 0;

        StreamWriter stream;

        public CodeGen() { }

        public CodeGen(LexicalAnalizer.LA input)
        {
            Input = input;
            queue = new Queue<uint>();
            stack = new Stack<uint>();
            vars = new Dictionary<uint, variable>();
            varRegs = new Dictionary<uint, string>();
        }

        public void Generator(TreeAnalysis tree)
        {  
            stream = new StreamWriter("RESULT.txt", false, Encoding.Default);
            DepthFirstRun(tree.RootNode);
            stream.Close();
        }

        void DepthFirstRun(Node tree)
        {
            if (tree is OneParentNode)
            {
                Node children = ((OneParentNode)tree).getChildren();
                switch (tree.Name)
                {
                    case "Expr":
                        if (children is ContainerNode)
                        {
                            DepthFirstRun(children);
                            stream.WriteLine($"Mov eax,{Input.Constants[stack.Pop()]}");
                        }
                        else
                        {
                            DepthFirstRun(children);
                            uint idbuff = ValueBuffer;
                            stream.WriteLine($"Mov eax,{Input.Identifiers[idbuff]}[ebx]");
                            //stream.WriteLine($"mov eax,{varRegs[idbuff]}[ebx]");
                        }
                        break;
                    default:
                        DepthFirstRun(((OneParentNode)tree).getChildren());
                        break;
                }
                
                return;
            }
            if (tree is ParentNode)
            {
                List<Node> children = ((ParentNode)tree).getChildren();
                switch (tree.Name)
                {
                    case "Program":
                        DepthFirstRun(children[1]);
                        stream.Write(ASM.BegProgr(Input.Identifiers[stack.Pop()]));
                        DepthFirstRun(children[3]);
                        stream.Write(ASM.End());
                        break;
                    case "VariableDeclaration":
                        stream.WriteLine(ASM.Data());
                        DepthFirstRun(children[1]);
                        break;
                    case "Declaration":
                        DepthFirstRun(children[0]);
                        DepthFirstRun(children[2]);
                        break;
                    case "Attribute":
                        IdBuffer = stack.Pop();
                        DepthFirstRun(children[0]);
                        switch(stack.Pop())
                        {
                            case 405:
                                if (vars.ContainsKey(IdBuffer))
                                    stream.WriteLine($";Error, dublicate id: {Input.Identifiers[IdBuffer]}");
                                else
                                    stream.WriteLine(ASM.DW(Input.Identifiers[IdBuffer]));
                                vars[IdBuffer] = new variable { size = 16, flag = false };
                                break;
                            case 406:
                                if (vars.ContainsKey(IdBuffer))
                                    stream.WriteLine($";Error, dublicate id: {Input.Identifiers[IdBuffer]}");
                                else
                                    stream.WriteLine(ASM.DD(Input.Identifiers[IdBuffer]));
                                vars[IdBuffer] = new variable { size = 32, flag = false };
                                break;
                            case 91:
                                DepthFirstRun(children[1]);
                                break;
                        }
                        break;
                    case "Range":
                        DepthFirstRun(children[0]);
                        DepthFirstRun(children[2]);
                        ValueBuffer = stack.Pop();
                        uint Low = stack.Peek();
                        if (vars.ContainsKey(IdBuffer))
                            stream.WriteLine($";Error, dublicate id: {Input.Identifiers[IdBuffer]}");
                        else
                        {
                            vars[IdBuffer] = new variable { size = 0, flag = false,
                                low = uint.Parse(Input.Constants[Low])};
                            stream.WriteLine(ASM.Array(Input.Identifiers[IdBuffer],
                                uint.Parse(Input.Constants[stack.Pop()]),
                                uint.Parse(Input.Constants[ValueBuffer])));
                        }
                        
                        break;
                    case "Block":
                        DepthFirstRun(children[0]);
                        stream.WriteLine(ASM.Begin());
                        foreach (var pair in vars)
                        {
                            stream.WriteLine(ASM.Mov(TargetCounter, Input.Identifiers[pair.Key]));
                            //Console.WriteLine(ASM.Reg32[TargetCounter]);
                            //vars[pair.Key].setReg(ASM.Reg32[TargetCounter++]);
                            //Console.WriteLine(vars[pair.Key].reg);
                            varRegs[pair.Key] = ASM.Reg32[TargetCounter++];
                        }
                        stream.WriteLine();
                        DepthFirstRun(children[2]);
                        break;
                    case "Stmt":
                        if (children[1] is ContainerNode)
                        {
                            DepthFirstRun(children[0]);
                            uint idbuff = ValueBuffer;
                            stream.WriteLine("Mov edx,ebx");
                            DepthFirstRun(children[2]);
                            if (vars.ContainsKey(idbuff))
                                stream.WriteLine($"Mov {Input.Identifiers[idbuff]}[edx], eax");
                                //stream.WriteLine($"Mov {varRegs[idbuff]}[edx], eax");
                            else
                                stream.WriteLine($";Error, undeclared variable: {Input.Identifiers[idbuff]}");
                        }
                        else
                        {
                            uint loop = LoopCounter++;
                            stream.WriteLine(ASM.BeginLoop(loop));
                            DepthFirstRun(children[1]);
                            stream.WriteLine(ASM.EndLoop(loop));
                        }
                        stream.WriteLine();
                        break;
                    case "Variable":
                        DepthFirstRun(children[0]);
                        uint buff = stack.Pop();
                        if (!vars.ContainsKey(buff))
                        {
                            stream.WriteLine($";Error! Undeclared variable: {Input.Identifiers[buff]}");
                            stream.WriteLine("Mov eax,0");
                        }
                        else
                            if (((ParentNode)children[1]).getChildren().Capacity > 0)
                            {
                                if (vars[buff].size != 0)
                                    stream.WriteLine($";Warning, variable is not an array: {Input.Identifiers[buff]}");
                                DepthFirstRun(children[1]);
                                stream.WriteLine("Mov ebx,eax");
                                stream.WriteLine(ASM.Sub("ebx", vars[buff].low));
                            }
                            else
                                stream.WriteLine("Mov ebx,0");
                        ValueBuffer = buff;
                        break;
                    case "Dimension":
                        DepthFirstRun(children[1]);
                        break;
                    default:
                        foreach (Node child in ((ParentNode)tree).getChildren())
                            DepthFirstRun(child);
                        stack.Clear();
                        break;
                }
                return;
            }
            if (tree is ContainerNode)
            {
                ValueBuffer = ((ContainerNode)tree).getValue();
                stack.Push(((ContainerNode)tree).getValue());
                return;
            }
        }

    }
}
