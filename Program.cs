using FFMpegCore;
using FFMpegCore.Pipes;
using System.Diagnostics;
using System.Drawing;
using System.Runtime;
using System.Runtime.CompilerServices;

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

            char[] shader = {' ', '.', '\'', 'I', '!', '-', '|', 'X', 'Y', '0', 'O', 'Z', 'h', 'o', '#', 'M', 'W', '8', 'B', '@', '$'};

            String vidPath = @"G:\BadApple\BadApple.mp4";
            IMediaAnalysis vidInfo = FFProbe.Analyse(vidPath);

            setWindowSize(vidInfo);
            char[][] arr = vidToFrames(vidPath, vidInfo, shader);
            playAscii(arr, getFps(vidInfo));
        }

        public static void setWindowSize(IMediaAnalysis vidInfo) {
            if (vidInfo.PrimaryVideoStream == null) throw new Exception("Primary video stream is null");

            int width = vidInfo.PrimaryVideoStream.Width;
            int height = vidInfo.PrimaryVideoStream.Height;
            asciiWidth = 37 * (width * 3 / height) * 2;
            asciiHeight = 111;

            Console.SetWindowSize(asciiWidth + 8, asciiHeight + 4);
            Console.SetBufferSize(asciiWidth + 8, asciiHeight + 8);
        }

        public static double getFps(IMediaAnalysis vidInfo) {
            if (vidInfo.PrimaryVideoStream == null) throw new Exception("Primary video stream is null");

            return vidInfo.PrimaryVideoStream.FrameRate;
        }

        public static char[][] vidToFrames(string vidPath, IMediaAnalysis vidInfo, char[] shader)
        {
            // TODO: Set Font Size to 16 (or whatever the default is)

            if (vidInfo.PrimaryVideoStream == null) throw new Exception("Primary video stream is null");
            int frameCount = (int)Math.Floor(vidInfo.Duration.TotalMilliseconds / 1000 * vidInfo.PrimaryVideoStream.FrameRate) - 1;
            //frameCount = 1000;

            char[][] arr = new char[frameCount][];
            for (int i = 0; i < frameCount; i++)
            {
                Bitmap img = Snapshot(vidPath, new Size(asciiWidth, asciiHeight), TimeSpan.FromMilliseconds(i * (1000 / vidInfo.PrimaryVideoStream.FrameRate)));
                //Bitmap img = new Bitmap("F:\\Projects\\CMD_Apple\\res\\frame-" + (i + 1) + ".png"); // GET FROM VID
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

        public static Bitmap Snapshot(string input, Size? size = null, TimeSpan? captureTime = null, int? streamIndex = null, int inputFileIndex = 0)
        {
            var source = FFProbe.Analyse(input);
            var (arguments, outputOptions) = SnapshotArgumentBuilder.BuildSnapshotArguments(input, source, size, captureTime, streamIndex, inputFileIndex);
            using var ms = new MemoryStream();

            arguments
                .OutputToPipe(new StreamPipeSink(ms), options => outputOptions(options
                    .ForceFormat("rawvideo")))
                .ProcessSynchronously();

            ms.Position = 0;
            using var bitmap = new Bitmap(ms);
            return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);
        }

    }
}