﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;

namespace Hitbox2Screens
{
    class Program
    {
        private static void ScaleImage(string pathStr)
        {
            FileStream fileStream = null;
            Image image;
            bool isVisualLevel = false;
            try
            {
                if (File.Exists(pathStr + "\\level.png"))
                {
                    Console.WriteLine("Found level.png");
                    fileStream = new FileStream(pathStr + "\\level.png", FileMode.Open, FileAccess.Read);
                }
                else if (File.Exists(pathStr + "\\visual_level.png"))
                {
                    Console.WriteLine("Found visual_level.png");
                    fileStream = new FileStream(pathStr + "\\visual_level.png", FileMode.Open, FileAccess.Read);
                    isVisualLevel = true;
                }
                else
                {
                    throw new FileNotFoundException();
                }
                image = Image.FromStream(fileStream, true, false);
            }
            finally
            {
                fileStream?.Close();
                fileStream?.Dispose();
            }

            int newWidth = image.Width * 8;
            int newHeight = image.Height * 8;
            Bitmap scaledBitmap = new Bitmap(newWidth, newHeight);

            Graphics scaledGraph = Graphics.FromImage(scaledBitmap);
            scaledGraph.CompositingQuality = CompositingQuality.AssumeLinear;
            scaledGraph.InterpolationMode = InterpolationMode.NearestNeighbor;
            scaledGraph.SmoothingMode = SmoothingMode.AntiAlias;
            scaledGraph.PixelOffsetMode = PixelOffsetMode.Half;

            Rectangle imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
            scaledGraph.DrawImage(image, imageRectangle);

            pathStr = pathStr + ("\\{0}.png");
            int screenNo = 0;
            if (!isVisualLevel)
            {
                if (image.Width != 780 || image.Height != 585)
                {
                    Console.WriteLine("Invalid level.png image size");
                    Console.WriteLine("Should be 780px by 585px but found " + image.Width + "px by " + image.Height + "px");
                    Console.ReadLine();
                    return;
                }
                for (int i = 0; i < 13; i++)
                {
                    for (int j = 0; j < 13; j++)
                    {
                        screenNo++;
                        Rectangle cropArea = new Rectangle(480 * i, 360 * j, 480, 360);
                        scaledBitmap.Clone(cropArea, scaledBitmap.PixelFormat).Save(string.Format(pathStr, screenNo));
                    }
                }
            }
            else
            {
                if (image.Width != 60 || image.Height != 7605)
                {
                    Console.WriteLine("Invalid visual_level.png image size");
                    Console.WriteLine("Should be 60px by 7605px but found " + image.Width + "px by " + image.Height + "px");
                    Console.ReadLine();
                    return;
                }
                for (int i = 0; i < 169; i++)
                {
                    screenNo++;
                    Rectangle cropArea = new Rectangle(0, 360 * (168 - i), 480, 360);
                    scaledBitmap.Clone(cropArea, scaledBitmap.PixelFormat).Save(string.Format(pathStr, screenNo));
                }
            }

            scaledGraph.Dispose();
            scaledBitmap.Dispose();
            image.Dispose();

            Console.WriteLine("Cleaning up memory");
            Console.WriteLine("Program has finished running");
            Console.ReadLine();
        }

        static void Main()
        {
            string pathStr = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine("Searching for png in: " + pathStr);
            try
            {
                ScaleImage(pathStr);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("No level.png or visual_level.png found");
                Console.WriteLine("Please make sure the png file is in the same folder as the exe");
                Console.ReadLine();
            }
        }
    }
}
