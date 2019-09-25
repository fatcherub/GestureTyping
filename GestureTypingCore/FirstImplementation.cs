using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GestureTypingCore
{
    public class FirstImplementation
    {
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

        static SortedDictionary<char, Point> KBD;
        static SortedDictionary<char, char[]> Neighbors;
        static Dictionary<int, List<string>> WordDict;
        static Dictionary<int, List<string>> PrefixDict;

        static FirstImplementation()
        {
            KBD = new SortedDictionary<char, Point>();
            Neighbors = new SortedDictionary<char, char[]>();
            WordDict = new Dictionary<int, List<string>>();
            PrefixDict = new Dictionary<int, List<string>>();

            // QWERTY
            KBD = Keyboard(new string[] {
                   "Q W E R T Y U I O P",
                   " A S D F G H J K L ",
                   "   Z X C V B N M   " });
            Neighbors = NeighboringKeys(KBD);
        }

        public static void Init()
        {
            var words = File.ReadAllLines("TWL06.txt");

            foreach (var w in words)
            {
                var length = (int)Math.Ceiling(PathLength(w));
                if (!WordDict.ContainsKey(length))
                {
                    WordDict.Add(length, new List<string>());
                    PrefixDict.Add(length, new List<string>());
                }
                WordDict[length].Add(w);
            }

            foreach (var wd in WordDict)
            {
                PrefixDict[wd.Key] = Prefixes(wd.Value);
            }
        }


        public static string[] Confusions(string input)
        {
            List<string> results = new List<string>();
            Queue<string> queue = new Queue<string>();

            var pathLength = (int)Math.Ceiling(PathLength(input));

            var word = input.ToCharArray();
            var words = WordDict[pathLength];
            var prefixes = PrefixDict[pathLength];

            queue.Enqueue("");
            Console.WriteLine("Start finding possible words...");
            while (queue.Count() > 0)
            {
                var path = queue.Dequeue();
                if (path.Length < word.Length)
                {
                    var len = Neighbors[word[path.Length]].Length;

                    for (int i = 0; i < len; i++)
                    {
                        var newpath = path + Neighbors[word[path.Length]][i];

                        if (SimilarSegments(newpath, word) &&
                            (PrefixDict.ContainsKey(pathLength) && PrefixDict[pathLength].Contains(newpath) ||
                                PrefixDict.ContainsKey(pathLength + 1) && PrefixDict[pathLength + 1].Contains(newpath) ||
                                PrefixDict.ContainsKey(pathLength - 1) && PrefixDict[pathLength - 1].Contains(newpath)))
                        {
                            queue.Enqueue(newpath);
                        }
                    }
                }
                else if (WordDict.ContainsKey(pathLength) && WordDict[pathLength].Contains(path) ||
                            WordDict.ContainsKey(pathLength + 1) && WordDict[pathLength + 1].Contains(path) ||
                            WordDict.ContainsKey(pathLength - 1) && WordDict[pathLength - 1].Contains(path))
                {
                    results.Add(path);
                }
            }

            return results.ToArray();
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
        static double SquareDistance(Point left, Point right)
        {
            return (left.X - right.X) * (left.X - right.X) + (left.Y - right.Y) * (left.Y - right.Y);
        }
        static double PathLength(string word)
        {
            double distance = 0;
            for (int i = 0; i < word.Length - 1; i++)
            {
                distance += Distance(KBD[word[i]], KBD[word[i + 1]]);
            }
            return distance;
        }
        static SortedDictionary<char, char[]> NeighboringKeys(SortedDictionary<char, Point> keyboard, double radius = 1.5d)
        {
            SortedDictionary<char, char[]> neighboringKeys = new SortedDictionary<char, char[]>();
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
                neighboringKeys.Add(key.Key, neighbors.ToCharArray());
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
        static List<string> Prefixes(List<string> words)
        {
            List<string> prefixes = new List<string>();
            foreach (var w in words)
            {
                for (int i = 1; i <= w.Length; i++)
                {
                    prefixes.Add(w.Substring(0, i));
                }
            }
            return prefixes.Distinct().ToList();
        }
        static bool SimilarSegments(string path, char[] word)
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
            var P = KBD[p2] - KBD[p1];
            var W = KBD[w2] - KBD[w1];
            return (P.X * W.X >= 0) && (P.Y * W.Y >= 0) && SquareDistance(P, W) < 4;
        }
    }
}
