using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laba5_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void OKButton1_Click(object sender, EventArgs e) //ввод количества наборов через TrackBar
        {    
            int count = CountTrackBar.Value;
            GroupBox[] groupBoxes = new GroupBox[] { groupBox1, groupBox2, groupBox3, groupBox4, groupBox5, groupBox6, groupBox7};
            CountGroupBox.Visible = false; 
            for (int i=0; i < count; i++)
            {
                groupBoxes[i].Visible = true;
            }
            DataGroupBox.Visible = true;
        }

        private void OKButton2_Click(object sender, EventArgs e) 
        {
            ResultListBox.Items.Clear();
            DateTime localDate = DateTime.Now;
            string timenow = localDate.ToString();
            string pathInfo = @"myProgram.log";
            string name = "Лабораторная работа №5, Вариант 17";
            string func = "y/log2(x)";
            System.IO.File.WriteAllText(pathInfo, string.Empty); //очищаем файл myProgram.log
            System.IO.StreamWriter fileInfo = new System.IO.StreamWriter(pathInfo);
            fileInfo.WriteLine("Название программы: " + name);
            fileInfo.WriteLine("Время начала работы: " + timenow);
            fileInfo.WriteLine("Функция: " + func);

            string pathError = @"myErrors.log";
            System.IO.File.WriteAllText(pathError, string.Empty); //очищаем файл myError.log
            System.IO.StreamWriter fileError = new System.IO.StreamWriter(pathError);
            //массив textboxов для считывания данных
            TextBox[,] textBoxes = new TextBox[6,7] { { X0textBox1, X0textBox2, X0textBox3, X0textBox4, X0textBox5, X0textBox6, X0textBox7 },
                                                      { XktextBox1, XktextBox2, XktextBox3, XktextBox4, XktextBox5, XktextBox6, XktextBox7 },
                                                      { HxtextBox1, HxtextBox2, HxtextBox3, HxtextBox4, HxtextBox5, HxtextBox6, HxtextBox7 },
                                                      { Y0textBox1, Y0textBox2, Y0textBox3, Y0textBox4, Y0textBox5, Y0textBox6, Y0textBox7 },
                                                      { YktextBox1, YktextBox2, YktextBox3, YktextBox4, YktextBox5, YktextBox6, YktextBox7 },
                                                      { HytextBox1, HytextBox2, HytextBox3, HytextBox4, HytextBox5, HytextBox6, HytextBox7 } }; 
            for (int j = 0; j < CountTrackBar.Value; j++)
            {
                //имя файла для записи результатов
                string filename = System.IO.Path.Combine(Environment.CurrentDirectory, "G" + string.Format("{0:d4}", j+1) + ".dat");  
                try
                {
                    double[] x = Data(Convert.ToDouble(textBoxes[0, j].Text), Convert.ToDouble(textBoxes[1, j].Text), Convert.ToDouble(textBoxes[2, j].Text));
                    double[] y = Data(Convert.ToDouble(textBoxes[3, j].Text), Convert.ToDouble(textBoxes[4, j].Text), Convert.ToDouble(textBoxes[5, j].Text));
                    double gr;
                    System.IO.File.WriteAllText(filename, string.Empty); //очищаем файл G####.dat
                    using (System.IO.StreamWriter GFile = new System.IO.StreamWriter(filename, true))
                    {
                        GFile.WriteLine(func); //записываем функцию
                        int pointCount = 0;
                        string firstRow = "x/y".PadRight(8);
                        string datarow = ""; 
                        for (int i = 0; i < x.Length; i++)
                        {
                            datarow = (x[i]).ToString("##0.0##").PadRight(8); //значение функции или столбик х
                            for (int k = 0; k < y.Length; k++)
                            {
                                double xi = x[i]; //запоминаем х и у
                                double yj = y[j];
                                string funcResult = "";
                                gr = G(x[i], y[k]); //рассчитываем функцию
                                firstRow += (y[k]).ToString("##0.0##").PadRight(8); //шапка у
                                if (double.IsNaN(gr) || double.IsInfinity(gr)) // если значение функии NaN или Inf
                                {
                                    funcResult = ("Nan").PadRight(7);
                                    fileError.WriteLine(filename);
                                    fileError.WriteLine(func);
                                    fileError.WriteLine("x={0} y={1}", xi, yj);
                                    fileError.WriteLine("Неопределенное значение");
                                    fileError.WriteLine(" ");
                                }
                                else if (x[i] == 1) //если есть деление на 0
                                {
                                    funcResult = ("Nan").PadRight(7);
                                    fileError.WriteLine(filename);
                                    fileError.WriteLine(func);
                                    fileError.WriteLine("x={0} y={1}", xi, yj);
                                    fileError.WriteLine("Деление на 0");
                                    fileError.WriteLine(" ");
                                }
                                else
                                {
                                    funcResult = gr.ToString("#0.0000").PadRight(7); //записываем результат
                                }
                                //вывод результата в ListBox
                                ResultListBox.Items.Add("G" + string.Format("{0:d4}", j + 1) + ".dat -> " + "(" + xi + ";" + yj + ") = " + funcResult);
                                datarow += funcResult + " ";
                                pointCount++; //количество точек
                            }
                            if (i == 0)
                                GFile.WriteLine(firstRow); //шапка у

                            GFile.WriteLine(datarow); //значение функции
                        }
                        string countInfo = string.Format("Количество точек: {0}", pointCount);
                        GFile.WriteLine(countInfo);
                        fileInfo.WriteLine(filename);
                    }
                }
                catch (FormatException) //если данные не введены или введены неверно
                {
                    fileError.WriteLine(filename);
                    fileError.WriteLine(func);
                    fileError.WriteLine("Неверный формат данных или не введены данные");
                    fileError.WriteLine(" ");
                }
            }
            fileInfo.Close();
            fileError.Close();

            GroupBox[] groupBoxes = new GroupBox[] { groupBox1, groupBox2, groupBox3, groupBox4, groupBox5, groupBox6, groupBox7, DataGroupBox };
            for (int i = 0; i < groupBoxes.Length; i++)
            {
                groupBoxes[i].Visible = false;
            }
            ResultGroupBox.Visible = true;
        }

        private double G(double x, double y)
        {
            double g = y / Math.Abs(Math.Log2(x));
            return g;
        } //функция

        private double[] Data(double z0, double zk, double hz)
        {
            int Nz = Convert.ToInt32((zk - z0) / hz);
            double[] z = new double[Nz+1];
            for (int i = 0; i <= Nz; i++)
            {
                z[i] = z0 + i * hz;
            }
            return z;
        } //расчет точек х и у

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FileButton_Click(object sender, EventArgs e)
        {
            CountGroupBox.Visible = false;
            ResultGroupBox.Visible = true;
            string row = "";
            string pathInfo = @"myProgram.log";
            using (System.IO.StreamReader fileInfo = new System.IO.StreamReader(pathInfo)) //файл для считывания названий G-файлов
            {
                int count = 0;
                while ((row = fileInfo.ReadLine()) != null)
                {
                    if (count > 2) //начинаются названия файлов
                    {
                        try
                        {
                            ResultListBox.Items.Add(row); //записываем название файла
                            System.IO.StreamReader gfile = new System.IO.StreamReader(row);
                            List<string> glines = new List<string>(); //список строк в G-файле
                            while ((row = gfile.ReadLine()) != null)
                            {
                                glines.Add(row); 
                            }
                            gfile.Close();
                            string firstrow = glines[1]; //шапка у
                            string[] dataRow = firstrow.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            string[,] resArray = new string[glines.Count - 1, dataRow.Length + 1]; //массив результатов и столбца х
                            for (int i = 1; i < glines.Count - 1; i++)
                            {
                                string[] resArray2 = glines[i].Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries); //значения функции в i-строке
                                for (int j = 0; j < resArray2.Length; j++)
                                {
                                    //запись в массив каждого значения
                                    if (resArray2[j] == "NaN") { resArray[i, j] = "NaN"; }
                                    else { resArray[i, j] = resArray2[j]; }
                                }
                            }
                            ResultListBox.Items.Add(glines[0]); //записываем саму функцию
                            for (int i = 2; i < resArray.GetLength(0); i++)
                            {
                                for (int j = 1; j < resArray.GetLength(1) - 1; j++)
                                {
                                    //записываем значения функции в каждой точке
                                    ResultListBox.Items.Add("G" + string.Format("{0:d4}", count - 2) + ".dat -> " + "(" + resArray[i, 0] + ";" + resArray[1, j] + ") = " + resArray[i, j]);
                                }
                            }
                            ResultListBox.Items.Add(glines[glines.Count-1]); //записываем количество точек
                        }
                        catch (Exception) //если ошибка при считывании файла
                        {
                            ResultListBox.Items.Add("Error");
                        }
                    }
                    count++; //строка в myProgram.log
                }
            }
        }
    }
}
