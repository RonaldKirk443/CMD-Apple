using FFMpegCore;
using FFMpegCore.Enums;
using System.Diagnostics;
using System.Drawing;
using WMPLib;
using IniParser.Model;
using IniParser;

namespace CMD_Apple
{
    internal class Program
    {
        private static int asciiWidth = 0;
        private static int asciiHeight = 0;
        static void Main(string[] args)
        {
            char[] shaderMax = {' ', '.', '\'', '`', '^', '"', ',', ':', ';', 'I', 'l', '!', 'i', '>', '<',
                '~', '+', '_', '-', '?', ']', '[', '}', '{', '1', ')', '(', '|', '\\', '/', 't', 'f',
                'j', 'r', 'x', 'n', 'u', 'v', 'c', 'z', 'X', 'Y', 'U', 'J', 'C', 'L', 'Q', '0', 'O',
                'Z', 'm', 'w', 'q', 'p', 'd', 'b', 'k', 'h', 'a', 'o', '*', '#', 'M', 'W', '&', '8',
                '%', 'B', '@', '$'};

            char[] shaderMid = {' ', '.', '\'', '`', '^', '"', ',', ':', ';', 'I', 'l', '!',
                '+', '_', '-', '1', '|', '\\', '/', 't', 'f', 'j', 'r', 'x', 'n', 'u',
                'v', 'c', 'z', 'X', 'Y', 'J', 'C', 'Q', '0', 'O', 'Z', 'm', 'q', 'd',
                'b', 'h', 'a', 'o', '*', '#', 'M', 'W', '8', 'B', '@', '$'};

            char[] shaderSmall = {' ', '.', '\'', 'I', '!', '-', '|', 'X', 'Y', '0', 'O', 'Z', 'h', 'o', '#', 'M', 'W', '8', 'B', '@', '$'};

            char[] shaderMin = {' ', '.', '\'', 'X', 'Y', '0', 'M', '@', '$'};
            FileIniDataParser settingsParser = new FileIniDataParser();
            IniData settingsData = settingsParser.ReadFile("settings.ini");

            GlobalFFOptions.Configure(options => options.BinaryFolder = @settingsData["settings"]["ffmpeg-path"]);

            char[] shader = int.Parse(settingsData["settings"]["shader"]) switch
            {
                1 => shaderMin,
                2 => shaderSmall,
                3 => shaderMid,
                4 => shaderMax,
                _ => shaderMin,
            };

            FontChanger.setFontSize(16);
            string vidPath = settingsData["settings"]["video-name"];

            IMediaAnalysis vidInfo = FFProbe.Analyse(vidPath);

            setPlayBackSize(vidInfo);

            if (getFps(vidInfo) > int.Parse(settingsData["settings"]["max-realtime-fps"]))
            {
                Console.WriteLine("Since the video is over 30 fps, the frames will be pre-rendered");
                Console.WriteLine("Thank you for your patience");
                Thread.Sleep(5000);
                vidToFrames(vidPath, vidInfo);
                char[][] arr = framesToCharArr(shader);
                exportAudio(vidPath);
                setWindowSize(asciiWidth, asciiHeight);
                playAscii(arr, getFps(vidInfo), int.Parse(settingsData["settings"]["volume"]));
            }
            else {
                vidToFrames(vidPath, vidInfo);
                exportAudio(vidPath);
                setWindowSize(asciiWidth, asciiHeight);
                renderAndPlayAscii(getFps(vidInfo), shader, int.Parse(settingsData["settings"]["volume"]));
            }

            FontChanger.setFontSize(16);
            Console.Clear();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        public static void exportAudio(string vidPath) {
            Console.WriteLine("Exporting Audio ...");
            FFMpeg.ExtractAudio(vidPath, @"res\audio.mp3");
            Console.Clear();
        }

        public static void setPlayBackSize(IMediaAnalysis vidInfo) {
            if (vidInfo.PrimaryVideoStream == null) throw new Exception("Primary video stream is null");

            int width = vidInfo.PrimaryVideoStream.Width;
            int height = vidInfo.PrimaryVideoStream.Height;
            asciiWidth = (int)MathF.Round(width * 111f / height * (8f/5f));
            asciiHeight = 111;
        }

        public static void setWindowSize(int width, int height) {
            FontChanger.setFontSize(8);

            Console.SetWindowSize(width + 8, height + 4);
            Console.SetBufferSize(width + 8, height + 8);
        }

        public static double getFps(IMediaAnalysis vidInfo) {
            if (vidInfo.PrimaryVideoStream == null) throw new Exception("Primary video stream is null");

            return vidInfo.PrimaryVideoStream.FrameRate;
        }

        public static void vidToFrames(string vidPath, IMediaAnalysis vidInfo)
        {
            if (vidInfo.PrimaryVideoStream == null) throw new Exception("Primary video stream is null");

            Console.WriteLine("Exporting Frames ...");
            if (Directory.Exists(@"res")) Directory.Delete(@"res", true);
            Directory.CreateDirectory(@"res");
            FFMpegArguments.FromFileInput(vidPath).OutputToFile(@"res\%d.png", true, Options => { Options.WithVideoCodec(VideoCodec.Png); }).ProcessSynchronously();
            Console.Clear();
        }

        public static char[][] framesToCharArr(char[] shader) {
            int frameCount = Directory.GetFiles(@"res\").Length;
            char[][] arr = new char[frameCount][];
            for (int i = 0; i < frameCount; i++)
            {
                Bitmap img = new Bitmap(@"res\" + (i + 1) + ".png");
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

        public static void playAscii(char[][] frames, double fps, int volume) {

            WindowsMediaPlayer myplayer = new WindowsMediaPlayer();
            myplayer.settings.volume = volume;
            myplayer.URL = @"res\audio.mp3";
            myplayer.controls.play();

            var watch = Stopwatch.StartNew();
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

        public static void renderAndPlayAscii(double fps, char[] shader, int volume)
        {
            WindowsMediaPlayer myplayer = new WindowsMediaPlayer();
            myplayer.settings.volume = volume;
            myplayer.URL = @"res\audio.mp3";
            myplayer.controls.play();

            int frameCount = Directory.GetFiles(@"res\").Length - 1;
            char[] frame = new char[(asciiWidth + 1) * asciiHeight];

            var watch = Stopwatch.StartNew();
            for (int i = 0; i < frameCount; i++)
            {
                Console.SetCursorPosition(0, 0);

                Bitmap img = new Bitmap(@"res\" + (i + 1) + ".png");
                img = new Bitmap(img, new Size(asciiWidth, asciiHeight));
                frame = new char[(asciiWidth + 1) * asciiHeight];
                for (int y = 0; y < asciiHeight; y++)
                {
                    for (int x = 0; x < asciiWidth; x++)
                    {
                        frame[(y * (asciiWidth + 1)) + x] = shader[(int)(img.GetPixel(x, y).GetBrightness() * (shader.Length - 1))];
                    }
                    frame[(y * (asciiWidth + 1)) + asciiWidth] = '\n';
                }
                Console.Out.WriteAsync(frame);

                if (watch.Elapsed.TotalMilliseconds < (1 / fps) * 1000 * i)
                {
                    Thread.Sleep((int)Math.Round(((1 / fps) * 1000 * i) - (int)watch.Elapsed.TotalMilliseconds));
                }
            }
        }

    }
}