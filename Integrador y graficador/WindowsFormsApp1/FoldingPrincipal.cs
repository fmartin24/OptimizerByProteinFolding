using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using OxyPlot.Reporting;

namespace OptimizacionFolding
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string FORMATO_PROBABILIDAD = "F3";
        public string FORMATO_H_Y_SU_SUMA = "D2";

        public const string FORMATO_POSICIONES = "E1";
        public const string FORMATO_POSICIONES_INTERMEDIO = "F2";
        public const string FORMATO_VALORES_FUNCION = "E1";

        public const int MAX_PROTEINAS = 1002;
        public const int MAX_AMINOACIDOS = 502;
        public const int MAX_PLIEGUES = 30;
        public const int MAX_REPETICIONES = 52;
        public const int MAX_GENERACIONES = 3000;
        public const int MAX_DATOS_INTEGRADOS = 5001;

        public int maximas_generaciones;
        public int cantidad_de_proteinas;
        public int cantidad_de_repeticiones;
        public int dimensiones;

        public struct grafico_de_una_proteina
        {
            public string nombre_proteina;
            public double [,] valor_de_la_funcion_datos_de_grafico_por_proteina_por_repeticion_y_generacion;
            public double[,,] posiciones_de_aminoacidos_datos_de_grafico_por_proteina_por_repeticion_y_generacion;
        }
        public grafico_de_una_proteina[] datos_de_grafico=new grafico_de_una_proteina[MAX_PROTEINAS];
        public int grafico_cantidad_de_funciones;
        public int grafico_repeticion;
        public int grafico_proteina_inicio;
        public int grafico_proteina_final;
        public int grafico_generacion_inicio;
        public int grafico_generacion_final;
        public bool graficar_mejor_valor;

        private void Form1_Load(object sender, EventArgs e)
        {
            Funciones temp = new Funciones();
            ArrayList nombre = temp.devuelve_funciones_nombre();
            panel9.Visible = true;
            panel16.Visible = true;
        }

        void borra_controles()
        {
            numericUpDown3.Value = 1;
            numericUpDown3.Refresh();
            checkBox5.Checked = false;
            checkBox5.Refresh();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            grafico_repeticion = (int)numericUpDown1.Value;
            grafico_proteina_inicio = (int)numericUpDown6.Value;
            grafico_proteina_final = (int)numericUpDown5.Value;
            grafico_generacion_inicio = (int)numericUpDown2.Value;
            grafico_generacion_final = (int)numericUpDown3.Value;
            graficar_mejor_valor = checkBox5.Checked;
            if (grafico_proteina_inicio > grafico_proteina_final)
            {
                MessageBox.Show("Error verifique sus datos. La proteina inicial no puede ser mayor que la proteina final",
                                         "Error en datos para graficar",
                                         MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                         MessageBoxOptions.RtlReading); ;
                return;
            }
            if (grafico_generacion_inicio > grafico_generacion_final)
            {
                MessageBox.Show("Error verifique sus datos. La generación inicial no puede ser mayor que la generación final",
                                         "Error en datos para graficar",
                                         MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                         MessageBoxOptions.RtlReading); ;
                return;
            }
            Form2 f2 = new Form2(datos_de_grafico, grafico_cantidad_de_funciones,
                                 grafico_repeticion, grafico_proteina_inicio, grafico_proteina_final,
                                 grafico_generacion_inicio, grafico_generacion_final, graficar_mejor_valor, cantidad_de_proteinas+1);
            f2.ShowDialog();
        }
        public struct datos_que_se_leen
        {
            public string archivo;
            public string linea;
            public double valor_leido;
        }

        public struct datos_para_integrar
        {
            public string linea_de_comandos;
            public datos_que_se_leen [] valores_de_funcion_objetivo;
            public datos_que_se_leen [] tiempo_de_ejecucion;
            public double min_valor_funcion;
            public double max_valor_funcion;
            public double desviacion_estandar_valor_funcion;
            public double promedio_del_valor_de_la_funcion;
            public double min_tiempo;
            public double max_tiempo;
            public double desviacion_estandar_tiempo;
            public double promedio_del_tiempo;
            public int cantidad_de_repeticiones;
        }

        

        private void button7_Click(object sender, EventArgs e)
        {
            long linea_del_archivo;
            panel17.Visible = true;
            uint cantidad_datos_integrados;
            cantidad_datos_integrados = 0;
            int escrituras_hechas = 0;
            openFileDialog2.InitialDirectory = Application.StartupPath;
            openFileDialog2.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog2.FilterIndex = 1;
            openFileDialog2.RestoreDirectory = true;
            string nombre_archivo_leer_1;
            string archivo_de_salida_csv;
            StreamWriter escribe;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
                nombre_archivo_leer_1 = openFileDialog2.FileNames[0];
            else
            {
                panel17.Visible = false;
                return;
            }
            saveFileDialog1.InitialDirectory = Application.StartupPath;
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                archivo_de_salida_csv = saveFileDialog1.FileName;
            else
                return;
            this.Cursor = Cursors.WaitCursor;
            escribe = new StreamWriter(archivo_de_salida_csv);
            escribe.AutoFlush = true;
            escribe.WriteLine("Promedio de la FO, Desv. Standart FO, Minimo de la FO, Maximo de la FO, Promedio del tiempo, Desv. Standart del Tiempo, Minimo del Tiempo, Maximo del tiempo, Cantidad de repeticiones, Linea de comandos");
            this.Cursor = Cursors.WaitCursor;
            datos_para_integrar[] datos = new datos_para_integrar[MAX_DATOS_INTEGRADOS];
            for (int i = 1; i < MAX_DATOS_INTEGRADOS; i++)
            {
                datos[i].linea_de_comandos = "";
                datos[i].valores_de_funcion_objetivo = new datos_que_se_leen[MAX_REPETICIONES];
                datos[i].tiempo_de_ejecucion = new datos_que_se_leen[MAX_REPETICIONES];
                datos[i].min_valor_funcion = double.MaxValue;
                datos[i].max_valor_funcion = double.MinValue;
                datos[i].desviacion_estandar_valor_funcion = 0.0D;
                datos[i].promedio_del_valor_de_la_funcion = 0.0D;
                datos[i].promedio_del_tiempo = 0.0D;
                datos[i].min_tiempo = double.MaxValue;
                datos[i].max_tiempo = double.MinValue;
                datos[i].desviacion_estandar_tiempo = 0.0D;
                datos[i].cantidad_de_repeticiones = 0;
            }
            StreamReader lee;
            string cadena;
            int repeticion = 0;
            bool no_ha_terminado = false;
            foreach (string nombre_archivo_leer in openFileDialog2.FileNames)
            {
                cadena=nombre_archivo_leer.Substring(nombre_archivo_leer.LastIndexOf("\\")+1);
                textBox57.Text = cadena;
                textBox57.Refresh();
                lee = new StreamReader(nombre_archivo_leer);
                linea_del_archivo = 0;
                do
                {
                    if (!no_ha_terminado)
                    {
                        cadena = lee.ReadLine();
                        linea_del_archivo++;
                    }
                    else
                        no_ha_terminado = false;
                    if (cadena == "")
                        continue;
                    if (cadena.StartsWith("Linea de comandos:"))
                    {
                        cantidad_datos_integrados++;
                        if (cantidad_datos_integrados+1 == MAX_DATOS_INTEGRADOS)
                        {
                            escrituras_hechas++;
                            for (int i = 1; i <= cantidad_datos_integrados-1; i++)
                            {
                                datos[i].promedio_del_tiempo /= datos[i].cantidad_de_repeticiones;
                                datos[i].promedio_del_valor_de_la_funcion /= datos[i].cantidad_de_repeticiones;
                                for (int j = 1; j <= datos[i].cantidad_de_repeticiones; j++)
                                {
                                    datos[i].desviacion_estandar_valor_funcion += Math.Pow(datos[i].valores_de_funcion_objetivo[j].valor_leido
                                                                                                                   - datos[i].promedio_del_valor_de_la_funcion, 2.0);
                                    datos[i].desviacion_estandar_tiempo += Math.Pow(datos[i].tiempo_de_ejecucion[j].valor_leido
                                                                                                                   - datos[i].promedio_del_tiempo, 2.0);
                                }
                                datos[i].desviacion_estandar_valor_funcion = Math.Sqrt(datos[i].desviacion_estandar_valor_funcion / datos[i].cantidad_de_repeticiones);
                                datos[i].desviacion_estandar_tiempo = Math.Sqrt(datos[i].desviacion_estandar_tiempo / datos[i].cantidad_de_repeticiones);
                            }
                            for (int i = 1; i <= cantidad_datos_integrados-1; i++)
                            {
                                if (datos[i].promedio_del_valor_de_la_funcion == 0 &&
                                    datos[i].valores_de_funcion_objetivo[1].archivo == null)
                                    continue;
                                escribe.Write(datos[i].promedio_del_valor_de_la_funcion.ToString() + ",");
                                escribe.Write(datos[i].desviacion_estandar_valor_funcion.ToString() + ",");
                                escribe.Write(datos[i].min_valor_funcion.ToString() + ",");
                                escribe.Write(datos[i].max_valor_funcion.ToString() + ",");
                                escribe.Write(datos[i].promedio_del_tiempo.ToString() + ",");
                                escribe.Write(datos[i].desviacion_estandar_tiempo.ToString() + ",");
                                escribe.Write(datos[i].min_tiempo + ",");
                                escribe.Write(datos[i].max_tiempo + ",");
                                escribe.Write(datos[i].cantidad_de_repeticiones + ",");
                                escribe.WriteLine(datos[i].linea_de_comandos);
                                escribe.Flush();
                            }
                            for (int i = 1; i < MAX_DATOS_INTEGRADOS; i++)
                            {
                                datos[i].linea_de_comandos = "";
                                datos[i].valores_de_funcion_objetivo = new datos_que_se_leen[MAX_REPETICIONES];
                                datos[i].tiempo_de_ejecucion = new datos_que_se_leen[MAX_REPETICIONES];
                                datos[i].min_valor_funcion = double.MaxValue;
                                datos[i].max_valor_funcion = double.MinValue;
                                datos[i].desviacion_estandar_valor_funcion = 0.0D;
                                datos[i].promedio_del_valor_de_la_funcion = 0.0D;
                                datos[i].promedio_del_tiempo = 0.0D;
                                datos[i].min_tiempo = double.MaxValue;
                                datos[i].max_tiempo = double.MinValue;
                                datos[i].desviacion_estandar_tiempo = 0.0D;
                                datos[i].cantidad_de_repeticiones = 0;
                            }
                            cantidad_datos_integrados = 0;
                            no_ha_terminado = true;
                            continue;
                        }
                        textBox58.Text = (escrituras_hechas * MAX_DATOS_INTEGRADOS + cantidad_datos_integrados).ToString();
                        textBox58.Refresh();
                        cadena = cadena.Substring(cadena.IndexOf(":") + 1).Trim();
                        datos[cantidad_datos_integrados].linea_de_comandos = cadena;
                        continue;
                    }
                    if (cadena.StartsWith("Repeticion:"))
                    {
                        cadena = cadena.Substring(cadena.IndexOf(":") + 1).Trim();
                        datos[cantidad_datos_integrados].cantidad_de_repeticiones = System.Convert.ToInt32(cadena);
                        repeticion = System.Convert.ToInt32(cadena);
                        continue;
                    }
                    if (cadena.StartsWith("Mejor valor obtenido:"))
                    {
                        cadena = cadena.Substring(cadena.IndexOf(":") + 1).Trim();
                        datos[cantidad_datos_integrados].valores_de_funcion_objetivo[repeticion].valor_leido = System.Convert.ToDouble(cadena);
                        datos[cantidad_datos_integrados].valores_de_funcion_objetivo[repeticion].linea = linea_del_archivo.ToString();
                        datos[cantidad_datos_integrados].valores_de_funcion_objetivo[repeticion].archivo = nombre_archivo_leer;
                        datos[cantidad_datos_integrados].promedio_del_valor_de_la_funcion += datos[cantidad_datos_integrados].valores_de_funcion_objetivo[repeticion].valor_leido;
                        if (datos[cantidad_datos_integrados].min_valor_funcion > datos[cantidad_datos_integrados].valores_de_funcion_objetivo[repeticion].valor_leido)
                            datos[cantidad_datos_integrados].min_valor_funcion = datos[cantidad_datos_integrados].valores_de_funcion_objetivo[repeticion].valor_leido;
                        if (datos[cantidad_datos_integrados].max_valor_funcion < datos[cantidad_datos_integrados].valores_de_funcion_objetivo[repeticion].valor_leido)
                            datos[cantidad_datos_integrados].max_valor_funcion = datos[cantidad_datos_integrados].valores_de_funcion_objetivo[repeticion].valor_leido;
                        continue;
                    }
                    if (cadena.StartsWith("Tiempo de ejecucion:"))
                    {
                        cadena = cadena.Substring(cadena.IndexOf(":") + 1).Trim();
                        cadena = cadena.Substring(0, cadena.IndexOf("segundos")).Trim();
                        datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].valor_leido = System.Convert.ToDouble(cadena);
                        datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].linea = linea_del_archivo.ToString();
                        datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].archivo = nombre_archivo_leer;
                        datos[cantidad_datos_integrados].promedio_del_tiempo += datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].valor_leido;
                        if (datos[cantidad_datos_integrados].min_tiempo > datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].valor_leido)
                            datos[cantidad_datos_integrados].min_tiempo = datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].valor_leido;
                        if (datos[cantidad_datos_integrados].max_tiempo < datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].valor_leido)
                            datos[cantidad_datos_integrados].max_tiempo = datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].valor_leido;
                        continue;
                    }
                }
                while (!lee.EndOfStream);
                lee.Close();
            }
            for (int i = 1; i <= cantidad_datos_integrados; i++)
            {
                datos[i].promedio_del_tiempo /= datos[i].cantidad_de_repeticiones;
                datos[i].promedio_del_valor_de_la_funcion /= datos[i].cantidad_de_repeticiones;
                for (int j = 1; j <= datos[i].cantidad_de_repeticiones; j++)
                {
                    datos[i].desviacion_estandar_valor_funcion += Math.Pow(datos[i].valores_de_funcion_objetivo[j].valor_leido
                                                                                                   - datos[i].promedio_del_valor_de_la_funcion, 2.0);
                    datos[i].desviacion_estandar_tiempo += Math.Pow(datos[i].tiempo_de_ejecucion[j].valor_leido
                                                                                                   - datos[i].promedio_del_tiempo, 2.0);
                }
                datos[i].desviacion_estandar_valor_funcion = Math.Sqrt(datos[i].desviacion_estandar_valor_funcion / datos[i].cantidad_de_repeticiones);
                datos[i].desviacion_estandar_tiempo = Math.Sqrt(datos[i].desviacion_estandar_tiempo / datos[i].cantidad_de_repeticiones);
            }
            for (int i = 1; i <= cantidad_datos_integrados; i++)
            {
                if (datos[i].promedio_del_valor_de_la_funcion == 0 &&
                    datos[i].valores_de_funcion_objetivo[1].archivo == null)
                    continue;
                escribe.Write(datos[i].promedio_del_valor_de_la_funcion.ToString() + ",");
                escribe.Write(datos[i].desviacion_estandar_valor_funcion.ToString() + ",");
                escribe.Write(datos[i].min_valor_funcion.ToString() + ",");
                escribe.Write(datos[i].max_valor_funcion.ToString() + ",");
                escribe.Write(datos[i].promedio_del_tiempo.ToString() + ",");
                escribe.Write(datos[i].desviacion_estandar_tiempo.ToString() + ",");
                escribe.Write(datos[i].min_tiempo + ",");
                escribe.Write(datos[i].max_tiempo + ",");
                escribe.Write(datos[i].cantidad_de_repeticiones + ",");
                escribe.WriteLine(datos[i].linea_de_comandos);
                escribe.Flush();
            }
            escribe.Close();
            panel17.Visible = false;
            this.Cursor = Cursors.Default;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int grafico_repeticion_puntos = (int)numericUpDown20.Value;
            int grafico_atomo_inicio_puntos = (int)numericUpDown17.Value;
            int grafico_atomo_final_puntos = (int)numericUpDown16.Value;
            int grafico_generacion_inicio_puntos = (int)numericUpDown19.Value;
            int grafico_generacion_final_puntos = (int)numericUpDown18.Value;
            if (grafico_atomo_inicio_puntos > grafico_atomo_final_puntos)
            {
                MessageBox.Show("Error verifique sus datos. La proteina inicial no puede ser mayor que la proteína final.",
                                         "Error en datos para graficar",
                                         MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                         MessageBoxOptions.RtlReading); ;
                return;
            }
            if (grafico_generacion_inicio_puntos > grafico_generacion_final_puntos)
            {
                MessageBox.Show("Error verifique sus datos. La generación inicial no puede ser mayor que la generación final.",
                                         "Error en datos para graficar",
                                         MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                         MessageBoxOptions.RtlReading); ;
                return;
            }
            
            GraficoAtomos f2 = new GraficoAtomos(datos_de_grafico, grafico_repeticion_puntos,
                                                grafico_atomo_inicio_puntos, grafico_atomo_final_puntos,
                                                grafico_generacion_inicio_puntos, grafico_generacion_final_puntos,
                                                maximas_generaciones, cantidad_de_proteinas,
                                                cantidad_de_repeticiones, dimensiones);
            f2.ShowDialog();
        }

        
    }
    
}
