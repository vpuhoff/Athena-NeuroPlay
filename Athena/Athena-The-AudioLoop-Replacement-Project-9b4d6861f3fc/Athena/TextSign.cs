using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Athena
{
    class TextSign
    {
        public double[] mergeArrays(List<double[]> bps)
        {
            int cnt = 0;
            foreach (var item in bps)
            {
                cnt += item.Length;
            }
            double[] bp = new double[cnt];
            int k = 0;
            foreach (var item in bps)
            {
                for (int i = 0; i < item.Length; i++)
                {
                    bp[k] = item[i];
                    k++;
                }
            }
            return bp;
        }
        public double[] getBitmapArrayString(string[] data)
        {
            List<double[]> bps = new List<double[]>();
            for (int i = 0; i < data.Length; i++)
            {
                bps.Add(getBitmapString(data[i]));
            }
            var bp = mergeArrays(bps);
            return bp;
        }
        public double[]  getBitmapString(string s)
        {
            return GetBoolVector(s);
        }
        //public Bitmap getBitmapVector(bool[] vec)
        //{
        //    Bitmap bp = new Bitmap(vec.Length, 1);
        //    for (int i = 0; i < vec.Length; i++)
        //    {
        //        if (vec[i])
        //        {
        //            bp.SetPixel(i, 0, Color.Black);
        //        }
        //        else
        //        {
        //            bp.SetPixel(i, 0, Color.White);
        //        }
        //    }
        //    return bp;
        //}
        public double[] GetBoolVector(string s)
        {
            var vec = getVector(s);
            vec = multisimplify(vec, 3);
            var vec2 = boolifice(vec);
            return vec2;
        }

        public double[] boolifice(double[] vec,int start=0,int descret=64)
        {
            double d = -9999;
            for (int i = start; i < vec.Length; i++)
            {
                if (vec[i]>d)
                {
                    d = vec[i];
                }
            }
            double aver = Math.Sqrt(d);
            if (aver>d)
            {
                aver = vec.Average();
            }
            int len=vec.Length;
            double[] newvec = new double[len];
            double val = 0;
            
            for (int i = 0; i < len; i++)
            {
                if (i%descret ==0)
                {
                    if (i!=0)
                    {
                        if (i + descret < len)
                        {
                            for (int l = i; l < i + descret; l++)
                            {
                                if (vec[i] > d)
                                {
                                    d = vec[l];
                                }
                            }
                        }
                        aver = Math.Sqrt(d);
                        if (aver > d)
                        {
                            aver = vec.Average();
                        }
                    }
                }
                if (i < start)
                {
                    newvec[i] = vec[i];
                }
                else
                {
                    val = vec[i];
                    if (val <= aver)
                    {
                        newvec[i] = -0.5;
                    }
                    else
                    {
                        newvec[i] = 0.5;
                    }
                }
            }
            return newvec;
        }
        public double[] multisimplify(double[] vec, int value)
        {
            double[] newvec;
            newvec = vec;
            for (int i = 0; i < value; i++)
            {
                newvec = simplifyVector(newvec);
            }
            //for (int i = 0; i < newvec.Length; i++)
            //{
            //    if (newvec[i] > 0)
            //    {
            //        Console.Write("#");
            //    }
            //    else
            //    {
            //        Console.Write("*");
            //    }
            //}
            //Console.WriteLine();
            return newvec;
        }
        public double[] simplifyVector(double[] vec)
        {
            double[] newvec = new double[vec.Length / 2];
            for (int i = 0; i < vec.Length / 2; i++)
            {
                newvec[i] = vec[i * 2] + vec[i * 2 + 1];
            }
            //for (int i = 0; i < newvec.Length ; i++)
            //{
            //    if (newvec[i] > 0)
            //    {
            //        Console.Write("#");
            //    }
            //    else
            //    {
            //        Console.Write("*");
            //    }
            //}
            //Console.WriteLine();
            return newvec;
        }

        public double[] getVector(string text)
        {
            const int siz=128;
            double[] vec = new double[siz];
            for (int i = 0; i < siz; i++)
            {
                vec[i] = 2;
            }
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i]!=' ')
                {
                    int n = (int)text[i];
                    n = n + siz;
                    n = n % siz;
                    vec[n] = vec[n] * (Math.Sqrt(vec[n]));
                }
            }
            for (int i = 0; i < siz; i++)
            {
                vec[i] = vec[i]-2;
            }
            //for (int i = 0; i < 255; i++)
            //{
            //    if (vec[i]>0)
            //    {
            //        Console.Write("#");
            //    }
            //    else
            //    {
            //        Console.Write("*");
            //    }
            //}
            //Console.WriteLine();
            return vec;
        }
    }
}
