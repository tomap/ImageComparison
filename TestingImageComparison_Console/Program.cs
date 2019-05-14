using System;
using XnaFan.ImageComparison;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Diagnostics;

// Created in 2012 by Jakob Krarup (www.xnafan.net).
// Use, alter and redistribute this code freely,
// but please leave this comment :)

namespace TestingImageComparison_Console
{
    class Program
    {

        static DirectoryInfo exeFolder, projectFolder, sampleImagesFolder, duplicateImagesFolder;

        public static void Main(string[] args)
        {
            //find the image folder
            string codebase = Assembly.GetExecutingAssembly().Location;
            exeFolder = new DirectoryInfo(Path.GetDirectoryName(codebase));
            projectFolder = exeFolder.Parent.Parent;
            sampleImagesFolder = new DirectoryInfo(Path.Combine(projectFolder.FullName, "SampleImages"));
            duplicateImagesFolder = new DirectoryInfo(Path.Combine(sampleImagesFolder.FullName, "duplicateImages"));

            //compare different images
            Compare("lab200.jpg", "lab100.jpg");
            Compare("img_one.png", "img_two.png");
            Compare("firefox1.png", "firefox2.png");

            //show the images in the folder
            Process.Start(sampleImagesFolder.FullName);

            //display a search for duplicates
            ShowDuplicates();


            Console.WriteLine("Any key to end...");
            Console.ReadKey();
        }

        static void Compare(string bmp1, string bmp2, byte threshold = 3)
        {

            //get the full path of the images
            string image1Path = Path.Combine(sampleImagesFolder.FullName, bmp1);
            string image2Path = Path.Combine(sampleImagesFolder.FullName, bmp2);
            //compare the two
            Console.WriteLine("Comparing: " + bmp1 + " and " + bmp2 + ", with a threshold of " + threshold);
            Bitmap firstBmp = (Bitmap)Image.FromFile(image1Path);
            Bitmap secondBmp = (Bitmap)Image.FromFile(image2Path);
            SetMaximumSize(firstBmp, secondBmp, 64);
            //get the difference as a bitmap
            firstBmp.GetDifferenceImage(secondBmp, true)
                .Save(image1Path + "_diff.png");
            Console.WriteLine(string.Format("Difference: {0:0.0} %", firstBmp.PercentageDifference(secondBmp, threshold) * 100));
            Console.WriteLine(string.Format("BhattacharyyaDifference: {0:0.0} %", firstBmp.BhattacharyyaDifference(secondBmp) * 100));
            Console.WriteLine("ENTER to see histogram for " + bmp1);
            Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("Creating histogram for " + bmp1);
            Histogram hist = new Histogram(firstBmp);
            firstBmp.Dispose();
            secondBmp.Dispose();
            hist.Visualize().Save(image1Path + "_hist.png");
            Console.WriteLine(hist.ToString());
            Console.WriteLine("ENTER to continue...");
            Console.ReadLine();
        }

        private static void SetMaximumSize(Bitmap firstBmp, Bitmap secondBmp, float avgSize)
        {
            float w = Math.Min(firstBmp.Width, secondBmp.Width);
            float h = Math.Min(firstBmp.Height, secondBmp.Height);
            float avg = Math.Min((float)w / avgSize, (float)h / avgSize);
            XnaFan.ImageComparison.ExtensionMethods.SetCompressSize((int)Math.Round(w / avg), (int)Math.Round(h / avg));
        }

        static void ShowDuplicates()
        {
            Console.WriteLine("Finding duplicates in " + duplicateImagesFolder.FullName);
            foreach (var list in ImageTool.GetDuplicateImages(duplicateImagesFolder.FullName, true))
            {
                Console.WriteLine(list.Count + " duplicates:");
                foreach (var item in list)
                {
                    Console.WriteLine(" - " + Path.GetFileName(item));
                }
            }
        }
    }
}
