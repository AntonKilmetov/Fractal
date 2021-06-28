using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fractal
{
    public partial class Form1 : Form
    {
        private static int sideLength; // длина стороны
        private static int R; // радиус
        private static double p = 0.8; 
        private int[,] field; // поле занятых ячеек
        private Point[,] coordinates; // координаты ячеек
        private List<int> radiusList = new List<int>(); // список радиусов
        private List<int> massList = new List<int>(); // список масс
        static SolidBrush black = new SolidBrush(Color.Black);
        static SolidBrush white = new SolidBrush(Color.White);
        Random rnd = new Random();
        Bitmap bmp;
        Graphics graph;
        public Form1()
        {
            InitializeComponent();
            textBox5.Text = (p * 100).ToString();
            R = 1;
            sideLength = 101;
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graph = Graphics.FromImage(bmp);
            field = new int[sideLength, sideLength];
            coordinates = new Point[sideLength, sideLength];
            field[sideLength / 2, sideLength / 2] = 1;
            radiusList.Add(R);
            MakeCoord();
            MassCalculate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Draw();
        }
        /// <summary>
        /// Координаты ячеек
        /// </summary>
        private void MakeCoord()
        {
            var a = sideLength / 2;
            for (int i = -sideLength / 2; i < sideLength / 2 + 1; i++)
                for (int j = -sideLength / 2; j < sideLength / 2 + 1; j++)
                    coordinates[sideLength / 2 + i, sideLength / 2 + j] = new Point(i, j);
        }
        /// <summary>
        /// Отрисрвка кластера
        /// </summary>
        private void Draw()
        {
            var length = pictureBox1.Width / sideLength;
            for (int i = 0; i < sideLength; i++)
            {
                for (int j = 0; j < sideLength; j++)
                {                   
                    if (field[i, j] == 1)
                        graph.FillRectangle(black, j * length, i * length, length, length);
                    else
                        graph.FillRectangle(white, j * length, i * length, length, length);
                }
            }
            pictureBox1.Image = bmp;         
        }
        /// <summary>
        /// Занять ячейку периметра 
        /// </summary>
        private void Method()
        {
            var a = new double[sideLength, sideLength];
            for (int i = 0; i < sideLength; i++)
                for (int j = 0; j < sideLength; j++)
                    a[i, j] = -1.0;
            for (int i = 1; i < sideLength - 1; i++)
            {
                for (int j = 1; j < sideLength - 1; j++)
                {
                    if (field[i, j] == 1 || field[i, j] == -1)
                    {
                        if (field[i + 1, j] == 0)
                            a[i + 1, j] = rnd.NextDouble();
                        if (field[i - 1, j] == 0)
                            a[i - 1, j] = rnd.NextDouble();
                        if (field[i, j + 1] == 0)
                            a[i, j + 1] = rnd.NextDouble();
                        if (field[i, j - 1] == 0)
                            a[i, j - 1] = rnd.NextDouble();
                    }
                }
            }
            var indexes = GetIndexOfMin(a);
            if(rnd.NextDouble() < p)
                field[indexes.Item1, indexes.Item2] = 1;
            else
                field[indexes.Item1, indexes.Item2] = -1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>минимальный элемент матрицы</returns>
        private double Min(double[,] matrix)
        {
            var minValue = double.MaxValue;
            foreach (var item in matrix)
                if (item >= 0 && item < minValue)
                    minValue = item;
            return minValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>Индекс минимального элемента</returns>
        private Tuple<int,int> GetIndexOfMin(double[,] matrix)
        {
            var min = Min(matrix);
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    if (Math.Abs(matrix[i, j] - min) < 1E-10)
                        return Tuple.Create(i, j);
            return null;
        }
        /// <summary>
        /// Занять ячейку периметра и отрисовать новый кластер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            var n = int.Parse(textBox1.Text);
            for (int i = 0; i < n; i++)
            {
                Method();
                Draw();
            }
        }
        /// <summary>
        /// Увеличить радиус
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            R += 1;
            textBox2.Text = textBox3.Text = R.ToString();
            radiusList.Add(R);
            MassCalculate();
        }
        /// <summary>
        /// Рассчитать массу в радиусе R от начальной ячейки
        /// </summary>
        private void MassCalculate()
        {
            var mass = 0;
            for (int i = 0; i < sideLength; i++)
                for (int j = 0; j < sideLength; j++)
                    if ((coordinates[i, j].X * coordinates[i, j].X + coordinates[i, j].Y * coordinates[i, j].Y) <= (R * R)
                        && field[i, j] == 1)
                        mass++;
            massList.Add(mass);
            textBox4.Text = mass.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var radius = radiusList
                .Select(x => Math.Log(x).ToString())
                .ToArray();
            var mass = massList
                .Select(x => Math.Log(x).ToString())
                .ToArray();
            File.WriteAllLines(@"C:\anton1\СomputationalPhysics\data.txt", radius);

            File.WriteAllLines(@"C:\anton1\СomputationalPhysics\data2.txt", mass);

            var a = File.ReadAllLines(@"C:\anton1\СomputationalPhysics\data.txt")
                .Select(x => x.Replace(",", "."))
                .ToArray();
            File.WriteAllLines(@"C:\anton1\СomputationalPhysics\data.txt", a);

            var b = File.ReadAllLines(@"C:\anton1\СomputationalPhysics\data2.txt")
                .Select(x => x.Replace(",", "."))
                .ToArray();
            File.WriteAllLines(@"C:\anton1\СomputationalPhysics\data2.txt", b);
        }
    }
}
