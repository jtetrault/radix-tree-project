using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadixTree
{
    public enum NodeColor { Black, Red };

    class RedBlackTree : StringDictionary
    {
        /*** Public Interface ************************************************/
        /// <summary>
        /// Initialize the Red Black Tree. Set the Root to Nil, and Size to 0.
        /// </summary>
        public RedBlackTree()
        {
            this.Nil = new Node(null);
            this.Nil.Left = this.Nil;
            this.Nil.Right = this.Nil;
            this.Nil.Color = NodeColor.Black;
            this.Root = this.Nil;
            this.Size = 0;
        }

        /// <summary>
        /// Search the tree for key. Return true if the key is found, and false if not.
        /// </summary>
        public bool Search(String key)
        {
            return this.SearchForNode(key) != this.Nil;
        }

        /// <summary>
        /// Insert a key into the tree. If the key already exists in the tree, then do not insert.
        /// </summary>
        public void Insert(String key)
        {
            Node current = this.Root;
            Node previous = this.Nil;
            Node toInsert;

            // Find the leaf location to place toInsert.
            while (current != this.Nil && !current.Key.Equals(key))
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

            if (current != this.Nil) // Disallow duplicate keys.
            {
                return;
            }

            toInsert = new Node(key);
            toInsert.Parent = previous;

            if (previous == this.Nil) // Empty Tree.
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
            toInsert.Left = this.Nil;
            toInsert.Right = this.Nil;
            toInsert.Color = NodeColor.Red;

            this.InsertFixup(toInsert);

            this.Size++;
        }

        /// <summary>
        /// Removes a key from the tree.
        /// </summary>
        public void Delete(String key)
        {
            Node toDelete = this.SearchForNode(key);
            if (toDelete != this.Nil)
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
            if (toDelete.Left == this.Nil) // 0 children or only right child.
            {
                Transplant(toDelete, toDelete.Right);
            }
            else if (toDelete.Right == this.Nil) // Only left child.
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
        /// Fixes up the Red Black Tree after an insertion, ensuring that the Red Black Tree properties are intact.
        /// </summary>
        private void InsertFixup(Node current)
        {
            Node grandparent;
            Node uncle;
            while (current.Parent.Color == NodeColor.Red)
            {
                grandparent = current.Parent.Parent;
                if (current.Parent == grandparent.Left)
                {
                    uncle = grandparent.Right;
                    if (uncle.Color == NodeColor.Red)
                    {
                        current.Parent.Color = NodeColor.Black;
                        uncle.Color = NodeColor.Black;
                        grandparent.Color = NodeColor.Red;
                        current = grandparent;
                    }
                    else
                    {
                        if (current == current.Parent.Right)
                        {
                            current = current.Parent;
                            this.LeftRotate(current);
                        }
                        current.Parent.Color = NodeColor.Black;
                        grandparent = current.Parent.Parent;
                        grandparent.Color = NodeColor.Red;
                        this.RightRotate(grandparent);
                    }
                }
                else // current.Parent == grandparent.Right
                {
                    uncle = grandparent.Left;
                    if (uncle.Color == NodeColor.Red)
                    {
                        current.Parent.Color = NodeColor.Black;
                        uncle.Color = NodeColor.Black;
                        grandparent.Color = NodeColor.Red;
                        current = grandparent;
                    }
                    else
                    {
                        if (current == current.Parent.Left)
                        {
                            current = current.Parent;
                            this.RightRotate(current);
                        }
                        current.Parent.Color = NodeColor.Black;
                        grandparent = current.Parent.Parent;
                        grandparent.Color = NodeColor.Red;
                        this.LeftRotate(grandparent);
                    }
                }
            }
            this.Root.Color = NodeColor.Black;
        }

        /// <summary>
        /// Perform a left rotation on the Node toRotate.
        /// </summary>
        private void LeftRotate(Node toRotate)
        {
            Node newParent = toRotate.Right;
            toRotate.Right = newParent.Left;
            if (newParent.Left != this.Nil)
            {
                newParent.Left.Parent = toRotate;
            }
            newParent.Parent = toRotate.Parent;
            if (toRotate.Parent == this.Nil)
            {
                this.Root = newParent;
            }
            else if (toRotate == toRotate.Parent.Left)
            {
                toRotate.Parent.Left = newParent;
            }
            else
            {
                toRotate.Parent.Right = newParent;
            }
            newParent.Left = toRotate;
            toRotate.Parent = newParent;
        }

        /// <summary>
        /// Perform a right rotation on the node toRotate.
        /// </summary>
        private void RightRotate(Node toRotate)
        {
            Node newParent = toRotate.Left;
            toRotate.Left = newParent.Right;
            if (newParent.Right != this.Nil)
            {
                newParent.Right.Parent = toRotate;
            }
            newParent.Parent = toRotate.Parent;
            if (toRotate.Parent == this.Nil)
            {
                this.Root = newParent;
            }
            else if (toRotate == toRotate.Parent.Right)
            {
                toRotate.Parent.Right = newParent;
            }
            else
            {
                toRotate.Parent.Left = newParent;
            }
            newParent.Right = toRotate;
            toRotate.Parent = newParent;
        }

        /// <summary>
        /// Search the tree for a Node containing key and return it. If no node is found, return null.
        /// </summary>
        private Node SearchForNode(string key)
        {
            Node current = Root;
            while (current != this.Nil && !key.Equals(current.Key))
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
            if (replacement != this.Nil) // Update the parent of the replacement node
            {
                replacement.Parent = current.Parent;
            } 
        }

        /*** Instance Variables **********************************************/

        /// <summary>
        /// The root of the Red Black Tree.
        /// </summary>
        private Node Root { get; set; }

        /// <summary>
        /// A sentinel Node used as a placeholder for null.
        /// </summary>
        private Node Nil { get; set; }

        /// <summary>
        /// The number of keys stored in the Red Black Tree.
        /// </summary>
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
            this.Color = NodeColor.Red;
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

        /*** Instance Variables **********************************************/
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

        /// <summary>
        /// The current color of this Node.
        /// </summary>
        public NodeColor Color { get; set; }
    }
}
