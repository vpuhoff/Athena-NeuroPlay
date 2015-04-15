using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using ClassLibraryNeuralNetworks;
using System.IO.Compression;
using System.Threading;

namespace Athena
{
    
    class NeuroHelper
    {
       public  List<BaseSign> Signes = new List<BaseSign>();
        
        [Serializable]
       public class BaseSign
        {
            public string filepath;
            public double[] TGI;
            public string filename;
            public MusicID3Tag Tags;
            double Rating;
            public double[] Days;
            public double[] Hours;
            public double[] NeuroDays;
            public double[] NeuroHours;
            public double[] Spectr;
            public double[] SpecData;
            public DateTime LastPlay;
            public double[] GetIn()
            {
                var r = mergeArrays(Days,Hours);
                return r;
            }
            
            public double[] GetAllIn()
            {
                if (TGI!=null )
                {
                    if (Spectr != null)
                    {
                        TextSign ts = new TextSign();
                        //var spec= ts.simplifyVector(Spectr);
                        var r1 = ts.boolifice(TGI);
                        //TGI = r1;
                        var r = mergeArrays(r1, Spectr);
                        r = ts.boolifice(r,96);
                        return r;
                    }
                }
                return null;
            }
            public void SetOut(double[] data)
            {
                Init();
                for (int i = 0; i < 7; i++)
                {
                    NeuroDays[i] = data[i];
                }
                for (int i = 0; i < 24; i++)
                {
                    NeuroHours[i] = data[7 + i];
                }
            }
            
        

            public List<CorrectHistoryItem> CorrectHistory;
            public enum corValues
            {
                VeryGood,
                Good,
                Norm,
                Skip,
                Bad,
                VeryBad
            }

            public void Correct(corValues Value,byte day,byte hour)
            {
                Init();
                CorrectHistoryItem hi = new CorrectHistoryItem(Value, day, hour);
                CorrectHistory.Add(hi);
                 double k1;
                 double k2;
                for (int i = 0; i < 7; i++)
                {
                    if (i==day )
                    {
                        k1 = 0.5;
                        k2 = 0.3;
                    }
                    else
                    {
                        k1 = 0.2;
                        k2 = 0.1;
                    }
                    
                    for (int j = 0; j < Days.Length; j++)
                    {
                        if (Days[j] > 0.99)
                        {
                            Days[j] = 0.99;
                        }
                        else if (Days[j] < 0.01)
                        {
                            Days[j] = 0.01;
                        }

                    }
                }
                k1 = 0.5;
                k2 = 0.3;
                if (Value == corValues.VeryGood)
                {
                    Hours[hour] = Hours[hour] + Hours[hour] * k1;
                }
                if (Value == corValues.VeryBad)
                {
                    Hours[hour] = Hours[hour] - Hours[hour] * k1;
                }
                if (Value == corValues.Good)
                {
                    Hours[hour] = Hours[hour] + Hours[hour] * k1 * k2;
                }
                if (Value == corValues.Norm)
                {
                    Hours[hour] = Hours[hour] + Hours[hour] * k1 * k2 / 2;
                }
                if (Value == corValues.Skip)
                {
                    Hours[hour] = Hours[hour] - Hours[hour] * k1 * k2 / 2;
                }

                for (int j = 0; j < Hours.Length; j++)
                {
                    if (Hours[j] > 0.99)
                    {
                        Hours[j] = 0.99;
                    }
                    else if (Hours[j] < 0.01)
                    {
                        Hours[j] = 0.01;
                    }
                }
            }
            double[] mergeArrays(double[] A, double[] B)
            {
                if ((A!=null )&(B!=null ))
                {
                    int cnt = A.Length + B.Length;
                    double[] bp = new double[cnt];
                    int k = 0;
                    for (int i = 0; i < A.Length; i++)
                    {
                        bp[k] = A[i];
                        k++;
                    }
                    for (int i = 0; i < B.Length; i++)
                    {
                        bp[k] = B[i];
                        k++;
                    }
                    return bp;
                }
                return null;
            }
            public void Correct(corValues Value)
            {
                Init();
                for (int day = 0; day < 7; day++)
                {
                    for (int hour = 0; hour < 24; hour++)
                    {
                        if (Value == corValues.VeryGood)
                        {
                            Days[day] = Days[day] + Days[day] * 0.019;
                            Hours[hour] = Hours[hour] + Hours[hour] * 0.019;
                        }
                        if (Value == corValues.VeryBad)
                        {
                            Days[day] = Days[day] - Days[day] * 0.019;
                            Hours[hour] = Hours[hour] - Hours[hour] * 0.019;
                        }
                        if (Value == corValues.Good)
                        {
                            Days[day] = Days[day] + Days[day] * 0.013;
                            Hours[hour] = Hours[hour] + Hours[hour] * 0.013;
                        }
                        if (Value == corValues.Norm)
                        {
                            Days[day] = Days[day] + Days[day] * 0.015;
                            Hours[hour] = Hours[hour] + Hours[hour] * 0.015;
                        }
                        if (Value == corValues.Skip)
                        {
                            Days[day] = Days[day] - Days[day] * 0.005;
                            Hours[hour] = Hours[hour] - Hours[hour] * 0.005;
                        }
                    }
                }
                for (int i = 0; i < Days.Length; i++)
                {
                    if (Days[i]>0.99)
                    {
                        Days[i] = 0.99;
                    }
                    else if (Days[i] < 0.01)
                    {
                        Days[i] = 0.01;
                    }
                    
                }
                for (int i = 0; i < Hours.Length; i++)
                {
                    if (Hours[i] > 0.99)
                    {
                        Hours[i] = 0.99;
                    }
                    else if (Hours[i] < 0.01)
                    {
                        Hours[i] = 0.01;
                    }
                }
            }
            public void Init()
            {
                if (Days == null)
                {
                    Days = new double[7];
                    for (int i = 0; i < Days.Length ; i++)
                    {
                        Days[i] = 0.5;
                    }
                }
                if (Hours==null )
                {
                    Hours = new double[24];
                    for (int i = 0; i < Hours.Length; i++)
                    {
                        Hours[i] = 0.5;
                    }
                }
                if (NeuroDays  == null)
                {
                    NeuroDays = new double[7];
                }
                if (NeuroHours == null)
                {
                    NeuroHours = new double[24];
                }
                if (CorrectHistory == null)
                {
                    CorrectHistory = new List<CorrectHistoryItem>();
                }
            }

            [Serializable]
            public class CorrectHistoryItem
            {
                public corValues Value;
                public byte day;
                public byte hour;
                public CorrectHistoryItem(corValues lValue, byte lday, byte lhour)
                {
                    Value = lValue;
                    day = lday;
                    hour = lhour;
                }
            }
        }

        string BrainFile = "Brain.db";
        public NeuroHelper()
        {
            Load();
            if (File.Exists(BrainFile))
            {
                NET = new NeuralNW(BrainFile);
            }
            else
            {
                //int[] layers = new int[5];
                //layers[0] = (31 * 31) / 4;
                //layers[1] = (31*31)/2;
                //layers[2] = 31*31;
                //layers[3] = 31 * 5;
                //layers[4] = 31;
                //CreateNW(96, layers);
                //NET.SaveNW(BrainFile);
            }
            

        }

        public void GetNeuroData(ref BaseSign bs)
        {
            double[] X = bs.GetAllIn ();
            double[] Y;
            if (X!=null )
            {
                if (NET!=null)
                {
                    NET.NetOUT(X, out Y);
                    bs.SetOut(Y);
                }
            }
            
            
        }

        public NeuralNW NET;
        // Путь к сети
        String path = "";
        // 1 - идет обучение. 0 - нет
        bool run = false;

        public void CreateNW(int SizeX, int[] Layers)
        {
            NET = new NeuralNW(SizeX, Layers);
            path = "";
        }

        public void LearnAll()
        {
            Form frm = new Form();
            TextBox txtLogs = new TextBox();
            txtLogs.Multiline = true;
            frm.Size = new Size(200, 100);
            frm.BackColor = Color.Black;
            frm.ForeColor = Color.Violet;
            txtLogs.BackColor = Color.Black;
            txtLogs.ForeColor = Color.Violet;
            
            frm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            frm.Text = "Обучение сети...";
            frm.Controls.Add(txtLogs);
            frm.TopMost = true;
            frm.StartPosition = FormStartPosition.CenterScreen;
            txtLogs.Enabled = false;
            txtLogs.Dock = DockStyle.Fill;
            frm.Show();
            if (NET == null)
            {
                txtLogs.AppendText("Не создана сеть!\r\n");
                return;
            }
            if (LearnBook.Count == 0)
            {
                txtLogs.AppendText("Нет данных для обучения!\r\n");
                return;
            }
            txtLogs.AppendText("Запущен процесс обучения\r\n");

            // Очищаем список в обучающей выборке
            txtLogs.AppendText("Загружена обучающая выборка\r\n");

            int currPos = 0;
            double kErr = 1E256;
            double kErrNorm = KErr;
            double kLern = KLern;

            double[] X = new double[NET.GetX];
            double[] Y = new double[NET.GetY];
            double oldkErr = 1;
            double effect = 1;
            //StartLearn.Enabled = false;
            //btnStop.Enabled = true;
            run = true;
            while (kErr > kErrNorm)
            {
                kLern = kLern - kLern / 10;
                kErr = 0;
                txtLogs.Text = "";
                for (currPos = 0; currPos < TmpLearnBook.Count - 1; currPos++)
                {
                    txtLogs.Text = "";
                    double nErr = 0;
                    // Загружаем обучающую пару
                    try
                    {
                        // Загружаем текущий входной файл
                        //currFile = File.ReadAllLines(txtLernFiles.Lines[currPos] + ".in.txt");

                        X = TmpLearnBook[currPos].IN;

                        // Загружаем текущий выходной файл
                        //currFile = File.ReadAllLines(txtLernFiles.Lines[currPos] + ".out.txt");
                        Y = TmpLearnBook[currPos].OUT;
                        //for (int i = 0; i < NET.GetY; i++)
                        //    Y[i] = Convert.ToDouble(currFile[i]);
                    }
                    finally
                    { }
                    nErr=NET.LernNW(X, Y, kLern);
                    kErr += nErr;
                    try
                    {
                        string s = "Прогресс: " + Math.Ceiling(((double)currPos / (double)TmpLearnBook.Count) * 100).ToString();
                        s += "%" + "\r\nОшибка: " + Convert.ToString(Math.Round(kErr, 4));
                        s += "\r\n Эффективность: " + Math.Round(effect, 4).ToString();
                        s+="\r\n Точность: " + (Math.Round( 100-oldkErr,4) ).ToString()+"%";
                        txtLogs.AppendText( s);
                        
                        Application.DoEvents();
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    // Обучаем текущую пару

                    if (!run)
                        return;
                }
                try
                {
                    effect = oldkErr - kErr;
                    if (Math.Abs(effect) < 0.01)
                    {
                        break;
                    }
                    oldkErr = kErr;
                    txtLogs.AppendText("Текущая ошибка: " + Convert.ToString(kErr) + "\r\n");
                }
                catch (Exception)
                {
                    break;
                }
            }
            try
            {
                txtLogs.AppendText("Обучение завершено!\r\n");
                frm.Close();
                frm.Dispose();
            }
            catch (Exception)
            {
            }
            //StartLearn.Enabled = true;
            //btnStop.Enabled = false;
            NET.SaveNW(BrainFile);
            run = false;
        }

        public List<LearnItem> LearnBook = new List<LearnItem>();
        public List<LearnItem> TmpLearnBook = new List<LearnItem>();
        [Serializable]
        public class LearnItem
        {
            public double[] IN;
            public double[] OUT;
        }
        public double KLern = 0.05;
        public double KErr = 0.1;
        public string Status = "";

        public void CreateLearnBook()
        {
            TmpLearnBook.Clear();
            foreach (var item in LearnBook)
            {
                TmpLearnBook.Add(item);
            }
            foreach (var item in Signes)
            {
                if (item.Days != null)
                {
                    if (item.Days.Average() > 0)
                    {
                        if (item.TGI != null)
                        {
                            if (item.TGI.Length > 0)
                            {
                                if (item.Spectr != null)
                                {
                                    LearnItem li = new LearnItem();
                                    li.IN = item.GetAllIn();
                                    li.OUT = item.GetIn();
                                    if (li.IN != null)
                                    {
                                        if (li.OUT != null)
                                        {
                                            TmpLearnBook.Add(li);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AddItemToLearnBook(BaseSign item)
        {
            LearnItem li = new LearnItem();
            li.IN = item.GetAllIn();
            li.OUT = item.GetIn();
            if (li.IN != null)
            {
                if (li.OUT != null)
                {
                    LearnBook.Add(li);
                }
            }
        }

        string LBFileName = "LearnBook.db";
        public void SaveLB()
        {
            //Сохраняем резервную копию
            if (File.Exists(LBFileName))
            {
                File.Copy(LBFileName, LBFileName + DateTime.Now.ToShortDateString() + ".bak", true);
            }
            BinaryFormatter bf = new BinaryFormatter();
            //откроем поток для записи в файл
            using (FileStream fs = new FileStream(LBFileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (GZipStream gz = new GZipStream(fs, CompressionMode.Compress, false))
            {
                bf.Serialize(gz, LearnBook );//сериализация
            }
        }
        public void LoadLB()
        {
            if (!File.Exists(LBFileName))
            {
                //SaveLB();
            }
            else
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream fs = new FileStream(LBFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (GZipStream gz = new GZipStream(fs, CompressionMode.Decompress, false))
                {
                    LearnBook = (List<LearnItem>)bf.Deserialize(gz); //указать тип объекта
                }
            }
        }


       public BaseSign GetBaseSign(string filename)
        {
            string id = Path.GetFileNameWithoutExtension(filename);
           ret1: var t = from nn in Signes where nn.filename == id select nn;
            try
            {
                if (!t.Any())
                {
                    var bs = CreateBaseSign(filename);
                    Signes.Add(bs);
                    return bs;
                }
                var finded = t.First();
                if (finded.filepath != filename)
                {
                    finded.filepath = filename;
                }
                return t.First();
            }
            catch (Exception)
            {
                goto ret1;
            } 
        }
        BaseSign CreateBaseSign(string filename)
        {
            BaseSign bs = new BaseSign();
            bs.filename = Path.GetFileNameWithoutExtension(filename);
            bs.filepath = filename;
            bs.TGI = GenTGI(filename,out bs.Tags);
            return bs;
        }

        string FileName = "Signes.db";
        public void Save()
        {
            int n = 0;
           ret1: try
            {
                n++;
                //Сохраняем резервную копию
                if (File.Exists(FileName))
                {
                    File.Copy(FileName, FileName + DateTime.Now.ToShortDateString() + ".bak", true);
                }
                BinaryFormatter bf = new BinaryFormatter();
                //откроем поток для записи в файл
                using (FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (GZipStream gz = new GZipStream(fs, CompressionMode.Compress, false))
                {
                    bf.Serialize(gz, Signes);//сериализация
                }
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
                if (n<10)
                {
                    goto ret1;
                }
            }
        }
        public void Load()
        {
            if (!File.Exists(FileName))
            {
                Signes = new List<BaseSign>(); //указать тип нового объекта
                Save();
            }
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (GZipStream gz = new GZipStream(fs, CompressionMode.Decompress, false))
            {
                Signes = (List<BaseSign>)bf.Deserialize(gz); //указать тип объекта
            }
        }


        double[] GenTGI(string filename,out MusicID3Tag tags)
        {
            var curfileinfo = GetTags(filename);
            
            if (curfileinfo != null)
            {
                var bp = ts.getBitmapArrayString(curfileinfo.GetAll());
                tags = curfileinfo;
                return bp;
            }
            else
            {
                tags = new MusicID3Tag();
                return new double[96];
            }
        }
        TextSign ts = new TextSign();

        public MusicID3Tag GetTags(string filePath)
        {
            MusicID3Tag tags = null;
            using (FileStream fs = File.OpenRead(filePath))
            {
                if (fs.Length >= 128)
                {
                    MusicID3TagByte tag = new MusicID3TagByte();
                    fs.Seek(-128, SeekOrigin.End);
                    fs.Read(tag.TAGID, 0, tag.TAGID.Length);
                    fs.Read(tag.Title, 0, tag.Title.Length);
                    fs.Read(tag.Artist, 0, tag.Artist.Length);
                    fs.Read(tag.Album, 0, tag.Album.Length);
                    fs.Read(tag.Year, 0, tag.Year.Length);
                    fs.Read(tag.Comment, 0, tag.Comment.Length);
                    fs.Read(tag.Genre, 0, tag.Genre.Length);
                    string theTAGID = Encoding.Default.GetString(tag.TAGID);

                    if (theTAGID.Equals("TAG"))
                    {
                        tags = new MusicID3Tag();
                        tags.Title = Encoding.Default.GetString(tag.Title).Replace("\0", "");
                        tags.Artist = Encoding.Default.GetString(tag.Artist).Replace("\0", "");
                        tags.Album = Encoding.Default.GetString(tag.Album).Replace("\0", "");
                        tags.Year = Encoding.Default.GetString(tag.Year).Replace("\0", "");
                        tags.Comment = Encoding.Default.GetString(tag.Comment).Replace("\0", "");
                        tags.Genre = Encoding.Default.GetString(tag.Genre).Replace("\0", "");
                    }
                }
            }
            return tags;
        }
        [Serializable]
        public class MusicID3TagByte
        {
            public byte[] TAGID = new byte[3];      //  3
            public byte[] Title = new byte[30];     //  30
            public byte[] Artist = new byte[30];    //  30 
            public byte[] Album = new byte[30];     //  30 
            public byte[] Year = new byte[4];       //  4 
            public byte[] Comment = new byte[30];   //  30 
            public byte[] Genre = new byte[1];      //  1
        }
        [Serializable]
        public class MusicID3Tag
        {     //  3
            public string Title = "";    //  30
            public string Artist = "";    //  30 
            public string Album = "";     //  30 
            public string Year = "";      //  4 
            public string Comment = "";   //  30 
            public string Genre = "";      //  1
            public string[] GetAll()
            {
                string[] all = new string[6];
                all[0] = Title;
                all[1] = Artist;
                all[2] = Album;
                all[3] = Year;
                all[4] = Comment;
                all[5] = Genre;
                return all;
            }
        }
    }
}
