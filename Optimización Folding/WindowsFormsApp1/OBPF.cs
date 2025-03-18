#define IMPRIME_CODIGO_INTERMEDIO 
//#define CALCULO_DE_SHANON
#define CODIGO_CON_VENTANAS_DE_EXPLICACION
//#define POSIBILIDAD_DE_GRAFICAR

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

        public const string FORMATO_POSICIONES = "E4";
        public const string FORMATO_POSICIONES_INTERMEDIO = "F4";
        public const string FORMATO_VALORES_FUNCION = "E1";

        public const int MAX_PROTEINAS = 51;
        public const int MAX_AMINOACIDOS = 51;
        public const int MAX_PLIEGUES = 31;
        public const int MAX_REPETICIONES = 51;
        public const int MAX_GENERACIONES = 5001;
        public const int MAX_DATOS_INTEGRADOS = 5001;

        public const long tamano_por_archivo = 1048576000;
        
        public enum Tipos_aminoacidos { HIDROFOBICO = 1, POLAR = 0, INTERNO, SUPERFICIAL }
        public struct proteina
        {
            public string nombre_proteina;
            public double[] aminoacidos_actual;
            public double[] aminoacidos_reserva;
            public double[] aminoacidos_mejor;
            public double valor_funcion_actual;
            public double valor_funcion_reserva;
            public double mejor_valor_de_la_funcion;
            public Tipos_aminoacidos[] SI;
            public Tipos_aminoacidos[] PH;
            public int[] densidad_local;
            public ArrayList quienes_son_PS;
            public ArrayList quienes_son_PI;
            public ArrayList quienes_son_HS;
            public ArrayList quienes_son_HI;
            public bool hay_chaperona;
            public ArrayList chaperonas;
            public bool hay_cuaternarias;
            public ArrayList cuaternarias;
            public ArrayList quienes_disparo_cascada;
#if IMPRIME_CODIGO_INTERMEDIO
            public ArrayList acciones;
#endif
        }

        public datos_de_corrida datos;

        public const int maximo_de_chaperonas = 4;
        public const int Maximo_numero_de_proteinas_para_operador_cuaternario = 6;
        public const double Probabilidad_final_para_aplicar_el_operador_cuaternario = 1.0;

        public struct datos_de_corrida
        {
            public proteina[] proteinas;
            public int cantidad_de_proteinas;
            public Funciones f;
            public double[] lb;
            public double[] ub;
            public Funciones.tipo_funciones funcion;
            public string nombre_de_la_funcion;
            public string nombre_de_la_funcion_detalles;
            public int dimensiones;
            public int semilla_aleatorio;
            public double mejor_valor_global;
            public double[] mejor_posicion_global;
            public double h_actual;
            public double h_inicial;
            public double h_final;
            public int Ro_corte;
            public int Q;
            public double PfH;
            public double PfP;
            public double Pnucleacion_H;
            public double Pnucleacion_P;
            public double Probabilidad_Hidrofobico_Polar_minima;
            public double Probabilidad_Hidrofobico_Polar_maxima;
            public double omega_inicial;
            public double omega_final;
            public double omega_actual;
            public int generacion_actual;
            public int maximas_generaciones;
            public StreamWriter archivo_de_salida;
            public double probabilidad_de_chaperonas;
            public bool chaperonas_entre_mejores;
            public Stopwatch stopWatch_por_repeticion;
            public Stopwatch stopWatch_por_corrida_total;
            public double probabilidad_de_aceptar_estados_iniciales;
            public double probabilidad_de_aceptar_estados_finales;
            public double temperatura_inicial;
            public double temperatura_final;
            public double temperatura_actual;
            public int repeticion_actual;
            public int cantidad_de_repeticiones;
            public bool ejecucion_por_lotes;
            public double probabilidad_de_colapso_H;
            public double probabilidad_de_interaccion_electrostatica_P;
            public double porciento_de_aminoacidos_de_colapso_H;
            public double porciento_de_aminoacidos_de_interaccion_electrostatica_P;
            public int numero_de_proteinas_para_proteosoma;
            public double porciento_de_proteinas_para_proteosoma;
            public string nombre_del_archivo_de_salida;
            public System.IO.FileInfo fichero_de_salida_informacion;
            public int continua_archivo;
            public long maximo_numero_de_evaluaciones_de_la_funcion;
            public long cantidad_de_evaluaciones_de_la_FO_actual;
            public bool terminar_por_iteraciones_maximas;
            public double Probabilidad_de_desnaturalizacion_PH;
            public double Balance_en_desnaturalizacion_de_H_a_P;
            public double Porciento_de_las_iteraciones_o_evaluaciones_para_aplicar_desnaturalizacion_de_H_a_P;
            public double ultimo_Porciento_de_las_iteraciones_o_evaluaciones_para_aplicar_desnaturalizacion_de_H_a_P;
            public double Porciento_maximo_de_aminoacidos_a_cambiar_por_proteina_en_desnaturalizacion;
            public double Porciento_de_iteraciones_o_evaluaciones_para_cambiar_las_H_y_las_P;
            public double ultimo_Porciento_de_iteraciones_o_evaluaciones_para_cambiar_las_H_y_las_P;
            public double probabilidad_inicial_para_cambiar_las_H_y_las_P;
            public double probabilidad_final_para_cambiar_las_H_y_las_P;
            public double probabilidad_actual_para_cambiar_las_H_y_las_P;
            public double Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_cuaternario;
            public double ultimo_Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_cuaternario;
            public double Probabilidad_inicial_para_aplicar_el_operador_cuaternario;
            public double Probabilidad_actual_para_aplicar_el_operador_cuaternario;
            public double Probabilidad_de_movimiento_aleatorio;
            public string linea_de_comandos_para_escribir;
            public double Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_H;
            public double ultimo_Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_H;
            public double Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_P;
            public double ultimo_Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_P;
            public int aminoacidos_modificables;
            public int cantidad_proteinas_por_paso;
            public Random aleatorio;
            public TimeSpan ts;
            public double tiempo;
        }

#if POSIBILIDAD_DE_GRAFICAR

        public struct valores_de_FO
        {
            public float mejor_valor_de_FO;
            public float evaluaciones_de_la_FO;
        }
        public struct grafico_de_una_proteina
        {
            public string nombre_proteina;
            public string[,] posiciones_de_aminoacidos_por_proteina_repeticion_y_generacion;
            public float[,] valor_de_la_FO_por_proteina_repeticion_y_generacion;
        }
        public grafico_de_una_proteina[] datos_de_grafico;
        public valores_de_FO[,] datos_de_grafico_FO;
        public StreamWriter archivo_de_salida_grafico;
        public long iteraciones_reales;
#endif

        public void inicia_datos_de_corrida(ref datos_de_corrida datos)
        {
            datos.proteinas = new proteina[MAX_PROTEINAS];
            for (int i = 1; i < MAX_PROTEINAS; i++)
            {
                datos.proteinas[i].nombre_proteina = "";
                datos.proteinas[i].aminoacidos_actual = new double[MAX_AMINOACIDOS];
                datos.proteinas[i].aminoacidos_reserva = new double[MAX_AMINOACIDOS];
                datos.proteinas[i].aminoacidos_mejor = new double[MAX_AMINOACIDOS];
                datos.proteinas[i].valor_funcion_actual = double.MaxValue;
                datos.proteinas[i].valor_funcion_reserva = double.MaxValue;
                datos.proteinas[i].mejor_valor_de_la_funcion = double.MaxValue;
                datos.proteinas[i].SI = new Tipos_aminoacidos[MAX_AMINOACIDOS];
                datos.proteinas[i].PH = new Tipos_aminoacidos[MAX_AMINOACIDOS];
                datos.proteinas[i].densidad_local = new int[MAX_AMINOACIDOS];
                datos.proteinas[i].quienes_son_PS = new ArrayList();
                datos.proteinas[i].quienes_son_PI = new ArrayList(); 
                datos.proteinas[i].quienes_son_HS = new ArrayList();
                datos.proteinas[i].quienes_son_HI = new ArrayList();
                datos.proteinas[i].hay_chaperona = false;
                datos.proteinas[i].chaperonas = new ArrayList();
                datos.proteinas[i].hay_cuaternarias = false;
                datos.proteinas[i].cuaternarias = new ArrayList();
                datos.proteinas[i].quienes_disparo_cascada = new ArrayList();
#if IMPRIME_CODIGO_INTERMEDIO
                datos.proteinas[i].acciones  = new ArrayList();
#endif
            }
            datos.cantidad_de_proteinas=0;
            //public Funciones f;
            datos.lb = new double[MAX_AMINOACIDOS];
            datos.ub = new double[MAX_AMINOACIDOS];
            //public Funciones.tipo_funciones funcion;
            datos.nombre_de_la_funcion = "";
            datos.nombre_de_la_funcion_detalles = "";
            datos.dimensiones = 0;
            datos.semilla_aleatorio = 0;
            datos.mejor_valor_global = double.MaxValue;
            datos.mejor_posicion_global = new double[MAX_AMINOACIDOS];
            datos.h_actual = 0.0;
            datos.h_inicial = 0.0;
            datos.h_final = 0.0;
            datos.Ro_corte = 0;
            datos.Q = 0;
            datos.PfH = 0.0;
            datos.PfP = 0.0;
            datos.Pnucleacion_H = 0.0;
            datos.Pnucleacion_P = 0.0;
            datos.Probabilidad_Hidrofobico_Polar_minima = 0.0;
            datos.Probabilidad_Hidrofobico_Polar_maxima = 0.0;
            datos.omega_inicial = 0.0;
            datos.omega_final = 0.0;
            datos.omega_actual = 0.0;
            datos.generacion_actual = 0;
            datos.maximas_generaciones = 0;
            datos.archivo_de_salida = null;
            datos.probabilidad_de_chaperonas = 0.0;
            datos.chaperonas_entre_mejores = true;
            datos.stopWatch_por_repeticion = new Stopwatch();
            datos.stopWatch_por_corrida_total = new Stopwatch();
            datos.probabilidad_de_aceptar_estados_iniciales = 0.0;
            datos.probabilidad_de_aceptar_estados_finales = 0.0;
            datos.temperatura_inicial = 0.0;
            datos.temperatura_final = 0.0;
            datos.temperatura_actual = 0.0;
            datos.repeticion_actual = 1;
            datos.cantidad_de_repeticiones = 0;
            datos.ejecucion_por_lotes = false;
            datos.probabilidad_de_colapso_H = 0.0;
            datos.probabilidad_de_interaccion_electrostatica_P = 0.0;
            datos.porciento_de_aminoacidos_de_colapso_H = 0.0;
            datos.porciento_de_aminoacidos_de_interaccion_electrostatica_P = 0.0;
            datos.numero_de_proteinas_para_proteosoma = 0;
            datos.porciento_de_proteinas_para_proteosoma = 0.0;
            datos.nombre_del_archivo_de_salida = "";
            datos.fichero_de_salida_informacion = null;
            datos.continua_archivo = 0;
            datos.maximo_numero_de_evaluaciones_de_la_funcion = 0;
            datos.cantidad_de_evaluaciones_de_la_FO_actual = 0;
            datos.terminar_por_iteraciones_maximas = true;
            datos.Probabilidad_de_desnaturalizacion_PH = 0.0;
            datos.Balance_en_desnaturalizacion_de_H_a_P = 0.0;
            datos.Porciento_de_las_iteraciones_o_evaluaciones_para_aplicar_desnaturalizacion_de_H_a_P = 0.0;
            datos.ultimo_Porciento_de_las_iteraciones_o_evaluaciones_para_aplicar_desnaturalizacion_de_H_a_P = 0.0;
            datos.Porciento_maximo_de_aminoacidos_a_cambiar_por_proteina_en_desnaturalizacion = 0.0;
            datos.Porciento_de_iteraciones_o_evaluaciones_para_cambiar_las_H_y_las_P = 0.0;
            datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_cambiar_las_H_y_las_P = 0.0;
            datos.probabilidad_inicial_para_cambiar_las_H_y_las_P = 0.0;
            datos.probabilidad_final_para_cambiar_las_H_y_las_P = 0.0;
            datos.probabilidad_actual_para_cambiar_las_H_y_las_P = 0.0;
            datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_cuaternario = 0.0;
            datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_cuaternario = 0.0;
            datos.Probabilidad_inicial_para_aplicar_el_operador_cuaternario = 0.0;
            datos.Probabilidad_actual_para_aplicar_el_operador_cuaternario = 0.0;
            datos.Probabilidad_de_movimiento_aleatorio = 0.0;
            datos.linea_de_comandos_para_escribir = "";
            datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_H = 0.0;
            datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_H = 0.0;
            datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_P = 0.0;
            datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_P = 0.0;
            datos.aminoacidos_modificables = 0;
            datos.cantidad_proteinas_por_paso = 0;
            datos.aleatorio = new Random();
            datos.ts = new TimeSpan();
            datos.tiempo = 0.0;
    }
   

#if CALCULO_DE_SHANON
        public int[] suma_de_H_por_aminoacidos = new int[MAX_AMINOACIDOS];
        public int[] suma_de_H_por_proteinas = new int[MAX_PROTEINAS];
        public float[] probabilidad_de_H_por_aminoacidos = new float[MAX_AMINOACIDOS];
        public float[] logaritmo_de_la_probabilidad_de_H_por_aminoacidos = new float[MAX_AMINOACIDOS];
        public float[] probabilidad_por_logaritmo_de_la_probabilidad_de_H_por_aminoacidos = new float[MAX_AMINOACIDOS];
        public int suma_de_H_total;
        public float suma_de_probabilidades;
        public float suma_de_logaritmos_de_probabilidades;
        public float suma_de_probabilidades_por_logaritmo;
        public float[] probabilidad_de_H_por_proteina = new float[MAX_PROTEINAS];
        public float[] suma_de_logaritmos_por_cantidad_de_H_por_proteina = new float[MAX_PROTEINAS];
        public float[] suma_de_logaritmos_por_probabilidad_de_H_por_proteina = new float[MAX_PROTEINAS];
        public float[] suma_de_logaritmos_por_probabilidad_de_H_en_aminoacido_por_proteina = new float[MAX_PROTEINAS];
        public float[] suma_de_logaritmos_por_probabilidad_de_H_en_aminoacido_por_proteina_dividido_por_cantidad_de_H = new float[MAX_PROTEINAS];
#endif

        

#if CALCULO_DE_SHANON
        public void calcula_Shanon(ref datos_de_corrida datos)
        {

            for (int j = 1; j <= datos.dimensiones; j++)
                suma_de_H_por_aminoacidos[j] = 0;
            for (int i = 1; i <= datos.cantidad_de_proteinas; i++)
            {
                suma_de_logaritmos_por_probabilidad_de_H_en_aminoacido_por_proteina[i] = 0;
                suma_de_H_por_proteinas[i] = 0;
            }
            suma_de_H_total = 0;
            for (int i = 1; i <= datos.cantidad_de_proteinas; i++)
                for (int j = 1; j <= datos.dimensiones; j++)
                {
                    suma_de_H_por_proteinas[i] += (int)(datos.proteinas[i].PH[j]);
                    suma_de_H_por_aminoacidos[j] += (int)(datos.proteinas[i].PH[j]);
                }
            for (int j = 1; j <= datos.dimensiones; j++)
                suma_de_H_total += suma_de_H_por_aminoacidos[j];
            suma_de_probabilidades = 0.0f;
            suma_de_logaritmos_de_probabilidades = 0.0f;
            suma_de_probabilidades_por_logaritmo = 0.0f;
            for (int j = 1; j <= datos.dimensiones; j++)
            {
                probabilidad_de_H_por_aminoacidos[j] = (float)suma_de_H_por_aminoacidos[j] / suma_de_H_total;
                suma_de_probabilidades += probabilidad_de_H_por_aminoacidos[j];
                logaritmo_de_la_probabilidad_de_H_por_aminoacidos[j] = -(float)Math.Log(probabilidad_de_H_por_aminoacidos[j], 2.0);
                suma_de_logaritmos_de_probabilidades += logaritmo_de_la_probabilidad_de_H_por_aminoacidos[j];
                probabilidad_por_logaritmo_de_la_probabilidad_de_H_por_aminoacidos[j] = logaritmo_de_la_probabilidad_de_H_por_aminoacidos[j] *
                                                                                        probabilidad_de_H_por_aminoacidos[j];
                suma_de_probabilidades_por_logaritmo += probabilidad_por_logaritmo_de_la_probabilidad_de_H_por_aminoacidos[j];
            }
            for (int i = 1; i <= datos.cantidad_de_proteinas; i++)
            {
                probabilidad_de_H_por_proteina[i] = (float)suma_de_H_por_proteinas[i] / suma_de_H_total;
                suma_de_logaritmos_por_cantidad_de_H_por_proteina[i] = suma_de_H_por_proteinas[i] * suma_de_probabilidades_por_logaritmo;
                suma_de_logaritmos_por_probabilidad_de_H_por_proteina[i] = probabilidad_de_H_por_proteina[i] * suma_de_probabilidades_por_logaritmo;
            }
            for (int i = 1; i <= datos.cantidad_de_proteinas; i++)
            {
                for (int j = 1; j <= datos.dimensiones; j++)
                    suma_de_logaritmos_por_probabilidad_de_H_en_aminoacido_por_proteina[i] += (int)(datos.proteinas[i].PH[j]) * logaritmo_de_la_probabilidad_de_H_por_aminoacidos[j];
                if (suma_de_H_por_proteinas[i] != 0)
                    suma_de_logaritmos_por_probabilidad_de_H_en_aminoacido_por_proteina_dividido_por_cantidad_de_H[i] =
                                    suma_de_logaritmos_por_probabilidad_de_H_en_aminoacido_por_proteina[i] / suma_de_H_por_proteinas[i];
                else
                    suma_de_logaritmos_por_probabilidad_de_H_en_aminoacido_por_proteina_dividido_por_cantidad_de_H[i] = 0;
            }

            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\nInformacion de Entropias\n");
            string cadena = "     ";
            string subcadena;
            for (int i = 1; i <= datos.dimensiones; i++)
            {
                subcadena = "A" + i.ToString();
                cadena += String.Format("{0,8:D}", subcadena);
            }
            cadena += "      sum    P(Xj)   Fj*H(X)    Pj*H(X)    I(Xj)     NRI(Xj)";
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, cadena + "\n");
            cadena = "";
            for (int i = 1; i <= datos.cantidad_de_proteinas; i++)
            {
                cadena = "P" + datos.proteinas[i].nombre_proteina.Substring(datos.proteinas[i].nombre_proteina.IndexOf(' ')).Trim();
                string espacios_anadir = "";
                while ((espacios_anadir.Length + cadena.Length) < 5)
                    espacios_anadir += " ";
                cadena = espacios_anadir + cadena;
                for (int j = 1; j <= datos.dimensiones; j++)
                    cadena += String.Format("{0,8:D}", ((int)datos.proteinas[i].PH[j]));
                cadena += String.Format("{0,9:D}", suma_de_H_por_proteinas[i]);
                cadena += String.Format("{0,9:F4}", probabilidad_de_H_por_proteina[i]);
                cadena += String.Format("{0,10:F3}", suma_de_logaritmos_por_cantidad_de_H_por_proteina[i]);
                cadena += String.Format("{0,10:F3}", suma_de_logaritmos_por_probabilidad_de_H_por_proteina[i]);
                cadena += String.Format("{0,10:F3}", suma_de_logaritmos_por_probabilidad_de_H_en_aminoacido_por_proteina[i]);
                cadena += String.Format("{0,10:F3}", suma_de_logaritmos_por_probabilidad_de_H_en_aminoacido_por_proteina_dividido_por_cantidad_de_H[i]);
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, cadena + "\n");
            }
            cadena = "F(Xj)";
            for (int j = 1; j <= datos.dimensiones; j++)
                cadena += String.Format("{0,8:D}", suma_de_H_por_aminoacidos[j]);
            cadena += String.Format("{0,9:D}", suma_de_H_total);
            float sumas = 0.0f;
            for (int i = 1; i <= datos.cantidad_de_proteinas; i++)
                sumas += probabilidad_de_H_por_proteina[i];
            cadena += String.Format("{0,9:F4}", sumas);
            sumas = 0.0f;
            for (int i = 1; i <= datos.cantidad_de_proteinas; i++)
                sumas += suma_de_logaritmos_por_cantidad_de_H_por_proteina[i];
            cadena += String.Format("{0,10:F3}", sumas);
            sumas = 0.0f;
            for (int i = 1; i <= datos.cantidad_de_proteinas; i++)
                sumas += suma_de_logaritmos_por_probabilidad_de_H_por_proteina[i];
            cadena += String.Format("{0,10:F3}", sumas);
            sumas = 0.0f;
            for (int i = 1; i <= datos.cantidad_de_proteinas; i++)
                sumas += suma_de_logaritmos_por_probabilidad_de_H_en_aminoacido_por_proteina[i];
            cadena += String.Format("{0,10:F3}", sumas);
            sumas = 0.0f;
            for (int i = 1; i <= datos.cantidad_de_proteinas; i++)
                sumas += suma_de_logaritmos_por_probabilidad_de_H_en_aminoacido_por_proteina_dividido_por_cantidad_de_H[i];
            cadena += String.Format("{0,10:F3}", sumas);
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, cadena + "\n");
            cadena = "P(Xj)";
            for (int j = 1; j <= datos.dimensiones; j++)
                cadena += String.Format("{0,8:F4}", probabilidad_de_H_por_aminoacidos[j]);
            sumas = 0.0f;
            for (int j = 1; j <= datos.dimensiones; j++)
                sumas += probabilidad_de_H_por_aminoacidos[j];
            cadena += String.Format("{0,9:F4}", sumas);
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, cadena + "\n");
            cadena = "A(Xj)";
            for (int j = 1; j <= datos.dimensiones; j++)
                cadena += String.Format("{0,8:F3}", logaritmo_de_la_probabilidad_de_H_por_aminoacidos[j]);
            sumas = 0.0f;
            for (int j = 1; j <= datos.dimensiones; j++)
                sumas += logaritmo_de_la_probabilidad_de_H_por_aminoacidos[j];
            cadena += String.Format("{0,9:F3}", sumas);
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, cadena + "\n");
            cadena = "H(Xj)";
            for (int j = 1; j <= datos.dimensiones; j++)
                cadena += String.Format("{0,8:F3}", probabilidad_por_logaritmo_de_la_probabilidad_de_H_por_aminoacidos[j]);
            sumas = 0.0f;
            for (int j = 1; j <= datos.dimensiones; j++)
                sumas += probabilidad_por_logaritmo_de_la_probabilidad_de_H_por_aminoacidos[j];
            cadena += String.Format("{0,9:F3}", sumas);
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, cadena + "\n");

        }
#endif

        /*public void QuickSort_vector_aminoacidos(ref double[] arr, int start, int end)
        {
            int i;
            if (start < end)
            {
                i = Partition_vector_aminoacidos(ref arr, start, end);

                QuickSort_vector_aminoacidos(ref arr, start, i - 1);
                QuickSort_vector_aminoacidos(ref arr, i + 1, end);
            }
        }*/

        private int Partition_vector_aminoacidos(ref double[] arr, int start, int end)
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

        public void QuickSort_vector_proteinas(ref proteina[] arr, int start, int end)
        {
            int i;
            if (start < end)
            {
                i = Partition_vector_proteinas(ref arr, start, end);

                QuickSort_vector_proteinas(ref arr, start, i - 1);
                QuickSort_vector_proteinas(ref arr, i + 1, end);
            }
        }

        private int Partition_vector_proteinas(ref proteina[] arr, int start, int end)
        {
            proteina temp;
            double p = arr[end].valor_funcion_actual;
            int i = start - 1;

            for (int j = start; j <= end - 1; j++)
            {
                if (arr[j].valor_funcion_actual <= p)
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

        public void clasifica_aminoacidos(ref datos_de_corrida datos, int proteina_num, int aminoacido)
        {
            if (datos.proteinas[proteina_num].densidad_local[aminoacido] >= datos.Ro_corte)
                datos.proteinas[proteina_num].SI[aminoacido] = Tipos_aminoacidos.INTERNO;
            else
                datos.proteinas[proteina_num].SI[aminoacido] = Tipos_aminoacidos.SUPERFICIAL;
            if (datos.proteinas[proteina_num].PH[aminoacido] == Tipos_aminoacidos.POLAR && datos.proteinas[proteina_num].SI[aminoacido] == Tipos_aminoacidos.INTERNO)
                datos.proteinas[proteina_num].quienes_son_PI.Add(aminoacido);
            else if (datos.proteinas[proteina_num].PH[aminoacido] == Tipos_aminoacidos.HIDROFOBICO && datos.proteinas[proteina_num].SI[aminoacido] == Tipos_aminoacidos.INTERNO)
                datos.proteinas[proteina_num].quienes_son_HI.Add(aminoacido);
            else if (datos.proteinas[proteina_num].PH[aminoacido] == Tipos_aminoacidos.POLAR && datos.proteinas[proteina_num].SI[aminoacido] == Tipos_aminoacidos.SUPERFICIAL)
                datos.proteinas[proteina_num].quienes_son_PS.Add(aminoacido);
            else if (datos.proteinas[proteina_num].PH[aminoacido] == Tipos_aminoacidos.HIDROFOBICO && datos.proteinas[proteina_num].SI[aminoacido] == Tipos_aminoacidos.SUPERFICIAL)
                datos.proteinas[proteina_num].quienes_son_HS.Add(aminoacido);
        }

        public int calcula_Ro(ref datos_de_corrida datos, int proteina_num, int aminoacido)
        {
            double valor_menor = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] - datos.h_actual;
            double valor_mayor = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] + datos.h_actual;
            int cuenta = 0;
            for (int i = 1; i <= datos.dimensiones; i++)
                if (i != aminoacido)
                    if (datos.proteinas[proteina_num].aminoacidos_actual[i] >= valor_menor && datos.proteinas[proteina_num].aminoacidos_actual[i] <= valor_mayor)
                        cuenta++;
            if (datos.proteinas[proteina_num].hay_chaperona)
                foreach (int chaperona in datos.proteinas[proteina_num].chaperonas)
                    for (int i = 1; i <= datos.dimensiones; i++)
                        if (datos.proteinas[chaperona].aminoacidos_actual[i] >= valor_menor && datos.proteinas[chaperona].aminoacidos_actual[i] <= valor_mayor)
                            cuenta++;
            return cuenta;
        }

        public void sintesis(ref datos_de_corrida datos)
        {
            for (int i = 1; i < MAX_PROTEINAS; i++)
            {
                datos.proteinas[i].nombre_proteina = "Proteina " + i.ToString();
                datos.proteinas[i].aminoacidos_actual = new double[MAX_AMINOACIDOS];
                datos.proteinas[i].aminoacidos_reserva = new double[MAX_AMINOACIDOS];
                datos.proteinas[i].aminoacidos_mejor = new double[MAX_AMINOACIDOS];
                datos.proteinas[i].SI = new Tipos_aminoacidos[MAX_AMINOACIDOS];
                datos.proteinas[i].PH = new Tipos_aminoacidos[MAX_AMINOACIDOS];
                datos.proteinas[i].mejor_valor_de_la_funcion = double.MaxValue;
                datos.proteinas[i].valor_funcion_actual = double.MaxValue;
                datos.proteinas[i].valor_funcion_reserva = double.MaxValue;
                datos.proteinas[i].densidad_local = new int[MAX_AMINOACIDOS];
                datos.proteinas[i].quienes_son_HI = new ArrayList();
                datos.proteinas[i].quienes_son_HS = new ArrayList();
                datos.proteinas[i].quienes_son_PS = new ArrayList();
                datos.proteinas[i].quienes_son_PI = new ArrayList();
                datos.proteinas[i].hay_chaperona = false;
                datos.proteinas[i].chaperonas = new ArrayList();
                datos.proteinas[i].hay_cuaternarias = false;
                datos.proteinas[i].cuaternarias = new ArrayList();
                datos.proteinas[i].quienes_disparo_cascada = new ArrayList();
#if IMPRIME_CODIGO_INTERMEDIO
                datos.proteinas[i].acciones = new ArrayList();
#endif
            }
            for (int j = 1; j <= datos.cantidad_de_proteinas; j++)
            {
                double posicion;
                double probabilidad_PH;
                for (int i = 1; i <= datos.dimensiones; i++)
                {
                    posicion = datos.lb[i] + datos.aleatorio.NextDouble() * (datos.ub[i] - datos.lb[i]);
                    datos.proteinas[j].aminoacidos_actual[i] = posicion;
                    probabilidad_PH = datos.Probabilidad_Hidrofobico_Polar_minima + datos.aleatorio.NextDouble() * (datos.Probabilidad_Hidrofobico_Polar_maxima - datos.Probabilidad_Hidrofobico_Polar_minima);
                    if (datos.aleatorio.NextDouble() > probabilidad_PH)
                        datos.proteinas[j].PH[i] = Tipos_aminoacidos.POLAR;
                    else
                        datos.proteinas[j].PH[i] = Tipos_aminoacidos.HIDROFOBICO;
                }
                //QuickSort_vector_aminoacidos(ref datos.proteinas[j].aminoacidos_actual, 1, datos.dimensiones);
                for (int i = 1; i <= datos.dimensiones; i++)
                {
                    datos.proteinas[j].aminoacidos_mejor[i] = datos.proteinas[j].aminoacidos_actual[i];
                    datos.proteinas[j].aminoacidos_reserva[i] = datos.proteinas[j].aminoacidos_actual[i];
                }
                datos.proteinas[j].valor_funcion_actual = datos.f.Function(datos.proteinas[j].aminoacidos_actual, datos.dimensiones, datos.funcion);
                datos.proteinas[j].mejor_valor_de_la_funcion = datos.proteinas[j].valor_funcion_actual;
                datos.proteinas[j].valor_funcion_reserva = datos.proteinas[j].valor_funcion_actual;
            }
            QuickSort_vector_proteinas(ref datos.proteinas, 1, datos.cantidad_de_proteinas);
            datos.mejor_valor_global = datos.proteinas[1].valor_funcion_actual;
            if (datos.mejor_valor_global < System.Convert.ToDouble(textBox23.Text))
            {
                textBox23.Text = datos.mejor_valor_global.ToString();
                textBox23.Refresh();
            }
            for (int i = 1; i <= datos.dimensiones; i++)
                datos.mejor_posicion_global[i] = datos.proteinas[1].aminoacidos_actual[i];
            for (int j = 1; j <= datos.cantidad_de_proteinas; j++)
            {
                datos.proteinas[j].quienes_son_HI.Clear();
                datos.proteinas[j].quienes_son_HS.Clear();
                datos.proteinas[j].quienes_son_PI.Clear();
                datos.proteinas[j].quienes_son_PS.Clear();
                for (int i = 1; i <= datos.dimensiones; i++)
                {
                    datos.proteinas[j].densidad_local[i] = calcula_Ro(ref datos, j, i);
                    clasifica_aminoacidos(ref datos, j, i);
                }
            }
        }

        public double temperatura_por_iteraciones(datos_de_corrida datos)
        {
            return (datos.temperatura_inicial - ((datos.temperatura_inicial - datos.temperatura_final) * datos.generacion_actual / datos.maximas_generaciones));
        }

        public double temperatura_por_evaluaciones(datos_de_corrida datos)
        {
            return (datos.temperatura_inicial - ((datos.temperatura_inicial - datos.temperatura_final) * datos.cantidad_de_evaluaciones_de_la_FO_actual / datos.maximo_numero_de_evaluaciones_de_la_funcion));
        }


        public double probabilidad_de_cambio_de_H_y_P_por_iteraciones(datos_de_corrida datos)
        {
            return (datos.probabilidad_inicial_para_cambiar_las_H_y_las_P -
                           ((datos.probabilidad_inicial_para_cambiar_las_H_y_las_P - datos.probabilidad_final_para_cambiar_las_H_y_las_P) * datos.generacion_actual / datos.maximas_generaciones));
        }

        public double probabilidad_de_cambio_de_H_y_P_por_evaluaciones(datos_de_corrida datos)
        {
            return (datos.probabilidad_inicial_para_cambiar_las_H_y_las_P -
                           ((datos.probabilidad_inicial_para_cambiar_las_H_y_las_P - datos.probabilidad_final_para_cambiar_las_H_y_las_P) * datos.cantidad_de_evaluaciones_de_la_FO_actual
                             / datos.maximo_numero_de_evaluaciones_de_la_funcion));
        }
        public double omega_por_iteraciones(datos_de_corrida datos)
        {
            return (datos.omega_inicial - ((datos.omega_inicial - datos.omega_final) * datos.generacion_actual / datos.maximas_generaciones));
        }

        public double omega_por_evaluaciones(datos_de_corrida datos)
        {
            return (datos.omega_inicial - ((datos.omega_inicial - datos.omega_final) * datos.cantidad_de_evaluaciones_de_la_FO_actual / datos.maximo_numero_de_evaluaciones_de_la_funcion));
        }
        public double h_por_iteraciones(datos_de_corrida datos)
        {
            return (datos.h_inicial - ((datos.h_inicial - datos.h_final) * datos.generacion_actual / datos.maximas_generaciones));
        }

        public double h_por_evaluaciones(datos_de_corrida datos)
        {
            return (datos.h_inicial - ((datos.h_inicial - datos.h_final) * datos.cantidad_de_evaluaciones_de_la_FO_actual / datos.maximo_numero_de_evaluaciones_de_la_funcion));
        }

        public bool operador_Folding_H(ref datos_de_corrida datos, int proteina_num)
        {
            if (datos.proteinas[proteina_num].quienes_son_HS.Count == 0)
                return (false);
            int aminoacido_seleccionado;
            int valor_aleatorio = datos.aleatorio.Next(datos.proteinas[proteina_num].quienes_son_HS.Count);
            aminoacido_seleccionado = (int)datos.proteinas[proteina_num].quienes_son_HS[valor_aleatorio];
            int densidad_mayor = int.MinValue;
            int aminoacido_de_densidad_mayor = 0;
            foreach (int i in datos.proteinas[proteina_num].quienes_son_HI)
                if (densidad_mayor < datos.proteinas[proteina_num].densidad_local[i])
                {
                    densidad_mayor = datos.proteinas[proteina_num].densidad_local[i];
                    aminoacido_de_densidad_mayor = i;
                }
            foreach (int i in datos.proteinas[proteina_num].quienes_son_PI)
                if (densidad_mayor < datos.proteinas[proteina_num].densidad_local[i])
                {
                    densidad_mayor = datos.proteinas[proteina_num].densidad_local[i];
                    aminoacido_de_densidad_mayor = i;
                }
            if (aminoacido_de_densidad_mayor == 0 || aminoacido_de_densidad_mayor == aminoacido_seleccionado)
                return (false);
#if IMPRIME_CODIGO_INTERMEDIO
            double valor_aleatorio_operacion = datos.aleatorio.NextDouble();
            double valor_aleatorio_if = datos.aleatorio.NextDouble();
            double resultado;
            string cadena_accion;
            string cadena_proteinas;
            if (valor_aleatorio_if < 0.5)
            {
                resultado = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor] + valor_aleatorio_operacion * datos.omega_actual;
                cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                cadena_accion = crea_mensaje_accion("Folding H el aminoacido ", aminoacido_seleccionado, aminoacido_de_densidad_mayor,
                                                    resultado, datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor], " + ",
                                                    valor_aleatorio_if, datos.omega_actual);
                datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = resultado;
                datos.proteinas[proteina_num].acciones.Add(cadena_accion);
                cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
            }
            else
            {
                resultado = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor] - valor_aleatorio_operacion * datos.omega_actual;
                cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                cadena_accion = crea_mensaje_accion("Folding H el aminoacido ", aminoacido_seleccionado, aminoacido_de_densidad_mayor,
                                                    resultado, datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor], " - ",
                                                    valor_aleatorio_if, datos.omega_actual);
                datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = resultado;
                datos.proteinas[proteina_num].acciones.Add(cadena_accion);
                cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
            }
#else
            if (datos.aleatorio.NextDouble() < 0.5)
                datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor] + datos.aleatorio.NextDouble() * datos.omega_actual;
            else
                datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor] - datos.aleatorio.NextDouble() * datos.omega_actual;
#endif
            if (datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] < datos.lb[aminoacido_seleccionado] ||
                    datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] > datos.ub[aminoacido_seleccionado])
                    datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = datos.lb[aminoacido_seleccionado]
                                                                + datos.aleatorio.NextDouble() * (datos.ub[aminoacido_seleccionado] - datos.lb[aminoacido_seleccionado]);
            return (true);
        }

        public bool operador_Folding_P(ref datos_de_corrida datos, int proteina_num)
        {
            if (datos.proteinas[proteina_num].quienes_son_PI.Count == 0)
                return (false);
            int aminoacido_seleccionado;
            int valor_aleatorio = datos.aleatorio.Next(datos.proteinas[proteina_num].quienes_son_PI.Count);
            aminoacido_seleccionado = (int)datos.proteinas[proteina_num].quienes_son_PI[valor_aleatorio];
            int densidad_menor = int.MaxValue;
            int aminoacido_de_densidad_menor = 0;
            foreach (int i in datos.proteinas[proteina_num].quienes_son_HS)
                if (densidad_menor > datos.proteinas[proteina_num].densidad_local[i])
                {
                    densidad_menor = datos.proteinas[proteina_num].densidad_local[i];
                    aminoacido_de_densidad_menor = i;
                }
            foreach (int i in datos.proteinas[proteina_num].quienes_son_PS)
                if (densidad_menor > datos.proteinas[proteina_num].densidad_local[i])
                {
                    densidad_menor = datos.proteinas[proteina_num].densidad_local[i];
                    aminoacido_de_densidad_menor = i;
                }
            if (aminoacido_de_densidad_menor == 0 || aminoacido_de_densidad_menor == aminoacido_seleccionado)
                return (false);
#if IMPRIME_CODIGO_INTERMEDIO
            double valor_aleatorio_operacion = datos.aleatorio.NextDouble();
            double valor_aleatorio_if = datos.aleatorio.NextDouble();
            double resultado;
            string cadena_accion;
            string cadena_proteinas;
            if (valor_aleatorio_if < 0.5)
            {
                resultado = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor] + valor_aleatorio_operacion * datos.omega_actual;
                cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                cadena_accion = crea_mensaje_accion("Folding P el aminoacido ", aminoacido_seleccionado, aminoacido_de_densidad_menor,
                                                    resultado, datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor], " + ",
                                                    valor_aleatorio_if, datos.omega_actual);
                datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = resultado;
                datos.proteinas[proteina_num].acciones.Add(cadena_accion);
                cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
            }
            else
            {
                resultado = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor] - valor_aleatorio_operacion * datos.omega_actual;
                cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                cadena_accion = crea_mensaje_accion("Folding P el aminoacido ", aminoacido_seleccionado, aminoacido_de_densidad_menor,
                                                    resultado, datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor], " - ",
                                                    valor_aleatorio_if, datos.omega_actual);
                datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = resultado;
                datos.proteinas[proteina_num].acciones.Add(cadena_accion);
                cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
            }
#else
            if (datos.aleatorio.NextDouble() < 0.5)
                datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor] + datos.aleatorio.NextDouble() * datos.omega_actual;
            else
                datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor] - datos.aleatorio.NextDouble() * datos.omega_actual;
#endif
            if (datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] < datos.lb[aminoacido_seleccionado] ||
                    datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] > datos.ub[aminoacido_seleccionado])
                datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = datos.lb[aminoacido_seleccionado]
                                                                + datos.aleatorio.NextDouble() * (datos.ub[aminoacido_seleccionado] - datos.lb[aminoacido_seleccionado]);
            return (true);
        }

        private bool nuclear_HS(ref datos_de_corrida datos, int proteina_num, int aminoacido_de_inicio, int aminoacido_final)
        {
            int densidad_mayor = int.MinValue;
            int aminoacido_de_densidad_mayor = 0;
            for (int aminoacido = aminoacido_de_inicio; aminoacido <= aminoacido_final; aminoacido++)
            {
                if (datos.proteinas[proteina_num].quienes_son_HS.Contains(aminoacido))
                {
                    if (densidad_mayor < datos.proteinas[proteina_num].densidad_local[aminoacido])
                    {
                        densidad_mayor = datos.proteinas[proteina_num].densidad_local[aminoacido];
                        aminoacido_de_densidad_mayor = aminoacido;
                    }
                }
                else if (datos.proteinas[proteina_num].quienes_son_HI.Contains(aminoacido))
                {
                    if (densidad_mayor < datos.proteinas[proteina_num].densidad_local[aminoacido])
                    {
                        densidad_mayor = datos.proteinas[proteina_num].densidad_local[aminoacido];
                        aminoacido_de_densidad_mayor = aminoacido;
                    }
                }
            }
            if (aminoacido_de_densidad_mayor == 0)
                return (false);
#if IMPRIME_CODIGO_INTERMEDIO
            double valor_aleatorio_operacion;
            double valor_aleatorio_if;
            string cadena_accion;
            string cadena_proteinas;
            double resultado;
            for (int aminoacido = aminoacido_de_inicio; aminoacido <= aminoacido_final; aminoacido++)
                if (aminoacido != aminoacido_de_densidad_mayor)
                {
                    valor_aleatorio_operacion = datos.aleatorio.NextDouble();
                    valor_aleatorio_if = datos.aleatorio.NextDouble();
                    if (valor_aleatorio_if < 0.5)
                    {
                        resultado = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor] + valor_aleatorio_operacion * datos.omega_actual;
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                        cadena_accion = crea_mensaje_accion("Nucleacion H del aminoacido ", aminoacido, aminoacido_de_densidad_mayor,
                                                            resultado, datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor], " + ",
                                                            valor_aleatorio_if, datos.omega_actual);
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] = resultado;
                        datos.proteinas[proteina_num].acciones.Add(cadena_accion);
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                    }
                    else
                    {
                        resultado = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor] - valor_aleatorio_operacion * datos.omega_actual;
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                        cadena_accion = crea_mensaje_accion("Nucleacion H del aminoacido ", aminoacido, aminoacido_de_densidad_mayor,
                                                            resultado, datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor], " - ",
                                                            valor_aleatorio_if, datos.omega_actual);
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] = resultado;
                        datos.proteinas[proteina_num].acciones.Add(cadena_accion);
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                    }
                    if (datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] < datos.lb[aminoacido] ||
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] > datos.ub[aminoacido])
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] = datos.lb[aminoacido]
                                  + datos.aleatorio.NextDouble() * (datos.ub[aminoacido] - datos.lb[aminoacido]);
                }
#else
            for (int aminoacido = aminoacido_de_inicio; aminoacido <= aminoacido_final; aminoacido++)
                if (datos.proteinas[proteina_num].quienes_son_HS.Contains(aminoacido) && aminoacido != aminoacido_de_densidad_mayor)
                {
                    if (datos.aleatorio.NextDouble() < 0.5)
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor] + datos.aleatorio.NextDouble() * datos.omega_actual;
                    else
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor] - datos.aleatorio.NextDouble() * datos.omega_actual;
                    if (datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] < datos.lb[aminoacido] ||
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] > datos.ub[aminoacido])
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] = datos.lb[aminoacido]
                                  + datos.aleatorio.NextDouble() * (datos.ub[aminoacido] - datos.lb[aminoacido]);
                }
#endif
            return (true);
        }

        public bool operador_de_nucleacion_H(ref datos_de_corrida datos, int proteina_num)
        {
            if (datos.proteinas[proteina_num].quienes_son_HS.Count == 0)
                return (false);
            string cadena = "";
            for (int i = 1; i <= datos.dimensiones; i++)
            {
                if (datos.proteinas[proteina_num].quienes_son_HI.Contains(i))
                    cadena += "HI";
                else if (datos.proteinas[proteina_num].quienes_son_HS.Contains(i))
                    cadena += "HS";
                else if (datos.proteinas[proteina_num].quienes_son_PI.Contains(i))
                    cadena += "PI";
                else if (datos.proteinas[proteina_num].quienes_son_PS.Contains(i))
                    cadena += "PS";
            }
            MatchCollection secuencias_HI;
            MatchCollection secuencias_3 = Regex.Matches(cadena, @"H[S|I]H[S|I]H[S|I]");
            ArrayList secuencia_final_3 = new ArrayList();
            for (int i = 0; i < secuencias_3.Count; i++)
                if (secuencias_3[i].Value.Contains("HS"))
                {
                    secuencias_HI = Regex.Matches(secuencias_3[i].Value, @"HI");
                    if (secuencias_HI.Count == 1)
                        secuencia_final_3.Add(secuencias_3[i].Value);
                }
            MatchCollection secuencias_4 = Regex.Matches(cadena, @"H[S|I]H[S|I]H[S|I]H[S|I]");
            ArrayList secuencia_final_4 = new ArrayList();
            for (int i = 0; i < secuencias_4.Count; i++)
                if (secuencias_4[i].Value.Contains("HS"))
                {
                    secuencias_HI = Regex.Matches(secuencias_4[i].Value, @"HI");
                    if (secuencias_HI.Count == 1)
                        secuencia_final_4.Add(secuencias_4[i].Value);
                }
            MatchCollection secuencias_5 = Regex.Matches(cadena, @"H[S|I]H[S|I]H[S|I]H[S|I]H[S|I]");
            ArrayList secuencia_final_5 = new ArrayList();
            for (int i = 0; i < secuencias_5.Count; i++)
                if (secuencias_5[i].Value.Contains("HS"))
                {
                    secuencias_HI = Regex.Matches(secuencias_5[i].Value, @"HI");
                    if (secuencias_HI.Count == 1)
                        secuencia_final_5.Add(secuencias_5[i].Value);
                }
            MatchCollection secuencias_6 = Regex.Matches(cadena, @"H[S|I]H[S|I]H[S|I]H[S|I]H[S|I]H[S|I]");
            ArrayList secuencia_final_6 = new ArrayList();
            for (int i = 0; i < secuencias_6.Count; i++)
                if (secuencias_6[i].Value.Contains("HS"))
                {
                    secuencias_HI = Regex.Matches(secuencias_6[i].Value, @"HI");
                    if (secuencias_HI.Count == 1)
                        secuencia_final_6.Add(secuencias_6[i].Value);
                }
            if (secuencia_final_3.Count == 0 && secuencia_final_4.Count == 0 && secuencia_final_5.Count == 0 && secuencia_final_6.Count == 0)
                return (false);
            int aminoacido_de_inicio;
            int aminoacido_final;
            int posicion;
            string subcadena_de_inicio;
            ArrayList secuencia_3_inicio_fin = new ArrayList();
            ArrayList secuencia_4_inicio_fin = new ArrayList();
            ArrayList secuencia_5_inicio_fin = new ArrayList();
            ArrayList secuencia_6_inicio_fin = new ArrayList();
            if (secuencia_final_3.Count != 0)
            {
                string cadena_respaldo = cadena;
                aminoacido_final = 0;
                foreach (string subcadena in secuencia_final_3)
                {
                    posicion = cadena_respaldo.IndexOf(subcadena);
                    subcadena_de_inicio = cadena_respaldo.Substring(0, posicion);
                    aminoacido_de_inicio = subcadena_de_inicio.Length / 2 + aminoacido_final + 1;
                    aminoacido_final = aminoacido_de_inicio + subcadena.Length / 2 - 1;
                    cadena_respaldo = cadena_respaldo.Substring(subcadena_de_inicio.Length + subcadena.Length);
                    secuencia_3_inicio_fin.Add(aminoacido_de_inicio);
                    secuencia_3_inicio_fin.Add(aminoacido_final);
                }
            }
            if (secuencia_final_4.Count != 0)
            {
                string cadena_respaldo = cadena;
                aminoacido_final = 0;
                foreach (string subcadena in secuencia_final_4)
                {
                    posicion = cadena_respaldo.IndexOf(subcadena);
                    subcadena_de_inicio = cadena_respaldo.Substring(0, posicion);
                    aminoacido_de_inicio = subcadena_de_inicio.Length / 2 + aminoacido_final + 1;
                    aminoacido_final = aminoacido_de_inicio + subcadena.Length / 2 - 1;
                    cadena_respaldo = cadena_respaldo.Substring(subcadena_de_inicio.Length + subcadena.Length);
                    secuencia_4_inicio_fin.Add(aminoacido_de_inicio);
                    secuencia_4_inicio_fin.Add(aminoacido_final);
                }
            }
            if (secuencia_final_5.Count != 0)
            {
                string cadena_respaldo = cadena;
                aminoacido_final = 0;
                foreach (string subcadena in secuencia_final_5)
                {
                    posicion = cadena_respaldo.IndexOf(subcadena);
                    subcadena_de_inicio = cadena_respaldo.Substring(0, posicion);
                    aminoacido_de_inicio = subcadena_de_inicio.Length / 2 + aminoacido_final + 1;
                    aminoacido_final = aminoacido_de_inicio + subcadena.Length / 2 - 1;
                    cadena_respaldo = cadena_respaldo.Substring(subcadena_de_inicio.Length + subcadena.Length);
                    secuencia_5_inicio_fin.Add(aminoacido_de_inicio);
                    secuencia_5_inicio_fin.Add(aminoacido_final);
                }
            }
            if (secuencia_final_6.Count != 0)
            {
                string cadena_respaldo = cadena;
                aminoacido_final = 0;
                foreach (string subcadena in secuencia_final_6)
                {
                    posicion = cadena_respaldo.IndexOf(subcadena);
                    subcadena_de_inicio = cadena_respaldo.Substring(0, posicion);
                    aminoacido_de_inicio = subcadena_de_inicio.Length / 2 + aminoacido_final + 1;
                    aminoacido_final = aminoacido_de_inicio + subcadena.Length / 2 - 1;
                    cadena_respaldo = cadena_respaldo.Substring(subcadena_de_inicio.Length + subcadena.Length);
                    secuencia_6_inicio_fin.Add(aminoacido_de_inicio);
                    secuencia_6_inicio_fin.Add(aminoacido_final);
                }
            }
            int secuencia_seleccionada=0;
            if (secuencia_6_inicio_fin.Count != 0)
                secuencia_seleccionada = 6;
            else if ((secuencia_5_inicio_fin.Count != 0))
                     secuencia_seleccionada = 5;
            else if ((secuencia_4_inicio_fin.Count != 0))
                     secuencia_seleccionada = 4;
            else if ((secuencia_3_inicio_fin.Count != 0))
                    secuencia_seleccionada = 3;
            switch (secuencia_seleccionada)
            {
                    case 3:
                        if (secuencia_3_inicio_fin.Count != 0)
                        {
                            secuencia_seleccionada = datos.aleatorio.Next(secuencia_3_inicio_fin.Count / 2);
                            if (nuclear_HS(ref datos, proteina_num, 
                                (int)secuencia_3_inicio_fin[secuencia_seleccionada],
                                (int)secuencia_3_inicio_fin[secuencia_seleccionada + 1]))
                                return (true);
                        }
                        break;
                    case 4:
                        if (secuencia_4_inicio_fin.Count != 0)
                        {
                            secuencia_seleccionada = datos.aleatorio.Next(secuencia_4_inicio_fin.Count / 2);
                            if (nuclear_HS(ref datos, proteina_num, 
                                (int)secuencia_4_inicio_fin[secuencia_seleccionada],
                                (int)secuencia_4_inicio_fin[secuencia_seleccionada + 1]))
                                return (true);
                        }
                        break;
                    case 5:
                        if (secuencia_5_inicio_fin.Count != 0)
                        {
                            secuencia_seleccionada = datos.aleatorio.Next(secuencia_5_inicio_fin.Count / 2);
                            if (nuclear_HS(ref datos, proteina_num, 
                                (int)secuencia_5_inicio_fin[secuencia_seleccionada],
                                (int)secuencia_5_inicio_fin[secuencia_seleccionada + 1]))
                                return (true);
                        }
                        break;
                    case 6:
                        if (secuencia_6_inicio_fin.Count != 0)
                        {
                            secuencia_seleccionada = datos.aleatorio.Next(secuencia_6_inicio_fin.Count / 2);
                            if (nuclear_HS(ref datos, proteina_num, 
                                (int)secuencia_6_inicio_fin[secuencia_seleccionada],
                                (int)secuencia_6_inicio_fin[secuencia_seleccionada + 1]))
                                return (true);
                        }
                        break;
            }
            return (false);
        }

        private bool nuclear_PI(ref datos_de_corrida datos, int proteina_num, int aminoacido_de_inicio, int aminoacido_final)
        {
            int densidad_menor = int.MaxValue;
            int aminoacido_de_densidad_menor = 0;
            for (int aminoacido = aminoacido_de_inicio; aminoacido <= aminoacido_final; aminoacido++)
            {
                if (datos.proteinas[proteina_num].quienes_son_PI.Contains(aminoacido))
                {
                    if (densidad_menor > datos.proteinas[proteina_num].densidad_local[aminoacido])
                    {
                        densidad_menor = datos.proteinas[proteina_num].densidad_local[aminoacido];
                        aminoacido_de_densidad_menor = aminoacido;
                    }
                }
                else if (datos.proteinas[proteina_num].quienes_son_PS.Contains(aminoacido))
                { 
                    if (densidad_menor > datos.proteinas[proteina_num].densidad_local[aminoacido])
                    {
                        densidad_menor = datos.proteinas[proteina_num].densidad_local[aminoacido];
                        aminoacido_de_densidad_menor = aminoacido;
                    }
                }
            }
            if (aminoacido_de_densidad_menor == 0)
                return (false);
#if IMPRIME_CODIGO_INTERMEDIO
            double valor_aleatorio_operacion;
            double valor_aleatorio_if;
            string cadena_accion;
            string cadena_proteinas;
            double resultado;
            for (int aminoacido = aminoacido_de_inicio; aminoacido <= aminoacido_final; aminoacido++)
                if (aminoacido != aminoacido_de_densidad_menor)
                {
                    valor_aleatorio_operacion = datos.aleatorio.NextDouble();
                    valor_aleatorio_if = datos.aleatorio.NextDouble();
                    if (valor_aleatorio_if < 0.5)
                    {
                        resultado = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor] + valor_aleatorio_operacion * datos.omega_actual;
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                        cadena_accion = crea_mensaje_accion("Nucleacion P del aminoacido ", aminoacido, aminoacido_de_densidad_menor,
                                                            resultado, datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor], " + ",
                                                            valor_aleatorio_if, datos.omega_actual);
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] = resultado;
                        datos.proteinas[proteina_num].acciones.Add(cadena_accion);
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                    }
                    else
                    {
                        resultado = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor] - valor_aleatorio_operacion * datos.omega_actual;
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                        cadena_accion = crea_mensaje_accion("Nucleacion P del aminoacido ", aminoacido, aminoacido_de_densidad_menor,
                                                            resultado, datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor], " - ",
                                                            valor_aleatorio_if, datos.omega_actual);
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] = resultado;
                        datos.proteinas[proteina_num].acciones.Add(cadena_accion);
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                    }
                    if (datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] < datos.lb[aminoacido] ||
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] > datos.ub[aminoacido])
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] = datos.lb[aminoacido]
                                  + datos.aleatorio.NextDouble() * (datos.ub[aminoacido] - datos.lb[aminoacido]);
                }
#else
            for (int aminoacido = aminoacido_de_inicio; aminoacido <= aminoacido_final; aminoacido++)
                if (datos.proteinas[proteina_num].quienes_son_PI.Contains(aminoacido) && aminoacido != aminoacido_de_densidad_menor)
                {
                    if (datos.aleatorio.NextDouble() < 0.5)
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor] + datos.aleatorio.NextDouble() * datos.omega_actual;
                    else
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor] - datos.aleatorio.NextDouble() * datos.omega_actual;
                    if (datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] < datos.lb[aminoacido] ||
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] > datos.ub[aminoacido])
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido] = datos.lb[aminoacido]
                                  + datos.aleatorio.NextDouble() * (datos.ub[aminoacido] - datos.lb[aminoacido]);
                }
#endif
            return (true);
        }

        public bool operador_de_nucleacion_P(ref datos_de_corrida datos, int proteina_num)
        {
            if (datos.proteinas[proteina_num].quienes_son_PI.Count == 0)
                return (false);
            string cadena = "";
            for (int i = 1; i <= datos.dimensiones; i++)
            {
                if (datos.proteinas[proteina_num].quienes_son_HI.Contains(i))
                    cadena += "HI";
                else if (datos.proteinas[proteina_num].quienes_son_HS.Contains(i))
                    cadena += "HS";
                else if (datos.proteinas[proteina_num].quienes_son_PI.Contains(i))
                    cadena += "PI";
                else if (datos.proteinas[proteina_num].quienes_son_PS.Contains(i))
                    cadena += "PS";
            }
            MatchCollection secuencias_3 = Regex.Matches(cadena, @"P[S|I]P[S|I]P[S|I]");
            MatchCollection secuencias_PS;
            ArrayList secuencia_final_3 = new ArrayList();
            for (int i = 0; i < secuencias_3.Count; i++)
                if (secuencias_3[i].Value.Contains("PI"))
                {
                    secuencias_PS = Regex.Matches(secuencias_3[i].Value, @"PS");
                    if (secuencias_PS.Count == 1)
                        secuencia_final_3.Add(secuencias_3[i].Value);
                }
            MatchCollection secuencias_4 = Regex.Matches(cadena, @"P[S|I]P[S|I]P[S|I]P[S|I]");
            ArrayList secuencia_final_4 = new ArrayList();
            for (int i = 0; i < secuencias_4.Count; i++)
                if (secuencias_4[i].Value.Contains("PI"))
                {
                    secuencias_PS = Regex.Matches(secuencias_4[i].Value, @"PS");
                    if (secuencias_PS.Count == 1)
                        secuencia_final_4.Add(secuencias_4[i].Value);
                }
            MatchCollection secuencias_5 = Regex.Matches(cadena, @"P[S|I]P[S|I]P[S|I]P[S|I]P[S|I]");
            ArrayList secuencia_final_5 = new ArrayList();
            for (int i = 0; i < secuencias_5.Count; i++)
                if (secuencias_5[i].Value.Contains("PI"))
                {
                    secuencias_PS = Regex.Matches(secuencias_5[i].Value, @"PS");
                    if (secuencias_PS.Count == 1)
                        secuencia_final_5.Add(secuencias_5[i].Value);
                }
            MatchCollection secuencias_6 = Regex.Matches(cadena, @"P[S|I]P[S|I]P[S|I]P[S|I]P[S|I]P[S|I]");
            ArrayList secuencia_final_6 = new ArrayList();
            for (int i = 0; i < secuencias_6.Count; i++)
                if (secuencias_6[i].Value.Contains("PI"))
                {
                    secuencias_PS = Regex.Matches(secuencias_6[i].Value, @"PS");
                    if (secuencias_PS.Count == 1)
                        secuencia_final_6.Add(secuencias_6[i].Value);
                }
            if (secuencia_final_3.Count == 0 && secuencia_final_4.Count == 0 && secuencia_final_5.Count == 0 && secuencia_final_6.Count == 0)
                return (false);
            int aminoacido_de_inicio;
            int aminoacido_final;
            int posicion;
            string subcadena_de_inicio;
            ArrayList secuencia_3_inicio_fin = new ArrayList();
            ArrayList secuencia_4_inicio_fin = new ArrayList();
            ArrayList secuencia_5_inicio_fin = new ArrayList();
            ArrayList secuencia_6_inicio_fin = new ArrayList();
            if (secuencia_final_3.Count != 0)
            {
                string cadena_respaldo = cadena;
                aminoacido_final = 0;
                foreach (string subcadena in secuencia_final_3)
                {
                    posicion = cadena_respaldo.IndexOf(subcadena);
                    subcadena_de_inicio = cadena_respaldo.Substring(0, posicion);
                    aminoacido_de_inicio = subcadena_de_inicio.Length / 2 + aminoacido_final + 1;
                    aminoacido_final = aminoacido_de_inicio + subcadena.Length / 2 - 1;
                    cadena_respaldo = cadena_respaldo.Substring(subcadena_de_inicio.Length + subcadena.Length);
                    secuencia_3_inicio_fin.Add(aminoacido_de_inicio);
                    secuencia_3_inicio_fin.Add(aminoacido_final);
                }
            }
            if (secuencia_final_4.Count != 0)
            {
                string cadena_respaldo = cadena;
                aminoacido_final = 0;
                foreach (string subcadena in secuencia_final_4)
                {
                    posicion = cadena_respaldo.IndexOf(subcadena);
                    subcadena_de_inicio = cadena_respaldo.Substring(0, posicion);
                    aminoacido_de_inicio = subcadena_de_inicio.Length / 2 + aminoacido_final + 1;
                    aminoacido_final = aminoacido_de_inicio + subcadena.Length / 2 - 1;
                    cadena_respaldo = cadena_respaldo.Substring(subcadena_de_inicio.Length + subcadena.Length);
                    secuencia_4_inicio_fin.Add(aminoacido_de_inicio);
                    secuencia_4_inicio_fin.Add(aminoacido_final);
                }
            }
            if (secuencia_final_5.Count != 0)
            {
                string cadena_respaldo = cadena;
                aminoacido_final = 0;
                foreach (string subcadena in secuencia_final_5)
                {
                    posicion = cadena_respaldo.IndexOf(subcadena);
                    subcadena_de_inicio = cadena_respaldo.Substring(0, posicion);
                    aminoacido_de_inicio = subcadena_de_inicio.Length / 2 + aminoacido_final + 1;
                    aminoacido_final = aminoacido_de_inicio + subcadena.Length / 2 - 1;
                    cadena_respaldo = cadena_respaldo.Substring(subcadena_de_inicio.Length + subcadena.Length);
                    secuencia_5_inicio_fin.Add(aminoacido_de_inicio);
                    secuencia_5_inicio_fin.Add(aminoacido_final);
                }
            }
            if (secuencia_final_6.Count != 0)
            {
                string cadena_respaldo = cadena;
                aminoacido_final = 0;
                foreach (string subcadena in secuencia_final_6)
                {
                    posicion = cadena_respaldo.IndexOf(subcadena);
                    subcadena_de_inicio = cadena_respaldo.Substring(0, posicion);
                    aminoacido_de_inicio = subcadena_de_inicio.Length / 2 + aminoacido_final + 1;
                    aminoacido_final = aminoacido_de_inicio + subcadena.Length / 2 - 1;
                    cadena_respaldo = cadena_respaldo.Substring(subcadena_de_inicio.Length + subcadena.Length);
                    secuencia_6_inicio_fin.Add(aminoacido_de_inicio);
                    secuencia_6_inicio_fin.Add(aminoacido_final);
                }
            }
            int secuencia_seleccionada = 0;
            if (secuencia_6_inicio_fin.Count != 0)
                secuencia_seleccionada = 6;
            else if ((secuencia_5_inicio_fin.Count != 0))
                      secuencia_seleccionada = 5;
            else if ((secuencia_4_inicio_fin.Count != 0))
                      secuencia_seleccionada = 4;
            else if ((secuencia_3_inicio_fin.Count != 0))
                      secuencia_seleccionada = 3;
            switch (secuencia_seleccionada)
            {
                    case 3:
                        if (secuencia_3_inicio_fin.Count != 0)
                        {
                            secuencia_seleccionada = datos.aleatorio.Next(secuencia_3_inicio_fin.Count / 2);
                            if (nuclear_PI(ref datos, proteina_num, 
                                (int)secuencia_3_inicio_fin[secuencia_seleccionada],
                                (int)secuencia_3_inicio_fin[secuencia_seleccionada + 1]))
                                return (true);
                        }
                        break;
                    case 4:
                        if (secuencia_4_inicio_fin.Count != 0)
                        {
                            secuencia_seleccionada = datos.aleatorio.Next(secuencia_4_inicio_fin.Count / 2);
                            if (nuclear_PI(ref datos, proteina_num, 
                                (int)secuencia_4_inicio_fin[secuencia_seleccionada],
                                (int)secuencia_4_inicio_fin[secuencia_seleccionada + 1]))
                                return (true);
                        }
                        break;
                    case 5:
                        if (secuencia_5_inicio_fin.Count != 0)
                        {
                            secuencia_seleccionada = datos.aleatorio.Next(secuencia_5_inicio_fin.Count / 2);
                            if (nuclear_PI(ref datos, proteina_num, 
                                (int)secuencia_5_inicio_fin[secuencia_seleccionada],
                                (int)secuencia_5_inicio_fin[secuencia_seleccionada + 1]))
                                return (true);
                        }
                        break;
                    case 6:
                        if (secuencia_6_inicio_fin.Count != 0)
                        {
                            secuencia_seleccionada = datos.aleatorio.Next(secuencia_6_inicio_fin.Count / 2);
                            if (nuclear_PI(ref datos, proteina_num, 
                                (int)secuencia_6_inicio_fin[secuencia_seleccionada],
                                (int)secuencia_6_inicio_fin[secuencia_seleccionada + 1]))
                                return (true);
                        }
                        break;
            }
            return (false);
        }

        public bool operador_de_movimiento_aleatorio(ref datos_de_corrida datos, int proteina_num)
        {
            int aminoacido_seleccionado = datos.aleatorio.Next(datos.dimensiones) + 1;
            double valor = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado];
#if IMPRIME_CODIGO_INTERMEDIO
            double valor_aleatorio_operacion = datos.aleatorio.NextDouble();
            string cadena_accion;
            string cadena_proteinas;
            double resultado;
            double valor_aleatorio_if = datos.aleatorio.NextDouble();
            if (valor_aleatorio_if < 0.5)
            {
                resultado = datos.mejor_posicion_global[aminoacido_seleccionado] + valor_aleatorio_operacion * datos.omega_actual;
                cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                cadena_accion = crea_mensaje_accion("Movimiento aleatorio del aminoacido ", aminoacido_seleccionado, aminoacido_seleccionado,
                                                    resultado, datos.mejor_posicion_global[aminoacido_seleccionado], " + ",
                                                    valor_aleatorio_if, datos.omega_actual);
                datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = resultado;
                datos.proteinas[proteina_num].acciones.Add(cadena_accion);
                cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
            }
            else
            {
                resultado = datos.mejor_posicion_global[aminoacido_seleccionado] - valor_aleatorio_operacion * datos.omega_actual;
                cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                cadena_accion = crea_mensaje_accion("Movimiento aleatorio del aminoacido ", aminoacido_seleccionado, aminoacido_seleccionado,
                                                    resultado, datos.mejor_posicion_global[aminoacido_seleccionado], " - ",
                                                    valor_aleatorio_if, datos.omega_actual);
                datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = resultado;
                datos.proteinas[proteina_num].acciones.Add(cadena_accion);
                cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
            }
#endif

            if (datos.aleatorio.NextDouble() < 0.5)
                datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = datos.mejor_posicion_global[aminoacido_seleccionado] + datos.aleatorio.NextDouble() * datos.omega_actual;
            else
                datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = datos.mejor_posicion_global[aminoacido_seleccionado] - datos.aleatorio.NextDouble() * datos.omega_actual;
            if (datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] < datos.lb[aminoacido_seleccionado] ||
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] > datos.ub[aminoacido_seleccionado])
                datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = datos.lb[aminoacido_seleccionado]
                          + datos.aleatorio.NextDouble() * (datos.ub[aminoacido_seleccionado] - datos.lb[aminoacido_seleccionado]);
            return (true);
        }

        public bool operador_colapso_H(ref datos_de_corrida datos, int proteina_num)
        {
            if (datos.proteinas[proteina_num].quienes_son_HS.Count <= 2)
                return (false);
            int densidad_mayor = int.MinValue;
            int aminoacido_de_densidad_mayor = 0;
            int a_cuantos_le_aplico;
            do
            {
                a_cuantos_le_aplico = (int)Math.Ceiling(datos.aleatorio.NextDouble() * datos.dimensiones * datos.porciento_de_aminoacidos_de_colapso_H / 100.0) + 1;
            }
            while (a_cuantos_le_aplico > datos.proteinas[proteina_num].quienes_son_HS.Count);
            if (a_cuantos_le_aplico <= 2)
                return(false);
            ArrayList para_aplicar = new ArrayList();
            int valor;
            int aminoacido = 0;
            int cuantos_HS;
            do
            {
                cuantos_HS = 0;
                para_aplicar.Clear();
                do
                {
                    if (datos.aleatorio.NextDouble() < 0.5)
                    {
                        valor = datos.aleatorio.Next(datos.proteinas[proteina_num].quienes_son_HS.Count);
                        aminoacido = (int)datos.proteinas[proteina_num].quienes_son_HS[valor];
                        cuantos_HS++;
                    }
                    else
                    {
                        if (datos.proteinas[proteina_num].quienes_son_HI.Count != 0)
                        {
                            valor = datos.aleatorio.Next(datos.proteinas[proteina_num].quienes_son_HI.Count);
                            aminoacido = (int)datos.proteinas[proteina_num].quienes_son_HI[valor];
                        }
                    }
                    if (aminoacido != 0 && !para_aplicar.Contains(aminoacido))
                        para_aplicar.Add(aminoacido);
                }
                while (para_aplicar.Count < a_cuantos_le_aplico);
            }
            while (cuantos_HS < 2);
            foreach (int i in para_aplicar)
                if (densidad_mayor < datos.proteinas[proteina_num].densidad_local[i])
                {
                    densidad_mayor = datos.proteinas[proteina_num].densidad_local[i];
                    aminoacido_de_densidad_mayor = i;
                }
            foreach (int aminoacido_seleccionado in para_aplicar)
            {
                if (aminoacido_seleccionado != aminoacido_de_densidad_mayor)
                {
#if IMPRIME_CODIGO_INTERMEDIO
                    double valor_aleatorio_operacion = datos.aleatorio.NextDouble();
                    double valor_aleatorio_if = datos.aleatorio.NextDouble();
                    double resultado;
                    string cadena_accion;
                    string cadena_proteinas;
                    if (valor_aleatorio_if < 0.5)
                    {
                        resultado = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor] + valor_aleatorio_operacion * datos.omega_actual;
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                        cadena_accion = crea_mensaje_accion("Colapso H del aminoacido ", aminoacido_seleccionado, aminoacido_de_densidad_mayor,
                                                            resultado, datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor], " + ",
                                                            valor_aleatorio_if, datos.omega_actual);
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = resultado;
                        datos.proteinas[proteina_num].acciones.Add(cadena_accion);
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                    }
                    else
                    {
                        resultado = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor] - valor_aleatorio_operacion * datos.omega_actual;
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                        cadena_accion = crea_mensaje_accion("Colapso H del aminoacido ", aminoacido_seleccionado, aminoacido_de_densidad_mayor,
                                                            resultado, datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor], " - ",
                                                            valor_aleatorio_if, datos.omega_actual);
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = resultado;
                        datos.proteinas[proteina_num].acciones.Add(cadena_accion);
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                    }
#else
                    if (datos.aleatorio.NextDouble() < 0.5)
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor] + datos.aleatorio.NextDouble() * datos.omega_actual;
                    else
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_mayor] - datos.aleatorio.NextDouble() * datos.omega_actual;
#endif
                    if (datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] < datos.lb[aminoacido_seleccionado] ||
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] > datos.ub[aminoacido_seleccionado])
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = datos.lb[aminoacido_seleccionado]
                                                                        + datos.aleatorio.NextDouble() * (datos.ub[aminoacido_seleccionado] - datos.lb[aminoacido_seleccionado]);
                }
            }
            return (true);
        }

        public bool operador_interaccion_electrostática_P(ref datos_de_corrida datos, int proteina_num)
        {
            if (datos.proteinas[proteina_num].quienes_son_PI.Count <= 2)
                return (false);
            int densidad_menor = int.MaxValue;
            int aminoacido_de_densidad_menor = 0;
            int a_cuantos_le_aplico;
            do
            {
               a_cuantos_le_aplico = (int)Math.Ceiling(datos.aleatorio.NextDouble() * datos.dimensiones * datos.porciento_de_aminoacidos_de_interaccion_electrostatica_P / 100.0) + 1;
            }
            while (a_cuantos_le_aplico > datos.proteinas[proteina_num].quienes_son_PI.Count);
            if (a_cuantos_le_aplico <= 2)
                return (false);
            ArrayList para_aplicar = new ArrayList();
            int valor;
            int aminoacido = 0;
            int cuantos_PI;
            do
            {
                cuantos_PI = 0;
                para_aplicar.Clear();
                do
                {
                    if (datos.aleatorio.NextDouble() < 0.5)
                    {
                        valor = datos.aleatorio.Next(datos.proteinas[proteina_num].quienes_son_PI.Count);
                        aminoacido = (int)datos.proteinas[proteina_num].quienes_son_PI[valor];
                        cuantos_PI++;
                    }
                    else
                    {
                        if (datos.proteinas[proteina_num].quienes_son_PS.Count != 0)
                        {
                            valor = datos.aleatorio.Next(datos.proteinas[proteina_num].quienes_son_PS.Count);
                            aminoacido = (int)datos.proteinas[proteina_num].quienes_son_PS[valor];
                        }
                    }
                    if (aminoacido != 0 && !para_aplicar.Contains(aminoacido))
                        para_aplicar.Add(aminoacido);
                }
                while (para_aplicar.Count < a_cuantos_le_aplico);
            }
            while (cuantos_PI < 2);
            foreach (int i in para_aplicar)
                if (densidad_menor > datos.proteinas[proteina_num].densidad_local[i])
                {
                    densidad_menor = datos.proteinas[proteina_num].densidad_local[i];
                    aminoacido_de_densidad_menor = i;
                }
            foreach (int aminoacido_seleccionado in para_aplicar)
            {
                if (aminoacido_seleccionado != aminoacido_de_densidad_menor)
                {
#if IMPRIME_CODIGO_INTERMEDIO
                    double valor_aleatorio_operacion = datos.aleatorio.NextDouble();
                    double valor_aleatorio_if = datos.aleatorio.NextDouble();
                    double resultado;
                    string cadena_accion;
                    string cadena_proteinas;
                    if (valor_aleatorio_if < 0.5)
                    {
                        resultado = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor] + valor_aleatorio_operacion * datos.omega_actual;
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                        cadena_accion = crea_mensaje_accion("Interaccion electrostatica del aminoacido ", aminoacido_seleccionado, aminoacido_de_densidad_menor,
                                                            resultado, datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor], " + ",
                                                            valor_aleatorio_if, datos.omega_actual);
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = resultado;
                        datos.proteinas[proteina_num].acciones.Add(cadena_accion);
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                    }
                    else
                    {
                        resultado = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor] - valor_aleatorio_operacion * datos.omega_actual;
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                        cadena_accion = crea_mensaje_accion("Interaccion electrostatica del aminoacido ", aminoacido_seleccionado, aminoacido_de_densidad_menor,
                                                            resultado, datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor], " - ",
                                                            valor_aleatorio_if, datos.omega_actual);
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = resultado;
                        datos.proteinas[proteina_num].acciones.Add(cadena_accion);
                        cadena_proteinas = convierte_proteina_a_cadena(datos, proteina_num, false, false, false);
                        datos.proteinas[proteina_num].acciones.Add(cadena_proteinas);
                    }
#else
                    if (datos.aleatorio.NextDouble() < 0.5)
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor] + datos.aleatorio.NextDouble() * datos.omega_actual;
                    else
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_de_densidad_menor] - datos.aleatorio.NextDouble() * datos.omega_actual;
#endif
                    if (datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] < datos.lb[aminoacido_seleccionado] ||
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] > datos.ub[aminoacido_seleccionado])
                        datos.proteinas[proteina_num].aminoacidos_actual[aminoacido_seleccionado] = datos.lb[aminoacido_seleccionado]
                                                                        + datos.aleatorio.NextDouble() * (datos.ub[aminoacido_seleccionado] - datos.lb[aminoacido_seleccionado]);
                }
            }
            return (true);
        }

        public void metodo_de_folding_evaluando_la_funcion_al_terminar_ciclo_Q(ref datos_de_corrida datos, int proteina_num)
        {
            //Evalua la función al finalizar Q
            bool random_move;
            double valor_de_la_funcion;
            datos.proteinas[proteina_num].valor_funcion_reserva = datos.proteinas[proteina_num].valor_funcion_actual;
            for (int k = 1; k <= datos.dimensiones; k++)
                datos.proteinas[proteina_num].aminoacidos_reserva[k] = datos.proteinas[proteina_num].aminoacidos_actual[k];
            for (int q = 1; q <= datos.Q; q++)
            {
                random_move = true;
                if (datos.aleatorio.NextDouble() < datos.probabilidad_de_colapso_H)
                {
                    if (operador_colapso_H(ref datos, proteina_num))
                    {
                        datos.proteinas[proteina_num].quienes_son_HI.Clear();
                        datos.proteinas[proteina_num].quienes_son_HS.Clear();
                        datos.proteinas[proteina_num].quienes_son_PI.Clear();
                        datos.proteinas[proteina_num].quienes_son_PS.Clear();
                        for (int j = 1; j <= datos.dimensiones; j++)
                        {
                            datos.proteinas[proteina_num].densidad_local[j] = calcula_Ro(ref datos, proteina_num, j);
                            clasifica_aminoacidos(ref datos, proteina_num, j);
                        }
                        random_move = false;
                    }
                }
                if (datos.aleatorio.NextDouble() < datos.probabilidad_de_interaccion_electrostatica_P)
                {
                    if (operador_interaccion_electrostática_P(ref datos, proteina_num))
                    {
                        datos.proteinas[proteina_num].quienes_son_HI.Clear();
                        datos.proteinas[proteina_num].quienes_son_HS.Clear();
                        datos.proteinas[proteina_num].quienes_son_PI.Clear();
                        datos.proteinas[proteina_num].quienes_son_PS.Clear();
                        for (int j = 1; j <= datos.dimensiones; j++)
                        {
                            datos.proteinas[proteina_num].densidad_local[j] = calcula_Ro(ref datos, proteina_num, j);
                            clasifica_aminoacidos(ref datos, proteina_num, j);
                        }
                        random_move = false;
                    }
                }
                if (datos.aleatorio.NextDouble() < datos.PfH)
                {
                    if (operador_Folding_H(ref datos, proteina_num))
                    {
                        datos.proteinas[proteina_num].quienes_son_HI.Clear();
                        datos.proteinas[proteina_num].quienes_son_HS.Clear();
                        datos.proteinas[proteina_num].quienes_son_PI.Clear();
                        datos.proteinas[proteina_num].quienes_son_PS.Clear();
                        for (int j = 1; j <= datos.dimensiones; j++)
                        {
                            datos.proteinas[proteina_num].densidad_local[j] = calcula_Ro(ref datos, proteina_num, j);
                            clasifica_aminoacidos(ref datos, proteina_num, j);
                        }
                        random_move = false;
                    }
                }
                if (datos.aleatorio.NextDouble() < datos.PfP)
                {
                    if (operador_Folding_P(ref datos, proteina_num))
                    {
                        datos.proteinas[proteina_num].quienes_son_HI.Clear();
                        datos.proteinas[proteina_num].quienes_son_HS.Clear();
                        datos.proteinas[proteina_num].quienes_son_PI.Clear();
                        datos.proteinas[proteina_num].quienes_son_PS.Clear();
                        for (int j = 1; j <= datos.dimensiones; j++)
                        {
                            datos.proteinas[proteina_num].densidad_local[j] = calcula_Ro(ref datos, proteina_num, j);
                            clasifica_aminoacidos(ref datos, proteina_num, j);
                        }
                        random_move = false;
                    }
                }

                if (datos.aleatorio.NextDouble() < datos.Pnucleacion_H)
                    if (operador_de_nucleacion_H(ref datos, proteina_num))
                        random_move = false;
                if (datos.aleatorio.NextDouble() < datos.Pnucleacion_P)
                    if (operador_de_nucleacion_P(ref datos, proteina_num))
                        random_move = false;
                if (random_move && (datos.aleatorio.NextDouble() < datos.Probabilidad_de_movimiento_aleatorio))
                {
                    operador_de_movimiento_aleatorio(ref datos, proteina_num);
                    datos.proteinas[proteina_num].quienes_son_HI.Clear();
                    datos.proteinas[proteina_num].quienes_son_HS.Clear();
                    datos.proteinas[proteina_num].quienes_son_PI.Clear();
                    datos.proteinas[proteina_num].quienes_son_PS.Clear();
                    for (int j = 1; j <= datos.dimensiones; j++)
                    {
                        datos.proteinas[proteina_num].densidad_local[j] = calcula_Ro(ref datos, proteina_num, j);
                        clasifica_aminoacidos(ref datos, proteina_num, j);
                    }
                }
            }    
            valor_de_la_funcion = datos.f.Function(datos.proteinas[proteina_num].aminoacidos_actual, datos.dimensiones, datos.funcion);
            if (valor_de_la_funcion <= datos.proteinas[proteina_num].valor_funcion_actual)
            {
                datos.proteinas[proteina_num].valor_funcion_actual = valor_de_la_funcion;
                if (valor_de_la_funcion < datos.proteinas[proteina_num].mejor_valor_de_la_funcion)
                {
                    datos.proteinas[proteina_num].mejor_valor_de_la_funcion = datos.proteinas[proteina_num].valor_funcion_actual;
                    for (int k = 1; k <= datos.dimensiones; k++)
                        datos.proteinas[proteina_num].aminoacidos_mejor[k] = datos.proteinas[proteina_num].aminoacidos_actual[k];
                    if (datos.proteinas[proteina_num].mejor_valor_de_la_funcion < datos.mejor_valor_global)
                    {
                        datos.mejor_valor_global = datos.proteinas[proteina_num].valor_funcion_actual;
                        if (datos.mejor_valor_global < System.Convert.ToDouble(textBox23.Text))
                        {
                            textBox23.Text = datos.mejor_valor_global.ToString();
                            textBox23.Refresh();
                        }
                        for (int k = 1; k <= datos.dimensiones; k++)
                            datos.mejor_posicion_global[k] = datos.proteinas[proteina_num].aminoacidos_actual[k];
                    }
                }
            }
            else
            {
                double probabilidad_Boltzman;
                probabilidad_Boltzman = Math.Exp(-valor_de_la_funcion / datos.temperatura_actual);
                if (datos.aleatorio.NextDouble() > probabilidad_Boltzman)
                {
                    datos.proteinas[proteina_num].valor_funcion_actual = datos.proteinas[proteina_num].valor_funcion_reserva;
                    for (int k = 1; k <= datos.dimensiones; k++)
                        datos.proteinas[proteina_num].aminoacidos_actual[k] = datos.proteinas[proteina_num].aminoacidos_reserva[k];
                    datos.proteinas[proteina_num].quienes_son_HI.Clear();
                    datos.proteinas[proteina_num].quienes_son_HS.Clear();
                    datos.proteinas[proteina_num].quienes_son_PI.Clear();
                    datos.proteinas[proteina_num].quienes_son_PS.Clear();
                    for (int j = 1; j <= datos.dimensiones; j++)
                    {
                        datos.proteinas[proteina_num].densidad_local[j] = calcula_Ro(ref datos, proteina_num, j);
                        clasifica_aminoacidos(ref datos, proteina_num, j);
                    }
                }
                else
                {
                    datos.proteinas[proteina_num].valor_funcion_reserva = valor_de_la_funcion;
                    for (int k = 1; k <= datos.dimensiones; k++)
                        datos.proteinas[proteina_num].aminoacidos_reserva[k] = datos.proteinas[proteina_num].aminoacidos_actual[k];
                }
            }
        }

        
        private void datos_de_funcion(ref datos_de_corrida datos)
        {
            
            
            datos.f = new Funciones();
            if (datos.nombre_de_la_funcion.StartsWith("CEC_"))
                datos.f.inicia_datos_de_corrida_matrices_y_desplazamientos(datos.dimensiones);
            datos.f.convierte_cadena_a_nombre_de_funcion(datos.nombre_de_la_funcion, ref datos.funcion);
            datos.f.detalles_de_funcion(datos.funcion, ref datos.dimensiones, ref datos.lb, ref datos.ub, ref datos.nombre_de_la_funcion_detalles);
            datos.dimensiones = System.Convert.ToInt32(textBox5.Text);
            datos.Ro_corte = (int)Math.Ceiling(System.Convert.ToSingle(textBox2.Text) / 100.0 * datos.dimensiones);
            textBox5.Text = datos.dimensiones.ToString();
            textBox5.Refresh();
            //
            //
            // Cambio a futuro en vez de lb el tamaño del espacio de búsqueda
            //
            //
            //
            //
            //
            datos.cantidad_de_proteinas = System.Convert.ToInt16(numericUpDown4.Value);
            datos.h_inicial = (int)(System.Convert.ToSingle(textBox1.Text) / 100.0 * datos.ub[1]);
            datos.h_final = (int)(System.Convert.ToSingle(textBox25.Text) / 100.0 * datos.ub[1]);
            datos.omega_inicial = System.Convert.ToSingle(textBox15.Text) / 100.0 * datos.ub[1];
            datos.omega_final = System.Convert.ToSingle(textBox17.Text) / 100.0 * datos.ub[1];
            datos.Q = System.Convert.ToInt32(textBox7.Text);
            datos.PfH = System.Convert.ToDouble(textBox8.Text);
            datos.PfP = System.Convert.ToDouble(textBox9.Text);
            datos.Pnucleacion_H = System.Convert.ToDouble(textBox12.Text);
            datos.Pnucleacion_P = System.Convert.ToDouble(textBox19.Text);
            datos.Probabilidad_Hidrofobico_Polar_minima = System.Convert.ToDouble(textBox13.Text);
            datos.Probabilidad_Hidrofobico_Polar_maxima = System.Convert.ToDouble(textBox36.Text);
            datos.maximas_generaciones = System.Convert.ToInt32(textBox18.Text);
            datos.probabilidad_de_chaperonas = System.Convert.ToDouble(textBox20.Text);
            if (radioButton3.Checked)
                datos.chaperonas_entre_mejores = true;
            else
                datos.chaperonas_entre_mejores = false;
            datos.probabilidad_de_aceptar_estados_iniciales = System.Convert.ToDouble(textBox27.Text);
            datos.probabilidad_de_aceptar_estados_finales = System.Convert.ToDouble(textBox26.Text);
            datos.cantidad_de_repeticiones = System.Convert.ToInt16(textBox28.Text);
            datos.probabilidad_de_colapso_H = System.Convert.ToDouble(textBox31.Text);
            datos.porciento_de_aminoacidos_de_colapso_H = System.Convert.ToDouble(textBox29.Text);
            datos.probabilidad_de_interaccion_electrostatica_P = System.Convert.ToDouble(textBox33.Text);
            datos.porciento_de_aminoacidos_de_interaccion_electrostatica_P = System.Convert.ToDouble(textBox32.Text);
            datos.porciento_de_proteinas_para_proteosoma = System.Convert.ToSingle(textBox34.Text);
            datos.numero_de_proteinas_para_proteosoma = ((int)(datos.porciento_de_proteinas_para_proteosoma / 100.0 * datos.cantidad_de_proteinas + 0.5));
            datos.Probabilidad_de_desnaturalizacion_PH = System.Convert.ToDouble(textBox39.Text);
            datos.Balance_en_desnaturalizacion_de_H_a_P = System.Convert.ToDouble(textBox40.Text);
            datos.Porciento_de_las_iteraciones_o_evaluaciones_para_aplicar_desnaturalizacion_de_H_a_P = System.Convert.ToDouble(textBox41.Text);
            datos.Porciento_maximo_de_aminoacidos_a_cambiar_por_proteina_en_desnaturalizacion = System.Convert.ToDouble(textBox42.Text);
            datos.maximo_numero_de_evaluaciones_de_la_funcion = System.Convert.ToInt64(textBox38.Text);
            if (checkBox1.Checked)
                datos.terminar_por_iteraciones_maximas = false;
            else
                datos.terminar_por_iteraciones_maximas = true;
            datos.Porciento_de_iteraciones_o_evaluaciones_para_cambiar_las_H_y_las_P = System.Convert.ToDouble(textBox45.Text);
            datos.probabilidad_inicial_para_cambiar_las_H_y_las_P = System.Convert.ToDouble(textBox46.Text);
            datos.probabilidad_final_para_cambiar_las_H_y_las_P = System.Convert.ToDouble(textBox43.Text);
            datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_cuaternario = System.Convert.ToDouble(textBox48.Text);
            datos.Probabilidad_inicial_para_aplicar_el_operador_cuaternario = System.Convert.ToDouble(textBox50.Text);
            datos.Probabilidad_de_movimiento_aleatorio = System.Convert.ToDouble(textBox53.Text);
            datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_H = System.Convert.ToDouble(textBox52.Text);
            datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_P = System.Convert.ToDouble(textBox56.Text);
            textBox23.Text = "1e308";
            textBox23.Refresh();
        }

        public void imprime_proteinas(ref datos_de_corrida datos, int proteina_num, bool imprime_chaperonas, bool imprime_FO, bool imprime_cuaternarias)
        {
            string cadena;
            for (int aminoacido = 1; aminoacido <= datos.dimensiones; aminoacido++)
            {
                cadena = " (" + aminoacido.ToString() + "," + datos.proteinas[proteina_num].aminoacidos_actual[aminoacido].ToString(FORMATO_POSICIONES_INTERMEDIO);
                cadena += ",";
                cadena += datos.proteinas[proteina_num].densidad_local[aminoacido].ToString();
                if (datos.proteinas[proteina_num].quienes_son_HI.Contains(aminoacido))
                    cadena += ",HI)";
                if (datos.proteinas[proteina_num].quienes_son_HS.Contains(aminoacido))
                    cadena += ",HS)";
                if (datos.proteinas[proteina_num].quienes_son_PI.Contains(aminoacido))
                    cadena += ",PI)";
                if (datos.proteinas[proteina_num].quienes_son_PS.Contains(aminoacido))
                    cadena += ",PS)";
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, cadena);
            }
            if (imprime_chaperonas)
            {
                cadena = " Chaperonas: (";
                string subcadena;
                foreach (int c in datos.proteinas[proteina_num].chaperonas)
                {
                    subcadena = datos.proteinas[c].nombre_proteina;
                    subcadena = subcadena.Substring(datos.proteinas[c].nombre_proteina.IndexOf(' ')).Trim();
                    cadena += " " + subcadena + " ";
                }
                cadena += ")";
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, cadena);
            }
            if (imprime_cuaternarias)
            {
                cadena = " Cuaternarias: (";
                string subcadena;
                foreach (int c in datos.proteinas[proteina_num].cuaternarias)
                {
                    subcadena = datos.proteinas[c].nombre_proteina;
                    subcadena = subcadena.Substring(datos.proteinas[c].nombre_proteina.IndexOf(' ')).Trim();
                    cadena += " " + subcadena + " ";
                }
                cadena += ")";
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, cadena);
            }
            if (imprime_FO)
            {
                cadena = " FO= " + datos.proteinas[proteina_num].valor_funcion_actual.ToString(FORMATO_VALORES_FUNCION);
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, cadena);
            }
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\n");
        }

        public string crea_mensaje_accion(string tipo_de_operador,
                                          int sobre_cual_aminoacido_actua,
                                          int cual_aminoacido_se_utiliza,
                                          double resultado,
                                          double posicion_del_aminoacido,
                                          string signo,
                                          double valor_aleatorio_operacion,
                                          double omega)
        {
            string mensaje;
            if (!tipo_de_operador.Contains("Movimiento aleatorio del aminoacido "))
            {
                mensaje = tipo_de_operador;
                mensaje += sobre_cual_aminoacido_actua.ToString();
                mensaje += " toma la posicion del aminoacido ";
                mensaje += cual_aminoacido_se_utiliza.ToString();
                mensaje += " ( " + resultado.ToString(FORMATO_POSICIONES) + " = " + posicion_del_aminoacido.ToString(FORMATO_POSICIONES);
                mensaje += signo + " " + valor_aleatorio_operacion.ToString(FORMATO_POSICIONES) + " * ";
                mensaje += omega.ToString(FORMATO_POSICIONES) + " )";
            }
            else
            {
                mensaje = tipo_de_operador;
                mensaje += sobre_cual_aminoacido_actua.ToString();
                mensaje += " cambia a la posicion del mejor en la búsqueda ";
                mensaje += " ( " + resultado.ToString(FORMATO_POSICIONES) + " = " + posicion_del_aminoacido.ToString(FORMATO_POSICIONES);
                mensaje += signo + " " + valor_aleatorio_operacion.ToString(FORMATO_POSICIONES) + " * ";
                mensaje += omega.ToString(FORMATO_POSICIONES) + " )";
            }
            return (mensaje);
        }

        public string convierte_proteina_a_cadena(datos_de_corrida datos, int proteina_num, bool imprime_chaperona, bool imprime_funcion_objetivo, bool imprime_cuaternarias)
        {
            string cadena;
            string cadena_final = "";
            for (int aminoacido = 1; aminoacido <= datos.dimensiones; aminoacido++)
            {
                cadena = "(" + aminoacido.ToString() + "," + datos.proteinas[proteina_num].aminoacidos_actual[aminoacido].ToString(FORMATO_POSICIONES_INTERMEDIO);
                cadena += ",";
                cadena += datos.proteinas[proteina_num].densidad_local[aminoacido].ToString();
                if (datos.proteinas[proteina_num].quienes_son_HI.Contains(aminoacido))
                    cadena += ",HI)";
                if (datos.proteinas[proteina_num].quienes_son_HS.Contains(aminoacido))
                    cadena += ",HS)";
                if (datos.proteinas[proteina_num].quienes_son_PI.Contains(aminoacido))
                    cadena += ",PI)";
                if (datos.proteinas[proteina_num].quienes_son_PS.Contains(aminoacido))
                    cadena += ",PS)";
                cadena_final += cadena;
            }
            if (imprime_chaperona)
            {
                cadena = " Chaperonas: (";
                string subcadena;
                foreach (int c in datos.proteinas[proteina_num].chaperonas)
                {
                    subcadena = datos.proteinas[c].nombre_proteina;
                    subcadena = subcadena.Substring(datos.proteinas[c].nombre_proteina.IndexOf(' ')).Trim();
                    cadena += " " + subcadena + " ";
                }
                cadena += ")";
                cadena_final += cadena;
            }
            if (imprime_cuaternarias)
            {
                cadena = " Cuaternarias: (";
                string subcadena;
                foreach (int c in datos.proteinas[proteina_num].cuaternarias)
                {
                    subcadena = datos.proteinas[c].nombre_proteina;
                    subcadena = subcadena.Substring(datos.proteinas[c].nombre_proteina.IndexOf(' ')).Trim();
                    cadena += " " + subcadena + " ";
                }
                cadena += ")";
                cadena_final += cadena;
            }
            if (imprime_funcion_objetivo)
            {
                cadena = " FO=" + datos.proteinas[proteina_num].valor_funcion_actual.ToString(FORMATO_VALORES_FUNCION);
                cadena_final += cadena;
            }
            return cadena_final;
        }

        public void escribe_en_archivo_de_salida(ref StreamWriter archivo_de_salida, string mensaje)
        {
            archivo_de_salida.Write(mensaje);
            archivo_de_salida.Flush();
        }

        public void imprime_informacion(ref datos_de_corrida datos, string Mensaje)
        {
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, Mensaje);
            for (int i = 1; i <= datos.cantidad_de_proteinas; i++)
            {
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " " + datos.proteinas[i].nombre_proteina + "\n");
#if IMPRIME_CODIGO_INTERMEDIO
                for (int k = 1; k <= datos.proteinas[i].acciones.Count / 3; k++)
                {
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "    Operador de folding: " + datos.proteinas[i].acciones[(k - 1) * 3 + 1] + "\n");
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "        " + datos.proteinas[i].acciones[(k - 1) * 3] + "\n");
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "        " + datos.proteinas[i].acciones[(k - 1) * 3 + 2] + "\n");
                }
                imprime_proteinas(ref datos, i, true, true, false);
#else
                imprime_proteinas(ref datos, i, true, true, false);
#endif
            }
        }

        private void operador_proteostasis(ref datos_de_corrida datos)
        {
            
            int cantidad_pliegues = 1;
            while (cantidad_pliegues * 3 < datos.numero_de_proteinas_para_proteosoma)
                cantidad_pliegues++;
            int cantidad_proteinas_temporal = datos.cantidad_de_proteinas - datos.numero_de_proteinas_para_proteosoma;
            ArrayList[] pliegues = new ArrayList[MAX_PLIEGUES];
            for (int i = 1; i < MAX_PLIEGUES; i++)
                pliegues[i] = new ArrayList();
            int cantidad_de_proteinas_por_pliegue = cantidad_proteinas_temporal / cantidad_pliegues;
            for (int i = 1; i <= cantidad_de_proteinas_por_pliegue; i++)
                pliegues[1].Add(i);
            for (int i = 2; i <= cantidad_pliegues; i++)
                for (int j = (i - 1) * cantidad_de_proteinas_por_pliegue + 1; j <= i * cantidad_de_proteinas_por_pliegue; j++)
                    if (j <= cantidad_proteinas_temporal)
                        pliegues[i].Add(j);
            int proteina_min;
            int proteina_max;
            int proteina_promedio;
            double aminoacido_min;
            double aminoacido_promedio;
            double aminoacido_max;
            int suma_de_H;
            double promedio_de_H;
            for (int i = 1; i <= cantidad_pliegues; i++)
            {
                proteina_min = cantidad_proteinas_temporal + (i - 1) * 3 + 1;
                proteina_promedio = cantidad_proteinas_temporal + (i - 1) * 3 + 2;
                proteina_max = cantidad_proteinas_temporal + (i - 1) * 3 + 3;
                for (int k = 1; k <= datos.dimensiones; k++)
                {
                    aminoacido_min = double.MaxValue;
                    aminoacido_promedio = 0.0;
                    aminoacido_max = double.MinValue;
                    suma_de_H = 0;
                    foreach (int proteina_analizada in pliegues[i])
                    {
                        if (aminoacido_min > datos.proteinas[proteina_analizada].aminoacidos_actual[k])
                            aminoacido_min = datos.proteinas[proteina_analizada].aminoacidos_actual[k];
                        if (aminoacido_max < datos.proteinas[proteina_analizada].aminoacidos_actual[k])
                            aminoacido_max = datos.proteinas[proteina_analizada].aminoacidos_actual[k];
                        aminoacido_promedio += datos.proteinas[proteina_analizada].aminoacidos_actual[k];
                        suma_de_H += (int)datos.proteinas[proteina_analizada].PH[k];
                    }
                    promedio_de_H = (float)suma_de_H / pliegues[i].Count;
                    if (promedio_de_H >= 0.5)
                    {
                        datos.proteinas[proteina_min].PH[k] = Tipos_aminoacidos.HIDROFOBICO;
                        datos.proteinas[proteina_max].PH[k] = Tipos_aminoacidos.HIDROFOBICO;
                        datos.proteinas[proteina_promedio].PH[k] = Tipos_aminoacidos.HIDROFOBICO;
                    }
                    else
                    {
                        datos.proteinas[proteina_min].PH[k] = Tipos_aminoacidos.POLAR;
                        datos.proteinas[proteina_max].PH[k] = Tipos_aminoacidos.POLAR;
                        datos.proteinas[proteina_promedio].PH[k] = Tipos_aminoacidos.POLAR;
                    }
                    datos.proteinas[proteina_min].aminoacidos_actual[k] = aminoacido_min;
                    datos.proteinas[proteina_max].aminoacidos_actual[k] = aminoacido_max;
                    datos.proteinas[proteina_promedio].aminoacidos_actual[k] = aminoacido_promedio / pliegues[i].Count;
                }
            }
            evalua_proteinas_ordena_aplica_o_no_Boltzman(ref datos, false);
#if IMPRIME_CODIGO_INTERMEDIO
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\n\nOperador de Proteostasis " + "\n");
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " Cantidad de pliegues: " + cantidad_pliegues.ToString() + "\n");
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " Cantidad de datos.proteinas eliminadas: " + datos.numero_de_proteinas_para_proteosoma.ToString() + "\n");
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " Cantidad datos.proteinas por pliegues: " + cantidad_de_proteinas_por_pliegue.ToString() + "\n");
            for (int i = 1; i <= cantidad_pliegues; i++)
            {
                proteina_min = cantidad_proteinas_temporal + (i - 1) * 3 + 1;
                proteina_promedio = cantidad_proteinas_temporal + (i - 1) * 3 + 2;
                proteina_max = cantidad_proteinas_temporal + (i - 1) * 3 + 3;
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "  Pliegue: " + i.ToString() + "\n");
                foreach (int proteina_analizada in pliegues[i])
                {
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "   " + datos.proteinas[proteina_analizada].nombre_proteina + " ");
                    imprime_proteinas(ref datos, proteina_analizada, false, true, false);
                }
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Operador Minimo ");
                //datos.proteinas[proteina_min].valor_funcion_actual = datos.f.Function(datos.proteinas[proteina_min].aminoacidos_actual, datos.dimensiones, datos.funcion);
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, datos.proteinas[proteina_min].nombre_proteina);
                imprime_proteinas(ref datos, proteina_min, false, true, false);
                if (proteina_promedio <= datos.cantidad_de_proteinas)
                {
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Operador Promedio ");
                    //datos.proteinas[proteina_promedio].valor_funcion_actual = datos.f.Function(datos.proteinas[proteina_promedio].aminoacidos_actual, datos.dimensiones, datos.funcion);
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, datos.proteinas[proteina_promedio].nombre_proteina);
                    imprime_proteinas(ref datos, proteina_promedio, false, true, false);
                }
                if (proteina_max <= datos.cantidad_de_proteinas)
                {
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Operador Maximo ");
                    //datos.proteinas[proteina_max].valor_funcion_actual = datos.f.Function(datos.proteinas[proteina_max].aminoacidos_actual, datos.dimensiones, datos.funcion);
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, datos.proteinas[proteina_max].nombre_proteina);
                    imprime_proteinas(ref datos, proteina_max, false, true, false);
                }
            }
#endif
            
#if IMPRIME_CODIGO_INTERMEDIO
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\nDespues del operador de Proteotasis:" + "\n");
            for (int i = 1; i <= datos.cantidad_de_proteinas; i++)
            {
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " " + datos.proteinas[i].nombre_proteina + "\n");
                imprime_proteinas(ref datos, i, false, true, false);
            }
#endif
        }

        public void operador_desnaturalizacion_PH(ref datos_de_corrida datos)
        {
            int cantidad_de_aminoacidos_a_cambiar;
            int aminoacido_actual;
            ArrayList lista_aminoacidos_cambiados = new ArrayList();
#if IMPRIME_CODIGO_INTERMEDIO
            int cantidad_proteinas_desnaturalizadas = 0;
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\nOperador de desnaturalizacion en la iteracion: " + datos.generacion_actual.ToString() +"\n");
#endif
            for (int proteina_actual = 1; proteina_actual <= datos.cantidad_de_proteinas; proteina_actual++)
            {
                if (datos.aleatorio.NextDouble() < datos.Probabilidad_de_desnaturalizacion_PH)
                {
#if IMPRIME_CODIGO_INTERMEDIO
                    datos.proteinas[proteina_actual].quienes_son_HI.Clear();
                    datos.proteinas[proteina_actual].quienes_son_HS.Clear();
                    datos.proteinas[proteina_actual].quienes_son_PI.Clear();
                    datos.proteinas[proteina_actual].quienes_son_PS.Clear();
                    for (int am = 1; am <= datos.dimensiones; am++)
                    {
                        datos.proteinas[proteina_actual].densidad_local[am] = calcula_Ro(ref datos, proteina_actual, am);
                        clasifica_aminoacidos(ref datos, proteina_actual, am);
                    }
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, datos.proteinas[proteina_actual].nombre_proteina + "\n");
                    imprime_proteinas(ref datos, proteina_actual, true, false, false);
                    cantidad_proteinas_desnaturalizadas++;
#endif
                    lista_aminoacidos_cambiados.Clear();
                    cantidad_de_aminoacidos_a_cambiar = (int)(datos.Porciento_maximo_de_aminoacidos_a_cambiar_por_proteina_en_desnaturalizacion / 100.0 * datos.dimensiones);
                    cantidad_de_aminoacidos_a_cambiar = datos.aleatorio.Next(1, cantidad_de_aminoacidos_a_cambiar + 1);
                    while (lista_aminoacidos_cambiados.Count < cantidad_de_aminoacidos_a_cambiar)
                    {
                        if (datos.aleatorio.NextDouble() < datos.Balance_en_desnaturalizacion_de_H_a_P)
                        {
                            if ((datos.aleatorio.NextDouble()<0.5) && (datos.proteinas[proteina_actual].quienes_son_HI.Count !=0))
                            {
                                aminoacido_actual = datos.aleatorio.Next(datos.proteinas[proteina_actual].quienes_son_HI.Count);
                                aminoacido_actual = (int)datos.proteinas[proteina_actual].quienes_son_HI[aminoacido_actual];
                                if (lista_aminoacidos_cambiados.Contains(aminoacido_actual))
                                    continue;
                                else
                                {
                                    if (datos.aleatorio.NextDouble() < 0.5)
                                        datos.proteinas[proteina_actual].aminoacidos_actual[aminoacido_actual] = datos.mejor_posicion_global[aminoacido_actual] + datos.aleatorio.NextDouble() * datos.omega_actual;
                                    else
                                        datos.proteinas[proteina_actual].aminoacidos_actual[aminoacido_actual] = datos.mejor_posicion_global[aminoacido_actual] - datos.aleatorio.NextDouble() * datos.omega_actual;
                                    lista_aminoacidos_cambiados.Add(aminoacido_actual);
                                }
                            }
                            else if ((datos.aleatorio.NextDouble() < 0.5) && (datos.proteinas[proteina_actual].quienes_son_HS.Count != 0))
                            {
                                aminoacido_actual = datos.aleatorio.Next(datos.proteinas[proteina_actual].quienes_son_HS.Count);
                                aminoacido_actual = (int)datos.proteinas[proteina_actual].quienes_son_HS[aminoacido_actual];
                                if (lista_aminoacidos_cambiados.Contains(aminoacido_actual))
                                    continue;
                                else
                                {
                                    if (datos.aleatorio.NextDouble() < 0.5)
                                        datos.proteinas[proteina_actual].aminoacidos_actual[aminoacido_actual] = datos.mejor_posicion_global[aminoacido_actual] + datos.aleatorio.NextDouble() * datos.omega_actual;
                                    else
                                        datos.proteinas[proteina_actual].aminoacidos_actual[aminoacido_actual] = datos.mejor_posicion_global[aminoacido_actual] - datos.aleatorio.NextDouble() * datos.omega_actual;
                                    lista_aminoacidos_cambiados.Add(aminoacido_actual);
                                }
                            }
                        }
                        else
                        {
                            if ((datos.aleatorio.NextDouble() < 0.5) && (datos.proteinas[proteina_actual].quienes_son_PI.Count != 0))
                            {
                                aminoacido_actual = datos.aleatorio.Next(datos.proteinas[proteina_actual].quienes_son_PI.Count);
                                aminoacido_actual = (int)datos.proteinas[proteina_actual].quienes_son_PI[aminoacido_actual];
                                if (lista_aminoacidos_cambiados.Contains(aminoacido_actual))
                                    continue;
                                else
                                {
                                    if (datos.aleatorio.NextDouble() < 0.5)
                                        datos.proteinas[proteina_actual].aminoacidos_actual[aminoacido_actual] = datos.mejor_posicion_global[aminoacido_actual] + datos.aleatorio.NextDouble() * datos.omega_actual;
                                    else
                                        datos.proteinas[proteina_actual].aminoacidos_actual[aminoacido_actual] = datos.mejor_posicion_global[aminoacido_actual] - datos.aleatorio.NextDouble() * datos.omega_actual;
                                    lista_aminoacidos_cambiados.Add(aminoacido_actual);
                                }
                            }
                            else if ((datos.aleatorio.NextDouble() < 0.5) && (datos.proteinas[proteina_actual].quienes_son_PS.Count != 0))
                            {
                                aminoacido_actual = datos.aleatorio.Next(datos.proteinas[proteina_actual].quienes_son_PS.Count);
                                aminoacido_actual = (int)datos.proteinas[proteina_actual].quienes_son_PS[aminoacido_actual];
                                if (lista_aminoacidos_cambiados.Contains(aminoacido_actual))
                                    continue;
                                else
                                {
                                    if (datos.aleatorio.NextDouble() < 0.5)
                                        datos.proteinas[proteina_actual].aminoacidos_actual[aminoacido_actual] = datos.mejor_posicion_global[aminoacido_actual] + datos.aleatorio.NextDouble() * datos.omega_actual;
                                    else
                                        datos.proteinas[proteina_actual].aminoacidos_actual[aminoacido_actual] = datos.mejor_posicion_global[aminoacido_actual] - datos.aleatorio.NextDouble() * datos.omega_actual;
                                    lista_aminoacidos_cambiados.Add(aminoacido_actual);
                                }
                            }
                        }
                    }
#if IMPRIME_CODIGO_INTERMEDIO
                    datos.proteinas[proteina_actual].quienes_son_HI.Clear();
                    datos.proteinas[proteina_actual].quienes_son_HS.Clear();
                    datos.proteinas[proteina_actual].quienes_son_PI.Clear();
                    datos.proteinas[proteina_actual].quienes_son_PS.Clear();
                    for (int am = 1; am <= datos.dimensiones; am++)
                    {
                        datos.proteinas[proteina_actual].densidad_local[am] = calcula_Ro(ref datos, proteina_actual, am);
                        clasifica_aminoacidos(ref datos, proteina_actual, am);
                    }
                    imprime_proteinas(ref datos, proteina_actual, true, false, false);
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " Aminoacidos cambiados: " + lista_aminoacidos_cambiados.Count.ToString() + "\n");
#endif
                }
            }
#if IMPRIME_CODIGO_INTERMEDIO
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Cantidad de datos.proteinas desnaturalizadas: "+ cantidad_proteinas_desnaturalizadas.ToString() + "\n");
#endif
        }


        public void operador_cuaternario(ref datos_de_corrida datos)
        {
            
            int cantidad_de_proteinas_cuaternarias;
            ArrayList proteinas_que_ya_son_cuaternarias = new ArrayList();
            int proteina_aleatorio;
            double menor_valor_P;
            double menor_valor_H;
            int mejor_proteina_P;
            int mejor_proteina_H;
            for (int proteina_actual = 1; proteina_actual <= datos.cantidad_de_proteinas; proteina_actual++)
                datos.proteinas[proteina_actual].cuaternarias.Clear();
            for (int proteina_actual = 1; proteina_actual <= datos.cantidad_de_proteinas; proteina_actual++)
            {
                if ((datos.aleatorio.NextDouble() < datos.Probabilidad_actual_para_aplicar_el_operador_cuaternario) && (proteinas_que_ya_son_cuaternarias.Count < datos.cantidad_de_proteinas))
                {
                    proteinas_que_ya_son_cuaternarias.Add(proteina_actual);
                    datos.proteinas[proteina_actual].hay_cuaternarias = true;
                    datos.proteinas[proteina_actual].cuaternarias.Clear();
                    cantidad_de_proteinas_cuaternarias = datos.aleatorio.Next(2, Maximo_numero_de_proteinas_para_operador_cuaternario + 1);
                    if ((datos.cantidad_de_proteinas - proteinas_que_ya_son_cuaternarias.Count) < cantidad_de_proteinas_cuaternarias)
                        cantidad_de_proteinas_cuaternarias = datos.cantidad_de_proteinas - proteinas_que_ya_son_cuaternarias.Count;
                    while (datos.proteinas[proteina_actual].cuaternarias.Count < cantidad_de_proteinas_cuaternarias)
                    {
                        proteina_aleatorio = datos.aleatorio.Next(1, datos.cantidad_de_proteinas + 1);
                        if (!proteinas_que_ya_son_cuaternarias.Contains(proteina_aleatorio) && !datos.proteinas[proteina_actual].cuaternarias.Contains(proteina_aleatorio))
                        {
                            datos.proteinas[proteina_actual].cuaternarias.Add(proteina_aleatorio);
                            proteinas_que_ya_son_cuaternarias.Add(proteina_aleatorio);
                        }
                    }
                }
            }
#if IMPRIME_CODIGO_INTERMEDIO
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\nOperador cuaternario en la iteracion: " + datos.generacion_actual.ToString() + "\n");
#endif
            for (int proteina_actual = 1; proteina_actual <= datos.cantidad_de_proteinas; proteina_actual++)
            {
                if (datos.proteinas[proteina_actual].hay_cuaternarias)
                {
#if IMPRIME_CODIGO_INTERMEDIO
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Antes de operador cuaternario \n");
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, datos.proteinas[proteina_actual].nombre_proteina + " ");
                    imprime_proteinas(ref datos, proteina_actual, false, true, true);
                    foreach(int imprime in datos.proteinas[proteina_actual].cuaternarias)
                    {
                        escribe_en_archivo_de_salida(ref datos.archivo_de_salida, datos.proteinas[imprime].nombre_proteina + " ");
                        imprime_proteinas(ref datos, imprime, false, true, true);
                    }
#endif
                    for (int am = 1; am <= datos.dimensiones; am++)
                    {
                        if (datos.proteinas[proteina_actual].PH[am] == Tipos_aminoacidos.HIDROFOBICO)
                        {
                            menor_valor_H = datos.proteinas[proteina_actual].valor_funcion_actual;
                            mejor_proteina_H = proteina_actual;
                            menor_valor_P = double.MaxValue;
                            mejor_proteina_P = -1;
                        }
                        else
                        {
                            menor_valor_P = datos.proteinas[proteina_actual].valor_funcion_actual;
                            mejor_proteina_P = proteina_actual;
                            menor_valor_H = double.MaxValue;
                            mejor_proteina_H = -1;
                        }
                        foreach (int proteina_cuaternaria in datos.proteinas[proteina_actual].cuaternarias)
                        {
                            if (datos.proteinas[proteina_cuaternaria].PH[am] == Tipos_aminoacidos.HIDROFOBICO)
                            {
                                if (menor_valor_H > datos.proteinas[proteina_cuaternaria].valor_funcion_actual)
                                {
                                    menor_valor_H = datos.proteinas[proteina_cuaternaria].valor_funcion_actual;
                                    mejor_proteina_H = proteina_cuaternaria;
                                }
                            }
                            else
                            {
                                if (menor_valor_P > datos.proteinas[proteina_cuaternaria].valor_funcion_actual)
                                {
                                    menor_valor_P = datos.proteinas[proteina_cuaternaria].valor_funcion_actual;
                                    mejor_proteina_P = proteina_cuaternaria;
                                }
                            }
                        }
                        if (datos.proteinas[proteina_actual].PH[am] == Tipos_aminoacidos.HIDROFOBICO)
                        {
                            if (mejor_proteina_H != -1)
                                datos.proteinas[proteina_actual].aminoacidos_actual[am] = datos.proteinas[mejor_proteina_H].aminoacidos_actual[am];
                        }
                        else
                        {
                            if (mejor_proteina_P != -1)
                                datos.proteinas[proteina_actual].aminoacidos_actual[am] = datos.proteinas[mejor_proteina_P].aminoacidos_actual[am];
                        }
                        foreach (int proteina_cuaternaria in datos.proteinas[proteina_actual].cuaternarias)
                            if (datos.proteinas[proteina_cuaternaria].PH[am] == Tipos_aminoacidos.HIDROFOBICO)
                            {
                                if (mejor_proteina_H != -1)
                                    datos.proteinas[proteina_cuaternaria].aminoacidos_actual[am] = datos.proteinas[mejor_proteina_H].aminoacidos_actual[am];
                            }
                            else
                            {
                                if (mejor_proteina_P != -1)
                                    datos.proteinas[proteina_cuaternaria].aminoacidos_actual[am] = datos.proteinas[mejor_proteina_P].aminoacidos_actual[am];
                            }
                    }
#if IMPRIME_CODIGO_INTERMEDIO
                    datos.proteinas[proteina_actual].quienes_son_HI.Clear();
                    datos.proteinas[proteina_actual].quienes_son_HS.Clear();
                    datos.proteinas[proteina_actual].quienes_son_PI.Clear();
                    datos.proteinas[proteina_actual].quienes_son_PS.Clear();
                    for (int am = 1; am <= datos.dimensiones; am++)
                    {
                        datos.proteinas[proteina_actual].densidad_local[am] = calcula_Ro(ref datos, proteina_actual, am);
                        clasifica_aminoacidos(ref datos, proteina_actual, am);
                    }
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Despues de operador cuaternario \n");
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, datos.proteinas[proteina_actual].nombre_proteina + " ");
                    imprime_proteinas(ref datos, proteina_actual, false, true, true);
                    foreach (int imprime in datos.proteinas[proteina_actual].cuaternarias)
                    {
                        escribe_en_archivo_de_salida(ref datos.archivo_de_salida, datos.proteinas[imprime].nombre_proteina + " ");
                        imprime_proteinas(ref datos, imprime, false, true, true);
                    }
#endif
                }
            }
        }
    

        public void operador_cambios_de_H_y_P(ref datos_de_corrida datos)
        {
            int cantidad_proteinas_cambiadas = 0;
            int cantidad_de_aminoacidos_a_cambiar;
            int aminoacido_actual;
            int numero;
            ArrayList cambiados = new ArrayList();

#if IMPRIME_CODIGO_INTERMEDIO
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\nOperador de cambios de H y de P en la iteracion: " + datos.generacion_actual.ToString() + "\n");
#endif
            if (datos.terminar_por_iteraciones_maximas)
                datos.probabilidad_actual_para_cambiar_las_H_y_las_P = probabilidad_de_cambio_de_H_y_P_por_iteraciones(datos);
            else
                datos.probabilidad_actual_para_cambiar_las_H_y_las_P = probabilidad_de_cambio_de_H_y_P_por_evaluaciones(datos);
            for (int proteina_actual = 1; proteina_actual <= datos.cantidad_de_proteinas; proteina_actual++)
            {
                if (datos.aleatorio.NextDouble() < datos.probabilidad_actual_para_cambiar_las_H_y_las_P)
                {
#if IMPRIME_CODIGO_INTERMEDIO
                    for (int aminoacido = 1; aminoacido <= datos.dimensiones; aminoacido++)
                        datos.proteinas[proteina_actual].densidad_local[aminoacido] = calcula_Ro(ref datos, proteina_actual , aminoacido);
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, datos.proteinas[proteina_actual].nombre_proteina + "\n");
                    imprime_proteinas(ref datos, proteina_actual, false, false, false);
#endif
                    cantidad_de_aminoacidos_a_cambiar = datos.aleatorio.Next(1, datos.dimensiones + 1);
                    cantidad_proteinas_cambiadas++;
                    cambiados.Clear();
                    while (cambiados.Count < cantidad_de_aminoacidos_a_cambiar)
                    {
                        numero = datos.aleatorio.Next(1, 5);
                        switch (numero)
                        {
                            case 1:
                                if (datos.proteinas[proteina_actual].quienes_son_PS.Count == 0)
                                    break;
                                numero = datos.aleatorio.Next(datos.proteinas[proteina_actual].quienes_son_PS.Count);
                                aminoacido_actual = (int)datos.proteinas[proteina_actual].quienes_son_PS[numero];
                                if (cambiados.Contains(aminoacido_actual))
                                    break;
                                datos.proteinas[proteina_actual].quienes_son_PS.Remove(aminoacido_actual);
                                cambiados.Add(aminoacido_actual);
                                datos.proteinas[proteina_actual].PH[aminoacido_actual] = Tipos_aminoacidos.HIDROFOBICO;
                                break;
                            case 2:
                                if (datos.proteinas[proteina_actual].quienes_son_PI.Count == 0)
                                    break;
                                numero = datos.aleatorio.Next(datos.proteinas[proteina_actual].quienes_son_PI.Count);
                                aminoacido_actual = (int)datos.proteinas[proteina_actual].quienes_son_PI[numero];
                                if (cambiados.Contains(aminoacido_actual))
                                    break;
                                datos.proteinas[proteina_actual].quienes_son_PI.Remove(aminoacido_actual);
                                cambiados.Add(aminoacido_actual);
                                datos.proteinas[proteina_actual].PH[aminoacido_actual] = Tipos_aminoacidos.HIDROFOBICO;
                                 break;
                            case 3:
                                if (datos.proteinas[proteina_actual].quienes_son_HS.Count == 0)
                                    break;
                                numero = datos.aleatorio.Next(datos.proteinas[proteina_actual].quienes_son_HS.Count);
                                aminoacido_actual = (int)datos.proteinas[proteina_actual].quienes_son_HS[numero];
                                if (cambiados.Contains(aminoacido_actual))
                                    break;
                                datos.proteinas[proteina_actual].quienes_son_HS.Remove(aminoacido_actual);
                                cambiados.Add(aminoacido_actual);
                                datos.proteinas[proteina_actual].PH[aminoacido_actual] = Tipos_aminoacidos.POLAR;
                                break;
                            case 4:
                                if (datos.proteinas[proteina_actual].quienes_son_HI.Count == 0)
                                    break;
                                numero = datos.aleatorio.Next(datos.proteinas[proteina_actual].quienes_son_HI.Count);
                                aminoacido_actual = (int)datos.proteinas[proteina_actual].quienes_son_HI[numero];
                                if (cambiados.Contains(aminoacido_actual))
                                    break;
                                datos.proteinas[proteina_actual].quienes_son_HI.Remove(aminoacido_actual);
                                cambiados.Add(aminoacido_actual);
                                datos.proteinas[proteina_actual].PH[aminoacido_actual] = Tipos_aminoacidos.POLAR;
                                break;
                        }
                    }
                    datos.proteinas[proteina_actual].quienes_son_HI.Clear();
                    datos.proteinas[proteina_actual].quienes_son_HS.Clear();
                    datos.proteinas[proteina_actual].quienes_son_PI.Clear();
                    datos.proteinas[proteina_actual].quienes_son_PS.Clear();
                    for (int aminoacido = 1; aminoacido <= datos.dimensiones; aminoacido++)
                        clasifica_aminoacidos(ref datos, proteina_actual, aminoacido);
                   
#if IMPRIME_CODIGO_INTERMEDIO
                    imprime_proteinas(ref datos, proteina_actual, false, false, false);
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " Aminoacidos cambiados: " + cambiados.Count.ToString() + "\n");
#endif
                }
            }
#if IMPRIME_CODIGO_INTERMEDIO
            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Cantidad de datos.proteinas con cambios en H y P: " + cantidad_proteinas_cambiadas.ToString() + "\n");
            
#endif
        }

        private void operador_de_cascada_de_H_con_reinicio(ref datos_de_corrida datos,
                                                           int proteina_que_dispara,
                                                           ArrayList proteinas_ya_en_cascada,
                                                           ArrayList proteinas_a_cambiar,
                                                           ArrayList lista_de_aminoacidos)
        {
            ArrayList nuevos_aminoacidos_para_cambiar = new ArrayList();
            ArrayList nuevas_proteinas_para_cambio_en_la_siguiente = new ArrayList();
            int densidad_mayor;
            int aminoacido_de_densidad_mayor;
            int cantidad_de_proteinas_cambiar;
            int cantidad_de_aminoacidos_cambiar;
            int cantidad_de_proteinas_iniciales_a_cambiar = proteinas_a_cambiar.Count;
            double[] posiciones_temporales = new double[MAX_AMINOACIDOS];
            int proteina_seleccionada;
            int cuantas_quedan_con_H;
#if IMPRIME_CODIGO_INTERMEDIO
            ArrayList listado_de_acciones_temporales = new ArrayList();
            string cadena;
#endif
            foreach (int p in proteinas_a_cambiar)
            {
                if (p == proteina_que_dispara)
                    continue;
#if IMPRIME_CODIGO_INTERMEDIO
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, datos.proteinas[p].nombre_proteina + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, convierte_proteina_a_cadena(datos, p, false, true, false) + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Intentando modificarse por la " + datos.proteinas[proteina_que_dispara].nombre_proteina + " en los siguientes aminoacidos ( ");
                cadena = "";
                foreach (int am in lista_de_aminoacidos)
                    cadena += am.ToString() + " ";
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, cadena + ")\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, convierte_proteina_a_cadena(datos, proteina_que_dispara, false, true, false) + "\n");
#endif
                for (int am = 1; am <= datos.dimensiones; am++)
                    posiciones_temporales[am] = datos.proteinas[p].aminoacidos_actual[am];
#if IMPRIME_CODIGO_INTERMEDIO
                double aleatorio_de_la_operacion;
#endif
                foreach (int am in lista_de_aminoacidos)
                {
#if IMPRIME_CODIGO_INTERMEDIO
                    if (datos.proteinas[p].quienes_son_HI.Contains(am) || datos.proteinas[p].quienes_son_HS.Contains(am))
                    {
                        aleatorio_de_la_operacion = datos.aleatorio.NextDouble();
                        if (datos.aleatorio.NextDouble() < 0.5)
                        {
                            posiciones_temporales[am] = datos.proteinas[proteina_que_dispara].aminoacidos_actual[am] + aleatorio_de_la_operacion * datos.omega_actual;
                            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " Aminoacido " + am.ToString() + " " + posiciones_temporales[am].ToString(FORMATO_POSICIONES_INTERMEDIO) + "= " +
                                                                datos.proteinas[proteina_que_dispara].aminoacidos_actual[am].ToString(FORMATO_POSICIONES_INTERMEDIO) + " + " +
                                                                aleatorio_de_la_operacion.ToString(FORMATO_PROBABILIDAD) + " * " + datos.omega_actual.ToString(FORMATO_PROBABILIDAD) + "\n");
                        }
                        else
                        {
                            posiciones_temporales[am] = datos.proteinas[proteina_que_dispara].aminoacidos_actual[am] - aleatorio_de_la_operacion * datos.omega_actual;
                            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " Aminoacido " + am.ToString() + " " + posiciones_temporales[am].ToString(FORMATO_POSICIONES_INTERMEDIO) + "= " +
                                                                datos.proteinas[proteina_que_dispara].aminoacidos_actual[am].ToString(FORMATO_POSICIONES_INTERMEDIO) + " - " +
                                                                aleatorio_de_la_operacion.ToString(FORMATO_PROBABILIDAD) + " * " + datos.omega_actual.ToString(FORMATO_PROBABILIDAD) + "\n");
                        }
                    }
#else
                    if (datos.proteinas[p].quienes_son_HI.Contains(am) || datos.proteinas[p].quienes_son_HS.Contains(am))
                    {
                        if (datos.aleatorio.NextDouble() < 0.5)
                            posiciones_temporales[am] = datos.proteinas[proteina_que_dispara].aminoacidos_actual[am] + datos.aleatorio.NextDouble() * datos.omega_actual;
                        else
                            posiciones_temporales[am] = datos.proteinas[proteina_que_dispara].aminoacidos_actual[am] - datos.aleatorio.NextDouble() * datos.omega_actual;
                    }
#endif
                }
                for (int am = 1; am <= datos.dimensiones; am++)
                    datos.proteinas[p].aminoacidos_actual[am] = posiciones_temporales[am];
                datos.proteinas[p].quienes_son_HI.Clear();
                datos.proteinas[p].quienes_son_HS.Clear();
                datos.proteinas[p].quienes_son_PI.Clear();
                datos.proteinas[p].quienes_son_PS.Clear();
                for (int aminoacido = 1; aminoacido <= datos.dimensiones; aminoacido++)
                {
                     datos.proteinas[p].densidad_local[aminoacido] = calcula_Ro(ref datos, p, aminoacido);
                     clasifica_aminoacidos(ref datos, p, aminoacido);
                }
                //QuickSort_vector_aminoacidos(ref datos.proteinas[p].aminoacidos_actual, 1, datos.dimensiones);
                    
#if IMPRIME_CODIGO_INTERMEDIO
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Cambio aceptado " + convierte_proteina_a_cadena(datos, p, false, true, false) + "\n");
#endif
            }
            if (proteinas_ya_en_cascada.Count == datos.cantidad_de_proteinas)
                return;
            foreach (int p in proteinas_a_cambiar)
            {
                nuevas_proteinas_para_cambio_en_la_siguiente.Clear();
                cantidad_de_proteinas_cambiar = proteinas_a_cambiar.Count * 2;
                cuantas_quedan_con_H = 0;
                for (int p_revisa = 1; p_revisa <= datos.cantidad_de_proteinas; p_revisa++)
                    if (!proteinas_ya_en_cascada.Contains(p_revisa))
                        if (p != p_revisa)
                            if ((datos.proteinas[p_revisa].quienes_son_HI.Count + datos.proteinas[p_revisa].quienes_son_HS.Count) != 0)
                                cuantas_quedan_con_H++;
                if (cuantas_quedan_con_H == 0)
                    return;
                if (cantidad_de_proteinas_cambiar > cuantas_quedan_con_H)
                    cantidad_de_proteinas_cambiar = cuantas_quedan_con_H;
                nuevas_proteinas_para_cambio_en_la_siguiente.Clear();
                while (nuevas_proteinas_para_cambio_en_la_siguiente.Count < cantidad_de_proteinas_cambiar)
                {
                    proteina_seleccionada = datos.aleatorio.Next(2, datos.cantidad_de_proteinas + 1);
                    if (proteina_seleccionada == p)
                        continue;
                    if (proteinas_ya_en_cascada.Contains(proteina_seleccionada))
                        continue;
                    if ((datos.proteinas[proteina_seleccionada].quienes_son_HI.Count + datos.proteinas[proteina_seleccionada].quienes_son_HS.Count) == 0)
                        continue;
                    if (nuevas_proteinas_para_cambio_en_la_siguiente.Contains(proteina_seleccionada))
                        continue;
                    nuevas_proteinas_para_cambio_en_la_siguiente.Add(proteina_seleccionada);
                }
                foreach (int pro in nuevas_proteinas_para_cambio_en_la_siguiente)
                {
                    if ((datos.proteinas[pro].quienes_son_HI.Count + datos.proteinas[pro].quienes_son_HS.Count) == 0)
                        continue;
                    nuevos_aminoacidos_para_cambiar.Clear();
                    proteinas_ya_en_cascada.Add(pro);
                    cantidad_de_aminoacidos_cambiar = lista_de_aminoacidos.Count * 2;
                    if ((datos.proteinas[pro].quienes_son_HS.Count + datos.proteinas[pro].quienes_son_HI.Count) < cantidad_de_aminoacidos_cambiar)
                        cantidad_de_aminoacidos_cambiar = datos.proteinas[pro].quienes_son_HS.Count + datos.proteinas[pro].quienes_son_HI.Count;
                    while (nuevos_aminoacidos_para_cambiar.Count < cantidad_de_aminoacidos_cambiar)
                    {
                        aminoacido_de_densidad_mayor = 0;
                        densidad_mayor = int.MinValue;
                        for (int aminoacido = 1; aminoacido <= datos.dimensiones; aminoacido++)
                        {
                            if ((datos.proteinas[pro].densidad_local[aminoacido] >= densidad_mayor)
                                && (!nuevos_aminoacidos_para_cambiar.Contains(aminoacido)))
                            {
                                aminoacido_de_densidad_mayor = aminoacido;
                                densidad_mayor = datos.proteinas[pro].densidad_local[aminoacido];
                            }
                        }
                        if (aminoacido_de_densidad_mayor != 0)
                            nuevos_aminoacidos_para_cambiar.Add(aminoacido_de_densidad_mayor);
                    }
                    if (nuevas_proteinas_para_cambio_en_la_siguiente.Count == 1 && (int)nuevas_proteinas_para_cambio_en_la_siguiente[0] == pro)
                        continue;
                    else
                        operador_de_cascada_de_H_con_reinicio(ref datos, 1, proteinas_ya_en_cascada, nuevas_proteinas_para_cambio_en_la_siguiente, nuevos_aminoacidos_para_cambiar);
                    if (proteinas_ya_en_cascada.Count == 10)
                    {
                        proteinas_ya_en_cascada.Remove(pro);
                        foreach (int prot in nuevas_proteinas_para_cambio_en_la_siguiente)
                            proteinas_ya_en_cascada.Remove(prot);
                        return;
                    }
                }
            }
        }


        private void operador_de_cascada_de_P_con_reinicio( ref datos_de_corrida datos, 
                                                            int proteina_que_dispara,
                                                            ArrayList proteinas_ya_en_cascada,
                                                            ArrayList proteinas_a_cambiar,
                                                            ArrayList lista_de_aminoacidos)
        {
            ArrayList nuevos_aminoacidos_para_cambiar = new ArrayList();
            ArrayList nuevas_proteinas_para_cambio_en_la_siguiente = new ArrayList();
            int densidad_menor;
            int aminoacido_de_densidad_menor;
            int cantidad_de_proteinas_cambiar;
            int cantidad_de_aminoacidos_cambiar;
            int cantidad_de_proteinas_iniciales_a_cambiar = proteinas_a_cambiar.Count;
            double[] posiciones_temporales = new double[MAX_AMINOACIDOS];
            int proteina_seleccionada;
            int cuantas_quedan_con_P;
#if IMPRIME_CODIGO_INTERMEDIO
            ArrayList listado_de_acciones_temporales = new ArrayList();
            string cadena;
#endif
            foreach (int p in proteinas_a_cambiar)
            {
                if (p == proteina_que_dispara)
                    continue;
#if IMPRIME_CODIGO_INTERMEDIO
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, datos.proteinas[p].nombre_proteina + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, convierte_proteina_a_cadena(datos, p, false, true, false) + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Intentando modificarse por la " + datos.proteinas[proteina_que_dispara].nombre_proteina + " en los siguientes aminoacidos ( ");
                cadena = "";
                foreach (int am in lista_de_aminoacidos)
                    cadena += am.ToString() + " ";
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, cadena + ")\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, convierte_proteina_a_cadena(datos, proteina_que_dispara, false, true, false) + "\n");
#endif
                for (int am = 1; am <= datos.dimensiones; am++)
                    posiciones_temporales[am] = datos.proteinas[p].aminoacidos_actual[am];
#if IMPRIME_CODIGO_INTERMEDIO
                double aleatorio_de_la_operacion;
#endif
                foreach (int am in lista_de_aminoacidos)
                {
#if IMPRIME_CODIGO_INTERMEDIO
                    if (datos.proteinas[p].quienes_son_PI.Contains(am) || datos.proteinas[p].quienes_son_PS.Contains(am))
                    {
                        aleatorio_de_la_operacion = datos.aleatorio.NextDouble();
                        if (datos.aleatorio.NextDouble() < 0.5)
                        {
                            posiciones_temporales[am] = datos.proteinas[proteina_que_dispara].aminoacidos_actual[am] + aleatorio_de_la_operacion * datos.omega_actual;
                            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " Aminoacido " + am.ToString() + " " + posiciones_temporales[am].ToString(FORMATO_POSICIONES_INTERMEDIO) + "= " +
                                                                datos.proteinas[proteina_que_dispara].aminoacidos_actual[am].ToString(FORMATO_POSICIONES_INTERMEDIO) + " + " +
                                                                aleatorio_de_la_operacion.ToString(FORMATO_PROBABILIDAD) + " * " + datos.omega_actual.ToString(FORMATO_PROBABILIDAD) + "\n");
                        }
                        else
                        {
                            posiciones_temporales[am] = datos.proteinas[proteina_que_dispara].aminoacidos_actual[am] - aleatorio_de_la_operacion * datos.omega_actual;
                            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " Aminoacido " + am.ToString() + " " + posiciones_temporales[am].ToString(FORMATO_POSICIONES_INTERMEDIO) + "= " +
                                                                datos.proteinas[proteina_que_dispara].aminoacidos_actual[am].ToString(FORMATO_POSICIONES_INTERMEDIO) + " - " +
                                                                aleatorio_de_la_operacion.ToString(FORMATO_PROBABILIDAD) + " * " + datos.omega_actual.ToString(FORMATO_PROBABILIDAD) + "\n");
                        }
                    }
#else
                    if (datos.proteinas[p].quienes_son_PI.Contains(am) || datos.proteinas[p].quienes_son_PS.Contains(am))
                    {
                        if (datos.aleatorio.NextDouble() < 0.5)
                            posiciones_temporales[am] = datos.proteinas[proteina_que_dispara].aminoacidos_actual[am] + datos.aleatorio.NextDouble() * datos.omega_actual;
                        else
                            posiciones_temporales[am] = datos.proteinas[proteina_que_dispara].aminoacidos_actual[am] - datos.aleatorio.NextDouble() * datos.omega_actual;
                    }
#endif
                }
                for (int am = 1; am <= datos.dimensiones; am++)
                    datos.proteinas[p].aminoacidos_actual[am] = posiciones_temporales[am];
                datos.proteinas[p].quienes_son_HI.Clear();
                datos.proteinas[p].quienes_son_HS.Clear();
                datos.proteinas[p].quienes_son_PI.Clear();
                datos.proteinas[p].quienes_son_PS.Clear();
                for (int aminoacido = 1; aminoacido <= datos.dimensiones; aminoacido++)
                {
                    datos.proteinas[p].densidad_local[aminoacido] = calcula_Ro(ref datos, p, aminoacido);
                    clasifica_aminoacidos(ref datos, p, aminoacido);
                }
                //QuickSort_vector_aminoacidos(ref datos.proteinas[p].aminoacidos_actual, 1, datos.dimensiones);
                  
#if IMPRIME_CODIGO_INTERMEDIO
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Cambio aceptado " + convierte_proteina_a_cadena(datos, p, false, true, false) + "\n");
#endif
            }
            if (proteinas_ya_en_cascada.Count == datos.cantidad_de_proteinas)
                return;
            foreach (int p in proteinas_a_cambiar)
            {
                nuevas_proteinas_para_cambio_en_la_siguiente.Clear();
                cantidad_de_proteinas_cambiar = proteinas_a_cambiar.Count * 2;
                cuantas_quedan_con_P = 0;
                for (int p_revisa = 1; p_revisa <= datos.cantidad_de_proteinas; p_revisa++)
                    if (!proteinas_ya_en_cascada.Contains(p_revisa))
                        if (p != p_revisa)
                            if ((datos.proteinas[p_revisa].quienes_son_PI.Count + datos.proteinas[p_revisa].quienes_son_PS.Count) != 0)
                                cuantas_quedan_con_P++;
                if (cuantas_quedan_con_P == 0)
                    return;
                if (cantidad_de_proteinas_cambiar > cuantas_quedan_con_P)
                    cantidad_de_proteinas_cambiar = cuantas_quedan_con_P;
                nuevas_proteinas_para_cambio_en_la_siguiente.Clear();
                while (nuevas_proteinas_para_cambio_en_la_siguiente.Count < cantidad_de_proteinas_cambiar)
                {
                    proteina_seleccionada = datos.aleatorio.Next(2, datos.cantidad_de_proteinas + 1);
                    if (proteina_seleccionada == p)
                        continue;
                    if (proteinas_ya_en_cascada.Contains(proteina_seleccionada))
                        continue;
                    if ((datos.proteinas[proteina_seleccionada].quienes_son_PI.Count + datos.proteinas[proteina_seleccionada].quienes_son_PS.Count) == 0)
                        continue;
                    if (nuevas_proteinas_para_cambio_en_la_siguiente.Contains(proteina_seleccionada))
                        continue;
                    nuevas_proteinas_para_cambio_en_la_siguiente.Add(proteina_seleccionada);
                }
                foreach (int pro in nuevas_proteinas_para_cambio_en_la_siguiente)
                {
                    if ((datos.proteinas[pro].quienes_son_PI.Count + datos.proteinas[pro].quienes_son_PS.Count) == 0)
                        continue;
                    nuevos_aminoacidos_para_cambiar.Clear();
                    proteinas_ya_en_cascada.Add(pro);
                    cantidad_de_aminoacidos_cambiar = lista_de_aminoacidos.Count * 2;
                    if ((datos.proteinas[pro].quienes_son_PS.Count + datos.proteinas[pro].quienes_son_PI.Count) < cantidad_de_aminoacidos_cambiar)
                        cantidad_de_aminoacidos_cambiar = datos.proteinas[pro].quienes_son_PS.Count + datos.proteinas[pro].quienes_son_PI.Count;
                    while (nuevos_aminoacidos_para_cambiar.Count < cantidad_de_aminoacidos_cambiar)
                    {
                        aminoacido_de_densidad_menor = 0;
                        densidad_menor = int.MaxValue;
                        for (int aminoacido = 1; aminoacido <= datos.dimensiones; aminoacido++)
                        {
                            if ((datos.proteinas[pro].densidad_local[aminoacido] <= densidad_menor)
                                && (!nuevos_aminoacidos_para_cambiar.Contains(aminoacido)))
                            {
                                aminoacido_de_densidad_menor = aminoacido;
                                densidad_menor = datos.proteinas[pro].densidad_local[aminoacido];
                            }
                        }
                        if (aminoacido_de_densidad_menor != 0)
                            nuevos_aminoacidos_para_cambiar.Add(aminoacido_de_densidad_menor);
                    }
                    if (nuevas_proteinas_para_cambio_en_la_siguiente.Count == 1 && (int)nuevas_proteinas_para_cambio_en_la_siguiente[0] == pro)
                        continue;
                    else
                        operador_de_cascada_de_P_con_reinicio(ref datos, 1, proteinas_ya_en_cascada, nuevas_proteinas_para_cambio_en_la_siguiente, nuevos_aminoacidos_para_cambiar);
                    if (proteinas_ya_en_cascada.Count == 10)
                    {
                        proteinas_ya_en_cascada.Remove(pro);
                        foreach (int prot in nuevas_proteinas_para_cambio_en_la_siguiente)
                            proteinas_ya_en_cascada.Remove(prot);
                        return;
                    }
                }
            }
        }

        void lee_datos_de_la_interfaz_para_ejecutar(ref datos_de_corrida datos)
        {
            datos.semilla_aleatorio = Math.Abs((int)System.DateTime.Now.Ticks);
            //datos.semilla_aleatorio = 1607484024;
            datos.aleatorio = new Random(datos.semilla_aleatorio);
            if (!datos.ejecucion_por_lotes && datos.repeticion_actual == 1)
            {
                DateTime tiempo = System.DateTime.Now;
                datos.nombre_del_archivo_de_salida = "Resultados_" + tiempo.ToString("yyyy_MM_dd_HH_mm_ss") + ".txt";
                //nombre_del_archivo_de_salida = "Resultados.txt";
                datos.archivo_de_salida = new StreamWriter(datos.nombre_del_archivo_de_salida);
                datos.archivo_de_salida.AutoFlush = true;
                datos.continua_archivo = 0;
                textBox30.Text = "";
                textBox30.Refresh();
#if POSIBILIDAD_DE_GRAFICAR
                archivo_de_salida_grafico = new StreamWriter("Grafico_" + datos.nombre_del_archivo_de_salida);
                archivo_de_salida_grafico.WriteLine("Datos para graficos");
                archivo_de_salida_grafico.AutoFlush = true;
#endif
            }
            datos.dimensiones = System.Convert.ToInt32(textBox5.Text);
            datos.nombre_de_la_funcion = comboBox1.SelectedItem.ToString().Trim();
            datos.f = new Funciones();
            if (datos.nombre_de_la_funcion.StartsWith("CEC_"))
                datos.f.inicia_datos_de_corrida_matrices_y_desplazamientos(datos.dimensiones);
            datos.f.convierte_cadena_a_nombre_de_funcion(datos.nombre_de_la_funcion, ref datos.funcion);
            datos.f.detalles_de_funcion(datos.funcion, ref datos.dimensiones, ref datos.lb, ref datos.ub, ref datos.nombre_de_la_funcion_detalles);
            datos.Ro_corte = (int)Math.Ceiling(System.Convert.ToSingle(textBox2.Text) / 100.0 * datos.dimensiones);
            textBox5.Text = datos.dimensiones.ToString();
            textBox5.Refresh();
            datos.cantidad_de_proteinas = System.Convert.ToInt16(numericUpDown4.Value);
            datos.h_inicial = (int)(System.Convert.ToSingle(textBox1.Text) / 100.0 * datos.ub[1]);
            datos.h_final = (int)(System.Convert.ToSingle(textBox25.Text) / 100.0 * datos.ub[1]);
            datos.omega_inicial = System.Convert.ToSingle(textBox15.Text) / 100.0 * datos.ub[1];
            datos.omega_final = System.Convert.ToSingle(textBox17.Text) / 100.0 * datos.ub[1];
            datos.h_final = System.Convert.ToDouble(textBox25.Text);
            datos.Q = System.Convert.ToInt32(textBox7.Text);
            datos.PfH = System.Convert.ToDouble(textBox8.Text);
            datos.PfP = System.Convert.ToDouble(textBox9.Text);
            datos.Pnucleacion_H = System.Convert.ToDouble(textBox12.Text);
            datos.Pnucleacion_P = System.Convert.ToDouble(textBox19.Text);
            datos.Probabilidad_Hidrofobico_Polar_minima = System.Convert.ToDouble(textBox13.Text);
            datos.Probabilidad_Hidrofobico_Polar_maxima = System.Convert.ToDouble(textBox36.Text);
            datos.maximas_generaciones = System.Convert.ToInt32(textBox18.Text);
            datos.probabilidad_de_chaperonas = System.Convert.ToDouble(textBox20.Text);
            if (radioButton3.Checked)
                datos.chaperonas_entre_mejores = true;
            else
                datos.chaperonas_entre_mejores = false;
            datos.probabilidad_de_aceptar_estados_iniciales = System.Convert.ToDouble(textBox27.Text);
            datos.probabilidad_de_aceptar_estados_finales = System.Convert.ToDouble(textBox26.Text);
            datos.cantidad_de_repeticiones = System.Convert.ToInt16(textBox28.Text);
            datos.probabilidad_de_colapso_H = System.Convert.ToDouble(textBox31.Text);
            datos.porciento_de_aminoacidos_de_colapso_H = System.Convert.ToDouble(textBox29.Text);
            datos.probabilidad_de_interaccion_electrostatica_P = System.Convert.ToDouble(textBox33.Text);
            datos.porciento_de_aminoacidos_de_interaccion_electrostatica_P = System.Convert.ToDouble(textBox32.Text);
            datos.porciento_de_proteinas_para_proteosoma = System.Convert.ToSingle(textBox34.Text);
            datos.numero_de_proteinas_para_proteosoma = ((int)(datos.porciento_de_proteinas_para_proteosoma / 100.0 * datos.cantidad_de_proteinas + 0.5));
            datos.Probabilidad_de_desnaturalizacion_PH = System.Convert.ToDouble(textBox39.Text);
            datos.Balance_en_desnaturalizacion_de_H_a_P = System.Convert.ToDouble(textBox40.Text);
            datos.Porciento_de_las_iteraciones_o_evaluaciones_para_aplicar_desnaturalizacion_de_H_a_P = System.Convert.ToDouble(textBox41.Text);
            datos.Porciento_maximo_de_aminoacidos_a_cambiar_por_proteina_en_desnaturalizacion = System.Convert.ToDouble(textBox42.Text);
            datos.maximo_numero_de_evaluaciones_de_la_funcion = System.Convert.ToInt64(textBox38.Text);
            if (checkBox1.Checked)
                datos.terminar_por_iteraciones_maximas = false;
            else
                datos.terminar_por_iteraciones_maximas = true;

            datos.Porciento_de_iteraciones_o_evaluaciones_para_cambiar_las_H_y_las_P = System.Convert.ToDouble(textBox45.Text);
            datos.probabilidad_inicial_para_cambiar_las_H_y_las_P = System.Convert.ToDouble(textBox46.Text);
            datos.probabilidad_final_para_cambiar_las_H_y_las_P = System.Convert.ToDouble(textBox43.Text);
            datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_cuaternario = System.Convert.ToDouble(textBox48.Text);
            datos.Probabilidad_inicial_para_aplicar_el_operador_cuaternario = System.Convert.ToDouble(textBox50.Text);
            datos.Probabilidad_de_movimiento_aleatorio = System.Convert.ToDouble(textBox53.Text);
            datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_H = System.Convert.ToDouble(textBox52.Text);
            datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_P = System.Convert.ToDouble(textBox56.Text);
            if (datos.repeticion_actual == 1)
            {
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Linea de comandos: " + datos.linea_de_comandos_para_escribir + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Semilla de la corrida: " + datos.semilla_aleatorio.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Nombre de la Funcion: " + datos.nombre_de_la_funcion_detalles + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Maximas iteraciones: " + datos.maximas_generaciones.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Cantidad de repeticiones: " + datos.cantidad_de_repeticiones.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Proteoma (minimo 5 proteinas): " + datos.cantidad_de_proteinas.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Dimension de la Funcion Objetivo (FO): " + datos.dimensiones.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Maximo numero de evaluaciones de la FO: " + datos.maximo_numero_de_evaluaciones_de_la_funcion.ToString() + "\n");
                if (checkBox1.Checked)
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Control de la finalizacion: Finalizacion por evaluaciones de la FO \n");
                else
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Control de la finalizacion: Finalizacion por iteraciones maximas \n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Ro de corte: " + datos.Ro_corte.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Valor de h inicial: " + datos.h_inicial.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Valor de h final: " + datos.h_final.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Omega inicial: " + datos.omega_inicial.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Omega final: " + datos.omega_final.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad de Boltzman inicial: " + datos.probabilidad_de_aceptar_estados_iniciales.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad de Boltzman final: " + datos.probabilidad_de_aceptar_estados_finales.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Cantidad de transiciones: " + datos.Q.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad de Hidrofobico Polar minima: " + datos.Probabilidad_Hidrofobico_Polar_minima.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad de Hidrofobico Polar maxima: " + datos.Probabilidad_Hidrofobico_Polar_maxima.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Porciento de iteraciones o evaluaciones para cambiar H y P: " + datos.Porciento_de_iteraciones_o_evaluaciones_para_cambiar_las_H_y_las_P.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad inicial de cambio de H y P: " + datos.probabilidad_inicial_para_cambiar_las_H_y_las_P.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad final de cambio de H y P: " + datos.probabilidad_final_para_cambiar_las_H_y_las_P.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad de plegado del aminoacido H: " + datos.PfH.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad de plegado del aminoacido P: " + datos.PfP.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad de nucleacion H: " + datos.Pnucleacion_H.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad de nucleacion P: " + datos.Pnucleacion_P.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad de colapso H: " + datos.probabilidad_de_colapso_H.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Porciento de aminoacidos para colapso H: " + datos.porciento_de_aminoacidos_de_colapso_H.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad de interaccion electrostatica P: " + datos.probabilidad_de_interaccion_electrostatica_P.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Porciento de aminoacidos para interaccion electrostatica P: " + datos.porciento_de_aminoacidos_de_interaccion_electrostatica_P.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad de que haya chaperonas: " + datos.probabilidad_de_chaperonas.ToString() + "\n");
                if (radioButton3.Checked)
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Seleccion de chaperones: Entre las de mejor FO \n");
                else
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Seleccion de chaperones: Con FO mejor que la actual \n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Porciento de iteraciones o evaluaciones para aplicar el operador de la estructura cuaternaria: " + datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_cuaternario.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad de cambio inicial en el operador cuaternarios:  " + datos.Probabilidad_inicial_para_aplicar_el_operador_cuaternario.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Porciento de iteraciones o evaluaciones para aplicar desnaturalizacion: " + datos.Porciento_de_las_iteraciones_o_evaluaciones_para_aplicar_desnaturalizacion_de_H_a_P.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Porciento maximo de aminoacidos a cambiar por proteina en desnaturalizacion: " + datos.Porciento_maximo_de_aminoacidos_a_cambiar_por_proteina_en_desnaturalizacion.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad de cambio maxima para H para H en desnaturalizacion: " + datos.Probabilidad_de_desnaturalizacion_PH.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Proteosoma porciento de proteinas a eliminar: " + datos.porciento_de_proteinas_para_proteosoma.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Porciento de iteraciones o evaluaciones para aplicar operador de cascada H:  " + datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_H.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Porciento de iteraciones o evaluaciones para aplicar operador de cascada P:  " + datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_P.ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Probabilidad para aplicar el operador de movimiento aleatorio:  " + datos.Probabilidad_de_movimiento_aleatorio.ToString() + "\n");
            }
            textBox23.Text = "1e308";
            textBox23.Refresh();
        }

        void evalua_proteinas_ordena_aplica_o_no_Boltzman(ref datos_de_corrida datos, bool aplica_Boltzman)
        {
            double valor_de_la_funcion;
            for (int proteina_actual = 1; proteina_actual <= datos.cantidad_de_proteinas; proteina_actual++)
            {
                    valor_de_la_funcion = datos.f.Function(datos.proteinas[proteina_actual].aminoacidos_actual, datos.dimensiones, datos.funcion);
                    if (valor_de_la_funcion <= datos.proteinas[proteina_actual].mejor_valor_de_la_funcion)
                    {
                        datos.proteinas[proteina_actual].valor_funcion_actual = valor_de_la_funcion;
                        datos.proteinas[proteina_actual].valor_funcion_reserva = valor_de_la_funcion;
                        datos.proteinas[proteina_actual].mejor_valor_de_la_funcion = valor_de_la_funcion;
                        for (int k = 1; k <= datos.dimensiones; k++)
                        {
                            datos.proteinas[proteina_actual].aminoacidos_mejor[k] = datos.proteinas[proteina_actual].aminoacidos_actual[k];
                            datos.proteinas[proteina_actual].aminoacidos_reserva[k] = datos.proteinas[proteina_actual].aminoacidos_actual[k];
                        }
                        if (datos.proteinas[proteina_actual].mejor_valor_de_la_funcion < datos.mejor_valor_global)
                        {
                            datos.mejor_valor_global = datos.proteinas[proteina_actual].valor_funcion_actual;
                            if (datos.mejor_valor_global < System.Convert.ToDouble(textBox23.Text))
                            {
                                textBox23.Text = datos.mejor_valor_global.ToString();
                                textBox23.Refresh();
                            }
                            for (int k = 1; k <= datos.dimensiones; k++)
                                datos.mejor_posicion_global[k] = datos.proteinas[proteina_actual].aminoacidos_actual[k];
                        }
                    }
                    else if (aplica_Boltzman)
                         { 
                            double probabilidad_Boltzman;
                            probabilidad_Boltzman = Math.Exp(-valor_de_la_funcion / datos.temperatura_actual);
                            if (datos.aleatorio.NextDouble() > probabilidad_Boltzman)
                            {
                                datos.proteinas[proteina_actual].valor_funcion_actual = datos.proteinas[proteina_actual].valor_funcion_reserva;
                                for (int k = 1; k <= datos.dimensiones; k++)
                                    datos.proteinas[proteina_actual].aminoacidos_actual[k] = datos.proteinas[proteina_actual].aminoacidos_reserva[k];
                            }
                            else
                            {
                                datos.proteinas[proteina_actual].valor_funcion_reserva = valor_de_la_funcion;
                                for (int k = 1; k <= datos.dimensiones; k++)
                                    datos.proteinas[proteina_actual].aminoacidos_reserva[k] = datos.proteinas[proteina_actual].aminoacidos_actual[k];
                            }
                        }
            }
            datos.cantidad_de_evaluaciones_de_la_FO_actual = (long)datos.f.devuelve_cantidad_de_evaluaciones();
            for (int proteina_actual = 1; proteina_actual <= datos.cantidad_de_proteinas; proteina_actual++)
            {
                datos.proteinas[proteina_actual].hay_chaperona = false;
                datos.proteinas[proteina_actual].chaperonas.Clear();
                datos.proteinas[proteina_actual].hay_cuaternarias = false;
                datos.proteinas[proteina_actual].cuaternarias.Clear(); datos.proteinas[proteina_actual].quienes_son_HI.Clear();
                datos.proteinas[proteina_actual].quienes_son_HS.Clear();
                datos.proteinas[proteina_actual].quienes_son_PI.Clear();
                datos.proteinas[proteina_actual].quienes_son_PS.Clear();
                for (int aminoacido = 1; aminoacido <= datos.dimensiones; aminoacido++)
                {
                    datos.proteinas[proteina_actual].densidad_local[aminoacido] = calcula_Ro(ref datos, proteina_actual, aminoacido);
                    clasifica_aminoacidos(ref datos, proteina_actual, aminoacido);
                }
            }
            QuickSort_vector_proteinas(ref datos.proteinas, 1, datos.cantidad_de_proteinas);
        }

        void procedimiento_de_ejecucion_individual(ref datos_de_corrida datos)
        {
            int cuenta_para_los_operadores_desnaturalizacion_PH;
            int cuenta_para_los_operadores_cambios_de_H_y_P;
            int cuenta_para_los_operadores_cuaternario;
            int cuenta_para_los_operadores_cascada_P;
            int cuenta_para_los_operadores_cascada_H;
            int cuantas_proteinas_con_H_quedan;
            int cambiada_inicial;
            bool se_ejecuto_alguno;
            int aminoacido_de_densidad_mayor;
            int densidad_mayor;
            int aminoacido_de_densidad_menor;
            int densidad_menor;
            int cantidad;
            int cuantas_proteinas_con_P_quedan;
            double cuenta;
            ArrayList aminoacidos_para_cascada = new ArrayList();
            ArrayList proteinas_para_cascada = new ArrayList();
            ArrayList proteinas_ya_en_cascada = new ArrayList();
            datos.stopWatch_por_corrida_total.Restart();
            datos.stopWatch_por_corrida_total.Start();
            textBox22.Text = "";
            textBox22.Refresh();
            armar_la_linea_de_comandos(ref datos.linea_de_comandos_para_escribir);
            lee_datos_de_la_interfaz_para_ejecutar(ref datos);
            datos.mejor_valor_global = double.MaxValue;
#if POSIBILIDAD_DE_GRAFICAR
            datos_de_grafico = new grafico_de_una_proteina[MAX_PROTEINAS];
            datos_de_grafico_FO = new valores_de_FO[MAX_REPETICIONES, MAX_GENERACIONES];
            for (int i = 1; i < MAX_PROTEINAS; i++)
            {
                datos_de_grafico[i].nombre_proteina = "";
                datos_de_grafico[i].posiciones_de_aminoacidos_por_proteina_repeticion_y_generacion = new string[MAX_REPETICIONES, MAX_GENERACIONES];
                datos_de_grafico[i].valor_de_la_FO_por_proteina_repeticion_y_generacion = new float[MAX_REPETICIONES, MAX_GENERACIONES];
            }
#endif
            for (datos.repeticion_actual = 1; datos.repeticion_actual <= datos.cantidad_de_repeticiones; datos.repeticion_actual++)
            {
                if (datos.repeticion_actual > 1)
                {
                    lee_datos_de_la_interfaz_para_ejecutar(ref datos);
                    datos.mejor_valor_global = double.MaxValue;
                }
                textBox44.Text = datos.repeticion_actual.ToString();
                textBox44.Refresh();
                datos.semilla_aleatorio = Math.Abs((int)System.DateTime.Now.Ticks);
                //datos.semilla_aleatorio = 1607484024;
                datos.aleatorio = new Random(datos.semilla_aleatorio);
                if (datos.terminar_por_iteraciones_maximas)
                    datos.h_actual = h_por_iteraciones(datos);
                else
                    datos.h_actual = h_por_evaluaciones(datos);
                sintesis(ref datos);
                datos.temperatura_inicial = -datos.proteinas[datos.cantidad_de_proteinas].valor_funcion_actual / Math.Log(datos.probabilidad_de_aceptar_estados_iniciales);
                datos.temperatura_final = -1E-100 / Math.Log(datos.probabilidad_de_aceptar_estados_finales);
                datos.f.fija_cantidad_de_evaluaciones(0);
                if (datos.repeticion_actual == 1)
                {
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Temperatura de recocido inicial: " + datos.temperatura_inicial.ToString() + "\n");
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Temperatura de recocido final: " + datos.temperatura_final.ToString() + "\n");
                }
                if (datos.terminar_por_iteraciones_maximas)
                {
                    cuenta_para_los_operadores_desnaturalizacion_PH = (int)(datos.Porciento_de_las_iteraciones_o_evaluaciones_para_aplicar_desnaturalizacion_de_H_a_P / 100.0 * datos.maximas_generaciones);
                    if (cuenta_para_los_operadores_desnaturalizacion_PH == 0)
                        cuenta_para_los_operadores_desnaturalizacion_PH = 1;
                    cuenta_para_los_operadores_cambios_de_H_y_P = (int)(datos.Porciento_de_iteraciones_o_evaluaciones_para_cambiar_las_H_y_las_P / 100.0 * datos.maximas_generaciones);
                    if (cuenta_para_los_operadores_cambios_de_H_y_P == 0)
                        cuenta_para_los_operadores_cambios_de_H_y_P = 1;
                    cuenta_para_los_operadores_cuaternario = (int)(datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_cuaternario / 100.0 * datos.maximas_generaciones);
                    if (cuenta_para_los_operadores_cuaternario == 0)
                        cuenta_para_los_operadores_cuaternario = 1;
                    cuenta_para_los_operadores_cascada_P = (int)(datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_P / 100.0 * datos.maximas_generaciones);
                    if (cuenta_para_los_operadores_cascada_P == 0)
                        cuenta_para_los_operadores_cascada_P = 1;
                    cuenta_para_los_operadores_cascada_H = (int)(datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_H / 100.0 * datos.maximas_generaciones);
                    if (cuenta_para_los_operadores_cascada_H == 0)
                        cuenta_para_los_operadores_cascada_H = 1;
                }
                else
                {
                    datos.ultimo_Porciento_de_las_iteraciones_o_evaluaciones_para_aplicar_desnaturalizacion_de_H_a_P = datos.Porciento_de_las_iteraciones_o_evaluaciones_para_aplicar_desnaturalizacion_de_H_a_P;
                    cuenta_para_los_operadores_desnaturalizacion_PH = 1;
                    datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_cambiar_las_H_y_las_P = datos.Porciento_de_iteraciones_o_evaluaciones_para_cambiar_las_H_y_las_P;
                    cuenta_para_los_operadores_cambios_de_H_y_P = 1;
                    datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_cuaternario = datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_cuaternario;
                    cuenta_para_los_operadores_cuaternario = 1;
                    datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_P = datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_P;
                    cuenta_para_los_operadores_cascada_P = 1;
                    datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_H = datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_H;
                    cuenta_para_los_operadores_cascada_H = 1;
                }
                datos.stopWatch_por_repeticion.Restart();
                datos.stopWatch_por_repeticion.Start();
                if (!datos.terminar_por_iteraciones_maximas)
                    datos.maximas_generaciones = int.MaxValue;
                for (datos.generacion_actual = 1; datos.generacion_actual <= datos.maximas_generaciones; datos.generacion_actual++)
                {
                    if (datos.terminar_por_iteraciones_maximas)
                    {
                        datos.omega_actual = omega_por_iteraciones(datos);
                        datos.h_actual = h_por_iteraciones(datos);
                        datos.temperatura_actual = temperatura_por_iteraciones(datos);
                    }
                    else
                    {
                        datos.omega_actual = omega_por_evaluaciones(datos);
                        datos.h_actual = h_por_evaluaciones(datos);
                        datos.temperatura_actual = temperatura_por_evaluaciones(datos);
                    }
                    if (datos.terminar_por_iteraciones_maximas)
                        textBox51.Text = datos.generacion_actual.ToString();
                    else
                        textBox51.Text = datos.cantidad_de_evaluaciones_de_la_FO_actual.ToString();
                    textBox51.Refresh();
                    for (int proteina_actual = 1; proteina_actual <= datos.cantidad_de_proteinas; proteina_actual++)
                    {
#if IMPRIME_CODIGO_INTERMEDIO
                        datos.proteinas[proteina_actual].acciones.Clear();
#endif
                        datos.proteinas[proteina_actual].hay_chaperona = false;
                        datos.proteinas[proteina_actual].chaperonas.Clear();
                        datos.proteinas[proteina_actual].hay_cuaternarias = false;
                        datos.proteinas[proteina_actual].cuaternarias.Clear();

                        if (datos.aleatorio.NextDouble() < datos.probabilidad_de_chaperonas && proteina_actual != 1)
                        {
                            if (datos.chaperonas_entre_mejores)
                            {
                                datos.proteinas[proteina_actual].hay_chaperona = true;
                                int cuantas_chaperonas = datos.aleatorio.Next(maximo_de_chaperonas) + 1;
                                int chaperona = 0;
                                while (cuantas_chaperonas > 0)
                                {
                                    chaperona++;
                                    if (!datos.proteinas[chaperona].nombre_proteina.Equals(datos.proteinas[proteina_actual].nombre_proteina)
                                        && !datos.proteinas[proteina_actual].chaperonas.Contains(chaperona))
                                    {
                                        datos.proteinas[proteina_actual].chaperonas.Add(chaperona);
                                        cuantas_chaperonas--;
                                    }
                                }
                            }
                            else
                            {
                                datos.proteinas[proteina_actual].hay_chaperona = true;
                                int cuantas_chaperonas = datos.aleatorio.Next(maximo_de_chaperonas) + 1;
                                if (cuantas_chaperonas >= proteina_actual)
                                    cuantas_chaperonas = proteina_actual - 2;
                                int chaperona = 0;
                                while (cuantas_chaperonas > 0)
                                {
                                    chaperona = datos.aleatorio.Next(1, proteina_actual);
                                    if (!datos.proteinas[chaperona].nombre_proteina.Equals(datos.proteinas[proteina_actual].nombre_proteina)
                                        && !datos.proteinas[proteina_actual].chaperonas.Contains(chaperona)
                                        && datos.proteinas[chaperona].valor_funcion_actual <= datos.proteinas[proteina_actual].valor_funcion_actual)
                                    {
                                        datos.proteinas[proteina_actual].chaperonas.Add(chaperona);
                                        cuantas_chaperonas--;
                                    }
                                }
                            }
                        }
                        datos.proteinas[proteina_actual].quienes_son_HI.Clear();
                        datos.proteinas[proteina_actual].quienes_son_HS.Clear();
                        datos.proteinas[proteina_actual].quienes_son_PI.Clear();
                        datos.proteinas[proteina_actual].quienes_son_PS.Clear();
                        for (int aminoacido = 1; aminoacido <= datos.dimensiones; aminoacido++)
                        {
                            datos.proteinas[proteina_actual].densidad_local[aminoacido] = calcula_Ro(ref datos, proteina_actual, aminoacido);
                            clasifica_aminoacidos(ref datos, proteina_actual , aminoacido);
                        }
                    }
#if IMPRIME_CODIGO_INTERMEDIO
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\n\nIteracion " + datos.generacion_actual.ToString() + " antes de los operadores de folding." + "\n");
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " Temperatura: " + datos.temperatura_actual.ToString() + "\n");
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " h: " + datos.h_actual.ToString() + "\n");
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " Omega: " + datos.omega_actual.ToString() + "\n");
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " Evaluaciones de la FO: " + datos.f.devuelve_cantidad_de_evaluaciones().ToString() + "\n");
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " Mejor valor obtenido: " + datos.mejor_valor_global.ToString() + "\n");
                    imprime_informacion(ref datos, "");
#endif
                    for (int proteina_actual = 1; proteina_actual <= datos.cantidad_de_proteinas; proteina_actual++)
                        metodo_de_folding_evaluando_la_funcion_al_terminar_ciclo_Q(ref datos, proteina_actual);
                    datos.cantidad_de_evaluaciones_de_la_FO_actual = (long)datos.f.devuelve_cantidad_de_evaluaciones();
#if IMPRIME_CODIGO_INTERMEDIO
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\n\nIteracion " + datos.generacion_actual.ToString() + " despues de los operadores de folding." + "\n");
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " Evaluaciones de la FO: " + datos.f.devuelve_cantidad_de_evaluaciones().ToString() + "\n");
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, " Mejor valor obtenido: " + datos.mejor_valor_global.ToString() + "\n");
                    imprime_informacion(ref datos, "");
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\n");
#endif
                    se_ejecuto_alguno = false;
                    if (datos.terminar_por_iteraciones_maximas)
                    {
                        if ((datos.generacion_actual % cuenta_para_los_operadores_desnaturalizacion_PH) == 0)
                        {
                            operador_desnaturalizacion_PH(ref datos);
                            se_ejecuto_alguno = true;
                        }
                        if ((datos.generacion_actual % cuenta_para_los_operadores_cambios_de_H_y_P) == 0)
                            operador_cambios_de_H_y_P(ref datos);
                        
                        if ((datos.generacion_actual % cuenta_para_los_operadores_cuaternario) == 0)
                        {
                            datos.Probabilidad_actual_para_aplicar_el_operador_cuaternario = datos.Probabilidad_inicial_para_aplicar_el_operador_cuaternario -
                                             ((datos.Probabilidad_inicial_para_aplicar_el_operador_cuaternario - Probabilidad_final_para_aplicar_el_operador_cuaternario) *
                                             datos.generacion_actual / datos.maximas_generaciones);
                            operador_cuaternario(ref datos);
                            se_ejecuto_alguno = true;
                        }
                    }
                    else 
                    {
                        cuenta = datos.cantidad_de_evaluaciones_de_la_FO_actual * 100.0 / datos.maximo_numero_de_evaluaciones_de_la_funcion;
                        if (cuenta >= datos.ultimo_Porciento_de_las_iteraciones_o_evaluaciones_para_aplicar_desnaturalizacion_de_H_a_P)
                        {
                             operador_desnaturalizacion_PH(ref datos);
                             datos.ultimo_Porciento_de_las_iteraciones_o_evaluaciones_para_aplicar_desnaturalizacion_de_H_a_P += datos.Porciento_de_las_iteraciones_o_evaluaciones_para_aplicar_desnaturalizacion_de_H_a_P;
                             se_ejecuto_alguno = true;
                        }
                        if (cuenta >= datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_cambiar_las_H_y_las_P)
                        {
                            operador_cambios_de_H_y_P(ref datos);
                            datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_cambiar_las_H_y_las_P += datos.Porciento_de_iteraciones_o_evaluaciones_para_cambiar_las_H_y_las_P;
                        }
                        if (cuenta >= datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_cuaternario)
                        {
                            datos.Probabilidad_actual_para_aplicar_el_operador_cuaternario = datos.Probabilidad_inicial_para_aplicar_el_operador_cuaternario -
                                             ((datos.Probabilidad_inicial_para_aplicar_el_operador_cuaternario - Probabilidad_final_para_aplicar_el_operador_cuaternario) *
                                             datos.cantidad_de_evaluaciones_de_la_FO_actual / datos.maximo_numero_de_evaluaciones_de_la_funcion);
                            operador_cuaternario(ref datos);
                            datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_cuaternario += datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_cuaternario;
                            se_ejecuto_alguno = true;
                        }
                    }
                    if (se_ejecuto_alguno)
                          evalua_proteinas_ordena_aplica_o_no_Boltzman(ref datos, true);
                        
                    //for (int i = 1; i <= datos.cantidad_de_proteinas; i++)
                    //    QuickSort_vector_aminoacidos(ref datos.proteinas[i].aminoacidos_actual, 1, datos.dimensiones);
                    if (datos.terminar_por_iteraciones_maximas)
                    {
                        if ((datos.generacion_actual % cuenta_para_los_operadores_cascada_H) == 0)
                        {
                            if ((datos.proteinas[1].quienes_son_HI.Count + datos.proteinas[1].quienes_son_HS.Count) == 0)
                                goto sale_3;
                            cuantas_proteinas_con_H_quedan = 0;
                            for (int l = 2; l <= datos.cantidad_de_proteinas; l++)
                                if ((datos.proteinas[l].quienes_son_HI.Count + datos.proteinas[l].quienes_son_HS.Count) != 0)
                                    cuantas_proteinas_con_H_quedan++;
                            if (cuantas_proteinas_con_H_quedan == 0)
                                goto sale_3;
                            do
                            {
                                cambiada_inicial = datos.aleatorio.Next(2, datos.cantidad_de_proteinas + 1);
                            }
                            while ((datos.proteinas[cambiada_inicial].quienes_son_HI.Count + datos.proteinas[cambiada_inicial].quienes_son_HS.Count) == 0);
                            aminoacido_de_densidad_mayor = 0;
                            densidad_mayor = int.MinValue;
                            proteinas_ya_en_cascada.Clear();
                            proteinas_ya_en_cascada.Add(1);
                            cantidad = 2;
                            aminoacidos_para_cascada.Clear();
                            proteinas_para_cascada.Clear();
                            proteinas_para_cascada.Add(cambiada_inicial);
                            while (aminoacidos_para_cascada.Count < cantidad)
                            {
                                aminoacido_de_densidad_mayor = 0;
                                densidad_mayor = int.MinValue;
                                for (int aminoacido = 1; aminoacido <= datos.dimensiones; aminoacido++)
                                {
                                    if ((datos.proteinas[1].densidad_local[aminoacido] >= densidad_mayor)
                                        && (!aminoacidos_para_cascada.Contains(aminoacido)))
                                    {
                                        aminoacido_de_densidad_mayor = aminoacido;
                                        densidad_mayor = datos.proteinas[1].densidad_local[aminoacido];
                                    }
                                }
                                if (aminoacido_de_densidad_mayor != 0)
                                    aminoacidos_para_cascada.Add(aminoacido_de_densidad_mayor);
                            }
                            proteinas_ya_en_cascada.Clear();
                            proteinas_ya_en_cascada.Add(1);
#if IMPRIME_CODIGO_INTERMEDIO
                            for (int pr = 1; pr <= datos.cantidad_de_proteinas; pr++)
                                datos.proteinas[pr].acciones.Clear();
#endif
                            operador_de_cascada_de_H_con_reinicio(ref datos, 1, proteinas_ya_en_cascada, proteinas_para_cascada, aminoacidos_para_cascada);
                            evalua_proteinas_ordena_aplica_o_no_Boltzman(ref datos, true);
                        }
                    }
                    else
                    {
                        cuenta = datos.cantidad_de_evaluaciones_de_la_FO_actual * 100.0 / datos.maximo_numero_de_evaluaciones_de_la_funcion;
                        if (cuenta >= datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_H)
                        {
                            datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_H += datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_H;
                            if ((datos.proteinas[1].quienes_son_HI.Count + datos.proteinas[1].quienes_son_HS.Count) == 0)
                                goto sale_3;
                            cuantas_proteinas_con_H_quedan = 0;
                            for (int l = 2; l <= datos.cantidad_de_proteinas; l++)
                                if ((datos.proteinas[l].quienes_son_HI.Count + datos.proteinas[l].quienes_son_HS.Count) != 0)
                                    cuantas_proteinas_con_H_quedan++;
                            if (cuantas_proteinas_con_H_quedan == 0)
                                goto sale_3;
                            do
                            {
                                cambiada_inicial = datos.aleatorio.Next(2, datos.cantidad_de_proteinas + 1);
                            }
                            while ((datos.proteinas[cambiada_inicial].quienes_son_HI.Count + datos.proteinas[cambiada_inicial].quienes_son_HS.Count) == 0);
                            aminoacido_de_densidad_mayor = 0;
                            densidad_mayor = int.MinValue;
                            proteinas_ya_en_cascada.Clear();
                            proteinas_ya_en_cascada.Add(1);
                            cantidad = 2;
                            aminoacidos_para_cascada.Clear();
                            proteinas_para_cascada.Clear();
                            proteinas_para_cascada.Add(cambiada_inicial);
                            while (aminoacidos_para_cascada.Count < cantidad)
                            {
                                aminoacido_de_densidad_mayor = 0;
                                densidad_mayor = int.MinValue;
                                for (int aminoacido = 1; aminoacido <= datos.dimensiones; aminoacido++)
                                {
                                    if ((datos.proteinas[1].densidad_local[aminoacido] >= densidad_mayor)
                                        && (!aminoacidos_para_cascada.Contains(aminoacido)))
                                    {
                                        aminoacido_de_densidad_mayor = aminoacido;
                                        densidad_mayor = datos.proteinas[1].densidad_local[aminoacido];
                                    }
                                }
                                if (aminoacido_de_densidad_mayor != 0)
                                    aminoacidos_para_cascada.Add(aminoacido_de_densidad_mayor);
                            }
                            proteinas_ya_en_cascada.Clear();
                            proteinas_ya_en_cascada.Add(1);
#if IMPRIME_CODIGO_INTERMEDIO
                            for (int pr = 1; pr <= datos.cantidad_de_proteinas; pr++)
                                datos.proteinas[pr].acciones.Clear();
#endif
                            operador_de_cascada_de_H_con_reinicio(ref datos, 1, proteinas_ya_en_cascada, proteinas_para_cascada, aminoacidos_para_cascada);
                            evalua_proteinas_ordena_aplica_o_no_Boltzman(ref datos, true);
                        }
                    }
                sale_3:
                    datos.cantidad_de_evaluaciones_de_la_FO_actual = (long)datos.f.devuelve_cantidad_de_evaluaciones();
                    if (datos.terminar_por_iteraciones_maximas)
                    {
                        if ((datos.generacion_actual % cuenta_para_los_operadores_cascada_P) == 0)
                        {
                            if ((datos.proteinas[1].quienes_son_PI.Count + datos.proteinas[1].quienes_son_PS.Count) == 0)
                                goto sale_4;
                            cuantas_proteinas_con_P_quedan = 0;
                            for (int l = 2; l <= datos.cantidad_de_proteinas; l++)
                                if ((datos.proteinas[l].quienes_son_PI.Count + datos.proteinas[l].quienes_son_PS.Count) != 0)
                                    cuantas_proteinas_con_P_quedan++;
                            if (cuantas_proteinas_con_P_quedan == 0)
                                goto sale_4;
                            do
                            {
                                cambiada_inicial = datos.aleatorio.Next(2, datos.cantidad_de_proteinas + 1);
                            }
                            while ((datos.proteinas[cambiada_inicial].quienes_son_PI.Count + datos.proteinas[cambiada_inicial].quienes_son_PS.Count) == 0);
                            aminoacido_de_densidad_menor = 0;
                            densidad_menor = int.MaxValue;
                            proteinas_ya_en_cascada.Clear();
                            proteinas_ya_en_cascada.Add(1);
                            cantidad = 2;
                            aminoacidos_para_cascada.Clear();
                            proteinas_para_cascada.Clear();
                            proteinas_para_cascada.Add(cambiada_inicial);
                            while (aminoacidos_para_cascada.Count < cantidad)
                            {
                                aminoacido_de_densidad_menor = 0;
                                densidad_menor = int.MaxValue;
                                for (int aminoacido = 1; aminoacido <= datos.dimensiones; aminoacido++)
                                {
                                    if ((datos.proteinas[1].densidad_local[aminoacido] <= densidad_menor)
                                        && (!aminoacidos_para_cascada.Contains(aminoacido)))
                                    {
                                        aminoacido_de_densidad_menor = aminoacido;
                                        densidad_menor = datos.proteinas[1].densidad_local[aminoacido];
                                    }
                                }
                                if (aminoacido_de_densidad_menor != 0)
                                    aminoacidos_para_cascada.Add(aminoacido_de_densidad_menor);
                            }
                            proteinas_ya_en_cascada.Clear();
                            proteinas_ya_en_cascada.Add(1);
#if IMPRIME_CODIGO_INTERMEDIO
                            for (int pr = 1; pr <= datos.cantidad_de_proteinas; pr++)
                                datos.proteinas[pr].acciones.Clear();
                            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\nOperador de cascada P\n");
#endif
                            operador_de_cascada_de_P_con_reinicio(ref datos, 1, proteinas_ya_en_cascada, proteinas_para_cascada, aminoacidos_para_cascada);
                            evalua_proteinas_ordena_aplica_o_no_Boltzman(ref datos, true);
                        }
                    }
                    else
                    {
                        cuenta = datos.cantidad_de_evaluaciones_de_la_FO_actual * 100.0 / datos.maximo_numero_de_evaluaciones_de_la_funcion;
                        if (cuenta >= datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_P)
                        {
                            datos.ultimo_Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_P += datos.Porciento_de_iteraciones_o_evaluaciones_para_aplicar_el_operador_de_cascada_P;
                            if ((datos.proteinas[1].quienes_son_PI.Count + datos.proteinas[1].quienes_son_PS.Count) == 0)
                                goto sale_4;
                            cuantas_proteinas_con_P_quedan = 0;
                            for (int l = 2; l <= datos.cantidad_de_proteinas; l++)
                                if ((datos.proteinas[l].quienes_son_PI.Count + datos.proteinas[l].quienes_son_PS.Count) != 0)
                                    cuantas_proteinas_con_P_quedan++;
                            if (cuantas_proteinas_con_P_quedan == 0)
                                goto sale_4;
                            do
                            {
                                cambiada_inicial = datos.aleatorio.Next(2, datos.cantidad_de_proteinas + 1);
                            }
                            while ((datos.proteinas[cambiada_inicial].quienes_son_PI.Count + datos.proteinas[cambiada_inicial].quienes_son_PS.Count) == 0);
                            aminoacido_de_densidad_menor = 0;
                            densidad_menor = int.MaxValue;
                            proteinas_ya_en_cascada.Clear();
                            proteinas_ya_en_cascada.Add(1);
                            cantidad = 2;
                            aminoacidos_para_cascada.Clear();
                            proteinas_para_cascada.Clear();
                            proteinas_para_cascada.Add(cambiada_inicial);
                            while (aminoacidos_para_cascada.Count < cantidad)
                            {
                                aminoacido_de_densidad_menor = 0;
                                densidad_menor = int.MaxValue;
                                for (int aminoacido = 1; aminoacido <= datos.dimensiones; aminoacido++)
                                {
                                    if ((datos.proteinas[1].densidad_local[aminoacido] <= densidad_menor)
                                        && (!aminoacidos_para_cascada.Contains(aminoacido)))
                                    {
                                        aminoacido_de_densidad_menor = aminoacido;
                                        densidad_menor = datos.proteinas[1].densidad_local[aminoacido];
                                    }
                                }
                                if (aminoacido_de_densidad_menor != 0)
                                    aminoacidos_para_cascada.Add(aminoacido_de_densidad_menor);
                            }
                            proteinas_ya_en_cascada.Clear();
                            proteinas_ya_en_cascada.Add(1);
#if IMPRIME_CODIGO_INTERMEDIO
                            for (int pr = 1; pr <= datos.cantidad_de_proteinas; pr++)
                                datos.proteinas[pr].acciones.Clear();
                            escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\nOperador de cascada P\n");
#endif
                            operador_de_cascada_de_P_con_reinicio(ref datos, 1, proteinas_ya_en_cascada, proteinas_para_cascada, aminoacidos_para_cascada);
                            evalua_proteinas_ordena_aplica_o_no_Boltzman(ref datos, true);
                        }
                    }
                sale_4:
                    operador_proteostasis(ref datos);
                    datos.cantidad_de_evaluaciones_de_la_FO_actual = (long)datos.f.devuelve_cantidad_de_evaluaciones();
#if CALCULO_DE_SHANON
                    calcula_Shanon(ref datos);
#endif
#if POSIBILIDAD_DE_GRAFICAR
                    string cadena_temporal;
                    int numero_proteina;
                    for (int l = 1; l <= datos.cantidad_de_proteinas; l++)
                    {
                        cadena_temporal = datos.proteinas[l].nombre_proteina.Substring(datos.proteinas[l].nombre_proteina.IndexOf(' ')).Trim();
                        numero_proteina = System.Convert.ToInt16(cadena_temporal);
                        datos_de_grafico[numero_proteina].nombre_proteina = cadena_temporal;
                        datos_de_grafico[numero_proteina].valor_de_la_FO_por_proteina_repeticion_y_generacion[datos.repeticion_actual, datos.generacion_actual]
                            = (float)datos.proteinas[numero_proteina].valor_funcion_actual;
                        datos_de_grafico[numero_proteina].posiciones_de_aminoacidos_por_proteina_repeticion_y_generacion
                                [datos.repeticion_actual, datos.generacion_actual] = "";
                        for (int p = 1; p <= datos.dimensiones; p++)
                                datos_de_grafico[numero_proteina].posiciones_de_aminoacidos_por_proteina_repeticion_y_generacion
                                [datos.repeticion_actual, datos.generacion_actual] += datos.proteinas[l].aminoacidos_actual[p].ToString(FORMATO_POSICIONES) + " ";
                    }
                    datos_de_grafico_FO[datos.repeticion_actual, datos.generacion_actual].evaluaciones_de_la_FO = datos.cantidad_de_evaluaciones_de_la_FO_actual;
                    datos_de_grafico_FO[datos.repeticion_actual, datos.generacion_actual].mejor_valor_de_FO = (float)datos.mejor_valor_global;
                    iteraciones_reales = datos.generacion_actual;
#endif
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\nIteracion: " + datos.generacion_actual.ToString());
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\nValor de la FO: " + datos.mejor_valor_global.ToString());
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\nEvaluaciones de la FO acumuladas: " + datos.cantidad_de_evaluaciones_de_la_FO_actual.ToString());
                    datos.archivo_de_salida.Flush();
                    datos.fichero_de_salida_informacion = new System.IO.FileInfo(datos.nombre_del_archivo_de_salida);
                    if (datos.fichero_de_salida_informacion.Length > tamano_por_archivo)
                    {
                        datos.archivo_de_salida.Close();
                        if (!datos.nombre_del_archivo_de_salida.Contains("continuacion"))
                            datos.nombre_del_archivo_de_salida = datos.nombre_del_archivo_de_salida.Substring(0, datos.nombre_del_archivo_de_salida.IndexOf(".txt"));
                        else
                            datos.nombre_del_archivo_de_salida = datos.nombre_del_archivo_de_salida.Substring(0, datos.nombre_del_archivo_de_salida.IndexOf("_continuacion"));
                        datos.continua_archivo++;
                        datos.nombre_del_archivo_de_salida += "_continuacion_" + datos.continua_archivo.ToString() + ".txt";
                        datos.archivo_de_salida = new StreamWriter(datos.nombre_del_archivo_de_salida);
                        datos.archivo_de_salida.AutoFlush = true;
                    }
                    if (!datos.terminar_por_iteraciones_maximas && (datos.cantidad_de_evaluaciones_de_la_FO_actual >= datos.maximo_numero_de_evaluaciones_de_la_funcion))
                       break;
                }
                datos.stopWatch_por_repeticion.Stop();
                datos.ts = datos.stopWatch_por_repeticion.Elapsed;
                datos.tiempo = datos.ts.Hours * 3600.0 + datos.ts.Minutes * 60.0 + datos.ts.Seconds + datos.ts.Milliseconds / 1000.0;
                textBox4.Text = datos.tiempo.ToString();
                textBox4.Refresh();
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\n\nRepeticion: " + datos.repeticion_actual.ToString());
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\nMejor valor obtenido: " + datos.mejor_valor_global.ToString());
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\nMejor posicion: \n");
                for (int k = 1; k <= datos.dimensiones; k++)
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, datos.mejor_posicion_global[k].ToString(FORMATO_POSICIONES) + " ");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\nTiempo de ejecucion: " + datos.tiempo.ToString() + " segundos\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Cantidad de evaluaciones de la FO: " + datos.f.devuelve_cantidad_de_evaluaciones().ToString() + "\n");
                escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "Semilla de la corrida: " + datos.semilla_aleatorio.ToString() + "\n");
                textBox30.Text += datos.nombre_de_la_funcion_detalles + " repeticion " + datos.repeticion_actual.ToString() + Environment.NewLine;
                textBox30.Text += "Mejor valor obtenido: " + datos.mejor_valor_global.ToString(FORMATO_VALORES_FUNCION) + Environment.NewLine;
                textBox30.Text += "Cantidad de evaluaciones de la FO: " + datos.f.devuelve_cantidad_de_evaluaciones().ToString() + Environment.NewLine;
                textBox30.Refresh();
            }
#if POSIBILIDAD_DE_GRAFICAR
            
            archivo_de_salida_grafico.WriteLine("Linea de comandos: " + datos.linea_de_comandos_para_escribir);
            archivo_de_salida_grafico.WriteLine("Nombre de la Funcion: " + datos.nombre_de_la_funcion_detalles);
            if (checkBox1.Checked)
            {
                archivo_de_salida_grafico.WriteLine("Control de la finalizacion: Finalizacion por evaluaciones de la FO"); 
                archivo_de_salida_grafico.WriteLine("Maximo numero de evaluaciones de la FO: " + datos.maximo_numero_de_evaluaciones_de_la_funcion.ToString());
            }
            else
            {
                archivo_de_salida_grafico.WriteLine("Control de la finalizacion: Finalizacion por iteraciones maximas");
                archivo_de_salida_grafico.WriteLine("Maximas iteraciones: " + datos.maximas_generaciones.ToString());
            }
            archivo_de_salida_grafico.WriteLine("Cantidad de repeticiones: " + datos.cantidad_de_repeticiones.ToString());
            archivo_de_salida_grafico.WriteLine("Proteoma (minimo 5 proteinas): " + datos.cantidad_de_proteinas.ToString());
            archivo_de_salida_grafico.WriteLine("Dimension de la Funcion Objetivo (FO): " + datos.dimensiones.ToString());
            for (int k = 1; k <= datos.cantidad_de_repeticiones; k++)
            {
                archivo_de_salida_grafico.WriteLine("Repeticion: " + k.ToString());
                for (int j = 1; j <= iteraciones_reales; j++)
                {
                    archivo_de_salida_grafico.WriteLine("Generacion: " + j.ToString());
                    archivo_de_salida_grafico.WriteLine("Valor de la FO: " + datos_de_grafico_FO[k, j].mejor_valor_de_FO.ToString(FORMATO_VALORES_FUNCION));
                    archivo_de_salida_grafico.WriteLine("Evaluaciones de la FO: " + datos_de_grafico_FO[k, j].evaluaciones_de_la_FO.ToString());
                }
            }
            for (int i = 1; i <= datos.cantidad_de_proteinas; i++)
            {
                archivo_de_salida_grafico.WriteLine("Proteina: " + datos_de_grafico[i].nombre_proteina);
                for (int k = 1; k <= datos.cantidad_de_repeticiones; k++)
                {
                    archivo_de_salida_grafico.WriteLine("Repeticion: " + k.ToString());
                    for (int j = 1; j <= iteraciones_reales; j++)
                    {
                        archivo_de_salida_grafico.WriteLine("Generacion: " + j.ToString());  
                        archivo_de_salida_grafico.WriteLine("Valor de la FO: " + datos_de_grafico[i].valor_de_la_FO_por_proteina_repeticion_y_generacion[k, j].ToString(FORMATO_VALORES_FUNCION));
                        archivo_de_salida_grafico.WriteLine("Posiciones de los aminoacidos: ");
                        archivo_de_salida_grafico.WriteLine(datos_de_grafico[i].posiciones_de_aminoacidos_por_proteina_repeticion_y_generacion[k, j]);
                    }
                }
            }
            archivo_de_salida_grafico.WriteLine();
            archivo_de_salida_grafico.WriteLine();
#endif
            if (!datos.ejecucion_por_lotes)
            {
                datos.archivo_de_salida.Close();
#if POSIBILIDAD_DE_GRAFICAR
                archivo_de_salida_grafico.Close();
#endif
                datos.stopWatch_por_corrida_total.Stop();
                datos.ts = datos.stopWatch_por_corrida_total.Elapsed;
                datos.tiempo = datos.ts.Hours * 3600.0 + datos.ts.Minutes * 60.0 + datos.ts.Seconds + datos.ts.Milliseconds / 1000.0;
                textBox22.Text = datos.tiempo.ToString();
                textBox22.Refresh();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Funciones temp = new Funciones();
            ArrayList nombre = temp.devuelve_funciones_nombre();
            foreach (string s in nombre)
                comboBox1.Items.Add(s);
            comboBox1.SelectedItem = "SPHERE";
            datos.ejecucion_por_lotes = false;
            numericUpDown4.Minimum = 5;
            numericUpDown4.Maximum = 200;

#if CODIGO_CON_VENTANAS_DE_EXPLICACION

            toolTip1.SetToolTip(this.label53, "Definición del problema de optimización.");
            toolTip1.SetToolTip(this.panel8, "Definición del problema de optimización.");
            toolTip1.SetToolTip(this.label4, "Parámetros del algoritmo.");
            toolTip1.SetToolTip(this.panel7, "Parámetros del algoritmo.");
            toolTip1.SetToolTip(this.label52, "Datos de la proteínas nacientes.");
            toolTip1.SetToolTip(this.panel9, "Datos de la proteínas nacientes.");
            toolTip1.SetToolTip(this.panel11, "Datos para la biosíntesis.");
            toolTip1.SetToolTip(this.label51, "Datos para la biosíntesis.");
            toolTip1.SetToolTip(this.panel1, "Probabilidades para el plegado de aminoácidos.");
            toolTip1.SetToolTip(this.label15, "Probabilidades para el plegado de aminoácidos.");
            toolTip1.SetToolTip(this.panel3, "Probabilidades para la nucleación de aminoácidos.");
            toolTip1.SetToolTip(this.label14, "Probabilidades para la nucleación de aminoácidos.");
            toolTip1.SetToolTip(this.panel2, "Datos para el colapso hidrofóbico.");
            toolTip1.SetToolTip(this.label49, "Datos para el colapso hidrofóbico.");
            toolTip1.SetToolTip(this.panel4, "Datos para interacción electrostática.");
            toolTip1.SetToolTip(this.label84, "Datos para interacción electrostática.");
            toolTip1.SetToolTip(this.panel6, "Datos para las proteínas chaperonas.");
            toolTip1.SetToolTip(this.label85, "Datos para las proteínas chaperonas.");
            toolTip1.SetToolTip(this.panel5, "Datos para estructura cuaternaria.");
            toolTip1.SetToolTip(this.label86, "Datos para estructura cuaternaria.");
            toolTip1.SetToolTip(this.panel10, "Datos para el operador de desnaturalización.");
            toolTip1.SetToolTip(this.label88, "Datos para el operador de desnaturalización.");
            toolTip1.SetToolTip(this.panel12, "Datos para el proteosoma.");
            toolTip1.SetToolTip(this.label89, "Datos para el proteosoma.");
            toolTip1.SetToolTip(this.panel15, "Datos para el operador de cascada.");
            toolTip1.SetToolTip(this.label46, "Datos para el operador de cascada.");
            toolTip1.SetToolTip(this.panel14, "Datos para el operador de movimiento aleatorio.");
            toolTip1.SetToolTip(this.label87, "Datos para el operador de movimiento aleatorio.");

            toolTip1.SetToolTip(this.comboBox1,      "Aquí se selecciona la función que desea minimizar.");
            toolTip1.SetToolTip(this.label30,        "Aquí se selecciona la función que desea minimizar.");
            
            toolTip1.SetToolTip(this.textBox18,      "Determina el número máximo de iteraciones que realiza el\n" +
                                                     "algoritmo de optimización.");
            toolTip1.SetToolTip(this.label26,        "Determina el número máximo de iteraciones que realiza el\n" +
                                                     "algoritmo de optimización.");

            toolTip1.SetToolTip(this.textBox28,      "Número de veces que se realizará la optimización con el\n" +
                                                     "mismo set de parámetros.");
            toolTip1.SetToolTip(this.label38,        "Número de veces que se realizará la optimización con el\n" +
                                                     "mismo set de parámetros.");

            toolTip1.SetToolTip(this.numericUpDown4, "Especifica el numero de proteinas que se generarán en el Proteoma\n" +
                                                     "para ejecutar el algorimo.El número mínimo permitido es cinco.");
            toolTip1.SetToolTip(this.label6,         "Especifica el numero de proteinas que se generarán en el Proteoma\n" +
                                                     "para ejecutar el algorimo.El número mínimo permitido es cinco.");

            toolTip1.SetToolTip(this.textBox5,       "Es el número de dimensiones de la Función Objetivo. La proteína\n" +
                                                     "tendrá tantos aminoacidos como dimensiones tenga la función a optimizar.");
            toolTip1.SetToolTip(this.label7,         "Es el número de dimensiones de la Función Objetivo. La proteína\n" +
                                                     "tendrá tantos aminoacidos como dimensiones tenga la función a optimizar.");

            toolTip1.SetToolTip(this.textBox38,      "Es el número máximo de evaluaciones de la Función Objetivo.\n" +
                                                     "Si el algoritmo está ejecutándose por iteraciones cuando\n" +
                                                     "se alcanza este valor se detiene la ejecución. El algoritmo\n" +
                                                     "puede también ejecutarse por el número de evaluaciones de\n" +
                                                     "la Función Objetivo.");
            toolTip1.SetToolTip(this.label50,        "Es el número máximo de evaluaciones de la Función Objetivo.\n" +
                                                     "Si el algoritmo está ejecutándose por iteraciones cuando\n" +
                                                     "se alcanza este valor se detiene la ejecución. El algoritmo\n" +
                                                     "puede también ejecutarse por el número de evaluaciones de\n" +
                                                     "la Función Objetivo.");

            toolTip1.SetToolTip(this.checkBox1,      "Al activar esta opción el algoritmo estará ejecutándose por\n" +
                                                     "la cantidad de evaluaciones de la Función Objetivo. El número \n" +
                                                     "de iteraciones máximas ya no es el factor que finaliza el\n" +
                                                     "proceso de optimización. Además todos los porcentajes estarán\n" +
                                                     "referidos a evaluaciones de la Función Objetivo.");

            toolTip1.SetToolTip(this.textBox2,      "Establece el porciento de aminoacidos de la proteína que\n" +
                                                    "será utilizado para clasificarlo como Superficial o Interno. \n" +
                                                    "Este porciento es expresado en términos de la cantidad de \n" +
                                                    "aminoacidos que tiene la proteína y que coincide con la \n" +
                                                    "dimensión de la Función Objetivo.");
            toolTip1.SetToolTip(this.label2,        "Establece el porciento de aminoacidos de la proteína que\n" +
                                                    "será utilizado para clasificarlo como Superficial o Interno. \n" +
                                                    "Este porciento es expresado en términos de la cantidad de \n" +
                                                    "aminoacidos que tiene la proteína y que coincide con la \n" +
                                                    "dimensión de la Función Objetivo.");

            toolTip1.SetToolTip(this.textBox1,      "Establece el porciento inicial para el valor del parámetro h.\n" +
                                                    "Este valor está referido en porciento del valor del límite\n" +
                                                    "superior del espacio de búsqueda de la primera dimensión de la\n" +
                                                    "Función Objetivo. Este valor de h se utiliza para calcular la\n" +
                                                    "cantidad de aminoácidos a una distancia h y clasificarlo como\n" +
                                                    "interno o externo. El valor de h es actualizado linealmente\n" +
                                                    "mientras se ejecuta el algoritmo.");
            toolTip1.SetToolTip(this.label1,        "Establece el porciento inicial para el valor del parámetro h.\n" +
                                                    "Este valor está referido en porciento del valor del límite\n" +
                                                    "superior del espacio de búsqueda de la primera dimensión de la\n" +
                                                    "Función Objetivo. Este valor de h se utiliza para calcular la\n" +
                                                    "cantidad de aminoácidos a una distancia h y clasificarlo como\n" +
                                                    "interno o externo. El valor de h es actualizado linealmente\n" +
                                                    "mientras se ejecuta el algoritmo.");

            toolTip1.SetToolTip(this.textBox25,     "Establece el porciento final para el valor del parámetro h.\n" +
                                                    "Este valor está referido en porciento del valor del límite\n" +
                                                    "superior del espacio de búsqueda de la primera dimensión de la\n" +
                                                    "Función Objetivo. Este valor de h se utiliza para calcular la\n" +
                                                    "cantidad de aminoácidos a una distancia h y clasificarlo como\n" +
                                                    "interno o externo. El valor de h es actualizado linealmente\n" +
                                                    "mientras se ejecuta el algoritmo.");
            toolTip1.SetToolTip(this.label37,       "Establece el porciento final para el valor del parámetro h.\n" +
                                                    "Este valor está referido en porciento del valor del límite\n" +
                                                    "superior del espacio de búsqueda de la primera dimensión de la\n" +
                                                    "Función Objetivo. Este valor de h se utiliza para calcular la\n" +
                                                    "cantidad de aminoácidos a una distancia h y clasificarlo como\n" +
                                                    "interno o externo. El valor de h es actualizado linealmente\n" +
                                                    "mientras se ejecuta el algoritmo.");

            toolTip1.SetToolTip(this.textBox15,     "Establece el porciento inicial para el valor del parámetro omega.\n" +
                                                    "Este valor está referido en porciento del valor del límite\n" +
                                                    "superior del espacio de búsqueda de la primera dimensión de la\n" +
                                                    "Función Objetivo. El valor de omega se utiliza para cambiar la\n" +
                                                    "posición de los aminoácidos en la proteína. El valor de omega es\n" +
                                                    "actualizado linealmente mientras se ejecuta el algoritmo.");
            toolTip1.SetToolTip(this.label21,       "Establece el porciento inicial para el valor del parámetro omega.\n" +
                                                    "Este valor está referido en porciento del valor del límite\n" +
                                                    "superior del espacio de búsqueda de la primera dimensión de la\n" +
                                                    "Función Objetivo. El valor de omega se utiliza para cambiar la\n" +
                                                    "posición de los aminoácidos en la proteína. El valor de omega es\n" +
                                                    "actualizado linealmente mientras se ejecuta el algoritmo.");

            toolTip1.SetToolTip(this.textBox17,     "Establece el porciento final para el valor del parámetro omega.\n" +
                                                    "Este valor está referido en porciento del valor del límite\n" +
                                                    "superior del espacio de búsqueda de la primera dimensión de la\n" +
                                                    "Función Objetivo. El valor de omega se utiliza para cambiar la\n" +
                                                    "posición de los aminoácidos en la proteína. El valor de omega es\n" +
                                                    "actualizado linealmente mientras se ejecuta el algoritmo.");
            toolTip1.SetToolTip(this.label25,      "Establece el porciento final para el valor del parámetro omega.\n" +
                                                    "Este valor está referido en porciento del valor del límite\n" +
                                                    "superior del espacio de búsqueda de la primera dimensión de la\n" +
                                                    "Función Objetivo. El valor de omega se utiliza para cambiar la\n" +
                                                    "posición de los aminoácidos en la proteína. El valor de omega es\n" +
                                                    "actualizado linealmente mientras se ejecuta el algoritmo.");

            toolTip1.SetToolTip(this.textBox27,     "Cuando al evaluar la Función Objetivo el valor obtenido en esa\n" +
                                                    "nueva solución no mejora el valor anterior, entonces se aplica el\n" +
                                                    "criterio de selección de Boltzman (similar a Recocido Simulado) para\n" +
                                                    "decidir si ese nueva solución será o no aceptada. Este parámetro fija\n" +
                                                    "la probabilidad inicial de aceptación y este valor de probabilidad es\n" +
                                                    "actualizado linealmente mientras se ejecuta el algoritmo.");
            toolTip1.SetToolTip(this.label39,      "Cuando al evaluar la Función Objetivo el valor obtenido en esa\n" +
                                                   "nueva solución no mejora el valor anterior, entonces se aplica el\n" +
                                                   "criterio de selección de Boltzman (similar a Recocido Simulado) para\n" +
                                                   "decidir si ese nueva solución será o no aceptada. Este parámetro fija\n" +
                                                   "la probabilidad inicial de aceptación y este valor de probabilidad es\n" +
                                                   "actualizado linealmente mientras se ejecuta el algoritmo.");

            toolTip1.SetToolTip(this.textBox26,     "Cuando al evaluar la Función Objetivo el valor obtenido en esa\n" +
                                                    "nueva solución no mejora el valor anterior, entonces se aplica el\n" +
                                                    "criterio de selección de Boltzman (similar a Recocido Simulado) para\n" +
                                                    "decidir si ese nueva solución será o no aceptada. Este parámetro fija\n" +
                                                    "la probabilidad inicial de aceptación y este valor de probabilidad es\n" +
                                                    "actualizado linealmente mientras se ejecuta el algoritmo.");
            toolTip1.SetToolTip(this.label40,     "Cuando al evaluar la Función Objetivo el valor obtenido en esa\n" +
                                                    "nueva solución no mejora el valor anterior, entonces se aplica el\n" +
                                                    "criterio de selección de Boltzman (similar a Recocido Simulado) para\n" +
                                                    "decidir si ese nueva solución será o no aceptada. Este parámetro fija\n" +
                                                    "la probabilidad inicial de aceptación y este valor de probabilidad es\n" +
                                                    "actualizado linealmente mientras se ejecuta el algoritmo.");

            toolTip1.SetToolTip(this.textBox7,      "Determina cuántas veces se ejecuta el ciclo de transiciones compuesto\n" +
                                                    "por los siguientes operadores: Colapso Hidrofóbico, Interacción Electrostática,\n" +
                                                    "Plegamiento Hidrofóbico, Plegamiento Polar, Nucleación Hidrofóbica,\n" +
                                                    "Nucleación Polar, atendiendo a su probabilidad de ocurrencia. Si ninguno\n" +
                                                    "de los operadores anteriores se ejecuta entonces el sistema realiza un\n" +
                                                    "cambio aleatorio de los aminoácidos de la proteína en dependencia de su\n" +
                                                    "probabilidad de ocurrencia.");
            toolTip1.SetToolTip(this.label11,      "Determina cuántas veces se ejecuta el ciclo de transiciones compuesto\n" +
                                                    "por los siguientes operadores: Colapso Hidrofóbico, Interacción Electrostática,\n" +
                                                    "Plegamiento Hidrofóbico, Plegamiento Polar, Nucleación Hidrofóbica,\n" +
                                                    "Nucleación Polar, atendiendo a su probabilidad de ocurrencia. Si ninguno\n" +
                                                    "de los operadores anteriores se ejecuta entonces el sistema realiza un\n" +
                                                    "cambio aleatorio de los aminoácidos de la proteína en dependencia de su\n" +
                                                    "probabilidad de ocurrencia.");

            toolTip1.SetToolTip(this.textBox13,     "Probabilidad mínima para que al crear las proteínas en la síntesis se\n" +
                                                    "fije un aminoácideo como Hidrofóbico o Polar. En la síntesis para cada\n" +
                                                    "aminoácido de cada proteína se calcula un valor aleatorio entre la probabilidad\n" +
                                                    "mínima y la máxima y si al generar un valor aleatorio éste es mayor que el\n" +
                                                    "anteriormente calculado entonces el aminoácido se cataloga como Polar sino\n" +
                                                    "se cataloga como Hidrofóbico.");
            toolTip1.SetToolTip(this.label16,       "Probabilidad mínima para que al crear las proteínas en la síntesis se\n" +
                                                    "fije un aminoácideo como Hidrofóbico o Polar. En la síntesis para cada\n" +
                                                    "aminoácido de cada proteína se calcula un valor aleatorio entre la probabilidad\n" +
                                                    "mínima y la máxima y si al generar un valor aleatorio éste es mayor que el\n" +
                                                    "anteriormente calculado entonces el aminoácido se cataloga como Polar sino\n" +
                                                    "se cataloga como Hidrofóbico.");

            toolTip1.SetToolTip(this.textBox36,     "Probabilidad máxima para que al crear las proteínas en la síntesis se\n" +
                                                    "fije un aminoácideo como Hidrofóbico o Polar. En la síntesis para cada\n" +
                                                    "aminoácido de cada proteína se calcula un valor aleatorio entre la probabilidad\n" +
                                                    "mínima y la máxima y si al generar un valor aleatorio éste es mayor que el\n" +
                                                    "anteriormente calculado entonces el aminoácido se cataloga como Polar sino\n" +
                                                    "se cataloga como Hidrofóbico.");
            toolTip1.SetToolTip(this.label47,       "Probabilidad máxima para que al crear las proteínas en la síntesis se\n" +
                                                    "fije un aminoácideo como Hidrofóbico o Polar. En la síntesis para cada\n" +
                                                    "aminoácido de cada proteína se calcula un valor aleatorio entre la probabilidad\n" +
                                                    "mínima y la máxima y si al generar un valor aleatorio éste es mayor que el\n" +
                                                    "anteriormente calculado entonces el aminoácido se cataloga como Polar sino\n" +
                                                    "se cataloga como Hidrofóbico.");

            toolTip1.SetToolTip(this.textBox45,     "Porciento de iteraciones (o evaluaciones de la Función Objetivo) para invocar\n" +
                                                    "al operador que cambia los aminoácidos de Hidrofóbicos a Polar y viceversa. El\n" +
                                                    "valor de esta probabilidad de cambio se actualiza linealmente en la medida que se\n" +
                                                    "ejecuta el algoritmo, entre un valor mínimo y uno máximo. Si usted no desea que\n" +
                                                    "este operador se ejecute entonces de un valor mayor al 100 porciento.");
            toolTip1.SetToolTip(this.label63,     "Porciento de iteraciones (o evaluaciones de la Función Objetivo) para invocar\n" +
                                                    "al operador que cambia los aminoácidos de Hidrofóbicos a Polar y viceversa. El\n" +
                                                    "valor de esta probabilidad de cambio se actualiza linealmente en la medida que se\n" +
                                                    "ejecuta el algoritmo, entre un valor mínimo y uno máximo. Si usted no desea que\n" +
                                                    "este operador se ejecute entonces de un valor mayor al 100 porciento.");

            toolTip1.SetToolTip(this.textBox46,     "Probabilidad inicial de cambio de aminoácidos de Polar a Hirofóbico y viceversa.\n" +
                                                    "El valor de esta probabilidad de cambio se actualiza linealmente en la medida que se\n" +
                                                    "ejecuta el algoritmo, entre un valor inicial y un valor final.");
            toolTip1.SetToolTip(this.label65,     "Probabilidad inicial de cambio de aminoácidos de Polar a Hirofóbico y viceversa.\n" +
                                                    "El valor de esta probabilidad de cambio se actualiza linealmente en la medida que se\n" +
                                                    "ejecuta el algoritmo, entre un valor inicial y un valor final.");

            toolTip1.SetToolTip(this.textBox43,     "Probabilidad final de cambio de aminoácidos de Polar a Hirofóbico y viceversa.\n" +
                                                    "El valor de esta probabilidad de cambio se actualiza linealmente en la medida que se\n" +
                                                    "ejecuta el algoritmo, entre un valor inicial y un valor final.");
            toolTip1.SetToolTip(this.label61,     "Probabilidad final de cambio de aminoácidos de Polar a Hirofóbico y viceversa.\n" +
                                                    "El valor de esta probabilidad de cambio se actualiza linealmente en la medida que se\n" +
                                                    "ejecuta el algoritmo, entre un valor inicial y un valor final.");

            toolTip1.SetToolTip(this.textBox8,      "Probabilidad para el Plegado Hidrofóbico de un aminoacido. Si al generar un número\n" +
                                                    "aleatorio este es menor que esta probabilidad entonces se aplica el operador de\n" +
                                                    "Plegado Hidrofóbico.");
            toolTip1.SetToolTip(this.label12,      "Probabilidad para el Plegado Hidrofóbico de un aminoacido. Si al generar un número\n" +
                                                   "aleatorio este es menor que esta probabilidad entonces se aplica el operador de\n" +
                                                   "Plegado Hidrofóbico.");

            toolTip1.SetToolTip(this.textBox9,      "Probabilidad para el Plegado Polar de un aminoacido. Si al generar un número\n" +
                                                    "aleatorio este es menor que esta probabilidad entonces se aplica el operador de\n" +
                                                    "Plegado Polar.");
            toolTip1.SetToolTip(this.label13,      "Probabilidad para el Plegado Polar de un aminoacido. Si al generar un número\n" +
                                                   "aleatorio este es menor que esta probabilidad entonces se aplica el operador de\n" +
                                                   "Plegado Polar.");

            toolTip1.SetToolTip(this.textBox12,     "Probabilidad para Nucleación Hidrofóbica de un aminoacido. Si al generar un número\n" +
                                                    "aleatorio este es menor que esta probabilidad entonces se aplica el operador de\n" +
                                                    "Nucleación Hidrofóbica.");
            toolTip1.SetToolTip(this.label17,       "Probabilidad para Nucleación Hidrofóbica de un aminoacido. Si al generar un número\n" +
                                                    "aleatorio este es menor que esta probabilidad entonces se aplica el operador de\n" +
                                                    "Nucleación Hidrofóbica.");

            toolTip1.SetToolTip(this.textBox19,     "Probabilidad para Nucleación Polar de un aminoacido. Si al generar un número\n" +
                                                    "aleatorio este es menor que esta probabilidad entonces se aplica el operador de\n" +
                                                    "Nucleación Polar.");
            toolTip1.SetToolTip(this.label27,       "Probabilidad para Nucleación Polar de un aminoacido. Si al generar un número\n" +
                                                    "aleatorio este es menor que esta probabilidad entonces se aplica el operador de\n" +
                                                    "Nucleación Polar.");

            toolTip1.SetToolTip(this.textBox31,     "Probabilidad para el Colapso Hidrofóbico de un aminoacido. Si al generar un número\n" +
                                                    "aleatorio este es menor que esta probabilidad entonces se aplica el operador de\n" +
                                                    "Colapso Hidrofóbico.");
            toolTip1.SetToolTip(this.label42,       "Probabilidad para el Colapso Hidrofóbico de un aminoacido. Si al generar un número\n" +
                                                    "aleatorio este es menor que esta probabilidad entonces se aplica el operador de\n" +
                                                    "Colapso Hidrofóbico.");

            toolTip1.SetToolTip(this.textBox29,     "Cuando se aplica el operador de Colapso Hidrofóbico este valor representa el porciento\n" +
                                                    "máximo de aminoacidos a los que se aplica el operador de Colapso. La cantidad de aminoacidos\n" +
                                                    "es un valor aleatorio entre dos y el resultados de multiplicar este valor porcentual por la\n" +
                                                    "cantidad de aminoacidos de una proteina.");
            toolTip1.SetToolTip(this.label41,       "Cuando se aplica el operador de Colapso Hidrofóbico este valor representa el porciento\n" +
                                                    "máximo de aminoacidos a los que se aplica el operador de Colapso. La cantidad de aminoacidos\n" +
                                                    "es un valor aleatorio entre dos y el resultados de multiplicar este valor porcentual por la\n" +
                                                    "cantidad de aminoacidos de una proteina.");

            toolTip1.SetToolTip(this.textBox33,     "Probabilidad para el operador de Interacción Electrostática. Si al generar un número\n" +
                                                    "aleatorio este es menor que esta probabilidad entonces se aplica el operador de\n" +
                                                    "Interacción Electrostática.");
            toolTip1.SetToolTip(this.label44,       "Probabilidad para el operador de Interacción Electrostática. Si al generar un número\n" +
                                                    "aleatorio este es menor que esta probabilidad entonces se aplica el operador de\n" +
                                                    "Interacción Electrostática.");

            toolTip1.SetToolTip(this.textBox32,     "Cuando se aplica el operador de Interacción Electrostática este valor representa el porciento\n" +
                                                    "máximo de aminoacidos a los que se aplica el operador. La cantidad de aminoacidos\n" +
                                                    "es un valor aleatorio entre dos y el resultados de multiplicar este valor porcentual por la\n" +
                                                    "cantidad de aminoacidos de una proteina.");
            toolTip1.SetToolTip(this.label43,       "Cuando se aplica el operador de Interacción Electrostática este valor representa el porciento\n" +
                                                    "máximo de aminoacidos a los que se aplica el operador. La cantidad de aminoacidos\n" +
                                                    "es un valor aleatorio entre dos y el resultados de multiplicar este valor porcentual por la\n" +
                                                    "cantidad de aminoacidos de una proteina.");

            toolTip1.SetToolTip(this.textBox20,     "Fija la probabilidad de que haya chaperonas para una proteina. Si al generar un valor aleatorio\n" +
                                                    "este es menor que esta probabilidad entonces la proteina tiene chaperonas y la cantidad de chaperonas\n" +
                                                    "es un valor aleatorio para cada proteina entre uno y cuatro.");
            toolTip1.SetToolTip(this.label28,       "Fija la probabilidad de que haya chaperonas para una proteina. Si al generar un valor aleatorio\n" +
                                                    "este es menor que esta probabilidad entonces la proteina tiene chaperonas y la cantidad de chaperonas\n" +
                                                    "es un valor aleatorio para cada proteina entre uno y cuatro.");

            toolTip1.SetToolTip(this.radioButton4,  "Para seleccionar las proteinas chaperonas se hace entre las proteinas que tienen valores de la Función\n" +
                                                    "Objetivo mejor que el de la proteína actual.");
            
            toolTip1.SetToolTip(this.radioButton3,  "Para seleccionar las proteinas chaperonas se hace entre las proteinas que tienen valores de la Función\n" +
                                                    "Objetivo mejor.");
            
            toolTip1.SetToolTip(this.textBox48,     "Porciento de iteraciones (o evaluaciones de la Función Objetivo) para invocar el operador de Estructura\n" +
                                                    "Cuaternaria.");
            toolTip1.SetToolTip(this.label66,       "Porciento de iteraciones (o evaluaciones de la Función Objetivo) para invocar el operador de Estructura\n" +
                                                    "Cuaternaria.");

            toolTip1.SetToolTip(this.textBox50,     "Probabilidad de cambio inicial para cuando se ejecuta el operador de Estructura Cuaternaria. Esta probabilidad\n +" +
                                                    "cambia linealmente a lo largo del proceso de optimización entre este valor y 1.0. Al aplicar el operador\n" +
                                                    "se genera un número aleatorio y si es menor que la probabilidad calculada entonces se acciona ejecuta\n" +
                                                    "para esa proteina el operador con un número aleatorio de otras proteínas del Proteoma.");
            toolTip1.SetToolTip(this.label68,       "Probabilidad de cambio inicial para cuando se ejecuta el operador de Estructura Cuaternaria. Esta probabilidad\n +" +
                                                    "cambia linealmente a lo largo del proceso de optimización entre este valor y 1.0. Al aplicar el operador\n" +
                                                    "se genera un número aleatorio y si es menor que la probabilidad calculada entonces se acciona ejecuta\n" +
                                                    "para esa proteina el operador con un número aleatorio de otras proteínas del Proteoma.");

            toolTip1.SetToolTip(this.textBox41,     "Porciento de iteraciones (o evaluaciones de la Función Objetivo) para invocar\n" +
                                                    "al operador de Desnaturalización.");
            toolTip1.SetToolTip(this.label59,     "Porciento de iteraciones (o evaluaciones de la Función Objetivo) para invocar\n" +
                                                    "al operador de Desnaturalización.");

            toolTip1.SetToolTip(this.textBox39,     "Cuando se aplica el operador de Desnaturalización para todas las proteínas del Proteoma se genera un número\n" +
                                                    "aleatorio y si este valor es menor que la probabilidad dada se procede a desnaturalizar la proteína.");
            toolTip1.SetToolTip(this.label54,       "Cuando se aplica el operador de Desnaturalización para todas las proteínas del Proteoma se genera un número\n" +
                                                    "aleatorio y si este valor es menor que la probabilidad dada se procede a desnaturalizar la proteína.");

            toolTip1.SetToolTip(this.textBox42,     "Porciento máximo del total de aminoácidos a cambiar cuando se aplica el operador de Desnaturalización. Para\n" +
                                                    "cada proteína desnaturalizada es un valor aleatorio entre 1 y el porciento de aminoácidos aquí indicado.");
            toolTip1.SetToolTip(this.label60,       "Porciento máximo del total de aminoácidos a cambiar cuando se aplica el operador de Desnaturalización. Para\n" +
                                                    "cada proteína desnaturalizada es un valor aleatorio entre 1 y el porciento de aminoácidos aquí indicado.");

            toolTip1.SetToolTip(this.textBox40,     "Probabilidad de que un aminoácido sea desnaturalizado si es Hidrofóbico o desnaturalizado si es Polar, si al generar\n" +
                                                    "un valor aleatorio, éste es menor que el valor de esta probabilidad entonces se desnaturalizan en la proteína los\n" +
                                                    "aminoácidos hidrofóbicos en caso contrario se desnaturalizan los aminoácidos Polares.");
            toolTip1.SetToolTip(this.label57,       "Probabilidad de que un aminoácido sea desnaturalizado si es Hidrofóbico o desnaturalizado si es Polar, si al generar\n" +
                                                    "un valor aleatorio, éste es menor que el valor de esta probabilidad entonces se desnaturalizan en la proteína los\n" +
                                                    "aminoácidos hidrofóbicos en caso contrario se desnaturalizan los aminoácidos Polares.");

            toolTip1.SetToolTip(this.textBox34,     "Porciento de proteínas para aplicarles el operador de Proteosoma.\n");
            toolTip1.SetToolTip(this.label45,       "Porciento de proteínas para aplicarles el operador de Proteosoma.\n");

            toolTip1.SetToolTip(this.textBox52,     "Porciento de iteraciones (o evaluaciones de la Función Objetivo) para invocar el operador de cascada Hidrofóbica\n");
            toolTip1.SetToolTip(this.label73,       "Porciento de iteraciones (o evaluaciones de la Función Objetivo) para invocar el operador de cascada Hidrofóbica\n");

            toolTip1.SetToolTip(this.textBox56,     "Porciento de iteraciones (o evaluaciones de la Función Objetivo) para invocar el operador de cascada Polar\n");
            toolTip1.SetToolTip(this.label76,       "Porciento de iteraciones (o evaluaciones de la Función Objetivo) para invocar el operador de cascada Polar\n");

            toolTip1.SetToolTip(this.textBox53,     "Esta es la probabilidad de aplicar un cambio aleatorio en la conformación de la proteínas si ninguno de lo \n" +
                                                    "operadores de ciclo de transiciones es ejecutado.");
            toolTip1.SetToolTip(this.label72,       "Esta es la probabilidad de aplicar un cambio aleatorio en la conformación de la proteínas si ninguno de lo \n" +
                                                    "operadores de ciclo de transiciones es ejecutado.");

            toolTip1.SetToolTip(this.textBox30,     "Aquí se muestra un resúmen de los resultados que va logrando el algoritmo de optimización. Un resumen detallado de las\n" +
                                                    "de las ejecuciones es almacena en un archivo que se crea con el nombre de \"Resultado\" más el día y la hora en que\n " +
                                                    "comenzó la ejecución. Este archivo es un archivo de texto plano. En caso de la ejecución por lotes el nombre del\n" +
                                                    "archivo creado es comienza con el nombre del archivo de lotes leido más la palabra Resultado más la fecha y la hora\n +" +
                                                    "en que comenzó la ejecución");

            toolTip1.SetToolTip(this.textBox44,     "Aquí se muestra la repetición actual del total de repeticiones fijadas para la ejecución del algoritmo.");
            toolTip1.SetToolTip(this.label62,       "Aquí se muestra la repetición actual del total de repeticiones fijadas para la ejecución del algoritmo.");

            toolTip1.SetToolTip(this.textBox51,     "Aquí se muestra la iteración actual o el total de evaluaciones de la Función Objetivo, en dependencia de \n" +
                                                    "si se termina por iteraciones o por cantidad de evaluaciones de la Función Objetivo.");
            toolTip1.SetToolTip(this.label69,       "Aquí se muestra la iteración actual o el total de evaluaciones de la Función Objetivo, en dependencia de \n" +
                                                    "si se termina por iteraciones o por cantidad de evaluaciones de la Función Objetivo.");

            toolTip1.SetToolTip(this.textBox23,     "Mejor valor encontrado de la Función Objetivo en la repeticón actual.");
            toolTip1.SetToolTip(this.label34,       "Mejor valor encontrado de la Función Objetivo en la repeticón actual.");

            toolTip1.SetToolTip(this.textBox4,      "Tiempo de ejecución de la última repetición.");
            toolTip1.SetToolTip(this.label71,       "Tiempo de ejecución de la última repetición.");
            toolTip1.SetToolTip(this.label70,       "Tiempo de ejecución de la última repetición.");

            toolTip1.SetToolTip(this.textBox22,     "Tiempo de ejecución total del algoritmo para todas las repeticiones y todas las Funciones Objetivo en todas las\n" +
                                                    "configuraciones dadas en el archivo de ejecución por lotes).");
            toolTip1.SetToolTip(this.label32,       "Tiempo de ejecución total del algoritmo para todas las repeticiones y todas las Funciones Objetivo en todas las\n" +
                                                    "configuraciones dadas en el archivo de ejecución por lotes).");
            toolTip1.SetToolTip(this.label31,       "Tiempo de ejecución total del algoritmo para todas las repeticiones y todas las Funciones Objetivo en todas las\n" +
                                                    "configuraciones dadas en el archivo de ejecución por lotes).");

#endif
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) 
        {
            datos.nombre_de_la_funcion = comboBox1.SelectedItem.ToString();
        }

        void borra_controles()
        {
            textBox39.Text = "";
            textBox39.Refresh(); 
            textBox40.Text = "";
            textBox40.Refresh();
            textBox41.Text = "";
            textBox41.Refresh();
            textBox42.Text = "";
            textBox42.Refresh();
            textBox48.Text = "";
            textBox48.Refresh();
            textBox50.Text = "";
            textBox50.Refresh();
            textBox38.Text = "";
            textBox38.Refresh();
            textBox45.Text = "";
            textBox45.Refresh();
            textBox46.Text = "";
            textBox46.Refresh();
            textBox43.Text = "";
            textBox43.Refresh();
            textBox1.Text = "";
            textBox1.Refresh();
            textBox2.Text = "";
            textBox2.Refresh();
            numericUpDown4.Value = numericUpDown4.Minimum;
            numericUpDown4.Refresh();
            textBox5.Text = "";
            textBox5.Refresh();
            textBox7.Text = "";
            textBox7.Refresh();
            textBox8.Text = "";
            textBox8.Refresh();
            textBox9.Text = "";
            textBox9.Refresh();
            textBox12.Text = "";
            textBox12.Refresh();
            textBox13.Text = "";
            textBox13.Refresh();
            textBox15.Text = "";
            textBox15.Refresh();
            textBox17.Text = "";
            textBox17.Refresh();
            textBox18.Text = "";
            textBox18.Refresh();
            textBox19.Text = "";
            textBox19.Refresh();
            textBox20.Text = "";
            textBox20.Refresh();
            textBox22.Text = "";
            textBox22.Refresh();
            textBox4.Text = "";
            textBox4.Refresh();
            textBox23.Text = "1e308";
            textBox23.Refresh();
            textBox25.Text = "";
            textBox25.Refresh();
            textBox26.Text = "";
            textBox26.Refresh();
            textBox27.Text = "";
            textBox27.Refresh();
            textBox28.Text = "";
            textBox28.Refresh();
            textBox29.Text = "";
            textBox29.Refresh();
            textBox30.Text = "";
            textBox30.Refresh();
            textBox31.Text = "";
            textBox31.Refresh();
            textBox32.Text = "";
            textBox32.Refresh();
            textBox33.Text = "";
            textBox33.Refresh();
            textBox34.Text = "";
            textBox34.Refresh();
            textBox36.Text = "";
            textBox36.Refresh();
            comboBox1.SelectedItem = "NO_FUNCTION";
            comboBox1.Refresh();
            numericUpDown4.Minimum = 5;
            textBox44.Text= "";
            textBox44.Refresh();
            textBox51.Text = "";
            textBox51.Refresh();
            textBox52.Text = "";
            textBox52.Refresh();
            textBox53.Text = "";
            textBox53.Refresh();
            textBox56.Text = "";
            textBox56.Refresh();
        }

        public void leer_archivo_por_lotes_y_ejecutar(ref datos_de_corrida datos)
        {
            openFileDialog1.InitialDirectory = Application.StartupPath;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            string archivo_de_entrada_de_datos;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                archivo_de_entrada_de_datos = openFileDialog1.FileName;
            else
                return;
            inicia_datos_de_corrida(ref datos);
            ejecutarPorLotesToolStripMenuItem.Enabled = false;
            copiarParámetrosAlClipboardToolStripMenuItem.Enabled = false;
            ejecutarToolStripMenuItem.Enabled = false;  
            menuStrip1.Refresh();
            borra_controles();
            datos.continua_archivo = 0;
            datos.ejecucion_por_lotes = true;
            DateTime tiempo1 = System.DateTime.Now;
            datos.nombre_del_archivo_de_salida = archivo_de_entrada_de_datos.Substring(0, archivo_de_entrada_de_datos.LastIndexOf('.'));
            datos.nombre_del_archivo_de_salida += "_Resultados_" + tiempo1.ToString("yyyy_MM_dd_HH_mm_ss") + ".txt";
            //datos.nombre_del_archivo_de_salida = "Resultados.txt";
            datos.archivo_de_salida = new StreamWriter(datos.nombre_del_archivo_de_salida);
            datos.archivo_de_salida.AutoFlush = true;

#if POSIBILIDAD_DE_GRAFICAR
            archivo_de_salida_grafico = new StreamWriter(datos.nombre_del_archivo_de_salida.Substring(0, datos.nombre_del_archivo_de_salida.LastIndexOf('.')) + "_Grafico.txt");
            archivo_de_salida_grafico.WriteLine("Datos para graficos");
            archivo_de_salida_grafico.AutoFlush = true;
#endif
            StreamReader datos_de_entrada = new StreamReader(archivo_de_entrada_de_datos);
            textBox30.Text = "";
            textBox30.Refresh();
            datos.stopWatch_por_corrida_total.Restart();
            datos.stopWatch_por_corrida_total.Start();
            while (!datos_de_entrada.EndOfStream)
            {
                string cadena = datos_de_entrada.ReadLine();
                
                if (!cadena.StartsWith("#"))
                {
                    string[] datos_separados_de_una_linea = Regex.Split(cadena, @",");
                    comboBox1.SelectedItem = datos_separados_de_una_linea[0].Trim();
                    comboBox1.Refresh();
                    textBox18.Text = datos_separados_de_una_linea[1].Trim();
                    textBox18.Refresh();
                    textBox28.Text = datos_separados_de_una_linea[2].Trim();
                    textBox28.Refresh();
                    numericUpDown4.Value = System.Convert.ToInt32(datos_separados_de_una_linea[3].Trim());
                    numericUpDown4.Refresh();
                    textBox5.Text = datos_separados_de_una_linea[4].Trim();
                    textBox5.Refresh();
                    textBox38.Text = datos_separados_de_una_linea[5].Trim();
                    textBox38.Refresh();
                    if (datos_separados_de_una_linea[6].Trim().Equals("0"))
                        checkBox1.Checked = false;
                    else
                        checkBox1.Checked = true;
                    checkBox1.Refresh();
                    textBox2.Text = datos_separados_de_una_linea[7].Trim();
                    textBox2.Refresh();
                    textBox1.Text = datos_separados_de_una_linea[8].Trim();
                    textBox1.Refresh();
                    textBox25.Text = datos_separados_de_una_linea[9].Trim();
                    textBox25.Refresh();
                    textBox15.Text = datos_separados_de_una_linea[10].Trim();
                    textBox15.Refresh();
                    textBox17.Text = datos_separados_de_una_linea[11].Trim();
                    textBox17.Refresh();
                    textBox27.Text = datos_separados_de_una_linea[12].Trim();
                    textBox27.Refresh();
                    textBox26.Text = datos_separados_de_una_linea[13].Trim();
                    textBox26.Refresh();
                    textBox7.Text = datos_separados_de_una_linea[14].Trim();
                    textBox7.Refresh();
                    textBox13.Text = datos_separados_de_una_linea[15].Trim();
                    textBox13.Refresh();
                    textBox36.Text = datos_separados_de_una_linea[16].Trim();
                    textBox36.Refresh();
                    textBox45.Text = datos_separados_de_una_linea[17].Trim();
                    textBox45.Refresh();
                    textBox46.Text = datos_separados_de_una_linea[18].Trim();
                    textBox46.Refresh();
                    textBox43.Text = datos_separados_de_una_linea[19].Trim();
                    textBox43.Refresh();
                    textBox8.Text = datos_separados_de_una_linea[20].Trim();
                    textBox8.Refresh();
                    textBox9.Text = datos_separados_de_una_linea[21].Trim();
                    textBox9.Refresh();
                    textBox12.Text = datos_separados_de_una_linea[22].Trim();
                    textBox12.Refresh();
                    textBox19.Text = datos_separados_de_una_linea[23].Trim();
                    textBox19.Refresh();
                    textBox31.Text = datos_separados_de_una_linea[24].Trim();
                    textBox31.Refresh();
                    textBox29.Text = datos_separados_de_una_linea[25].Trim();
                    textBox29.Refresh();
                    textBox33.Text = datos_separados_de_una_linea[26].Trim();
                    textBox33.Refresh();
                    textBox32.Text = datos_separados_de_una_linea[27].Trim();
                    textBox32.Refresh();
                    textBox20.Text = datos_separados_de_una_linea[28].Trim();
                    textBox20.Refresh();
                    if (datos_separados_de_una_linea[29].Trim().Equals("0"))
                        radioButton4.Checked = true;
                    if (datos_separados_de_una_linea[29].Trim().Equals("1"))
                        radioButton3.Checked = true;
                    textBox48.Text = datos_separados_de_una_linea[30].Trim();
                    textBox48.Refresh();
                    textBox50.Text = datos_separados_de_una_linea[31].Trim();
                    textBox50.Refresh();
                    textBox41.Text = datos_separados_de_una_linea[32].Trim();
                    textBox41.Refresh();
                    textBox39.Text = datos_separados_de_una_linea[33].Trim();
                    textBox39.Refresh();
                    textBox42.Text = datos_separados_de_una_linea[34].Trim();
                    textBox42.Refresh();
                    textBox40.Text = datos_separados_de_una_linea[35].Trim();
                    textBox40.Refresh();
                    textBox34.Text = datos_separados_de_una_linea[36].Trim();
                    textBox34.Refresh();
                    textBox52.Text = datos_separados_de_una_linea[37].Trim();
                    textBox52.Refresh();
                    textBox56.Text = datos_separados_de_una_linea[38].Trim();
                    textBox56.Refresh();
                    textBox53.Text = datos_separados_de_una_linea[39].Trim();
                    textBox53.Refresh();
                    escribe_en_archivo_de_salida(ref datos.archivo_de_salida, "\n\n");
                    datos.repeticion_actual = 1;
                    procedimiento_de_ejecucion_individual(ref datos);
                }
            }
            datos_de_entrada.Close();
            datos.stopWatch_por_corrida_total.Stop();
            datos.ts = datos.stopWatch_por_corrida_total.Elapsed;
            datos.tiempo = datos.ts.Hours * 3600.0 + datos.ts.Minutes * 60.0 + datos.ts.Seconds + datos.ts.Milliseconds / 1000.0;
            textBox22.Text = datos.tiempo.ToString();
            textBox22.Refresh();
            datos.archivo_de_salida.Close();
#if POSIBILIDAD_DE_GRAFICAR
            archivo_de_salida_grafico.Close();
#endif
            datos.repeticion_actual = 1;
            datos.archivo_de_salida = null;
            datos.ejecucion_por_lotes = false;
            ejecutarPorLotesToolStripMenuItem.Enabled = true;
            copiarParámetrosAlClipboardToolStripMenuItem.Enabled = true;
            ejecutarToolStripMenuItem.Enabled = true;
            menuStrip1.Refresh();
        }

        public void armar_la_linea_de_comandos(ref string cadena)
        {
            cadena = "";
            cadena += comboBox1.SelectedItem.ToString().Trim();
            cadena += ", " + textBox18.Text.Trim();
            cadena += ", " + textBox28.Text.Trim();
            cadena += ", " + numericUpDown4.Value.ToString();
            cadena += ", " + textBox5.Text.Trim();
            cadena += ", " + textBox38.Text.Trim();
            if (checkBox1.Checked)
                cadena += ", " + "1";
            else
                cadena += ", " + "0";
            cadena += ", " + textBox2.Text.Trim();
            cadena += ", " + textBox1.Text.Trim();
            cadena += ", " + textBox25.Text.Trim();
            cadena += ", " + textBox15.Text.Trim();
            cadena += ", " + textBox17.Text.Trim();
            cadena += ", " + textBox27.Text.Trim();
            cadena += ", " + textBox26.Text.Trim();
            cadena += ", " + textBox7.Text.Trim();
            cadena += ", " + textBox13.Text.Trim();
            cadena += ", " + textBox36.Text.Trim();
            cadena += ", " + textBox45.Text.Trim();
            cadena += ", " + textBox46.Text.Trim();
            cadena += ", " + textBox43.Text.Trim();
            cadena += ", " + textBox8.Text.Trim();
            cadena += ", " + textBox9.Text.Trim();
            cadena += ", " + textBox12.Text.Trim();
            cadena += ", " + textBox19.Text.Trim();
            cadena += ", " + textBox31.Text.Trim();
            cadena += ", " + textBox29.Text.Trim();
            cadena += ", " + textBox33.Text.Trim();
            cadena += ", " + textBox32.Text.Trim();
            cadena += ", " + textBox20.Text.Trim();
            if (radioButton4.Checked)
                cadena += ", " + "0";
            if (radioButton3.Checked)
                cadena += ", " + "1";
            cadena += ", " + textBox48.Text.Trim();
            cadena += ", " + textBox50.Text.Trim();
            cadena += ", " + textBox41.Text.Trim();
            cadena += ", " + textBox39.Text.Trim();
            cadena += ", " + textBox42.Text.Trim();
            cadena += ", " + textBox40.Text.Trim();
            cadena += ", " + textBox34.Text.Trim();
            cadena += ", " + textBox52.Text.Trim();
            cadena += ", " + textBox56.Text.Trim();
            cadena += ", " + textBox53.Text.Trim();
        }

        
        
        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                System.Convert.ToInt32(numericUpDown4.Value);
            }
            catch (Exception ex)
            {
                string mensaje_temporal = ex.ToString();
                return;
            }
        }

        private void textBox28_TextChanged(object sender, EventArgs e)
        {
            try
            {
                System.Convert.ToInt32(textBox28.Text);
            }
            catch (Exception ex)
            {
                string mensaje_temporal = ex.ToString();
                return;
            }
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            try
            {
                System.Convert.ToInt32(textBox18.Text);
            }
            catch (Exception ex)
            {
                string mensaje_temporal = ex.ToString();
                return;
            }
        }

        private void ejecutarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ejecutarPorLotesToolStripMenuItem.Enabled = false;
            copiarParámetrosAlClipboardToolStripMenuItem.Enabled = false;
            menuStrip1.Refresh();
            datos.ejecucion_por_lotes = false;
            textBox30.Text = "";
            textBox30.Refresh();
            inicia_datos_de_corrida(ref datos);
            procedimiento_de_ejecucion_individual(ref datos);
            ejecutarToolStripMenuItem.Enabled = true;
            ejecutarPorLotesToolStripMenuItem.Enabled = true;
            copiarParámetrosAlClipboardToolStripMenuItem.Enabled = true;
            menuStrip1.Refresh();

        }

        private void ejecutarPorLotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            leer_archivo_por_lotes_y_ejecutar(ref datos);
        }

        private void copiarParametrosAlClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string cadena = "";
            armar_la_linea_de_comandos(ref cadena);
            System.Windows.Forms.Clipboard.SetText(cadena);
        }


        private void salirDelProgramaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Refresh();
            if (checkBox1.Checked)
            {
                label26.Visible = false;
                textBox18.Visible = false;
                label63.Text = "% de evaluaciones máximas para cambios de H y P=";
                label66.Text = "% de evaluaciones máximas para aplicar estructura cuaternaria=";
                label59.Text = "% de evaluaciones máximas para aplicar desnaturalización =";
                label73.Text = "% de evaluaciones máximas para aplicar operador en H =";
                label69.Text = "Evaluaciones de la FO =";
            }
            else
            {
                label26.Visible = true;
                textBox18.Visible = true;
                label63.Text = "% de iteraciones máximas para cambios de H y P=";
                label66.Text = "% de iteraciones máximas para aplicar estructura cuaternaria=";
                label59.Text = "% de iteraciones máximas para aplicar desnaturalización =";
                label73.Text = "% de iteraciones máximas para aplicar operador en H =";
                label69.Text = "Iteración actual =";
            }
            label26.Refresh();
            textBox18.Refresh();
            label63.Refresh();
            label59.Refresh();
            label73.Refresh();
            label69.Refresh();
        }

        
    }
    
}
