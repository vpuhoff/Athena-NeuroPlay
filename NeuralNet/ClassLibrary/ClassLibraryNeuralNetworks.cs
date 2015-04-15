///////////////////////////////////
///////////////////////////////////
///	Created by dr.kernel	///
///	kernel-zone.ru		///
///	Nosov Alexander		///
///////////////////////////////////
///////////////////////////////////

using System;
using System.IO;
using System.Runtime.InteropServices;


namespace ClassLibraryNeuralNetworks
{

    // Структура дря разбиения переменных типа int и double на байты
    [StructLayout(LayoutKind.Explicit)]
    internal class DataToByte
    {
        [FieldOffset(0)]
        public double vDouble;

        [FieldOffset(0)]
        public int vInt;

        [FieldOffset(0)]
        public byte b1;
        [FieldOffset(1)]
        public byte b2;
        [FieldOffset(2)]
        public byte b3;
        [FieldOffset(3)]
        public byte b4;
        [FieldOffset(4)]
        public byte b5;
        [FieldOffset(5)]
        public byte b6;
        [FieldOffset(6)]
        public byte b7;
        [FieldOffset(7)]
        public byte b8;
    }

    // Класс - слой нейросети
    public class LayerNW
    {
        double[,] Weights;
        int cX, cY;

        // Заполняем веса случайными числами
        public void GenerateWeights()
        {
            Random rnd = new Random();
            for (int i = 0; i < cX; i++)
            {
                for (int j = 0; j < cY; j++)
                {
                    Weights[i, j] = rnd.NextDouble() - 0.5;
                }
            }
        }

        // Выделяет память под веса
        protected void GiveMemory()
        {
            Weights = new double[cX, cY];
        }

        // Конструктор с параметрами. передается количество входных и выходных нейронов
        public LayerNW(int countX, int countY)
        {
            cX = countX;
            cY = countY;
            GiveMemory();
        }

        public int countX
        {
            get { return cX; }
        }

        public int countY
        {
            get { return cY; }
        }

        public double this[int row, int col]
        {
            get { return Weights[row, col]; }
            set { Weights[row, col] = value; }
        }

    }

    // Класс - нейронная сеть
    public class NeuralNW
    {
        LayerNW[] Layers;
        int countLayers = 0, countX, countY;
        double[][] NETOUT;  // NETOUT[countLayers + 1][]
        double[][] DELTA;   // NETOUT[countLayers    ][]

        // Конструкторы
        /* Создает полносвязанную сеть из 1 слоя. 
           sizeX - размерность вектора входных параметров
           sizeY - размерность вектора выходных параметров */
        public NeuralNW(int sizeX, int sizeY)
        {
            countLayers = 1;
            Layers = new LayerNW[countLayers];
            Layers[0] = new LayerNW(sizeX, sizeY);
            Layers[0].GenerateWeights();
        }

        /* Создает полносвязанную сеть из n слоев. 
           sizeX - размерность вектора входных параметров
           layers - массив слоев. Значение элементов массива - количество нейронов в слое               
         */
        public NeuralNW(int sizeX, params int[] layers)
        {
            countLayers = layers.Length;
            countX = sizeX;
            countY = layers[layers.Length - 1];
            // Размерность выходов нейронов и Дельты
            NETOUT = new double[countLayers + 1][];
            NETOUT[0] = new double[sizeX];
            DELTA = new double[countLayers][];

            this.Layers = new LayerNW[countLayers];

            int countY1, countX1 = sizeX;
            // Устанавливаем размерность слоям и заполняем слоя случайнымичислами
            for (int i = 0; i < countLayers; i++)
            {
                countY1 = layers[i];

                NETOUT[i + 1] = new double[countY1];
                DELTA[i] = new double[countY1];

                this.Layers[i] = new LayerNW(countX1, countY1);
                this.Layers[i].GenerateWeights();
                countX1 = countY1;
            }
        }

        // Открывает НС
        public NeuralNW(String FileName)
        {
            OpenNW(FileName);
        }

        // Открывает НС
        public void OpenNW(String FileName)
        {
            byte[] binNW = File.ReadAllBytes(FileName);

            int k = 0;
            // Извлекаем количество слоев из массива
            countLayers = ReadFromArrayInt(binNW, ref k);
            Layers = new LayerNW[countLayers];

            // Извлекаем размерность слоев
            int CountY1=0, CountX1 = ReadFromArrayInt(binNW, ref k);
            // Размерность входа
            countX = CountX1;
            // Выделяемпамять под выходы нейронов и дельта
            NETOUT = new double[countLayers + 1][];
            NETOUT[0] = new double[CountX1];
            DELTA = new double[countLayers][];

            for (int i = 0; i < countLayers; i++)
            {
                CountY1 = ReadFromArrayInt(binNW, ref k);
                Layers[i] = new LayerNW(CountX1, CountY1);
                CountX1 = CountY1;

                // Выделяем память
                NETOUT[i + 1] = new double[CountY1];
                DELTA[i] = new double[CountY1];
            }
            // Размерность выхода
            countY = CountY1;
            // Извлекаем и записываем сами веса
            for (int r = 0; r < countLayers; r++)
                for (int p = 0; p < Layers[r].countX; p++)
                    for (int q = 0; q < Layers[r].countY; q++)
                    {
                        Layers[r][p, q] = ReadFromArrayDouble(binNW, ref k);
                    }
        }

        // Сохраняет НС
        public void SaveNW(String FileName)
        {
            // размер сети в байтах
            int sizeNW = GetSizeNW();
            byte[] binNW = new byte[sizeNW];

            int k = 0;
            // Записываем размерности слоев в массив байтов
            WriteInArray(binNW, ref k, countLayers);
            if (countLayers <= 0)
                return;

            WriteInArray(binNW, ref k, Layers[0].countX);
            for (int i = 0; i < countLayers; i++)
                WriteInArray(binNW, ref k, Layers[i].countY);

            // Зпаисвыаем сами веса
            for (int r = 0; r < countLayers; r++)
                for (int p = 0; p < Layers[r].countX; p++)
                    for (int q = 0; q < Layers[r].countY; q++)
                    {
                        WriteInArray(binNW, ref k, Layers[r][p, q]);
                    }


            File.WriteAllBytes(FileName, binNW);
        }

        // Возвращает значение j-го слоя НС
        public void NetOUT(double[] inX, out double[] outY, int jLayer)
        {
            GetOUT(inX, jLayer);
            int N = NETOUT[jLayer].Length;

            outY = new double[N];

            for (int i = 0; i < N; i++)
            {
                outY[i] = NETOUT[jLayer][i];
            }

        }

        // Возвращает значение НС
        public void NetOUT(double[] inX, out double[] outY)
        {
            int j = countLayers;
            NetOUT(inX, out outY, j);
        }

        // Возвращает ошибку (метод наименьших квадратов)
        public double CalcError(double[] X, double[] Y)
        {
            double kErr = 0;
            for (int i = 0; i < Y.Length; i++)
            {
                kErr += Math.Pow(Y[i] - NETOUT[countLayers][i], 2);
            }

            return 0.5 * kErr;
        }

        /* Обучает сеть, изменяя ее весовые коэффициэнты. 
           X, Y - обучающая пара. kLern - скорость обучаемости
           В качестве результата метод возвращает ошибку 0.5(Y-outY)^2 */
        public double LernNW(double[] X, double[] Y, double kLern)
        {
            double O;  // Вход нейрона
            double s;

            // Вычисляем выход сети
            GetOUT(X);

            // Заполняем дельта последнего слоя
            for (int j = 0; j < Layers[countLayers - 1].countY; j++)
            {
                O = NETOUT[countLayers][j];
                DELTA[countLayers - 1][j] = (Y[j] - O) * O * (1 - O);
            }

            // Перебираем все слои начиная споследнего 
            // изменяя веса и вычисляя дельта для скрытого слоя
            for (int k = countLayers - 1; k >= 0; k--)
            {
                // Изменяем веса выходного слоя
                for (int j = 0; j < Layers[k].countY; j++)
                {
                    for (int i = 0; i < Layers[k].countX; i++)
                    {
                        Layers[k][i, j] += kLern * DELTA[k][j] * NETOUT[k][i];
                    }
                }
                if (k > 0)
                {

                    // Вычисляем дельта слоя к-1
                    for (int j = 0; j < Layers[k - 1].countY; j++)
                    {

                        s = 0;
                        for (int i = 0; i < Layers[k].countY; i++)
                        {
                            s += Layers[k][j, i] * DELTA[k][i];
                        }

                        DELTA[k - 1][j] = NETOUT[k][j] * (1 - NETOUT[k][j]) * s;
                    }
                }
            }

            return CalcError(X, Y);
        }

        // Свойства. Возвращает число входов и выходов сети
        public int GetX
        {
            get { return countX; }
        }

        public int GetY
        {
            get { return countY; }
        }

        public int CountLayers
        {
            get { return countLayers; }
        }
        /* Вспомогательные закрытые функции */

        // Возвращает все значения нейронов до lastLayer слоя
        void GetOUT(double[] inX, int lastLayer)
        {
            double s;

            for (int j = 0; j < Layers[0].countX; j++)
                NETOUT[0][j] = inX[j];

            for (int i = 0; i < lastLayer; i++)
            {
                // размерность столбца проходящего через i-й слой
                for (int j = 0; j < Layers[i].countY; j++)
                {
                    s = 0;
                    for (int k = 0; k < Layers[i].countX; k++)
                    {
                        s += Layers[i][k, j] * NETOUT[i][k];
                    }

                    // Вычисляем значение активационной функции
                    s = 1.0 / (1 + Math.Exp(-s));
                    NETOUT[i + 1][j] = 0.998 * s + 0.001;

                }
            }

        }

        // Возвращает все значения нейронов всех слоев
        void GetOUT(double[] inX)
        {
            GetOUT(inX, countLayers);
        }

        // Возвращает размер НС в байтах
        int GetSizeNW()
        {
            int sizeNW = sizeof(int) * (countLayers + 2);
            for (int i = 0; i < countLayers; i++)
            {
                sizeNW += sizeof(double) * Layers[i].countX * Layers[i].countY;
            }
            return sizeNW;
        }

        // Возвращает num-й слой Нейронной сети
        public LayerNW Layer(int num)
        {
            return Layers[num]; 
        }

        // Разбивает переменную типа int на байты и записывает в массив
        void WriteInArray(byte[] mas, ref int pos, int value)
        {
            DataToByte DTB = new DataToByte();
            DTB.vInt = value;
            mas[pos++] = DTB.b1;
            mas[pos++] = DTB.b2;
            mas[pos++] = DTB.b3;
            mas[pos++] = DTB.b4;
        }

        // Разбивает переменную типа int на байты и записывает в массив
        void WriteInArray(byte[] mas, ref int pos, double value)
        {
            DataToByte DTB = new DataToByte();
            DTB.vDouble = value;
            mas[pos++] = DTB.b1;
            mas[pos++] = DTB.b2;
            mas[pos++] = DTB.b3;
            mas[pos++] = DTB.b4;
            mas[pos++] = DTB.b5;
            mas[pos++] = DTB.b6;
            mas[pos++] = DTB.b7;
            mas[pos++] = DTB.b8;
        }

        // Извлекает переменную типа int из 4-х байтов массива
        int ReadFromArrayInt(byte[] mas, ref int pos)
        {
            DataToByte DTB = new DataToByte();
            DTB.b1 = mas[pos++];
            DTB.b2 = mas[pos++];
            DTB.b3 = mas[pos++];
            DTB.b4 = mas[pos++];

            return DTB.vInt;
        }

        // Извлекает переменную типа double из 8-ми байтов массива
        double ReadFromArrayDouble(byte[] mas, ref int pos)
        {
            DataToByte DTB = new DataToByte();
            DTB.b1 = mas[pos++];
            DTB.b2 = mas[pos++];
            DTB.b3 = mas[pos++];
            DTB.b4 = mas[pos++];
            DTB.b5 = mas[pos++];
            DTB.b6 = mas[pos++];
            DTB.b7 = mas[pos++];
            DTB.b8 = mas[pos++];

            return DTB.vDouble;
        }

    }
}
