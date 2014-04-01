using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadixTree
{
    class Program
    {
        static void Main(string[] args)
        {
            RubiconTest();
        }

        static void StressTest(string fileName)
        {
            StringDictionary dictionary = new PatriciaTree();
            LinkedList<string> words = new LinkedList<string>();
            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        string line = reader.ReadLine();
                        words.AddLast(line);
                        dictionary.Insert(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            string prevWord = "";
            foreach (var word in words)
            {
                Console.WriteLine(String.Format("{0}: {1}", word, dictionary.Search(word)));
                Console.WriteLine(String.Format("{0}: {1}", word + prevWord, dictionary.Search(word + prevWord)));
                prevWord = word;
            }
        }

        static void RubiconTest()
        {
            StringDictionary dictionary = new PatriciaTree();
            string[] testers = { "romane", "romanus", "romulus", "rubens", "ruber", "rubicon", "rubicundus" };
            string[] halfers = { "romane", "romulus", "ruber", "rubicundus" };

            foreach(var word in testers)
            {
                dictionary.Insert(word);
            }
            foreach(var word in halfers)
            {
                dictionary.Delete(word);
            }
            foreach (var word in halfers)
            {
                dictionary.Delete(word);
            }
            foreach(var word in testers)
            {
                Console.WriteLine(dictionary.Search(word));
            }
        }
    }
}
