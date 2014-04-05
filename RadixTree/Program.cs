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

                StringDictionary dictionary1 = new RedBlackTree();
                StringDictionary dictionary2 = new PatriciaTree();

                Action toTime1 = () =>
                {
                    foreach (var word in words)
                    {
                        dictionary1.Insert(word);
                    }
                    StressTest(words, dictionary1);
                    foreach (var word in words)
                    {
                        dictionary1.Delete(word);
                    }
                };

                long time1 = Time(toTime1);
                dictionary1 = null;
                System.GC.Collect();

                Action toTime2 = () =>
                {
                    foreach (var word in words)
                    {
                        dictionary2.Insert(word);
                    }
                    StressTest(words, dictionary2);
                    foreach (var word in words)
                    {
                        dictionary2.Delete(word);
                    }
                };

                long time2 = Time(toTime2);
                dictionary2 = null;
                System.GC.Collect();

                Console.WriteLine(String.Format("RedBlackTree: {0}ms", time1));
                Console.WriteLine(String.Format("PatriciaTree: {0}ms", time2));
            }
            else
            {
                Console.WriteLine(String.Format("Usage: {0}. <path_to_text_file>", Process.GetCurrentProcess().ProcessName));
            }
        }

        static void StressTest(LinkedList<string> words, StringDictionary dictionary)
        {
            foreach (var word in words)
            {
                dictionary.Search(word);
                // Debug.Assert(dictionary.Search(word), String.Format("Failed to find inserted word {0}", word));
            }
            /*foreach(var word in words)
            {
                dictionary.Delete(word);
            }
             */
/*           foreach(var word in words)
            {
                Debug.Assert(!dictionary.Search(word), "Erroneously found deleted word {0}", word);
            }
 */
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

        static long Time(Action toTime)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            toTime();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
