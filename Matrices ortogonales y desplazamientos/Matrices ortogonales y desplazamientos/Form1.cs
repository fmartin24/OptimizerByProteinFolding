using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static alglib;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Matrices_ortogonales_y_desplazamientos
{
    public partial class Form1 : Form
    {

        public const int MAX_FUNCIONES_COMPUESTAS = 21;
        public const int MAX_DIMENSIONS = 2001;
        public static string folderName;
        public static uint cantidad_de_funciones;
        public static uint dimension_inicial;
        public static uint dimension_final;
        public static uint paso_en_dimensiones;
        public static float corrimiento_inicial;
        public static float corrimiento_final;
        public static int minimo_inicial;
        public static int minimo_final;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            folderName = Application.StartupPath.ToString();
            cantidad_de_funciones = (uint)numericUpDown1.Value;
            dimension_inicial = (uint)numericUpDown4.Value;
            dimension_final = (uint)numericUpDown2.Value;
            paso_en_dimensiones = (uint)numericUpDown3.Value;
            corrimiento_inicial = System.Convert.ToSingle(textBox2.Text);
            corrimiento_final = System.Convert.ToSingle(textBox3.Text);
            minimo_inicial = (int)numericUpDown5.Value;
            minimo_final = (int)numericUpDown6.Value;
            toolTip1.SetToolTip(this.numericUpDown1, "Se define aquí la cantidad máxima funciones para problemas\n" +
                    "en los cuales se generan funciones compuestas.");
            toolTip1.SetToolTip(this.label1, "Se define aquí la cantidad máxima funciones para problemas\n" +
                    "en los cuales se generan funciones compuestas.");
            toolTip1.SetToolTip(this.numericUpDown4, "Se define aquí la dimensión mínima de los problemas para generar \n" +
                    "matrices ortogonales y desplazamientos en las variables independientes.");
            toolTip1.SetToolTip(this.label4, "Se define aquí la dimensión mínima de los problemas para generar \n" +
                    "matrices ortogonales y desplazamientos en las variables independientes.");
            toolTip1.SetToolTip(this.numericUpDown2, "Se define aquí la dimensión máxima de los problemas para generar \n" +
                    "matrices ortogonales y desplazamientos en las variables independientes.");
            toolTip1.SetToolTip(this.label2, "Se define aquí la dimensión máxima de los problemas para generar \n" +
                    "matrices ortogonales y desplazamientos en las variables independientes.");
            toolTip1.SetToolTip(this.numericUpDown3, "Se define aquí el incremento en dimensión para generar \n" +
                    "las matrices ortogonales.");
            toolTip1.SetToolTip(this.label3, "Se define aquí el incremento en dimensión para generar \n" +
                    "la matrices ortogonales.");
            toolTip1.SetToolTip(this.textBox2, "Se define aquí el valor mínimo para generar \n" +
                    "los desplazamientos en las variables independientes.");
            toolTip1.SetToolTip(this.label5, "Se define aquí el valor mínimo para generar \n" +
                    "los desplazamientos en las variables independientes.");
            toolTip1.SetToolTip(this.textBox3, "Se define aquí el valor máximo para generar \n" +
                    "los desplazamientos en las variables independientes.");
            toolTip1.SetToolTip(this.label6, "Se define aquí el valor máximo para generar \n" +
                    "los desplazamientos en las variables independientes.");
            toolTip1.SetToolTip(this.numericUpDown5, "Se define aquí el valor mínimo para generar \n" +
                    "los corrimientos en el mínimo de las funciones.");
            toolTip1.SetToolTip(this.label7, "Se define aquí el valor mínimo para generar \n" +
                    "los corrimientos en el mínimo de las funciones.");
            toolTip1.SetToolTip(this.numericUpDown6, "Se define aquí el valor máximo para generar \n" +
                    "los corrimientos en el mínimo de las funciones.");
            toolTip1.SetToolTip(this.label8, "Se define aquí el valor máximo para generar \n" +
                    "los corrimientos en el mínimo de las funciones.");
            toolTip1.SetToolTip(this.button3, "Crear o seleccionar la carpeta en la que se crearán las matrices, \n" +
                    "corrimientos en las variables independientes y desplazamientos de los mínimos.");
            toolTip1.SetToolTip(this.button1, "Generar las matrices ortogonales para funciones compuestas.");
            toolTip1.SetToolTip(this.button4, "Generar los corrimientos en la posición del mínimo de las variables independientes.");
            toolTip1.SetToolTip(this.button5, "Generar los valores del mínimo para cada una de las funciones en las compuestas.");
            toolTip1.SetToolTip(this.button2, "Revisar que las matrices ortogonales creadas son correctas,\n" +
                "para esto en la ventana de texto podemos ver sus determinantes.");
            toolTip1.SetToolTip(this.textBox1, "Se muestra el avance en la creación de los diferentes archivos.\n" +
                "Si se revisan los datos creados aqupi se imprime el determinante de la \n" +
                "matriz ortogonal que debe ser muy cercano a 1 para ser correcto.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double[,] M;
            StreamWriter escritor;
            textBox1.Text = "";
            double determinante;
            string nombre;
            string cad;
            Random aleatorio = new Random((int)DateTime.Now.Ticks);
            Cursor.Current = Cursors.WaitCursor;
            for (uint d = dimension_inicial; d <= dimension_final; d+=paso_en_dimensiones)
            {
                cad = folderName.Substring(folderName.Length - 1);
                if (cad != "\\")
                    folderName += "\\";
                nombre = folderName + "M_D";
                nombre += d.ToString() + ".txt";
                escritor = new StreamWriter(nombre);
                for (int f = 1; f <= cantidad_de_funciones; f++)
                {
                    do
                    {
                        alglib.rmatrixrndorthogonal((int)d, out M);
                        determinante = alglib.rmatrixdet(M);
                    }
                    while (determinante != 1);
                    for (int p = 0; p < d; p++)
                    {
                        for (int l = 0; l < d; l++)
                        {
                            if (M[p, l] > 0)
                                cad = " " + string.Format("{0:0.00000000000000E+000}", M[p, l]);
                            else
                                cad = string.Format("{0:0.00000000000000E+000}", M[p, l]);
                            escritor.Write(cad + " ");
                            escritor.Flush();
                        }
                        escritor.WriteLine();
                    }
                    textBox1.Text += "Funcion= " + f.ToString() + " Dimension= " + d.ToString() + Environment.NewLine;
                    textBox1.Refresh();
                }
                escritor.Close();
            }
            Cursor.Current = Cursors.Default;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StreamReader lector;
            string cad;
            string nombre;
            textBox1.Text = "";
            int dimension;
            int posicion;
            Cursor.Current = Cursors.WaitCursor;
            cad = folderName.Substring(folderName.Length - 1);
            if (cad != "\\")
                folderName += "\\";
            openFileDialog1.InitialDirectory = folderName;
            openFileDialog1.Title = "Seleccione los archivos de matrices ortogonales";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.DefaultExt = "txt";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.ReadOnlyChecked = true;
            openFileDialog1.ShowReadOnly = true;
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in openFileDialog1.FileNames)
                {
                    posicion = file.LastIndexOf("M_D");
                    cad = file.Substring(posicion+3);
                    posicion = cad.LastIndexOf(".");
                    cad = cad.Substring(0, posicion);
                    dimension = System.Convert.ToInt32(cad);
                    lector = new StreamReader(file);
                    for (int f = 1; f <= cantidad_de_funciones; f++)
                    {
                        double[,] M = new double[dimension, dimension];
                        for (uint p = 0; p < dimension; p++)
                        {
                            string s;
                            string subcadena;
                            s = lector.ReadLine().Trim();
                            string pattern = @"[-+]?[0-9]\.[0-9]+[eE][-+][0-9]+";
                            Regex rg = new Regex(pattern);
                            MatchCollection matchednumber = rg.Matches(s);
                            for (int count = 0; count < matchednumber.Count; count++)
                            {
                                subcadena = matchednumber[count].Value;
                                M[p, count] = System.Convert.ToDouble(subcadena);
                            }
                        }
                        double det = alglib.rmatrixdet(M);
                        textBox1.Text += "Funcion= " + f.ToString() + " Dimension= " + dimension.ToString() + " Determinante=" + det.ToString() + Environment.NewLine;
                        textBox1.Refresh();
                    }
                    lector.Close();
                }
            }
            Cursor.Current = Cursors.Default;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = folderName;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
                folderName = folderBrowserDialog1.SelectedPath;

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            cantidad_de_funciones = (uint)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            dimension_final = (uint)numericUpDown2.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            paso_en_dimensiones = (uint)numericUpDown3.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
           dimension_inicial = (uint)numericUpDown4.Value;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            minimo_inicial = (int)numericUpDown5.Value;
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            minimo_final = (int)numericUpDown6.Value;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string nombre;
            string cad;
            StreamWriter escritor;
            nombre = folderName;
            nombre += "oinew_data.txt";
            escritor = new StreamWriter(nombre);
            double valor;
            Random aleatorio = new Random((int)DateTime.Now.Ticks);
            for (int f = 1; f < MAX_FUNCIONES_COMPUESTAS; f++)
            {
                for (int p = 1; p < MAX_DIMENSIONS; p++)
                {
                    valor = corrimiento_inicial + aleatorio.NextDouble() * (corrimiento_final - corrimiento_inicial);
                    if (valor > 0)
                        cad = " " + string.Format("{0:0.00000000000000E+000}", valor);
                    else
                        cad = string.Format("{0:0.00000000000000E+000}", valor);
                    escritor.Write(cad + " ");
                    escritor.Flush();
                }
                escritor.WriteLine();
                textBox1.Text += "Corrimiento de variables en función= " + f.ToString() + Environment.NewLine;
                textBox1.Refresh();
            }
            escritor.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string nombre;
            string cad;
            Random aleatorio = new Random((int)DateTime.Now.Ticks);
            StreamWriter escritor;
            nombre = folderName;
            nombre += "function_offset_data.txt";
            escritor = new StreamWriter(nombre);
            float[] minimos = new float[MAX_FUNCIONES_COMPUESTAS];
            float espacio_minimo = (minimo_final - minimo_inicial) / (MAX_FUNCIONES_COMPUESTAS - 1);
            for (uint f = 1; f < MAX_FUNCIONES_COMPUESTAS; f++)
                minimos[f] = (float)Math.Round(minimo_inicial + (f - 1) * espacio_minimo, 0, MidpointRounding.ToEven);
            int n = MAX_FUNCIONES_COMPUESTAS - 1;
            for (int i = n - 1; i > 0; i--)
            {
                int j = aleatorio.Next(1, i);
                float temp = minimos[i];
                minimos[i] = minimos[j];
                minimos[j] = temp;
            }
            for (int f = 1; f < MAX_FUNCIONES_COMPUESTAS; f++)
            {
                if (minimos[f] > 0)
                    cad = " " + string.Format("{0:0.0}", minimos[f]);
                else
                    cad = string.Format("{0:0.0}", minimos[f]);
                escritor.Write(cad + " ");
                escritor.Flush();
                textBox1.Text += "Valores de mínimos en función= " + f.ToString() + Environment.NewLine;
                textBox1.Refresh();
            }
            escritor.WriteLine();
            escritor.Close();
        }
    }
}
