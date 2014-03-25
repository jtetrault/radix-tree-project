using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadixTree
{
    class Program
    {
        static void Main(string[] args)
        {
            StringDictionary dictionary = new RedBlackTree();
            string[] testers = {"a", "b", "c", "d", "a"};

            foreach (var key in testers)
            {
                dictionary.Insert(key);
            }
            foreach (var key in testers)
            {
                Console.WriteLine(dictionary.Search(key));
            }
            Console.WriteLine(dictionary.Search("0"));
            Console.WriteLine(dictionary.Search("e"));
            Console.WriteLine(dictionary.Search("bb"));

            Console.WriteLine(dictionary.Search("b"));
            Console.WriteLine(dictionary.Search("a"));
        }
    }
}
