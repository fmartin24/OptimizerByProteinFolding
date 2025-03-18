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
    
    public partial class Form2 : Form
    {
        PlotModel pm = new PlotModel
        {
            //Title = "Trigonometric functions",
            //Subtitle = "Example using the FunctionSeries",
            PlotType = PlotType.Cartesian,
            Background = OxyColors.White
        };

        public int grafico_proteina_inicio;
        public int grafico_proteina_final;
        public int grafico_iteracion_inicial;
        public int grafico_iteracion_final;
        public int grafico_repeticion_actual;
        public int grafico_proteina_actual;
        public int grafico_generacion_actual;
        public bool graficar_mejor_valor;
        public Form1.Grafico_de_una_proteina[] datos_de_grafico;
        public Form1.Valores_de_FO[,] datos_de_grafico_FO;
        public Form2(Form1.Grafico_de_una_proteina[] datos_de_grafico_1, 
                    Form1.Valores_de_FO[,] datos_de_grafico_FO_1,
                    int cantidad_de_repeticiones, int cantidad_de_proteinas, 
                    int cantidad_de_iteraciones, bool graficar_mejor_valor)
        {
            InitializeComponent();
            datos_de_grafico = datos_de_grafico_1;
            datos_de_grafico_FO = datos_de_grafico_FO_1;
            numericUpDown11.Value = 1;
            numericUpDown8.Value = 1;
            numericUpDown7.Value = cantidad_de_proteinas;
            numericUpDown10.Value = 1;
            numericUpDown2.Value = cantidad_de_iteraciones;
            numericUpDown11.Minimum = 1;
            numericUpDown8.Minimum = 1;
            numericUpDown7.Minimum = 1;
            numericUpDown10.Minimum = 1;
            numericUpDown2.Minimum = 1;
            numericUpDown11.Maximum = cantidad_de_repeticiones;
            numericUpDown8.Maximum = cantidad_de_proteinas;
            numericUpDown7.Maximum = cantidad_de_proteinas; 
            numericUpDown10.Maximum = cantidad_de_iteraciones;
            numericUpDown2.Maximum = cantidad_de_iteraciones;
            numericUpDown11.Refresh();
            numericUpDown8.Refresh();
            numericUpDown7.Refresh();
            numericUpDown10.Refresh();
            numericUpDown2.Refresh();
            checkBox1.Checked = graficar_mejor_valor;
            grafico_repeticion_actual = (int)numericUpDown11.Value;
            grafico_iteracion_inicial = (int)numericUpDown10.Value;
            grafico_iteracion_final = (int)numericUpDown2.Value;
            grafico_proteina_inicio = (int)numericUpDown8.Value;
            grafico_proteina_final = (int)numericUpDown7.Value;
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
            checkBox1.Checked = graficar_mejor_valor;
            grafico_repeticion_actual = (int)numericUpDown11.Value;
            grafico_iteracion_inicial = (int)numericUpDown10.Value;
            grafico_iteracion_final = (int)numericUpDown2.Value;
            grafico_proteina_inicio = (int)numericUpDown8.Value;
            grafico_proteina_final = (int)numericUpDown7.Value;
            float minimoy = float.MaxValue;
            float maximoy = float.MinValue;
            if (!checkBox2.Checked)
                for (int fun = grafico_proteina_inicio; fun <= grafico_proteina_final; fun++)
                {
                    var energyCurve = new LineSeries
                    {
                        Title = datos_de_grafico[fun].nombre_proteina,
                        StrokeThickness = 3,
                        MarkerType = MarkerType.Circle,
                        MarkerSize = 1,
                        MarkerStrokeThickness = 1
                    };
                    for (int ite = grafico_iteracion_inicial; ite <= grafico_iteracion_final; ite++)
                    {
                        float valor = datos_de_grafico[fun].valor_de_la_FO_por_proteina_repeticion_y_generacion[grafico_repeticion_actual, ite];
                        if (valor == 43000)
                            valor = 43000;
                        if (minimoy > valor)
                            minimoy = valor;
                        if (maximoy < valor)
                            maximoy = valor;
                        energyCurve.Points.Add(new DataPoint(ite, valor));
                    }
                    pm.Series.Add(energyCurve);
                }
            if (graficar_mejor_valor)
            {
                var energyCurve = new LineSeries
                {
                    Title = "Best Objetive Funtion Value",
                    Color = OxyColor.FromArgb(0, 0, 0, 0),
                    MarkerFill = OxyColor.FromArgb(255, 255, 0, 0),
                    MarkerStroke = OxyColor.FromArgb(255, 255, 0, 0),
                    MarkerType = MarkerType.Star,
                };
                for (int ite = grafico_iteracion_inicial; ite <= grafico_iteracion_final; ite++)
                {
                    float valor = datos_de_grafico_FO[grafico_repeticion_actual, ite].mejor_valor_de_FO;
                    if (minimoy > valor)
                        minimoy = valor;
                    if (maximoy < valor)
                        maximoy = valor;
                    energyCurve.Points.Add(new DataPoint(ite, valor));
                }
                pm.Series.Add(energyCurve);
            }
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
                //MinorStep = 2,
                //MajorStep = 2,
                Title = "Iterations",
                Key = "Generaciones",
                UseSuperExponentialFormat = false,
                MajorGridlineStyle = LineStyle.None,
                TickStyle = OxyPlot.Axes.TickStyle.Outside
            });

            pm.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                AbsoluteMinimum = minimoy - minimoy * 0.01,
                AbsoluteMaximum = maximoy + maximoy * 0.01,
                //MinorStep = 2,
                //MajorStep = 2,
                Title = "Objetive Funtion Value",
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

        private void button4_Click(object sender, EventArgs e)
        {
            grafico_repeticion_actual = (int)numericUpDown11.Value;
            grafico_proteina_inicio = (int)numericUpDown8.Value;
            grafico_proteina_final = (int)numericUpDown7.Value;
            grafico_iteracion_inicial = (int)numericUpDown10.Value;
            grafico_iteracion_final = (int)numericUpDown2.Value;
            graficar_mejor_valor = checkBox1.Checked;
            if (grafico_proteina_inicio > grafico_proteina_final)
            {
                MessageBox.Show("Error verifique sus datos. La proteina inicial no puede ser mayor que la proteina final",
                                         "Error en datos para graficar",
                                         MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                         MessageBoxOptions.RtlReading); ;
                return;
            }
            if (grafico_iteracion_inicial > grafico_iteracion_final)
            {
                MessageBox.Show("Error verifique sus datos. La generación inicial no puede ser mayor que la generación final",
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

            toolTip1.SetToolTip(this.numericUpDown8, "Seleccionar proteína inicial para graficar.");
            toolTip1.SetToolTip(this.label9, "Seleccionar proteína inicial para graficar.");

            toolTip1.SetToolTip(this.numericUpDown7, "Seleccionar proteína final para graficar.");
            toolTip1.SetToolTip(this.label8, "Seleccionar proteína final para graficar.");

            toolTip1.SetToolTip(this.numericUpDown10, "Seleccionar iteración inicial para graficar.");
            toolTip1.SetToolTip(this.label11, "Seleccionar iteración inicial para graficar.");

            toolTip1.SetToolTip(this.numericUpDown2, "Seleccionar iteración final para graficar.");
            toolTip1.SetToolTip(this.label2, "Seleccionar iteración final para graficar.");

            toolTip1.SetToolTip(this.button4, "Realiza el gráfico según las opciones seleccionadas de proteínas, iteración\n" +
                                              "y repetición.");

            toolTip1.SetToolTip(this.button2, "Exporta el gráfico actual a un archivo tipo .pdf.");

            toolTip1.SetToolTip(this.checkBox1, "En el gráfico de convergencia se dibuja un asterisco rojo con el mejor valor de FO obtenido.");

            toolTip1.SetToolTip(this.checkBox2, "Si está marcado no se dibuja la convergencia de ninguna de la proteínas.");
            
        }

        
    }
}
