using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RadixTreeProject;

namespace RadixTreeProject.AugmentedPatricia
{
    public class AugmentedPatriciaTree : StringDictionary
    {
        /*** Public Interface ************************************************/
        /// <summary>
        /// Create a new, empty Patricia Tree.
        /// </summary>
        public AugmentedPatriciaTree()
        {
            this.Root = new Node();
        }

        /// <summary>
        /// Search for a string in the tree and indicate whether it was found or not.
        /// </summary>
        public bool Search(String key)
        {
            Node resultNode = this.SearchNode(key);
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

            Node currentNode = this.Root;
            Edge currentEdge = null;
            string partialKey = key;
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

                    currentEdge = currentNode.GetChildEdgeOrNull(partialKey[0]);
                    if (currentEdge == null)  // No substrings starting with this letter exit this node yet.
                    {
                        // Create a new edge from this node, assign its label, and assign the child node as a terminator.
                        Node childNode = new Node();
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
                            Node splittingNode = new Node();
                            
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
            Node toDelete = this.SearchNode(key);
            if (toDelete != null && toDelete.Terminator)
            {
                this.DeleteNode(toDelete);
            }
            else throw new Exception(String.Format("Unable to delete key {0}", key));
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
        /// <summary>
        /// Remove the key accessed by the given node from the tree.
        /// Also perform post removal fixups, such as deleting nodes that are no longer necessary.
        /// </summary>
        private void DeleteNode(Node toDelete)
        {
            // Removes a node that falls between two other nodes, joining the parent and child directly.
            Action<Node> removeIntermediateNode = (node) =>
            {
                Edge childEdge = node.ChildEdges.First().Value;
                string label = node.ParentEdge.Label + childEdge.Label;
                JoinNodes(node.ParentNode, childEdge.ChildNode, label);
            };

            int numChildren = toDelete.NumberOfChildren;
            Node parentNode = toDelete.ParentNode;
            if (numChildren == 0)  // No children: the node is a leaf.
            {
                parentNode.ChildEdges.Remove(toDelete.ParentEdge.Label[0]);
                if (!parentNode.Terminator && parentNode != this.Root && parentNode.NumberOfChildren == 1)
                {
                    removeIntermediateNode(parentNode);
                }
            }
            else if (numChildren == 1) // Node has one child: join its parent and child directly.
            {
                removeIntermediateNode(toDelete);
            }
            else // Node has many children. Remove its Terminator flag so that it no longer represents a key.
            {
                toDelete.Terminator = false;
            }
        }

        /// <summary>
        /// Search the tree for a node matching the given key. If a node is found, return it.
        /// Otherwise return null.
        /// </summary>
        private Node SearchNode(string key)
        {
            if (key == null || key.Length == 0)
            {
                throw new Exception(String.Format("Invalid value for key: {0}", key));
            }
            Node currentNode = this.Root;
            Edge currentEdge = null;
            string partialKey = key;
            Node result = null;

            while (partialKey != null)
            {
                if (partialKey.Length > 0)
                {
                    currentEdge = currentNode.GetChildEdgeOrNull(partialKey[0]);
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
        /// <summary>
        /// The root of the Tree.
        /// </summary>
        private Node Root { get; set; }

        /*** Class Methods ***************************************************/
        /// <summary>
        /// Create a new edge joining parent and child nodes.
        /// 
        /// Place the edge in the correct array index based on the value of label.
        /// </summary>
        private static void JoinNodes(Node parent, Node child, string label)
        {
            // Assign edge values.
            Edge edge = new Edge();
            edge.ParentNode = parent;
            edge.ChildNode = child;
            edge.Label = label;

            // Assign parent node value.
            parent.ChildEdges[label[0]] = edge;

            // Assign child node value.
            child.ParentEdge = edge;
        }

    }

    class Node
    {
        /*** Public Interface ************************************************/
        public Node()
        {
            this.ChildEdges = new Dictionary<char, Edge>(17);
        }

        /// <summary>
        /// Return the child edge that matches the given key, or null if there is none.
        /// </summary>
        public Edge GetChildEdgeOrNull(char key)
        {
            return this.ChildEdges.ContainsKey(key) ? this.ChildEdges[key] : null;
        }

        /*** Instance Variables **********************************************/
        /// <summary>
        /// The edge above this Node in the Tree.
        /// </summary>
        public Edge ParentEdge { get; set; }

        /// <summary>
        /// The Edges below this Node in the tree.
        /// </summary>
        public Dictionary<char, Edge> ChildEdges { get; set; }

        /// <summary>
        /// Return the number of non-null ChildEdges stored at this node.
        /// </summary>
        public int NumberOfChildren { get { return ChildEdges.Count; } }

        /// <summary>
        /// Indicates whether a search that ends at this node is successful.
        /// </summary>
        public bool Terminator { get; set; }

        /// <summary>
        /// The node at the other end of ParentEdge.
        /// </summary>
        public Node ParentNode
        {
            get { return ParentEdge.ParentNode; }
            set { ParentEdge.ParentNode = value; }
        }
    }

    /// <summary>
    /// An edge between two Nodes, with a string label.
    /// </summary>
    class Edge
    {
        /*** Instance Variables **********************************************/
        public string Label { get; set; }

        /// <summary>
        /// The Node above this Edge in the tree.
        /// </summary>
        public Node ParentNode { get; set; }

        /// <summary>
        /// The Node below this Edge in the tree.
        /// </summary>
        public Node ChildNode { get; set; }
    }
}
