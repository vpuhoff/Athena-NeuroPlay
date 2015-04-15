using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BassGetSpectrum;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Spectrum sp = new Spectrum();
            Spectrum.FFTSize size = Spectrum.FFTSize.FFT2048   ;
            float[] data = new float[(int)size];
            var files = Directory.GetFiles(@"E:\Музыка\_Музыка\Soniani Super Sonico [OST]", "*.mp3");
            foreach (var item in files)
            {
                var tdata = sp.GetBitmapSpectrum (item, size, 100,2000,1000);
                //int w = tdata.Width;
                //int h = tdata.Height;
                //for (int x = 1; x < w-1 ; x++)
                //{
                //    for (int y = 1; y < h-1; y++)
                //    {
                //        byte c = tdata.GetPixel(x, y).B;
                //        byte c2 = tdata.GetPixel(x-1, y-1).B;
                //        byte c3 = tdata.GetPixel(x + 1, y + 1).B;
                //        byte c4 = tdata.GetPixel(x , y - 1).B;
                //        byte c5 = tdata.GetPixel(x + 1, y).B;
                //        double d = (c2 + c3 + c4 + c5) / 4;
                //        d = d + c;
                //        d = Math.Ceiling(d);
                //        if (d>255)
                //        {
                //            d = 255;
                //        }
                //        c = (byte)(d);
                //        tdata.SetPixel(x,y,Color.FromArgb(tdata.GetPixel(x, y).R,0,c));
                //    }
                //}
                tdata.Save(@"D:\111\" + Guid.NewGuid().ToString() + ".png", ImageFormat.Png);
                //foreach (var arr in tdata)
                //{

                //    for (int i = 0; i < (int)size; i++)
                //    {
                //        data[i] = data[i] + Math.Abs(arr[i]);
                //    }
                    
                //}
            }
            //Console.ReadLine();
        }
    }
}
