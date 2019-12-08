using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Flow.Collections
{
    public class BinaryTree<Node> : IEnumerable<Node> where Node : IComparable<Node>
    {
        public BinaryTree<Node> Left { get; private set; }
        public BinaryTree<Node> Right { get; private set; }
        public Node Value { get; private set; }
        public int Count
        {
            get
            {
                int left = 0;
                int right = 0;
                if (Left != null) left = Left.Count;
                if (Right != null) right = Right.Count;
                return left + right + 1;
            }
        }
        public BinaryTree(Node root)
        {
            Value = root;
        }
        public Node Insert(Node node)
        {
            int comparison = Value.CompareTo(node);
            if (comparison > 0)
            {
                if (Left != null)
                    return Left.Insert(node);
                Left = new BinaryTree<Node>(node);
                return node;
            }
            else if (comparison < 0)
            {
                if (Right != null)
                    return Right.Insert(node);
                Right = new BinaryTree<Node>(node);
                return node;
            }
            return Value;
        }
        public IEnumerator<Node> GetEnumerator()
        {
            if (Left != null)
                foreach (Node node in Left)
                    yield return node;
            yield return Value;
            if (Right != null)
                foreach (Node node in Right)
                    yield return node;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (Left != null)
                foreach (Node node in Left)
                    yield return node;
            yield return Value;
            if (Right != null)
                foreach (Node node in Right)
                    yield return node;
        }
    }
}
