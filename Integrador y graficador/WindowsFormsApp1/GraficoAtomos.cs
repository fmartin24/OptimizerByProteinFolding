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
using System.Diagnostics;

namespace OptimizacionFolding
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using OxyPlot;
    using OxyPlot.Series;
    using OxyPlot.Axes;
    using OxyPlot.WindowsForms;
    

    public partial class GraficoAtomos : Form
    {
        PlotView pview = new PlotView();
        
        PlotModel pm = new PlotModel
        {
            //Title = "Trigonometric functions",
            //Subtitle = "Example using the FunctionSeries",
            PlotType = PlotType.Cartesian,
            Background = OxyColors.White
        };



        public Form1.Grafico_de_una_proteina [] datos_de_grafico;
        public int grafico_repeticion;
        public int grafico_atomo_inicio;
        public int grafico_atomo_final;
        public int grafico_iteracion_inicio;
        public int grafico_iteracion_final;
        public bool graficar_mejor_valor;
        public int grafico_coordenada_uno_puntos;
        public int grafico_coordenada_dos_puntos;
        public int maximas_iteraciones;
        public int maximos_atomos;
        public int maximas_repeticiones;
        public int maximas_dimensiones;
        public int grafico_iteracion_actual;
        public bool terminar_animacion = false;


        public GraficoAtomos(Form1.Grafico_de_una_proteina[] datos_de_grafico_parametro,
                            int maximas_repeticiones_parametros, int maximas_dimensiones_parametros,
                            int maximas_iteraciones_parametros, int maximos_atomos_parametros)
        {
            InitializeComponent();
            maximas_iteraciones = maximas_iteraciones_parametros;
            maximos_atomos = maximos_atomos_parametros;
            maximas_repeticiones = maximas_repeticiones_parametros;
            maximas_dimensiones = maximas_dimensiones_parametros;
            numericUpDown2.Maximum = maximas_iteraciones;
            numericUpDown10.Maximum = maximas_iteraciones;
            numericUpDown2.Minimum = 1;
            numericUpDown10.Minimum = 1;
            numericUpDown2.Value = maximas_iteraciones;
            numericUpDown10.Value = 1;
            numericUpDown10.Refresh();
            numericUpDown2.Refresh();
            numericUpDown8.Maximum = maximos_atomos;
            numericUpDown7.Maximum = maximos_atomos;
            numericUpDown8.Value = 1;
            numericUpDown7.Value = maximos_atomos;
            numericUpDown8.Minimum = 1;
            numericUpDown7.Minimum = 1;
            numericUpDown8.Refresh();   
            numericUpDown7.Refresh();
            numericUpDown11.Value = 1;
            numericUpDown11.Minimum = 1;
            numericUpDown11.Maximum = maximas_repeticiones;
            numericUpDown11.Refresh();
            datos_de_grafico = datos_de_grafico_parametro;
            grafico_repeticion = 1;
            grafico_atomo_inicio = 1;
            grafico_atomo_final = maximos_atomos_parametros;
            grafico_iteracion_inicio = 1;
            grafico_iteracion_final = maximas_iteraciones;
            grafico_iteracion_actual = 1;
            label11.Refresh();
            textBox2.Text = "1";
            textBox2.Refresh();
            pm.InvalidatePlot(true);
            hace_grafico();
        }

        public void hace_grafico()
        {
            float minimoy = float.MaxValue;
            float maximoy = float.MinValue;
            for (int atomo = grafico_atomo_inicio; atomo <= grafico_atomo_final; atomo++)
            {
                var energyCurve = new LineSeries
                {
                    //Title = datos_de_grafico[atomo].nombre_proteina,
                    //Color = OxyColor.FromUInt32(0xff9e6c3c),
                    StrokeThickness = 3,
                    //MarkerStroke = OxyColor.FromUInt32(0xff9e6c3c),
                    //MarkerFill = OxyColor.FromUInt32(0xff9e6c3c),
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 5,
                    MarkerStrokeThickness = 3
                };
                for (int d = 1; d <= maximas_dimensiones; d++)
                {
                    float valor = datos_de_grafico[atomo].posiciones_de_aminoacidos_por_proteina_repeticion_generacion_y_aminoacido[grafico_repeticion, grafico_iteracion_actual, d];
                    if (minimoy > valor)
                        minimoy = valor;
                    if (maximoy < valor)
                        maximoy = valor;
                    energyCurve.Points.Add(new DataPoint(d, valor));
                }
                pm.Series.Add(energyCurve);
            }

            pm.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = maximas_dimensiones + 1,
                MinorStep = 1,
                MajorStep = 1,
                Title = "Aminoacid",
                Key = "Coordenada",
                UseSuperExponentialFormat = false,
                MajorGridlineStyle = LineStyle.None,
                TickStyle = OxyPlot.Axes.TickStyle.Outside
            });

            pm.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                AbsoluteMinimum = minimoy,
                AbsoluteMaximum = maximoy,
                //MinorStep = 2,
                //MajorStep = 1,
                Title = "Position",
                Key = "energyAxis",
                UseSuperExponentialFormat = true,
                MajorGridlineStyle = LineStyle.None,
                TickStyle = OxyPlot.Axes.TickStyle.Outside
            });

            pm.PlotMargins = new OxyPlot.OxyThickness(80, 50, 30, 50);
            pm.InvalidatePlot(true);
            plot1.InvalidatePlot(true);
            plot1.OnModelChanged();
            plot1.Model = pm;
            plot1.Refresh();
        }

       

        private void button1_Click(object sender, EventArgs e)
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

        private void button2_Click(object sender, EventArgs e)
        {
            grafico_repeticion = (int)numericUpDown11.Value;
            grafico_atomo_inicio = (int)numericUpDown8.Value;
            grafico_atomo_final = (int)numericUpDown7.Value;
            grafico_iteracion_actual = (int)numericUpDown10.Value;
            grafico_iteracion_final = (int)numericUpDown10.Value;
            grafico_iteracion_final = (int)numericUpDown2.Value;
            textBox2.Text = grafico_iteracion_actual.ToString();
            textBox2.Refresh();
            plot1.Model.InvalidatePlot(true);
            plot1.Model.Series.Clear();
            plot1.Model.Axes.Clear();
            if (grafico_atomo_inicio > grafico_atomo_final)
            {
                MessageBox.Show("Error verifique sus datos. La proteina inicial no puede ser mayor que la proteína final.",
                                         "Error en datos para graficar",
                                         MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                         MessageBoxOptions.RtlReading); ;
                return;
            }
            if (grafico_iteracion_inicio > grafico_iteracion_final)
            {
                MessageBox.Show("Error verifique sus datos. La iteración inicial no puede ser mayor que la iteración final.",
                                         "Error en datos para graficar",
                                         MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                         MessageBoxOptions.RtlReading); ;
                return;
            }
            hace_grafico();
        }



        private void button3_Click(object sender, EventArgs e)
        {
            Stopwatch stopWatch_por_repeticion = new Stopwatch();
            TimeSpan ts;
            terminar_animacion = false;
            stopWatch_por_repeticion.Reset();
            stopWatch_por_repeticion.Start();
            grafico_iteracion_actual = grafico_iteracion_inicio;
            textBox2.Text = grafico_iteracion_actual.ToString();
            textBox2.Refresh();
            do
            {
                grafico_repeticion = (int)numericUpDown11.Value;
                grafico_atomo_inicio = (int)numericUpDown8.Value;
                grafico_atomo_final = (int)numericUpDown7.Value;
                plot1.Model.InvalidatePlot(true);
                plot1.Model.Series.Clear();
                plot1.Model.Axes.Clear();
                hace_grafico();
                stopWatch_por_repeticion.Reset();
                stopWatch_por_repeticion.Start();
                do
                {
                    ts = stopWatch_por_repeticion.Elapsed;
                }
                while (ts.TotalMilliseconds < (int)numericUpDown1.Value);
                grafico_iteracion_actual++;
                textBox2.Text = grafico_iteracion_actual.ToString();
                textBox2.Refresh();
            }
            while (grafico_iteracion_actual < grafico_iteracion_final);
        }

        private void GraficoAtomos_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(this.numericUpDown11, "Seleccionar la repetición que se desea graficar.");
            toolTip1.SetToolTip(this.label12, "Seleccionar la repetición que se desea graficar.");

            toolTip1.SetToolTip(this.numericUpDown8, "Seleccionar proteína inicial para graficar.");
            toolTip1.SetToolTip(this.label9, "Seleccionar proteína inicial para graficar.");

            toolTip1.SetToolTip(this.numericUpDown7, "Seleccionar proteína final para graficar.");
            toolTip1.SetToolTip(this.label8, "Seleccionar proteína final para graficar.");

            toolTip1.SetToolTip(this.numericUpDown10, "Seleccionar iteración inicial para graficar.");
            toolTip1.SetToolTip(this.label11, "Seleccionar iteración inicial para graficar.");

            toolTip1.SetToolTip(this.numericUpDown2, "Seleccionar iteración final para graficar.");
            toolTip1.SetToolTip(this.label2, "Seleccionar iteración final para graficar.");

            toolTip1.SetToolTip(this.numericUpDown1, "Seleccionar la velocidad con la que se muestran las diferentes \n" +
                                                     "gráficas si usted selecciona la opción de animar.");
            toolTip1.SetToolTip(this.label1, "Seleccionar la velocidad con la que se muestran las diferentes \n" +
                                                     "gráficas si usted selecciona la opción de animar.");

            toolTip1.SetToolTip(this.button2, "Realiza el gráfico según las opciones seleccionadas de proteínas, iteración\n" +
                                              "y repetición.");

            toolTip1.SetToolTip(this.button1, "Exporta el gráfico actual a un archivo tipo .pdf.");

            toolTip1.SetToolTip(this.button3, "Realiza una secuencia de los gráficos de las proteínas en el proceso de optimización.\n" +
                                              "Se grafica de forma automática desde la iteración inicial hasta la iteración final. Las" +
                                              "proteínas mostradas están comprendidas entre la proteína inicial y final seleccionada.");

            toolTip1.SetToolTip(this.textBox2, "Muestra la iteración que se está graficando.");
            toolTip1.SetToolTip(this.label4, "Muestra la iteración que se está graficando.");
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            grafico_iteracion_final = (int)numericUpDown2.Value;
        }

        
    }
}
