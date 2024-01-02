using System.Drawing;
using System.Runtime.CompilerServices;

namespace CMD_Apple
{
    internal class Program
    {

        public static bool[][] prevImg = new bool[240][];

        static void Main(string[] args)
        {
            bool[][][] arr = new bool[6573][][];
            for (int i = 0; i < 6573; i++)
            {
                Bitmap img = new Bitmap("F:\\Projects\\CMD_Apple\\res\\frame-" + (i+1) + ".png");
                img = new Bitmap(img, new Size(img.Width / 2, img.Height / 4));
                arr[i] = new bool[90][];
                for (int y = 0; y < img.Height; y++)
                {
                    arr[i][y] = new bool[240];
                    for (int x = 0; x < img.Width; x++)
                    {
                        arr[i][y][x] = (int)MathF.Round(img.GetPixel(x, y).GetBrightness()) != 0;
                    }
                }

            }

            for (int i = 0; i < 240; i++) {
                prevImg[i] = new bool[90];
            }

            Console.SetWindowSize(280, 120);
            for (int i = 1; i < 6574; i++)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                imgToAscii(arr[i-1]);
                watch.Stop();
                int elapsedMs = (int)watch.ElapsedMilliseconds;
                if (elapsedMs < 17) //17
                {
                    Thread.Sleep(17-elapsedMs);
                }
                //if (elapsedMs > max) max = elapsedMs;
                //if (elapsedMs < min) min = elapsedMs;
            }
            //Console.WriteLine("MAX: " + max);

        }

        public static void imgToAscii(bool[][] img) {
            bool b;
            for (int y = 0; y < 90; y++) {
                for (int x = 0; x < 240; x++) {
                    b = img[y][x];
                    if (b == prevImg[x][y]) {
                        continue;
                    }
                    Console.SetCursorPosition(x, y);
                    prevImg[x][y] = b;
                    if (b)
                    {
                        Console.Out.WriteAsync("$");
                    }
                    else
                    {
                        Console.Out.WriteAsync(" ");
                    }
                }
            }
            //Console.SetCursorPosition(0, img.Height);
        }
    }
}