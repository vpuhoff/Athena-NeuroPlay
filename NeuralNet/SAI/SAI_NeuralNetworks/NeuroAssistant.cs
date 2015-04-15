using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClassLibraryNeuralNetworks;

namespace SAI_NeuralNetworks
{
    public partial class NeuroAssistant : UserControl
    {
        // Собственно сама нейросеть
        NeuralNW NET;
        // Путь к сети
        String path = "";
        // 1 - идет обучение. 0 - нет
        bool run = false;

        public void CreateNW(int SizeX, int[] Layers)
        {
            NET = new NeuralNW(SizeX, Layers);
            path = "";
            txtLogs.AppendText("Создана полносвязная сеть:\r\n");
            txtLogs.AppendText("Число входов: " + Convert.ToString(SizeX) + "\r\n");
            txtLogs.AppendText("Число выходов: " + Convert.ToString(Layers[Layers.Count() - 1]) + "\r\n");
            txtLogs.AppendText("Число скрытых слоёв: " + Convert.ToString(Layers.Count() - 1) + "\r\n");

            for (int i = 0; i < Layers.Count() - 1; i++)
            {
                txtLogs.AppendText("Нейронов в " + Convert.ToString(i + 1) + " скрытом слое: "
                                                    + Convert.ToString(Layers[i]) + "\r\n");

            }
        }

        public NeuroAssistant()
        {
            InitializeComponent();
            //int[] layers = new int[5];
            //layers[0] = (31 * 608) / 8;
            //layers[1] = (31 * 608) / 4;
            //layers[2] = 31 * 608;
            //layers[3] = 31 * 5;
            //layers[4] = 31;
            //CreateNW(608, layers);
        }

        private void NeuroAssistant_Load(object sender, EventArgs e)
        {

        }

        public List<LearnItem> LearnBook = new List<LearnItem>();
        public class LearnItem
        {
           public double[] IN;
           public double[] OUT;
        }
        private void StartLearn_Click(object sender, EventArgs e)
        {
            if (NET == null)
            {
                txtLogs.AppendText("Не создана сеть!\r\n");
                return;
            }
            if (LearnBook.Count==0)
            {
                txtLogs.AppendText("Нет данных для обучения!\r\n");
                return;
            }
            txtLogs.AppendText("Запущен процесс обучения\r\n");

            // Очищаем список в обучающей выборке
            txtLogs.AppendText("Загружена обучающая выборка\r\n");

            int currPos = 0;
            double kErr = 1E256;
            double kErrNorm = Convert.ToDouble(txtKErr.Text);
            double kLern = Convert.ToDouble(txtKLern.Text);

            double[] X = new double[NET.GetX];
            double[] Y = new double[NET.GetY];

            StartLearn.Enabled = false;
            btnStop.Enabled = true;
            run = true;
            while (kErr > kErrNorm)
            {
                kErr = 0;
                for (currPos = 0; currPos < LearnBook.Count  - 1; currPos++)
                {
                    // Загружаем обучающую пару
                    try
                    {
                        // Загружаем текущий входной файл
                        //currFile = File.ReadAllLines(txtLernFiles.Lines[currPos] + ".in.txt");

                        X = LearnBook[currPos].IN;

                        // Загружаем текущий выходной файл
                        //currFile = File.ReadAllLines(txtLernFiles.Lines[currPos] + ".out.txt");
                        Y = LearnBook[currPos].OUT ;
                        //for (int i = 0; i < NET.GetY; i++)
                        //    Y[i] = Convert.ToDouble(currFile[i]);
                    }
                    finally
                    { }

                    // Обучаем текущую пару
                    kErr += NET.LernNW(X, Y, kLern);

                    Application.DoEvents();

                    if (!run)
                        return;
                }
                txtLogs.AppendText("Текущая ошибка: " + Convert.ToString(kErr) + "\r\n");
            }
            txtLogs.AppendText("Обучение завершено!\r\n");

            StartLearn.Enabled = true;
            btnStop.Enabled = false;
            run = false;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StartLearn.Enabled = true;
            btnStop.Enabled = false;
            run = false;
            txtLogs.AppendText("Обучение остановлено пользователем\r\n");
        }
    }
}
