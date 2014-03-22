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
            return this.SearchForNode(key) != null;
        }

        /// <summary>
        /// Insert a key into the tree. If the key already exists in the tree, then do not insert.
        /// </summary>
        public void Insert(String key)
        {
            Node current = this.Root;
            Node previous = null;

            // Find the leaf location to place toInsert.
            while (current != null && !current.Key.Equals(key))
            {
                previous = current;
                if (key.CompareTo(current.Key) < 0)
                {
                    current = current.Left;
                }
                else
                {
                    current = current.Right;
                }
            }

            if (current != null) // Disallow duplicate keys.
            {
                return;
            }

            Node toInsert = new Node(key);
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

            this.Size++;
        }

        /// <summary>
        /// Removes a key from the tree.
        /// </summary>
        public void Delete(String key)
        {
            Node toDelete = this.SearchForNode(key);
            if (toDelete != null)
            {
                this.DeleteNode(toDelete);
                this.Size--;
            }
        }

        public String Predecessor(String key)
        {
            return null;
        }

        public String Successor(String key)
        {
            return null;
        }

        /** Private Methods **************************************************/
        /// <summary>
        /// Removes a Node from the tree, replacing it with one of its children as needed.
        /// </summary>
        private void DeleteNode(Node toDelete)
        {
            if (toDelete.Left == null) // 0 children or only right child.
            {
                Transplant(toDelete, toDelete.Right);
            }
            else if (toDelete.Right == null) // Only left child.
            {
                Transplant(toDelete, toDelete.Left);
            }
            else // Left and Right child. Replace toDelete with its successor.
            {
                Node successor = toDelete.Minimum();
                if (successor.Parent != toDelete)
                {
                    Transplant(successor, successor.Right);
                    successor.Right = toDelete.Right;
                    successor.Right.Parent = successor;
                }
                Transplant(toDelete, successor);
                successor.Left = toDelete.Left;
                successor.Left.Parent = successor;
            }
        }

        /// <summary>
        /// Search the tree for a Node containing key and return it. If no node is found, return null.
        /// </summary>
        private Node SearchForNode(string key)
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

            return current;
        }

        /// <summary>
        /// Replace one subtree (current) with another (replacement).
        /// 
        /// current's parent node will be updated to point to replacement, and
        /// replacement's parent will be updated to current's parent.
        /// </summary>
        private void Transplant(Node current, Node replacement)
        {
            if (current == this.Root) // Replacing the Root node.
            {
                this.Root = replacement;
            }
            else if (current == current.Parent.Left) // Replacing a left node
            {
                current.Parent.Left = replacement;
            }
            else // Replacing a right node
            {
                current.Parent.Right = replacement;
            }
            if (replacement != null) // Update the parent of the replacement node
            {
                replacement.Parent = current.Parent;
            } 
        }

        /*** Instance Variables **********************************************/

        private Node Root { get; set; }

        private int Size { get; set; }
    }

    public class Node
    {
        /*** Public Interface ************************************************/
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
        /// Returns the leftmost Node in the subtree rooted at this Node. 
        /// </summary>
        public Node Minimum()
        {
            Node current = this;
            while (current.Left != null)
            {
                current = current.Left;
            }
            return current;
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
