using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GestureTypingCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Gesture Typing!");

            #region test path length function
            //var W = qwerty['W'];
            //var O = qwerty['O'];
            //var R = qwerty['R'];
            //var D = qwerty['D'];
            //var dis = Distance(W, O) + Distance(O, R) + Distance(R, D);
            //Console.WriteLine(dis);

            //Console.WriteLine(PathLength("WORD", qwerty));
            //Console.WriteLine(PathLength("W", qwerty));
            //Console.WriteLine(PathLength("TYPEWRITER", qwerty) == (1 + 4 + 7 + 1 + 2 + 4 + 3 + 2 + 1));
            #endregion

            #region get longest
            //var maxWord = words.Max(w => PathLength(w, qwerty));

            //var longestWords = words.Select(w => new { Word = w, Length = PathLength(w, qwerty) }).OrderByDescending(w => w.Length).Take(10);
            //foreach (var w in longestWords)
            //{
            //    Console.WriteLine($"{w.Length.ToString("F1")}\t{w.Word}");
            //}
            #endregion

            #region neighbors
            //foreach (var key in qwerty_neighbors)
            //{
            //    Console.WriteLine($"{key.Key}, { key.Value}");
            //}
            #endregion

            #region prefix
            //var prefixes1 = Prefixes(new string[] { "THESE", "THEY", "THOSE" });
            //Console.WriteLine(prefixes.Length);
            #endregion

            #region confusion version 1
            //Console.WriteLine("##### Confusion ver 1 #####");
            //Console.WriteLine(DateTime.Now.Ticks);
            //var confusions = Confusions("SOMETHING", words, prefixes, qwerty_neighbors);
            //foreach (var c in confusions)
            //{
            //    Console.WriteLine(c);
            //}
            //Console.WriteLine(DateTime.Now.Ticks);
            #endregion

            #region calculate direction
            //var JE = qwerty['E'] - qwerty['J'];
            //var BR = qwerty['R'] - qwerty['B'];
            //var dir = Distance(JE, BR);
            //Console.WriteLine(dir);

            //Console.WriteLine(SimilarSegments(qwerty, "BR", "JELLO"));
            //Console.WriteLine(SimilarSegments(qwerty, "HE", "JELLO"));
            //Console.WriteLine(SimilarSegments(qwerty, "HEP", "JELLO"));
            //Console.WriteLine(SimilarSegments(qwerty, "J", "JELLO"));
            #endregion

            #region confusion version 2
            //Console.WriteLine("##### Confusion ver 2 #####");
            //var tick1 = DateTime.Now.Ticks;
            //Console.WriteLine(tick1);
            //var confusions = Confusions2("CONFUSION", words, prefixes, qwerty_neighbors, qwerty);
            //foreach (var c in confusions)
            //{
            //    Console.WriteLine(c);
            //}
            //var tick2 = DateTime.Now.Ticks;
            //Console.WriteLine(tick2);
            //Console.WriteLine(tick2 - tick1);
            #endregion

            FirstImplementation.Init();
            SecondImplementation.Init();
            RankedImplementation.Init();

            var word = "efvfertyuiokngre";

            Console.WriteLine("\r\n##### 2nd implementation #####");

            var tick1 = DateTime.Now.Ticks;
            Console.WriteLine(tick1);

            var confusions = SecondImplementation.Confusions(word.ToUpper());
            foreach (var c in confusions)
            {
                Console.WriteLine(c);
            }

            var tick2 = DateTime.Now.Ticks;
            Console.WriteLine(tick2);
            Console.WriteLine(tick2 - tick1);
            double seconds = (double)(tick2 - tick1) / (double)TimeSpan.TicksPerSecond;
            Console.WriteLine(seconds);

            Console.WriteLine("");
            Console.WriteLine("\r\n##### 2nd implementation #####");

            tick1 = DateTime.Now.Ticks;
            Console.WriteLine(tick1);

            var rankedConfusions = RankedImplementation.Confusions(word.ToLower());
            var finalResult = rankedConfusions.OrderByDescending(r => r.Item2).Take(5);
            foreach (var c in finalResult)
            {
                Console.WriteLine($"{c.Item1}\t{c.Item2}");
            }

            tick2 = DateTime.Now.Ticks;
            Console.WriteLine(tick2);
            Console.WriteLine(tick2 - tick1);
            seconds = (double)(tick2 - tick1) / (double)TimeSpan.TicksPerSecond;
            Console.WriteLine(seconds);

            Console.ReadKey();
        }

    }


}
