using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using FFmpeg.NET;

namespace CMD_Apple
{
    internal class Program
    {
        private static int asciiWidth = 0;
        private static int asciiHeight = 0;
        static void Main(string[] args)
        {
            //char[] shader = {' ', '.', '\'', '`', '^', '"', ',', ':', ';', 'I', 'l', '!', 'i', '>', '<', 
            //    '~', '+', '_', '-', '?', ']', '[', '}', '{', '1', ')', '(', '|', '\\', '/', 't', 'f', 
            //    'j', 'r', 'x', 'n', 'u', 'v', 'c', 'z', 'X', 'Y', 'U', 'J', 'C', 'L', 'Q', '0', 'O', 
            //    'Z', 'm', 'w', 'q', 'p', 'd', 'b', 'k', 'h', 'a', 'o', '*', '#', 'M', 'W', '&', '8', 
            //    '%', 'B', '@', '$'};

            //char[] shader = {' ', '.', '\'', '`', '^', '"', ',', ':', ';', 'I', 'l', '!',
            //    '+', '_', '-', '1', '|', '\\', '/', 't', 'f', 'j', 'r', 'x', 'n', 'u', 
            //    'v', 'c', 'z', 'X', 'Y', 'J', 'C', 'Q', '0', 'O', 'Z', 'm', 'q', 'd', 
            //    'b', 'h', 'a', 'o', '*', '#', 'M', 'W', '8', 'B', '@', '$'};

            char[] shader = { ' ', '.', '\'', 'I', '!', '-', '|', 'X', 'Y', '0', 'O', 'Z', 'h', 'o', '#', 'M', 'W', '8', 'B', '@', '$' };

            //var vidFile = new MediaFile { Filename = @"G:\BadApple\BadApple.mp4" };
            //var ffmpegFilePath = @"C:\ffmpeg\bin\ffmpeg.exe";
            //var service = MediaToolkitService.CreateInstance(ffmpegFilePath);
            //using (var engine = new Engine())
            //{
            //    engine.GetMetadata(vidFile);
            //    Console.WriteLine(vidFile.Metadata.VideoData.Fps);
            //}
            //Thread.Sleep(5000);

            InputFile vidFile = new InputFile(@"G:\BadApple\BadApple.mp4");
            Engine ffmpeg = new Engine(@"C:\ffmpeg\bin\ffmpeg.exe");
            MetaData metadata = ffmpeg.GetMetaDataAsync(vidFile, new CancellationToken()).Result;

            setWindowSize(metadata);
            char[][] arr = vidToFrames(vidFile, ffmpeg, shader);
            playAscii(arr, getFps(metadata));
        }

        public static void setWindowSize(MetaData metadata) {
            int width = int.Parse(metadata.VideoData.FrameSize.Split('x')[0]);
            int height = int.Parse(metadata.VideoData.FrameSize.Split('x')[1]);
            asciiWidth = 37 * (width * 3 / height) * 2;
            asciiHeight = 111;

            Console.SetWindowSize(asciiWidth + 8, asciiHeight + 4);
            Console.SetBufferSize(asciiWidth + 8, asciiHeight + 8);
        }

        public static double getFps(MetaData metadata) {
            return metadata.VideoData.Fps;
        }

        public static char[][] vidToFrames(InputFile vidFile, Engine ffmpeg, char[] shader) {
            // TODO: Set Font Size to 16 (or whatever the default is)

            int frameCount = 6573; // GET FROM VID
            char[][] arr = new char[frameCount][];
            for (int i = 0; i < frameCount; i++)
            {
                Bitmap img = new Bitmap("F:\\Projects\\CMD_Apple\\res\\frame-" + (i + 1) + ".png"); // GET FROM VID
                img = new Bitmap(img, new Size(asciiWidth, asciiHeight));
                arr[i] = new char[(asciiWidth + 1) * asciiHeight];
                for (int y = 0; y < asciiHeight; y++)
                {
                    for (int x = 0; x < asciiWidth; x++)
                    {
                        arr[i][(y * (asciiWidth + 1)) + x] = shader[(int)(img.GetPixel(x, y).GetBrightness() * (shader.Length - 1))];
                    }
                    arr[i][(y * (asciiWidth + 1)) + asciiWidth] = '\n';
                }
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Rendered Frames: " + (i + 1) + "/" + (frameCount));
            }
            Console.Clear();
            return arr;
        }

        public static void playAscii(char[][] frames, double fps) {
            // TODO: Set Font Size to 8

            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < frames.Length; i++)
            {
                Console.SetCursorPosition(0, 0);
                Console.Out.WriteAsync(frames[i]);
                if (watch.Elapsed.TotalMilliseconds < (1 / fps) * 1000 * i)
                {
                    Thread.Sleep((int)Math.Round(((1 / fps) * 1000 * i) - (int)watch.Elapsed.TotalMilliseconds));
                }
            }
        }

    }
}