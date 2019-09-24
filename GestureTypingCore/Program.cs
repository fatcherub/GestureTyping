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
            Console.WriteLine("Hello World!");

            var words = File.ReadAllLines("TWL06.txt");
            Console.WriteLine(words.Length);

            var qwerty = Keyboard(new string[] {
                   "Q W E R T Y U I O P",
                   " A S D F G H J K L ",
                   "   Z X C V B N M   " });
            var qwerty_neighbors = NeighboringKeys(qwerty);
            var prefixes = Prefixes(words);

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
            Console.WriteLine("##### Confusion ver 2 #####");
            var tick1 = DateTime.Now.Ticks;
            Console.WriteLine(tick1);
            var confusions = Confusions2("CONFUSION", words, prefixes, qwerty_neighbors, qwerty);
            foreach (var c in confusions)
            {
                Console.WriteLine(c);
            }
            var tick2 = DateTime.Now.Ticks;
            Console.WriteLine(tick2);
            Console.WriteLine(tick2 - tick1);
            #endregion
            Console.ReadKey();
        }

        static SortedDictionary<char, Point> Keyboard(string[] keyboard)
        {
            SortedDictionary<char, Point> dict = new SortedDictionary<char, Point>();
            for (int y = 0; y < keyboard.Length; y++)
            {
                for (int x = 0; x < keyboard[y].Length; x++)
                {
                    var key = keyboard[y][x];
                    if (key != ' ')
                        dict.Add(key, new Point(x / 2d, y));
                }
            }
            return dict;
        }
        static double Distance(Point left, Point right)
        {
            return Math.Sqrt((left.X - right.X) * (left.X - right.X) + (left.Y - right.Y) * (left.Y - right.Y));
        }
        static double PathLength(string word, SortedDictionary<char, Point> keyboard)
        {
            double distance = 0;
            for (int i = 0; i < word.Length - 1; i++)
            {
                distance += Distance(keyboard[word[i]], keyboard[word[i + 1]]);
            }
            return distance;
        }
        static SortedDictionary<char, string> NeighboringKeys(SortedDictionary<char, Point> keyboard, double radius = 1.5d)
        {
            SortedDictionary<char, string> neighboringKeys = new SortedDictionary<char, string>();
            foreach (var key in keyboard)
            {
                string neighbors = "";
                foreach (var n in keyboard)
                {
                    if (Distance(key.Value, n.Value) < radius)
                    {
                        neighbors += n.Key;
                    }
                }
                neighboringKeys.Add(key.Key, neighbors);
            }

            return neighboringKeys;
        }
        static string[] Prefixes(string[] words)
        {
            List<string> prefixes = new List<string>();
            foreach (var w in words)
            {
                for (int i = 1; i <= w.Length; i++)
                {
                    prefixes.Add(w.Substring(0, i));
                }
            }
            return prefixes.Distinct().ToArray();
        }
        static string[] Confusions(string word, string[] words, string[] prefixes, SortedDictionary<char, string> neighbors)
        {
            List<string> results = new List<string>();
            Queue<string> queue = new Queue<string>();
            queue.Enqueue("");
            Console.WriteLine("Start finding possible words...");
            while (queue.Count() > 0)
            {
                var path = queue.Dequeue();
                if (path.Length < word.Length)
                {
                    foreach (var l in neighbors[word[path.Length]])
                    {
                        if (prefixes.Contains(path + l))
                        {
                            queue.Enqueue(path + l);
                        }
                    }
                }
                else if (words.Contains(path))
                {
                    results.Add(path);
                }
            }

            return results.ToArray();
        }

        static string[] Confusions2(string word, string[] words, string[] prefixes,
            SortedDictionary<char, string> neighbors, SortedDictionary<char, Point> keyboard)
        {
            List<string> results = new List<string>();
            Queue<string> queue = new Queue<string>();
            queue.Enqueue("");
            Console.WriteLine("Start finding possible words...");
            while (queue.Count() > 0)
            {
                var path = queue.Dequeue();
                if (path.Length < word.Length)
                {
                    foreach (var l in neighbors[word[path.Length]])
                    {
                        var newpath = path + l;

                        if (SimilarSegments(keyboard, newpath, word) && prefixes.Contains(newpath))
                        {
                            queue.Enqueue(path + l);
                        }
                    }
                }
                else if (words.Contains(path))
                {
                    results.Add(path);
                }
            }

            return results.ToArray();
        }
        static bool SimilarSegments(SortedDictionary<char, Point> keyboard, string path, string word)
        {
            var n = path.Length;
            if (n < 2)
            {
                return true;
            }
            var p1 = path[n - 2];
            var p2 = path[n - 1];
            var w1 = word[n - 2];
            var w2 = word[n - 1];
            var P = keyboard[p2] - keyboard[p1];
            var W = keyboard[w2] - keyboard[w1];
            return (P.X * W.X >= 0) && (P.Y * W.Y >= 0) && Distance(P, W) < 2;
        }
    }

    struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }
    }
}
