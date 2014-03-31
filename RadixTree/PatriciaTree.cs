using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadixTree
{
    public class PatriciaTree : StringDictionary
    {
        /*** Public Interface ************************************************/
        /// <summary>
        /// Create a new, empty Patricia Tree.
        /// </summary>
        public PatriciaTree()
        {
            this.Root = new PatriciaNode();
        }

        /// <summary>
        /// Search for a string in the tree and indicate whether it was found or not.
        /// </summary>
        public bool Search(String key)
        {
            PatriciaNode resultNode = this.SearchNode(key);
            return resultNode != null ? resultNode.Terminator : false;
        }

        /// <summary>
        /// Inserts a key into the tree.
        /// </summary>
        public void Insert(String key)
        {
            if (key == null || key.Length == 0)
            {
                throw new Exception(String.Format("Invalid value for key: {0}", key));
            }

            PatriciaNode currentNode = this.Root;
            PatriciaEdge currentEdge = null;
            string partialKey = key;
            int index;
            while (partialKey != null)
            {
                if (partialKey.Length == 0)
                {
                    // The key was completely consumed at a Node. Set the node as a terminator.
                    currentNode.Terminator = true;
                    partialKey = null;
                }
                else
                {
                    index = CharacterToIndex(partialKey[0]);
                    currentEdge = currentNode.ChildEdges[index];
                    if (currentEdge == null)  // No substrings starting with this letter exit this node yet.
                    {
                        // Create a new edge from this node, assign its label, and assign the child node as a terminator.
                        PatriciaNode childNode = new PatriciaNode();
                        childNode.Terminator = true;
                        JoinNodes(currentNode, childNode, partialKey);
                        partialKey = null;
                    }
                    else  // Consume some part of the partialKey by traversing an edge.
                    {
                        int length = Math.Min(currentEdge.Label.Length, partialKey.Length);
                        int matching = 0;

                        // Count the number of matching chars at the start of partialKey and Label.
                        for (int i = 0; i < length; i++)
                        {
                            if (currentEdge.Label[i] == partialKey[i])
                            {
                                matching++;
                            }
                            else break;
                        }

                        partialKey = partialKey.Substring(matching); // Remove the consumed part of partialKey.
                        if (matching == currentEdge.Label.Length)  // We've matched the entire edge Label. Iterate.
                        {
                            currentNode = currentEdge.ChildNode;
                        }
                        else // We are stick midway through an edge's label.  Split into two edges.
                        {
                            string upperLabel = currentEdge.Label.Substring(0, matching);
                            string lowerLabel = currentEdge.Label.Substring(matching);
                            PatriciaNode splittingNode = new PatriciaNode();
                            
                            // Join upper and splittingNode. currentEdge gets implicitly removed from currentNode's ChildEdges.
                            JoinNodes(currentNode, splittingNode, upperLabel);

                            // Join child and splittingNode.
                            JoinNodes(splittingNode, currentEdge.ChildNode, lowerLabel);

                            currentNode = splittingNode;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Remove a key from the tree, if it exists.
        /// </summary>
        public void Delete(String key)
        {
            PatriciaNode toDelete = this.SearchNode(key);
            if (toDelete != null && toDelete.Terminator)
            {
                this.DeleteNode(toDelete);
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

        /*** Private Methods *************************************************/

        private void DeleteNode(PatriciaNode toDelete)
        {
            int numChildren = toDelete.NumberOfChildren;
            int index = CharacterToIndex(toDelete.ParentEdge.Label[0]);
            PatriciaNode parentNode = toDelete.ParentEdge.ParentNode;
            if (numChildren == 0)  // No children: the node is a leaf.
            {
                parentNode.ChildEdges[index] = null;
                if (!parentNode.Terminator && parentNode != this.Root && parentNode.NumberOfChildren == 1)
                {
                    PatriciaEdge childEdge = parentNode.ChildEdges.First(edge => edge != null);
                    string label = parentNode.ParentEdge.Label + childEdge.Label;
                    JoinNodes(parentNode.ParentEdge.ParentNode, childEdge.ChildNode, label);
                }
            }
            else if (numChildren == 1)
            {
                PatriciaEdge childEdge = toDelete.ChildEdges.First(edge => edge != null);
                string label = toDelete.ParentEdge.Label + childEdge.Label;
                JoinNodes(toDelete.ParentEdge.ParentNode, childEdge.ChildNode, label);
            }
            else
            {
                toDelete.Terminator = false;
            }
        }

        /// <summary>
        /// Search the tree for a node matching the given key. If a node is found, return it.
        /// Otherwise return null.
        /// </summary>
        private PatriciaNode SearchNode(string key)
        {
            if (key == null || key.Length == 0)
            {
                throw new Exception(String.Format("Invalid value for key: {0}", key));
            }
            PatriciaNode currentNode = this.Root;
            PatriciaEdge currentEdge = null;
            string partialKey = key;
            PatriciaNode result = null;

            while (partialKey != null)
            {
                if (partialKey.Length > 0)
                {
                    int index = CharacterToIndex(partialKey[0]);
                    currentEdge = currentNode.ChildEdges[index];
                }
                else // We have consumed the entire key. Check if we are looking at a Terminator node.
                {
                    result = currentNode;
                }

                // If we have consumed the key and are looking at a terminator node, then the key exists.
                if (currentEdge != null)
                {
                    // If the edge label matches part of the partialKey, then traverse that edge.
                    if (partialKey.StartsWith(currentEdge.Label))
                    {
                        partialKey = partialKey.Substring(currentEdge.Label.Length);
                        currentNode = currentEdge.ChildNode;
                        currentEdge = null;
                    }
                    // Without a match, we are stuck and the key does not exist in the tree.
                    else
                    {
                        partialKey = null;
                    }
                }
                // Without an edge to attempt to traverse, we are stuck and the key does not exist in the tree.
                else
                {
                    partialKey = null;
                }
            }

            return result;
        }

        /*** Instance Variables **********************************************/
        private PatriciaNode Root { get; set; }

        /*** Class Methods ***************************************************/
        /// <summary>
        /// Given a key, returns an offset into an Node's array of edges.
        /// </summary>
        private static int CharacterToIndex(char key)
        {
            if (key < 'a' || key > 'z')
            {
                throw new Exception(String.Format("Invalid key '{0}': must be between a and z\nValue: {1}", key, key - 'a'));
            }
            return key - 'a';
        }

        private static void JoinNodes(PatriciaNode parent, PatriciaNode child, string label)
        {
            // Assign edge values.
            PatriciaEdge edge = new PatriciaEdge();
            edge.ParentNode = parent;
            edge.ChildNode = child;
            edge.Label = label;

            // Assign parent node value.
            int index = CharacterToIndex(label[0]);
            parent.ChildEdges[index] = edge;

            // Assign child node value.
            child.ParentEdge = edge;
        }

    }

    public class PatriciaNode
    {
        /*** Public Interface ************************************************/
        public PatriciaNode()
        {
            this.ChildEdges = new PatriciaEdge[26];
        }
        /// <summary>
        /// The edge above this Node in the Tree.
        /// </summary>
        public PatriciaEdge ParentEdge { get; set; }

        /// <summary>
        /// The Edges below this Node in the tree.
        /// </summary>
        public PatriciaEdge[] ChildEdges { get; set; }

        /// <summary>
        /// Return the number of non-null ChildEdges stored at this node.
        /// </summary>
        public int NumberOfChildren { get { return ChildEdges.Count(edge => edge != null); } }

        /// <summary>
        /// Indicates whether a search that ends at this node is successful.
        /// </summary>
        public bool Terminator { get; set; }
    }

    /// <summary>
    /// An edge between two Nodes, with a string label.
    /// </summary>
    public class PatriciaEdge
    {
        public string Label { get; set; }

        /// <summary>
        /// The Node above this Edge in the tree.
        /// </summary>
        public PatriciaNode ParentNode { get; set; }

        /// <summary>
        /// The Node below this Edge in the tree.
        /// </summary>
        public PatriciaNode ChildNode { get; set; }
    }
}
