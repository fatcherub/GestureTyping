using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GestureTypingCore
{
    public class SecondImplementation
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
        static SortedDictionary<char, SortedDictionary<int, List<string>>> WordDict;
        static SortedDictionary<char, SortedDictionary<int, List<string>>> PrefixDict;

        static SecondImplementation()
        {
            KBD = new SortedDictionary<char, Point>();
            Neighbors = new SortedDictionary<char, char[]>();
            WordDict = new SortedDictionary<char, SortedDictionary<int, List<string>>>();
            PrefixDict = new SortedDictionary<char, SortedDictionary<int, List<string>>>();

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

            foreach (var k in KBD)
            {
                WordDict.Add(k.Key, new SortedDictionary<int, List<string>>());
                PrefixDict.Add(k.Key, new SortedDictionary<int, List<string>>());
            }

            foreach (var w in words)
            {
                var length = (int)Math.Ceiling(PathLength(w));
                var initial = w[0];

                if (!WordDict[initial].ContainsKey(length))
                {
                    WordDict[initial].Add(length, new List<string>());
                    PrefixDict[initial].Add(length, new List<string>());
                }
                WordDict[initial][length].Add(w);
            }

            foreach (var wd in WordDict)
            {
                foreach (var dict in wd.Value)
                {
                    PrefixDict[wd.Key][dict.Key] = Prefixes(dict.Value);
                }
            }
        }

        public static string[] SameLengthConfusions(string input)
        {
            List<string> results = new List<string>();
            Queue<string> queue = new Queue<string>();

            var pathLength = (int)Math.Ceiling(PathLength(input));

            var word = input.ToCharArray();
            var initial = word[0];
            var prefixDict = PrefixDict[initial];
            var wordDict = WordDict[initial];

            int times = 0;
            long timestamp = DateTime.UtcNow.Ticks;
            long smallestTick = long.MaxValue;
            long largestTick = 0;

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
                            (prefixDict.ContainsKey(pathLength) && prefixDict[pathLength].Contains(newpath) ||
                                prefixDict.ContainsKey(pathLength + 1) && prefixDict[pathLength + 1].Contains(newpath) ||
                                prefixDict.ContainsKey(pathLength - 1) && prefixDict[pathLength - 1].Contains(newpath)))
                        {
                            queue.Enqueue(newpath);
                        }
                    }
                }
                else if (wordDict.ContainsKey(pathLength) && wordDict[pathLength].Contains(path) ||
                            wordDict.ContainsKey(pathLength + 1) && wordDict[pathLength + 1].Contains(path) ||
                            wordDict.ContainsKey(pathLength - 1) && wordDict[pathLength - 1].Contains(path))
                {
                    results.Add(path);
                }

                var diff = DateTime.UtcNow.Ticks - timestamp;
                smallestTick = diff < smallestTick ? diff : smallestTick;
                largestTick = diff > largestTick ? diff : largestTick;
                timestamp = DateTime.UtcNow.Ticks;
                times++;
            }

            Console.WriteLine($"Loop Count:      {times}");
            Console.WriteLine($"Smallest period: {smallestTick}");
            Console.WriteLine($"Largest period:  {largestTick}");

            return results.ToArray();
        }

        public static string[] Confusions(string input, int bias = 3)
        {
            List<string> results = new List<string>();
            Queue<string> queue = new Queue<string>();

            var pathLength = (int)Math.Ceiling(PathLength(input));

            var word = input.ToCharArray();
            var initial = word[0];
            var prefixDict = PrefixDict[initial];
            var wordDict = WordDict[initial];

            int times = 0;
            long timestamp = DateTime.UtcNow.Ticks;
            long smallestTick = long.MaxValue;
            long largestTick = 0;

            foreach (var initNabor in Neighbors[word[0]])
            {
                queue.Enqueue(initNabor.ToString());
            }

            Console.WriteLine("Start finding possible words...");
            while (queue.Count() > 0)
            {
                var path = queue.Dequeue();
                if (path.Length < word.Length)
                {
                    var currentPathChar = path.Last();

                    for (int k = path.Length - 1; k < word.Length - 1; k++)
                    {
                        var currentWordChar = word[k];

                        for (int i = k + 1; i < word.Length; i++)
                        {
                            var nextWordChar = word[i];
                            var nextWordNeighbors = Neighbors[nextWordChar];

                            for (int j = 0; j < nextWordNeighbors.Length; j++)
                            {
                                var p2 = nextWordNeighbors[j];

                                var newpath = path + p2;

                                if (!queue.Contains(newpath) && SimilarSegments(currentPathChar, p2, currentWordChar, nextWordChar) &&
                                       (prefixDict.ContainsKey(pathLength) && prefixDict[pathLength].Contains(newpath) ||
                                        prefixDict.ContainsKey(pathLength + 1) && prefixDict[pathLength + 1].Contains(newpath) ||
                                        prefixDict.ContainsKey(pathLength - 1) && prefixDict[pathLength - 1].Contains(newpath)))
                                {
                                    queue.Enqueue(newpath);
                                }
                            }
                        }
                    }
                }
                if (Math.Abs((int)Math.Ceiling(PathLength(path)) - pathLength) < bias)
                {
                    if (wordDict.ContainsKey(pathLength) && wordDict[pathLength].Contains(path) ||
                        wordDict.ContainsKey(pathLength + 1) && wordDict[pathLength + 1].Contains(path) ||
                        wordDict.ContainsKey(pathLength - 1) && wordDict[pathLength - 1].Contains(path))
                    {
                        results.Add(path);
                    }
                }

                var diff = DateTime.UtcNow.Ticks - timestamp;
                smallestTick = diff < smallestTick ? diff : smallestTick;
                largestTick = diff > largestTick ? diff : largestTick;
                timestamp = DateTime.UtcNow.Ticks;
                times++;
            }

            Console.WriteLine($"Loop Count:      {times}");
            Console.WriteLine($"Smallest period: {smallestTick}");
            Console.WriteLine($"Largest period:  {largestTick}");

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

        static bool SimilarSegments(char p1, char p2, char w1, char w2)
        {
            var P = KBD[p2] - KBD[p1];
            var W = KBD[w2] - KBD[w1];
            return (P.X * W.X >= 0) && (P.Y * W.Y >= 0) && SquareDistance(P, W) < 2;
        }
    }
}
