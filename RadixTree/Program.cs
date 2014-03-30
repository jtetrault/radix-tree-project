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
            StringDictionary dictionary = new PatriciaTree();
            string[] inserted = {"romane", "romanus", "romulus", "rubens"};
            string[] all = {"romane", "romanus", "romulus", "rubens", "ruber", "rubicon", "rubicundus" };
            foreach (var key in inserted)
            {
                dictionary.Insert(key);
            }
            foreach(var key in all)
            {
                Console.WriteLine(dictionary.Search(key));
            }

        }
    }
}
