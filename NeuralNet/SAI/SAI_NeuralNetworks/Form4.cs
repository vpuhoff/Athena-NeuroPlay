using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace SAI_NeuralNetworks
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        int num = 0;

        void SaveBin(String path, String name, String digit, Bitmap bmp)
        {

            int W = bmp.Width;
            int H = bmp.Height;
            int N = W * H;
            double val = 0;

            String[] mas = new String[N];

            for (int j = 0, k = 0; j < H; j++)
            {
                for (int i = 0; i < W; i++)
                {
                    val = 0.3 * bmp.GetPixel(i, j).R + 0.59 * bmp.GetPixel(i, j).G + 0.11 * bmp.GetPixel(i, j).B;
                    //val = val - 127;
                    //val = val / 255;
                    //val = Math.Round(val, 3);
                    //mas[k++] = val.ToString() ;
                    if (val > 127)
                    {
                        mas[k++] = "-0,5";
                    }
                    else
                    {
                        mas[k++] = "0,5";
                    }
                }
            }
            
            File.WriteAllLines(path + "\\" + name + ".in.txt", mas);

            N = (int)numericUpDown1.Value;
            if (N > 0)
            {
                String[] mas2 = new string[N];

                for (int i = 0; i < N; i++)
                    mas2[i] = textBox2.Text;

                int num2 = Convert.ToInt32(numericUpDown2.Value - 1);
                mas2[num2] = textBox1.Text;


                File.WriteAllLines(path + "\\" + name + ".out.txt", mas2);
            }
        }

        void StartStop(bool flag)
        {
            button3.Enabled = !flag;
            button4.Enabled = flag;
            button5.Enabled = flag;
            button6.Enabled = flag;
            button8.Enabled = flag;

            textBox1.Enabled = !flag;
            textBox2.Enabled = !flag;

            numericUpDown1.Enabled = !flag;
            numericUpDown2.Enabled = flag;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            txtDir.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            txtDestDir.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            numericUpDown2.Maximum = numericUpDown1.Value;
            numericUpDown2.Minimum = (numericUpDown1.Value == 0) ? 0 : 1;

            String strSrc = txtDir.Text;
            FileInfo[] fInfo = new DirectoryInfo(strSrc).GetFiles("*.bmp");
            foreach (FileInfo f in fInfo)
            {
                txtAllFiles.AppendText(f.Name + "\r\n");
            }
            if (txtAllFiles.Lines.Count() == 0)
            {
                MessageBox.Show("Не найдено файлов *.bmp");
                return;
            }

            StartStop(true);
            pictureBox1.ImageLocation = strSrc + "\\" + txtAllFiles.Lines[num];
        }

        private void button8_Click(object sender, EventArgs e)
        {
            StartStop(false);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (num >= txtAllFiles.Lines.Count() - 1)
                return;
            String str = txtDir.Text + "\\" + txtAllFiles.Lines[num];
            pictureBox1.ImageLocation = str;

            Bitmap bmp = new Bitmap(str);

            SaveBin(txtDestDir.Text, txtAllFiles.Lines[num], Convert.ToString(numericUpDown2.Value), bmp);

            str = txtDir.Text + "\\" + txtAllFiles.Lines[++num];
            pictureBox1.ImageLocation = str;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            num--;
            if (num < 0)
                num = 0;

            String str = txtDir.Text + "\\" + txtAllFiles.Lines[num];
            pictureBox1.ImageLocation = str;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            num++;
            if (num > txtAllFiles.Lines.Count() - 2)
                num = txtAllFiles.Lines.Count() - 2;

            String str = txtDir.Text + "\\" + txtAllFiles.Lines[num];
            pictureBox1.ImageLocation = str;
        }
    }
}
