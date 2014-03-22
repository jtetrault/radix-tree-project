using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadixTree
{
    class RedBlackTree : StringDictionary
    {
        /*** Public Interface ************************************************/
        /// <summary>
        /// Search the tree for key. Return true if the key is found, and false if not.
        /// </summary>
        public bool Search(String key)
        {
            Node current = Root;
            while (current != null && !key.Equals(current.Key))
            {
                if (key.CompareTo(current.Key) < 0) 
                {
                    current = current.Left;
                }
                else
                {
                    current = current.Right;
                }
            }

            // If we are looking at null, then key was not found and we return false.
            // If we are not looking at null, then key was found and we return true.
            return current != null;
        }

        public void Insert(String key)
        {
            Node current = this.Root;
            Node previous = null;
            Node toInsert = new Node(key);

            // Find the leaf location to place toInsert.
            while (current != null)
            {
                previous = current;
                if (toInsert.Key.CompareTo(current.Key) < 0)
                {
                    current = current.Left;
                }
                else
                {
                    current = current.Right;
                }
            }

            toInsert.Parent = previous;

            if (previous == null) // Empty Tree.
            {
                this.Root = toInsert;
            }
            else if (toInsert.Key.CompareTo(previous.Key) < 0) // toInsert is a left child.
            {
                previous.Left = toInsert;
            }
            else // toInsert is a right child.
            {
                previous.Right = toInsert;
            }
        }

        public void Delete(String key)
        {

        }

        public String Predecessor(String key)
        {
            return null;
        }

        public String Successor(String key)
        {
            return null;
        }

        /*** Instance Variables **********************************************/

        private Node Root { get; set; }

        private int Size { get; set; }
    }

    public class Node
    {
        /// <summary>
        /// Creates a Node instance with null Parent, Left and Right pointers,
        /// with its Key set to key.
        /// </summary>
        public Node(String key)
        {
            this.Key = key;
            this.Parent = this.Left = this.Right = null;
        }

        /// <summary>
        /// The key contained in this Node.
        /// </summary>
        public String Key { get; set; }

        /// <summary>
        /// The Node's parent in the Binary Search Tree.
        /// </summary>
        public Node Parent { get; set; }

        /// <summary>
        /// The Node's left child.
        /// </summary>
        public Node Left { get; set; }

        /// <summary>
        /// The Node's right child.
        /// </summary>
        public Node Right { get; set; }
    }
}
