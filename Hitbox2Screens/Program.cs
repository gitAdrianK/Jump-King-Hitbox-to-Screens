using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;

namespace Hitbox2Screens
{
    public static class Program
    {
        public static void Main()
        {
            var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (exePath is null)
            {
                Console.WriteLine("Something went wrong getting the path!");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Searching for png in: {exePath}");
            string imagePath;
            var isVisualLevel = false;
            if (File.Exists(Path.Combine(exePath, "level.png")))
            {
                imagePath = Path.Combine(exePath, "level.png");
                Console.WriteLine("Found level.png");
            }
            else if (File.Exists(Path.Combine(exePath,"visual_level.png")))
            {
                imagePath = Path.Combine(exePath, "visual_level.png");
                isVisualLevel = true;
                Console.WriteLine("Found visual_level.png");
            }
            else
            {
                Console.WriteLine("No level.png or visual_level.png found");
                Console.WriteLine("Please make sure the png file is in the same folder as the exe");
                Console.ReadLine();
                return;
            }

            using var image = Image.FromFile(imagePath);
            Console.WriteLine($"Found {image.Width}x{image.Height} image");
            if (image.Width % 60 != 0 || image.Height % 45 != 0)
            {
                Console.WriteLine("Invalid size. Should be a multiple of 60x45");
                Console.WriteLine($"Width off by: {image.Width % 60}");
                Console.WriteLine($"Height off by: {image.Height % 45}");
                return;
            }

            var scaledWidth = image.Width * 8;
            var scaledHeight = image.Height * 8;
            using var scaledBitmap = new Bitmap(scaledWidth, scaledHeight);
            using var scaledGraphics = Graphics.FromImage(scaledBitmap);
            scaledGraphics.CompositingQuality = CompositingQuality.AssumeLinear;
            scaledGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            scaledGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            scaledGraphics.PixelOffsetMode = PixelOffsetMode.Half;
            scaledGraphics.DrawImage(image, new Rectangle(0, 0, scaledWidth, scaledHeight));

            var outDir = Path.Combine(exePath, "out");
            Directory.CreateDirectory(outDir);

            var widthSegments = image.Width / 60;
            var heightSegments = image.Height / 45;
            var totalScreens = widthSegments * heightSegments;
            Console.WriteLine($"Width segments: {widthSegments}");
            Console.WriteLine($"Height segments: {heightSegments}");
            Console.WriteLine($"Creating images for {totalScreens} screens");

            for (var i = 0; i < widthSegments; i++)
            {
                for (var j = 0; j < heightSegments; j++)
                {
                    var screen = isVisualLevel ? totalScreens - (widthSegments * i + j) : widthSegments * i + j + 1;
                    scaledBitmap
                        .Clone(new Rectangle(480 * i, 360 * j, 480, 360), scaledBitmap.PixelFormat)
                        .Save(Path.Combine(outDir, screen + ".png"));
                }
            }

            Console.WriteLine("Program has finished running");
            Console.ReadLine();
        }
    }
}
