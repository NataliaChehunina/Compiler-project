using System;
using System.Collections.Generic;

namespace Lexical_Analyzer
{
    class TreeAnalysis
    {
        public OneParentNode RootNode;
        public TreeAnalysis() { }
        public TreeAnalysis(OneParentNode root) { RootNode = root; }
    }

    class Node
    {
        public string Name { set; get; }
        public Node() {Name = "DefaultNode";}
        public Node(string name) {Name = name;}
        //public Node getChildren() { return this; };
        public virtual void PrintNode(int level = 0)
        {
            string offset = "";
            for (int i = 0; i < level; i++)
                offset += "  ";
            Console.Write($"{offset}({Name} ");
        }
    }

    class OneParentNode : Node
    {
        protected Node children;
        public Node getChildren() { return children; }
        public OneParentNode() { }
        public OneParentNode(string name, Node child) : base(name)
        {
            children = child;
        }

        public override void PrintNode(int level = 0)
        {
            base.PrintNode(level);
            children.PrintNode();
            Console.Write(")");
        }
    }

    class ContainerNode : Node
    {
        protected uint value;
        public uint getValue() { return value; }
        public ContainerNode() { }
        public ContainerNode(string name, uint val) : base(name)
        {
            value = val;
        }

        public override void PrintNode(int level = 0)
        {
            base.PrintNode(level);
            Console.Write(value + ")");
        }
    }

    class ParentNode : Node
    {
        protected List<Node> Children;
        public List<Node> getChildren() { return Children; }
        public ParentNode() { Children = new List<Node>(); }
        public ParentNode(string name, params Node[] nodes) : base(name)
        {
            Children = new List<Node>();
            foreach (Node elem in nodes)
                Children.Add(elem);           
        }

        public override void PrintNode(int level = 0)
        {
            base.PrintNode(level);
            Console.WriteLine();
            for (int i = 0; i < Children.Count-1; i++)
            {
                Children[i].PrintNode(level + 1);
                Console.WriteLine();
            }
            Children[Children.Count - 1].PrintNode(level + 1);
            Console.Write(")");
        }

        public ParentNode AddChildren(Node child)
        {
            Children.Add(child);
            return this;
        }
        public ParentNode AddChildren(string name, uint value)
        {
            Children.Add(new ContainerNode(name, value));
            return this;
        }
        public ParentNode PushChildren(Node child)
        {
            Children.Insert(0, child);
            return this;
        }
        public ParentNode PushChildren(string name, uint value)
        {
            Children.Insert(0, new ContainerNode(name, value));
            return this;
        }
    }

    class OneLineParentNode : ParentNode
    {
        public OneLineParentNode(string name, params Node[] nodes) : base(name, nodes) { }

        public override void PrintNode(int level = 0)
        {
            string offset = "";
            for (int i = 0; i < level; i++)
                offset += "  ";
            Console.Write($"{offset}({Name} ");        
            for (int i = 0; i < Children.Count - 1; i++)
            {
                Children[i].PrintNode();
                Console.Write(" ");
            }
            if (Children.Count>0)
                Children[Children.Count - 1].PrintNode();
            Console.Write(")");
        }

        public new OneLineParentNode AddChildren(Node child)
        {
            Children.Add(child);
            return this;
        }
        public new OneLineParentNode AddChildren(string name, uint value)
        {
            Children.Add(new ContainerNode(name, value));
            return this;
        }
        public new OneLineParentNode PushChildren(Node child)
        {
            Children.Insert(0, child);
            return this;
        }
        public new OneLineParentNode PushChildren(string name, uint value)
        {
            Children.Insert(0, new ContainerNode(name, value));
            return this;
        }
    }
}
