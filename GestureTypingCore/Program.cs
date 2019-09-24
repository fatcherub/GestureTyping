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
            var longestWords = words.Select(w => new { Word = w, Length = PathLength(w, qwerty) }).OrderByDescending(w => w.Length).Take(10);
            foreach (var w in longestWords)
            {
                Console.WriteLine($"{w.Length.ToString("F1")}\t{w.Word}");
            }
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
    }
}
