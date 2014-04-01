using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RadixTreeProject.Patricia;
using RadixTreeProject.RedBlack;

namespace RadixTreeProject
{
    class Program
    {
        static void Main(string[] args)
        {
            StressTest("..\\..\\us.dic");
        }

        static void StressTest(string fileName)
        {
            StringDictionary dictionary = new PatriciaTree();
            LinkedList<string> words = new LinkedList<string>();
            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        words.AddLast(line);
                        dictionary.Insert(line);
                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            string prevWord = "huehue";
            foreach (var word in words)
            {
                // Console.WriteLine(String.Format("{0}: {1}", word, dictionary.Search(word)));
                Debug.Assert(dictionary.Search(word), String.Format("Failed to find inserted word {0}", word));
                // Console.WriteLine(String.Format("{0}: {1}", word + prevWord, dictionary.Search(word + prevWord)));
                Debug.Assert(!dictionary.Search(word + prevWord), String.Format("Erroneously found non-inserted word {0}", word + prevWord));
                prevWord = word.ToUpper();
            }
            foreach(var word in words)
            {
                dictionary.Delete(word);
            }
            foreach(var word in words)
            {
                // Console.WriteLine(String.Format("{0}: {1}", word, dictionary.Search(word)));
                Debug.Assert(!dictionary.Search(word), "Erroneously found deleted word {0}", word);
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
