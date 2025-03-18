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

        public const int MAX_PROTEINAS = 51;
        public const int MAX_AMINOACIDOS = 101;
        public const int MAX_REPETICIONES = 51;
        public const int MAX_REPETICIONES_GRAFICO = 11;
        public const int MAX_GENERACIONES = 1001;
        public const int MAX_DATOS_INTEGRADOS = 1001;

        public int maximas_evaluaciones_de_funcion;
        public int maximas_generaciones;
        public int cantidad_de_proteinas;
        public int cantidad_de_repeticiones;
        public int dimensiones;
        public bool control_de_finalizacion_por_evaluaciones_de_la_FO;

        public struct Valores_de_FO
        {
            public float mejor_valor_de_FO;
            public float evaluaciones_de_la_FO;
        }
        public struct Grafico_de_una_proteina
        {
            public string nombre_proteina;
            public float[,,] posiciones_de_aminoacidos_por_proteina_repeticion_generacion_y_aminoacido;
            public float[,] valor_de_la_FO_por_proteina_repeticion_y_generacion;
        }
        public Grafico_de_una_proteina[] datos_de_grafico;
        public Valores_de_FO[,] datos_de_grafico_FO;
        public long iteraciones_reales;
        public int grafico_cantidad_de_funciones;
        public int grafico_repeticiones_totales;
        public int grafico_proteina_inicio;
        public int grafico_proteina_final;
        public int grafico_generacion_inicio;
        public int grafico_generacion_final;
        public bool graficar_mejor_valor;
        public string nombre_archivo_leer_para_grafico;
        public string seleccion_para_graficar;


        private void button6_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Application.StartupPath;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.FileName = "";
            
            string cadena;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                nombre_archivo_leer_para_grafico = openFileDialog1.FileNames[0];
            else
                return;
            comboBox1.Items.Clear();
            StreamReader sr = new StreamReader(nombre_archivo_leer_para_grafico);
            cadena = sr.ReadLine();
            if (!cadena.StartsWith("Datos para graficos"))
            {
                cadena = nombre_archivo_leer_para_grafico;
                cadena = cadena.Substring(cadena.LastIndexOf("\\") + 1).Trim();
                MessageBox.Show("Error verifique su archivo:\n" + cadena +
                                "\nEste tipo de archivo no tiene información para graficar",
                                "Error en datos para graficar",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.RtlReading);
                sr.Close();
                return;
            }
            while (!sr.EndOfStream)
            {
                cadena=sr.ReadLine();
                if (cadena != "" && cadena.Contains("Linea de comandos:"))
                {
                    cadena=cadena.Substring(cadena.IndexOf(":")+1).Trim();
                    comboBox1.Items.Add(cadena);
                }
            }
            sr.Close();
            label1.Enabled = true;
            comboBox1.Enabled = true;
        }
        public struct datos_que_se_leen_enteros
        {
            public string archivo;
            public string linea;
            public long valor_leido;
        }

        public struct datos_que_se_leen_flotantes
        {
            public string archivo;
            public string linea;
            public float valor_leido;
        }

        public struct datos_para_integrar
        {
            public string linea_de_comandos;
            public datos_que_se_leen_flotantes [] valores_de_funcion_objetivo;
            public datos_que_se_leen_flotantes[] tiempo_de_ejecucion;
            public datos_que_se_leen_enteros [] evaluaciones_FO;
            public float min_valor_funcion;
            public float max_valor_funcion;
            public float desviacion_estandar_valor_funcion;
            public float promedio_del_valor_de_la_funcion;
            public float mediana_del_valor_de_la_funcion;
            public float min_tiempo;
            public float max_tiempo;
            public float desviacion_estandar_tiempo;
            public float promedio_del_tiempo;
            public float mediana_del_tiempo;
            public long min_evaluaciones_FO;
            public long max_evaluaciones_FO;
            public float desviacion_estandar_evaluaciones_FO;
            public float promedio_de_evaluaciones_FO;
            public float mediana_de_evaluaciones_FO;
            public int cantidad_de_repeticiones;
        }

        
        void ordena_datos_y_devuelve_medianas(ref datos_para_integrar datos, int cantidad_repeticiones)
        {
            datos_que_se_leen_flotantes temp_float;
            datos_que_se_leen_enteros temp_entero;
            for (int i = 1; i <= cantidad_repeticiones - 1; i++)
            {
                for(int j = i+1; j <= cantidad_repeticiones; j++)
                {
                    if (datos.valores_de_funcion_objetivo[i].valor_leido > datos.valores_de_funcion_objetivo[j].valor_leido)
                    {
                        temp_float = datos.valores_de_funcion_objetivo[i];
                        datos.valores_de_funcion_objetivo[i] = datos.valores_de_funcion_objetivo[j];
                        datos.valores_de_funcion_objetivo[j] = temp_float;
                    }
                    if (datos.tiempo_de_ejecucion[i].valor_leido > datos.tiempo_de_ejecucion[j].valor_leido)
                    {
                        temp_float = datos.tiempo_de_ejecucion[i];
                        datos.tiempo_de_ejecucion[i] = datos.tiempo_de_ejecucion[j];
                        datos.tiempo_de_ejecucion[j] = temp_float;
                    }
                    if (datos.evaluaciones_FO[i].valor_leido > datos.evaluaciones_FO[j].valor_leido)
                    {
                        temp_entero = datos.evaluaciones_FO[i];
                        datos.evaluaciones_FO[i] = datos.evaluaciones_FO[j];
                        datos.evaluaciones_FO[j] = temp_entero;
                    }
                }
            }
            int marca = (int)Math.Floor(cantidad_repeticiones / 2.0);
            if (cantidad_repeticiones % 2 == 0)
            {
                datos.mediana_del_tiempo = (datos.tiempo_de_ejecucion[marca].valor_leido + datos.tiempo_de_ejecucion[marca + 1].valor_leido) / 2.0f;
                datos.mediana_del_valor_de_la_funcion = (datos.valores_de_funcion_objetivo[marca].valor_leido + datos.valores_de_funcion_objetivo[marca + 1].valor_leido) / 2.0f;
                datos.mediana_de_evaluaciones_FO = (datos.evaluaciones_FO[marca].valor_leido + datos.evaluaciones_FO[marca + 1].valor_leido) / 2.0f;
            }
            else
            {
                datos.mediana_del_tiempo = datos.tiempo_de_ejecucion[marca + 1].valor_leido;
                datos.mediana_del_valor_de_la_funcion = datos.valores_de_funcion_objetivo[marca + 1].valor_leido;
                datos.mediana_de_evaluaciones_FO = datos.evaluaciones_FO[marca + 1].valor_leido;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            long linea_del_archivo;
            uint cantidad_datos_integrados;
            cantidad_datos_integrados = 0;
            int escrituras_hechas = 0;
            openFileDialog2.InitialDirectory = Application.StartupPath;
            openFileDialog2.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog2.FilterIndex = 1;
            openFileDialog2.RestoreDirectory = true;
            openFileDialog2.FileName = "";
            string nombre_archivo_leer_1;
            string archivo_de_salida_csv;
            StreamWriter escribe;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
                nombre_archivo_leer_1 = openFileDialog2.FileNames[0];
            else
                return;
            saveFileDialog1.InitialDirectory = Application.StartupPath;
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = "Result.csv";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                archivo_de_salida_csv = saveFileDialog1.FileName;
            else
                return;
            this.Cursor = Cursors.WaitCursor;
            escribe = new StreamWriter(archivo_de_salida_csv);
            escribe.AutoFlush = true;
            escribe.WriteLine("Promedio de la FO, Mediana de la FO, Desv. Standart FO, Minimo de la FO, Maximo de la FO, Promedio del tiempo, Mediana del tiempo, Desv. Standart del Tiempo, Minimo del Tiempo, Maximo del tiempo, Promedio de evaluaciones de la FO, Mediana de evaluaciones de la FO, Desv. Standart de evaluaciones de la FO, Minimo de evaluaciones de la FO, Maximo de evaluaciones de la FO, Cantidad de repeticiones, Linea de comandos");
            this.Cursor = Cursors.WaitCursor;
            datos_para_integrar[] datos = new datos_para_integrar[MAX_DATOS_INTEGRADOS];
            for (int i = 1; i < MAX_DATOS_INTEGRADOS; i++)
            {
                datos[i].linea_de_comandos = "";
                datos[i].valores_de_funcion_objetivo = new datos_que_se_leen_flotantes[MAX_REPETICIONES];
                datos[i].tiempo_de_ejecucion = new datos_que_se_leen_flotantes[MAX_REPETICIONES];
                datos[i].evaluaciones_FO = new datos_que_se_leen_enteros[MAX_REPETICIONES];
                datos[i].min_valor_funcion = float.MaxValue;
                datos[i].max_valor_funcion = float.MinValue;
                datos[i].desviacion_estandar_valor_funcion = 0.0f;
                datos[i].promedio_del_valor_de_la_funcion = 0.0f;
                datos[i].mediana_del_valor_de_la_funcion = 0.0f;
                datos[i].promedio_del_tiempo = 0.0f;
                datos[i].min_tiempo = float.MaxValue;
                datos[i].max_tiempo = float.MinValue;
                datos[i].desviacion_estandar_tiempo = 0.0f;
                datos[i].mediana_del_tiempo = 0.0f;
                datos[i].min_evaluaciones_FO = long.MaxValue;
                datos[i].max_evaluaciones_FO = long.MinValue;
                datos[i].desviacion_estandar_evaluaciones_FO = 0.0f;
                datos[i].mediana_de_evaluaciones_FO = 0.0f;
                datos[i].promedio_de_evaluaciones_FO = 0.0f;
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
                    if (cadena.StartsWith("Datos para graficos"))
                    {
                        MessageBox.Show("Error verifique su archivo:\n" + nombre_archivo_leer +
                                        "\nEste tipo de archivo no tiene información para integrar.",
                                        "Error en datos para graficar",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                        MessageBoxOptions.RtlReading);
                        break;
                    }
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
                                    datos[i].desviacion_estandar_valor_funcion += (float)Math.Pow(datos[i].valores_de_funcion_objetivo[j].valor_leido
                                                                                                                   - datos[i].promedio_del_valor_de_la_funcion, 2.0);
                                    datos[i].desviacion_estandar_tiempo += (float)Math.Pow(datos[i].tiempo_de_ejecucion[j].valor_leido
                                                                                                                   - datos[i].promedio_del_tiempo, 2.0);
                                }
                                datos[i].desviacion_estandar_valor_funcion = (float)Math.Sqrt(datos[i].desviacion_estandar_valor_funcion / datos[i].cantidad_de_repeticiones);
                                datos[i].desviacion_estandar_tiempo = (float)Math.Sqrt(datos[i].desviacion_estandar_tiempo / datos[i].cantidad_de_repeticiones);
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
                                datos[i].valores_de_funcion_objetivo = new datos_que_se_leen_flotantes[MAX_REPETICIONES];
                                datos[i].tiempo_de_ejecucion = new datos_que_se_leen_flotantes[MAX_REPETICIONES];
                                datos[i].evaluaciones_FO = new datos_que_se_leen_enteros[MAX_REPETICIONES];
                                datos[i].min_valor_funcion = float.MaxValue;
                                datos[i].max_valor_funcion = float.MinValue;
                                datos[i].desviacion_estandar_valor_funcion = 0.0f;
                                datos[i].promedio_del_valor_de_la_funcion = 0.0f;
                                datos[i].mediana_del_valor_de_la_funcion = 0.0f;
                                datos[i].promedio_del_tiempo = 0.0f;
                                datos[i].min_tiempo = float.MaxValue;
                                datos[i].max_tiempo = float.MinValue;
                                datos[i].desviacion_estandar_tiempo = 0.0f;
                                datos[i].mediana_del_tiempo = 0.0f;
                                datos[i].min_evaluaciones_FO = long.MaxValue;
                                datos[i].max_evaluaciones_FO = long.MinValue;
                                datos[i].desviacion_estandar_evaluaciones_FO = 0.0f;
                                datos[i].mediana_de_evaluaciones_FO = 0.0f;
                                datos[i].promedio_de_evaluaciones_FO = 0.0f;
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
                        datos[cantidad_datos_integrados].valores_de_funcion_objetivo[repeticion].valor_leido = System.Convert.ToSingle(cadena);
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
                        datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].valor_leido = System.Convert.ToSingle(cadena);
                        datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].linea = linea_del_archivo.ToString();
                        datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].archivo = nombre_archivo_leer;
                        datos[cantidad_datos_integrados].promedio_del_tiempo += datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].valor_leido;
                        if (datos[cantidad_datos_integrados].min_tiempo > datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].valor_leido)
                            datos[cantidad_datos_integrados].min_tiempo = datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].valor_leido;
                        if (datos[cantidad_datos_integrados].max_tiempo < datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].valor_leido)
                            datos[cantidad_datos_integrados].max_tiempo = datos[cantidad_datos_integrados].tiempo_de_ejecucion[repeticion].valor_leido;
                        continue;
                    }
                    if (cadena.StartsWith("Cantidad de evaluaciones de la FO:"))
                    {
                        cadena = cadena.Substring(cadena.IndexOf(":") + 1).Trim();
                        datos[cantidad_datos_integrados].evaluaciones_FO[repeticion].valor_leido = System.Convert.ToInt64(cadena);
                        datos[cantidad_datos_integrados].evaluaciones_FO[repeticion].linea = linea_del_archivo.ToString();
                        datos[cantidad_datos_integrados].evaluaciones_FO[repeticion].archivo = nombre_archivo_leer;
                        datos[cantidad_datos_integrados].promedio_de_evaluaciones_FO += datos[cantidad_datos_integrados].evaluaciones_FO[repeticion].valor_leido;
                        if (datos[cantidad_datos_integrados].min_evaluaciones_FO > datos[cantidad_datos_integrados].evaluaciones_FO[repeticion].valor_leido)
                            datos[cantidad_datos_integrados].min_evaluaciones_FO = datos[cantidad_datos_integrados].evaluaciones_FO[repeticion].valor_leido;
                        if (datos[cantidad_datos_integrados].max_evaluaciones_FO < datos[cantidad_datos_integrados].evaluaciones_FO[repeticion].valor_leido)
                            datos[cantidad_datos_integrados].max_evaluaciones_FO = datos[cantidad_datos_integrados].evaluaciones_FO[repeticion].valor_leido;
                        continue;
                    }
                }
                while (!lee.EndOfStream);
                lee.Close();
            }
            if (cantidad_datos_integrados == 0)
            {
                escribe.Close();
                System.IO.File.Delete(archivo_de_salida_csv);
                textBox58.Text = "0";
                textBox58.Refresh();
                return;
            }
            for (int i = 1; i <= cantidad_datos_integrados; i++)
            {
                datos[i].promedio_del_tiempo /= datos[i].cantidad_de_repeticiones;
                datos[i].promedio_del_valor_de_la_funcion /= datos[i].cantidad_de_repeticiones;
                datos[i].promedio_de_evaluaciones_FO /= datos[i].cantidad_de_repeticiones;
                for (int j = 1; j <= datos[i].cantidad_de_repeticiones; j++)
                {
                    datos[i].desviacion_estandar_valor_funcion += (float)Math.Pow(datos[i].valores_de_funcion_objetivo[j].valor_leido
                                                                                                   - datos[i].promedio_del_valor_de_la_funcion, 2.0);
                    datos[i].desviacion_estandar_tiempo += (float)Math.Pow(datos[i].tiempo_de_ejecucion[j].valor_leido
                                                                                                   - datos[i].promedio_del_tiempo, 2.0);
                    datos[i].desviacion_estandar_evaluaciones_FO += (float)Math.Pow(datos[i].evaluaciones_FO[j].valor_leido
                                                                                                   - datos[i].promedio_de_evaluaciones_FO, 2.0);
                }
                datos[i].desviacion_estandar_valor_funcion = (float)Math.Sqrt(datos[i].desviacion_estandar_valor_funcion / datos[i].cantidad_de_repeticiones);
                datos[i].desviacion_estandar_tiempo = (float)Math.Sqrt(datos[i].desviacion_estandar_tiempo / datos[i].cantidad_de_repeticiones);
                datos[i].desviacion_estandar_evaluaciones_FO = (float)Math.Sqrt(datos[i].desviacion_estandar_evaluaciones_FO / datos[i].cantidad_de_repeticiones);
            }
            for (int i = 1; i <= cantidad_datos_integrados; i++)
                 ordena_datos_y_devuelve_medianas(ref datos[i], datos[i].cantidad_de_repeticiones);
            for (int i = 1; i <= cantidad_datos_integrados; i++)
            {
                if (datos[i].promedio_del_valor_de_la_funcion == 0 &&
                    datos[i].valores_de_funcion_objetivo[1].archivo == null)
                    continue;
                escribe.Write(datos[i].promedio_del_valor_de_la_funcion.ToString() + ",");
                escribe.Write(datos[i].mediana_del_valor_de_la_funcion.ToString() + ",");
                escribe.Write(datos[i].desviacion_estandar_valor_funcion.ToString() + ",");
                escribe.Write(datos[i].min_valor_funcion.ToString() + ",");
                escribe.Write(datos[i].max_valor_funcion.ToString() + ",");
                escribe.Write(datos[i].promedio_del_tiempo.ToString() + ",");
                escribe.Write(datos[i].mediana_del_tiempo.ToString() + ",");
                escribe.Write(datos[i].desviacion_estandar_tiempo.ToString() + ",");
                escribe.Write(datos[i].min_tiempo + ",");
                escribe.Write(datos[i].max_tiempo + ",");
                escribe.Write(datos[i].promedio_de_evaluaciones_FO.ToString() + ",");
                escribe.Write(datos[i].mediana_de_evaluaciones_FO.ToString() + ",");
                escribe.Write(datos[i].desviacion_estandar_evaluaciones_FO.ToString() + ",");
                escribe.Write(datos[i].min_evaluaciones_FO + ",");
                escribe.Write(datos[i].max_evaluaciones_FO + ",");
                escribe.Write(datos[i].cantidad_de_repeticiones + ",");
                escribe.WriteLine(datos[i].linea_de_comandos);
                escribe.Flush();
            }
            escribe.Close();
            this.Cursor = Cursors.Default;
        }

        

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (nombre_archivo_leer_para_grafico == "")
            {
                MessageBox.Show("No ha seleccionado un archivo de ejecución.",
                                "Error en datos para graficar",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.RtlReading);
                return;
            }
            seleccion_para_graficar = comboBox1.Text;
            string cadena;
            StreamReader sr = new StreamReader(openFileDialog1.FileName);
            int cuenta_generaciones = 1;
            this.Cursor = Cursors.WaitCursor;
            while (!sr.EndOfStream)
            {
                cadena = sr.ReadLine();
                if (cadena != "" && cadena.Contains("Linea de comandos:"))
                {
                    cadena = cadena.Substring(cadena.IndexOf(":") + 1).Trim();
                    if (cadena == seleccion_para_graficar)
                    {
                        cadena = sr.ReadLine();
                        cadena = sr.ReadLine();
                        if (cadena.Contains("Finalizacion por iteraciones maximas"))
                        {
                            control_de_finalizacion_por_evaluaciones_de_la_FO = false;
                            textBox1.Text = "Finalizacion por iteraciones máximas";
                            textBox1.Refresh();
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            maximas_generaciones = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            cantidad_de_repeticiones = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            cantidad_de_proteinas = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            dimensiones = System.Convert.ToInt32(cadena);
                            for (int k = 1; k <= cantidad_de_repeticiones; k++)
                            {
                                cadena = sr.ReadLine();
                                for (int j = 1; j <= maximas_generaciones; j++)
                                {
                                    cadena = sr.ReadLine();
                                    cadena = sr.ReadLine();
                                    cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                    datos_de_grafico_FO[k, j].mejor_valor_de_FO = System.Convert.ToSingle(cadena);
                                    cadena = sr.ReadLine();
                                    cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                    datos_de_grafico_FO[k, j].evaluaciones_de_la_FO = System.Convert.ToInt32(cadena);
                                }
                            }
                            for (int i = 1; i <= cantidad_de_proteinas; i++)
                            {
                                cadena = sr.ReadLine();
                                for (int k = 1; k <= cantidad_de_repeticiones; k++)
                                {
                                    cadena = sr.ReadLine();
                                    for (int j = 1; j <= maximas_generaciones; j++)
                                    {
                                        cadena = sr.ReadLine();
                                        cadena = sr.ReadLine();
                                        cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                        datos_de_grafico[i].valor_de_la_FO_por_proteina_repeticion_y_generacion[k, j] = System.Convert.ToSingle(cadena);
                                        cadena = sr.ReadLine().Trim();
                                        cadena = sr.ReadLine().Trim();
                                        string[] pedazos = cadena.Split(' ');
                                        for (int h = 0; h < pedazos.Length; h++)
                                            datos_de_grafico[i].posiciones_de_aminoacidos_por_proteina_repeticion_generacion_y_aminoacido[k, j, h + 1] = System.Convert.ToSingle(pedazos[h]);
                                    }
                                }
                            }
                        }
                        else
                        {
                            control_de_finalizacion_por_evaluaciones_de_la_FO = true;
                            textBox1.Text = "Finalizacion por evaluaciones de la FO";
                            textBox1.Refresh();
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            maximas_generaciones = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            cantidad_de_repeticiones = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            cantidad_de_proteinas = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            dimensiones = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            for (int k = 1; k <= cantidad_de_repeticiones; k++)
                            {
                                cuenta_generaciones = 1;
                                while (cuenta_generaciones <= MAX_GENERACIONES - 1)
                                {
                                    cadena = sr.ReadLine();
                                    if (cadena.Contains("Repeticion:"))
                                        goto otra;
                                    if (cadena.Contains("Proteina:"))
                                        goto sale;
                                    cadena = sr.ReadLine();
                                    cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                    datos_de_grafico_FO[k, cuenta_generaciones].mejor_valor_de_FO = System.Convert.ToSingle(cadena);
                                    cadena = sr.ReadLine();
                                    cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                    datos_de_grafico_FO[k, cuenta_generaciones].evaluaciones_de_la_FO = System.Convert.ToInt32(cadena);
                                    cuenta_generaciones++;
                                }
                            otra:
                                continue;
                            }
                        sale:
                            maximas_generaciones = cuenta_generaciones - 1;
                            for (int i = 1; i <= cantidad_de_proteinas; i++)
                            {
                                if (i!=1)
                                    cadena = sr.ReadLine();
                                for (int k = 1; k <= cantidad_de_repeticiones; k++)
                                {
                                    cadena = sr.ReadLine();
                                    for (int j = 1; j <= maximas_generaciones; j++)
                                    {
                                        cadena = sr.ReadLine();
                                        cadena = sr.ReadLine();
                                        cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                        datos_de_grafico[i].valor_de_la_FO_por_proteina_repeticion_y_generacion[k, j] = System.Convert.ToSingle(cadena);
                                        cadena = sr.ReadLine().Trim();
                                        cadena = sr.ReadLine().Trim();
                                        string[] pedazos = cadena.Split(' ');
                                        for (int h = 0; h < pedazos.Length; h++)
                                            datos_de_grafico[i].posiciones_de_aminoacidos_por_proteina_repeticion_generacion_y_aminoacido[k, j, h + 1] = System.Convert.ToSingle(pedazos[h]);
                                    }
                                }
                            }
                        }
                    }
                }
                
            }
            sr.Close();
            this.Cursor = Cursors.Default;
            label2.Enabled = true;
            textBox1.Enabled = true;
            label2.Refresh();
            textBox1.Refresh();
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (seleccion_para_graficar == "")
            {
                MessageBox.Show("No ha seleccionado una ejecución para graficar.",
                                "Error en datos para graficar",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.RtlReading);
                return;
            }
            GraficoAtomos f2 = new GraficoAtomos(datos_de_grafico,cantidad_de_repeticiones,dimensiones,maximas_generaciones,cantidad_de_proteinas);
            f2.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            nombre_archivo_leer_para_grafico = "";
            seleccion_para_graficar = "";
            datos_de_grafico = new Grafico_de_una_proteina[MAX_PROTEINAS];
            datos_de_grafico_FO = new Valores_de_FO[MAX_REPETICIONES, MAX_GENERACIONES];
            for (int i = 1; i < MAX_PROTEINAS; i++)
            {
                datos_de_grafico[i].nombre_proteina = "";
                datos_de_grafico[i].posiciones_de_aminoacidos_por_proteina_repeticion_generacion_y_aminoacido = new float[MAX_REPETICIONES, MAX_GENERACIONES, MAX_AMINOACIDOS];
                datos_de_grafico[i].valor_de_la_FO_por_proteina_repeticion_y_generacion = new float[MAX_REPETICIONES, MAX_GENERACIONES];
            }
            
            toolTip1.SetToolTip(this.button7, "Permite integrar varios archivos de ejecución y se crea un archivo\n" +
                                              "resumen de las ejecuciones de valores separados por comas que puede \n" +
                                              "abrirse con Excel.");

            toolTip1.SetToolTip(this.label82, "Se muestra el archivo de ejecución que se está analizando.");
            toolTip1.SetToolTip(this.textBox57, "Se muestra el archivo de ejecución que se está analizando.");

            toolTip1.SetToolTip(this.label83, "Cantidad de ejecuciones diferentes encontradas en los diferentes\n" +
                                                "archivos de ejecuciones que se seleccionaron."); 
            toolTip1.SetToolTip(this.textBox58, "Cantidad de ejecuciones diferentes encontradas en los diferentes\n" +
                                                "archivos de ejecuciones que se seleccionaron.");

            toolTip1.SetToolTip(this.panel17, "Con esta opción puede integrar varios archivos de ejecuciones en uno solo\n" +
                                                "que puede abrir desde Excel, con información tal como: medias de valores mínimos\n" +
                                                "obtenidos, desviaciones estándar, medianas, etc. Solamente selecciones los archivos\n" +
                                                "a integrar con el botón de comando \"Integra archivos de corrida\".");
            toolTip1.SetToolTip(this.label3, "Con esta opción puede integrar varios archivos de ejecuciones en uno solo\n" +
                                                "que puede abrir desde Excel, con información tal como: medias de valores mínimos\n" +
                                                "obtenidos, desviaciones estándar, medianas, etc. Solamente selecciones los archivos\n" +
                                                "a integrar con el botón de comando \"Integra archivos de corrida\".");

            toolTip1.SetToolTip(this.panel9, "Con esta opción puede graficar la evolución de los aminoácidos que componen una\n" +
                                               "proteína durante el proceso de optimización. El gráfico muestra la posición de los\n" +
                                               "aminoácidos que componen cada proteína. Usted puede seleccionar las proteínas a\n" +
                                                "graficar, la iteración y la repetición. Si quiere esta opción selecciones el\n" +
                                                "botón de comando \"Gráfico de proteínas\".\n" +
                                               "También se puede graficar cómo evolucionan los mínimos de la Función Objetivo\n" + 
                                               "obtenido por cada una de las proteínas en las diferentes iteraciones y repeticiones\n" +
                                               "del proceso de optimización. Para esta opción seleccione \"Gráfico de convergencia\".\n");
            toolTip1.SetToolTip(this.label4, "Con esta opción puede graficar la evolución de los aminoácidos que componen una\n" +
                                               "proteína durante el proceso de optimización. El gráfico muestra la posición de los\n" +
                                               "aminoácidos que componen cada proteína. Usted puede seleccionar las proteínas a\n" +
                                                "graficar, la iteración y la repetición. Si quiere esta opción selecciones el\n" +
                                                "botón de comando \"Gráfico de proteínas\".\n" +
                                               "También se puede graficar cómo evolucionan los mínimos de la Función Objetivo\n" +
                                               "obtenido por cada una de las proteínas en las diferentes iteraciones y repeticiones\n" +
                                               "del proceso de optimización. Para esta opción seleccione \"Gráfico de convergencia\".\n");

            toolTip1.SetToolTip(this.button6, "Seleccione el archivo de ejecución para realizar los diferentes gráficos.\n");

            toolTip1.SetToolTip(this.button1, "Muestra los gráficos de convergencia del archivo seleccionado.\n");

            toolTip1.SetToolTip(this.button2, "Muestra el gráfico de la evolución de los aminoácidos de las proteínas del.\n" +
                                              "archivo seleccionado.");

            toolTip1.SetToolTip(this.label1, "Seleccione del archivo de ejecución la línea de comandos que desea graficar\n" +
                                                "ya que en cada archivo de ejecución puede haber varias optimizaciones.");
            toolTip1.SetToolTip(this.comboBox1, "Seleccione del archivo de ejecución la línea de comandos que desea graficar\n" +
                                                "ya que en cada archivo de ejecución puede haber varias optimizaciones.");

            toolTip1.SetToolTip(this.label2, "Se muestra si en la ejecución seleccionada el criterio de finalización seleccionado\n" +
                                             "fue por iteraciones o evaluaciones de la Función Objetivo. En el caso de finalización\n" +
                                             "por evaluaciones máximas de la Función Objetivo la iteración máxima a graficar se\n" +
                                             "obtiene al leer los datos del archivo de ejecución.");
            toolTip1.SetToolTip(this.textBox1, "Se muestra si en la ejecución seleccionada el criterio de finalización seleccionado\n" +
                                             "fue por iteraciones o evaluaciones de la Función Objetivo. En el caso de finalización\n" +
                                             "por evaluaciones máximas de la Función Objetivo la iteración máxima a graficar se\n" +
                                             "obtiene al leer los datos del archivo de ejecución.");

            toolTip1.SetToolTip(this.numericUpDown11, "Seleccionar la repetición inicial a consolidar.");
            toolTip1.SetToolTip(this.label12, "Seleccionar la repetición inicial a consolidar.");
            
            toolTip1.SetToolTip(this.numericUpDown1, "Seleccionar la repetición final a consolidar.");
            toolTip1.SetToolTip(this.label10, "Seleccionar la repetición final a consolidar.");

            toolTip1.SetToolTip(this.numericUpDown8, "Seleccionar proteína inicial a consolidar.");
            toolTip1.SetToolTip(this.label9, "Seleccionar proteína inicial a consolidar.");

            toolTip1.SetToolTip(this.numericUpDown7, "Seleccionar proteína final a consolidar.");
            toolTip1.SetToolTip(this.label8, "Seleccionar proteína final a consolidar.");

            toolTip1.SetToolTip(this.button3, "Archivo con datos de ejecución de gráfico para crear un archivo de datos para estudio de\n" +
                                               "convergencia.");

            toolTip1.SetToolTip(this.button4, "Crea un archivo de datos separados por comas con los valores de evaluación de la Función Objetivo.\n" +
                                              "Cada línea corresponde a una repetición o proteína de una repetición, en las columnas estan los valores\n" +
                                              "obtenidos de la evaluación de la Función Objetivo.\n" +
                                              "Si ya existe el archivo entonces la nueva información se adiciona al archivo ya existente.");

            toolTip1.SetToolTip(this.checkBox2, "Si está marcado no se toman en cuenta los valores para cada proteína, solamente los mejores valores\n" +
                                                 "globales obtenidos en la optimización para la Función Objetivo.");

            toolTip1.SetToolTip(this.label7, "Seleccione del archivo de ejecución la línea de comandos que desea unificar en el archivo de estudio de\n" +
                                                "convergencia.");
            toolTip1.SetToolTip(this.comboBox2, "Seleccione del archivo de ejecución la línea de comandos que desea unificar en el archivo de estudio de\n" +
                                                "convergencia.");

            toolTip1.SetToolTip(this.panel2, "Con esta opción puede realizar estudios de convergencia en las funciones que\n" +
                                                "está optimizando, con el análisis de las etapas de intensificación y exploración.");
            toolTip1.SetToolTip(this.label6, "Con esta opción puede realizar estudios de convergencia en las funciones que\n" +
                                                "está optimizando, con el análisis de las etapas de intensificación y exploración.");

            toolTip1.SetToolTip(this.button8, "Seleccione el archivo de ejecución para realizar los diferentes gráficos.\n");

            toolTip1.SetToolTip(this.button9, "Muestra los gráficos para el análisis de las fases de intensificación y exploración del algoritmo.");

            toolTip1.SetToolTip(this.label14, "Seleccione del archivo de ejecución la línea de comandos que desea graficar\n" +
                                                "ya que en cada archivo de ejecución puede haber varias optimizaciones.");
            toolTip1.SetToolTip(this.comboBox3, "Seleccione del archivo de ejecución la línea de comandos que desea graficar\n" +
                                                "ya que en cada archivo de ejecución puede haber varias optimizaciones.");

            toolTip1.SetToolTip(this.label13, "Se muestra si en la ejecución seleccionada el criterio de finalización seleccionado\n" +
                                             "fue por iteraciones o evaluaciones de la Función Objetivo. En el caso de finalización\n" +
                                             "por evaluaciones máximas de la Función Objetivo la iteración máxima a graficar se\n" +
                                             "obtiene al leer los datos del archivo de ejecución.");
            toolTip1.SetToolTip(this.textBox2, "Se muestra si en la ejecución seleccionada el criterio de finalización seleccionado\n" +
                                             "fue por iteraciones o evaluaciones de la Función Objetivo. En el caso de finalización\n" +
                                             "por evaluaciones máximas de la Función Objetivo la iteración máxima a graficar se\n" +
                                             "obtiene al leer los datos del archivo de ejecución.");

            toolTip1.SetToolTip(this.button5, "Realiza un estudio de convergencia en el espacio de búsqueda por las diferentes dimensiones\n." +
                                              "de la Función Objetivo.");
            

            button7.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (seleccion_para_graficar == "")
            {
                MessageBox.Show("No ha seleccionado una ejecución para graficar.",
                                "Error en datos para graficar",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.RtlReading);
                return;
            }
            Form2 f2 = new Form2(datos_de_grafico, datos_de_grafico_FO, cantidad_de_repeticiones, cantidad_de_proteinas, maximas_generaciones, true);
            f2.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Application.StartupPath;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.FileName = "";

            string cadena;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                nombre_archivo_leer_para_grafico = openFileDialog1.FileNames[0];
            else
                return;
            comboBox2.Items.Clear();
            StreamReader sr = new StreamReader(nombre_archivo_leer_para_grafico);
            cadena = sr.ReadLine();
            if (!cadena.StartsWith("Datos para graficos"))
            {
                cadena = nombre_archivo_leer_para_grafico;
                cadena = cadena.Substring(cadena.LastIndexOf("\\") + 1).Trim();
                MessageBox.Show("Error verifique su archivo:\n" + cadena +
                                "\nEste tipo de archivo no tiene información para graficar y no puede usarse en estudios de convergencia",
                                "Error en el archivo con datos para estudio de convergencia",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.RtlReading);
                sr.Close();
                return;
            }
            while (!sr.EndOfStream)
            {
                cadena = sr.ReadLine();
                if (cadena != "" && cadena.Contains("Linea de comandos:"))
                {
                    cadena = cadena.Substring(cadena.IndexOf(":") + 1).Trim();
                    comboBox2.Items.Add(cadena);
                }
            }
            sr.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string seleccion_para_unificar = comboBox2.Text;
            StreamWriter escribe;
            string cadena;
            saveFileDialog1.InitialDirectory = Application.StartupPath;
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = "Convergencia_estudio.csv";
            saveFileDialog1.CheckFileExists = false;
            string archivo_de_salida_csv;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                archivo_de_salida_csv = saveFileDialog1.FileName;
            else
                return;
            if (File.Exists(archivo_de_salida_csv))
            {
                escribe = new StreamWriter(archivo_de_salida_csv, true);
                escribe.WriteLine();
                escribe.WriteLine();
                escribe.WriteLine(seleccion_para_unificar);
            }
            else
            {
                escribe = new StreamWriter(archivo_de_salida_csv, false);
                escribe.WriteLine(seleccion_para_unificar);
            }
            int repeticion_inicial = (int)numericUpDown11.Value;
            int repeticion_final = (int)numericUpDown1.Value;
            int proteina_inicial = (int)numericUpDown8.Value;
            int proteina_final = (int)numericUpDown7.Value;
            this.Cursor = Cursors.WaitCursor;
            cadena = "Iteracion , ";
            for (int j = 1; j <= maximas_generaciones; j++)
                 cadena += j.ToString() + " , ";
            escribe.WriteLine(cadena);
            for (int i = repeticion_inicial; i <= repeticion_final; i++)
            {
                cadena = "Repeticion " + i.ToString() + " , ";
                for (int j = 1; j <= maximas_generaciones; j++)
                    cadena += datos_de_grafico_FO[i, j].mejor_valor_de_FO.ToString(FORMATO_VALORES_FUNCION) + " , ";
                escribe.WriteLine(cadena);
            }
            if (checkBox2.Checked)
            {
                for (int i = repeticion_inicial; i <= repeticion_final; i++)
                {
                    for (int k = proteina_inicial; k <= proteina_final; k++)
                    {
                        cadena = "Repeticion " + i.ToString() + " Proteina " + k.ToString() + ",";
                        for (int j = 1; j <= maximas_generaciones; j++)
                            cadena += datos_de_grafico[k].valor_de_la_FO_por_proteina_repeticion_y_generacion[i, j].ToString(FORMATO_VALORES_FUNCION) + " , ";
                        escribe.WriteLine(cadena);
                    }
                }
            }
            this.Cursor = Cursors.Default;
            escribe.Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (nombre_archivo_leer_para_grafico == "")
            {
                MessageBox.Show("No ha seleccionado un archivo de ejecución.",
                                "Error en datos para graficar",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.RtlReading);
                return;
            }
            seleccion_para_graficar = comboBox2.Text;
            string cadena;
            StreamReader sr = new StreamReader(openFileDialog1.FileName);
            int cuenta_generaciones = 1;
            this.Cursor = Cursors.WaitCursor;
            while (!sr.EndOfStream)
            {
                cadena = sr.ReadLine();
                if (cadena != "" && cadena.Contains("Linea de comandos:"))
                {
                    cadena = cadena.Substring(cadena.IndexOf(":") + 1).Trim();
                    if (cadena == seleccion_para_graficar)
                    {
                        cadena = sr.ReadLine();
                        cadena = sr.ReadLine();
                        if (cadena.Contains("Finalizacion por iteraciones maximas"))
                        {
                            control_de_finalizacion_por_evaluaciones_de_la_FO = false;
                            textBox1.Text = "Finalizacion por iteraciones máximas";
                            textBox1.Refresh();
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            maximas_generaciones = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            cantidad_de_repeticiones = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            cantidad_de_proteinas = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            dimensiones = System.Convert.ToInt32(cadena);
                            for (int k = 1; k <= cantidad_de_repeticiones; k++)
                            {
                                cadena = sr.ReadLine();
                                for (int j = 1; j <= maximas_generaciones; j++)
                                {
                                    cadena = sr.ReadLine();
                                    cadena = sr.ReadLine();
                                    cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                    datos_de_grafico_FO[k, j].mejor_valor_de_FO = System.Convert.ToSingle(cadena);
                                    cadena = sr.ReadLine();
                                    cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                    datos_de_grafico_FO[k, j].evaluaciones_de_la_FO = System.Convert.ToInt32(cadena);
                                }
                            }
                            for (int i = 1; i <= cantidad_de_proteinas; i++)
                            {
                                cadena = sr.ReadLine();
                                for (int k = 1; k <= cantidad_de_repeticiones; k++)
                                {
                                    cadena = sr.ReadLine();
                                    for (int j = 1; j <= maximas_generaciones; j++)
                                    {
                                        cadena = sr.ReadLine();
                                        cadena = sr.ReadLine();
                                        cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                        datos_de_grafico[i].valor_de_la_FO_por_proteina_repeticion_y_generacion[k, j] = System.Convert.ToSingle(cadena);
                                        cadena = sr.ReadLine().Trim();
                                        cadena = sr.ReadLine().Trim();
                                        string[] pedazos = cadena.Split(' ');
                                        for (int h = 0; h < pedazos.Length; h++)
                                            datos_de_grafico[i].posiciones_de_aminoacidos_por_proteina_repeticion_generacion_y_aminoacido[k, j, h + 1] = System.Convert.ToSingle(pedazos[h]);
                                    }
                                }
                            }
                        }
                        else
                        {
                            control_de_finalizacion_por_evaluaciones_de_la_FO = true;
                            textBox1.Text = "Finalizacion por evaluaciones de la FO";
                            textBox1.Refresh();
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            maximas_generaciones = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            cantidad_de_repeticiones = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            cantidad_de_proteinas = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            dimensiones = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            for (int k = 1; k <= cantidad_de_repeticiones; k++)
                            {
                                cuenta_generaciones = 1;
                                while (cuenta_generaciones <= MAX_GENERACIONES - 1)
                                {
                                    cadena = sr.ReadLine();
                                    if (cadena.Contains("Repeticion:"))
                                        goto otra;
                                    if (cadena.Contains("Proteina:"))
                                        goto sale;
                                    cadena = sr.ReadLine();
                                    cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                    datos_de_grafico_FO[k, cuenta_generaciones].mejor_valor_de_FO = System.Convert.ToSingle(cadena);
                                    cadena = sr.ReadLine();
                                    cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                    datos_de_grafico_FO[k, cuenta_generaciones].evaluaciones_de_la_FO = System.Convert.ToInt32(cadena);
                                    cuenta_generaciones++;
                                }
                            otra:
                                continue;
                            }
                        sale:
                            maximas_generaciones = cuenta_generaciones - 1;
                            for (int i = 1; i <= cantidad_de_proteinas; i++)
                            {
                                if (i != 1)
                                    cadena = sr.ReadLine();
                                for (int k = 1; k <= cantidad_de_repeticiones; k++)
                                {
                                    cadena = sr.ReadLine();
                                    for (int j = 1; j <= maximas_generaciones; j++)
                                    {
                                        cadena = sr.ReadLine();
                                        cadena = sr.ReadLine();
                                        cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                        datos_de_grafico[i].valor_de_la_FO_por_proteina_repeticion_y_generacion[k, j] = System.Convert.ToSingle(cadena);
                                        cadena = sr.ReadLine().Trim();
                                        cadena = sr.ReadLine().Trim();
                                        string[] pedazos = cadena.Split(' ');
                                        for (int h = 0; h < pedazos.Length; h++)
                                            datos_de_grafico[i].posiciones_de_aminoacidos_por_proteina_repeticion_generacion_y_aminoacido[k, j, h + 1] = System.Convert.ToSingle(pedazos[h]);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            sr.Close();
            this.Cursor = Cursors.Default;
            numericUpDown11.Minimum = 1;
            numericUpDown1.Minimum = 1;
            numericUpDown8.Minimum = 1;
            numericUpDown7.Minimum = 1;
            numericUpDown11.Maximum = cantidad_de_repeticiones;
            numericUpDown1.Maximum = cantidad_de_repeticiones;
            numericUpDown8.Maximum = cantidad_de_proteinas;
            numericUpDown7.Maximum = cantidad_de_proteinas;
            numericUpDown11.Value = 1;
            numericUpDown1.Value = cantidad_de_repeticiones;
            numericUpDown8.Value = 1;
            numericUpDown7.Value = cantidad_de_proteinas;
            numericUpDown11.Refresh();
            numericUpDown1.Refresh();
            numericUpDown8.Refresh();
            numericUpDown7.Refresh();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Application.StartupPath;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.FileName = "";

            string cadena;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                nombre_archivo_leer_para_grafico = openFileDialog1.FileNames[0];
            else
                return;
            comboBox3.Items.Clear();
            StreamReader sr = new StreamReader(nombre_archivo_leer_para_grafico);
            cadena = sr.ReadLine();
            if (!cadena.StartsWith("Datos para graficos"))
            {
                cadena = nombre_archivo_leer_para_grafico;
                cadena = cadena.Substring(cadena.LastIndexOf("\\") + 1).Trim();
                MessageBox.Show("Error verifique su archivo:\n" + cadena +
                                "\nEste tipo de archivo no tiene información para graficar y no puede usarse en estudios de convergencia",
                                "Error en el archivo con datos para estudio de convergencia",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.RtlReading);
                sr.Close();
                return;
            }
            while (!sr.EndOfStream)
            {
                cadena = sr.ReadLine();
                if (cadena != "" && cadena.Contains("Linea de comandos:"))
                {
                    cadena = cadena.Substring(cadena.IndexOf(":") + 1).Trim();
                    comboBox3.Items.Add(cadena);
                }
            }
            sr.Close();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (nombre_archivo_leer_para_grafico == "")
            {
                MessageBox.Show("No ha seleccionado un archivo de ejecución.",
                                "Error en datos para graficar",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.RtlReading);
                return;
            }
            seleccion_para_graficar = comboBox3.Text;
            string cadena;
            StreamReader sr = new StreamReader(openFileDialog1.FileName);
            int cuenta_generaciones = 1;
            this.Cursor = Cursors.WaitCursor;
            while (!sr.EndOfStream)
            {
                cadena = sr.ReadLine();
                if (cadena != "" && cadena.Contains("Linea de comandos:"))
                {
                    cadena = cadena.Substring(cadena.IndexOf(":") + 1).Trim();
                    if (cadena == seleccion_para_graficar)
                    {
                        cadena = sr.ReadLine();
                        cadena = sr.ReadLine();
                        if (cadena.Contains("Finalizacion por iteraciones maximas"))
                        {
                            control_de_finalizacion_por_evaluaciones_de_la_FO = false;
                            textBox2.Text = "Finalizacion por iteraciones máximas";
                            textBox2.Refresh();
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            maximas_generaciones = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            cantidad_de_repeticiones = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            cantidad_de_proteinas = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            dimensiones = System.Convert.ToInt32(cadena);
                            for (int k = 1; k <= cantidad_de_repeticiones; k++)
                            {
                                cadena = sr.ReadLine();
                                for (int j = 1; j <= maximas_generaciones; j++)
                                {
                                    cadena = sr.ReadLine();
                                    cadena = sr.ReadLine();
                                    cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                    datos_de_grafico_FO[k, j].mejor_valor_de_FO = System.Convert.ToSingle(cadena);
                                    cadena = sr.ReadLine();
                                    cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                    datos_de_grafico_FO[k, j].evaluaciones_de_la_FO = System.Convert.ToInt32(cadena);
                                }
                            }
                            for (int i = 1; i <= cantidad_de_proteinas; i++)
                            {
                                cadena = sr.ReadLine();
                                for (int k = 1; k <= cantidad_de_repeticiones; k++)
                                {
                                    cadena = sr.ReadLine();
                                    for (int j = 1; j <= maximas_generaciones; j++)
                                    {
                                        cadena = sr.ReadLine();
                                        cadena = sr.ReadLine();
                                        cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                        datos_de_grafico[i].valor_de_la_FO_por_proteina_repeticion_y_generacion[k, j] = System.Convert.ToSingle(cadena);
                                        cadena = sr.ReadLine().Trim();
                                        cadena = sr.ReadLine().Trim();
                                        string[] pedazos = cadena.Split(' ');
                                        for (int h = 0; h < pedazos.Length; h++)
                                            datos_de_grafico[i].posiciones_de_aminoacidos_por_proteina_repeticion_generacion_y_aminoacido[k, j, h + 1] = System.Convert.ToSingle(pedazos[h]);
                                    }
                                }
                            }
                        }
                        else
                        {
                            control_de_finalizacion_por_evaluaciones_de_la_FO = true;
                            textBox2.Text = "Finalizacion por evaluaciones de la FO";
                            textBox2.Refresh();
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            maximas_generaciones = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            cantidad_de_repeticiones = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            cantidad_de_proteinas = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            cadena = cadena.Substring(cadena.LastIndexOf(":") + 1).Trim();
                            dimensiones = System.Convert.ToInt32(cadena);
                            cadena = sr.ReadLine();
                            for (int k = 1; k <= cantidad_de_repeticiones; k++)
                            {
                                cuenta_generaciones = 1;
                                while (cuenta_generaciones <= MAX_GENERACIONES - 1)
                                {
                                    cadena = sr.ReadLine();
                                    if (cadena.Contains("Repeticion:"))
                                        goto otra;
                                    if (cadena.Contains("Proteina:"))
                                        goto sale;
                                    cadena = sr.ReadLine();
                                    cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                    datos_de_grafico_FO[k, cuenta_generaciones].mejor_valor_de_FO = System.Convert.ToSingle(cadena);
                                    cadena = sr.ReadLine();
                                    cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                    datos_de_grafico_FO[k, cuenta_generaciones].evaluaciones_de_la_FO = System.Convert.ToInt32(cadena);
                                    cuenta_generaciones++;
                                }
                            otra:
                                continue;
                            }
                        sale:
                            maximas_generaciones = cuenta_generaciones - 1;
                            for (int i = 1; i <= cantidad_de_proteinas; i++)
                            {
                                if (i != 1)
                                    cadena = sr.ReadLine();
                                for (int k = 1; k <= cantidad_de_repeticiones; k++)
                                {
                                    cadena = sr.ReadLine();
                                    for (int j = 1; j <= maximas_generaciones; j++)
                                    {
                                        cadena = sr.ReadLine();
                                        cadena = sr.ReadLine();
                                        cadena = cadena.Substring(cadena.LastIndexOf(':') + 1).Trim();
                                        datos_de_grafico[i].valor_de_la_FO_por_proteina_repeticion_y_generacion[k, j] = System.Convert.ToSingle(cadena);
                                        cadena = sr.ReadLine().Trim();
                                        cadena = sr.ReadLine().Trim();
                                        string[] pedazos = cadena.Split(' ');
                                        for (int h = 0; h < pedazos.Length; h++)
                                            datos_de_grafico[i].posiciones_de_aminoacidos_por_proteina_repeticion_generacion_y_aminoacido[k, j, h + 1] = System.Convert.ToSingle(pedazos[h]);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            sr.Close();
            this.Cursor = Cursors.Default;
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (seleccion_para_graficar == "")
            {
                MessageBox.Show("No ha seleccionado una ejecución para graficar.",
                                "Error en datos para graficar",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.RtlReading);
                return;
            }
            Form3 f3 = new Form3(datos_de_grafico, datos_de_grafico_FO, cantidad_de_repeticiones, maximas_generaciones, dimensiones, cantidad_de_proteinas);
            f3.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (seleccion_para_graficar == "")
            {
                MessageBox.Show("No ha seleccionado una ejecución para graficar.",
                                "Error en datos para graficar",
                                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.RtlReading);
                return;
            }
            GraficoConvergenciaPorDimension f3 = new GraficoConvergenciaPorDimension(datos_de_grafico, cantidad_de_repeticiones, maximas_generaciones, dimensiones, cantidad_de_proteinas);
            f3.ShowDialog();
        }
    }
    
}
