using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate bool Condition();

public class Node
{
    public Node[] children;
    public Condition condition;

    public Node()
    {
        condition = null;
        children = null;
    }

    public Node(Condition _condition)
    {
        condition = _condition;
        children = null;
    }
}

public class Branch 
{
    public Condition condition;
    public Node node;

    public Branch(Condition _condition, Node _node)
    {
        condition = _condition;
        node = _node;
    }
}

public class Fork : Node
{

    public Branch[] branches;
    public Node defaultBranch;

    public Fork(Branch[] _branches, Node _default)
    {
        if (_default == null)
            defaultBranch = null;
        else
            defaultBranch = _default;

        branches = _branches;
    }

    public Fork()
    {
        branches = null;
        defaultBranch = null;
    }
}

public class Leaf : Node
{
    public delegate void Callback();

    public Callback callback;

    public Leaf(Callback _callback)
    {
        callback = _callback;
    }
}

public class BehaviourTree
{
    public Node root = new Node();

    public void Execute()
    {
        Node[] threads = { root };
        
        while (threads.Length > 0)
        {
            List<Node> newThreads = new List<Node>();

            for (int i = 0; i < threads.Length; ++i)
            {
                Node node = threads[i];
                Node[] children = node.children;

                if (node.GetType() == typeof(Node))
                {
                    if (node.condition != null)
                    {
                        if (!node.condition())
                            continue;
                    }

                    if (children == null)
                        continue;

                    for (int j = 0; j < children.Length; ++j)
                        newThreads.Add(children[j]);
                }

                if (node.GetType() == typeof(Fork))
                {
                    Fork fork = (Fork)node;
                    bool foundPath = false;
                    for (int j = 0; j < fork.branches.Length; ++j)
                    {
                        if (fork.branches[j].condition())
                        {
                            foundPath = true;
                            newThreads.Add(fork.branches[j].node);
                            break;
                        }
                    }

                    if (!foundPath && fork.defaultBranch != null)
                        newThreads.Add(fork.defaultBranch);
                }

                if (node.GetType() == typeof(Leaf))
                {
                    Leaf leaf = (Leaf)node;
                    leaf.callback();
                }
            }

            threads = newThreads.ToArray();
        }
    }
}
