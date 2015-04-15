using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;

namespace BassGetSpectrum
{
    public class Spectrum
    {
        float time = 0;
        List<float[]> Data = new List<float[]>();

        public enum FFTSize
        {
            FFT128=128,
            FFT256=256,
            FFT512=512,
            FFT1024=1024,
            FFT2048=2048,
            FFT4096=4096,
            FFT8192=8192,
        }

        GLForm glfrm;
        float[] RawSpectr;

        public Bitmap GetBitmapSpectrum(string filename, FFTSize size, float fps,int maxwidth, int startscan=0)
        {
            Bitmap Image;
            //glfrm = new GLForm();
            //glfrm.Width = 1024;
            //glfrm.Height = 518;
            //glfrm.Show();
            GetFFTData(filename, size, fps, maxwidth);
          
            int heigth = (int)size;
            int width = (int)Data.Count ;
            if (width==0)
            {
                Image = new Bitmap(1, 1);
                return Image;
            }
            Image = new Bitmap(width, heigth);
            float min = 9999;
            float max = -9999;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < heigth; y++)
                {
                    if (Data[x][y]>max)
                    {
                        max = Data[x][y];
                    }
                    else
                    {
                        if (Data[x][y]<min)
                        {
                            min = Data[x][y];
                        }
                    }
                }
            }
            float amin = Math.Abs(min);
            float kor = 1 / (max + amin);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < heigth; y++)
                {
                    Data[x][y] = Data[x][y] + amin;
                    Data[x][y] = Data[x][y] * kor * 16*50;
                    Data[x][y] = Data[x][y] * Data[x][y];
                }
            }
            double dd, dd2,d,dd3,d2;
            byte d1, d3, d4,d5;
            Color c;
            for (int x = 0; x < width; x++)
            {
                dd = 0;
                dd2 = 0;
                for (int y = 0; y < heigth; y++)
                {
                    d = Math.Ceiling(Data[x][y]);
                    dd3 = 0;
                    if (d>255)
                    {
                        d = 255;
                    }
                    if (d < 0)
                    {
                        d = 0;
                    }
                    dd = dd * 100 + d; dd = dd / 101;
                    dd2 = dd2 * 10 + d; dd2 = dd2 / 11;
                    d1=(byte)d;
                    d2 = Math.Ceiling(dd2);
                    d3= (byte)d2;
                    d4 = (byte)Math.Ceiling(dd) ;
                    dd3 = d4 + d3 + d1;
                    dd3 = dd3 / 3;
                    d5 = (byte)Math.Ceiling(dd3);

                    c = Color.FromArgb(255, d3,d4, d5);
                    Image.SetPixel(x, y, c);
                }
            }
            //glfrm.BackgroundImage = Image;
            //glfrm.BackgroundImageLayout = ImageLayout.Zoom;
            //glfrm.Invalidate();
            //Application.DoEvents();
            //glfrm.Show ();
            //Thread.Sleep(3000);
            //glfrm.Close();
            return Image;
        }

        void glControl1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
           
        }

        public Bitmap GrabScreenshot(GLForm frm)
        {
            if (GraphicsContext.CurrentContext == null)
                throw new GraphicsContextMissingException();
            int Width = frm.glControl1.ClientSize.Width;
            int Height = frm.glControl1.ClientSize.Height;
            Bitmap bmp = new Bitmap(frm.glControl1.ClientSize.Width, Height);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(new Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, Width, Height, OpenTK.Graphics.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }

       public float[] GetRawSpectrData(string filename,FFTSize size,float fps,int maxwidth,int startscan=0)
        {
            float[] Spectr;
            GetFFTData(filename, size, fps, maxwidth, startscan);
            Spectr = DoScanSpectr(ref Data);
            return Spectr;
        }

       public float[] DoScanSpectr(ref List<float[]> data)
        {
            float[] RawSpectr = new float[1024];
            foreach (var SpectrumArrayRight in data)
            {
                if (SpectrumArrayRight != null)
                {
                    float pik = 1F;
                    float max = -9999;
                    for (int i = 0; i < SpectrumArrayRight.Length ; i++)
                    {
                        if (SpectrumArrayRight[i] >max )
                        {
                            max = SpectrumArrayRight[i];
                        }
                    }
                    float maf = 0.05F/max ;
                    for (int i = 0; i < SpectrumArrayRight.Length; i++)
                    {
                        float ddd = (float)i / 1023F;
                        pik = pik + ddd / (pik / 2F);
                        float k = (SpectrumArrayRight[i]) * 4 * pik;
                        k = k * maf;
                        if (k > 0.9) { k = 0.9F; }
                        if (k > 0.0005)
                        {
                            RawSpectr[i] = (RawSpectr[i] * 5500 + k) / 5501;
                        }
                        if (k > 0.05)
                        {
                            RawSpectr[i] = (RawSpectr[i] * 100 + k) / 101;
                        }
                        if (k > 0.1)
                        {
                            RawSpectr[i] = (RawSpectr[i] * 70 + k) / 71;
                        }
                        if (k > 0.3)
                        {
                            RawSpectr[i] = (RawSpectr[i] * 30 + k) / 31;
                        }
                        if (k > 0.5)
                        {
                            RawSpectr[i] = (RawSpectr[i] * 15 + k) / 16;
                        }
                        if (k > 0.9)
                        {
                            RawSpectr[i] = (RawSpectr[i] * 10 + k) / 11;
                        }
                    }
                    float[] ffts = new float[5];
                    for (int i = 2; i < RawSpectr.Length - 2; i += 1)
                    {
                        ffts[0] = RawSpectr[i - 2] / 4F;
                        ffts[1] = RawSpectr[i + 2] / 4F;
                        ffts[2] = RawSpectr[i - 1] / 2F;
                        ffts[3] = RawSpectr[i + 1] / 2F;
                        ffts[4] = RawSpectr[i];
                        RawSpectr[i] = ffts[0] + ffts[1] + ffts[2] + ffts[3] + ffts[4];
                        RawSpectr[i] = RawSpectr[i] / 3F;
                    }
                }
            }
            float maxx = -9999;
            for (int i = 0; i < RawSpectr.Length; i++)
            {
                if (RawSpectr[i] > maxx)
                {
                    maxx = RawSpectr[i];
                }
            }
            float maff = 5F/maxx;
            for (int i = 0; i < RawSpectr.Length; i++)
            {
                RawSpectr[i] = RawSpectr[i] * maff;
            }
            return RawSpectr;
        }

        public List<float[]> GetFFTData(string filename, FFTSize size, float fps, int maxwidth, int startscan=0)
        {
            Un4seen.Bass.BASSData FFTType = Un4seen.Bass.BASSData.BASS_DATA_FFT256;
            int arlen = 0;
            if (size == FFTSize.FFT128) FFTType = Un4seen.Bass.BASSData.BASS_DATA_FFT256;
            if (size == FFTSize.FFT256) FFTType = Un4seen.Bass.BASSData.BASS_DATA_FFT512;
            if (size == FFTSize.FFT512) FFTType = Un4seen.Bass.BASSData.BASS_DATA_FFT1024;
            if (size == FFTSize.FFT1024) FFTType = Un4seen.Bass.BASSData.BASS_DATA_FFT2048;
            if (size == FFTSize.FFT2048) FFTType = Un4seen.Bass.BASSData.BASS_DATA_FFT4096;
            if (size == FFTSize.FFT4096) FFTType = Un4seen.Bass.BASSData.BASS_DATA_FFT8192;
            if (size == FFTSize.FFT8192) FFTType = Un4seen.Bass.BASSData.BASS_DATA_FFT16384;
            arlen=(int)size;
            int len = (int)FFTType;

            GC.Collect();
            time = 0;
            Data.Clear();
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_NOSPEAKER, IntPtr.Zero);
            // create the stream
            int chan = Bass.BASS_StreamCreateFile(filename, 0, 0,
                              BASSFlag.BASS_SAMPLE_FLOAT  | BASSFlag.BASS_STREAM_DECODE);
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 0);
            var pos = Bass.BASS_ChannelGetLength(chan);
            var p = Bass.BASS_ChannelBytes2Seconds (chan, pos);
            //Console.Write("ScanFile");
            //Console.Clear();
            long byte_pos=0;
            int n = 0;
            while (byte_pos<pos)
            {
                n++;
                
                if (Data.Count > maxwidth)
                {
                    break;
                }
                else if(n>startscan)
                {
                    byte_pos = Bass.BASS_ChannelSeconds2Bytes(chan, time);
                    //double d = (double)n / (double)maxwidth;
                    //d = d * 100;
                    //d = Math.Ceiling(d);
                    //if (d % 10 == 0)
                    //{
                    //    Console.SetCursorPosition(0, 0);
                    //    Console.Write(filename);
                    //    Console.SetCursorPosition(0, 1);
                    //    Console.Write(d.ToString() + "%");
                    //}
                    Bass.BASS_ChannelSetPosition(chan, byte_pos, BASSMode.BASS_POS_BYTES);

                    float[] fft = new float[arlen];
                    Bass.BASS_ChannelGetData(chan, fft, len);
                    Data.Add(fft);
                }
                time += 1F / fps;
            }
            Bass.BASS_ChannelPause(chan);
            Bass.BASS_Stop();
            Bass.BASS_StreamFree(chan);
            GC.Collect();
            return Data;
        }
    }
}
