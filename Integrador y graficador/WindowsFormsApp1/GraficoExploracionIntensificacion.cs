using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OptimizacionFolding
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using OxyPlot;
    using OxyPlot.Series;
    using OxyPlot.Axes;
    
    public partial class Form3 : Form
    {
        PlotModel pm = new PlotModel
        {
            PlotType = PlotType.Cartesian,
            Background = OxyColors.White
        };

        public int grafico_maximas_iteraciones;
        public int grafico_iteracion_inicial;
        public int grafico_iteracion_final;
        public int grafico_repeticion_actual;
        public int grafico_iteracion_actual;
        public int cantidad_dimensiones;
        public int cantidad_proteinas;
        public Form1.Grafico_de_una_proteina[] datos_de_grafico;
        public Form1.Valores_de_FO[,] datos_de_grafico_FO;

        public float[] Div_por_iteracion = new float[Form1.MAX_GENERACIONES];
        public float[] XPL_porcent = new float[Form1.MAX_GENERACIONES];
        public float[] XPT_porcent = new float[Form1.MAX_GENERACIONES];
        public float[] incremento_decremento = new float[Form1.MAX_GENERACIONES];
        public float maxima_div;



        public Form3(Form1.Grafico_de_una_proteina[] datos_de_grafico_1, 
                    Form1.Valores_de_FO[,] datos_de_grafico_FO_1,
                    int cantidad_de_repeticiones, int cantidad_de_iteraciones, int dimensiones, int maximas_proteinas)
        {
            InitializeComponent();
            cantidad_dimensiones = dimensiones;
            cantidad_proteinas = maximas_proteinas; 
            datos_de_grafico = datos_de_grafico_1;
            datos_de_grafico_FO = datos_de_grafico_FO_1;
            numericUpDown11.Value = 1;
            numericUpDown10.Value = 1;
            numericUpDown2.Value = cantidad_de_iteraciones;
            numericUpDown11.Minimum = 1;
            numericUpDown10.Minimum = 1;
            numericUpDown2.Minimum = 1;
            numericUpDown11.Maximum = cantidad_de_repeticiones;
            numericUpDown10.Maximum = cantidad_de_iteraciones;
            numericUpDown2.Maximum = cantidad_de_iteraciones;
            grafico_maximas_iteraciones = cantidad_de_iteraciones;
            numericUpDown11.Refresh();
            numericUpDown10.Refresh();
            numericUpDown2.Refresh();
            grafico_repeticion_actual = (int)numericUpDown11.Value;
            grafico_iteracion_inicial = (int)numericUpDown10.Value;
            grafico_iteracion_final = (int)numericUpDown2.Value;
            hace_grafico();
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            PdfExporter pdfExporter;
            System.IO.FileStream stream;
            saveFileDialog1.InitialDirectory = Application.StartupPath;
            saveFileDialog1.Filter = "pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            string archivo_de_grafico;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                archivo_de_grafico = saveFileDialog1.FileName;
                archivo_de_grafico = archivo_de_grafico.Substring(0, archivo_de_grafico.IndexOf("."));
                archivo_de_grafico += ".pdf";
                pdfExporter = new PdfExporter { Width = 600, Height = 400 };
                stream = File.Create(archivo_de_grafico);
                pdfExporter.Export(pm, stream);
            }
            else
                return;
        }

        void hace_grafico()
        {
            double [] vector = new double[Form1.MAX_PROTEINAS];
            double [] div = new double[Form1.MAX_AMINOACIDOS];
            double mediana;
            float minimoy = float.MaxValue;
            float maximoy = float.MinValue;
            float valor;
            float promedio_XPL = 0.0f;
            float promedio_XPT = 0.0f; 
            maxima_div = float.MinValue;
            grafico_repeticion_actual = (int)numericUpDown11.Value;
            grafico_iteracion_inicial = (int)numericUpDown10.Value;
            grafico_iteracion_final = (int)numericUpDown2.Value;
            for (int ite = grafico_iteracion_inicial; ite <= grafico_iteracion_final; ite++)
            {
                Div_por_iteracion[ite] = 0.0f;
                for (int dim = 1; dim <= cantidad_dimensiones; dim++)
                {
                    div[dim] = 0.0;
                    for (int prot = 1; prot <= cantidad_proteinas; prot++)
                        vector[prot] = datos_de_grafico[prot].posiciones_de_aminoacidos_por_proteina_repeticion_generacion_y_aminoacido[grafico_repeticion_actual, ite, dim];
                    mediana = calcula_mediana(vector, cantidad_proteinas);
                    for (int prot = 1; prot <= cantidad_proteinas; prot++)
                        div[dim] += Math.Abs(mediana - datos_de_grafico[prot].posiciones_de_aminoacidos_por_proteina_repeticion_generacion_y_aminoacido[grafico_repeticion_actual, ite, dim]);
                }
                for (int dim = 1; dim <= cantidad_dimensiones; dim++)
                    div[dim] /= cantidad_proteinas;
                for (int dim = 1; dim <= cantidad_dimensiones; dim++)
                    Div_por_iteracion[ite] += (float)div[dim];
                Div_por_iteracion[ite] /= cantidad_dimensiones;
                if (Div_por_iteracion[ite] > maxima_div)
                    maxima_div = Div_por_iteracion[ite];
            }
            incremento_decremento[0] = -2.0f;
            for (int ite = grafico_iteracion_inicial; ite <= grafico_iteracion_final; ite++)
            {
                XPL_porcent[ite] = Div_por_iteracion[ite] / maxima_div * 100.0f;
                XPT_porcent[ite] = (float)Math.Abs(Div_por_iteracion[ite] - maxima_div) / maxima_div * 100.0f;
                valor = XPL_porcent[ite] - XPT_porcent[ite];
                if  (valor >= 0.0f)
                    incremento_decremento[ite] = incremento_decremento[ite - 1] + 2.0f;
                else
                    incremento_decremento[ite] = incremento_decremento[ite - 1] - 2.0f;
            }
            for (int ite = grafico_iteracion_inicial; ite <= grafico_iteracion_final; ite++)
                if (incremento_decremento[ite] < 0.0f)
                    incremento_decremento[ite] = 0.0f;
            for (int ite = grafico_iteracion_inicial; ite <= grafico_iteracion_final; ite++)
            {
                valor = incremento_decremento[ite];
                if (minimoy > valor)
                    minimoy = valor;
                if (maximoy < valor)
                    maximoy = valor;
            }
            for (int ite = grafico_iteracion_inicial; ite <= grafico_iteracion_final; ite++)
            {
                valor = (maximoy - incremento_decremento[ite]) * 100.0f / (maximoy - minimoy);
                incremento_decremento[ite] = 100.0f - valor;
            }
            for (int ite = grafico_iteracion_inicial; ite <= grafico_iteracion_final; ite++)
            {
                valor = XPL_porcent[ite];
                promedio_XPL += XPL_porcent[ite]; 
                if (minimoy > valor)
                    minimoy = valor;
                if (maximoy < valor)
                    maximoy = valor;
                valor = XPT_porcent[ite];
                promedio_XPT += XPT_porcent[ite];
                if (minimoy > valor)
                    minimoy = valor;
                if (maximoy < valor)
                    maximoy = valor;
                valor = incremento_decremento[ite];
                if (minimoy > valor)
                    minimoy = valor;
                if (maximoy < valor)
                    maximoy = valor;
            }
            promedio_XPL /= (grafico_iteracion_final - grafico_iteracion_inicial);
            promedio_XPT /= (grafico_iteracion_final - grafico_iteracion_inicial);
            var XPL_curve = new LineSeries
                                {
                                        Title = "Exploration Average="+promedio_XPL.ToString("0.0"),         
                                        StrokeThickness = 2,
                                        LineJoin = LineJoin.Round,
                                        LineStyle = LineStyle.Solid,
                                        Color = OxyColor.FromArgb(255, 255, 0, 0),
                                        MarkerType = MarkerType.None,
                                        
            };
            for (int ite = grafico_iteracion_inicial; ite <= grafico_iteracion_final; ite++)
                XPL_curve.Points.Add(new DataPoint(ite, XPL_porcent[ite]));
            pm.Series.Add(XPL_curve);
            var XPT_curve = new LineSeries
                            {
                                Title = "Exploitation Average=" + promedio_XPT.ToString("0.0"),
                                StrokeThickness = 2,
                                LineStyle = LineStyle.Solid,
                                LineJoin = LineJoin.Round,
                                Color = OxyColor.FromArgb(255, 0, 0, 255),
                                MarkerType = MarkerType.None,
                                
                            };
            for (int ite = grafico_iteracion_inicial; ite <= grafico_iteracion_final; ite++)
                XPT_curve.Points.Add(new DataPoint(ite, XPT_porcent[ite]));
            pm.Series.Add(XPT_curve);
            var increment_decrement_curve = new LineSeries
            {
                Title = "Incremental-Decremental",
                StrokeThickness = 2,
                LineStyle = LineStyle.Solid,
                LineJoin = LineJoin.Round,
                Color = OxyColor.FromArgb(255, 0, 255, 0),
                MarkerType = MarkerType.None,

            };
            for (int ite = grafico_iteracion_inicial; ite <= grafico_iteracion_final; ite++)
                increment_decrement_curve.Points.Add(new DataPoint(ite, incremento_decremento[ite]));
            pm.Series.Add(increment_decrement_curve);
            if (pm.Series.Count == 0)
            {
                MessageBox.Show("Error verifique sus datos. No hay nada que graficar ni proteínas ni mejor valor",
                                         "Error en datos para graficar",
                                         MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                         MessageBoxOptions.RtlReading); ;
                return;
            }
            pm.Axes.Add(new LinearAxis
                        {
                            Position = AxisPosition.Bottom,
                            AbsoluteMinimum = grafico_iteracion_inicial - grafico_iteracion_final * 0.01,
                            AbsoluteMaximum = grafico_iteracion_final + grafico_iteracion_final * 0.01,
                            Title = "Iterations",
                            Key = "Generaciones",
                            UseSuperExponentialFormat = false,
                            MajorGridlineStyle = LineStyle.None,
                            TickStyle = OxyPlot.Axes.TickStyle.Outside
                        }
            );

            pm.Axes.Add(new LinearAxis
                        {
                            Position = AxisPosition.Left,
                            AbsoluteMinimum = minimoy,
                            AbsoluteMaximum = maximoy,
                            Title = "Percentage",
                            Key = "energyAxis",
                            UseSuperExponentialFormat = false,
                            MajorGridlineStyle = LineStyle.None,
                            TickStyle = OxyPlot.Axes.TickStyle.Outside
                        }
            );

            pm.PlotMargins = new OxyPlot.OxyThickness(80, 50, 30, 50);
            pm.InvalidatePlot(true);
            plot1.InvalidatePlot(true);
            plot1.OnModelChanged();
            plot1.Model = pm;
            plot1.Refresh();
        }

        public void QuickSort_vector_double(ref double[] arr, int start, int end)
        {
            int i;
            if (start < end)
            {
                i = Partition_vector_proteinas(ref arr, start, end);

                QuickSort_vector_double(ref arr, start, i - 1);
                QuickSort_vector_double(ref arr, i + 1, end);
            }
        }

        private int Partition_vector_proteinas(ref double[] arr, int start, int end)
        {
            double temp;
            double p = arr[end];
            int i = start - 1;

            for (int j = start; j <= end - 1; j++)
            {
                if (arr[j] <= p)
                {
                    i++;
                    temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                }
            }

            temp = arr[i + 1];
            arr[i + 1] = arr[end];
            arr[end] = temp;
            return i + 1;
        }

        double calcula_mediana(double[] vector, int cantidad)
        {
            QuickSort_vector_double(ref vector, 1, cantidad);
            int medio = cantidad / 2;
            if (cantidad % 2 == 0)
                return (vector[medio] + vector[medio + 1]) / 2.0;
            else
                return vector[medio + 1];
        }

        private void button4_Click(object sender, EventArgs e)
        {
            grafico_repeticion_actual = (int)numericUpDown11.Value;
            grafico_iteracion_inicial = (int)numericUpDown10.Value;
            grafico_iteracion_final = (int)numericUpDown2.Value;
            if (grafico_iteracion_inicial >= grafico_iteracion_final)
            {
                MessageBox.Show("Error verifique sus datos. La iteración inicial tiene que ser menor que la iteración final",
                                         "Error en datos para graficar",
                                         MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                         MessageBoxOptions.RtlReading); ;
                return;
            }
            plot1.Model.InvalidatePlot(true);
            plot1.Model.Series.Clear();
            plot1.Model.Axes.Clear();
            hace_grafico();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(this.numericUpDown11, "Seleccionar la repetición que se desea graficar.");
            toolTip1.SetToolTip(this.label12, "Seleccionar la repetición que se desea graficar.");

            toolTip1.SetToolTip(this.numericUpDown10, "Seleccionar iteración inicial para graficar.");
            toolTip1.SetToolTip(this.label11, "Seleccionar iteración inicial para graficar.");

            toolTip1.SetToolTip(this.numericUpDown2, "Seleccionar iteración final para graficar.");
            toolTip1.SetToolTip(this.label2, "Seleccionar iteración final para graficar.");

            toolTip1.SetToolTip(this.button4, "Realiza el gráfico según las opciones seleccionadas de proteínas, iteración\n" +
                                              "y repetición.");

            toolTip1.SetToolTip(this.button2, "Exporta el gráfico actual a un archivo tipo .pdf.");

                        
        }
    }
}
