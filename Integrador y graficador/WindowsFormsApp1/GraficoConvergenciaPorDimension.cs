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
    

    public partial class GraficoConvergenciaPorDimension : Form
    {
        //PlotView pview = new PlotView();
        
        PlotModel pm = new PlotModel
        {
            //Title = "Trigonometric functions",
            //Subtitle = "Example using the FunctionSeries",
            PlotType = PlotType.Cartesian,
            Background = OxyColors.White
        };



        public Form1.Grafico_de_una_proteina [] datos_de_grafico;
        public int grafico_repeticion;
        public int grafico_iteracion_inicio;
        public int grafico_iteracion_final;
        public bool graficar_mejor_valor;
        public int maximas_iteraciones;
        public int maximas_particulas;
        public int maximas_repeticiones;
        public int maximas_dimensiones;
        public int grafico_iteracion_actual;
        public bool terminar_animacion = false;
        public int dimension_eje_X = 1;
        public int dimension_eje_Y = 2;
        float minimox = float.MaxValue;
        float maximox = float.MinValue;
        float minimoz = float.MaxValue;
        float maximoz = float.MinValue;


        public GraficoConvergenciaPorDimension(Form1.Grafico_de_una_proteina[] datos_de_grafico_parametro,
                                                int maximas_repeticiones_parametros, int maximas_iteraciones_parametros,
                                                int maximas_dimensiones_parametros, int maximas_particulas_parametros)
        {
            InitializeComponent();
            maximas_iteraciones = maximas_iteraciones_parametros;
            maximas_particulas = maximas_particulas_parametros;
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
            numericUpDown8.Maximum = maximas_dimensiones;
            numericUpDown7.Maximum = maximas_dimensiones;
            numericUpDown8.Value = 1;
            numericUpDown7.Value = 2;
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
            grafico_iteracion_inicio = 1;
            grafico_iteracion_final = maximas_iteraciones;
            grafico_iteracion_actual = 1;
            label11.Refresh();
            textBox2.Text = "1";
            textBox2.Refresh();
            pm.InvalidatePlot(true);
            minimox = float.MaxValue;
            maximox = float.MinValue;
            float valor;
            for (int dimension = 1; dimension <= maximas_dimensiones; dimension++)
                for (int repeticion = 1; repeticion <= maximas_repeticiones; repeticion++)
                    for (int iteracion = 1; iteracion <= maximas_iteraciones; iteracion++)
                        for (int particula = 1; particula <= maximas_particulas; particula++)
                        {
                            valor = datos_de_grafico[particula].posiciones_de_aminoacidos_por_proteina_repeticion_generacion_y_aminoacido[repeticion, iteracion, dimension];
                            if (minimox > valor)
                                minimox = valor;
                            if (maximox < valor)
                                maximox = valor;
                            valor = datos_de_grafico[particula].valor_de_la_FO_por_proteina_repeticion_y_generacion[grafico_repeticion, grafico_iteracion_actual];
                            if (minimoz > valor)
                                minimoz = valor;
                            if (maximoz < valor)
                                maximoz = valor;
                        }
            Hace_grafico();
        }

        public void Hace_grafico()
        {
            float valor_x;
            float valor_z;
            float valor_y;
            //int color;
            pm.Series.Clear();
            pm.Axes.Clear();
            pm.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = minimox * 1.05,
                Maximum = maximox * 1.05,
                //MinorStep = 1,
                //MajorStep = 1,
                Title = "Dimension " + dimension_eje_X.ToString(),
                Key = "Coordenada",
                UseSuperExponentialFormat = false,
                MajorGridlineStyle = LineStyle.None,
                TickStyle = OxyPlot.Axes.TickStyle.Outside
            });

            pm.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = minimox * 1.05,
                Maximum = maximox * 1.05,
                //MinorStep = 2,
                //MajorStep = 1,
                Title = "Dimension " + dimension_eje_Y.ToString(),
                Key = "energyAxis",
                UseSuperExponentialFormat = false,
                MajorGridlineStyle = LineStyle.None,
                TickStyle = OxyPlot.Axes.TickStyle.Outside
            });
            LineSeries energyCurve;
            for (int particula = 1; particula <= maximas_particulas; particula++)
            {
                valor_x = datos_de_grafico[particula].posiciones_de_aminoacidos_por_proteina_repeticion_generacion_y_aminoacido[grafico_repeticion, grafico_iteracion_actual, dimension_eje_X];
                valor_y = datos_de_grafico[particula].posiciones_de_aminoacidos_por_proteina_repeticion_generacion_y_aminoacido[grafico_repeticion, grafico_iteracion_actual, dimension_eje_Y];
                valor_z = datos_de_grafico[particula].valor_de_la_FO_por_proteina_repeticion_y_generacion[grafico_repeticion, grafico_iteracion_actual];
                if (valor_z <= ((maximoz + minimoz) / 2.0))
                {
                    //color = (int) (255 * Math.Abs(minimoz / valor_z));
                    energyCurve = new LineSeries
                    {
                        Color = OxyColor.FromArgb(255, 255, 0, 0),
                        StrokeThickness = 3,
                        MarkerType = MarkerType.Circle,
                        MarkerSize = 5,
                        MarkerStrokeThickness = 3
                    };
                    energyCurve.Points.Add(new DataPoint(valor_x, valor_y));
                    pm.Series.Add(energyCurve);
                }
                else
                {
                    //color = (int) (255 * Math.Abs(valor_z / maximoz));
                    energyCurve = new LineSeries
                    {
                        Color = OxyColor.FromArgb(255, 0, 0, 255),
                        StrokeThickness = 3,
                        MarkerType = MarkerType.Circle,
                        MarkerSize = 5,
                        MarkerStrokeThickness = 3
                    };
                    energyCurve.Points.Add(new DataPoint(valor_x, valor_y));
                    pm.Series.Add(energyCurve);
                }
                
            }
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
            dimension_eje_X = (int)numericUpDown8.Value;
            dimension_eje_Y = (int)numericUpDown7.Value;
            grafico_iteracion_actual = (int)numericUpDown10.Value;
            grafico_iteracion_final = (int)numericUpDown10.Value;
            grafico_iteracion_final = (int)numericUpDown2.Value;
            textBox2.Text = grafico_iteracion_actual.ToString();
            textBox2.Refresh();
            if (dimension_eje_X == dimension_eje_Y)
            {
                MessageBox.Show("Error verifique sus datos. Las dimensiones a graficar en cada eje deben ser diferentes.",
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
            Hace_grafico();
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
                dimension_eje_X = (int)numericUpDown8.Value;
                dimension_eje_Y = (int)numericUpDown7.Value; 
                plot1.Model.InvalidatePlot(true);
                plot1.Model.Series.Clear();
                plot1.Model.Axes.Clear();
                Hace_grafico();
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

        private void GraficoConvergenciaPorDimension_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(this.numericUpDown11, "Seleccionar la repetición que se desea graficar.");
            toolTip1.SetToolTip(this.label12, "Seleccionar la repetición que se desea graficar.");

            toolTip1.SetToolTip(this.numericUpDown8, "Seleccionar la dimensión para graficar en el eje X.");
            toolTip1.SetToolTip(this.label9, "Seleccionar la dimensión para graficar en el eje X.");

            toolTip1.SetToolTip(this.numericUpDown7, "Seleccionar la dimensión para graficar en el eje Y.");
            toolTip1.SetToolTip(this.label8, "Seleccionar la dimensión para graficar en el eje Y.");

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

            toolTip1.SetToolTip(this.button3, "Realiza una secuencia de los gráficos de convergencia en el proceso de optimización.\n" +
                                              "Se grafica de forma automática desde la iteración inicial hasta la iteración final.");

            toolTip1.SetToolTip(this.textBox2, "Muestra la iteración que se está graficando.");
            toolTip1.SetToolTip(this.label4, "Muestra la iteración que se está graficando.");

            
            grafico_repeticion = (int)numericUpDown11.Value;
            dimension_eje_X = (int)numericUpDown8.Value;
            dimension_eje_Y = (int)numericUpDown7.Value;
            grafico_iteracion_actual = (int)numericUpDown10.Value;
            grafico_iteracion_final = (int)numericUpDown10.Value;
            grafico_iteracion_final = (int)numericUpDown2.Value;
            textBox2.Text = grafico_iteracion_actual.ToString();
            textBox2.Refresh();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            grafico_iteracion_final = (int)numericUpDown2.Value;
        }

       

        

        
        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            grafico_repeticion = (int) numericUpDown11.Value;
            
        }

       
    }
}
