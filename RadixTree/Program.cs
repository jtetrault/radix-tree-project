using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RadixTreeProject.AugmentedPatricia;
using RadixTreeProject.Patricia;
using RadixTreeProject.RedBlack;

namespace RadixTreeProject
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                string fileName = args[0];

                LinkedList<string> words = new LinkedList<string>();
                try
                {
                    using (StreamReader reader = new StreamReader(fileName))
                    {
                        string line = reader.ReadLine();
                        while (line != null)
                        {
                            words.AddLast(line);
                            line = reader.ReadLine();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }

                StringDictionary redBlack = new RedBlackTree();
                StringDictionary patricia = new PatriciaTree();
                StringDictionary augmentedPatricia = new AugmentedPatriciaTree();

                long redBlackTotalTime = 0;
                long redBlackSearchTime = 0;
                long patriciaTotalTime = 0;
                long patriciaSearchTime = 0;
                long augmentedPatriciaSearchTime = 0;
                long augmentedPatriciaTotalTime = 0;

                StressTest(words, redBlack, ref redBlackTotalTime, ref redBlackSearchTime);
                redBlack = null;
                System.GC.Collect();

                StressTest(words, patricia, ref patriciaTotalTime, ref patriciaSearchTime);
                patricia = null;
                System.GC.Collect();

                StressTest(words, augmentedPatricia, ref augmentedPatriciaTotalTime, ref augmentedPatriciaSearchTime);
                augmentedPatricia = null;
                System.GC.Collect();

                Console.WriteLine("{0,-24}{1,-20}{2,-20}", "Dictionary", "Total Time", "Search Time");
                Console.WriteLine("{0,-24}{1,-20}{2,-20}", "RedBlackTree", redBlackTotalTime, redBlackSearchTime);
                Console.WriteLine("{0,-24}{1,-20}{2,-20}", "PatriciaTree", patriciaTotalTime, patriciaSearchTime);
                Console.WriteLine("{0,-24}{1,-20}{2,-20}", "AugmentedPatriciaTree", augmentedPatriciaTotalTime, augmentedPatriciaSearchTime);
            }
            else
            {
                Console.WriteLine(String.Format("Usage: {0}. <path_to_text_file>", Process.GetCurrentProcess().ProcessName));
            }
        }

        static void StressTest(LinkedList<string> words, StringDictionary dictionary, ref long totalTime, ref long searchTime)
        {
            Stopwatch totalStopwatch = new Stopwatch();
            Stopwatch searchStopwatch = new Stopwatch();

            totalStopwatch.Start();
            foreach (var word in words)
            {
                dictionary.Insert(word);
            }
            searchStopwatch.Start();
            foreach (var word in words)
            {
                dictionary.Search(word);
            }
            searchStopwatch.Stop();
            foreach(var word in words)
            {
                dictionary.Delete(word);
            }
            totalStopwatch.Stop();

            totalTime = totalStopwatch.ElapsedMilliseconds;
            searchTime = searchStopwatch.ElapsedMilliseconds;
        }

        static void RubiconTest()
        {
            StringDictionary dictionary = new AugmentedPatriciaTree();
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
