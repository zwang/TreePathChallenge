using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreePathChallenge
{
    #region Support Classes and Comparer
    /// <summary>
    /// Regular TreeNode, not necessary a Binary tree
    /// </summary>
    /// <typeparam name="T">Type of the value stored in a node of tree</typeparam>
    /// <remarks>Use non-generic also works, in this challenge, T will be string</remarks>
    public class TreeNode<T>
    {
        public TreeNode(T value, TreeNode<T> parent)
        {
            this.Value = value;
            this.ChildNodes = new List<TreeNode<T>>();

            //Build the Path Structure
            StringBuilder sb = new StringBuilder("");
            if (parent != null)
            {
                sb.Append(parent.PathStructure);
            }
            sb.Append("/");
            sb.Append(value);
            this.PathStructure = sb.ToString();
        }
        public T Value { get; set; }

        public string PathStructure;

        /// <summary>
        /// List of Child Nodes, if we are sure about the number of Child Nodes
        /// We can have multiple properties for this 
        /// like LeftChildNode and RightChildNode for binary tree
        /// </summary>
        public List<TreeNode<T>> ChildNodes { get; set; }
    }

    /// <summary>
    /// To temporary hold node value and level
    /// </summary>
    public class TreeNodeValue
    {
        public int Level { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Level, Value);
        }
    }

    /// <summary>
    /// Customized Equality Comparer for A list of String for used in Linq Query
    /// </summary>
    public class ListOfStringComparer : IEqualityComparer<List<string>>
    {
        public bool Equals(List<string> list1, List<string> list2)
        {
            list1.Sort();
            list2.Sort();
            if (list1 == null || list2 == null)
            {
                return list1 == list2;
            }
            if (list1.Count == list2.Count)
            {
                for (var i = 0; i < list1.Count; i++)
                {
                    if (list1[i] != list2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public int GetHashCode(List<string> list)
        {
            StringBuilder sb = new StringBuilder("");
            foreach (var s in list)
            {
                sb.Append(s);
            }
            return sb.ToString().GetHashCode();
        }
    }
    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            //Test Part1: Insert Into a Tree
            //var root = new TreeNode<string>("RootTest", null);
            //InsertIntoATree(root, @"/home/sports/basketball/ncaa/");
            //InsertIntoATree(root, @"");

            //Test Combination
            //var combinations = GetCombinations("hello|rap|music|pop".Split('|').Where(s => s.Length > 0).ToList());
            //foreach (var c in combinations)
            //{
            //    Console.WriteLine(c);
            //}

            //Test GetNodesOfCombination
            //var nodes = GetNodesOfCombination("/home/sports|music/misc|favorites");
            //foreach (var n in nodes)
            //{
            //    Console.WriteLine(n.ToString());
            //}

            //Test GetSubNodesPath
            //string result = GetSubNodesPath(new List<string> { "music", "pop", "music-pop" });
            //Console.WriteLine(result);

            TestPart1();
            TestPart2();
            TestPart3();
            TestPart4();
            TestPart5();
            TestPart6();

            Console.WriteLine("Done. Press any key to continue...");
            Console.ReadKey();
        }

        #region Solutions

        #region Alternative Solution Methods with different interface for Part 1 - 3
        /// <summary>
        /// This function support Part1(Regular one node insert) And Part2 (Dual Leaf Node Insert)
        /// </summary>
        /// <param name="rootNode">the root Node of the tree</param>
        /// <param name="pathStructure">The path Structure</param>
        /// <returns>true if inserted successfully, otherwise, false</returns>
        static bool InsertIntoATree(TreeNode<string> rootNode, string pathStructure)
        {
            string lastString;
            TreeNode<string> traversalNode = GetParentNodeToInsert(rootNode, pathStructure, out lastString);
            if (traversalNode != null && lastString.Length > 0)
            {
                foreach (var value in lastString.Split('|').Where(s => s.Length > 0))
                {
                    traversalNode.ChildNodes.Add(new TreeNode<string>(value, traversalNode));
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// This function can work for Part 3
        /// </summary>
        /// <param name="rootNode">root node of the tree</param>
        /// <param name="pathStructure">path structure</param>
        /// <returns>true if inserted successfully</returns>
        static bool InsertIntoATreeCombination(TreeNode<string> rootNode, string pathStructure)
        {
            string lastString;
            TreeNode<string> traversalNode = GetParentNodeToInsert(rootNode, pathStructure, out lastString);
            if (traversalNode != null && lastString.Length > 0)
            {
                //Can get the combinationOfValues in one step, this is just make it easy to read.
                var values = lastString.Split('|').Where(s => s.Length > 0).ToList();
                var combinationOfValues = GetCombinations(values);
                foreach (var value in combinationOfValues)
                {
                    traversalNode.ChildNodes.Add(new TreeNode<string>(value, traversalNode));
                }
                return true;
            }
            return false;
        } 
        #endregion

        /// <summary>
        /// This function will work for Part 1, 2, 3, 4
        /// </summary>
        /// <param name="pathStructure">The pathStructure to build the tree</param>
        /// <returns>The root node of the tree</returns>
        static TreeNode<string> InsertIntoATreeSupportMultipleLevelCombination(string pathStructure)
        {
            List<TreeNodeValue> nodeValues = GetNodesOfCombination(pathStructure);
            if (nodeValues.Count == 0)
            {
                return null;
            }
            Queue<TreeNode<string>> queue = new Queue<TreeNode<string>>();
            TreeNode<string> flagForLevel = new TreeNode<string>("", null);
            TreeNode<string> root = new TreeNode<string>(nodeValues.Single(s => s.Level == 0).Value, null);
            queue.Enqueue(root);
            queue.Enqueue(flagForLevel);
            int maxLevel = nodeValues.Select(s => s.Level).Max();
            int nextLevel = 1;
            while (queue.Count > 0 && nextLevel <= maxLevel)
            {
                TreeNode<string> n = queue.Dequeue();
                if (n == flagForLevel)
                {
                    nextLevel++;
                    if (nextLevel <= maxLevel)
                    {
                        queue.Enqueue(flagForLevel);
                    }
                }
                else
                {
                    foreach (var value in nodeValues.Where(v => v.Level == nextLevel))
                    {
                        var newNode = new TreeNode<string>(value.Value, n);
                        n.ChildNodes.Add(newNode);
                        queue.Enqueue(newNode);
                    }
                }
            }
            return root;
        }

        /// <summary>
        /// Write a function that takes as input a Tree and outputs a combinatorial tree. For example, 
        /// in the Tree displayed in Part 4, the output of the function would be:/home/sports|music/misc|favorites
        /// </summary>
        /// <param name="rootNode">Root node</param>
        /// <returns>the Path Structure</returns>
        static string CollapseCombinationalTreeToPathStructure(TreeNode<string> rootNode)
        {
            StringBuilder sb = new StringBuilder("/");
            List<TreeNode<string>> tempListForSubNodes = new List<TreeNode<string>>();
            if (rootNode != null)
            {
                sb.Append(rootNode.Value);
                sb.Append("/");
                tempListForSubNodes = rootNode.ChildNodes;
                while (tempListForSubNodes != null && tempListForSubNodes.Count > 0)
                {
                    string subNodePath = GetSubNodesPath(tempListForSubNodes.Select(t => t.Value).ToList());
                    sb.Append(subNodePath);
                    sb.Append("/");
                    var copyOfTempList = new List<TreeNode<string>>(tempListForSubNodes);
                    tempListForSubNodes.Clear();
                    tempListForSubNodes.AddRange(copyOfTempList.SelectMany(t => t.ChildNodes));
                }
            }
            return sb.ToString(0, sb.Length - 1); //Remove the ending "/"
        }

        /// <summary>
        /// Find similar node of the origina node in the tree.
        /// </summary>
        /// <param name="root">root node of the tree</param>
        /// <param name="original">node to search</param>
        /// <returns>path structure, "/" if not found</returns>
        static string GetSynonymOfTreeNode(TreeNode<string> root, TreeNode<string> original)
        {
            if (root == null || original == null)
            {
                return "/";
            }
            TreeNode<string> result = new TreeNode<string>("", null);
            Queue<TreeNode<string>> queue = new Queue<TreeNode<string>>();
            queue.Enqueue(root);
            ListOfStringComparer comparer = new ListOfStringComparer();
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node.Value != original.Value 
                    && AreTwoListEqual(node.ChildNodes.Select(s=>s.Value).ToList(), 
                                        original.ChildNodes.Select(s=>s.Value).ToList()
                                    )
                    )
                {
                    result = node;
                    break;
                }
                foreach (var n in node.ChildNodes)
                {
                    queue.Enqueue(n);
                }
            }
            return result.PathStructure;
        }

        #endregion

        #region Support functions
        /// <summary>
        /// Get a combination of strings based on a list of string
        /// </summary>
        /// <param name="listOfValues">List of string</param>
        /// <returns>A list of combinational strings based on the original strings</returns>
        private static IEnumerable<string> GetCombinations(List<string> listOfValues)
        {
            listOfValues.Sort();
            var baseList = listOfValues.Select(s => new List<string> { s }).ToList();
            int level = 2;
            while (level <= listOfValues.Count())
            {
                foreach (var s in listOfValues)
                {
                    foreach (var list in baseList.Where(l => l.Count == level - 1 && !l.Contains(s)).ToList())
                    {
                        var newList = new List<string>(list);
                        newList.Add(s);
                        newList.Sort();
                        if (!baseList.Contains(newList, new ListOfStringComparer()))
                        {
                            baseList.Add(newList);
                        }
                    }
                }
                level++;
            }
            return baseList.Select(l => ConvertToString(l)); ;
        }

        /// <summary>
        /// Convert a list to string
        /// </summary>
        /// <param name="list">List of string</param>
        /// <returns>A string concatenated with "-"</returns>
        private static string ConvertToString(List<string> list)
        {
            StringBuilder sb = new StringBuilder("");
            foreach (var s in list)
            {
                sb.Append("-");
                sb.Append(s);
            }
            return sb.ToString().Substring(1);
        }

        private static string GetSubNodesPath(List<string> list)
        {
            StringBuilder sb = new StringBuilder("");
            var values = list.SelectMany(t => t.Split('-')).Distinct();
            foreach (var s in values)
            {
                sb.Append("|");
                sb.Append(s);
            }
            return sb.ToString().Substring(1);
        }

        /// <summary>
        /// Get the parent node, which is the to-be parent node of the new nodes
        /// </summary>
        /// <param name="rootNode">the root Node of the tree</param>
        /// <param name="pathStructure">The path Structure</param>
        /// <param name="lastString">get the last string in the path structure</param>
        /// <returns>A node which is the parent node of the new nodes, null if not found</returns>
        private static TreeNode<string> GetParentNodeToInsert(TreeNode<string> rootNode, string pathStructure, out string lastString)
        {
            lastString = "";
            //Make sure pathStructure starts with "/" and not end with "/"
            if (!pathStructure.StartsWith("/") || pathStructure.EndsWith("/"))
            {
                Console.WriteLine("Invalid Path Structure");
                return null;
            }
            var nodeNames = pathStructure.Split('/').Where(n => n.Length > 0).ToList();
            lastString = nodeNames.Last();
            if (rootNode == null || rootNode.Value != nodeNames.First())
            {
                return null;
            }
            var traversalNode = rootNode;
            //Traversal from the second node and until the last parent node, which is where to insert the new leaf node
            foreach (var nodeName in nodeNames.Skip(1).Take(nodeNames.Count - 2))
            {
                Console.WriteLine(nodeName);
                //Use FirstOrDefault to make sure app not error out 
                //if there is multiple child nodes with the same name or no child node with this name
                var tempNode = traversalNode.ChildNodes.FirstOrDefault(s => s.Value == nodeName);
                if (tempNode != null)
                {
                    traversalNode = tempNode;
                }
                else
                {
                    return null;
                }
            }
            return traversalNode;
        }

        private static List<TreeNodeValue> GetNodesOfCombination(string pathStructure)
        {
            List<TreeNodeValue> results = new List<TreeNodeValue>();
            //Make sure pathStructure starts with "/" and not end with "/"
            if (!pathStructure.StartsWith("/") || pathStructure.EndsWith("/"))
            {
                Console.WriteLine(@"Invalid Path Structure. The pathStructure must start with '/' and not end with '/'");
                return results;
            }
            var nodeNames = pathStructure.Split('/').Where(n => n.Length > 0);
            if (nodeNames.First().Contains('|'))
            {
                Console.WriteLine("Invalid Path Structure. The root node can not be a combination.");
                return results;
            }
            results = nodeNames.SelectMany((s, i) =>
                                                GetCombinations(s.Split('|')
                                                                .Where(s2 => s.Length > 0)
                                                                .ToList()
                                                            ).Select(s3 =>
                                                                new TreeNodeValue
                                                                {
                                                                    Value = s3,
                                                                    Level = i
                                                                }
                                                            )
                                        ).ToList();
            return results;
        }

        /// <summary>
        /// Check if two list of strings are equal in the content
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns>true if equal, false if not</returns>
        static bool AreTwoListEqual(List<string> list1, List<string> list2)
        {
            list1.Sort();
            list2.Sort();
            if (list1 == null || list2 == null)
            {
                return list1 == list2;
            }
            if (list1.Count == list2.Count)
            {
                for (var i = 0; i < list1.Count; i++)
                {
                    if (list1[i] != list2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        #endregion

        #region Test Functions

        static void TestPart1()
        {
            var rootNode = InsertIntoATreeSupportMultipleLevelCombination("/home/sports/basketball/ncaa");
            var result = CollapseCombinationalTreeToPathStructure(rootNode);
            Console.WriteLine(result);
        }

        static void TestPart2()
        {
            var rootNode = InsertIntoATreeSupportMultipleLevelCombination("/home/sports/football/NFL|NCAA");
            var result = CollapseCombinationalTreeToPathStructure(rootNode);
            Console.WriteLine(result);
        }

        static void TestPart3()
        {
            var rootNode = InsertIntoATreeSupportMultipleLevelCombination("/home/music/rap|rock|pop");
            var result = CollapseCombinationalTreeToPathStructure(rootNode);
            Console.WriteLine(result);
        }

        static void TestPart4()
        {
            var rootNode = InsertIntoATreeSupportMultipleLevelCombination("/home/sports|music/misc|favorites");
            var result = CollapseCombinationalTreeToPathStructure(rootNode);
            Console.WriteLine(result);
        }

        static void TestPart5()
        {
            var root = new TreeNode<string>("home", null);
            var level2Nodes = new List<TreeNode<string>>
            {
                new TreeNode<string>("music", root),
                new TreeNode<string>("sports", root),
                new TreeNode<string>("music-sports", root),
            };
            root.ChildNodes = level2Nodes;
            foreach (var n in level2Nodes)
            {
                var level3Nodes = new List<TreeNode<string>>
                {
                    new TreeNode<string>("pop", n),
                    new TreeNode<string>("rock", n),
                    new TreeNode<string>("pop-rock", n),
                };
                n.ChildNodes = level3Nodes;
            }
            string result = CollapseCombinationalTreeToPathStructure(root);
            Console.WriteLine(result);
        }

        static void TestPart6()
        {
            var root = new TreeNode<string>("home", null);
            var level2Nodes = new List<TreeNode<string>>
            {
                new TreeNode<string>("music", root),
                new TreeNode<string>("sports", root),
                new TreeNode<string>("music-sports", root),
            };
            root.ChildNodes = level2Nodes;
            foreach (var n in level2Nodes)
            {
                var level3Nodes = new List<TreeNode<string>>
                {
                    new TreeNode<string>("pop", n),
                    new TreeNode<string>("rock", n),
                    new TreeNode<string>("pop-rock", n),
                };
                n.ChildNodes = level3Nodes;
            }
            string result = GetSynonymOfTreeNode(root, root.ChildNodes.Single(s => s.Value == "music"));
            Console.WriteLine(result);
        }




        #endregion
    }
}
