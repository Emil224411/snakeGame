using System;
using System.IO;
using System.Threading;

namespace snakegame
{
    public class Draw
    {
        public static int maxX;
        public static int maxY;
        public static short highscore;
        static string path = @"score.txt";

        static public void at(int x, int y, string s)
        {
            Console.SetCursorPosition(x, y);
            switch (s)
            {
                case "O":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case "F":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.Write(s);
        }
        static public void del(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(" ");
        }
        public static void gameover(short score)
        {
            Console.Clear();
            Console.SetCursorPosition(Convert.ToInt32(Draw.maxX / 2), Convert.ToInt32(Draw.maxY / 2));
            Console.Write("Game Over!");
            Console.CursorTop += 1;
            Console.CursorLeft -= 10;
            if (score > highscore)
            {
                save(score);
            }
            Console.WriteLine("press any key to continue");
            Console.ReadKey(true);
            Environment.Exit(0);
        }
        public static void setup()
        {
            for (int i = 0; i < maxX; i++)
            {
                Draw.at(i, maxY, "-");
            }
            for (int i = 0; i < maxY; i++)
            {
                Draw.at(maxX, i, "|");
            }
            if (!File.Exists(path))
            {
                highscore = 0;
            }
            else
            {
                highscore = Convert.ToInt16(File.ReadAllText(path));
            }
        }
        static void save(short score)
        {
            if (!File.Exists(path))
            {
                File.CreateText(path);
            }
            File.WriteAllText(path, Convert.ToString(score));

        }

    }

    class Program
    {
        static public void Main()
        {

            Console.Clear();
            Keyboard keyboard = new();
            Draw.maxX = 20;
            Draw.maxY = 10;
            Food food = new();
            Snake snake = new(food, keyboard);

            Draw.setup();
            Thread t = new Thread(new ThreadStart(keyboard.lisen));
            t.Start();
            food.spawn(snake.posXY);

            while (true)
            {
                snake.update();
                if (snake.size > Draw.highscore)
                {
                    Draw.at(Draw.maxX + 1, 0, "score: " + Convert.ToString(snake.size));
                    Draw.at(Draw.maxX + 1, 1, "high score: " + Convert.ToString(snake.size));
                }
                else
                {
                    Draw.at(Draw.maxX + 1, 0, "score: " + Convert.ToString(snake.size));
                    Draw.at(Draw.maxX + 1, 1, "high score: " + Convert.ToString(Draw.highscore));
                }
                Thread.Sleep(350);
            }
        }

    }
    public class Keyboard
    {
        public ConsoleKeyInfo key;
        public void lisen()
        {
            while (true)
            {
                key = Console.ReadKey(true);
            }
        }
    }
    class Snake
    {
        int currentX = 1;
        int currentY = 1;
        public List<(int, int)> posXY = new List<(int, int)>();
        int speedX = 0;
        int speedY = 0;
        public short size = 1;
        string icon = "O";

        Keyboard keyboard;
        ConsoleKeyInfo lastkey;
        Food food;
        public Snake(Food fod, Keyboard keyb)
        {
            keyboard = keyb;
            food = fod;

            this.currentX = fod.randnum.Next(0, Draw.maxX);
            this.currentY = fod.randnum.Next(0, Draw.maxY);

            while (this.currentX != fod.X && this.currentY != fod.Y)
            {
                this.currentX = fod.randnum.Next(0, Draw.maxX);
                this.currentY = fod.randnum.Next(0, Draw.maxY);
            }
            this.posXY.Add((this.currentX, this.currentY));
        }

        public void update()
        {
            if (this.keyboard.key != this.lastkey)
            {
                switch (this.keyboard.key.KeyChar)
                {
                    case 'a':
                        if (this.lastkey.KeyChar != 'd' || this.size == 1)
                        {
                            this.speedX = -1;
                            this.speedY = 0;
                        }
                        break;
                    case 's':
                        if (this.lastkey.KeyChar != 'w' || this.size == 1)
                        {
                            this.speedX = 0;
                            this.speedY = 1;
                        }
                        break;
                    case 'd':
                        if (this.lastkey.KeyChar != 'a' || this.size == 1)
                        {
                            this.speedX = 1;
                            this.speedY = 0;
                        }
                        break;
                    case 'w':
                        if (this.lastkey.KeyChar != 's' || this.size == 1)
                        {
                            this.speedX = 0;
                            this.speedY = -1;
                        }
                        break;
                    default:
                        speedX = 0;
                        speedY = 0;
                        break;
                }
            }
            this.lastkey = this.keyboard.key;
            this.currentX += this.speedX;
            this.currentY += this.speedY;

            for (int i = 0; i < this.size - 1; i++)
            {

                if (this.posXY.Contains((this.currentX, this.currentY)))
                {
                    Draw.gameover(this.size);
                }
            }
            if (this.currentX == food.X && this.currentY == food.Y)
            {
                this.size += 1;
                this.posXY.Add((this.currentX, this.currentY));
                food.spawn(this.posXY);
            }

            this.posXY.Add((this.currentX, this.currentY));

            if (this.currentX < Draw.maxX && this.currentX >= 0 && this.currentY < Draw.maxY && this.currentY >= 0)
            {
                Draw.at(this.currentX, this.currentY, this.icon);
                Draw.del(this.posXY[0].Item1, this.posXY[0].Item2);
            }
            else
            {
                Draw.gameover(this.size);
            }

            this.posXY.RemoveAt(0);
        }
    }
    class Food
    {
        public Random randnum = new Random();
        public int X;
        public int Y;
        String icon = "F";

        public void spawn(List<(int, int)> posXY)
        {
            this.X = this.randnum.Next(0, Draw.maxX);
            this.Y = this.randnum.Next(0, Draw.maxY);
            while (posXY.Contains((this.X, this.Y)))
            {
                this.X = this.randnum.Next(0, Draw.maxX);
                this.Y = this.randnum.Next(0, Draw.maxY);
            }
            Draw.at(this.X, this.Y, this.icon);

        }
    }
}