using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;

namespace OptimizacionFolding
{
    public class Funciones
    {
        public const int MAX_DIMENSIONS = 101;
        public const double INF = 1.0e99;
        public const int MAX_FUNCIONES_COMPUESTAS = 10;
        public const double PI_x_2 = 2.0D * Math.PI;
        public const double PI_x_3 = 3.0D * Math.PI;
        public const double PI_x_04 = 0.4D * Math.PI;
        public const double PI_e_2 = Math.PI / 2.0D;
        public const double PI_x_5 = 5.0D * Math.PI;
        public double[,] diagonal_matrix_CEC_2013 = new double[MAX_DIMENSIONS, MAX_DIMENSIONS];
        public double[] z_compuesta_CEC_2005 = new double[MAX_FUNCIONES_COMPUESTAS+1];
        public double[,] oinew_CEC_2005 = new double[MAX_FUNCIONES_COMPUESTAS+1, MAX_DIMENSIONS];
        public double[,] oinew_CEC_2013 = new double[MAX_FUNCIONES_COMPUESTAS + 1, MAX_DIMENSIONS];
        public double[,] oiold_CEC_2005 = new double[MAX_FUNCIONES_COMPUESTAS+1, MAX_DIMENSIONS];
        public double[,,] orthogonal_matrix_M_CEC_2005 = new double[MAX_FUNCIONES_COMPUESTAS+1, MAX_DIMENSIONS, MAX_DIMENSIONS];
        public double[,,] orthogonal_matrix_M_CEC_2013 = new double[MAX_FUNCIONES_COMPUESTAS + 1, MAX_DIMENSIONS, MAX_DIMENSIONS];
        public double[] f_bias_CEC_2005 = new double[MAX_FUNCIONES_COMPUESTAS+1];
        public double function_off_set;
        public double oraculo_actual = 1E10;
        public Random aleatorio = new Random((int)System.DateTime.Now.Ticks);
        public ulong cantidad_de_veces_que_se_evalua_la_funcion = 0;
              
        public Funciones()
        {
            
        }

        public void inicia_datos_de_corrida_matrices_y_desplazamientos(int dimension)
        {
            genera_matriz_orthogonal_M_CEC_2005(dimension);
            genera_oinew_para_compuestas_CEC_2005(dimension, -5.0D, 5.0D);
            lee_matriz_orthogonal_M_CEC_2013(dimension);
            lee_oinew_para_compuestas_CEC_2013(dimension);
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
                for (int j = 1; j <= dimension; j++)
                    oiold_CEC_2005[i, j] = 0.0D;
            for (int j = 1; j <= dimension; j++)
                oinew_CEC_2005[1, j] = 0.0D;
            for (int j = 1; j <= MAX_FUNCIONES_COMPUESTAS; j++)
            {
                z_compuesta_CEC_2005[j] = 5.0D;
                f_bias_CEC_2005[j] = (j - 1) * 100.0D;
            }
            
        }
        
        public void fija_function_offset(double valor)
        {
            function_off_set = valor;
        }

        public void genera_oinew_para_compuestas_CEC_2005(int dimension, double inicio_min, double inicio_max)
        {
            Random r = new Random((int)DateTime.Now.Ticks);
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
                for (int k = 1; k <= dimension; k++)
                     oinew_CEC_2005[i, k] = inicio_min + (Math.Abs(inicio_min) + Math.Abs(inicio_max)) * r.NextDouble();
                
        }

        public void genera_matriz_orthogonal_M_CEC_2005(int dimension)
        {
            double[,] M;
            double det_compuesta;
            Random r = new Random((int)DateTime.Now.Ticks);
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                do
                {
                    alglib.rmatrixrndorthogonal(dimension, out M);
                    det_compuesta = alglib.rmatrixdet(M);
                }
                while (det_compuesta != 1.0D);
                for (int j = 1; j <= dimension; j++)
                    for (int k = 1; k <= dimension; k++)
                        orthogonal_matrix_M_CEC_2005[i, j, k] = M[j-1, k-1];
            }
        }
        public void lee_oinew_para_compuestas_CEC_2013(int dimension)
        {
            Regex rx = new Regex(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?", RegexOptions.Compiled);
            string carpeta = Application.ExecutablePath;
            string cadena;
            carpeta=carpeta.Substring(0, carpeta.LastIndexOf("\\")+1)+ "Datos_Matrices_oinew_offsetFO\\oinew_data.txt";
            StreamReader archivo = new StreamReader(carpeta);
            for (int i = 1; i <= 10; i++)
            {
                cadena = archivo.ReadLine().Trim();
                MatchCollection matches = rx.Matches(cadena);
                int d = 1;
                foreach (Match a in matches)
                {
                    oinew_CEC_2013[i, d] = System.Convert.ToDouble(a.ToString());
                    d++;
                }
            }
            archivo.Close();
        }
        public void lee_matriz_orthogonal_M_CEC_2013(int dimension)
        {

            
            Regex rx = new Regex(@"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?", RegexOptions.Compiled);
            string carpeta = Application.ExecutablePath;
            string cadena;
            carpeta = carpeta.Substring(0, carpeta.LastIndexOf("\\") + 1) + "Datos_Matrices_oinew_offsetFO\\M_D" + dimension.ToString() + ".txt";
            StreamReader archivo = new StreamReader(carpeta);
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                for (int j = 1; j <= dimension; j++)
                {
                    cadena = archivo.ReadLine().Trim();
                    MatchCollection matches = rx.Matches(cadena);
                    int k = 1;
                    foreach (Match a in matches)
                    {
                        orthogonal_matrix_M_CEC_2013[i, j, k] = System.Convert.ToDouble(a.ToString()); 
                        k++;
                    }
                }
            }
        }
                      
        public ulong devuelve_cantidad_de_evaluaciones()
        {
            return (cantidad_de_veces_que_se_evalua_la_funcion);
        }

        public void fija_cantidad_de_evaluaciones(ulong valor)
        {
            cantidad_de_veces_que_se_evalua_la_funcion=valor;
        }

        public void convierte_cadena_a_nombre_de_funcion(string cadena, ref tipo_funciones actual_function)
        {
            if (cadena.CompareTo("ACKLEY_1") == 0) actual_function = tipo_funciones.ACKLEY_1;
            else if (cadena.CompareTo("ROTATED_ACKLEY_1") == 0) actual_function = tipo_funciones.ROTATED_ACKLEY_1;
            else if (cadena.CompareTo("ACKLEY_2") == 0) actual_function = tipo_funciones.ACKLEY_2;
            else if (cadena.CompareTo("ACKLEY_3") == 0) actual_function = tipo_funciones.ACKLEY_3;
            else if (cadena.CompareTo("ACKLEY_4") == 0) actual_function = tipo_funciones.ACKLEY_4;
            else if (cadena.CompareTo("ADJIMAN") == 0) actual_function = tipo_funciones.ADJIMAN;
            else if (cadena.CompareTo("ALPINE_1") == 0) actual_function = tipo_funciones.ALPINE_1;
            else if (cadena.CompareTo("ALPINE_2") == 0) actual_function = tipo_funciones.ALPINE_2;

            else if (cadena.CompareTo("BARTELS") == 0) actual_function = tipo_funciones.BARTELS;
            else if (cadena.CompareTo("BEALE") == 0) actual_function = tipo_funciones.BEALE;
            else if (cadena.CompareTo("BIRD") == 0) actual_function = tipo_funciones.BIRD;

            else if (cadena.CompareTo("BIGGS_EXP2") == 0) actual_function = tipo_funciones.BIGGS_EXP2;
            else if (cadena.CompareTo("BIGGS_EXP3") == 0) actual_function = tipo_funciones.BIGGS_EXP3;
            else if (cadena.CompareTo("BIGGS_EXP4") == 0) actual_function = tipo_funciones.BIGGS_EXP4;
            else if (cadena.CompareTo("BIGGS_EXP5") == 0) actual_function = tipo_funciones.BIGGS_EXP5;
            else if (cadena.CompareTo("BIGGS_EXP6") == 0) actual_function = tipo_funciones.BIGGS_EXP6;

            else if (cadena.CompareTo("BOHACHEVSKY_1") == 0) actual_function = tipo_funciones.BOHACHEVSKY_1;
            else if (cadena.CompareTo("BOHACHEVSKY_2") == 0) actual_function = tipo_funciones.BOHACHEVSKY_2;
            else if (cadena.CompareTo("BOHACHEVSKY_3") == 0) actual_function = tipo_funciones.BOHACHEVSKY_3;

            else if (cadena.CompareTo("BOOTH") == 0) actual_function = tipo_funciones.BOOTH;
            else if (cadena.CompareTo("BOX_BETTS") == 0) actual_function = tipo_funciones.BOX_BETTS;

            else if (cadena.CompareTo("BRAD") == 0) actual_function = tipo_funciones.BRAD;

            else if (cadena.CompareTo("BRANIN_1") == 0) actual_function = tipo_funciones.BRANIN_1;
            else if (cadena.CompareTo("BRANIN_2") == 0) actual_function = tipo_funciones.BRANIN_2;

            else if (cadena.CompareTo("BRENT") == 0) actual_function = tipo_funciones.BRENT;
            else if (cadena.CompareTo("BROWN") == 0) actual_function = tipo_funciones.BROWN;

            else if (cadena.CompareTo("BUKIN_2") == 0) actual_function = tipo_funciones.BUKIN_2;
            else if (cadena.CompareTo("BUKIN_4") == 0) actual_function = tipo_funciones.BUKIN_4;
            else if (cadena.CompareTo("BUKIN_6") == 0) actual_function = tipo_funciones.BUKIN_6;

            else if (cadena.CompareTo("CEC_2005_CF1") == 0) actual_function = tipo_funciones.CEC_2005_CF1;
            else if (cadena.CompareTo("CEC_2005_CF2") == 0) actual_function = tipo_funciones.CEC_2005_CF2;
            else if (cadena.CompareTo("CEC_2005_CF3") == 0) actual_function = tipo_funciones.CEC_2005_CF3;
            else if (cadena.CompareTo("CEC_2005_CF4") == 0) actual_function = tipo_funciones.CEC_2005_CF4;
            else if (cadena.CompareTo("CEC_2005_CF5") == 0) actual_function = tipo_funciones.CEC_2005_CF5;
            else if (cadena.CompareTo("CEC_2005_CF6") == 0) actual_function = tipo_funciones.CEC_2005_CF6;
            else if (cadena.CompareTo("CHEN_BIRD") == 0) actual_function = tipo_funciones.CHEN_BIRD;
            else if (cadena.CompareTo("CHEN_V") == 0) actual_function = tipo_funciones.CHEN_V;
            else if (cadena.CompareTo("CHICHINADZE") == 0) actual_function = tipo_funciones.CHICHINADZE;
            else if (cadena.CompareTo("CHUNG_REYNOLDS") == 0) actual_function = tipo_funciones.CHUNG_REYNOLDS;


            else if (cadena.CompareTo("COLVILLE") == 0) actual_function = tipo_funciones.COLVILLE;
            else if (cadena.CompareTo("CORANA") == 0) actual_function = tipo_funciones.CORANA;
            else if (cadena.CompareTo("COSINE_MIXTURE_2") == 0) actual_function = tipo_funciones.COSINE_MIXTURE_2;
            else if (cadena.CompareTo("COSINE_MIXTURE_4") == 0) actual_function = tipo_funciones.COSINE_MIXTURE_4;

            else if (cadena.CompareTo("CROSS_IN_TRAY") == 0) actual_function = tipo_funciones.CROSS_IN_TRAY;
            else if (cadena.CompareTo("CSENDES") == 0) actual_function = tipo_funciones.CSENDES;
            else if (cadena.CompareTo("CUBE") == 0) actual_function = tipo_funciones.CUBE;

            else if (cadena.CompareTo("DAMAVANDI") == 0) actual_function = tipo_funciones.DAMAVANDI;
            else if (cadena.CompareTo("DEB_1") == 0) actual_function = tipo_funciones.DEB_1;
            else if (cadena.CompareTo("DEB_3") == 0) actual_function = tipo_funciones.DEB_3;
            else if (cadena.CompareTo("DECKKERS_AARTS") == 0) actual_function = tipo_funciones.DECKKERS_AARTS;
            else if (cadena.CompareTo("DE_VILLIERS_GLASSER_1") == 0) actual_function = tipo_funciones.DE_VILLIERS_GLASSER_1;
            else if (cadena.CompareTo("DE_VILLIERS_GLASSER_2") == 0) actual_function = tipo_funciones.DE_VILLIERS_GLASSER_2;

            else if (cadena.CompareTo("DIXON_PRICE") == 0) actual_function = tipo_funciones.DIXON_PRICE;

            else if (cadena.CompareTo("DOLAN") == 0) actual_function = tipo_funciones.DOLAN;

            else if (cadena.CompareTo("EASOM") == 0) actual_function = tipo_funciones.EASOM;
            else if (cadena.CompareTo("EGG_CRATE") == 0) actual_function = tipo_funciones.EGG_CRATE;
            else if (cadena.CompareTo("EGG_HOLDER") == 0) actual_function = tipo_funciones.EGG_HOLDER;
            else if (cadena.CompareTo("EL_ATTAR_VIDYASAGAR_DUTTA") == 0) actual_function = tipo_funciones.EL_ATTAR_VIDYASAGAR_DUTTA;
            else if (cadena.CompareTo("EXPONENTIAL") == 0) actual_function = tipo_funciones.EXPONENTIAL;
            else if (cadena.CompareTo("EXP_2") == 0) actual_function = tipo_funciones.EXP_2;

            else if (cadena.CompareTo("FREUDENSTEIN_ROTH") == 0) actual_function = tipo_funciones.FREUDENSTEIN_ROTH;

            else if (cadena.CompareTo("GIUNTA") == 0) actual_function = tipo_funciones.GIUNTA;

            else if (cadena.CompareTo("GEAR") == 0) actual_function = tipo_funciones.GEAR;
            else if (cadena.CompareTo("GENERALIZED_PENALIZED_1") == 0) actual_function = tipo_funciones.GENERALIZED_PENALIZED_1;
            else if (cadena.CompareTo("GENERALIZED_PENALIZED_2") == 0) actual_function = tipo_funciones.GENERALIZED_PENALIZED_2;
            else if (cadena.CompareTo("GOLDSTEIN_PRICE") == 0) actual_function = tipo_funciones.GOLDSTEIN_PRICE;
            else if (cadena.CompareTo("GRIEWANK") == 0) actual_function = tipo_funciones.GRIEWANK;

            else if (cadena.CompareTo("HELICAL_VALLEY") == 0) actual_function = tipo_funciones.HELICAL_VALLEY;

            else if (cadena.CompareTo("JENNRICH_SAMPSON") == 0) actual_function = tipo_funciones.JENNRICH_SAMPSON;

            else if (cadena.CompareTo("KEANE") == 0) actual_function = tipo_funciones.KEANE;

            else if (cadena.CompareTo("MIELE_CANTRELL") == 0) actual_function = tipo_funciones.MIELE_CANTRELL;

            else if (cadena.CompareTo("ROTATED_GRIEWANK") == 0) actual_function = tipo_funciones.ROTATED_GRIEWANK;


            else if (cadena.CompareTo("HANSEN") == 0) actual_function = tipo_funciones.HANSEN;
            else if (cadena.CompareTo("HARTMANN_3D") == 0) actual_function = tipo_funciones.HARTMANN_3D;
            else if (cadena.CompareTo("HARTMANN_6D") == 0) actual_function = tipo_funciones.HARTMANN_6D;
            else if (cadena.CompareTo("HIMMELBLAU") == 0) actual_function = tipo_funciones.HIMMELBLAU;
            else if (cadena.CompareTo("HYPERELLIPSOID") == 0) actual_function = tipo_funciones.HYPERELLIPSOID;
            else if (cadena.CompareTo("HOLZMAN_1") == 0) actual_function = tipo_funciones.HOLZMAN_1;
            else if (cadena.CompareTo("HOLZMAN_2") == 0) actual_function = tipo_funciones.HOLZMAN_2;
            else if (cadena.CompareTo("HOSAKI") == 0) actual_function = tipo_funciones.HOSAKI;

            else if (cadena.CompareTo("KATSUURAS") == 0) actual_function = tipo_funciones.KATSUURAS;
            else if (cadena.CompareTo("KOWALIK") == 0) actual_function = tipo_funciones.KOWALIK;

            else if (cadena.CompareTo("LAGERMAN") == 0) actual_function = tipo_funciones.LAGERMAN;
            else if (cadena.CompareTo("LEON") == 0) actual_function = tipo_funciones.LEON;
            else if (cadena.CompareTo("LEVY_8") == 0) actual_function = tipo_funciones.LEVY_8;

            else if (cadena.CompareTo("MATYAS") == 0) actual_function = tipo_funciones.MATYAS;
            else if (cadena.CompareTo("MC_CORMICK") == 0) actual_function = tipo_funciones.MC_CORMICK;
            else if (cadena.CompareTo("MICHALEWICZ") == 0) actual_function = tipo_funciones.MICHALEWICZ;

            else if (cadena.CompareTo("NEUMAIER_PERM") == 0) actual_function = tipo_funciones.NEUMAIER_PERM;
            else if (cadena.CompareTo("NEUMAIER_PERM_0") == 0) actual_function = tipo_funciones.NEUMAIER_PERM_0;

            else if (cadena.CompareTo("PARSOPOULOS") == 0) actual_function = tipo_funciones.PARSOPOULOS;
            else if (cadena.CompareTo("PATHOLOGICAL") == 0) actual_function = tipo_funciones.PATHOLOGICAL;
            else if (cadena.CompareTo("PAVIANI") == 0) actual_function = tipo_funciones.PAVIANI;
            else if (cadena.CompareTo("PEN_HOLDER") == 0) actual_function = tipo_funciones.PEN_HOLDER;
            else if (cadena.CompareTo("PINTER") == 0) actual_function = tipo_funciones.PINTER;
            else if (cadena.CompareTo("POWELL") == 0) actual_function = tipo_funciones.POWELL;
            else if (cadena.CompareTo("POWELL_SINGULAR_2") == 0) actual_function = tipo_funciones.POWELL_SINGULAR_2;
            else if (cadena.CompareTo("POWELL_SUM") == 0) actual_function = tipo_funciones.POWELL_SUM;
            else if (cadena.CompareTo("PRICE_1") == 0) actual_function = tipo_funciones.PRICE_1;
            else if (cadena.CompareTo("PRICE_2") == 0) actual_function = tipo_funciones.PRICE_2;
            else if (cadena.CompareTo("PRICE_4") == 0) actual_function = tipo_funciones.PRICE_4;

            else if (cadena.CompareTo("QING") == 0) actual_function = tipo_funciones.QING;
            else if (cadena.CompareTo("QUADRATIC") == 0) actual_function = tipo_funciones.QUADRATIC;

            else if (cadena.CompareTo("QUARTIC_WITH_NOISE") == 0) actual_function = tipo_funciones.QUARTIC_WITH_NOISE;
            else if (cadena.CompareTo("QUARTIC_WITHOUT_NOISE") == 0) actual_function = tipo_funciones.QUARTIC_WITHOUT_NOISE;

            else if (cadena.CompareTo("QUINTIC") == 0) actual_function = tipo_funciones.QUINTIC;

            else if (cadena.CompareTo("RANA") == 0) actual_function = tipo_funciones.RANA;
            else if (cadena.CompareTo("RASTRIGIN") == 0) actual_function = tipo_funciones.RASTRIGIN;
            else if (cadena.CompareTo("ROTATED_RASTRIGIN") == 0) actual_function = tipo_funciones.ROTATED_RASTRIGIN;
            else if (cadena.CompareTo("ROTATED_NON_CONTINUOUS_RASTRIGIN") == 0) actual_function = tipo_funciones.ROTATED_NON_CONTINUOUS_RASTRIGIN;
            else if (cadena.CompareTo("NON_CONTINUOUS_RASTRIGIN") == 0) actual_function = tipo_funciones.NON_CONTINUOUS_RASTRIGIN;
            else if (cadena.CompareTo("ROSENBROCK") == 0) actual_function = tipo_funciones.ROSENBROCK;
            else if (cadena.CompareTo("ROTATED_HYPERELLIPSOID") == 0) actual_function = tipo_funciones.ROTATED_HYPERELLIPSOID;

            else if (cadena.CompareTo("RUMP") == 0) actual_function = tipo_funciones.RUMP;

            else if (cadena.CompareTo("SALOMON") == 0) actual_function = tipo_funciones.SALOMON;
            else if (cadena.CompareTo("SCHWEFEL") == 0) actual_function = tipo_funciones.SCHWEFEL;
            else if (cadena.CompareTo("ROTATED_SCHWEFEL") == 0) actual_function = tipo_funciones.ROTATED_SCHWEFEL;
            else if (cadena.CompareTo("SCHWEFEL_221") == 0) actual_function = tipo_funciones.SCHWEFEL_221;
            else if (cadena.CompareTo("SCHWEFEL_222") == 0) actual_function = tipo_funciones.SCHWEFEL_222;
            else if (cadena.CompareTo("SHEKEL_FOXHOLE") == 0) actual_function = tipo_funciones.SHEKEL_FOXHOLE;
            else if (cadena.CompareTo("SHEKEL_4_5") == 0) actual_function = tipo_funciones.SHEKEL_4_5;
            else if (cadena.CompareTo("SHEKEL_4_7") == 0) actual_function = tipo_funciones.SHEKEL_4_7;
            else if (cadena.CompareTo("SHEKEL_4_10") == 0) actual_function = tipo_funciones.SHEKEL_4_10;
            else if (cadena.CompareTo("SHUBERT") == 0) actual_function = tipo_funciones.SHUBERT;
            else if (cadena.CompareTo("SIX_HUMP_CAMEL") == 0) actual_function = tipo_funciones.SIX_HUMP_CAMEL;
            else if (cadena.CompareTo("SPHERE") == 0) actual_function = tipo_funciones.SPHERE;
            else if (cadena.CompareTo("CEC_2013_1_SPHERE") == 0) actual_function = tipo_funciones.CEC_2013_1_SPHERE;
            else if (cadena.CompareTo("CEC_2013_2_ROTATED_HIGH_CONDITIONED_ELLIPTIC") == 0) actual_function = tipo_funciones.CEC_2013_2_ROTATED_HIGH_CONDITIONED_ELLIPTIC;
            else if (cadena.CompareTo("CEC_2013_3_ROTATED_BENT_CIGAR") == 0) actual_function = tipo_funciones.CEC_2013_3_ROTATED_BENT_CIGAR;
            else if (cadena.CompareTo("CEC_2013_4_ROTATED_DISCUS") == 0) actual_function = tipo_funciones.CEC_2013_4_ROTATED_DISCUS;
            else if (cadena.CompareTo("CEC_2013_5_DIFFERENT_POWERS") == 0) actual_function = tipo_funciones.CEC_2013_5_DIFFERENT_POWERS;
            else if (cadena.CompareTo("CEC_2013_5A_ROTATED_DIFFERENT_POWERS") == 0) actual_function = tipo_funciones.CEC_2013_5A_ROTATED_DIFFERENT_POWERS;
            else if (cadena.CompareTo("CEC_2013_6_ROTATED_ROSENBROCK") == 0) actual_function = tipo_funciones.CEC_2013_6_ROTATED_ROSENBROCK;
            else if (cadena.CompareTo("CEC_2013_7_ROTATED_SCHAFFER_F7") == 0) actual_function = tipo_funciones.CEC_2013_7_ROTATED_SCHAFFER_F7;
            else if (cadena.CompareTo("CEC_2013_8_ROTATED_ACKLEY") == 0) actual_function = tipo_funciones.CEC_2013_8_ROTATED_ACKLEY;
            else if (cadena.CompareTo("CEC_2013_9_ROTATED_WEIERSTRASS") == 0) actual_function = tipo_funciones.CEC_2013_9_ROTATED_WEIERSTRASS;
            else if (cadena.CompareTo("CEC_2013_10_ROTATED_GRIEWANK") == 0) actual_function = tipo_funciones.CEC_2013_10_ROTATED_GRIEWANK;
            else if (cadena.CompareTo("CEC_2013_11_RASTRIGIN") == 0) actual_function = tipo_funciones.CEC_2013_11_RASTRIGIN;
            else if (cadena.CompareTo("CEC_2013_12_ROTATED_RASTRIGIN") == 0) actual_function = tipo_funciones.CEC_2013_12_ROTATED_RASTRIGIN;
            else if (cadena.CompareTo("CEC_2013_13_NON_CONTINUOUS_ROTATED_RASTRIGIN") == 0) actual_function = tipo_funciones.CEC_2013_13_NON_CONTINUOUS_ROTATED_RASTRIGIN;
            else if (cadena.CompareTo("CEC_2013_14_SCHWEFEL") == 0) actual_function = tipo_funciones.CEC_2013_14_SCHWEFEL;
            else if (cadena.CompareTo("CEC_2013_15_ROTATED_SCHWEFEL") == 0) actual_function = tipo_funciones.CEC_2013_15_ROTATED_SCHWEFEL;
            else if (cadena.CompareTo("CEC_2013_16_ROTATED_KATSUURA") == 0) actual_function = tipo_funciones.CEC_2013_16_ROTATED_KATSUURA;
            else if (cadena.CompareTo("CEC_2013_17_LUNACEK_BI_RASTRIGIN") == 0) actual_function = tipo_funciones.CEC_2013_17_LUNACEK_BI_RASTRIGIN;
            else if (cadena.CompareTo("CEC_2013_18_ROTATED_LUNACEK_BI_RASTRIGIN") == 0) actual_function = tipo_funciones.CEC_2013_18_ROTATED_LUNACEK_BI_RASTRIGIN;
            else if (cadena.CompareTo("CEC_2013_19_ROTATED_EXPANDED_GRIEWANK_PLUS_ROSENBROCK") == 0) actual_function = tipo_funciones.CEC_2013_19_ROTATED_EXPANDED_GRIEWANK_PLUS_ROSENBROCK;
            else if (cadena.CompareTo("CEC_2013_20_ROTATED_EXPANDED_SCAFFER_6") == 0) actual_function = tipo_funciones.CEC_2013_20_ROTATED_EXPANDED_SCAFFER_6;
            else if (cadena.CompareTo("CEC_2013_21_COMPOSITION_FUNCTION_1") == 0) actual_function = tipo_funciones.CEC_2013_21_COMPOSITION_FUNCTION_1;
            else if (cadena.CompareTo("CEC_2013_22_COMPOSITION_FUNCTION_2") == 0) actual_function = tipo_funciones.CEC_2013_22_COMPOSITION_FUNCTION_2;
            else if (cadena.CompareTo("CEC_2013_23_COMPOSITION_FUNCTION_3") == 0) actual_function = tipo_funciones.CEC_2013_23_COMPOSITION_FUNCTION_3;
            else if (cadena.CompareTo("CEC_2013_24_COMPOSITION_FUNCTION_4") == 0) actual_function = tipo_funciones.CEC_2013_24_COMPOSITION_FUNCTION_4;
            else if (cadena.CompareTo("CEC_2013_25_COMPOSITION_FUNCTION_5") == 0) actual_function = tipo_funciones.CEC_2013_25_COMPOSITION_FUNCTION_5;
            else if (cadena.CompareTo("CEC_2013_26_COMPOSITION_FUNCTION_6") == 0) actual_function = tipo_funciones.CEC_2013_26_COMPOSITION_FUNCTION_6;
            else if (cadena.CompareTo("CEC_2013_27_COMPOSITION_FUNCTION_7") == 0) actual_function = tipo_funciones.CEC_2013_27_COMPOSITION_FUNCTION_7;
            else if (cadena.CompareTo("CEC_2013_28_COMPOSITION_FUNCTION_8") == 0) actual_function = tipo_funciones.CEC_2013_28_COMPOSITION_FUNCTION_8;
            else if (cadena.CompareTo("STEP_1") == 0) actual_function = tipo_funciones.STEP_1;
            else if (cadena.CompareTo("STEP_2") == 0) actual_function = tipo_funciones.STEP_2;
            else if (cadena.CompareTo("STEP_3") == 0) actual_function = tipo_funciones.STEP_3;
            else if (cadena.CompareTo("STYBLINSKI_TANG") == 0) actual_function = tipo_funciones.STYBLINSKI_TANG;

            else if (cadena.CompareTo("SUM_OF_DIFFERENT_POWERS") == 0) actual_function = tipo_funciones.SUM_OF_DIFFERENT_POWERS;
            else if (cadena.CompareTo("SUM_SQUARES") == 0) actual_function = tipo_funciones.SUM_SQUARES;

            else if (cadena.CompareTo("THREE_HUMP_CAMEL") == 0) actual_function = tipo_funciones.THREE_HUMP_CAMEL;
            else if (cadena.CompareTo("TRID") == 0) actual_function = tipo_funciones.TRID;

            else if (cadena.CompareTo("WEIERSTRASS") == 0) actual_function = tipo_funciones.WEIERSTRASS;
            else if (cadena.CompareTo("ROTATED_WEIERSTRASS") == 0) actual_function = tipo_funciones.ROTATED_WEIERSTRASS;

            else if (cadena.CompareTo("XIN_SHE_YANG_1") == 0) actual_function = tipo_funciones.XIN_SHE_YANG_1;
            else if (cadena.CompareTo("XIN_SHE_YANG_2") == 0) actual_function = tipo_funciones.XIN_SHE_YANG_2;
            else if (cadena.CompareTo("XIN_SHE_YANG_3") == 0) actual_function = tipo_funciones.XIN_SHE_YANG_3;
            else if (cadena.CompareTo("XIN_SHE_YANG_4") == 0) actual_function = tipo_funciones.XIN_SHE_YANG_4;
            else if (cadena.CompareTo("XIN_SHE_YANG_5") == 0) actual_function = tipo_funciones.XIN_SHE_YANG_5;
            else if (cadena.CompareTo("XIN_SHE_YANG_6") == 0) actual_function = tipo_funciones.XIN_SHE_YANG_6;
            else if (cadena.CompareTo("XIN_SHE_YANG_7") == 0) actual_function = tipo_funciones.XIN_SHE_YANG_7;
            else if (cadena.CompareTo("XIN_SHE_YANG_8") == 0) actual_function = tipo_funciones.XIN_SHE_YANG_8;

            else if (cadena.CompareTo("YAOLIU09") == 0) actual_function = tipo_funciones.YAOLIU09;
            else if (cadena.CompareTo("YAOLIU04") == 0) actual_function = tipo_funciones.YAOLIU04;

            else if (cadena.CompareTo("ZIRILLI") == 0) actual_function = tipo_funciones.ZIRILLI;
            else if (cadena.CompareTo("ZETTL") == 0) actual_function = tipo_funciones.ZETTL;
            else if (cadena.CompareTo("ZEROSUM") == 0) actual_function = tipo_funciones.ZEROSUM;
            else if (cadena.CompareTo("ZACHAROV") == 0) actual_function = tipo_funciones.ZACHAROV;
            //else if (cadena.CompareTo("PROBLEMA_TENSION_COMPRESSION_SPRING") == 0) actual_function = tipo_funciones.PROBLEMA_TENSION_COMPRESSION_SPRING;
            //else if (cadena.CompareTo("FUNCION_RESTRICCIONES_G1_CEC_2006") == 0) actual_function = tipo_funciones.FUNCION_RESTRICCIONES_G1_CEC_2006;
            
        }

        public bool detalles_de_funcion(tipo_funciones funcion, ref int dimension, ref double[] lb, ref double[] ub, ref string nombre_funcion)
        {
            switch (funcion)
            {
                // SPHERE 
                case tipo_funciones.SPHERE:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    nombre_funcion = "SPHERE ( min= " + function_off_set.ToString() + " en 0,0,0,...)";
                    break;


                // CEC_2013_SPHERE 
                case tipo_funciones.CEC_2013_1_SPHERE:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = -1400.0D;
                    nombre_funcion = "CEC_2013_1_SPHERE ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;

                // CEC_2013_ROTATED_HIGH_CONDITIONED_ELLIPTIC 
                case tipo_funciones.CEC_2013_2_ROTATED_HIGH_CONDITIONED_ELLIPTIC:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = -1300.0D;
                    nombre_funcion = "CEC_2013_2_ROTATED_HIGH_CONDITIONED_ELLIPTIC ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;

                // CEC_2013_3_ROTATED_BENT_CIGAR 
                case tipo_funciones.CEC_2013_3_ROTATED_BENT_CIGAR:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = -1200.0D;
                    nombre_funcion = "CEC_2013_3_ROTATED_BENT_CIGAR ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;

                // CEC_2013_4_ROTATED_DISCUS 
                case tipo_funciones.CEC_2013_4_ROTATED_DISCUS:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = -1100.0D;
                    nombre_funcion = "CEC_2013_4_ROTATED_DISCUS ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;

                // CEC_2013_5_DIFFERENT_POWERS
                case tipo_funciones.CEC_2013_5_DIFFERENT_POWERS:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = -1000.0D;
                    nombre_funcion = "CEC_2013_5_DIFFERENT_POWERS ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;

                // CEC_2013_6_ROTATED_ROSENBROCK
                case tipo_funciones.CEC_2013_6_ROTATED_ROSENBROCK:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = -900.0D;
                    nombre_funcion = "CEC_2013_6_ROTATED_ROSENBROCK ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;

                // CEC_2013_7_ROTATED_SCHAFFER_F7
                case tipo_funciones.CEC_2013_7_ROTATED_SCHAFFER_F7:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = -800.0D;
                    nombre_funcion = "CEC_2013_7_ROTATED_SCHAFFER_F7 ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;

                // CEC_2013_8_ROTATED_ACKLEY
                case tipo_funciones.CEC_2013_8_ROTATED_ACKLEY:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = -700.0D;
                    nombre_funcion = "CEC_2013_8_ROTATED_ACKLEY ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;

                // CEC_2013_9_ROTATED_WEIERSTRASS
                case tipo_funciones.CEC_2013_9_ROTATED_WEIERSTRASS:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = -600.0D;
                    nombre_funcion = "CEC_2013_9_ROTATED_WEIERSTRASS ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;

                // CEC_2013_10_ROTATED_GRIEWANK
                case tipo_funciones.CEC_2013_10_ROTATED_GRIEWANK:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = -500.0D;
                    nombre_funcion = "CEC_2013_10_ROTATED_GRIEWANK ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;

                // CEC_2013_11_RASTRIGIN
                case tipo_funciones.CEC_2013_11_RASTRIGIN:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = -400.0D;
                    nombre_funcion = "CEC_2013_11_RASTRIGIN ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;


                // CEC_2013_12_ROTATED_RASTRIGIN
                case tipo_funciones.CEC_2013_12_ROTATED_RASTRIGIN:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = -300.0D;
                    nombre_funcion = "CEC_2013_12_ROTATED_RASTRIGIN ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;


                // CEC_2013_13_NON_CONTINUOUS_ROTATED_RASTRIGIN
                case tipo_funciones.CEC_2013_13_NON_CONTINUOUS_ROTATED_RASTRIGIN:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = -200.0D;
                    nombre_funcion = "CEC_2013_13_NON_CONTINUOUS_ROTATED_RASTRIGIN ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;


                // CEC_2013_14_SCHWEFEL
                case tipo_funciones.CEC_2013_14_SCHWEFEL:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = -100.0D;
                    nombre_funcion = "CEC_2013_14_SCHWEFEL ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;


                // CEC_2013_15_ROTATED_SCHWEFEL
                case tipo_funciones.CEC_2013_15_ROTATED_SCHWEFEL:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = 100.0D;
                    nombre_funcion = "CEC_2013_15_ROTATED_SCHWEFEL ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;


                // CEC_2013_16_ROTATED_KATSUURA
                case tipo_funciones.CEC_2013_16_ROTATED_KATSUURA:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = 200.0D;
                    nombre_funcion = "CEC_2013_16_ROTATED_KATSUURA ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;


                // CEC_2013_17_LUNACEK_BI_RASTRIGIN
                case tipo_funciones.CEC_2013_17_LUNACEK_BI_RASTRIGIN:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = 300.0D;
                    nombre_funcion = "CEC_2013_17_LUNACEK_BI_RASTRIGIN ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;


                // CEC_2013_18_ROTATED_LUNACEK_BI_RASTRIGIN
                case tipo_funciones.CEC_2013_18_ROTATED_LUNACEK_BI_RASTRIGIN:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = 400.0D;
                    nombre_funcion = "CEC_2013_18_ROTATED_LUNACEK_BI_RASTRIGIN ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;


                // CEC_2013_19_ROTATED_EXPANDED_GRIEWANK_PLUS_ROSENBROCK
                case tipo_funciones.CEC_2013_19_ROTATED_EXPANDED_GRIEWANK_PLUS_ROSENBROCK:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = 500.0D;
                    nombre_funcion = "CEC_2013_19_ROTATED_EXPANDED_GRIEWANK_PLUS_ROSENBROCK ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;


                // CEC_2013_20_ROTATED_EXPANDED_SCAFFER_6
                case tipo_funciones.CEC_2013_20_ROTATED_EXPANDED_SCAFFER_6:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = 600.0D;
                    nombre_funcion = "CEC_2013_20_ROTATED_EXPANDED_SCAFFER_6 ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;


                // CEC_2013_21_COMPOSITION_FUNCTION_1
                case tipo_funciones.CEC_2013_21_COMPOSITION_FUNCTION_1:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = 700.0D;
                    nombre_funcion = "CEC_2013_21_COMPOSITION_FUNCTION_1 ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;

                // CEC_2013_22_COMPOSITION_FUNCTION_2
                case tipo_funciones.CEC_2013_22_COMPOSITION_FUNCTION_2:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = 800.0D;
                    nombre_funcion = "CEC_2013_22_COMPOSITION_FUNCTION_2 ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;

                // CEC_2013_23_COMPOSITION_FUNCTION_3
                case tipo_funciones.CEC_2013_23_COMPOSITION_FUNCTION_3:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = 900.0D;
                    nombre_funcion = "CEC_2013_23_COMPOSITION_FUNCTION_3 ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;

                // CEC_2013_24_COMPOSITION_FUNCTION_4
                case tipo_funciones.CEC_2013_24_COMPOSITION_FUNCTION_4:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = 1000.0D;
                    nombre_funcion = "CEC_2013_24_COMPOSITION_FUNCTION_4 ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;


                // CEC_2013_25_COMPOSITION_FUNCTION_5
                case tipo_funciones.CEC_2013_25_COMPOSITION_FUNCTION_5:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = 1100.0D;
                    nombre_funcion = "CEC_2013_25_COMPOSITION_FUNCTION_5 ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;


                // CEC_2013_26_COMPOSITION_FUNCTION_6
                case tipo_funciones.CEC_2013_26_COMPOSITION_FUNCTION_6:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = 1200.0D;
                    nombre_funcion = "CEC_2013_26_COMPOSITION_FUNCTION_6 ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;

                // CEC_2013_27_COMPOSITION_FUNCTION_7
                case tipo_funciones.CEC_2013_27_COMPOSITION_FUNCTION_7:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = 1300.0D;
                    nombre_funcion = "CEC_2013_27_COMPOSITION_FUNCTION_7 ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;


                // CEC_2013_28_COMPOSITION_FUNCTION_8
                case tipo_funciones.CEC_2013_28_COMPOSITION_FUNCTION_8:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    function_off_set = 1400.0D;
                    nombre_funcion = "CEC_2013_28_COMPOSITION_FUNCTION_8 ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += oinew_CEC_2013[1, i].ToString() + " ";
                    nombre_funcion += ")";
                    break;

                    
                // SCHWEFEL 2.22 (min=0 en 0,0,0,...)
                case tipo_funciones.SCHWEFEL_222:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -10.0D;
                        ub[i] = 10.0D;
                    }
                    nombre_funcion = "SCHWEFEL 2.22 ( min= " + function_off_set.ToString() + " en 0,0,0,...)";
                    break;

                // ROTATED HYPERELLIPSOID
                case tipo_funciones.ROTATED_HYPERELLIPSOID:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    nombre_funcion = "ROTATED HYPERELLIPSOID ( min= " + function_off_set.ToString() + " en 0,0,0,...)";
                    break;

                // RUMP dimension 2
                case tipo_funciones.RUMP:
                    nombre_funcion = "RUMP ( min= " + function_off_set.ToString() + " en 0,0)";
                    dimension = 2;
                    lb[1] = 0.0D;
                    lb[2] = 0.0D;
                    ub[1] = 10.0D;
                    ub[2] = 10.0D;
                    break;

                // SALOMON
                case tipo_funciones.SALOMON:
                    nombre_funcion = "SALOMON ( min= " + function_off_set.ToString() + " en 0,0)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    break;

                // HYPERELLIPSOID
                case tipo_funciones.HYPERELLIPSOID:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -1.0D;
                        ub[i] = 1.0D;
                    }
                    long suma_calcula_minimo = 0;
                    for (int i = 1; i <= dimension; i++)
                        suma_calcula_minimo += (long)Math.Pow(i, 2);
                    nombre_funcion = "HYPERELLIPSOID ( min= " + (suma_calcula_minimo + function_off_set).ToString() + " en 0,0,0,...)";
                    break;

                // SCHWEFEL 2.21
                case tipo_funciones.SCHWEFEL_221:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -10.0D;
                        ub[i] = 10.0D;
                    }
                    nombre_funcion = "SCHWEFEL 2.21 ( min= " + function_off_set.ToString() + " en 0,0,0,...)";
                    break;

                // ROSENBROCK
                case tipo_funciones.ROSENBROCK:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -30.0D;
                        ub[i] = 30.0D;
                    }
                    nombre_funcion = "ROSENBROCK ( min= " + function_off_set.ToString() + " en 1,1,1,...)";
                    break;

                // STEP 1
                case tipo_funciones.STEP_1:
                    nombre_funcion = "STEP 1 ( min= " + function_off_set.ToString() + " en -1 < x < 1)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    break;

                // STEP 2
                case tipo_funciones.STEP_2:
                    nombre_funcion = "STEP 2 ( min= " + function_off_set.ToString() + " en -0.5 <= x < 0.5)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    break;

                // STEP 3
                case tipo_funciones.STEP_3:
                    nombre_funcion = "STEP 3 ( min= " + function_off_set.ToString() + " en -1 < x < 1)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    break;

                // STYBLINSKI_TANG
                case tipo_funciones.STYBLINSKI_TANG:
                    nombre_funcion = "STYBLINSKI_TANG (min= " + (-39.16599D + function_off_set).ToString() + " en xi=-2.903534)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.0D;
                        ub[i] = 5.0D;
                    }
                    break;

                // SUM_OF_DIFFERENT_POWERS
                case tipo_funciones.SUM_OF_DIFFERENT_POWERS:
                    nombre_funcion = "SUM_OF_DIFFERENT_POWERS ( min= " + function_off_set.ToString() + " en xi=0)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -1.0D;
                        ub[i] = 1.0D;
                    }
                    break;

                // SUM_SQUARES
                case tipo_funciones.SUM_SQUARES:
                    nombre_funcion = "SUM_SQUARES ( min= " + function_off_set.ToString() + " en xi=0)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -10.0D;
                        ub[i] = 10.0D;
                    }
                    break;

                // QUARTIC WITH NOISE
                case tipo_funciones.QUARTIC_WITH_NOISE:
                    nombre_funcion = "QUARTIC WITH NOISE ( min= " + function_off_set.ToString() + " + random[0,1] en 0,0,0,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -1.28D;
                        ub[i] = 1.28D;
                    }
                    break;

                // QUARTIC WITHOUT NOISE
                case tipo_funciones.QUARTIC_WITHOUT_NOISE:
                    nombre_funcion = "QUARTIC WITHOUT NOISE ( min= " + function_off_set.ToString() + " en 0,0,0,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -1.28D;
                        ub[i] = 1.28D;
                    }
                    break;

                // QUINTIC
                case tipo_funciones.QUINTIC:
                    nombre_funcion = "QUINTIC ( min= " + function_off_set.ToString() + " en xi=-1 or 2)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -10.0D;
                        ub[i] = 10.0D;
                    }
                    break;

                // RANA
                case tipo_funciones.RANA:
                    nombre_funcion = "RANA ( min= " + (-928.5478 + function_off_set).ToString() + " en ?)";//REVISAR
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -400.0D;
                        ub[i] = 400.0D;
                    }
                    break;

                // SCHWEFEL
                case tipo_funciones.SCHWEFEL:
                    nombre_funcion = "SCHWEFEL ( min= " + function_off_set.ToString() + " en 420.9687, 420.9687, 420.9687,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -500.0D;
                        ub[i] = 500.0D;
                    }
                    break;

                // ROTATED SCHWEFEL
                case tipo_funciones.ROTATED_SCHWEFEL:
                    nombre_funcion = "ROTATED SCHWEFEL ( min= " + function_off_set.ToString() + " en 420.9687, 420.9687, 420.9687,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -500.0D;
                        ub[i] = 500.0D;
                    }
                    break;


                // RASTRIGIN
                case tipo_funciones.RASTRIGIN:
                    nombre_funcion = "RASTRIGIN ( min= " + function_off_set.ToString() + " en 0, 0, 0,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.12D;
                        ub[i] = 5.12D;
                    }
                    break;

                // ROTATED RASTRIGIN
                case tipo_funciones.ROTATED_RASTRIGIN:
                    nombre_funcion = "ROTATED RASTRIGIN ( min= " + function_off_set.ToString() + " en 0, 0, 0,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.12D;
                        ub[i] = 5.12D;
                    }
                    break;

                // ROTATED NON CONTINUOS RASTRIGIN
                case tipo_funciones.ROTATED_NON_CONTINUOUS_RASTRIGIN:
                    nombre_funcion = "ROTATED NON CONTINUOUS RASTRIGIN ( min= " + function_off_set.ToString() + " en 0, 0, 0,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.12D;
                        ub[i] = 5.12D;
                    }
                    break;

                // NON CONTINUOS RASTRIGIN
                case tipo_funciones.NON_CONTINUOUS_RASTRIGIN:
                    nombre_funcion = "NON CONTINUOUS RASTRIGIN ( min= " + function_off_set.ToString() + " en 0, 0, 0,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.12D;
                        ub[i] = 5.12D;
                    }
                    break;


                // ACKLEY 1
                case tipo_funciones.ACKLEY_1:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -32.0D;
                        ub[i] = 32.0D;
                    }
                    nombre_funcion = "ACKLEY 1 ( min= " + function_off_set.ToString() + " en 0,0,0,...)";
                    break;

                // ROTATED ACKLEY 1
                case tipo_funciones.ROTATED_ACKLEY_1:
                    nombre_funcion = "ROTATED ACKLEY 1 ( min= " + function_off_set.ToString() + " en 0,0,0,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -32.0D;
                        ub[i] = 32.0D;
                    }
                    break;


                // ACKLEY 2
                case tipo_funciones.ACKLEY_2:
                    nombre_funcion = "ACKLEY 2 ( min= " + (-200 + function_off_set).ToString() + " en 0,0)";
                    dimension = 2;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -32.0D;
                        ub[i] = 32.0D;
                    }
                    break;

                // ACKLEY 3
                case tipo_funciones.ACKLEY_3:
                    nombre_funcion = "ACKLEY 3 ( min= " + (-195.629028238419 + function_off_set).ToString() + " en (+0.682584587365898, -0.36075325513719) o (-0.682584587365898, -0.36075325513719))";
                    dimension = 2;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -4.0D;
                        ub[i] = 4.0D;
                    }
                    break;

                // ACKLEY 4
                case tipo_funciones.ACKLEY_4:
                    nombre_funcion = "ACKLEY 4 (para dimension 2 min= " + (-4.590101633799122 + function_off_set).ToString() + " en -1.51, -0.755))";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -35.0D;
                        ub[i] = 35.0D;
                    }
                    break;

                // ADJIMAN
                case tipo_funciones.ADJIMAN:
                    nombre_funcion = "ADJIMAN ( min= " + (-2.02181 + function_off_set).ToString() + " en 2, 0))";
                    dimension = 2;
                    lb[1] = -1.0D;
                    lb[2] = -1.0D;
                    ub[1] = 2.0D;
                    ub[2] = 1.0D;
                    break;

                // ALPINE 1
                case tipo_funciones.ALPINE_1:
                    nombre_funcion = "ALPINE 1 ( min= " + function_off_set.ToString() + " en 0,0,0 ....))";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = 0.0D;
                        ub[i] = 10.0D;
                    }
                    break;

                // ALPINE 2
                case tipo_funciones.ALPINE_2:
                    nombre_funcion = "ALPINE 2 ( min= " + (-6.1295 + function_off_set).ToString() + " en 7.917, ? ))";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = 0.0D;
                        ub[i] = 10.0D;
                    }
                    break;

                // BARTELS
                case tipo_funciones.BARTELS:
                    nombre_funcion = "BARTELS ( min= " + (1.0 + function_off_set).ToString() + " en 0,0 ))";
                    dimension = 2;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -500.0D;
                        ub[i] = 500.0D;
                    }
                    break;

                // GRIEWANK
                case tipo_funciones.GRIEWANK:
                    nombre_funcion = "GRIEWANK ( min= " + function_off_set.ToString() + " en 0,0,0,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -600.0D;
                        ub[i] = 600.0D;
                    }
                    break;

                // HELICAL_VALLEY dimension 3
                case tipo_funciones.HELICAL_VALLEY:
                    nombre_funcion = "HELICAL_VALLEY ( min= " + function_off_set.ToString() + " en 1,0,0)";
                    dimension = 3;
                    lb[1] = -10.0D;
                    lb[2] = -10.0D;
                    lb[3] = -10.0D;
                    ub[1] = 10.0D;
                    ub[2] = 10.0D;
                    ub[3] = 10.0D;
                    break;

                // JENNRICH_SAMPSON dimension 2
                case tipo_funciones.JENNRICH_SAMPSON:
                    nombre_funcion = "JENNRICH_SAMPSON ( min= " + (124.3612 + function_off_set).ToString() + " en 0.257825, 0.257825)";
                    dimension = 2;
                    lb[1] = -1.0D;
                    lb[2] = -1.0D;
                    ub[1] = 1.0D;
                    ub[2] = 1.0D;
                    break;

                // KEANE dimension 2
                case tipo_funciones.KEANE:
                    nombre_funcion = "KEANE ( min= " + (-0.673668 + function_off_set).ToString() + " en {0, 1.39325},{1.39325, 0})";
                    dimension = 3;
                    lb[1] = -10.0D;
                    lb[2] = -10.0D;
                    ub[1] = 10.0D;
                    ub[2] = 10.0D;
                    break;

                // MIELE_CANTRELL dimension 4
                case tipo_funciones.MIELE_CANTRELL:
                    nombre_funcion = "MIELE_CANTRELL ( min= " + function_off_set.ToString() + " en 0,1,1,1)";
                    dimension = 4;
                    lb[1] = -1.0D;
                    lb[2] = -1.0D;
                    lb[3] = -1.0D;
                    lb[4] = -1.0D;
                    ub[1] = 1.0D;
                    ub[2] = 1.0D;
                    ub[3] = 1.0D;
                    ub[4] = 1.0D;
                    break;

                // ROTATED GRIEWANK
                case tipo_funciones.ROTATED_GRIEWANK:
                    nombre_funcion = "ROTATED GRIEWANK ( min= " + function_off_set.ToString() + " en 0,0,0,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -600.0D;
                        ub[i] = 600.0D;
                    }
                    break;

                // GENERALIZED PENALIZED 1
                case tipo_funciones.GENERALIZED_PENALIZED_1:
                    nombre_funcion = "GENERALIZED PENALIZED 1 ( min= " + function_off_set.ToString() + " en -1,-1,-1,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -50.0D;
                        ub[i] = 50.0D;
                    }
                    break;

                // GENERALIZED PENALIZED 2
                case tipo_funciones.GENERALIZED_PENALIZED_2:
                    nombre_funcion = "GENERALIZED PENALIZED 2 ( min= " + function_off_set.ToString() + " en 1,1,1,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -50.0D;
                        ub[i] = 50.0D;
                    }
                    break;

                // SHEKEL FOXHOLE siempre dimension 2
                case tipo_funciones.SHEKEL_FOXHOLE:
                    nombre_funcion = "SHEKEL FOXHOLE ( min= " + (0.998003837794449325873406851315 + function_off_set).ToString() + " en -31.97833, -31.97833)";
                    dimension = 2;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -65.536D;
                        ub[i] = 65.536D;
                    }
                    break;

                // KOWALIK siempre dimension 4
                case tipo_funciones.KOWALIK:
                    nombre_funcion = "KOWALIK ( min= " + (0.00030748610 + function_off_set).ToString() + " en 0.192833,0.190836,0.123117,0.135766)";
                    dimension = 4;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.0D;
                        ub[i] = 5.0D;
                    }
                    break;

                // SIX HUMP CAMEL BACK siempre dimension 2
                case tipo_funciones.SIX_HUMP_CAMEL:
                    nombre_funcion = "SIX HUMP CAMEL BACK ( min= " + (-1.0316285 + function_off_set).ToString() + " en 0.08983,-0.7126 y -0.08983,0.7126)";
                    dimension = 2;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.0D;
                        ub[i] = 5.0D;
                    }
                    break;

                // CHEN BIRD siempre dimension 2
                case tipo_funciones.CHEN_BIRD:
                    nombre_funcion = "CHEN BIRD ( min= " + (2000.003999984000 + function_off_set).ToString() + " en 0.5, 0.5 )";
                    dimension = 2;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.0D;
                        ub[i] = 5.0D;
                    }
                    break;

                // CHEN V siempre dimension 2
                case tipo_funciones.CHEN_V:
                    nombre_funcion = "CHEN V ( min= " + (-2000 + function_off_set).ToString() + " en 0.388888888888889, 0.722222222222222))";
                    dimension = 2;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.0D;
                        ub[i] = 5.0D;
                    }
                    break;

                // BRANIN 1 siempre dimension 2
                case tipo_funciones.BRANIN_1:
                    nombre_funcion = "BRANIN 1 ( min= " + (0.397887 + function_off_set).ToString() + " en (-3.142, 12.275) o (3.142, 2.275)o (9.425, 2.425))";
                    dimension = 2;
                    lb[1] = -5.0D; //Modificado para Amsterdan
                    lb[2] = -5.0D;
                    ub[1] = 5.0D;
                    ub[2] = 5.0D;
                    break;

                // BRANIN 2 siempre dimension 2
                case tipo_funciones.BRANIN_2:
                    nombre_funcion = "BRANIN 2 ( min= " + (-0.179891239069905 + function_off_set).ToString() + " en -3.196988423389338, 12.526257883092258))";
                    dimension = 2;
                    lb[1] = -5.0D;
                    lb[2] = 0.0D;
                    ub[1] = 10.0D;
                    ub[2] = 15.0D;
                    break;

                // BRENT siempre dimension 2
                case tipo_funciones.BRENT:
                    nombre_funcion = "BRENT ( min= " + (1.3838965 + function_off_set).ToString() + " en -87 en -10, -10)";
                    dimension = 2;
                    lb[1] = -20.0D;
                    lb[2] = -20.0D;
                    ub[1] = -0.0D;
                    ub[2] = -0.0D;
                    break;

                // BROWN 
                case tipo_funciones.BROWN:
                    nombre_funcion = "BROWN ( min= " + function_off_set.ToString() + " en 0,0,0,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -1.0D;
                        ub[i] = 4.0D;
                    }
                    break;

                // BUKIN 2
                case tipo_funciones.BUKIN_2:
                    nombre_funcion = "BUKIN 2 ( min= " + function_off_set.ToString() + " en -10,0)";
                    dimension = 2;
                    lb[1] = -15.0D;
                    lb[2] = -3.0D;
                    ub[1] = -5.0D;
                    ub[2] = 3.0D;
                    break;

                // BUKIN 4
                case tipo_funciones.BUKIN_4:
                    nombre_funcion = "BUKIN 4 ( min= " + function_off_set.ToString() + " en -10,0)";
                    dimension = 2;
                    lb[1] = -15.0D;
                    lb[2] = -3.0D;
                    ub[1] = -5.0D;
                    ub[2] = 3.0D;
                    break;

                // BUKIN 6
                case tipo_funciones.BUKIN_6:
                    nombre_funcion = "BUKIN 6 ( min= " + function_off_set.ToString() + " en -10,1)";
                    dimension = 2;
                    lb[1] = -15.0D;
                    lb[2] = -3.0D;
                    ub[1] = -5.0D;
                    ub[2] = 3.0D;
                    break;

                // GOLDSTEIN PRICE siempre dimension 2
                case tipo_funciones.GOLDSTEIN_PRICE:
                    nombre_funcion = "GOLDSTEIN PRICE ( min= " + (3.0D + function_off_set).ToString() + " en 0, -1)";
                    dimension = 2;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -2.0D;
                        ub[i] = 2.0D;
                    }
                    break;

                // HARTMANN 3D siempre dimension 3
                case tipo_funciones.HARTMANN_3D:
                    nombre_funcion = "HARTMANN 3D  ( min= " + (-3.86 + function_off_set).ToString() + " en 0.114, 0.556, 0.852)";
                    dimension = 3;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = 0.0D;
                        ub[i] = 1.0D;
                    }
                    break;

                // HARTMANN 6D siempre dimension 6
                case tipo_funciones.HARTMANN_6D:
                    nombre_funcion = "HARTMANN 6D ( min= " + (-3.32 + function_off_set).ToString() + " en 0.201, 0.150, 0.477; 0.275, 0.311, 0.657)";
                    dimension = 6;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = 0.0D;
                        ub[i] = 1.0D;
                    }
                    break;

                // SHEKEL 4_5 siempre dimension 4
                case tipo_funciones.SHEKEL_4_5:
                    nombre_funcion = "SHEKEL 4 5 ( min= " + (-10.1532 + function_off_set).ToString() + " en 4.00004, 4.00013, 4.00004, 4.00013)";
                    dimension = 4;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = 0.0D;
                        ub[i] = 10.0D;
                    }
                    break;

                // SHEKEL 4 7 siempre dimension 4
                case tipo_funciones.SHEKEL_4_7:
                    nombre_funcion = "SHEKEL 4 7 ( min= " + (-10.403 + function_off_set).ToString() + " en 4.00057,4.00069,3.99949,3.99961)";
                    dimension = 4;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = 0.0D;
                        ub[i] = 10.0D;
                    }
                    break;

                // SHEKEL 4_10 siempre dimension 4
                case tipo_funciones.SHEKEL_4_10:
                    nombre_funcion = "SHEKEL 4 10 ( min= " + (-10.5364 + function_off_set).ToString() + " en 4.00075,4.00059,3.99966,3.99951)";
                    dimension = 4;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = 0.0D;
                        ub[i] = 10.0D;
                    }
                    break;

                // SHUBERT
                case tipo_funciones.SHUBERT:
                    nombre_funcion = "SHUBERT (min= " + (-186.7309 + function_off_set).ToString();
                    dimension = 2;
                    lb[1] = -10.0D;
                    lb[2] = -10.0D;
                    ub[1] = 10.0D;
                    ub[2] = 10.0D;
                    break;

                // WEIERSTRASS 
                case tipo_funciones.WEIERSTRASS:
                    nombre_funcion = "WEIERSTRASS ( min= " + (4.0D + function_off_set).ToString() + " en 0,0,0,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -0.5D;
                        ub[i] = 0.5D;
                    }
                    break;

                // ROTATED WEIERSTRASS 
                case tipo_funciones.ROTATED_WEIERSTRASS:
                    nombre_funcion = "ROTATED WEIERSTRASS ( min= " + (4.0D + function_off_set).ToString() + " en 0,0,0,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -0.5D;
                        ub[i] = 0.5D;
                    }
                    break;

                // MICHALEWICZ d=2
                case tipo_funciones.MICHALEWICZ:
                    //dimension = 2;
                    nombre_funcion = "MICHALEWICZ  (dimension 2 min= " + (-1.80130341009855321D + function_off_set).ToString() + " en 2.20290552014618, 1.57079632677565)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = 0.0D;
                        ub[i] = Math.PI;
                    }
                    break;

                // XIN_SHE_YANG_1 
                case tipo_funciones.XIN_SHE_YANG_1:
                    nombre_funcion = "XIN SHE YANG 1 ( min= " + (-1.0D + function_off_set).ToString() + " en 0,0,0, .... )";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -20.0D;
                        ub[i] = 20.0D;
                    }
                    break;

                // XIN_SHE_YANG_2 
                case tipo_funciones.XIN_SHE_YANG_2:
                    nombre_funcion = "XIN SHE YANG 2 ( min= " + function_off_set.ToString() + " en 0,0,0, .... )";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -10.0D;
                        ub[i] = 10.0D;
                    }
                    break;

                // XIN_SHE_YANG_3
                case tipo_funciones.XIN_SHE_YANG_3:
                    nombre_funcion = "XIN SHE YANG 3 ( min= " + function_off_set.ToString() + " en 0,0,0, .... )";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -PI_x_2;
                        ub[i] = PI_x_2;
                    }
                    break;

                // XIN_SHE_YANG_4
                case tipo_funciones.XIN_SHE_YANG_4:
                    nombre_funcion = "XIN SHE YANG 4 ( min= NA en 3.14,3.14 )";
                    dimension = 2;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = 0.0D;
                        ub[i] = 10.0D;
                    }
                    break;

                // XIN_SHE_YANG_5
                case tipo_funciones.XIN_SHE_YANG_5:
                    nombre_funcion = "XIN SHE YANG 5 ( min= " + function_off_set.ToString() + " en 0,0,0, .... )";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.0D;
                        ub[i] = 5.0D;
                    }
                    break;

                // XIN_SHE_YANG_6
                case tipo_funciones.XIN_SHE_YANG_6:
                    nombre_funcion = "XIN SHE YANG 6 ( min= " + (-1.0D + function_off_set).ToString() + " en 0,0,0, .... )";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -10.0D;
                        ub[i] = 10.0D;
                    }
                    break;

                // XIN_SHE_YANG_7
                case tipo_funciones.XIN_SHE_YANG_7:
                    nombre_funcion = "XIN SHE YANG 7 ( min= " + function_off_set.ToString() + " en 1,0.5,0.3333, 0.25, 1/i .... )";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.0D;
                        ub[i] = 5.0D;
                    }
                    break;

                // XIN_SHE_YANG_8
                case tipo_funciones.XIN_SHE_YANG_8:
                    nombre_funcion = "XIN SHE YANG 8 ( min= " + function_off_set.ToString() + " en 0, 0, 0, .... )";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -10.0D * Math.PI;
                        ub[i] = 10.0D * Math.PI;
                    }
                    break;

                //  BEALE dimension 2
                case tipo_funciones.BEALE:
                    nombre_funcion = "BEALE ( min= " + function_off_set.ToString() + " en 3, 0.5)";
                    dimension = 2;
                    lb[1] = -4.5D;
                    ub[1] = 4.5D;
                    lb[2] = -4.5D;
                    ub[2] = 4.5D;
                    break;

                //  BIRD dimension 2
                case tipo_funciones.BIRD:
                    nombre_funcion = "BIRD ( min= " + (-106.764537D + function_off_set).ToString() + " en (4.70104, 3.15294) y (-1.58214, -3.13024))";
                    dimension = 2;
                    lb[1] = -PI_x_2;
                    ub[1] = PI_x_2;
                    lb[2] = -PI_x_2;
                    ub[2] = PI_x_2;
                    break;

                // BOHACHEVKY 1 dimension 2
                case tipo_funciones.BOHACHEVSKY_1:
                    nombre_funcion = "BOHACHEVSKY 1 ( min= " + function_off_set.ToString() + " en 0,0)";
                    dimension = 2;
                    lb[1] = -100.0D;
                    ub[1] = 100.0D;
                    lb[2] = -100.0D;
                    ub[2] = 100.0D;
                    break;

                // BOHACHEVSKY 2 dimension 2
                case tipo_funciones.BOHACHEVSKY_2:
                    nombre_funcion = "BOHACHEVSKY 2 ( min= " + function_off_set.ToString() + " en 0,0)";
                    dimension = 2;
                    lb[1] = -100.0D;
                    ub[1] = 100.0D;
                    lb[2] = -100.0D;
                    ub[2] = 100.0D;
                    break;

                // BOHACHESVKY 3 dimension 2
                case tipo_funciones.BOHACHEVSKY_3:
                    nombre_funcion = "BOHACHEVSKY 3 ( min= " + function_off_set.ToString() + " en 0,0)";
                    dimension = 2;
                    lb[1] = -100.0D;
                    ub[1] = 100.0D;
                    lb[2] = -100.0D;
                    ub[2] = 100.0D;
                    break;

                // BOOTH dimension 2
                case tipo_funciones.BOOTH:
                    nombre_funcion = "BOOTH  (min= " + function_off_set.ToString() + " en 1,3)";
                    dimension = 2;
                    lb[1] = -10.0D;
                    ub[1] = 10.0D;
                    lb[2] = -10.0D;
                    ub[2] = 10.0D;
                    break;

                // BOX_BETTS dimension 3
                case tipo_funciones.BOX_BETTS:
                    nombre_funcion = "BOX BETTS ( min= " + function_off_set.ToString() + " en 1,10,1)";
                    dimension = 3;
                    lb[1] = -0.25D;
                    lb[2] = 0.01D;
                    lb[3] = 0.01D;
                    ub[1] = 0.25D;
                    ub[2] = 2.5D;
                    ub[3] = 2.5;
                    break;

                // BRAD dimension 3
                case tipo_funciones.BRAD:
                    nombre_funcion = "BRAD ( min= " + (0.00821487 + function_off_set.ToString()) + " en 0.0824,1.133,2.3437)";
                    dimension = 3;
                    lb[1] = 0.9D;
                    lb[2] = 9.0D;
                    lb[3] = 0.9D;
                    ub[1] = 1.2D;
                    ub[2] = 11.2D;
                    ub[3] = 1.2D;
                    break;

                // THREE HUMP CAMEL BACK siempre dimension 2
                case tipo_funciones.THREE_HUMP_CAMEL:
                    nombre_funcion = "THREE HUMP CAMEL BACK ( min= " + function_off_set.ToString() + " en 0,0)";
                    dimension = 2;
                    lb[1] = -5.0D;
                    ub[1] = 5.0D;
                    lb[2] = -5.0D;
                    ub[2] = 5.0D;
                    break;

                // CHICHINADZE siempre dimension 2
                case tipo_funciones.CHICHINADZE:
                    nombre_funcion = "CHICHINADZE ( min= " + (-42.94438701899098D + function_off_set).ToString() + " en 6.189866586965680,0.5)";
                    dimension = 2;
                    lb[1] = -30.0D;
                    ub[1] = 30.0D;
                    lb[2] = -30.0D;
                    ub[2] = 30.0D;
                    break;

                // CHUNG REYNOLDS 
                case tipo_funciones.CHUNG_REYNOLDS:
                    nombre_funcion = "CHUNG REYNOLDS ( min= " + function_off_set.ToString() + " en 0,0,0,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    break;

                // EASOM siempre dimension 2
                case tipo_funciones.EASOM:
                    nombre_funcion = "EASOM ( min= " + (-1.0D + function_off_set).ToString() + " en 3.14, 3.14)";
                    dimension = 2;
                    lb[1] = -10.0D;
                    ub[1] = 10.0D;
                    lb[2] = -10.0D;
                    ub[2] = 10.0D;
                    break;

                // EXP_2
                case tipo_funciones.EXP_2:
                    nombre_funcion = "EXP 2 ( min= " + function_off_set.ToString() + " en 1,0.1)";
                    dimension = 2;
                    lb[1] = 0.0D;
                    ub[1] = 20.0D;
                    lb[2] = 0.0D;
                    ub[2] = 20.0D;
                    break;

                // FREUDENSTEIN_ROTH dimension 2
                case tipo_funciones.FREUDENSTEIN_ROTH:
                    nombre_funcion = "FREUDENSTEIN_ROTH ( min= " + function_off_set.ToString() + " en 5,4)";
                    dimension = 2;
                    lb[1] = -10.0D;
                    lb[2] = -10.0D;
                    ub[1] = 10.0D;
                    ub[2] = 10.0D;
                    break;

                // GIUNTA dimension 2
                case tipo_funciones.GIUNTA:
                    nombre_funcion = "GIUNTA ( min= " + (0.060447042053690566D + function_off_set).ToString() + " en 0.45834282,0.45834282)";
                    dimension = 2;
                    lb[1] = -1.0D;
                    lb[2] = -1.0D;
                    ub[1] = 1.0D;
                    ub[2] = 1.0D;
                    break;

                // EGG_HOLDER
                case tipo_funciones.EGG_HOLDER:
                    nombre_funcion = "EGG HOLDER ( min= " + (-959.6407D + function_off_set).ToString() + " en 512,404.2319)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -512.0D;
                        ub[i] = 512.0D;
                    }
                    break;

                // EL_ATTAR_VIDYASAGAR_DUTTA dimension 2
                case tipo_funciones.EL_ATTAR_VIDYASAGAR_DUTTA:
                    nombre_funcion = "EL_ATTAR_VIDYASAGAR_DUTTA ( min= " + (1.712780354862198 + function_off_set).ToString() + " en 3.40918683,-2.17143304)";
                    dimension = 3;
                    lb[1] = -500.0D;
                    lb[2] = -500.0D;
                    ub[1] = 500.0D;
                    ub[2] = 500.0D;
                    break;

                // EXPONENTIAL
                case tipo_funciones.EXPONENTIAL:
                    nombre_funcion = "EXPONENTIAL ( min= " + (-1 + function_off_set).ToString() + " en xi=0)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -1.0D;
                        ub[i] = 1.0D;
                    }
                    break;

                // EGG_CRATE
                case tipo_funciones.EGG_CRATE:
                    nombre_funcion = "EGG CRATE ( min= " + function_off_set.ToString() + " en 0,0)";
                    dimension = 2;
                    lb[1] = -5.0D;
                    ub[1] = 5.0D;
                    lb[2] = -5.0D;
                    ub[2] = 5.0D;
                    break;

                // TRID
                case tipo_funciones.TRID:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -Math.Pow(dimension, 2.0d);
                        ub[i] = Math.Pow(dimension, 2.0d);
                    }
                    long minimo = dimension * (dimension + 4) * (dimension - 1) / 6;
                    nombre_funcion = "TRID ( min= " + (minimo + function_off_set).ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                    {
                        long posicion = i * (dimension + 1 - i);
                        nombre_funcion += posicion.ToString() + " ";
                    }
                    nombre_funcion += ")";
                    break;

                // GEAR siempre 4
                case tipo_funciones.GEAR:
                    nombre_funcion = "GEAR ( min= " + (2.7E-12 + function_off_set).ToString() + " en 16,19,43,49)";
                    dimension = 4;
                    lb[1] = 12.0D;
                    ub[1] = 60.0D;
                    lb[2] = 12.0D;
                    ub[2] = 60.0D;
                    lb[3] = 12.0D;
                    ub[3] = 60.0D;
                    lb[4] = 12.0D;
                    ub[4] = 60.0D;
                    break;

                // EXP2 siempre 2
                case tipo_funciones.BIGGS_EXP2:
                    nombre_funcion = "BIGGS EXP 2 ( min= " + function_off_set.ToString() + " en 1,10)";
                    dimension = 2;
                    lb[1] = 0.0D;
                    ub[1] = 20.0D;
                    lb[2] = 0.0D;
                    ub[2] = 20.0D;
                    break;

                // EXP3 siempre 3
                case tipo_funciones.BIGGS_EXP3:
                    nombre_funcion = "BIGGS EXP 3 ( min= " + function_off_set.ToString() + " en 1,10,5)";
                    dimension = 3;
                    lb[1] = 0.0D;
                    ub[1] = 20.0D;
                    lb[2] = 0.0D;
                    ub[2] = 20.0D;
                    lb[3] = 0.0D;
                    ub[3] = 20.0D;
                    break;

                // BIGGS EXP4 siempre 4
                case tipo_funciones.BIGGS_EXP4:
                    nombre_funcion = "BIGGS EXP 4 ( min= " + function_off_set.ToString() + " en 1,10,1,5)";
                    dimension = 4;
                    lb[1] = 0.0D;
                    ub[1] = 20.0D;
                    lb[2] = 0.0D;
                    ub[2] = 20.0D;
                    lb[3] = 0.0D;
                    ub[3] = 20.0D;
                    lb[4] = 0.0D;
                    ub[4] = 20.0D;
                    break;

                // BIGGS EXP5 siempre 5
                case tipo_funciones.BIGGS_EXP5:
                    nombre_funcion = "BIGGS EXP 5 ( min= " + function_off_set.ToString() + " en 1,10,1,5,4)";
                    dimension = 5;
                    lb[1] = 0.0D;
                    ub[1] = 20.0D;
                    lb[2] = 0.0D;
                    ub[2] = 20.0D;
                    lb[3] = 0.0D;
                    ub[3] = 20.0D;
                    lb[4] = 0.0D;
                    ub[4] = 20.0D;
                    lb[5] = 0.0D;
                    ub[5] = 20.0D;
                    break;

                // BIGGS_EXP6 siempre 6
                case tipo_funciones.BIGGS_EXP6:
                    nombre_funcion = "BIGGS EXP 6 ( min= " + function_off_set.ToString() + " en 1,10,1,5,4,?)";
                    dimension = 6;
                    lb[1] = 0.0D;
                    ub[1] = 20.0D;
                    lb[2] = 0.0D;
                    ub[2] = 20.0D;
                    lb[3] = 0.0D;
                    ub[3] = 20.0D;
                    lb[4] = 0.0D;
                    ub[4] = 20.0D;
                    lb[5] = 0.0D;
                    ub[5] = 20.0D;
                    lb[6] = 0.0D;
                    ub[6] = 20.0D;
                    break;

                // COLVILLE siempre 4
                case tipo_funciones.COLVILLE:
                    nombre_funcion = "COLVILLE ( min= " + function_off_set.ToString() + " en 1,1,1,1)";
                    dimension = 4;
                    lb[1] = -10.0D;
                    ub[1] = 10.0D;
                    lb[2] = -10.0D;
                    ub[2] = 10.0D;
                    lb[3] = -10.0D;
                    ub[3] = 10.0D;
                    lb[4] = -10.0D;
                    ub[4] = 10.0D;
                    break;

                // CORANA siempre 4
                case tipo_funciones.CORANA:
                    nombre_funcion = "CORANA ( min= " + function_off_set.ToString() + " en 0,0,0,0)";
                    dimension = 4;
                    lb[1] = -100.0D;
                    ub[1] = 100.0D;
                    lb[2] = -100.0D;
                    ub[2] = 100.0D;
                    lb[3] = -100.0D;
                    ub[3] = 100.0D;
                    lb[4] = -100.0D;
                    ub[4] = 100.0D;
                    break;

                // COSINE MIXTURE 2 siempre 2
                case tipo_funciones.COSINE_MIXTURE_2:
                    nombre_funcion = "COSINE MIXTURE  2 ( min= " + function_off_set.ToString() + " en 0.2, 0.2)";
                    dimension = 2;
                    lb[1] = -1.0D;
                    ub[1] = 1.0D;
                    lb[2] = -1.0D;
                    ub[2] = 1.0D;
                    break;

                // COSINE MIXTURE 4 siempre 4
                case tipo_funciones.COSINE_MIXTURE_4:
                    nombre_funcion = "COSINE MIXTURE  4 ( min= " + function_off_set.ToString() + " en 0.4, 0.4)";
                    dimension = 4;
                    lb[1] = -1.0D;
                    ub[1] = 1.0D;
                    lb[2] = -1.0D;
                    ub[2] = 1.0D;
                    lb[3] = -1.0D;
                    ub[3] = 1.0D;
                    lb[4] = -1.0D;
                    ub[4] = 1.0D;
                    break;

                //CROSS_IN_TRAY dimension 2
                case tipo_funciones.CROSS_IN_TRAY:
                    nombre_funcion = "CROSS_IN_TRAY ( min= " + (- 2.06261218 + function_off_set).ToString() + " en +-1.349406685353340, +-1.349406608602084)";
                    dimension = 2;
                    lb[1] = -10.0D;
                    ub[1] = 10.0D;
                    lb[2] = -10.0D;
                    ub[2] = 10.0D;
                    break;

                // CSENDES
                case tipo_funciones.CSENDES:
                    nombre_funcion = "CSENDES ( min= " + function_off_set.ToString() + " en xi=0)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -1.0D;
                        ub[i] = 1.0D;
                    }
                    break;

                // CUBE dimension 2
                case tipo_funciones.CUBE:
                    nombre_funcion = "CUBE ( min= " + function_off_set.ToString() + " en 1,1)";
                    dimension = 2;
                    lb[1] = -10.0D;
                    lb[2] = -10.0D;
                    ub[1] = 10.0D;
                    ub[2] = 10.0D;
                    break;

                // DAMAVANDI dimension 2
                case tipo_funciones.DAMAVANDI:
                    nombre_funcion = "DAMAVANDI ( min= " + function_off_set.ToString() + " en 2,2)";
                    dimension = 2;
                    lb[1] = 0.0D;
                    lb[2] = 0.0D;
                    ub[1] = 14.0D;
                    ub[2] = 14.0D;
                    break;

                // DEB_1
                case tipo_funciones.DEB_1:
                    nombre_funcion = "DEB_1 ( min= " + function_off_set.ToString() + " en ?)"; //REVISAR - Tiene varios mínimos pero no sé en dónde
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -1.0D;                        
                        ub[i] = 1.0D;
                    }
                    break;

                // DEB_3
                case tipo_funciones.DEB_3:
                    nombre_funcion = "DEB_3 ( min= " + function_off_set.ToString() + " en ?)"; //REVISAR - Tiene varios mínimos
                    dimension = 3;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -1.0D;
                        ub[i] = 1.0D;
                    }
                    break;

                // DECKKERS_AARTS dimension 2
                case tipo_funciones.DECKKERS_AARTS:
                    nombre_funcion = "DECKKERS_AARTS ( min= " + (-24777 + function_off_set).ToString() + " en 0,+-15)";
                    dimension = 2;
                    lb[1] = -20.0D;
                    lb[2] = -20.0D;
                    ub[1] = 20.0D;
                    ub[2] = 20.0D;
                    break;

                // DE_VILLIERS_GLASSER_1 dimension 4
                case tipo_funciones.DE_VILLIERS_GLASSER_1:
                    nombre_funcion = "DE_VILLIERS_GLASSER_1 ( min= " + function_off_set.ToString() + " en ?)"; //REVISAR
                    dimension = 4;
                    lb[1] = 1.0D;
                    lb[2] = 1.0D;
                    lb[3] = 1.0D;
                    lb[4] = 1.0D;
                    ub[1] = 100.0D;
                    ub[2] = 100.0D;
                    ub[3] = 100.0D;
                    ub[4] = 100.0D;
                    break;

                // DE_VILLIERS_GLASSER_2 dimension 4
                case tipo_funciones.DE_VILLIERS_GLASSER_2:
                    nombre_funcion = "DE_VILLIERS_GLASSER_2 ( min= " + function_off_set.ToString() + " en ?)"; //REVISAR
                    dimension = 5;
                    lb[1] = 1.0D;
                    lb[2] = 1.0D;
                    lb[3] = 1.0D;
                    lb[4] = 1.0D;
                    lb[5] = 0.0D;
                    ub[1] = 60.0D;
                    ub[2] = 5.0D;
                    ub[3] = 5.0D;
                    ub[4] = 5.0D;
                    ub[5] = 1.0D;
                    break;

                // DIXON_PRICE
                case tipo_funciones.DIXON_PRICE:
                    nombre_funcion = "DIXON_PRICE ( min= " + function_off_set.ToString() + " xi=2^((2^i - 2)/2^i))";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -10.0D;
                        ub[i] = 10.0D;
                    }
                    break;

                // DOLAN dimension 5
                case tipo_funciones.DOLAN:
                    nombre_funcion = "DOLAN ( min= " + function_off_set.ToString() + " en ?)"; //REVISAR
                    dimension = 5;
                    lb[1] = -100.0D;
                    lb[2] = -100.0D;
                    lb[3] = -100.0D;
                    lb[4] = -100.0D;
                    lb[5] = -100.0D;
                    ub[1] = 100.0D;
                    ub[2] = 100.0D;
                    ub[3] = 100.0D;
                    ub[4] = 100.0D;
                    ub[5] = 100.0D;
                    break;

                // HANSEN siempre 2
                case tipo_funciones.HANSEN:
                    nombre_funcion = "HANSEN ( min= " + (-176.54D + function_off_set).ToString() + " en (-7.589893,-7.708314), (-7.589893,-1.425128), (-7.589893, 4.858057), (-1.306708, -7.708314), (-1.306708, 4.858057), (4.976478, 4.858057), (-4.976478,-1.425128), (4.976478,-7.708314))";
                    dimension = 2;
                    lb[1] = -10.0D;
                    ub[1] = 10.0D;
                    lb[2] = -10.0D;
                    ub[2] = 10.0D;
                    break;

                // HIMMELBLAU siempre 2
                case tipo_funciones.HIMMELBLAU:
                    nombre_funcion = "HIMMELBLAU ( min= " + function_off_set.ToString() + " en (3,2),(3.584428340330,-1.848126526964),(-3.779310253378,-3.283185991286),(-2.805118086953,3.131312518250))";
                    dimension = 2;
                    lb[1] = -6.0D;
                    ub[1] = 6.0D;
                    lb[2] = -6.0D;
                    ub[2] = 6.0D;
                    break;

                //HOLZMAN_1 siempre 3
                case tipo_funciones.HOLZMAN_1:
                    nombre_funcion = "HOLZMAN 1 ( min= " + function_off_set.ToString() + " en 50,25,1.5)";
                    lb[1] = 0.0D;
                    lb[2] = 0.0D;
                    lb[3] = 0.0D;
                    ub[1] = 100.0D;
                    ub[2] = 25.6D;
                    ub[3] = 5.0D;
                    dimension = 3;
                    break;

                //LAGERMAN siempre 2
                case tipo_funciones.LAGERMAN:
                    nombre_funcion = "LAGERMAN ( min= " + (-5.1621259D + function_off_set).ToString() + " en 2.00299219, 1.006096)";
                    lb[1] = 0.0D;
                    lb[2] = 0.0D;
                    ub[1] = 10.0D;
                    ub[2] = 10.0D;
                    dimension = 2;
                    break;

                //LEON siempre 2
                case tipo_funciones.LEON:
                    nombre_funcion = "LEON ( min= " + function_off_set.ToString() + " en 1,1)";
                    lb[1] = -10.0D;
                    lb[2] = -10.0D;
                    ub[1] = 10.0D;
                    ub[2] = 10.0D;
                    dimension = 2;
                    break;

                //LEVY_8
                case tipo_funciones.LEVY_8:
                    nombre_funcion = "LEVY_8 ( min= " + function_off_set.ToString() + " en 1,1,1,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -10.0D;
                        ub[i] = 10.0D;
                    }
                    break;

                //MATYAS siempre 2
                case tipo_funciones.MATYAS:
                    nombre_funcion = "MATYAS ( min= " + function_off_set.ToString() + " en 0,0)";
                    lb[1] = -10.0D;
                    lb[2] = -10.0D;
                    ub[1] = 10.0D;
                    ub[2] = 10.0D;
                    dimension = 2;
                    break;

                //MC_CORMICK siempre 2
                case tipo_funciones.MC_CORMICK:
                    nombre_funcion = "MC_CORMICK ( min= " + (-1.9133D + function_off_set).ToString() + " en -0.54719,-1.54719)";
                    lb[1] = -1.5D;
                    lb[2] = -3.0D;
                    ub[1] = 4.0D;
                    ub[2] = 4.0D;
                    dimension = 2;
                    break;

                //NEUMAIER_PERM
                case tipo_funciones.NEUMAIER_PERM:
                    nombre_funcion = "NEUMAIER_PERM ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -dimension;
                        ub[i] = dimension;
                    }
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += i.ToString() + " ";
                    nombre_funcion += ")";
                    break;

                //NEUMAIER_PERM_0
                case tipo_funciones.NEUMAIER_PERM_0:
                    nombre_funcion = "NEUMAIER_PERM_0 ( min= " + function_off_set.ToString() + " en ";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -1.0D;
                        ub[i] = 1.0D;
                    }
                    for (int i = 1; i <= dimension; i++)
                        nombre_funcion += (1 / i).ToString() + " ";
                    nombre_funcion += ")";
                    break;

                //PARSOPOULOS siempre 2
                case tipo_funciones.PARSOPOULOS:
                    nombre_funcion = "PARSOPOULOS ( min= " + function_off_set.ToString() + " en(k PI/2, lambda PI) donde k= +-1,+-3,... y lambda=0,+-1,+-2,...";
                    dimension = 2;
                    lb[1] = -5.0D;
                    lb[2] = -5.0D;
                    ub[1] = 5.0D;
                    ub[2] = 5.0D;
                    break;

                //PATHOLOGICAL
                case tipo_funciones.PATHOLOGICAL:
                    nombre_funcion = "PATHOLOGICAL ( min= " + function_off_set.ToString() + "en xi =0)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -100.0D;
                        ub[i] = 100.0D;
                    }
                    break;

                //PAVIANI siempre 10
                case tipo_funciones.PAVIANI:
                    nombre_funcion = "PAVIANI ( min= " + (-45.7784684040686D + function_off_set).ToString() + " en xi=9.350266)";
                    dimension = 10;
                    lb[1] = 2.001D;
                    ub[1] = 9.999D;
                    lb[2] = 2.001D;
                    ub[2] = 9.999D;
                    lb[3] = 2.001D;
                    ub[3] = 9.999D;
                    lb[4] = 2.001D;
                    ub[4] = 9.999D;
                    lb[5] = 2.001D;
                    ub[5] = 9.999D;
                    lb[6] = 2.001D;
                    ub[6] = 9.999D;
                    lb[7] = 2.001D;
                    ub[7] = 9.999D;
                    lb[8] = 2.001D;
                    ub[8] = 9.999D;
                    lb[9] = 2.001D;
                    ub[9] = 9.999D;
                    lb[10] = 2.001D;
                    ub[10] = 9.999D;
                    break;

                //PEN_HOLDER siempre 2
                case tipo_funciones.PEN_HOLDER:
                    nombre_funcion = "PEN_HOLDER (min= " + (-0.9635348327265058D + function_off_set).ToString() + " en xi= +-9.646167671043401)";
                    dimension = 2;
                    lb[1] = -11.0D;
                    lb[2] = -11.0D;
                    ub[1] = 11.0D;
                    ub[2] = 11.0D;
                    break;

                // PINTER
                case tipo_funciones.PINTER:
                    nombre_funcion = "PINTER ( min= " + function_off_set.ToString() + " en 3,−1, 0, 1, ... , 3,−1, 0, 1)";
                    for (int i = 1; i < dimension; i++)
                    {
                        lb[i] = -10;
                        ub[i] = 10;
                    }
                    break;

                // POWELL
                case tipo_funciones.POWELL:
                    nombre_funcion = "POWELL ( min= " + function_off_set.ToString() + " en xi=0)";
                    // La dimensión debe ser múltiplo de 4
                    dimension = Convert.ToInt32(Math.Floor(Convert.ToDouble(dimension) / 4.0D));
                    if (dimension < 4) dimension = 4;
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -4.0D;
                        ub[i] = 5.0D;
                    }
                    break;

                // POWELL_SINGULAR_2
                case tipo_funciones.POWELL_SINGULAR_2:
                    nombre_funcion = "POWELL_SINGULAR_2 ( min= " + function_off_set.ToString() + " en ?)";//REVISAR
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -4.0D;
                        ub[i] = 5.0D;
                    }
                    break;

                // POWELL_SUM
                case tipo_funciones.POWELL_SUM:
                    nombre_funcion = "POWELL_SUM ( min= " + function_off_set.ToString() + " en xi=0)"; //REVISAR. El mínimo lo deduje
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -1.0D;
                        ub[i] = 1.0D;
                    }
                    break;

                // PRICE_1 dimension 2
                case tipo_funciones.PRICE_1:
                    nombre_funcion = "PRICE_1 ( min= " + function_off_set.ToString() + " en +-5,+-5)";
                    dimension = 2;
                    lb[1] = -500.0D;
                    lb[2] = -500.0D;
                    ub[1] = 500.0D;
                    ub[2] = 500.0D;
                    break;

                // PRICE_2 dimension 2
                case tipo_funciones.PRICE_2:
                    nombre_funcion = "PRICE_2 ( min= " + (0.9D + function_off_set).ToString() + " en 0,0)";
                    dimension = 2;
                    lb[1] = -10.0D;
                    lb[2] = -10.0D;
                    ub[1] = 10.0D;
                    ub[2] = 10.0D;
                    break;

                // PRICE_4 dimension 2
                case tipo_funciones.PRICE_4:
                    nombre_funcion = "PRICE_4 ( min= " + function_off_set.ToString() + " en {0, 0},{2, 4},{ 1.464,−2.506})";
                    dimension = 2;
                    lb[1] = -5.0D;
                    lb[2] = -5.0D;
                    ub[1] = 5.0D;
                    ub[2] = 5.0D;
                    break;

                // QING
                case tipo_funciones.QING:
                    nombre_funcion = "QING ( min= " + (-3873.7243 + function_off_set).ToString() + " en +-raiz(i))";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -500.0D;
                        ub[i] = 500.0D;
                    }
                    break;

                // QUADRATIC dimension 2
                case tipo_funciones.QUADRATIC:
                    nombre_funcion = "QUADRATIC ( min= " + function_off_set.ToString() + " en 0.19388, 0.48513)";
                    dimension = 2;
                    lb[1] = -10.0D;
                    lb[2] = -10.0D;
                    ub[1] = 10.0D;
                    ub[2] = 10.0D;
                    break;


                //HOSAKI siempre 2
                case tipo_funciones.HOSAKI:
                    nombre_funcion = "HOSAKI 1 (min= " + (-2.35458D + function_off_set).ToString() + " en 4, 2)";
                    lb[1] = 0.0D;
                    lb[2] = 0.0D;
                    ub[1] = 10.0D;
                    ub[2] = 10.0D;
                    dimension = 2;
                    break;

                //KATSUURA 
                case tipo_funciones.KATSUURAS:
                    nombre_funcion = "KATSUURAS (min= " + (1.0D + function_off_set).ToString() + " en 0,0,0....)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = 0.0D;
                        ub[i] = 100.0D;
                    }
                    break;

                //HOLZMAN_2 
                case tipo_funciones.HOLZMAN_2:
                    nombre_funcion = "HOLZMAN 2 (min= " + (function_off_set).ToString() + " en 0,0,0,...)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -10.0D;
                        ub[i] = 10.0D;
                    }
                    break;

                // COMPUESTA 1 
                case tipo_funciones.CEC_2005_CF1:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.0D;
                        ub[i] = 5.0D;
                    }
                    nombre_funcion = "CEC_2005_CF1 ( min= " + function_off_set.ToString() + " en 0,0,0...)";
                    break;
                
                // COMPUESTA 2 
                case tipo_funciones.CEC_2005_CF2:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.0D;
                        ub[i] = 5.0D;
                    }
                    nombre_funcion = "CEC_2005_CF2 ( min= " + function_off_set.ToString() + " en 0,0,0...)";
                    break;
                
                // COMPUESTA 3 
                case tipo_funciones.CEC_2005_CF3:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.0D;
                        ub[i] = 5.0D;
                    }
                    nombre_funcion = "CEC_2005_CF3 ( min= " + function_off_set.ToString() + " en 0,0,0...)";
                    break;
                
                // COMPUESTA 4 
                case tipo_funciones.CEC_2005_CF4:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.0D;
                        ub[i] = 5.0D;
                    }
                    nombre_funcion = "CEC_2005_CF4 ( min= " + function_off_set.ToString() + " en 0,0,0...)";
                    break;
                
                // COMPUESTA 5 
                case tipo_funciones.CEC_2005_CF5:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.0D;
                        ub[i] = 5.0D;
                    }
                    nombre_funcion = "CEC_2005_CF5 ( min= " + function_off_set.ToString() + " en 0,0,0...)";
                    break;
                
                // COMPUESTA 6 
                case tipo_funciones.CEC_2005_CF6:
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.0D;
                        ub[i] = 5.0D;
                    }
                    nombre_funcion = "CEC_2005_CF6 ( min= " + function_off_set.ToString() + " en 0,0,0...)";
                    break;

                // ZIRILLI 
                case tipo_funciones.ZIRILLI:
                    nombre_funcion = "ZIRILLI (min= " + (-0.3523 + function_off_set).ToString() + " en -1.0465, 0)";
                    dimension = 2;
                    lb[1] = -10.0D;
                    ub[1] = 10.0D;
                    lb[2] = -10.0D;
                    ub[2] = 10.0D;
                    break;

                // ZETTL
                case tipo_funciones.ZETTL:
                    nombre_funcion = "ZETTL (min= " + (-0.0037912 + function_off_set).ToString() + " en -0.029896, 0)";
                    dimension = 2;
                    lb[1] = -1.0D;
                    ub[1] = 5.0D;
                    lb[2] = -1.0D;
                    ub[2] = 5.0D;
                    break;

                // ZEROSUM 
                case tipo_funciones.ZEROSUM:
                    nombre_funcion = "ZEROSUM (min= " + function_off_set.ToString() + " SI SUMA Xi = 0)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -10.0D;
                        ub[i] = 10.0D;
                    }
                    break;

                // ZACHAROV 
                case tipo_funciones.ZACHAROV:
                    nombre_funcion = "ZACHAROV (min= " + function_off_set.ToString() + " en 0, 0, 0, ....)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.0D;
                        ub[i] = 10.0D;
                    }
                    break;

                // PROBLEMA_TENSION_COMPRESSION_SPRING 
                /*case tipo_funciones.PROBLEMA_TENSION_COMPRESSION_SPRING:
                    nombre_funcion = "PROBLEMA TENSION COMPRESSION SPRING (min=0.0126740 d=0.051144 D=0.343751 N=12.0955 )";
                    lb[1] = 0.05D;
                    ub[1] = 2.0D;
                    lb[2] = 0.25D;
                    ub[2] = 1.3D;
                    lb[3] = 2.0D;
                    ub[3] = 15.0D;
                    dimension = 3;
                    break;*/

                // FUNCION_RESTRICCIONES_G1_CEC_2006
                /*case tipo_funciones.FUNCION_RESTRICCIONES_G1_CEC_2006:
                    nombre_funcion = "FUNCION_RESTRICCIONES_G1_CEC_2006 (min=-15 en 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 3, 3, 1)";
                    for (int i = 1; i <= 9; i++)
                    {
                        lb[i] = 0.99D;
                        ub[i] = 1.01D;
                    }
                    for (int i = 10; i <= 12; i++)
                    {
                        lb[i] = 2.99D;
                        ub[i] = 3.01D;
                    }
                    lb[13] = 0.99D;
                    ub[13] = 1.01D;
                    dimension = 13;
                    break;*/

                // YAOLIU09 
                case tipo_funciones.YAOLIU09:
                    nombre_funcion = "YAOLIU09 (min= " + function_off_set.ToString() + " en 0, 0, 0, ....)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -5.12D;
                        ub[i] = 5.12D;
                    }
                    break;

                // YAOLIU04
                case tipo_funciones.YAOLIU04:
                    nombre_funcion = "YAOLIU04 (min= " + function_off_set.ToString() + " en 0, 0, 0, ....)";
                    for (int i = 1; i <= dimension; i++)
                    {
                        lb[i] = -10.0D;
                        ub[i] = 10.0D;
                    }
                    break;



                default:
                    return (false);
            }
            return (true);
        }

        //FUNCIONES

        void shiftfunc(double [] x, ref double [] xshift, int numero_funcion, int dimension)
        {
            for (int i = 1; i <= dimension; i++)
                xshift[i] = x[i] - oinew_CEC_2013[numero_funcion, i];
        }

        void multiplica_vector_por_matriz_CEC_2005(int numero_funcion, int dimension, double[] x, ref double[] xrot)
        {
            for (int i = 1; i <= dimension; i++)
            {
                xrot[i] = 0.0D;
                for (int j = 1; j <= dimension; j++)
                    xrot[i] += x[j] * orthogonal_matrix_M_CEC_2005[numero_funcion, i, j];
            }
        }

        void rotatefunc(double[] x, ref double[] xrot, int numero_funcion, int dimension)
        {
            for (int i = 1; i <= dimension; i++)
            {
                xrot[i] = 0.0D;
                for (int j = 1; j <= dimension; j++)
                    xrot[i] += x[j] * orthogonal_matrix_M_CEC_2013[numero_funcion, i, j];
            }
        }

        void asyfunc(double [] x, double [] original, ref double[] xasy, int dimension, double beta)
        {
            for (int i = 1; i <= dimension; i++)
                if (x[i] > 0)
                    xasy[i] = Math.Pow(x[i], 1.0D + beta * (i - 1) / (dimension - 1) * Math.Sqrt(x[i]));
                else
                    xasy[i] = original[i];
                    
        }

        void oszfunc(double [] x, ref double [] xosz, int dimension)
        {
            double c1, c2, xx;
            for (int i = 1; i <= dimension; i++)
            {
                if (i == 1 || i == dimension)
                {
                    if (x[i] != 0)
                        xx = Math.Log(Math.Abs(x[i]));
                    else
                        xx = 0.0D;
                    if (x[i] > 0)
                    {
                        c1 = 10.0D;
                        c2 = 7.9D;
                    }
                    else
                    {
                        c1 = 5.5D;
                        c2 = 3.1D;
                    }
                    xosz[i] = Math.Sign(x[i]) * Math.Exp(xx + 0.049D * (Math.Sin(c1 * xx) + Math.Sin(c2 * xx)));
                }
                else
                    xosz[i] = x[i];
            }
        }


        public double cf_cal(double [] x, int dimension, double [] sigma, double [] bias, double [] fit, int numero_de_funciones)
        {
            double [] w = new double[MAX_FUNCIONES_COMPUESTAS];
            double w_max = 0.0D;
            double w_sum = 0.0D;
            double f = 0.0D;
            for (int i = 1; i <= numero_de_funciones; i++)
            {
                fit[i] += bias[i];
                w[i] = 0;
                for (int j = 1; j <= dimension; j++)
                    w[i] += Math.Pow(x[j] - oinew_CEC_2013[i, j], 2.0);
                if (w[i] != 0)
                    w[i] = Math.Pow(1.0D / w[i], 0.5D) * Math.Exp(-w[i] / 2.0D / dimension / Math.Pow(sigma[i], 2.0D));
                else
                    w[i] = INF;
                if (w[i] > w_max)
                    w_max = w[i];
            }
            for (int i = 1; i <= numero_de_funciones; i++)
                w_sum += w[i];
            if (w_max == 0)
            {
                for (int i = 1; i <= numero_de_funciones; i++)
                    w[i] = 1;
                w_sum = numero_de_funciones;
            }
            for (int i = 1; i <= numero_de_funciones; i++)
                f += w[i] / w_sum * fit[i];
            return (f);
        }

        public double CEC_2013_1_SPHERE(double[] position, int dimensions, int numero_funcion)
        {
            double[] y = new double[MAX_DIMENSIONS];
            double result = 0.0D;
            shiftfunc(position, ref y, numero_funcion, dimensions);
            for(int i = 1; i <= dimensions; i++)
                result += (y[i] * y[i]);
            return(result);
        }

        public double CEC_2013_2_ROTATED_HIGH_CONDITIONED_ELLIPTIC(double[] position, int dimensions, int numero_funcion)
        {
            double[] x = new double[MAX_DIMENSIONS];
            double[] z = new double[MAX_DIMENSIONS];
            double[] y = new double[MAX_DIMENSIONS];
            double result = 0.0D;
            shiftfunc(position, ref x, numero_funcion, dimensions);
            rotatefunc(x, ref z, numero_funcion, dimensions);
            oszfunc(z, ref y, dimensions);
            for(int i = 1; i <= dimensions; i++)
                result += Math.Pow(10.0D, 6.0D * (i - 1) / (dimensions - 1)) * (y[i] * y[i]);
            return(result);
        }

        public double CEC_2013_3_ROTATED_BENT_CIGAR(double[] position, int dimensions, int numero_funcion)
        {
            double[] x = new double[MAX_DIMENSIONS];
            double[] z = new double[MAX_DIMENSIONS];
            double[] w = new double[MAX_DIMENSIONS];
            double[] y = new double[MAX_DIMENSIONS];
            double result;
            shiftfunc(position, ref x, numero_funcion, dimensions);
            rotatefunc(x, ref y, numero_funcion, dimensions);
            asyfunc(y, x, ref w, dimensions, 0.5D);
            rotatefunc(w, ref z, numero_funcion + 1, dimensions);
            result = z[1] * z[1];
            for(int i = 2; i <= dimensions; i++)
                result += Math.Pow(10,6) * z[i] * z[i];
            return(result);
        }

        public double CEC_2013_4_ROTATED_DISCUS(double[] position, int dimensions, int numero_funcion)
        {
            double[] x = new double[MAX_DIMENSIONS];
            double[] z = new double[MAX_DIMENSIONS];
            double[] y = new double[MAX_DIMENSIONS];
            double result;
            shiftfunc(position, ref x, numero_funcion, dimensions);
            rotatefunc(x, ref z, numero_funcion, dimensions);
            oszfunc(z, ref y, dimensions);
            result = Math.Pow(10, 6) * y[1] * y[1];
            for(int i = 2; i <= dimensions; i++)
                result += (y[i] * y[i]);
            return(result);
        }

        public double CEC_2013_5_DIFFERENT_POWERS(double[] position, int dimensions, int numero_funcion)
        {
            double[] z = new double[MAX_DIMENSIONS];
            double result = 0.0D;
            shiftfunc(position, ref z, numero_funcion, dimensions);
            for(int i = 1; i <= dimensions; i++)
                result += Math.Pow(Math.Abs(z[i]), 2 + 4 * (i - 1) / (dimensions - 1));
            return(Math.Sqrt(result));
        }

        public double CEC_2013_5A_ROTATED_DIFFERENT_POWERS(double[] position, int dimensions, int numero_funcion)
        {
            double[] z = new double[MAX_DIMENSIONS];
            double[] x = new double[MAX_DIMENSIONS];
            double result = 0.0D;
            shiftfunc(position, ref x, numero_funcion, dimensions);
            rotatefunc(x, ref z, 1, dimensions);
            for (int i = 1; i <= dimensions; i++)
                result += Math.Pow(Math.Abs(z[i]), 2 + 4 * (i - 1) / (dimensions - 1));
            return (Math.Sqrt(result));
        }

        public double CEC_2013_6_ROTATED_ROSENBROCK(double[] position, int dimensions, int numero_funcion)
        {
            double[] x = new double[MAX_DIMENSIONS];
            double[] z = new double[MAX_DIMENSIONS];
            double tmp1, tmp2;
            double result = 0.0D;
            shiftfunc(position, ref x, numero_funcion, dimensions);
            for(int i = 1; i <= dimensions; i++)
                x[i] *= 0.02048D;
            rotatefunc(x, ref z, numero_funcion, dimensions);
            for(int i = 1; i <= dimensions; i++)
                z[i] += 1.0D;
            for(int i = 1; i <= dimensions - 1; i++)
            {
                tmp1 = 100.0D * Math.Pow(z[i] * z[i] - z[i + 1], 2.0);
                tmp2 = Math.Pow(z[i] - 1.0D, 2.0D);
                result += tmp1 + tmp2;
            }
            return(result);
        }

        public double CEC_2013_7_ROTATED_SCHAFFER_F7(double[] position, int dimensions, int numero_funcion)
        {
            double tmp;
            double[] x = new double[MAX_DIMENSIONS];
            double[] z = new double[MAX_DIMENSIONS];
            double[] w = new double[MAX_DIMENSIONS];
            double[] y = new double[MAX_DIMENSIONS];
            double[] t = new double[MAX_DIMENSIONS];
            double result = 0.0D;
            shiftfunc(position, ref x, numero_funcion, dimensions);
            rotatefunc(x, ref y, numero_funcion, dimensions);
            asyfunc(y, x, ref w, dimensions, 0.5D);
            for (int i = 1; i <= dimensions; i++)
                w[i] *= Math.Pow(10.0D, 0.5D * (i - 1) / (dimensions - 1));
            rotatefunc(w, ref t, numero_funcion + 1, dimensions);
            for(int i = 1; i <= dimensions - 1; i++)
                z[i] = Math.Sqrt(t[i] * t[i] + t[i + 1] * t[i + 1]);
            for(int i = 1; i <= dimensions - 1; i++)
            {
                tmp = Math.Sin(50.0 * Math.Pow(z[i], 0.2D));
                result += Math.Sqrt(z[i]) + Math.Sqrt(z[i]) * tmp * tmp;
            }
            result = result * result / (dimensions - 1) / (dimensions - 1);
            return(result);
        }

        public double CEC_2013_8_ROTATED_ACKLEY(double[] position, int dimensions, int numero_funcion)
        {
            double sum1 = 0.0D;
            double sum2 = 0.0D;
            double[] x = new double[MAX_DIMENSIONS];
            double[] z = new double[MAX_DIMENSIONS];
            double[] w = new double[MAX_DIMENSIONS];
            double[] y = new double[MAX_DIMENSIONS];
            double result = 0.0D;
            shiftfunc(position, ref x, numero_funcion, dimensions);
            rotatefunc(x, ref z, numero_funcion, dimensions);
            asyfunc(z, x, ref w, dimensions, 0.5D);
            for (int i = 1; i <= dimensions; i++)
                w[i] *= Math.Pow(10.0D, 0.5D * (i - 1) / (dimensions - 1));
            rotatefunc(w, ref y, numero_funcion + 1, dimensions);
            for(int i = 1; i <= dimensions; i++)
            {
                sum1 += y[i] * y[i];
                sum2 += Math.Cos(PI_x_2 * y[i]);
            }
            sum1 = -0.2 * Math.Sqrt(sum1 / dimensions);
            sum2 /= dimensions;
            result += Math.E - 20.0 * Math.Exp(sum1) - Math.Exp(sum2) + 20.0;
            return(result);
        }


        public double CEC_2013_9_ROTATED_WEIERSTRASS(double[] position, int dimensions, int numero_funcion)
        {
            const int k_max = 20;
            double sum;
            double sum2 = 0.0D;
            const double a = 0.5D;
            const double b = 3.0D;
            
            double[] z = new double[MAX_DIMENSIONS];
            double[] w = new double[MAX_DIMENSIONS];
            double[] y = new double[MAX_DIMENSIONS];
            double[] s = new double[MAX_DIMENSIONS];
            double result = 0.0D;
            shiftfunc(position, ref z, numero_funcion, dimensions);
            for(int i = 1; i <= dimensions; i++)
                 z[i] *= 0.005D;
            rotatefunc(z, ref w, numero_funcion, dimensions);
            asyfunc(w, z, ref s, dimensions, 0.5D);
            for (int i = 1; i <= dimensions; i++)
                s[i] *= Math.Pow(10.0D, 0.5D * (i - 1) / (dimensions - 1));
            rotatefunc(s, ref y, numero_funcion + 1, dimensions);
            for(int i = 1; i <= dimensions; i++)
            {
                sum = 0.0;
                sum2 = 0.0;
                for(int j = 0; j <= k_max; j++)
                {
                    sum += Math.Pow(a, j) * Math.Cos(PI_x_2 * Math.Pow(b, j) * (y[i] + 0.5));
                    sum2 += Math.Pow(a, j) * Math.Cos(PI_x_2 * Math.Pow(b, j) * 0.5);
                }
                result += sum;
            }
            result -= dimensions * sum2;
            return(result);
        }


        public double CEC_2013_10_ROTATED_GRIEWANK(double[] position, int dimensions, int numero_funcion)
        {
            double s = 0.0D;
            double p = 1.0D;
            double[] z = new double[MAX_DIMENSIONS];
            double[] y = new double[MAX_DIMENSIONS];
            double result = 0.0D;
            shiftfunc(position, ref y, numero_funcion, dimensions);
            for(int i = 1; i <= dimensions; i++)
                y[i] *= 6.0D;
            rotatefunc(y, ref z, numero_funcion, dimensions);
            for(int i = 1; i <= dimensions; i++)
                z[i] *= Math.Pow(100.0, 0.5 * (i - 1) / (dimensions - 1));
            for(int i = 1; i <= dimensions; i++)
            {
                s += z[i] * z[i];
                p *= Math.Cos(z[i] / Math.Sqrt(1.0 + i));
            }
            result += 1.0D + s / 4000.0D - p;
            return(result);
        }

        public double CEC_2013_11_RASTRIGIN(double[] position, int dimensions, int numero_funcion)
        {
            double[] z = new double[MAX_DIMENSIONS];
            double[] w = new double[MAX_DIMENSIONS];
            double[] y = new double[MAX_DIMENSIONS];
            double result = 0.0D;
            shiftfunc(position, ref y, numero_funcion, dimensions);
            for(int i = 1; i <= dimensions; i++)
                y[i] *= 0.0512D;
            oszfunc(y, ref w, dimensions);
            asyfunc(w, y, ref z, dimensions, 0.2D);
            for(int i = 1; i <= dimensions; i++)
                z[i] *= Math.Pow(10.0D, 0.5D * (i - 1) / (dimensions - 1));
            for(int i = 1; i <= dimensions; i++)
                result += (z[i] * z[i] - 10.0 * Math.Cos(PI_x_2 * z[i]) + 10.0D);
            return(result);
        }

        public double CEC_2013_12_ROTATED_RASTRIGIN(double[] position, int dimensions, int numero_funcion)
        {
            double[] z = new double[MAX_DIMENSIONS];
            double[] w = new double[MAX_DIMENSIONS];
            double[] y = new double[MAX_DIMENSIONS];
            double[] t = new double[MAX_DIMENSIONS];
            double[] s = new double[MAX_DIMENSIONS];
            double[] r = new double[MAX_DIMENSIONS];
            double result = 0.0D;
            shiftfunc(position, ref y, numero_funcion, dimensions);
            for(int i = 1; i <= dimensions; i++)
                y[i] *= 0.0512D;
            rotatefunc(y, ref w, numero_funcion, dimensions);
            oszfunc(w, ref t, dimensions);
            asyfunc(t, w, ref s, dimensions, 0.2D);
            rotatefunc(s, ref r, numero_funcion + 1, dimensions);
            for (int i = 1; i <= dimensions; i++)
                r[i] *= Math.Pow(10.0D, 0.5D * (i - 1) / (dimensions - 1));
            rotatefunc(r, ref z, numero_funcion, dimensions);
            for (int i = 1; i <= dimensions; i++)
                result += (z[i] * z[i] - 10.0D * Math.Cos(PI_x_2 * z[i]) + 10.0D);
            return(result);
        }

        public double CEC_2013_13_NON_CONTINUOUS_ROTATED_RASTRIGIN(double[] position, int dimensions, int numero_funcion)
        {
            double[] x = new double[MAX_DIMENSIONS];
            double[] z = new double[MAX_DIMENSIONS];
            double[] w = new double[MAX_DIMENSIONS];
            double[] y = new double[MAX_DIMENSIONS];
            double[] t = new double[MAX_DIMENSIONS];
            double[] s = new double[MAX_DIMENSIONS];
            double result = 0.0D;
            shiftfunc(position, ref x, numero_funcion, dimensions);
            for(int i = 1; i <= dimensions; i++)
                x[i] *= 0.0512;
            rotatefunc(x, ref y, numero_funcion, dimensions);
            for(int i = 1; i <= dimensions; i++)
                if (Math.Abs(y[i]) > 0.5)
                    y[i] = Math.Floor(2.0D * y[i] + 0.5D) / 2.0D;
            oszfunc(y, ref t, dimensions);
            asyfunc(t, y, ref s, dimensions, 0.2D);
            rotatefunc(s, ref w, numero_funcion + 1, dimensions);
            for(int i = 1; i <= dimensions; i++)
                w[i] *= Math.Pow(10.0D, 0.5D * (i - 1) / (dimensions - 1));
            rotatefunc(w, ref z, numero_funcion, dimensions);
            for(int i = 1; i <= dimensions; i++)
                result += (z[i] * z[i] - 10.0 * Math.Cos(PI_x_2 * z[i]) + 10.0D);
            return(result);
        }

        public double CEC_2013_14_SCHWEFEL(double[] position, int dimensions, int numero_funcion)
        {
            double[] z = new double[MAX_DIMENSIONS];
            double[] y = new double[MAX_DIMENSIONS];
            double result = 0.0D;
            double tmp;
            shiftfunc(position, ref y, numero_funcion, dimensions);
            for (int i = 1; i <= dimensions; i++)
                y[i] *= 10.0D;
            for (int i = 1; i <= dimensions; i++)
                y[i] *= Math.Pow(10.0D, 0.5D * (i - 1) / (dimensions - 1));
            for (int i = 1; i <= dimensions; i++)
                z[i] = y[i] + 4.209687462275036e+002;
            for (int i = 1; i <= dimensions; i++)
            {
                if (z[i] > 500)
                {
                    result -= (500.0D - (z[i] % 500)) * Math.Sin(Math.Sqrt(500.0D - (z[i] % 500)));
                    tmp = (z[i] - 500.0D) / 100.0D;
                    result += tmp * tmp / dimensions;
                }
                else if (z[i] < -500)
                {
                    result -= (-500.0D + (Math.Abs(z[i]) % 500)) * Math.Sin(Math.Sqrt(500.0D - (Math.Abs(z[i]) % 500)));
                    tmp = (z[i] + 500.0D) / 100.0D;
                    result += tmp * tmp / dimensions;
                }
                else
                    result -= z[i] * Math.Sin(Math.Sqrt(Math.Abs(z[i])));
            }
            result = 4.189828872724338e+002 * dimensions + result;
            return (result);
        }

        public double CEC_2013_15_ROTATED_SCHWEFEL(double[] position, int dimensions, int numero_funcion)
        {
            double[] z = new double[MAX_DIMENSIONS];
            double[] y = new double[MAX_DIMENSIONS];
            double[] x = new double[MAX_DIMENSIONS];
            double result = 0.0D;
            double tmp;
            shiftfunc(position, ref y, numero_funcion, dimensions);
            for (int i = 1; i <= dimensions; i++)
                y[i] *= 10.0D;
            rotatefunc(y, ref x, numero_funcion, dimensions);
            for (int i = 1; i <= dimensions; i++)
                x[i] *= Math.Pow(10.0D, 0.5D * (i - 1) / (dimensions - 1));
            for (int i = 1; i <= dimensions; i++)
                z[i] = x[i] + 4.209687462275036e+002;
            for (int i = 1; i <= dimensions; i++)
            {
                if (z[i] > 500)
                {
                    result -= (500.0D - (z[i] % 500)) * Math.Sin(Math.Sqrt(500.0D - (z[i] % 500)));
                    tmp = (z[i] - 500.0D) / 100.0D;
                    result += tmp * tmp / dimensions;
                }
                else if (z[i] < -500)
                {
                    result -= (-500.0D + (Math.Abs(z[i]) % 500)) * Math.Sin(Math.Sqrt(500.0D - (Math.Abs(z[i]) % 500)));
                    tmp = (z[i] + 500.0D) / 100.0D;
                    result += tmp * tmp / dimensions;
                }
                else
                    result -= z[i] * Math.Sin(Math.Sqrt(Math.Abs(z[i])));
            }
            result = 4.189828872724338e+002 * dimensions + result;
            return (result);
        }

        public double CEC_2013_16_ROTATED_KATSUURA(double[] position, int dimensions, int numero_funcion)
        {

            double temp;
            double tmp1;
            double tmp2;
            double tmp3 = Math.Pow(dimensions, 1.2);
            double[] z = new double[MAX_DIMENSIONS];
            double[] y = new double[MAX_DIMENSIONS];
            double[] x = new double[MAX_DIMENSIONS];
            double result = 1.0D;
            shiftfunc(position, ref x, numero_funcion, dimensions);
            for (int i = 1; i <= dimensions; i++)
                x[i] *= 0.05D;
            rotatefunc(x, ref z, numero_funcion, dimensions);
            for (int i = 1; i <= dimensions; i++)
                z[i] *= Math.Pow(100.0D, 0.5D * (i - 1) / (dimensions - 1));
            rotatefunc(z, ref y, numero_funcion + 1, dimensions);
            for (int i = 1; i <= dimensions; i++)
            {
                temp = 0.0;
                for (int j = 1; j <= 32; j++)
                {
                    tmp1 = Math.Pow(2.0, j);
                    tmp2 = tmp1 * y[i];
                    temp += Math.Abs(tmp2 - Math.Floor(tmp2 + 0.5D)) / tmp1;
                }
                result *= Math.Pow(1.0D + i * temp, 10.0D / tmp3);
            }
            tmp1 = 10.0D / dimensions / dimensions;
            result = result * tmp1 - tmp1;
            return (result);
        }

        public double CEC_2013_17_LUNACEK_BI_RASTRIGIN(double[] position, int dimensions, int numero_funcion)
        {
            double mu0 = 2.5;
            double d = 1.0;
            double s;
            double mu1;
            double tmp;
            double tmp1;
            double tmp2;
            double [] tmpx = new double[MAX_DIMENSIONS];
            double [] y = new double[MAX_DIMENSIONS];
            double [] z = new double[MAX_DIMENSIONS];
            double result;
            s = 1.0 - 1.0 / (2.0 * Math.Pow(dimensions + 20.0, 0.5) - 8.2);
            mu1 = -Math.Sqrt((Math.Pow(mu0,2.0) - d) / s);
            shiftfunc(position, ref y, numero_funcion, dimensions);
            for (int i = 1; i <= dimensions; i++)
                y[i] *= 0.1;
            for (int i = 1; i <= dimensions; i++)
            {
                tmpx[i] = Math.Sign(oinew_CEC_2013[numero_funcion, i]) * 2 * y[i];
                y[i] = tmpx[i];
                tmpx[i] += mu0;
            }
            for (int i = 1; i <= dimensions; i++)
                y[i] *= Math.Pow(100.0, 0.5 * (i -1) / (dimensions - 1));
            tmp1 = 0.0;
            tmp2 = 0.0;
            for (int i = 1; i <= dimensions; i++)
            {
                tmp1 += Math.Pow(tmpx[i] - mu0, 2.0);
                tmp2 += Math.Pow(tmpx[i] - mu1, 2.0);
            }
            tmp2 *= s;
            tmp2 += d * dimensions;
            tmp = 0;
            for (int i = 1; i <= dimensions; i++)
                tmp += Math.Cos(PI_x_2 * y[i]);
            result = Math.Min(tmp1, tmp2);
            result += 10.0 * (dimensions - tmp);
            return (result);
        }

        public double CEC_2013_18_ROTATED_LUNACEK_BI_RASTRIGIN(double[] position, int dimensions, int numero_funcion)
        {
            double mu0 = 2.5;
            double d = 1.0;
            double s;
            double mu1;
            double tmp;
            double tmp1;
            double tmp2;
            double[] tmpx = new double[MAX_DIMENSIONS];
            double[] y = new double[MAX_DIMENSIONS];
            double[] w = new double[MAX_DIMENSIONS];
            double[] z = new double[MAX_DIMENSIONS];
            double result;
            s = 1.0 - 1.0 / (2.0 * Math.Pow(dimensions + 20.0, 0.5) - 8.2);
            mu1 = -Math.Pow((Math.Pow(mu0, 2.0) - d) / s, 0.5);
            shiftfunc(position, ref y, numero_funcion, dimensions);
            for (int i = 1; i <= dimensions; i++)
                y[i] *= 0.1D;
            for (int i = 1; i <= dimensions; i++)
                tmpx[i] = Math.Sign(oinew_CEC_2013[numero_funcion, i]) * 2 * y[i];
            for (int i = 1; i <= dimensions; i++)
            {
                z[i] = tmpx[i];
                tmpx[i] += mu0;
            }
            rotatefunc(z,ref w, numero_funcion, dimensions);
            for (int i = 0; i <= dimensions; i++)
                w[i] *= Math.Pow(100.0, 0.5 * (i - 1) / (dimensions - 1));
            rotatefunc(w, ref z, numero_funcion + 1, dimensions);
            tmp1 = 0.0;
            tmp2 = 0.0;
            for (int i = 1; i <= dimensions; i++)
            {
                tmp1 += Math.Pow(tmpx[i] - mu0, 2.0);
                tmp2 += Math.Pow(tmpx[i] - mu1, 2.0);
            }
            tmp2 *= s;
            tmp2 += d * dimensions;
            tmp = 0;
            for (int i = 1; i <= dimensions; i++)
                tmp += Math.Cos(PI_x_2 * z[i]);
            result = Math.Min(tmp1, tmp2);
            result += 10.0 * (dimensions - tmp);
            return (result);
        }


        public double CEC_2013_19_ROTATED_EXPANDED_GRIEWANK_PLUS_ROSENBROCK(double[] position, int dimensions, int numero_funcion) 
        {
            double temp;
            double[] z = new double[MAX_DIMENSIONS];
            double[] x = new double[MAX_DIMENSIONS];
            double result;
            shiftfunc(position, ref x, numero_funcion, dimensions);
            for (int i = 1; i <= dimensions; i++)
                x[i] *= 0.05D;
            rotatefunc(x, ref z, numero_funcion, dimensions);
            for (int i = 1; i <= dimensions; i++)
                z[i] += 1.0D;
                //z[i] = x[i] + 1; //Así es como dice la bilioteca pero está mal
            result = 0.0D;
            for (int i = 1; i <= dimensions - 1; i++)
            {
                temp = 100.0 * Math.Pow(Math.Pow(z[i], 2) - z[i + 1], 2) + Math.Pow(z[i] - 1.0, 2);
                result += Math.Pow(temp,2) / 4000.0 - Math.Cos(temp) + 1.0;
            }
            temp = 100.0 * Math.Pow(Math.Pow(z[dimensions], 2) - z[1], 2) + Math.Pow(z[dimensions] - 1.0, 2);
            result += Math.Pow(temp, 2) / 4000.0 - Math.Cos(temp) + 1.0;
            return (result);
        }


        public double CEC_2013_20_ROTATED_EXPANDED_SCAFFER_6(double[] position, int dimensions, int numero_funcion)
        {
            double[] z = new double[MAX_DIMENSIONS];
            double[] x = new double[MAX_DIMENSIONS];
            double[] y = new double[MAX_DIMENSIONS];
            double[] w = new double[MAX_DIMENSIONS];
            double result = 0.0D;
            double temp1;
            double temp2;
            shiftfunc(position, ref x, numero_funcion, dimensions);
            rotatefunc(x, ref y, numero_funcion, dimensions);
            asyfunc(y, x, ref w, dimensions, 0.5);
            rotatefunc(w, ref z, numero_funcion + 1, dimensions);
            for (int i = 1; i <= dimensions - 1; i++)
            {
                temp1 = Math.Pow(Math.Sin(Math.Sqrt(Math.Pow(z[i], 2.0D) + Math.Pow(z[i + 1], 2.0D))), 2.0D);
                temp2 = 1.0D + 0.001D * (Math.Pow(z[i], 2.0D) + Math.Pow(z[i + 1], 2.0D));
                result += 0.5D + (temp1 - 0.5D) / Math.Pow(temp2, 2.0D);
            }
            temp1 = Math.Pow(Math.Sin(Math.Sqrt(Math.Pow(z[dimensions], 2.0D) + Math.Pow(z[1], 2.0D))), 2.0D);
            temp2 = 1.0D + 0.001D * (Math.Pow(z[dimensions], 2.0D) + Math.Pow(z[1], 2.0D));
            result += 0.5D + (temp1 - 0.5D) / Math.Pow(temp2, 2.0D);
            return (result);
        }


        public double CEC_2013_21_COMPOSITION_FUNCTION_1(double[] position, int dimensions)
        {
            double [] fit = new double[6];
            double [] sigma = { 0, 10, 20, 30, 40, 50 };
            double [] bias = { 0, 0, 100, 200, 300, 400 };
            double[] lambda = { 0, 1, 1e-6, 1e-26, 1e-6, 0.1 };
            double result;
            fit[1] = lambda[1] * CEC_2013_6_ROTATED_ROSENBROCK(position, dimensions, 1);
            fit[2] = lambda[2] * CEC_2013_5A_ROTATED_DIFFERENT_POWERS(position, dimensions, 2);
            fit[3] = lambda[3] * CEC_2013_3_ROTATED_BENT_CIGAR(position, dimensions, 3);
            fit[4] = lambda[4] * CEC_2013_4_ROTATED_DISCUS(position, dimensions, 4);
            fit[5] = lambda[5] * CEC_2013_1_SPHERE(position, dimensions, 5);
            result = cf_cal(position, dimensions, sigma, bias, fit, 5);
            return (result);
        }

        public double CEC_2013_22_COMPOSITION_FUNCTION_2(double[] position, int dimensions) 
        {
            double [] fit = new double[4];
            double [] sigma = {0,  20, 20, 20 };
            double [] bias = {0,  0, 100, 200 };
            double result;
            for (int i = 1; i <= 3; i++)
                 fit[i] = CEC_2013_14_SCHWEFEL(position, dimensions, i);
            result = cf_cal(position, dimensions, sigma, bias, fit, 3);
            return (result);
        }

        public double CEC_2013_23_COMPOSITION_FUNCTION_3(double[] position, int dimensions)
        {
            double[] fit = new double[4];
            double[] sigma = { 0, 20, 20, 20 };
            double[] bias = { 0, 0, 100, 200 };
            double result;
            for (int i = 1; i <= 3; i++)
                fit[i] = CEC_2013_15_ROTATED_SCHWEFEL(position, dimensions, i);
            result = cf_cal(position, dimensions, sigma, bias, fit, 3);
            return (result);
        }

        public double CEC_2013_24_COMPOSITION_FUNCTION_4(double[] position, int dimensions)
        {
            double [] fit = new double[4];
            double [] sigma = { 0, 20, 20, 20 };
            double[] lambda = { 0, 0.25, 1, 2.5 };
            double [] bias = { 0, 0, 100, 200 };
            double result;
            fit[1] = lambda[1] * CEC_2013_15_ROTATED_SCHWEFEL(position, dimensions, 1);
            fit[2] = lambda[2] * CEC_2013_12_ROTATED_RASTRIGIN(position, dimensions, 2);
            fit[3] = lambda[3] * CEC_2013_9_ROTATED_WEIERSTRASS(position, dimensions, 3);
            result = cf_cal(position, dimensions, sigma, bias, fit, 3);
            return (result);
        }

        public double CEC_2013_25_COMPOSITION_FUNCTION_5(double[] position, int dimensions)
        {
            double[] fit = new double[4];
            double[] sigma = { 0, 10, 30, 50 };
            double[] lambda = { 0, 0.25, 1, 2.5 };
            double[] bias = { 0, 0, 100, 200 };
            double result;
            fit[1] = lambda[1] * CEC_2013_15_ROTATED_SCHWEFEL(position, dimensions, 1);
            fit[2] = lambda[2] * CEC_2013_12_ROTATED_RASTRIGIN(position, dimensions, 2);
            fit[3] = lambda[3] * CEC_2013_9_ROTATED_WEIERSTRASS(position, dimensions, 3);
            result = cf_cal(position, dimensions, sigma, bias, fit, 3);
            return (result);
        }

        public double CEC_2013_26_COMPOSITION_FUNCTION_6(double[] position, int dimensions)
        {
            double[] fit = new double[6];
            double[] sigma = { 0, 10, 10, 10, 10, 10 };
            double[] lambda = { 0, 0.25, 1, 1e-7, 2.5, 10.0 };
            double[] bias = { 0, 0, 100, 200, 300, 400 };
            double result;
            fit[1] = lambda[1] * CEC_2013_15_ROTATED_SCHWEFEL(position, dimensions, 1);
            fit[2] = lambda[2] * CEC_2013_12_ROTATED_RASTRIGIN(position, dimensions, 2);
            fit[3] = lambda[3] * CEC_2013_2_ROTATED_HIGH_CONDITIONED_ELLIPTIC(position, dimensions, 3);
            fit[4] = lambda[4] * CEC_2013_9_ROTATED_WEIERSTRASS(position, dimensions, 4);
            fit[5] = lambda[5] * CEC_2013_10_ROTATED_GRIEWANK(position, dimensions, 5);
            result = cf_cal(position, dimensions, sigma, bias, fit, 5);
            return (result);
        }

        public double CEC_2013_27_COMPOSITION_FUNCTION_7(double[] position, int dimensions)
        {
            double[] fit = new double[6];
            double[] sigma = { 0, 10, 10, 10, 20, 20 };
            double[] lambda = { 0, 100, 10, 2.5, 25, 0.1 };
            double[] bias = { 0, 0, 100, 200, 300, 400 };
            double result;
            fit[1] = lambda[1] * CEC_2013_10_ROTATED_GRIEWANK(position, dimensions, 1);
            fit[2] = lambda[2] * CEC_2013_12_ROTATED_RASTRIGIN(position, dimensions, 2);
            fit[3] = lambda[3] * CEC_2013_15_ROTATED_SCHWEFEL(position, dimensions, 3);
            fit[4] = lambda[4] * CEC_2013_9_ROTATED_WEIERSTRASS(position, dimensions, 4);
            fit[5] = lambda[5] * CEC_2013_1_SPHERE(position, dimensions, 5);
            result = cf_cal(position, dimensions, sigma, bias, fit, 5);
            return (result);
        }

        public double CEC_2013_28_COMPOSITION_FUNCTION_8(double[] position, int dimensions)
        {
            double[] fit = new double[6];
            double[] sigma = { 0, 10, 20, 30, 40, 50 };
            double[] lambda = { 0, 2.5, 2.5e-3, 2.5, 5e-4, 0.1 };
            double[] bias = { 0, 0, 100, 200, 300, 400 };
            double result;
            fit[1] = lambda[1] * CEC_2013_19_ROTATED_EXPANDED_GRIEWANK_PLUS_ROSENBROCK(position, dimensions, 1);
            fit[2] = lambda[2] * CEC_2013_7_ROTATED_SCHAFFER_F7(position, dimensions, 2);
            fit[3] = lambda[3] * CEC_2013_15_ROTATED_SCHWEFEL(position, dimensions, 3);
            fit[4] = lambda[4] * CEC_2013_20_ROTATED_EXPANDED_SCAFFER_6(position, dimensions, 4);
            fit[5] = lambda[5] * CEC_2013_1_SPHERE(position, dimensions, 5);
            result = cf_cal(position, dimensions, sigma, bias, fit, 5);
            return (result);
        }

        public double SPHERE(double[] position, int dimensions)
        {
            double result = 0.0D;
            for (int d = 1; d <= dimensions; d++)
                result += Math.Pow(position[d], 2.0D);
            return (result);
        }

        public double SCHWEFEL_222(double[] position, int dimensions)
        {
            double sum = 0.0D;
            double prod = 1.0D;
            for (int d = 1; d <= dimensions; d++)
            {
                sum += Math.Abs(position[d]);
                prod *= Math.Abs(position[d]);
            }
            return (sum + prod);
        }

        public double ROTATED_HYPERELLIPSOID(double[] position, int dimensions)
        {
            double sum1 = 0.0D;
            double sum2;
            for (int d = 1; d <= dimensions; d++)
            {
                sum2 = 0.0D;
                for (int j = 1; j <= d; j++)
                {
                    sum2 += position[d];
                }
                sum1 += Math.Pow(sum2, 2.0D);
            }
            return (sum1);
        }

        public double RUMP(double[] position, int dimensions)
        {
            return (333.75 - Math.Pow(position[1], 2.0D)) * Math.Pow(position[2], 6.0D) + Math.Pow(position[1], 2.0D) * (11.0D * Math.Pow(position[1], 2.0D) * Math.Pow(position[2], 2.0D) - 121 *
                Math.Pow(position[2], 4.0D) - 2) + 5.5 * Math.Pow(position[2], 8.0D) + position[1] + position[2];
        }

        public double SALOMON(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int i = 1; i <= dimensions; i++)
                sum += Math.Pow(position[i], 2.0D);
            return 1.0 - Math.Cos(2.0 * Math.PI * Math.Sqrt(sum)) + 0.1 * Math.Sqrt(sum);
        }

        public double HYPERELLIPSOID(double[] position, int dimensions)
        {
            double sum1 = 0.0D;
            for (int d = 1; d <= dimensions; d++)
                sum1 += Math.Pow(d, d) + Math.Pow(position[d], 2.0D);
            return (sum1);
        }

        public double SCHWEFEL_221(double[] position, int dimensions)
        {
            double max;
            max = Math.Abs(position[1]);
            for (int d = 2; d <= dimensions; d++)
                if (Math.Abs(position[d]) > max)
                    max = Math.Abs(position[d]);
            return (max);
        }

        public double ROSENBROCK(double[] position, int dimensions)
        {
            double result = 0.0D;
            for (int d = 1; d <= dimensions-1; d++)
                result += 100.0D * Math.Pow((position[d + 1]) - Math.Pow(position[d], 2), 2.0D)
                           + Math.Pow((position[d]) - 1.0D, 2.0D);
            return (result);
        }

        public double STEP_1(double[] position, int dimensions)
        {
            double result = 0.0D;
            for (int d = 1; d <= dimensions; d++)
                result += Math.Floor(Math.Abs(position[d]));
            return (result);
        }

        public double STEP_2(double[] position, int dimensions)
        {
            double result = 0.0D;
            for (int d = 1; d <= dimensions; d++)
                result += Math.Pow(Math.Floor(position[d] + 0.5D), 2.0D);
            return (result);
        }

        public double STEP_3(double[] position, int dimensions)
        {
            double result = 0.0D;
            for (int d = 1; d <= dimensions; d++)
                result += Math.Floor(Math.Pow(position[d], 2.0D));
            return (result);
        }

        public double STYBLINSKI_TANG(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int d = 1; d <= dimensions; d++)
            {
                sum += Math.Pow(position[d], 4)
                    - 16.0D * Math.Pow(position[d], 2)
                    + 5.0D * position[d];
            }
            return sum / 2.0D;
        }

        public double SUM_OF_DIFFERENT_POWERS(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int d = 1; d <= dimensions; d++)
            {
                sum += Math.Pow(Math.Abs(position[d]), d + 1);
            }
            return sum;
        }

        public double SUM_SQUARES(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int d = 1; d <= dimensions; d++)
            {
                sum += d * Math.Pow(position[d], 2);
            }
            return sum;
        }

        public double QUARTIC_WITH_NOISE(double[] position, int dimensions)
        {
            double result = 0.0D;
            for (int d = 1; d <= dimensions; d++)
                result += d * Math.Pow(position[d], 4.0D);
            return (result + aleatorio.NextDouble());
        }

        public double QUARTIC_WITHOUT_NOISE(double[] position, int dimensions)
        {
            double result = 0.0D;
            for (int d = 1; d <= dimensions; d++)
                result += d * Math.Pow(position[d], 4.0D);
            return (result);
        }

        public double QUINTIC(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int i = 1; i <= dimensions; i++)
                sum += Math.Abs(Math.Pow(position[i], 5.0D) - 3.0D * Math.Pow(position[i], 4.0D) + 4.0D * Math.Pow(position[i], 3.0D) + 2.0D * Math.Pow(position[i], 2.0D) - 10.0D * position[i] - 4.0D);
            return sum;
        }

        public double RANA(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int i = 1; i < dimensions; i++)
                sum += position[i] * Math.Sin(Math.Sqrt(Math.Abs(position[i + 1] - position[i] + 1.0D))) * Math.Cos(Math.Sqrt(Math.Abs(position[i + 1] + position[i] + 1.0D))) + (position[i + 1] + 1) 
                    * Math.Sin(Math.Sqrt(Math.Abs(position[i + 1] + position[i] + 1.0D))) * Math.Cos(Math.Sqrt(Math.Abs(position[i + 1] - position[i] + 1.0D)));
            return sum;
        }

        public double SCHWEFEL(double[] position, int dimensions)
        {
            double result = 0.0D; 
            for (int d = 1; d <= dimensions; d++)
                result -= (position[d] * Math.Sin(Math.Sqrt(Math.Abs(position[d]))));
            return ((418.9829D * dimensions) + result);
        }

        public double ROTATED_SCHWEFEL(double[] x, int dimensions)
        {
            double result = 0.0D;
            double zi;
            double[] position = new double[MAX_DIMENSIONS];
            for (int d = 1; d <= dimensions; d++)
            {
                x[d] = x[d] - 420.96D;
                position[d] = 0.0d;
            }
            multiplica_vector_por_matriz_CEC_2005(1, dimensions, x, ref position);
            for (int d = 1; d <= dimensions; d++)
                position[d] += 420.96D;
            for (int d = 1; d <= dimensions; d++)
            {
                if (Math.Abs(position[d]) <= 500.0D)
                    zi = position[d] * Math.Sin(Math.Sqrt(Math.Abs(position[d])));
                else
                    zi = 0.001D * Math.Pow(Math.Abs(position[d]) - 500.0D, 2.0D);
                result -= zi;
            }
            return ((418.9829D * dimensions) + result);
        }

        public double RASTRIGIN(double[] position, int dimensions)
        {
            double result = 0.0D;
            for (int d = 1; d <= dimensions; d++)
                result += (Math.Pow(position[d], 2.0D) - (10.0D * Math.Cos(PI_x_2 * position[d])) + 10.0D);
            return (result);
        }

        public double ROTATED_RASTRIGIN(double[] x, int dimensions)
        {
            double result = 0.0D;
            double[] position = new double[MAX_DIMENSIONS];
            for (int d = 1; d <= dimensions; d++)
                position[d] = 0.0D;
            multiplica_vector_por_matriz_CEC_2005(1, dimensions, x, ref position);
            for (int d = 1; d <= dimensions; d++)
                result += (Math.Pow(position[d], 2.0D) - (10.0D * Math.Cos(PI_x_2 * position[d])) + 10.0D);
            return (result);
        }

        public double NON_CONTINUOUS_RASTRIGIN(double[] position, int dimensions)
        {
            double result = 0.0D;
            for (int d = 1; d <= dimensions; d++)
            {
                if (Math.Abs(position[d]) >= 0.5)
                    position[d] = Math.Round(position[d] * 2.0) / 2.0;
                result += (Math.Pow(position[d], 2.0D) - (10.0D * Math.Cos(PI_x_2 * position[d])) + 10.0D);
            }
            return (result);
        }

        public double ROTATED_NON_CONTINUOUS_RASTRIGIN(double[] x, int dimensions)
        {
            double result = 0.0D;
            double[] position = new double[MAX_DIMENSIONS];
            for (int d = 1; d <= dimensions; d++)
                position[d] = 0.0D;
            multiplica_vector_por_matriz_CEC_2005(1, dimensions, x, ref position);
            for (int d = 1; d <= dimensions; d++)
            {
                if (Math.Abs(position[d]) >= 0.5)
                    position[d] = Math.Round(position[d] * 2.0) / 2.0;
                result += (Math.Pow(position[d], 2.0D) - (10.0D * Math.Cos(PI_x_2 * position[d])) + 10.0D);
            }
            return (result);
        }

        public double ACKLEY_1(double[] position, int dimensions)
        {
            double sum1 = 0.0D;
            double sum2 = 0.0D;
            for (int d = 1; d <= dimensions; d++)
            {
                    sum1 += Math.Pow(position[d], 2.0D);
                    sum2 += Math.Cos(PI_x_2 * position[d]);
            }
            return (-20.0D * Math.Exp(-0.2D * Math.Sqrt(sum1 / dimensions)) - Math.Exp(sum2 / dimensions) + 20.0D + Math.E);
        }

        public double ROTATED_ACKLEY_1(double[] x, int dimensions)
        {
            double sum1 = 0.0D;
            double sum2 = 0.0D;
            double[] position = new double[MAX_DIMENSIONS];
            for (int d = 1; d <= dimensions; d++)
                position[d] = 0.0D;
            multiplica_vector_por_matriz_CEC_2005(1, dimensions, x, ref position);
            for (int d = 1; d <= dimensions; d++)
            {
                sum1 += Math.Pow(position[d], 2.0D);
                sum2 += Math.Cos(PI_x_2 * position[d]);
            }
            return (-20.0D * Math.Exp(-0.2D * Math.Sqrt(sum1 / dimensions)) - Math.Exp(sum2 / dimensions) + 20.0D + Math.E);
        }

        public double ACKLEY_2(double[] position, int dimensions)
        {
            double sum1;
            sum1 = Math.Pow(position[1], 2.0D) + Math.Pow(position[2], 2.0D);
            return (-200.0D * Math.Exp(-0.02D * Math.Sqrt(sum1)));
        }

        public double ACKLEY_3(double[] position, int dimensions)
        {
            double sum1;
            double sum2;
            sum1 = Math.Pow(position[1], 2.0D) + Math.Pow(position[2], 2.0D);
            sum2 = Math.Cos(3.0D * position[1]) + Math.Sin(3.0D * position[2]);
            return (-200.0D * Math.Exp(-0.02D * Math.Sqrt(sum1)) + 5.0D * Math.Exp(sum2));
        }

        public double ACKLEY_4(double[] position, int dimensions)
        {
            double sum3 = 0.0D;
            double sum1;
            double sum2;
            for (int d = 1; d <= dimensions - 1; d++)
            {
                sum1 = Math.Sqrt(Math.Pow(position[d], 2.0D) + Math.Pow(position[d + 1], 2.0D));
                sum2 = Math.Cos(2.0D * position[d]) + Math.Sin(2.0D * position[d + 1]);
                sum3 += Math.Exp(-0.2D) * sum1 + 3.0D * sum2;
            }
            return (sum3);
        }

        public double ADJIMAN(double[] position, int dimensions)
        {
            double sum1 = Math.Cos(position[1]) * Math.Sin(position[2]);
            double sum2 = position[1] / (Math.Pow(position[2], 2.0D) + 1.0D);
            return (sum1 - sum2);
        }

        public double ALPINE_1(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int d = 1; d <= dimensions; d++)
                sum += Math.Abs(position[d] * Math.Sin(position[d]) + 0.1D * position[d]);
            return (sum);
        }

        public double ALPINE_2(double[] position, int dimensions)
        {
            double prod = 1.0D;
            for (int d = 1; d <= dimensions; d++)
                prod *= Math.Sqrt(position[d]) * Math.Sin(position[d]);
            return (prod);
        }

        public double BARTELS(double[] position, int dimensions)
        {
            double prod;
            prod = Math.Abs(Math.Pow(position[1], 2.0D) + Math.Pow(position[2], 2.0D) + position[1] * position[2]);
            prod += Math.Abs(Math.Sin(position[1])) + Math.Abs(Math.Cos(position[2]));
            return (prod);
        }

        public double GRIEWANK(double[] position, int dimensions)
        {
            double sum = 0.0D;
            double prod = 1.0D;
            for (int d = 1; d <= dimensions; d++)
            {
                sum += Math.Pow(position[d], 2.0D);
                prod *= Math.Cos(position[d] / Math.Sqrt(d));
            }
            return ((sum / 4000.0D) - prod + 1.0D);
        }

        public double HELICAL_VALLEY(double[] position, int dimensions)
        {
            double theta;
            if (position[1] >= 0)
                theta = 1.0D / (2.0D * Math.PI) * Math.Atan(position[2] / position[1]);
            else
                theta = 1.0D / (2.0D * Math.PI) * (Math.PI + Math.Atan(position[2] / position[1]));

            return 100.0D * Math.Pow(position[2] - 10.0D * theta, 2.0D) + Math.Pow(Math.Sqrt(Math.Pow(position[1], 2.0D) + Math.Pow(position[2], 2.0D)) - 1.0D, 2.0D) + Math.Pow(position[3], 2.0D);
        }

        public double JENNRICH_SAMPSON(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int i = 1; i <= 10; i++)
            {
                sum += Math.Pow(2.0D + 2.0D * i - (Math.Exp(i * position[1]) + Math.Exp(i * position[2])), 2.0D);
            }
            return sum;
        }

        public double KEANE(double[] position, int dimensions)
        {
            return -Math.Pow(Math.Sin(position[1] - position[2]), 2.0D) * Math.Pow(Math.Sin(position[1] + position[2]), 2.0D) / Math.Sqrt(Math.Pow(position[1], 2.0D) + Math.Pow(position[2], 2.0D));
        }

        public double MIELE_CANTRELL(double[] position, int dimensions)
        {
            return Math.Pow(Math.Exp(-position[1]) - position[2], 4.0D) + 100.0D * Math.Pow(position[2] - position[3], 6.0D) + Math.Pow(Math.Tan(position[3] - position[4]), 4.0D) + Math.Pow(position[1], 8.0D);
        }

        public double ROTATED_GRIEWANK(double[] x, int dimensions)
        {
            double sum = 0.0D;
            double prod = 1.0D;
            double[] position = new double[MAX_DIMENSIONS];
            for (int d = 1; d <= dimensions; d++)
                position[d] = 0.0D;
            multiplica_vector_por_matriz_CEC_2005(1, dimensions, x, ref position);
            for (int d = 1; d <= dimensions; d++)
            {
                sum += Math.Pow(position[d], 2.0D);
                prod *= Math.Cos(position[d] / Math.Sqrt(d));
            }
            return ((sum / 4000.0D) - prod + 1.0D);
        }

        public double GENERALIZED_PENALIZED_1(double[] position, int dimensions)
        {
            int a = 10;
            int k = 100;
            int m = 4;
            double sum1 = 0.0D;
            double sum2 = 0.0D;
            double result;
            double y1_value;
            double yn_value;
            double yi_value;
            double yi_plus1_value;
            double u_value;

            y1_value = 1.0D + 0.25D * (position[1] + 1.0D);
            yn_value = 1.0D + 0.25D * (position[dimensions] + 1.0D);

            result = 10.0D * Math.Pow(Math.Sin(Math.PI * y1_value), 2.0D);
            result += Math.Pow(yn_value - 1, 2.0D);

            for (int d = 1; d <= dimensions - 1; d++)
            {
                yi_value = 1.0D + 0.25D * (position[d] + 1.0D);
                yi_plus1_value = 1.0D + 0.25D * (position[d + 1] + 1.0D);
                sum1 += Math.Pow(yi_value - 1.0D, 2.0D) * (1.0D + 10.0D * Math.Pow(Math.Sin(Math.PI * yi_plus1_value), 2.0D));
            }
            for (int d = 1; d <= dimensions; d++)
            {
                if (position[d] > a)
                {
                    u_value = k * Math.Pow(position[d] - a, m);
                }
                else if (position[d] < -a)
                {
                    u_value = k * Math.Pow(-position[d] - a, m);
                }
                else
                {
                    u_value = 0.0D;
                }
                sum2 += u_value;
            }
            result += sum1;
            result *= Math.PI / dimensions;
            result += sum2;
            return (result);
        }

        public double GENERALIZED_PENALIZED_2(double[] position, int dimensions)
        {
            int a = 5;
            int k = 100;
            int m = 4;
            double u_value;
            double y1_value;
            double yn_value;
            double sum1 = 0.0D;
            double sum2 = 0.0D;
            double result;

            y1_value = Math.Pow(Math.Sin(PI_x_3 * position[1]), 2.0D);
            yn_value = Math.Pow(position[dimensions] - 1.0D, 2.0D) *
                       (1.0D + Math.Pow(Math.Sin(PI_x_2 * position[dimensions]), 2.0D));

            for (int d = 1; d <= dimensions - 1; d++)
                sum1 += Math.Pow(position[d] - 1.0D, 2.0D) * (1.0D + Math.Pow(Math.Sin(PI_x_3 * position[d + 1]), 2.0D));
            for (int d = 1; d <= dimensions; d++)
            {
                if (position[d] > a)
                {
                    u_value = k * Math.Pow(position[d] - a, m);
                }
                else if (position[d] < -a)
                {
                    u_value = k * Math.Pow(-position[d] - a, m);
                }
                else
                {
                    u_value = 0.0D;
                }
                sum2 += u_value;
            }

            result = 0.1D * (y1_value + sum1 + yn_value) + sum2;
            return (result);
        }

        public double SHEKEL_FOXHOLE(double[] position, int dimensions)
        {
            int[][] a_ij = new int[2][]
            {
                new int[] {-32,-16,  0, 16, 32,-32,-16,  0, 16, 32,-32,-16, 0, 16, 32, -32,-16, 0, 16, 32, -32, -16,  0, 16, 32 },
                new int[] {-32,-32,-32,-32,-32,-16,-16,-16,-16,-16,  0,  0, 0,  0,  0,  32, 32,32, 32, 32,  16,  16, 16, 16, 16}
            };
            double sum1 = 0.0D;
            double sum2;
            for (int j = 1; j <= 25; j++)
            {
                sum2 = 0.0D;
                for (int i = 1; i <= dimensions; i++)
                    sum2 += Math.Pow(position[i] - a_ij[i - 1][j - 1], 6.0D);
                sum1 += 1.0D / (j + sum2);
            }
            return (1.0D / (0.002D + sum1));
        }

        public double KOWALIK(double[] position, int dimensions)
        {
            double[] a = { 0.1957D, 0.1947D, 0.1735D, 0.16D, 0.0844D, 0.0627D, 0.0456D, 0.0342D, 0.0323D, 0.0235D, 0.0246D };
            double[] b = { 4.0D, 2.0D, 1.0D, 0.5D, 0.25D, 1.0D / 6.0D, 0.125D, 0.1D, 1.0D / 12.0D, 1.0D / 14.0D, 0.0625D };

            double result = 0.0D;
            double sum1;
            double sum2;
            for (int i = 0; i <= 10; i++)
            {
                sum1 = position[1] * (Math.Pow(b[i], 2.0D) + b[i] * position[2]);
                sum2 = Math.Pow(b[i], 2.0D) + b[i] * position[3] + position[4];
                result += Math.Pow(a[i] - (sum1 / sum2), 2.0D);
            }
            return (result);
        }

        public double SIX_HUMP_CAMEL(double[] position, int dimensions)
        {
            return (4.0D * Math.Pow(position[1], 2.0D))
                    - (2.1D * Math.Pow(position[1], 4.0D))
                    + (Math.Pow(position[1], 6.0D) / 3.0D)
                    + (position[1] * position[2])
                    - (4.0D * Math.Pow(position[2], 2.0D))
                    + (4.0D * Math.Pow(position[2], 4.0D));
        }

        public double CHEN_BIRD(double[] position, int dimensions)
        {
            const double b = 0.001D;
            const double b_cuadrada = 0.000001D;
            double sum1 = b_cuadrada + Math.Pow(Math.Pow(position[1], 2.0D) + Math.Pow(position[2], 2.0D) - 1.0D, 2.0D);
            double sum2 = b_cuadrada + Math.Pow(Math.Pow(position[1], 2.0D) + Math.Pow(position[2], 2.0D) - 0.5D, 2.0D);
            double sum3 = b_cuadrada + Math.Pow(Math.Pow(position[1], 2.0D) - Math.Pow(position[2], 2.0D), 2.0D);

            return (-b / sum1 - b / sum2 - b / sum3);
        }

        public double CHEN_V(double[] position, int dimensions)
        {

            const double b = 0.001D;
            const double b_cuadrada = 0.000001D;
            double sum1 = b_cuadrada + Math.Pow(position[1] - 0.4D * position[2] - 1.0D, 2.0D);
            double sum2 = b_cuadrada + Math.Pow(2.0D * position[1] + position[2] - 1.5D, 2.0D);
            return (-b / sum1 - b / sum2);
        }

        public double THREE_HUMP_CAMEL(double[] position, int dimensions)
        {
            double sum1 = 2.0D * Math.Pow(position[1], 2.0) - 1.05D * Math.Pow(position[1], 4.0);
            double sum2 = Math.Pow(position[1], 6.0D) / 6.0D + (position[1] * position[2]) + Math.Pow(position[2], 2.0D);
            return (sum1 + sum2);
        }

        public double BRANIN_1(double[] position, int dimensions)
        {
            const double a = -1.275D / (Math.PI * Math.PI);
            const double b = 5.0D / Math.PI;
            const double c = 10.0D - 1.25D / Math.PI;

            double sum1;
            double sum2;

            sum1 = a * Math.Pow(position[1], 2.0D) + b * position[1] + position[2] - 6.0D;
            sum2 = c * Math.Cos(position[1]);
            return (Math.Pow(sum1, 2.0D) + sum2 + 10.0D);
        }

        public double BRANIN_2(double[] position, int dimensions)
        {
            const double b = 5.1D / (4.0D * Math.PI * Math.PI);
            const double c = 5.0D / Math.PI;
            const double d = 6.0D;
            const double e = 10.0D;
            const double g = 1.0D / (8.0D * Math.PI);

            double sum1;
            double sum2;
            double sum3;

            sum1 = Math.Pow(position[2] - b * Math.Pow(position[1], 2.0D) + c * position[1] - d, 2.0D);
            sum2 = e * (1.0D - g) * Math.Cos(position[1]) * Math.Cos(position[2]);
            sum3 = Math.Log(Math.Pow(position[1], 2.0) + Math.Pow(position[2], 2.0D) + 1.0D);
            return (-1.0D / (sum1 + sum2 + sum3 + e));
        }

        public double BRENT(double[] position, int dimensions)
        {
            double sum1;
            double sum2;

            sum1 = Math.Pow(position[1] + 10.0D, 2.0D) + Math.Pow(position[2] + 10.0D, 2.0D);
            sum2 = Math.Exp(-Math.Pow(position[1], 2.0D) - Math.Pow(position[2], 2.0D));
            return (sum1 + sum2);
        }

        public double BROWN(double[] position, int dimensions)
        {
            double sum1;
            double sum2;
            double result = 0.0D;
            for (int d = 1; d <= dimensions - 1; d++)
            {
                sum1 = Math.Pow(Math.Pow(position[d], 2.0D), Math.Pow(position[d + 1], 2.0D) + 1.0D);
                sum2 = Math.Pow(Math.Pow(position[d + 1], 2.0D), Math.Pow(position[d], 2.0D) + 1.0D);
                result += sum1 + sum2;
            }
            return (result);
        }

        public double BUKIN_2(double[] position, int dimensions)
        {
            double sum1;
            double sum2;
            sum1 = position[2] - 0.01D * Math.Pow(position[1], 2.0D) + 1.0D;
            sum2 = 0.01D * Math.Pow(position[1] + 10.0D, 2.0D);
            return (100.0D * sum1 + sum2);
        }

        public double BUKIN_4(double[] position, int dimensions)
        {
            double sum1;
            double sum2;
            sum1 = 100.0D * Math.Pow(position[2], 2.0D);
            sum2 = 0.01D * Math.Abs(position[1] + 10.0D);
            return (sum1 + sum2);
        }

        public double BUKIN_6(double[] position, int dimensions)
        {
            double sum1;
            double sum2;
            sum1 = 100.0D * Math.Sqrt(Math.Abs(position[2] - 0.01D * Math.Pow(position[1], 2.0D)));
            sum2 = 0.01D * Math.Abs(position[1] + 10.0D);
            return (sum1 + sum2);
        }

        public double GOLDSTEIN_PRICE(double[] position, int dimensions)
        {
            return (1.0D + Math.Pow(position[1] + position[2] + 1.0D, 2.0D)
                    * (19.0D - 14.0D * position[1] + 3.0D * Math.Pow(position[1], 2.0D)
                    - 14.0D * position[2] + 6.0D * position[1] * position[2] + 3.0D * Math.Pow(position[2], 2.0D))) *
                    (30.0D + Math.Pow(2.0D * position[1] - 3.0D * position[2], 2.0D) *
                    (18.0D - 32.0D * position[1] + 12.0D * Math.Pow(position[1], 2.0D) +
                    48.0D * position[2] - 36.0D * position[1] * position[2] + 27.0D * Math.Pow(position[2], 2.0D)));
                    
        }

        public double HARTMANN_3D(double[] position, int dimensions)
        {
            double[] c = { 1.0D, 1.2D, 3.0D, 3.2D };
            double[][] A =  {
                                new double[] { 3.0D, 10.0D, 30.0D },
                                new double[] { 0.1D, 10.0D, 35.0D },
                                new double[] { 3.0D, 10.0D, 30.0D },
                                new double[] { 0.1D, 10.0D, 35.0D }
                             };
            double[][] P = {
                                new double[] { 0.3689D, 0.1170D, 0.2673D },
                                new double[] { 0.4699D, 0.4387D, 0.7470D },
                                new double[] { 0.1091D, 0.8732D, 0.5547D },
                                new double[] { 0.03815D, 0.5743D, 0.8828D}
                            };
            double result = 0.0D;
            double sum;
            for (int i = 1; i <= 4; i++)
            {
                sum = 0.0D;
                for (int j = 1; j <= 3; j++)
                    sum += A[i - 1][j - 1] * Math.Pow(position[j] - P[i - 1][j - 1], 2.0D);
                result += c[i - 1] * Math.Exp(-sum);
            }
            return (-result);
        }

        public double HARTMANN_6D(double[] position, int dimensions)
        {
            double[] c = { 1.0D, 1.2D, 3.0D, 3.2D };
            double[][] A = new double[4][]
            {
                new double[6] { 10.0D, 3.0D, 17.0D, 3.5D, 1.7D, 8.0D },
                new double[6] { 0.05D, 10.0D, 17.0D, 0.1D, 8.0D, 14.0D },
                new double[6] { 3.0D, 3.5D, 1.7D, 10.0D, 17.0D, 8.0D },
                new double[6] { 17.0D, 8.0D, 0.05D, 10.0D, 0.1D, 14.0D }
            };
            double[][] P =
            {
                new double[6] { 0.1312D, 0.1696D, 0.5569D, 0.0124D, 0.8283D, 0.5886D },
                new double[6] { 0.2329D, 0.4135D, 0.8307D, 0.3736D, 0.1004D, 0.9991D },
                new double[6] { 0.2348D, 0.1451D, 0.3522D, 0.2883D, 0.3047D, 0.6650D },
                new double[6] { 0.4047D, 0.8828D, 0.8732D, 0.5743D, 0.1091D, 0.0381D }
            };
            double result = 0.0D;
            double sum;
            for (int i = 1; i <= 4; i++)
            {
                sum = 0.0D;
                for (int j = 1; j <= 6; j++)
                    sum += A[i - 1][j - 1] * Math.Pow(position[j] - P[i - 1][j - 1], 2.0D);
                result += c[i - 1] * Math.Exp(-sum);
            }
            return (-result);
        }

        public double SHEKEL_4_5(double[] position, int dimensions)
        {
            double[][] A =
            {
                new double[] { 4.0D, 4.0D, 4.0D, 4.0D },
                new double[] { 1.0D, 1.0D, 1.0D, 1.0D },
                new double[] { 8.0D, 8.0D, 8.0D, 8.0D },
                new double[] { 6.0D, 6.0D, 6.0D, 6.0D },
                new double[] { 3.0D, 7.0D, 3.0D, 7.0D },
                new double[] { 2.0D, 9.0D, 2.0D, 9.0D },
                new double[] { 5.0D, 5.0D, 3.0D, 3.0D },
                new double[] { 8.0D, 1.0D, 8.0D, 1.0D },
                new double[] { 6.0D, 2.0D, 6.0D, 2.0D },
                new double[] { 7.0D, 3.6D, 7.0D, 3.6D}
            };
            double[] c = new double[10] { 0.1D, 0.2D, 0.2D, 0.4D, 0.4D, 0.6D, 0.3D, 0.7D, 0.5D, 0.5D };
            double[] resta = new double[4];
            double mult_matrices;
            double result = 0.0D;
            int m = 5;
            for (int i = 1; i <= m; i++)
            {
                mult_matrices = 0.0D;
                for (int d = 1; d <= 4; d++)
                {
                    resta[d - 1] = position[d] - A[i - 1][d - 1];
                    mult_matrices += Math.Pow(resta[d - 1], 2.0D);
                }
                result += 1.0D / (mult_matrices + c[i - 1]);
            }
            return (-result);
        }

        public double SHEKEL_4_7(double[] position, int dimensions)
        {
            double[][] A =
            {
                new double[] { 4.0D, 4.0D, 4.0D, 4.0D },
                new double[] { 1.0D, 1.0D, 1.0D, 1.0D },
                new double[] { 8.0D, 8.0D, 8.0D, 8.0D },
                new double[] { 6.0D, 6.0D, 6.0D, 6.0D },
                new double[] { 3.0D, 7.0D, 3.0D, 7.0D },
                new double[] { 2.0D, 9.0D, 2.0D, 9.0D },
                new double[] { 5.0D, 5.0D, 3.0D, 3.0D },
                new double[] { 8.0D, 1.0D, 8.0D, 1.0D },
                new double[] { 6.0D, 2.0D, 6.0D, 2.0D },
                new double[] { 7.0D, 3.6D, 7.0D, 3.6D}
            };
            double[] c = new double[10] { 0.1D, 0.2D, 0.2D, 0.4D, 0.4D, 0.6D, 0.3D, 0.7D, 0.5D, 0.5D };
            double[] resta = new double[4];
            double mult_matrices;
            double result = 0.0D;
            int m = 7;
            for (int i = 1; i <= m; i++)
            {
                mult_matrices = 0.0D;
                for (int d = 1; d <= 4; d++)
                {
                    resta[d - 1] = position[d] - A[i - 1][d - 1];
                    mult_matrices += Math.Pow(resta[d - 1], 2.0D);
                }
                result += 1 / (mult_matrices + c[i - 1]);
            }
            return (-result);
        }

        public double SHEKEL_4_10(double[] position, int dimensions)
        {
            double[][] A =
            {
                new double[] { 4.0D, 4.0D, 4.0D, 4.0D },
                new double[] { 1.0D, 1.0D, 1.0D, 1.0D },
                new double[] { 8.0D, 8.0D, 8.0D, 8.0D },
                new double[] { 6.0D, 6.0D, 6.0D, 6.0D },
                new double[] { 3.0D, 7.0D, 3.0D, 7.0D },
                new double[] { 2.0D, 9.0D, 2.0D, 9.0D },
                new double[] { 5.0D, 5.0D, 3.0D, 3.0D },
                new double[] { 8.0D, 1.0D, 8.0D, 1.0D },
                new double[] { 6.0D, 2.0D, 6.0D, 2.0D },
                new double[] { 7.0D, 3.6D, 7.0D, 3.6D}
            };
            double[] c = new double[10] { 0.1D, 0.2D, 0.2D, 0.4D, 0.4D, 0.6D, 0.3D, 0.7D, 0.5D, 0.5D };
            double[] resta = new double[4];
            double mult_matrices;
            double result = 0.0D;
            int m = 10;
            for (int i = 1; i <= m; i++)
            {
                mult_matrices = 0.0D;
                for (int d = 1; d <= 4; d++)
                {
                    resta[d - 1] = position[d] - A[i - 1][d - 1];
                    mult_matrices += Math.Pow(resta[d - 1], 2.0D);
                }
                result += 1 / (mult_matrices + c[i - 1]);
            }
            return (-result);
        }

        public double SHUBERT(double[] position, int dimensions)
        {
            double sum1 = 0.0D, sum2 = 0.0D;
            for (int i = 1; i <= 5; i++)
            {
                sum1 += i * Math.Cos((i + 1) * position[1] + i);
                sum2 += i * Math.Cos((i + 1) * position[2] + i);
            }
            return sum1 * sum2;
        }

        public double WEIERSTRASS(double[] position, int dimensions)
        {
            double sum1;
            double sum2;
            double sum3 = 0.0D;
            const double a = 0.5D;
            const int b = 3;
            const int Kmax = 20;

            for (int i = 1; i <= dimensions; i++)
            {
                sum1 = 0.0D;
                for (int k = 0; k <= Kmax; k++)
                    sum1 += (Math.Pow(a, k) * Math.Cos(PI_x_2 * Math.Pow(b, k) * (position[i] + 0.5D)));
                sum2 = 0.0D;
                for (int k = 0; k <= Kmax; k++)
                    sum2 += (Math.Pow(a, k) * Math.Cos(Math.PI * Math.Pow(b, k)));
                sum3 += sum1 - dimensions * sum2;
            }

            return (sum3);
        }

        public double ROTATED_WEIERSTRASS(double[] x, int dimensions)
        {
            double sum1;
            double sum2;
            double sum3 = 0.0D;
            const double a = 0.5D;
            const int b = 3;
            const int Kmax = 20;
            double[] position = new double[MAX_DIMENSIONS];
            for (int d = 1; d <= dimensions; d++)
                position[d] = 0.0D;
            multiplica_vector_por_matriz_CEC_2005(1, dimensions, x, ref position);
            for (int i = 1; i <= dimensions; i++)
            {
                sum1 = 0.0D;
                for (int k = 0; k <= Kmax; k++)
                    sum1 += (Math.Pow(a, k) * Math.Cos(PI_x_2 * Math.Pow(b, k) * (position[i] + 0.5D)));
                sum2 = 0.0D;
                for (int k = 0; k <= Kmax; k++)
                    sum2 += (Math.Pow(a, k) * Math.Cos(Math.PI * Math.Pow(b, k)));
                sum3 += sum1 - dimensions * sum2;
            }

            return (sum3);
        }

                
        public double CEC_2005_CF1(double[] position, int D)
        {
            double[] w = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] fit = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] fmax = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] f = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] x1 = new double[MAX_DIMENSIONS];
            double[] x2 = new double[MAX_DIMENSIONS];
            double SumW;
            double MaxW;
            double[] lambda = { 0.05D, 0.05D, 0.05D, 0.05D, 0.05D, 0.05D, 0.05D, 0.05D, 0.05D, 0.05D };

            const double C = 2000.0D;

            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                double suma = 0.0D;
                for (int j = 1; j <= D; j++)
                    suma += Math.Pow(position[j] - oinew_CEC_2005[i, j] + oiold_CEC_2005[i, j], 2.0D);
                w[i] = Math.Exp(-suma / 2.0D / D);
                for (int k = 1; k <= D; k++)
                {
                    x1[k] = (position[k] - oinew_CEC_2005[i, k] + oiold_CEC_2005[i, k]) / lambda[i - 1];
                    x2[k] = 0.0D;
                }
                multiplica_vector_por_matriz_CEC_2005(i, D, x1, ref x2);
                fit[i] = Function( x2, D, tipo_funciones.SPHERE);
                for (int k = 1; k <= D; k++)
                {
                    x1[k] = z_compuesta_CEC_2005[i] / lambda[i - 1];
                    x2[k] = 0.0D;
                }
                multiplica_vector_por_matriz_CEC_2005(i, D, x1, ref x2);
                fmax[i] = Function( x2, D, tipo_funciones.SPHERE);
                f[i] = C * fit[i] / fmax[i];
            }
            SumW = w[1];
            MaxW = w[1];
            for (int i = 2; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                SumW += w[i];
                if (w[i] > MaxW)
                    MaxW = w[i];
            }
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                if (w[i] != MaxW)
                    w[i] = w[i] * (1 - Math.Pow(MaxW, 10.0D));
                w[i] /= SumW;
            }
            double valor = 0.0D;
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
                valor += w[i] * (f[i] + f_bias_CEC_2005[i]);
            cantidad_de_veces_que_se_evalua_la_funcion -= 20;
            return (valor);
        }

        public double CEC_2005_CF2(double[] position, int D)
        {
            double[] w = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] fit = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] fmax = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] f = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] x1 = new double[MAX_DIMENSIONS];
            double[] x2 = new double[MAX_DIMENSIONS];
            double SumW;
            double MaxW;
            double[] lambda = { 0.05D, 0.05D, 0.05D, 0.05D, 0.05D, 0.05D, 0.05D, 0.05D, 0.05D, 0.05D };
            const double C = 2000.0D;
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                double suma = 0.0D;
                for (int j = 1; j <= D; j++)
                    suma += Math.Pow(position[j] - oinew_CEC_2005[i, j] + oiold_CEC_2005[i, j], 2.0D);
                w[i] = Math.Exp(-suma / 2.0D / D);
                for (int k = 1; k <= D; k++)
                {
                    x1[k] = (position[k] - oinew_CEC_2005[i, k] + oiold_CEC_2005[i, k]) / lambda[i - 1];
                    x2[k] = 0.0D;
                }
                multiplica_vector_por_matriz_CEC_2005(i, D, x1, ref x2);
                fit[i] = Function( x2, D, tipo_funciones.GRIEWANK);
                for (int k = 1; k <= D; k++)
                {
                    x1[k] = (z_compuesta_CEC_2005[i] / lambda[i - 1]);
                    x2[k] = 0.0D;
                }
                multiplica_vector_por_matriz_CEC_2005(i, D, x1, ref x2);
                fmax[i] = Function( x2, D, tipo_funciones.GRIEWANK);
                f[i] = C * fit[i] / fmax[i];
            }
            SumW = w[1];
            MaxW = w[1];
            for (int i = 2; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                SumW += w[i];
                if (w[i] > MaxW)
                    MaxW = w[i];
            }
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                if (w[i] != MaxW)
                    w[i] = w[i] * (1 - Math.Pow(MaxW, 10.0D));
                w[i] /= SumW;
            }
            double valor = 0.0D;
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
                valor += w[i] * (f[i] + f_bias_CEC_2005[i]);
            cantidad_de_veces_que_se_evalua_la_funcion -= 20;
            return (valor);
        }

        public double CEC_2005_CF3(double[] position, int D)
        {
            double[] w = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] fit = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] fmax = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] f = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] x1 = new double[MAX_DIMENSIONS];
            double[] x2 = new double[MAX_DIMENSIONS];
            double SumW;
            double MaxW;
            double[] sigma = { 1.0D, 1.0D, 1.0D, 1.0D, 1.0D, 1.0D, 1.0D, 1.0D, 1.0D, 1.0D };
            double[] lambda = { 0.1D, 0.1D, 0.1D, 0.1D, 0.1D, 0.1D, 0.1D, 0.1D, 0.1D, 0.1D };
            const double C = 2000.0D;
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                double suma = 0.0D;
                for (int j = 1; j <= D; j++)
                    suma += Math.Pow(position[j] - oinew_CEC_2005[i, j] + oiold_CEC_2005[i, j], 2.0D);
                w[i] = Math.Exp(-suma / 2.0D / D / Math.Pow(sigma[i - 1], 2.0D));
                for (int k = 1; k <= D; k++)
                {
                    x1[k] = (position[k] - oinew_CEC_2005[i, k] + oiold_CEC_2005[i, k]) / lambda[i - 1];
                    x2[k] = 0.0D;
                }
                multiplica_vector_por_matriz_CEC_2005(i, D, x1, ref x2);
                fit[i] = Function( x2, D, tipo_funciones.GRIEWANK);
                for (int k = 1; k <= D; k++)
                {
                    x1[k] = (z_compuesta_CEC_2005[i] / lambda[i - 1]);
                    x2[k] = 0.0D;
                }
                multiplica_vector_por_matriz_CEC_2005(i, D, x1, ref x2);
                fmax[i] = Function( x2, D, tipo_funciones.GRIEWANK);
                f[i] = C * fit[i] / fmax[i];
            }
            SumW = w[1];
            MaxW = w[1];
            for (int i = 2; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                SumW += w[i];
                if (w[i] > MaxW)
                    MaxW = w[i];
            }
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                if (w[i] != MaxW)
                    w[i] = w[i] * (1 - Math.Pow(MaxW, 10.0D));
                w[i] /= SumW;
            }
            double valor = 0.0D;
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
                valor += w[i] * (f[i] + f_bias_CEC_2005[i]);
            cantidad_de_veces_que_se_evalua_la_funcion -= 20;
            return (valor);
        }

        public double CEC_2005_CF4(double[] position, int D)
        {
            double[] w = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] fit = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] fmax = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] f = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] x1 = new double[MAX_DIMENSIONS];
            double[] x2 = new double[MAX_DIMENSIONS];
            double SumW;
            double MaxW;
            double[] lambda = { 0.15625D, 0.15625D, 1.0D, 1.0D, 10.0D, 10.0D, 0.05D, 0.05D, 0.05D, 0.05D };
            const double C = 2000.0D;
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                double suma = 0.0D;
                for (int j = 1; j <= D; j++)
                    suma += Math.Pow(position[j] - oinew_CEC_2005[i, j] + oiold_CEC_2005[i, j], 2.0D);
                w[i] = Math.Exp(-suma / 2.0D / D);
                for (int k = 1; k <= D; k++)
                {
                    x1[k] = (position[k] - oinew_CEC_2005[i, k] + oiold_CEC_2005[i, k]) / lambda[i - 1];
                    x2[k] = 0.0D;
                }
                multiplica_vector_por_matriz_CEC_2005(i, D, x1, ref x2);
                switch (i)
                {
                    case 1:
                    case 2:
                        fit[i] = Function( x2, D, tipo_funciones.ACKLEY_1);
                        break;
                    case 3:
                    case 4:
                        fit[i] = Function( x2, D, tipo_funciones.RASTRIGIN);
                        break;
                    case 5:
                    case 6:
                        fit[i] = Function( x2, D, tipo_funciones.WEIERSTRASS);
                        break;
                    case 7:
                    case 8:
                        fit[i] = Function( x2, D, tipo_funciones.GRIEWANK);
                        break;
                    case 9:
                    case 10:
                        fit[i] = Function( x2, D, tipo_funciones.SPHERE);
                        break;
                }
                for (int k = 1; k <= D; k++)
                {
                    x1[k] = (z_compuesta_CEC_2005[i] / lambda[i - 1]);
                    x2[k] = 0.0D;
                }
                multiplica_vector_por_matriz_CEC_2005(i, D, x1, ref x2);
                switch (i)
                {
                    case 1:
                    case 2:
                        fmax[i] = Function( x2, D, tipo_funciones.ACKLEY_1);
                        break;
                    case 3:
                    case 4:
                        fmax[i] = Function( x2, D, tipo_funciones.RASTRIGIN);
                        break;
                    case 5:
                    case 6:
                        fmax[i] = Function( x2, D, tipo_funciones.WEIERSTRASS);
                        break;
                    case 7:
                    case 8:
                        fmax[i] = Function( x2, D, tipo_funciones.GRIEWANK);
                        break;
                    case 9:
                    case 10:
                        fmax[i] = Function( x2, D, tipo_funciones.SPHERE);
                        break;
                }
                f[i] = C * fit[i] / fmax[i];
            }
            SumW = w[1];
            MaxW = w[1];
            for (int i = 2; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                SumW += w[i];
                if (w[i] > MaxW)
                    MaxW = w[i];
            }
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                if (w[i] != MaxW)
                    w[i] = w[i] * (1 - Math.Pow(MaxW, 10.0D));
                w[i] /= SumW;
            }
            double valor = 0.0D;
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
                valor += w[i] * (f[i] + f_bias_CEC_2005[i]);
            cantidad_de_veces_que_se_evalua_la_funcion -= 20;
            return (valor);
        }

        public double CEC_2005_CF5(double[] position, int D)
        {
            double[] w = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] fit = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] fmax = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] f = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] x1 = new double[MAX_DIMENSIONS];
            double[] x2 = new double[MAX_DIMENSIONS];
            double SumW;
            double MaxW;
            double[] lambda = { 0.2D, 0.2D, 10.0D, 10.0D, 0.05D, 0.05D, 0.15625D, 0.15625D, 0.05D, 0.05D };
            const double C = 2000.0D;
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                double suma = 0.0D;
                for (int j = 1; j <= D; j++)
                    suma += Math.Pow(position[j] - oinew_CEC_2005[i, j] + oiold_CEC_2005[i, j], 2.0D);
                w[i] = Math.Exp(-suma / 2.0D / D);
                for (int k = 1; k <= D; k++)
                {
                    x1[k] = (position[k] - oinew_CEC_2005[i, k] + oiold_CEC_2005[i, k]) / lambda[i - 1];
                    x2[k] = 0.0D;
                }
                multiplica_vector_por_matriz_CEC_2005(i, D, x1, ref x2);
                switch (i)
                {
                    case 1:
                    case 2:
                        fit[i] = Function( x2, D, tipo_funciones.RASTRIGIN);
                        break;
                    case 3:
                    case 4:
                        fit[i] = Function( x2, D, tipo_funciones.WEIERSTRASS);
                        break;
                    case 5:
                    case 6:
                        fit[i] = Function( x2, D, tipo_funciones.GRIEWANK);
                        break;
                    case 7:
                    case 8:
                        fit[i] = Function( x2, D, tipo_funciones.ACKLEY_1);
                        break;
                    case 9:
                    case 10:
                        fit[i] = Function( x2, D, tipo_funciones.SPHERE);
                        break;
                }
                for (int k = 1; k <= D; k++)
                {
                    x1[k] = (z_compuesta_CEC_2005[i] / lambda[i - 1]);
                    x2[k] = 0.0D;
                }
                multiplica_vector_por_matriz_CEC_2005(i, D, x1, ref x2);
                switch (i)
                {
                    case 1:
                    case 2:
                        fmax[i] = Function( x2, D, tipo_funciones.RASTRIGIN);
                        break;
                    case 3:
                    case 4:
                        fmax[i] = Function( x2, D, tipo_funciones.WEIERSTRASS);
                        break;
                    case 5:
                    case 6:
                        fmax[i] = Function( x2, D, tipo_funciones.GRIEWANK);
                        break;
                    case 7:
                    case 8:
                        fmax[i] = Function( x2, D, tipo_funciones.ACKLEY_1);
                        break;
                    case 9:
                    case 10:
                        fmax[i] = Function( x2, D, tipo_funciones.SPHERE);
                        break;
                }
                f[i] = C * fit[i] / fmax[i];
            }
            SumW = w[1];
            MaxW = w[1];
            for (int i = 2; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                SumW += w[i];
                if (w[i] > MaxW)
                    MaxW = w[i];
            }
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                if (w[i] != MaxW)
                    w[i] = w[i] * (1 - Math.Pow(MaxW, 10.0D));
                w[i] /= SumW;
            }
            double valor = 0.0D;
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
                valor += w[i] * (f[i] + f_bias_CEC_2005[i]);
            cantidad_de_veces_que_se_evalua_la_funcion -= 20;
            return (valor);
        }

        public double CEC_2005_CF6(double[] position, int D)
        {
            double[] w = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] fit = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] fmax = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] f = new double[MAX_FUNCIONES_COMPUESTAS + 1];
            double[] x1 = new double[MAX_DIMENSIONS];
            double[] x2 = new double[MAX_DIMENSIONS];
            double SumW;
            double MaxW;
            double[] sigma = { 0.1D, 0.2D, 0.3D, 0.4D, 0.5D, 0.6D, 0.7D, 0.8D, 0.9D, 1.0D };
            double[] lambda = { 0.02D, 0.04D, 3.0D, 4.0D, 0.025D, 0.03D, 0.109375D, 0.125D, 0.045D, 0.05D };
            const double C = 2000.0D;
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                double suma = 0.0D;
                for (int j = 1; j <= D; j++)
                    suma += Math.Pow(position[j] - oinew_CEC_2005[i, j] + oiold_CEC_2005[i, j], 2.0D);
                w[i] = Math.Exp(-suma / 2.0D / D / Math.Pow(sigma[i - 1], 2.0D));
                for (int k = 1; k <= D; k++)
                {
                    x1[k] = (position[k] - oinew_CEC_2005[i, k] + oiold_CEC_2005[i, k]) / lambda[i - 1];
                    x2[k] = 0.0D;
                }
                multiplica_vector_por_matriz_CEC_2005(i, D, x1, ref x2);
                switch (i)
                {
                    case 1:
                    case 2:
                        fit[i] = Function( x2, D, tipo_funciones.RASTRIGIN);
                        break;
                    case 3:
                    case 4:
                        fit[i] = Function( x2, D, tipo_funciones.WEIERSTRASS);
                        break;
                    case 5:
                    case 6:
                        fit[i] = Function( x2, D, tipo_funciones.GRIEWANK);
                        break;
                    case 7:
                    case 8:
                        fit[i] = Function( x2, D, tipo_funciones.ACKLEY_1);
                        break;
                    case 9:
                    case 10:
                        fit[i] = Function( x2, D, tipo_funciones.SPHERE);
                        break;
                }
                for (int k = 1; k <= D; k++)
                {
                    x1[k] = (z_compuesta_CEC_2005[i] / lambda[i - 1]);
                    x2[k] = 0.0D;
                }
                multiplica_vector_por_matriz_CEC_2005(i, D, x1, ref x2);
                switch (i)
                {
                    case 1:
                    case 2:
                        fmax[i] = Function( x2, D, tipo_funciones.RASTRIGIN);
                        break;
                    case 3:
                    case 4:
                        fmax[i] = Function( x2, D, tipo_funciones.WEIERSTRASS);
                        break;
                    case 5:
                    case 6:
                        fmax[i] = Function( x2, D, tipo_funciones.GRIEWANK);
                        break;
                    case 7:
                    case 8:
                        fmax[i] = Function( x2, D, tipo_funciones.ACKLEY_1);
                        break;
                    case 9:
                    case 10:
                        fmax[i] = Function( x2, D, tipo_funciones.SPHERE);
                        break;
                }
                f[i] = C * fit[i] / fmax[i];
            }
            SumW = w[1];
            MaxW = w[1];
            for (int i = 2; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                SumW += w[i];
                if (w[i] > MaxW)
                    MaxW = w[i];
            }
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
            {
                if (w[i] != MaxW)
                    w[i] = w[i] * (1 - Math.Pow(MaxW, 10.0D));
                w[i] /= SumW;
            }
            double valor = 0.0D;
            for (int i = 1; i <= MAX_FUNCIONES_COMPUESTAS; i++)
                valor += w[i] * (f[i] + f_bias_CEC_2005[i]);
            cantidad_de_veces_que_se_evalua_la_funcion -= 19;
            return (valor);
        }

        public double MICHALEWICZ(double[] position, int dimensions)
        {
            double result = 0.0D;
            for (int d = 1; d <= dimensions; d++)
                result += Math.Sin(position[d]) * Math.Pow(Math.Sin((d * Math.Pow(position[d], 2.0D)) / Math.PI), 20.0D);
            return (-result);
        }

        public double XIN_SHE_YANG_1(double[] position, int dimensions)
        {
            //Buena
            double sum1 = 0.0D;
            double sum2 = 0.0D;
            double prod1 = 1.0D;
            for (int d = 1; d <= dimensions; d++)
                sum1 += Math.Pow(position[d] / 15.0D, 10.0D);
            for (int d = 1; d <= dimensions; d++)
                sum2 += Math.Pow(position[d], 2.0D);
            for (int d = 1; d <= dimensions; d++)
                prod1 *= Math.Pow(Math.Cos(position[d]), 2.0D);
            return ((Math.Exp(-sum1) - 2.0D * Math.Exp(-sum2)) * prod1);
        }

        public double XIN_SHE_YANG_2(double[] position, int dimensions)
        {
            double sum1 = 0.0D;
            double sum2 = 0.0D;
            for (int d = 1; d <= dimensions; d++)
                sum1 += Math.Abs(position[d]);
            for (int d = 1; d <= dimensions; d++)
                sum2 += Math.Pow(position[d], 2.0D);
            return (sum1 * Math.Exp(-sum2));
        }

        public double HANSEN(double[] position, int dimensions)
        {

            double sum1 = 0.0D;
            double sum2 = 0.0D;
            for (int i = 0; i <= 4; i++)
                sum1 += (i + 1) * Math.Cos(i * position[1] + i + 1.0D);
            for (int j = 0; j <= 4; j++)
                sum2 += (j + 1) * Math.Cos((j + 2) * position[2] + j + 1.0D);//-7.58989583,-7.70831466
            return (sum1 * sum2);

        }

        public double XIN_SHE_YANG_3(double[] position, int dimensions)
        {
            double sum1 = 0.0D;
            double sum2 = 0.0D;
            for (int d = 1; d <= dimensions; d++)
                sum1 += Math.Abs(position[d]);
            for (int d = 1; d <= dimensions; d++)
                sum2 += Math.Sin(Math.Pow(position[d], 2.0D));
            return (sum1 * Math.Exp(-sum2));
        }

        public double XIN_SHE_YANG_4(double[] position, int dimensions)
        {
            double sum1;
            double sum2;
            double sum3 = 0.0D;
            sum1 = Math.Pow(position[1] - Math.PI, 2.0D) + Math.Pow(position[2] - Math.PI, 2.0D);
            sum1 = -5.0D * Math.Exp(-sum1);
            for (int j = 1; j <= 10; j++)
            {
                sum2 = 0.0D;
                for (int k = 1; k <= 10; k++)
                    sum2 += Math.Pow(position[1] - j, 2.0D) + Math.Pow(position[2] - k, 2.0D);
                sum3 += aleatorio.NextDouble() * Math.Exp(-sum2);
            }
            return (sum1 - sum3);
        }

        public double XIN_SHE_YANG_5(double[] position, int dimensions)
        {
            double sum2 = 0.0D;
            for (int d = 1; d <= dimensions; d++)
                sum2 += aleatorio.NextDouble() * Math.Pow(Math.Abs(position[d]), d);
            return (sum2);
        }

        public double XIN_SHE_YANG_6(double[] position, int dimensions)
        {
            double sum1 = 0.0D;
            double sum2 = 0.0D;
            double sum3 = 0.0D;
            for (int d = 1; d <= dimensions; d++)
            {
                sum1 += Math.Pow(Math.Sin(position[d]), 2.0D);
                sum2 += Math.Pow(Math.Sin(Math.Sqrt(Math.Abs(position[d]))), 2.0D);
                sum3 += Math.Pow(position[d], 2.0D);
            }
            return ((sum1 - Math.Exp(-sum3)) * Math.Exp(-sum2));
        }

        public double XIN_SHE_YANG_7(double[] position, int dimensions)
        {
            double sum1 = 0.0D;
            for (int d = 1; d <= dimensions; d++)
                sum1 += aleatorio.NextDouble() * Math.Abs(position[d] - 1.0D / d);
            return (sum1);
        }

        public double XIN_SHE_YANG_8(double[] position, int dimensions)
        {
            double sum1 = 0.0D;
            double sum2 = 0.0D;
            for (int d = 1; d <= dimensions; d++)
            {
                sum1 += aleatorio.NextDouble() * Math.Pow(position[d], 2.0D);
                sum2 += aleatorio.NextDouble() * Math.Pow(Math.Sin(PI_e_2 * dimensions * position[d]), 2.0D);
            }
            return (Math.Abs(1.0D - Math.Exp(-sum1)) + sum2);
        }

        public double BEALE(double[] position, int dimensions)
        {
            double sum1 = Math.Pow(1.5D - position[1] + position[1] * position[2], 2.0D);
            double sum2 = Math.Pow(2.25D - position[1] + position[1] * Math.Pow(position[2], 2), 2.0D);
            double sum3 = Math.Pow(2.625D - position[1] + position[1] * Math.Pow(position[2], 3), 2.0D);
            return (sum1 + sum2 + sum3);
        }

        public double BIRD(double[] position, int dimensions)
        {
            double sum1 = Math.Pow(position[1] - position[2], 2.0D);
            double sum2 = Math.Sin(position[1]) * Math.Exp(Math.Pow(1.0D - Math.Cos(position[2]), 2.0D));
            double sum3 = Math.Cos(position[2]) * Math.Exp(Math.Pow(1.0D - Math.Sin(position[1]), 2.0D));
            return (sum1 + sum2 + sum3);
        }

        public double BOHACHEVSKY_1(double[] position, int dimensions)
        {
            double sum1 = Math.Pow(position[1], 2.0D);
            double sum2 = 2.0D * Math.Pow(position[2], 2.0D);
            double sum3 = -0.3D * Math.Cos(PI_x_3 * position[1]);
            double sum4 = -0.4D * Math.Cos(PI_x_04 * position[2]);
            return (sum1 + sum2 + sum3 + sum4 + 0.7D);
        }

        public double BOHACHEVSKY_2(double[] position, int dimensions)
        {
            double sum1 = Math.Pow(position[1], 2.0D);
            double sum2 = 2.0D * Math.Pow(position[2], 2.0D);
            double sum3 = -0.3D * Math.Cos(PI_x_3 * position[1]);
            double sum4 = Math.Cos(PI_x_04 * position[2]);
            return (sum1 + sum2 + sum3 * sum4 + 0.3D);
        }

        public double BOHACHEVSKY_3(double[] position, int dimensions)
        {
            double sum1 = Math.Pow(position[1], 2.0D);
            double sum2 = 2.0D * Math.Pow(position[2], 2.0D);
            double sum3 = -0.3D * Math.Cos(PI_x_3 * position[1] + PI_x_04 * position[2]);
            return (sum1 + sum2 + sum3 + 0.3D);
        }

        public double BOOTH(double[] position, int dimensions)
        {
            double sum1 = Math.Pow(position[1] + 2.0D * position[2] - 7.0D, 2.0D);
            double sum2 = Math.Pow(2.0D * position[1] + position[2] - 5.0D, 2.0D);
            return (sum1 + sum2);
        }

        public double CHICHINADZE(double[] position, int dimensions)
        {
            double sum1 = Math.Pow(position[1], 2.0D);
            double sum2 = -12.0D * position[1] + 11.0D;
            double sum3 = 10.0D * Math.Cos(PI_e_2 * position[1]);
            double sum4 = 8.0D * Math.Sin(PI_x_5 * position[1] / 2.0D);
            double sum5 = 0.2D * Math.Sqrt(5.0D) / Math.Exp(0.5 * Math.Pow(position[2] - 0.5D, 2.0D));
            double results = sum1 + sum2 + sum3 + sum4 - sum5;
            return (results);
        }

        public double CHUNG_REYNOLDS(double[] position, int dimensions)
        {
            double results = 0.0D;
            for (int i = 1; i <= dimensions; i++)
                results += Math.Pow(position[i], 2.0D);
            return (Math.Pow(results, 2.0D));
        }

        public double COLA(double[] position, int dimensions)
        {
            double[] dis = {    1.27D,
                                 1.69D,1.43D,
                                 2.04D,2.35D,2.43D,
                                 3.09D,3.18D,3.26D,2.85D,
                                 3.20D,3.22D,3.27D,2.88D,1.55D,
                                 2.86D,2.56D,2.58D,2.59D,3.12D,3.06D,
                                 3.17D,3.18D,3.18D,3.12D,1.31D,1.64D,3.00D,
                                 3.21D,3.18D,3.18D,3.17D,1.70D,1.36D,2.95D,1.32D,
                                 2.38D,2.31D,2.42D,1.94D,2.85D,2.81D,2.56D,2.91D,2.97D
                             };
            double sum = 0.0D;
            double temp;
            int k = 1;
            double[] mt = { 0.0D, 0.0D, 0.0D, 0.0D, 0.0D, 0.0D, 0.0D, 0.0D, 0.0D, 0.0D, 0.0D, 0.0D,
                            0.0D, 0.0D, 0.0D, 0.0D, 0.0D, 0.0D, 0.0D, 0.0D };
            for (int i = 4; i < dimensions; i++)
                mt[i] = position[i - 3];
            for (int i = 1; i < 10; i++)
                for (int j = 0; j < i; j++)
                {
                    temp = 0.0D;
                    for (int t = 0; t < 2; t++)
                        temp += Math.Pow(mt[i * 2 + t] - mt[j * 2 + t], 2.0D);
                    sum += Math.Pow(dis[k - 1] - Math.Sqrt(temp), 2.0D);
                    k++;
                }
            return (sum);
        }

        public double BOX_BETTS(double[] position, int dimensions)
        {
            double results = 0.0D;
            for (int i = 1; i <= 10; i++)
            {
                double sum1 = Math.Exp(-0.1D * (i) * position[1]);
                double sum2 = Math.Exp(-0.1D * (i) * position[2]);
                double sum3 = Math.Exp(-0.1D * (i)) - Math.Exp(-(i + 1.0D)) * position[3];
                results += Math.Pow(sum1 - sum2 - sum3, 2.0D);
            }
            return (results);
        }

        public double BRAD(double[] position, int dimensions)
        {
            double v_i, w_i, sum = 0.0D;
            double[] y = {0.14, 0.18, 0.22, 0.25, 0.29,
                            0.32, 0.35, 0.39, 0.37, 0.58,
                            0.73, 0.96, 1.34, 2.10, 4.39 };
            for (int i = 1; i <= 15; i++)
            {
                v_i = 16 - i;
                w_i = Math.Min(i, v_i);
                sum += Math.Pow((y[i - 1] - position[1] - i) / (v_i * position[2] + w_i * position[3]), 2.0D);
            }

            return sum;
        }

        public double COLVILLE(double[] position, int dimensions)
        
        {
            double sum1 = 100.0D * Math.Pow(Math.Pow(position[1], 2.0D) - position[2], 2.0D);
            double sum2 = Math.Pow(1.0D - position[1], 2.0D);
            double sum3 = 90.0D * Math.Pow(position[4] - Math.Pow(position[3], 2.0D), 2.0D);
            double sum4 = Math.Pow(1.0D - position[3], 2.0D);
            double sum5 = 10.1D * (Math.Pow(position[2] - 1.0D, 2.0D) + Math.Pow(position[4] - 1.0D, 2.0D));
            double sum6 = 19.8D * (position[2] - 1.0D) * (position[4] - 1.0D);
            return (sum1 + sum2 + sum3 + sum4 + sum5 + sum6);
        }

        public double CORANA(double[] position, int dimensions)
        
        {
            double sum = 0.0D;
            double z;
            double[] d = { 1.0D, 1000.0D, 10.0D, 100.0D };
            for (int i = 1; i <= 4; i++)
            {
                z = Math.Floor(Math.Abs(position[i] / 0.2D) + 0.49999D) * Math.Sign(position[i]) * 0.2D;
                if (Math.Abs(position[i] - z) < 0.05D)
                    sum += 0.15D * Math.Pow(z - 0.05D * Math.Sign(z), 2.0D) * d[i - 1];
                else
                    sum += d[i - 1] * Math.Pow(position[i], 2.0D);
            };
            return (sum);
        }

        public double COSINE_MIXTURE_2(double[] position, int dimensions)

        {
            double sum1 = 0.0D;
            double sum2 = 0.0D;

            for (int i = 1; i <= dimensions; i++)
            {
                sum1 += Math.Cos(PI_x_5 * position[i]);
                sum2 += Math.Pow(position[i], 2.0D);

            };
            return (-0.1D * sum1 + sum2);
        }

        public double COSINE_MIXTURE_4(double[] position, int dimensions)
        {
            double sum1 = 0.0D;
            double sum2 = 0.0D;

            for (int i = 1; i <= dimensions; i++)
            {
                sum1 += Math.Cos(PI_x_5 * position[i]);
                sum2 += Math.Pow(position[i], 2.0D);

            };
            return (-0.1D * sum1 + sum2);
        }

        public double CROSS_IN_TRAY(double[] position, int dimensions)
        {
            return -0.0001 * Math.Pow(Math.Abs(Math.Sin(position[1]) * Math.Sin(position[2]) *
                Math.Exp(Math.Abs(100.0D - Math.Pow(Math.Pow(position[1], 2) + Math.Pow(position[2], 2), 0.5) / Math.PI))) + 1.0D, 0.1);
        }

        public double CSENDES(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int i = 1; i <= dimensions; i++)
                if (position[i] != 0)
                    sum += Math.Pow(position[i], 6.0) * (2.0D + Math.Sin(1.0D / position[i])); 
            return sum;
        }

        public double CUBE(double[] position, int dimensions)
        {
            return 100.0D * Math.Pow(position[2] - Math.Pow(position[1], 3.0D), 2.0D) + Math.Pow(1.0D - position[1], 2.0D);
        }

        public double DAMAVANDI(double[] position, int dimensions)
        {
            if (position[1] == 2 && position[2] == 2)
                return 0;

            return (1.0D - Math.Pow(Math.Abs((Math.Sin(Math.PI * (position[1] - 2.0D)) * Math.Sin(Math.PI * (position[2] - 2.0D)))
                / (Math.Pow(Math.PI, 2.0D) * (position[1] - 2.0D) * (position[2] - 2.0D))), 5.0D)) *
                (2.0D + Math.Pow(position[1] - 7.0D, 2.0D) + 2.0D * Math.Pow(position[2] - 7, 2.0D));
        }

        public double DEB_1(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int i = 1; i <= dimensions; i++)
                sum += Math.Pow(Math.Sin(5.0D * Math.PI * position[i]), 6.0D);
            return - sum / dimensions;
        }

        public double DEB_3(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int i = 1; i <= dimensions; i++)
                sum += Math.Pow(Math.Sin(5.0D * Math.PI * (Math.Pow(position[i], 3.0D / 4.0D) - 0.05D)), 6.0D);
            return - sum / dimensions;
        }

        public double DECKKERS_AARTS(double[] position, int dimensions)
        {
            return Math.Pow(10.0D, 5.0D) * Math.Pow(position[1], 2.0D) + Math.Pow(position[2], 2.0D) - Math.Pow(Math.Pow(position[1], 2.0D) + Math.Pow(position[2], 2.0D), 2.0D)
                + Math.Pow(10.0D, -5.0D) * Math.Pow(Math.Pow(position[1], 2.0D) + Math.Pow(position[2], 2.0D), 4.0D);
        }

        public double DE_VILLIERS_GLASSER_1(double[] position, int dimensions)
        {
            double t_i, y_i, sum = 0.0D;
            for (int i = 1; i <= 24; i++)
            {
                t_i = 0.1D * (i - 1.0D);
                y_i = 60.137D * Math.Pow(1.371D, t_i) * Math.Sin(3.112D * t_i + 1.761D);
                sum += Math.Pow(position[1] * Math.Pow(position[2], t_i) * Math.Sin(position[3] * t_i + position[4]) - y_i, 2.0D);
            }
            return sum;
        }

        public double DE_VILLIERS_GLASSER_2(double[] position, int dimensions)
        {
            double t_i, y_i, sum = 0.0D;
            for (int i = 1; i <= 16; i++)
            {
                t_i = 0.1D * (i - 1.0D);
                y_i = 53.81D * Math.Pow(1.27D, t_i) * Math.Tanh(3.012D * t_i + Math.Sin(2.13D * t_i)) * Math.Cos(Math.Exp(0.507D) * t_i);
                sum += Math.Pow(position[1] * Math.Pow(position[2], t_i) * Math.Tanh(position[3] * t_i + Math.Sin(position[4] * t_i)) * Math.Cos(t_i * Math.Exp(position[5])) - y_i, 2.0D);
            }
            return sum;
        }

        public double DIXON_PRICE(double[] position, int dimensions)
        {
            double sum = Math.Pow(position[1] - 1.0D, 2);
            for (int d = 2; d <= dimensions; d++)
            {
                sum += d * Math.Pow(2.0D * Math.Pow(position[d], 2) - position[d - 1], 2);
            }
            return sum;
        }

        public double DOLAN(double[] position, int dimensions)
        {
            return (position[1] + 1.7D * position[2]) * Math.Sin(position[1]) - 1.5D * position[3] - 0.1D * position[4] * Math.Cos(position[4] + position[5] - position[1])
                + 0.2D * Math.Pow(position[5], 2.0D) - position[2] - 1.0D;
        }

        public double EASOM(double[] position, int dimensions)

        {
            return -Math.Cos(position[1]) * Math.Cos(position[2]) *
                    Math.Exp(-Math.Pow(position[1] - Math.PI, 2.0D) -
                    Math.Pow(position[2] - Math.PI, 2.0D));
                    
        }

        public double EXP_2(double[] position, int dimensions)
        {

            double sum = 0.0D;
            double t;
            for (int i = 0; i <= 9; i++)
            {
                t = Math.Exp(-i * position[1] / 10.0D) - 5.0D * Math.Exp(-i * position[2] * 10.0D) - Math.Exp(-i / 10.0D) + 5.0D * Math.Exp(-i);
                sum += Math.Pow(t, 2.0D);
            }
            return (sum);
        }

        public double FREUDENSTEIN_ROTH(double[] position, int dimensions)
        {
            return Math.Pow(position[1] - 13.0D + ((5.0D - position[2]) * position[2] - 2.0D) * position[2], 2.0D) + Math.Pow(position[1] - 29.0D + ((1.0D + position[2]) * position[2] - 14.0D) * position[2], 2.0D);
        }

        public double GIUNTA(double[] position, int dimensions)
        {
            double sum = 0.6D;
            for (int i = 1; i <= 2; i++)
            {
                sum += Math.Pow(Math.Sin(1.0D - 16.0D * position[i] / 15.0D), 2.0D) - 0.02D * Math.Sin(4.0D - 64.0D * position[i] / 15.0D) - Math.Sin(1.0D - 16.0D * position[i] / 15.0D);
            }
            return sum;
        }

        public double EGG_HOLDER(double[] position, int dimensions)
        {
            double sum1 = 0.0D;
            double sum2 = 0.0D;
            double result = 0.0D;

            for (int i = 1; i <= dimensions - 1; i++)
            {
                sum1 += -(position[i + 1] + 47.0D) * Math.Sin(Math.Sqrt(Math.Abs(position[i + 1] + position[i] * 0.5D + 47.0D)));
                sum2 += -position[i] * Math.Sin(Math.Sqrt(Math.Abs(position[i] - position[i + 1] - 47.0D)));

                result += (sum1 + sum2);
            }
            return (result);
        }

        public double EL_ATTAR_VIDYASAGAR_DUTTA(double[] position, int dimensions)
        {
            return Math.Pow(Math.Pow(position[1], 2.0D) + position[2] - 10.0D, 2.0D) + Math.Pow(position[1] + Math.Pow(position[2], 2.0D) - 7.0D, 2.0D) + Math.Pow(Math.Pow(position[1], 2.0D) + Math.Pow(position[2], 3.0D) - 1.0D, 2.0D);
        }

        public double EXPONENTIAL(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int i = 1; i <= dimensions; i++)
            {
                sum += Math.Pow(position[i], 2.0D);
            }
            return -Math.Exp(-0.5D * sum);
        }

        public double EGG_CRATE(double[] position, int dimensions)
        {
            double sum1;
            double sum2;


            sum1 = Math.Pow(position[1], 2.0D) + Math.Pow(position[2], 2.0D);
            sum2 = 25.0D * (Math.Pow(Math.Sin(position[1]), 2.0D) + Math.Pow(Math.Sin(position[2]), 2.0D));

            return (sum1 + sum2);
        }

        public double BIGGS_EXP2(double[] position, int dimensions)
        
        {
            double sum1 = 0.0D;
            double sum2 = 0.0D;
            double t;
            double y;
            for (int i = 1; i <= 10; i++)
            {
                t = 0.1D * i;
                y = Math.Exp(-t) - 5.0D * Math.Exp(-10.0D * t);
                sum1 = Math.Exp(-t * position[1]) - 5.0D * Math.Exp(-t * position[2]) - y;
                sum2 += Math.Pow(sum1, 2.0D);
            }
            return (sum2);
        }

        public double BIGGS_EXP3(double[] position, int dimensions)
       
        {
            double sum1;
            double sum2 = 0.0D;
            double t;
            double y;
            for (int i = 1; i <= 10; i++)
            {
                t = 0.1D * i;
                y = Math.Exp(-t) - 5.0D * Math.Exp(-10.0D * t);
                sum1 = Math.Exp(-t * position[1]) - position[3] * Math.Exp(-t * position[2]) - y;
                sum2 += Math.Pow(sum1, 2.0D);
            }
            return (sum2);
        }

        public double BIGGS_EXP4(double[] position, int dimensions)
        {
            double sum1;
            double sum2 = 0.0D;
            double t;
            double y;
            for (int i = 1; i <= 10; i++)
            {
                t = 0.1D * i;
                y = Math.Exp(-t) - 5.0D * Math.Exp(-10.0D * t);
                sum1 = position[3] * Math.Exp(-t * position[1]) - position[4] * Math.Exp(-t * position[2]) - y;
                sum2 += Math.Pow(sum1, 2.0D);
            }
            return (sum2);
        }

        public double BIGGS_EXP5(double[] position, int dimensions)
        {
            double sum1;
            double sum2 = 0.0D;
            double t;
            double y;
            for (int i = 1; i <= 11; i++)
            {
                t = 0.1D * i;
                y = Math.Exp(-t) - 5.0D * Math.Exp(-10.0D * t) + 3.0D * Math.Exp(-4.0D * t);
                sum1 = position[3] * Math.Exp(-t * position[1]) - position[4] * Math.Exp(-t * position[2])
                       + 3.0D * Math.Exp(-t * position[5]) - y;
                sum2 += Math.Pow(sum1, 2.0D);
            }
            return (sum2);
        }

        public double BIGGS_EXP6(double[] position, int dimensions)
        {
            double sum1;
            double sum2 = 0.0D;
            double t;
            double y;
            for (int i = 1; i <= 13; i++)
            {
                t = 0.1D * i;
                y = Math.Exp(-t) - 5.0D * Math.Exp(-10.0D * t) + 3.0D * Math.Exp(-4.0D * t);
                sum1 = position[3] * Math.Exp(-t * position[1]) - position[4] * Math.Exp(-t * position[2])
                       + position[6] * Math.Exp(-t * position[5]) - y;
                sum2 += Math.Pow(sum1, 2.0D);
            }
            return (sum2);
        }

        public double GEAR(double[] position, int dimensions)
        {
            double t;
            t = 1.0D / 6.931D
                - Math.Floor(position[1]) * Math.Floor(position[2]) / (Math.Floor(position[3]) * Math.Floor(position[4]));
            return (Math.Pow(t, 2.0D));
        }

        public double HIMMELBLAU(double[] position, int dimensions)
        {
            return Math.Pow(Math.Pow(position[1], 2.0D) + position[2] - 11.0D, 2.0D)
                + Math.Pow(position[1] + Math.Pow(position[2], 2.0D) - 7.0D, 2.0D);
        }

        public double HOLZMAN_1(double[] position, int dimensions)

        {
            double sum = 0.0D;
            double ui;
            double sum1;
            double sum2;
            for (int i = 0; i <= 99; i++)
            {
                ui = 25.0D + Math.Pow(-50.0D * Math.Log(0.01D * (i + 1.0D)), 2.0D / 3.0D);
                sum2 = ui - position[2];
                sum1 = Math.Pow(sum2, position[3]) / position[1];
                if (double.IsNaN(sum1))
                    sum1 = 0.0D;
                sum += -0.1D * (i + 1.0D) + Math.Exp(sum1);
            }
            return (sum);
        }

        public double LAGERMAN(double[] position, int dimensions)
        {
            double[] c = { 1.0D, 2.0D, 5.0D, 2.0D, 3.0D };
            double[,] A = { { 3.0D, 5.0D }, { 5.0D, 2.0D }, { 2.0D, 1.0D }, { 1.0D, 4.0D }, { 7.0D, 9.0D } };
            double sum = 0.0D;
            double dist;
            for (int i = 1; i <= 5; i++)
            {
                dist = 0.0D;
                for (int j = 1; j <= dimensions; j++)
                    dist += Math.Pow(position[i] - A[i - 1, j - 1], 2.0D);
                sum += c[i - 1] * (Math.Exp(-dist / Math.PI) * Math.Cos(Math.PI * dist));
            }
            return (sum);
        }
        
        public double HOLZMAN_2(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int i = 1; i <= dimensions; i++)
                sum += i * Math.Pow(position[i], 4.0D);
            return (sum);
        }

        public double HOSAKI(double[] position, int dimensions)
        {
            return (1.0D - 8.0D * position[1] + 7.0D * Math.Pow(position[1], 2.0D)
                    - 7.0D / 3.0D * Math.Pow(position[1], 3.0D) + 1.0D / 4.0D * Math.Pow(position[1], 4.0D)) *
                    Math.Pow(position[2], 2.0D) * Math.Exp(-position[2]);
                    
        }

        public double KATSUURAS(double[] position, int dimensions)
        {
            int d = 32;
            double prod = 1.0D;
            double s;
            double pow2;
            for (int i = 1; i <= dimensions; i++)
            {
                s = 0.0D;
                for (int k = 1; k <= d; k++)
                {
                    pow2 = Math.Pow(2.0D, k);
                    s += Math.Round(pow2 * position[i]) / pow2;
                }
                prod *= 1.0D + (i + 1.0D) * s;
            }
            return (prod);
        }

        public double TRID(double[] position, int dimensions)
        {
            double prod = 0.0D;
            double suma = 0.0D;
            for (int i = 1; i <= dimensions; i++)
                suma += Math.Pow(position[i] - 1.0D, 2.0D);
            for (int i = 2; i <= dimensions; i++)
                prod += position[i] * position[i-1];

            return (suma - prod);
        }

        public double LEON(double[] position, int dimensions)
        {
            return 100.0D * Math.Pow(position[2]
                   - Math.Pow(position[1], 3.0D), 2.0D)
                    + Math.Pow(position[1] - 1.0D, 2.0D);
                    
        }

        public double LEVY_8(double[] position, int dimensions)
        {
            double yi = 1.0D + ((position[1] - 1.0D) / 4.0D);
            double result = Math.Pow(Math.Sin(Math.PI * yi), 2.0D);
            double yd = 1.0D + ((position[dimensions] - 1.0D) / 4.0D);
            for (int i = 1; i < dimensions; i++)
            {
                yi = 1.0D + ((position[i] - 1.0D) / 4.0D);
                result += (Math.Pow(yi - 1.0D, 2.0D) * (1.0D + (10.0D * Math.Pow(Math.Sin(Math.PI * yi + 1.0D), 2.0D)))) +
                            (Math.Pow(yd - 1.0D, 2.0D) * (1.0D + Math.Pow(Math.Sin(2.0D * Math.PI * yd), 2.0D)));
            }
            return (result);
        }

        public double MATYAS(double[] position, int dimensions)
        {
            return 0.26D * (Math.Pow(position[1], 2.0D) + Math.Pow(position[2], 2.0D))
                    - 0.48D * position[1] * position[2];
                    
        }

        public double MC_CORMICK(double[] position, int dimensions)
        {
            return Math.Sin(position[1] + position[2]) + Math.Pow(position[1] - position[2], 2.0D)
                    - (1.5D * position[1]) + (2.5D * position[2]) + 1.0D;
                    
        }

        public double NEUMAIER_PERM(double[] position, int dimensions)
        {
            //Suggested values for testing (dimensions,betta) are (4,50),(4,0.5),(10,10^9),(10,10^7)
            int betta = 50;
            double result = 0.0D;
            double sub_result;
            for (int k = 1; k <= dimensions; k++)
            {
                sub_result = 0.0D;
                for (int i = 1; i <= dimensions; i++)
                    sub_result += (Math.Pow(i, k) + betta) *
                                    (Math.Pow(position[i] / i, k) - 1.0D);
                result += Math.Pow(sub_result, 2.0D);
            }
            return (result);
        }

        public double NEUMAIER_PERM_0(double[] position, int dimensions)
        {
            //Suggested values for (dimensions,betta) are (4,10),(10,100)
            int betta = 10;
            double sum_k = 0.0D;
            double sum_i;
            for (int k = 1; k <= dimensions; k++)
            {
                sum_i = 0.0D;
                for (int i = 1; i <= dimensions; i++)
                    sum_i += (i + betta) *
                                (Math.Pow(position[i], k) - Math.Pow(1.0D / i, k));
                sum_k += Math.Pow(sum_i, 2.0D);
            }
            return (sum_k);
        }

        public double PARSOPOULOS(double[] position, int dimensions)
        {
            return Math.Pow(Math.Cos(position[1]), 2.0D) + Math.Pow(Math.Sin(position[2]), 2.0D);
        }

        public double PATHOLOGICAL(double [] position, int dimensions)
        {
            double sum = 0.0D;
            for (int d = 1; d < dimensions; d++)
            {
                sum += 0.5D;
                sum += (Math.Pow(Math.Sin(Math.Sqrt(100.0D * Math.Pow(position[d], 2.0D) + Math.Pow(position[d + 1], 2.0D))), 2.0D) - 0.5D) /
                        (1.0D + (0.001D * Math.Pow(Math.Pow(position[d], 2.0D) - (2.0D * position[d] * position[d + 1]) + Math.Pow(position[d + 1], 2.0D), 2.0D)));
            }
            return (sum);
        }

        public double PAVIANI(double[] position, int dimensions)
        {
            double sum = 0.0D;
            double prod = 1.0D;
            for (int d = 1; d <= dimensions; d++)
            {
                sum += Math.Pow(Math.Log(10.0D - position[d]), 2.0D) + Math.Pow(Math.Log(position[d] - 2.0D), 2.0D);
                prod *= position[d];
            }
            return (sum - Math.Pow(prod, 0.2D));
        }

        public double PEN_HOLDER(double[] position, int dimensions)
        {
            double result = -Math.Exp(- 1.0D / Math.Abs(Math.Cos(position[1]) * Math.Cos(position[2]) * 
                             Math.Exp(Math.Abs(1.0D - (Math.Sqrt(Math.Pow(position[1], 2.0D) 
                             + Math.Pow(position[2], 2.0D)) / Math.PI)))));
            return (result);
        }

        public double PINTER(double[] position, int dimensions)
        {
            double A, B, sum1 = 0.0D, sum2 = 0.0D, sum3 = 0.0D;
            for (int i = 1; i <= dimensions; i++)
            {
                if (i == 1)
                {
                    A = position[i] * Math.Sin(position[i]) + Math.Sin(position[i + 1]);
                    B = Math.Pow(position[i], 2.0D) - 2.0D * position[i] + 3.0D * position[i + 1] - Math.Cos(position[i]) + 1.0D;
                    sum1 = Math.Pow(position[i], 2.0D);
                    sum2 = 20.0D * Math.Pow(Math.Sin(A), 2.0D);
                    sum3 = Math.Log10(1.0D + Math.Pow(B, 2.0D));
                }
                else if (i == dimensions)
                {
                    A = position[i - 1] * Math.Sin(position[i]) + Math.Sin(position[1]);
                    B = Math.Pow(position[i - 1], 2.0D) - 2.0D * position[i] + 3.0D * position[1] - Math.Cos(position[i]) + 1.0D;
                    sum1 += i * Math.Pow(position[i], 2.0D);
                    sum2 += 20.0D * i * Math.Pow(Math.Sin(A), 2.0D);
                    sum3 += i * Math.Log10(1.0D + i * Math.Pow(B, 2.0D));
                }
                else
                {
                    A = position[i - 1] * Math.Sin(position[i]) + Math.Sin(position[i + 1]);
                    B = Math.Pow(position[i - 1], 2.0D) - 2.0D * position[i] + 3.0D * position[i + 1] - Math.Cos(position[i]) + 1.0D;
                    sum1 += i * Math.Pow(position[i], 2.0D);
                    sum2 += 20.0D * i * Math.Pow(Math.Sin(A), 2.0D);
                    sum3 += i * Math.Log10(1.0D + i * Math.Pow(B, 2.0D));
                }
                    
            }

            return sum1 + sum2 + sum3;
        }

        public double POWELL(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int d = 1; d <= dimensions / 4; d++)
            {
                sum += Math.Pow(position[4 * d - 3] + 10.0D * position[4 * d - 2], 2);
                sum += 5.0D * Math.Pow(position[4 * d - 1] - position[4 * d], 2);
                sum += Math.Pow(position[4 * d - 2] - 2.0D * position[4 * d - 1], 4);
                sum += 10.0D * Math.Pow(position[4 * d - 3] - position[4 * d], 4);
            }
            return sum;
        }

        public double POWELL_SINGULAR_2(double[] position, int dimensions)
        {
            double result = 0.0D;
            for (int i = 2; i <= dimensions - 2; i++)
                result += Math.Pow(position[i - 1] + 10.0D * position[i], 2.0D) + 5.0D * Math.Pow(position[i + 1] - position[i + 2], 2.0D) + Math.Pow(position[i] - 2.0D * position[i + 1], 4.0D) + 10.0D * Math.Pow(position[i - 1] - position[i + 2], 4.0D);
            
            return result;
        }

        public double POWELL_SUM(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int i = 1; i <= dimensions; i++)
            {
                sum += Math.Pow(Math.Abs(position[i]), i + 1);
            }
            return sum;
        }

        public double PRICE_1(double[] position, int dimensions)
        {
            return Math.Pow(Math.Abs(position[1]) - 5.0D, 2.0D) + Math.Pow(Math.Abs(position[2]) - 5.0D, 2.0D);
        }

        public double PRICE_2(double[] position, int dimensions)
        {
            return 1.0D + Math.Pow(Math.Sin(position[1]), 2.0D) + Math.Pow(Math.Sin(position[2]), 2.0D) - 0.1D * Math.Exp(-Math.Pow(position[1], 2.0D) - Math.Pow(position[2], 2.0D));
        }

        public double PRICE_4(double[] position, int dimensions)
        {
            return 100.0D * Math.Pow(position[2] - Math.Pow(position[1], 2.0D), 2.0D) + Math.Pow(6.4D * Math.Pow(position[2] - 0.5D, 2.0D) - position[1] - 0.6D, 2.0D);
        }

        public double QING(double[] position, int dimensions)
        {
            double sum = 0.0D;
            for (int i = 1; i <= dimensions; i++)
                sum += Math.Pow(Math.Pow(position[i], 2.0D) - i, 2.0D);
            return sum;
        }

        public double QUADRATIC(double[] position, int dimensions)
        {
            return -3803.84 - 138.08 * position[1] - 232.92 * position[2] + 128.08 * Math.Pow(position[1], 2.0D) + 203.64 * Math.Pow(position[2], 2.0D) + 182.25 * position[1] * position[2];
        }

        public double ZIRILLI(double[] position, int dimensions)
        {
            double result = 0.25D * Math.Pow(position[1],4.0D) - 0.5D * Math.Pow(position[1], 2.0D)
                            + 0.1D * position[1] + 0.5D * Math.Pow(position[2], 2.0D);
            return (result);
        }

        public double ZETTL(double[] position, int dimensions)
        {
            double result1 = 0.25D * position[1];
            double result2 = Math.Pow(Math.Pow(position[1], 2.0D) - 2.0D * position[1] + Math.Pow(position[2], 2.0D), 2.0D);
            return (result1 + result2);
        }

        public double ZEROSUM(double[] position, int dimensions)
        {
            double suma = 0.0D;
            for (int i = 1; i <= dimensions; i++)
                suma += position[i];
            if (suma == 0)
                return (0.0D);
            return (1.0D + Math.Sqrt(10000.0D * Math.Abs(suma)));
        }

        

        public void inicia_oraculo(Funciones.tipo_funciones funcion)
        {
            /*switch (funcion)
            {
                case tipo_funciones.PROBLEMA_TENSION_COMPRESSION_SPRING:
                    oraculo_actual = 0.0127420;
                    break;
                case tipo_funciones.FUNCION_RESTRICCIONES_G1_CEC_2006:
                    oraculo_actual = -15.0;
                    break;
            }*/
            
        }

        

        public double calcula_penalty(double valor_funcion, double oraculo, double res)
        {
            double constante = (6.0D * Math.Sqrt(3.0D) - 2.0D) / (6.0D * Math.Sqrt(3.0D));
            double alfa=0.0D;
            double penalty=0.0;
            double g0 = Math.Abs(valor_funcion - oraculo);
            if (Math.Abs(valor_funcion) <= Math.Abs(oraculo))
                alfa = 0.0;
            else
            {
                if (res <= (g0 / 3.0D))
                    alfa = (g0 * constante - res) / (g0 - res);
                else if ((g0 / 3.0D) <= res && res <= g0)
                    alfa = 1.0D - (1.0D / (2.0D * Math.Sqrt(g0 / res)));
                else if (res > g0)
                     alfa = 0.5D * Math.Sqrt(g0 / res);
            }
            if (valor_funcion > oraculo || res > 0.0D)
                penalty = alfa * Math.Abs(g0) + (1.0D - alfa) * res;
            else if (valor_funcion <= oraculo && res == 0.0D)
                    penalty = -g0;
            return penalty;
        }

        public double max_de_condiciones(double [] g, int cantidad)
        {
            double max= Math.Abs(Math.Min(0, g[1]));
            for (int i = 2; i <= cantidad; i++)
                max = Math.Max(max, Math.Abs(Math.Min(0, g[i])));
            return max;
        }

        public double PROBLEMA_TENSION_COMPRESSION_SPRING(double[] position, int dimensions)
        {
            double [] g=new double[5];
            double funcion = (position[3] + 2.0D) * position[2] * Math.Pow(position[1], 2.0D);
            g[1] = -1.0D + (Math.Pow(position[2], 3.0D) * position[3]) / (71785.0D * Math.Pow(position[1], 4.0D));
            double termino1 = 4.0D * Math.Pow(position[2], 2.0D) - position[1] * position[2];
            double termino2 = 12566.0D * (position[2] * Math.Pow(position[1], 3.0D) - Math.Pow(position[1], 4.0D));
            g[2] = -termino1 / termino2 - (1.0D / 5108.0D / Math.Pow(position[1], 2.0D)) + 1.0D;
            g[3] = -1.0D + (140.45D * position[1] / Math.Pow(position[2], 2.0D) / position[3]);
            g[4] = -((position[1] + position[2]) / 1.5D) + 1.0D;
            double res = max_de_condiciones(g, 4);
            double penalty = calcula_penalty(funcion, oraculo_actual, res);
            return funcion + penalty;
        }

        public double FUNCION_RESTRICCIONES_G1_CEC_2006(double[] position, int dimensions)
        {
            double funcion;
            double suma = 0.0D;
            double[] g = new double[10];
            for (int i = 1; i <= 4; i++)
                suma += position[i];
            funcion = 5.0D * suma;
            suma = 0.0D;
            for (int i = 1; i <= 4; i++)
                suma += Math.Pow(position[i],2.0D);
            funcion -= 5.0D * suma;
            suma = 0.0D;
            for (int i = 5; i <= 13; i++)
                suma += position[i];
            funcion -= suma;
            g[1] = 10.0D - 2.0D * position[1] - 2.0D * position[2] - position[10] - position[11];
            g[2] = 10.0D - 2.0D * position[1] - 2.0D * position[3] - position[10] - position[12];
            g[3] = 10.0D - 2.0D * position[2] - 2.0D * position[3] - position[11] - position[12];
            g[4] = 8.0D * position[1] - position[10];
            g[5] = 8.0D * position[2] - position[11];
            g[6] = 8.0D * position[3] - position[12];
            g[7] = 2.0D * position[4] + position[5] - position[10];
            g[8] = 2.0D * position[6] + position[7] - position[11];
            g[9] = 2.0D * position[8] + position[9] - position[12];
            double res = max_de_condiciones(g, 9);
            double penalty = calcula_penalty(funcion, oraculo_actual, res);
            return funcion + penalty;
        }

        public double ZACHAROV(double[] position, int dimensions)
        {
            double suma_cuadrados = 0.0D;
            double suma_por_i = 0.0D;
            for (int i = 1; i <= dimensions; i++)
            {
                suma_cuadrados += Math.Pow(position[i], 2.0D);
                suma_por_i += position[i] * i;
            }
            return suma_cuadrados + Math.Pow(0.5D * suma_por_i, 2.0D)
                   + Math.Pow(0.5D * suma_por_i, 4.0D);
        }

        public double YAOLIU09(double[] position, int dimensions)
        {
            double suma = 0.0D;
            for (int i = 1; i <= dimensions; i++)
                suma += Math.Pow(position[i], 2.0D) - 10.0D * Math.Cos(PI_x_2 * position[i]) + 10.0D;
            return (suma);
        }

        public double YAOLIU04(double[] position, int dimensions)
        { 
                double mayor = double.MinValue;
                for (int i = 1; i <= dimensions; i++)
                    if (mayor < Math.Abs(position[i]))
                        mayor = Math.Abs(position[i]);
                return (mayor);
        }


        public double Function(double[] position, int dimensions, tipo_funciones number_of_function)
        {
            double result;
            cantidad_de_veces_que_se_evalua_la_funcion++;
            switch (number_of_function)
            {
                case tipo_funciones.HIMMELBLAU:
                    result = HIMMELBLAU(position, dimensions);
                    break;
                case tipo_funciones.HOLZMAN_1:
                    result = HOLZMAN_1(position, dimensions);
                    break;
                case tipo_funciones.LAGERMAN:
                    result = LAGERMAN(position, dimensions);
                    break;
                case tipo_funciones.HOLZMAN_2:
                    result = HOLZMAN_2(position, dimensions);
                    break;
                case tipo_funciones.HOSAKI:
                    result = HOSAKI(position, dimensions);
                    break;
                case tipo_funciones.KATSUURAS:
                    result = KATSUURAS(position, dimensions);
                    break;
                case tipo_funciones.EASOM:
                    result = EASOM(position, dimensions);
                    break;
                case tipo_funciones.HANSEN:
                    result = HANSEN(position, dimensions);
                    break;
                case tipo_funciones.EGG_HOLDER:
                    result = EGG_HOLDER(position, dimensions);
                    break;
                case tipo_funciones.EL_ATTAR_VIDYASAGAR_DUTTA:
                    result = EL_ATTAR_VIDYASAGAR_DUTTA(position, dimensions);
                    break;
                case tipo_funciones.EXPONENTIAL:
                    result = EXPONENTIAL(position, dimensions);
                    break;
                case tipo_funciones.EXP_2:
                    result = EXP_2(position, dimensions);
                    break;
                case tipo_funciones.FREUDENSTEIN_ROTH:
                    result = FREUDENSTEIN_ROTH(position, dimensions);
                    break;
                case tipo_funciones.GIUNTA:
                    result = GIUNTA(position, dimensions);
                    break;
                case tipo_funciones.EGG_CRATE:
                    result = EGG_CRATE(position, dimensions);
                    break;
                case tipo_funciones.GEAR:
                    result = GEAR(position, dimensions);
                    break;
                case tipo_funciones.COLVILLE:
                    result = COLVILLE(position, dimensions);
                    break;
                case tipo_funciones.CORANA:
                    result = CORANA(position, dimensions);
                    break;
                case tipo_funciones.COSINE_MIXTURE_2:
                    result = COSINE_MIXTURE_2(position, dimensions);
                    break;
                case tipo_funciones.COSINE_MIXTURE_4:
                    result = COSINE_MIXTURE_4(position, dimensions);
                    break;
                case tipo_funciones.CROSS_IN_TRAY:
                    result = CROSS_IN_TRAY(position, dimensions);
                    break;
                case tipo_funciones.CSENDES:
                    result = CSENDES(position, dimensions);
                    break;
                case tipo_funciones.CUBE:
                    result = CUBE(position, dimensions);
                    break;
                case tipo_funciones.DAMAVANDI:
                    result = DAMAVANDI(position, dimensions);
                    break;
                case tipo_funciones.DEB_1:
                    result = DEB_1(position, dimensions);
                    break;
                case tipo_funciones.DEB_3:
                    result = DEB_3(position, dimensions);
                    break;
                case tipo_funciones.DECKKERS_AARTS:
                    result = DECKKERS_AARTS(position, dimensions);
                    break;
                case tipo_funciones.DE_VILLIERS_GLASSER_1:
                    result = DE_VILLIERS_GLASSER_1(position, dimensions);
                    break;
                case tipo_funciones.DE_VILLIERS_GLASSER_2:
                    result = DE_VILLIERS_GLASSER_2(position, dimensions);
                    break;
                case tipo_funciones.DIXON_PRICE:
                    result = DIXON_PRICE(position, dimensions);
                    break;
                case tipo_funciones.CHICHINADZE:
                    result = CHICHINADZE(position, dimensions);
                    break;
                case tipo_funciones.CHUNG_REYNOLDS:
                    result = CHUNG_REYNOLDS(position, dimensions);
                    break;
                case tipo_funciones.BOX_BETTS:
                    result = BOX_BETTS(position, dimensions);
                    break;
                case tipo_funciones.BRAD:
                    result = BRAD(position, dimensions);
                    break;
                case tipo_funciones.BEALE:
                    result = BEALE(position, dimensions);
                    break;
                case tipo_funciones.BIRD:
                    result = BIRD(position, dimensions);
                    break;
                case tipo_funciones.BOOTH:
                    result = BOOTH(position, dimensions);
                    break;
                case tipo_funciones.SPHERE:
                    result = SPHERE(position, dimensions);
                    break;
                case tipo_funciones.SCHWEFEL_222:
                    result = SCHWEFEL_222(position, dimensions);
                    break;
                case tipo_funciones.ROTATED_HYPERELLIPSOID:
                    result = ROTATED_HYPERELLIPSOID(position, dimensions);
                    break;
                case tipo_funciones.RUMP:
                    result = RUMP(position, dimensions);
                    break;
                case tipo_funciones.SALOMON:
                    result = SALOMON(position, dimensions);
                    break;
                case tipo_funciones.HYPERELLIPSOID:
                    result = HYPERELLIPSOID(position, dimensions);
                    break;
                case tipo_funciones.SCHWEFEL_221:
                    result = SCHWEFEL_221(position, dimensions);
                    break;
                case tipo_funciones.ROSENBROCK:
                    result = ROSENBROCK(position, dimensions);
                    break;
                case tipo_funciones.STEP_1:
                    result = STEP_1(position, dimensions);
                    break;
                case tipo_funciones.STEP_2:
                    result = STEP_2(position, dimensions);
                    break;
                case tipo_funciones.STEP_3:
                    result = STEP_3(position, dimensions);
                    break;
                case tipo_funciones.STYBLINSKI_TANG:
                    result = STYBLINSKI_TANG(position, dimensions);
                    break;
                case tipo_funciones.SUM_OF_DIFFERENT_POWERS:
                    result = SUM_OF_DIFFERENT_POWERS(position, dimensions);
                    break;
                case tipo_funciones.SUM_SQUARES:
                    result = SUM_SQUARES(position, dimensions);
                    break;
                case tipo_funciones.QUARTIC_WITH_NOISE:
                    result = QUARTIC_WITH_NOISE(position, dimensions);
                    break;
                case tipo_funciones.QUARTIC_WITHOUT_NOISE:
                    result = QUARTIC_WITHOUT_NOISE(position, dimensions);
                    break;
                case tipo_funciones.QUINTIC:
                    result = QUINTIC(position, dimensions);
                    break;
                case tipo_funciones.RANA:
                    result = RANA(position, dimensions);
                    break;
                case tipo_funciones.SCHWEFEL:
                    result = SCHWEFEL(position, dimensions);
                    break;
                case tipo_funciones.ROTATED_SCHWEFEL:
                    result = ROTATED_SCHWEFEL(position, dimensions);
                    break;
                case tipo_funciones.RASTRIGIN:
                    result = RASTRIGIN(position, dimensions);
                    break;
                case tipo_funciones.NON_CONTINUOUS_RASTRIGIN:
                    result = NON_CONTINUOUS_RASTRIGIN(position, dimensions);
                    break;
                case tipo_funciones.ROTATED_RASTRIGIN:
                    result = ROTATED_RASTRIGIN(position, dimensions);
                    break;
                case tipo_funciones.ROTATED_NON_CONTINUOUS_RASTRIGIN:
                    result = ROTATED_NON_CONTINUOUS_RASTRIGIN(position, dimensions);
                    break;
                case tipo_funciones.ACKLEY_1:
                    result = ACKLEY_1(position, dimensions);
                    break;
                case tipo_funciones.ROTATED_ACKLEY_1:
                    result = ROTATED_ACKLEY_1(position, dimensions);
                    break;
                case tipo_funciones.ACKLEY_2:
                    result = ACKLEY_2(position, dimensions);
                    break;
                case tipo_funciones.ACKLEY_3:
                    result = ACKLEY_3(position, dimensions);
                    break;
                case tipo_funciones.ACKLEY_4:
                    result = ACKLEY_4(position, dimensions);
                    break;
                case tipo_funciones.ADJIMAN:
                    result = ADJIMAN(position, dimensions);
                    break;
                case tipo_funciones.ALPINE_1:
                    result = ALPINE_1(position, dimensions);
                    break;
                case tipo_funciones.ALPINE_2:
                    result = ALPINE_2(position, dimensions);
                    break;
                case tipo_funciones.BARTELS:
                    result = BARTELS(position, dimensions);
                    break;
                case tipo_funciones.GRIEWANK:
                    result = GRIEWANK(position, dimensions);
                    break;
                case tipo_funciones.HELICAL_VALLEY:
                    result = HELICAL_VALLEY(position, dimensions);
                    break;
                case tipo_funciones.JENNRICH_SAMPSON:
                    result = JENNRICH_SAMPSON(position, dimensions);
                    break;
                case tipo_funciones.KEANE:
                    result = KEANE(position, dimensions);
                    break;
                case tipo_funciones.MIELE_CANTRELL:
                    result = MIELE_CANTRELL(position, dimensions);
                    break;
                case tipo_funciones.ROTATED_GRIEWANK:
                    result = ROTATED_GRIEWANK(position, dimensions);
                    break;
                case tipo_funciones.GENERALIZED_PENALIZED_1:
                    result = GENERALIZED_PENALIZED_1(position, dimensions);
                    break;
                case tipo_funciones.GENERALIZED_PENALIZED_2:
                    result = GENERALIZED_PENALIZED_2(position, dimensions);
                    break;
                case tipo_funciones.MICHALEWICZ:
                    result = MICHALEWICZ(position, dimensions);
                    break;
                case tipo_funciones.XIN_SHE_YANG_1:
                    result = XIN_SHE_YANG_1(position, dimensions);
                    break;
                case tipo_funciones.XIN_SHE_YANG_2:
                    result = XIN_SHE_YANG_2(position, dimensions);
                    break;
                case tipo_funciones.XIN_SHE_YANG_3:
                    result = XIN_SHE_YANG_3(position, dimensions);
                    break;
                case tipo_funciones.XIN_SHE_YANG_4:
                    result = XIN_SHE_YANG_4(position, dimensions);
                    break;
                case tipo_funciones.XIN_SHE_YANG_5:
                    result = XIN_SHE_YANG_5(position, dimensions);
                    break;
                case tipo_funciones.XIN_SHE_YANG_6:
                    result = XIN_SHE_YANG_6(position, dimensions);
                    break;
                case tipo_funciones.XIN_SHE_YANG_7:
                    result = XIN_SHE_YANG_7(position, dimensions);
                    break;
                case tipo_funciones.XIN_SHE_YANG_8:
                    result = XIN_SHE_YANG_8(position, dimensions);
                    break;
                case tipo_funciones.SHEKEL_FOXHOLE:
                    result = SHEKEL_FOXHOLE(position, dimensions);
                    break;
                case tipo_funciones.KOWALIK:
                    result = KOWALIK(position, dimensions);
                    break;
                case tipo_funciones.CHEN_BIRD:
                    result = CHEN_BIRD(position, dimensions);
                    break;
                case tipo_funciones.CHEN_V:
                    result = CHEN_V(position, dimensions);
                    break;
                case tipo_funciones.SIX_HUMP_CAMEL:
                    result = SIX_HUMP_CAMEL(position, dimensions);
                    break;
                case tipo_funciones.THREE_HUMP_CAMEL:
                    result = THREE_HUMP_CAMEL(position, dimensions);
                    break;
                case tipo_funciones.TRID:
                    result = TRID(position, dimensions);
                    break;
                case tipo_funciones.BRANIN_1:
                    result = BRANIN_1(position, dimensions);
                    break;
                case tipo_funciones.BRANIN_2:
                    result = BRANIN_2(position, dimensions);
                    break;
                case tipo_funciones.BRENT:
                    result = BRENT(position, dimensions);
                    break;
                case tipo_funciones.BROWN:
                    result = BROWN(position, dimensions);
                    break;
                case tipo_funciones.BUKIN_2:
                    result = BUKIN_2(position, dimensions);
                    break;
                case tipo_funciones.BUKIN_4:
                    result = BUKIN_4(position, dimensions);
                    break;
                case tipo_funciones.BUKIN_6:
                    result = BUKIN_6(position, dimensions);
                    break;
                case tipo_funciones.GOLDSTEIN_PRICE:
                    result = GOLDSTEIN_PRICE(position, dimensions);
                    break;
                case tipo_funciones.HARTMANN_3D:
                    result = HARTMANN_3D(position, dimensions);
                    break;
                case tipo_funciones.HARTMANN_6D:
                    result = HARTMANN_6D(position, dimensions);
                    break;
                case tipo_funciones.SHEKEL_4_5:
                    result = SHEKEL_4_5(position, dimensions);
                    break;
                case tipo_funciones.SHEKEL_4_7:
                    result = SHEKEL_4_7(position, dimensions);
                    break;
                case tipo_funciones.SHEKEL_4_10:
                    result = SHEKEL_4_10(position, dimensions);
                    break;
                case tipo_funciones.SHUBERT:
                    result = SHUBERT(position, dimensions);
                    break;
                case tipo_funciones.BOHACHEVSKY_1:
                    result = BOHACHEVSKY_1(position, dimensions);
                    break;
                case tipo_funciones.BOHACHEVSKY_2:
                    result = BOHACHEVSKY_2(position, dimensions);
                    break;
                case tipo_funciones.BOHACHEVSKY_3:
                    result = BOHACHEVSKY_3(position, dimensions);
                    break;
                case tipo_funciones.CEC_2005_CF1:
                    result = CEC_2005_CF1(position, dimensions);
                    break;
                case tipo_funciones.CEC_2005_CF2:
                    result = CEC_2005_CF2(position, dimensions);
                    break;
                case tipo_funciones.CEC_2005_CF3:
                    result = CEC_2005_CF3(position, dimensions);
                    break;
                case tipo_funciones.CEC_2005_CF4:
                    result = CEC_2005_CF4(position, dimensions);
                    break;
                case tipo_funciones.CEC_2005_CF5:
                    result = CEC_2005_CF5(position, dimensions);
                    break;
                case tipo_funciones.CEC_2005_CF6:
                    result = CEC_2005_CF6(position, dimensions);
                    break;
                case tipo_funciones.WEIERSTRASS:
                    result = WEIERSTRASS(position, dimensions);
                    break;
                case tipo_funciones.ROTATED_WEIERSTRASS:
                    result = ROTATED_WEIERSTRASS(position, dimensions);
                    break;
                case tipo_funciones.BIGGS_EXP2:
                    result = BIGGS_EXP2(position, dimensions);
                    break;
                case tipo_funciones.BIGGS_EXP3:
                    result = BIGGS_EXP3(position, dimensions);
                    break;
                case tipo_funciones.BIGGS_EXP4:
                    result = BIGGS_EXP4(position, dimensions);
                    break;
                case tipo_funciones.BIGGS_EXP5:
                    result = BIGGS_EXP5(position, dimensions);
                    break;
                case tipo_funciones.BIGGS_EXP6:
                    result = BIGGS_EXP6(position, dimensions);
                    break;
                case tipo_funciones.LEON:
                    result = LEON(position, dimensions);
                    break;
                case tipo_funciones.LEVY_8:
                    result = LEVY_8(position, dimensions);
                    break;
                case tipo_funciones.MATYAS:
                    result = MATYAS(position, dimensions);
                    break;
                case tipo_funciones.MC_CORMICK:
                    result = MC_CORMICK(position, dimensions);
                    break;
                case tipo_funciones.NEUMAIER_PERM:
                    result = NEUMAIER_PERM(position, dimensions);
                    break;
                case tipo_funciones.NEUMAIER_PERM_0:
                    result = NEUMAIER_PERM_0(position, dimensions);
                    break;
                case tipo_funciones.PARSOPOULOS:
                    result = PARSOPOULOS(position, dimensions);
                    break;
                case tipo_funciones.PATHOLOGICAL:
                    result = PATHOLOGICAL(position, dimensions);
                    break;
                case tipo_funciones.PAVIANI:
                    result = PAVIANI(position, dimensions);
                    break;
                case tipo_funciones.PEN_HOLDER:
                    result = PEN_HOLDER(position, dimensions);
                    break;
                case tipo_funciones.PINTER:
                    result = PINTER(position, dimensions);
                    break;
                case tipo_funciones.POWELL:
                    result = POWELL(position, dimensions);
                    break;
                case tipo_funciones.POWELL_SINGULAR_2:
                    result = POWELL_SINGULAR_2(position, dimensions);
                    break;
                case tipo_funciones.POWELL_SUM:
                    result = POWELL_SUM(position, dimensions);
                    break;
                case tipo_funciones.PRICE_1:
                    result = PRICE_1(position, dimensions);
                    break;
                case tipo_funciones.PRICE_2:
                    result = PRICE_2(position, dimensions);
                    break;
                case tipo_funciones.PRICE_4:
                    result = PRICE_4(position, dimensions);
                    break;
                case tipo_funciones.QING:
                    result = QING(position, dimensions);
                    break;
                case tipo_funciones.QUADRATIC:
                    result = QUADRATIC(position, dimensions);
                    break;
                case tipo_funciones.ZIRILLI:
                    result = ZIRILLI(position, dimensions);
                    break;
                case tipo_funciones.ZETTL:
                    result = ZETTL(position, dimensions);
                    break;
                case tipo_funciones.ZEROSUM:
                    result = ZEROSUM(position, dimensions);
                    break;
                case tipo_funciones.ZACHAROV:
                    result = ZACHAROV(position, dimensions);
                    break;
                /*case tipo_funciones.PROBLEMA_TENSION_COMPRESSION_SPRING:
                    result = PROBLEMA_TENSION_COMPRESSION_SPRING(position, dimensions);
                    break;
                case tipo_funciones.FUNCION_RESTRICCIONES_G1_CEC_2006:
                    result = FUNCION_RESTRICCIONES_G1_CEC_2006(position, dimensions);
                    break;*/
                case tipo_funciones.YAOLIU09:
                    result = YAOLIU09(position, dimensions);
                    break;
                case tipo_funciones.YAOLIU04:
                    result = YAOLIU04(position, dimensions);
                    break;
                case tipo_funciones.CEC_2013_1_SPHERE:
                    result = CEC_2013_1_SPHERE(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_2_ROTATED_HIGH_CONDITIONED_ELLIPTIC:
                    result = CEC_2013_2_ROTATED_HIGH_CONDITIONED_ELLIPTIC(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_3_ROTATED_BENT_CIGAR:
                    result = CEC_2013_3_ROTATED_BENT_CIGAR(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_4_ROTATED_DISCUS:
                    result = CEC_2013_4_ROTATED_DISCUS(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_5_DIFFERENT_POWERS:
                    result = CEC_2013_5_DIFFERENT_POWERS(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_5A_ROTATED_DIFFERENT_POWERS:
                    result = CEC_2013_5A_ROTATED_DIFFERENT_POWERS(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_6_ROTATED_ROSENBROCK:
                    result = CEC_2013_6_ROTATED_ROSENBROCK(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_7_ROTATED_SCHAFFER_F7:
                    result = CEC_2013_7_ROTATED_SCHAFFER_F7(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_8_ROTATED_ACKLEY:
                    result = CEC_2013_8_ROTATED_ACKLEY(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_9_ROTATED_WEIERSTRASS:
                    result = CEC_2013_9_ROTATED_WEIERSTRASS(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_10_ROTATED_GRIEWANK:
                    result = CEC_2013_10_ROTATED_GRIEWANK(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_11_RASTRIGIN:
                    result = CEC_2013_11_RASTRIGIN(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_12_ROTATED_RASTRIGIN:
                    result = CEC_2013_12_ROTATED_RASTRIGIN(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_13_NON_CONTINUOUS_ROTATED_RASTRIGIN:
                    result = CEC_2013_13_NON_CONTINUOUS_ROTATED_RASTRIGIN(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_14_SCHWEFEL:
                    result = CEC_2013_14_SCHWEFEL(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_15_ROTATED_SCHWEFEL:
                    result = CEC_2013_15_ROTATED_SCHWEFEL(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_16_ROTATED_KATSUURA:
                    result = CEC_2013_16_ROTATED_KATSUURA(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_17_LUNACEK_BI_RASTRIGIN:
                    result = CEC_2013_17_LUNACEK_BI_RASTRIGIN(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_18_ROTATED_LUNACEK_BI_RASTRIGIN:
                    result = CEC_2013_18_ROTATED_LUNACEK_BI_RASTRIGIN(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_19_ROTATED_EXPANDED_GRIEWANK_PLUS_ROSENBROCK:
                    result = CEC_2013_19_ROTATED_EXPANDED_GRIEWANK_PLUS_ROSENBROCK(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_20_ROTATED_EXPANDED_SCAFFER_6:
                    result = CEC_2013_20_ROTATED_EXPANDED_SCAFFER_6(position, dimensions, 1);
                    break;
                case tipo_funciones.CEC_2013_21_COMPOSITION_FUNCTION_1:
                    result = CEC_2013_21_COMPOSITION_FUNCTION_1(position, dimensions);
                    break;
                case tipo_funciones.CEC_2013_22_COMPOSITION_FUNCTION_2:
                    result = CEC_2013_22_COMPOSITION_FUNCTION_2(position, dimensions);
                    break;
                case tipo_funciones.CEC_2013_23_COMPOSITION_FUNCTION_3:
                    result = CEC_2013_23_COMPOSITION_FUNCTION_3(position, dimensions);
                    break;
                case tipo_funciones.CEC_2013_24_COMPOSITION_FUNCTION_4:
                    result = CEC_2013_24_COMPOSITION_FUNCTION_4(position, dimensions);
                    break;
                case tipo_funciones.CEC_2013_25_COMPOSITION_FUNCTION_5:
                    result = CEC_2013_25_COMPOSITION_FUNCTION_5(position, dimensions);
                    break;
                case tipo_funciones.CEC_2013_26_COMPOSITION_FUNCTION_6:
                    result = CEC_2013_26_COMPOSITION_FUNCTION_6(position, dimensions);
                    break;
                case tipo_funciones.CEC_2013_27_COMPOSITION_FUNCTION_7:
                    result = CEC_2013_27_COMPOSITION_FUNCTION_7(position, dimensions);
                    break;
                case tipo_funciones.CEC_2013_28_COMPOSITION_FUNCTION_8:
                    result = CEC_2013_28_COMPOSITION_FUNCTION_8(position, dimensions);
                    break;
                default:
                    return (double.MaxValue);

            }
            return result+function_off_set;
        }

        public ArrayList nombre_de_funciones = new ArrayList();

        public ArrayList devuelve_funciones_nombre()
        {
            nombre_de_funciones.Clear();
            nombre_de_funciones.Add("ACKLEY_1");
            nombre_de_funciones.Add("ROTATED_ACKLEY_1");
            nombre_de_funciones.Add("ACKLEY_2");
            nombre_de_funciones.Add("ACKLEY_3");
            nombre_de_funciones.Add("ACKLEY_4");
            nombre_de_funciones.Add("ADJIMAN");
            nombre_de_funciones.Add("ALPINE_1");
            nombre_de_funciones.Add("ALPINE_2");
            nombre_de_funciones.Add("BARTELS");
            nombre_de_funciones.Add("BEALE");
            nombre_de_funciones.Add("BIRD");
            nombre_de_funciones.Add("BIGGS_EXP2");
            nombre_de_funciones.Add("BIGGS_EXP3");
            nombre_de_funciones.Add("BIGGS_EXP4");
            nombre_de_funciones.Add("BIGGS_EXP5");
            nombre_de_funciones.Add("BIGGS_EXP6");
            nombre_de_funciones.Add("BOHACHEVSKY_1");
            nombre_de_funciones.Add("BOHACHEVSKY_2");
            nombre_de_funciones.Add("BOHACHEVSKY_3");
            nombre_de_funciones.Add("BOOTH");
            nombre_de_funciones.Add("BOX_BETTS");
            nombre_de_funciones.Add("BRAD");
            nombre_de_funciones.Add("BRANIN_1");
            nombre_de_funciones.Add("BRANIN_2");
            nombre_de_funciones.Add("BRENT");
            nombre_de_funciones.Add("BROWN");
            nombre_de_funciones.Add("BUKIN_2");
            nombre_de_funciones.Add("BUKIN_4");
            nombre_de_funciones.Add("BUKIN_6");
            nombre_de_funciones.Add("CEC_2005_CF1");
            nombre_de_funciones.Add("CEC_2005_CF2");
            nombre_de_funciones.Add("CEC_2005_CF3");
            nombre_de_funciones.Add("CEC_2005_CF4");
            nombre_de_funciones.Add("CEC_2005_CF5");
            nombre_de_funciones.Add("CEC_2005_CF6");
            nombre_de_funciones.Add("CHEN_BIRD");
            nombre_de_funciones.Add("CHEN_V");
            nombre_de_funciones.Add("CHICHINADZE");
            nombre_de_funciones.Add("CHUNG_REYNOLDS");
            nombre_de_funciones.Add("CEC_2013_1_SPHERE");
            nombre_de_funciones.Add("CEC_2013_2_ROTATED_HIGH_CONDITIONED_ELLIPTIC");
            nombre_de_funciones.Add("CEC_2013_3_ROTATED_BENT_CIGAR");
            nombre_de_funciones.Add("CEC_2013_4_ROTATED_DISCUS");
            nombre_de_funciones.Add("CEC_2013_5_DIFFERENT_POWERS");
            nombre_de_funciones.Add("CEC_2013_5A_ROTATED_DIFFERENT_POWERS");
            nombre_de_funciones.Add("CEC_2013_6_ROTATED_ROSENBROCK");
            nombre_de_funciones.Add("CEC_2013_7_ROTATED_SCHAFFER_F7");
            nombre_de_funciones.Add("CEC_2013_8_ROTATED_ACKLEY");
            nombre_de_funciones.Add("CEC_2013_9_ROTATED_WEIERSTRASS");
            nombre_de_funciones.Add("CEC_2013_10_ROTATED_GRIEWANK");
            nombre_de_funciones.Add("CEC_2013_11_RASTRIGIN");
            nombre_de_funciones.Add("CEC_2013_12_ROTATED_RASTRIGIN");
            nombre_de_funciones.Add("CEC_2013_13_NON_CONTINUOUS_ROTATED_RASTRIGIN");
            nombre_de_funciones.Add("CEC_2013_14_SCHWEFEL");
            nombre_de_funciones.Add("CEC_2013_15_ROTATED_SCHWEFEL");
            nombre_de_funciones.Add("CEC_2013_16_ROTATED_KATSUURA");
            nombre_de_funciones.Add("CEC_2013_17_LUNACEK_BI_RASTRIGIN");
            nombre_de_funciones.Add("CEC_2013_18_ROTATED_LUNACEK_BI_RASTRIGIN");
            nombre_de_funciones.Add("CEC_2013_19_ROTATED_EXPANDED_GRIEWANK_PLUS_ROSENBROCK");
            nombre_de_funciones.Add("CEC_2013_20_ROTATED_EXPANDED_SCAFFER_6");
            nombre_de_funciones.Add("CEC_2013_21_COMPOSITION_FUNCTION_1");
            nombre_de_funciones.Add("CEC_2013_22_COMPOSITION_FUNCTION_2");
            nombre_de_funciones.Add("CEC_2013_23_COMPOSITION_FUNCTION_3");
            nombre_de_funciones.Add("CEC_2013_24_COMPOSITION_FUNCTION_4");
            nombre_de_funciones.Add("CEC_2013_25_COMPOSITION_FUNCTION_5");
            nombre_de_funciones.Add("CEC_2013_26_COMPOSITION_FUNCTION_6");
            nombre_de_funciones.Add("CEC_2013_27_COMPOSITION_FUNCTION_7");
            nombre_de_funciones.Add("CEC_2013_28_COMPOSITION_FUNCTION_8");
            nombre_de_funciones.Add("COLVILLE");
            nombre_de_funciones.Add("CORANA");
            nombre_de_funciones.Add("COSINE_MIXTURE_2");
            nombre_de_funciones.Add("COSINE_MIXTURE_4");
            nombre_de_funciones.Add("CROSS_IN_TRAY");
            nombre_de_funciones.Add("CSENDES");
            nombre_de_funciones.Add("CUBE");
            nombre_de_funciones.Add("DAMAVANDI");
            nombre_de_funciones.Add("DEB_1");
            nombre_de_funciones.Add("DEB_3");
            nombre_de_funciones.Add("DECKKERS_AARTS");
            nombre_de_funciones.Add("DE_VILLIERS_GLASSER_1");
            nombre_de_funciones.Add("DE_VILLIERS_GLASSER_2");
            nombre_de_funciones.Add("DIXON_PRICE");
            nombre_de_funciones.Add("EASOM");
            nombre_de_funciones.Add("EGG_CRATE");
            nombre_de_funciones.Add("EGG_HOLDER");
            nombre_de_funciones.Add("EL_ATTAR_VIDYASAGAR_DUTTA");
            nombre_de_funciones.Add("EXPONENTIAL");
            nombre_de_funciones.Add("EXP_2");
            nombre_de_funciones.Add("FREUDENSTEIN_ROTH");
            nombre_de_funciones.Add("GIUNTA");
            nombre_de_funciones.Add("GEAR");
            nombre_de_funciones.Add("GENERALIZED_PENALIZED_1");
            nombre_de_funciones.Add("GENERALIZED_PENALIZED_2");
            nombre_de_funciones.Add("GOLDSTEIN_PRICE");
            nombre_de_funciones.Add("GRIEWANK");
            nombre_de_funciones.Add("HELICAL_VALLEY");
            nombre_de_funciones.Add("JENNRICH_SAMPSON");
            nombre_de_funciones.Add("KEANE");
            nombre_de_funciones.Add("MIELE_CANTRELL");
            nombre_de_funciones.Add("ROTATED_GRIEWANK");
            nombre_de_funciones.Add("HANSEN");
            nombre_de_funciones.Add("HARTMANN_3D");
            nombre_de_funciones.Add("HARTMANN_6D");
            nombre_de_funciones.Add("HIMMELBLAU");
            nombre_de_funciones.Add("HYPERELLIPSOID");
            nombre_de_funciones.Add("HOLZMAN_1");
            nombre_de_funciones.Add("HOLZMAN_2");
            nombre_de_funciones.Add("HOSAKI");
            nombre_de_funciones.Add("KATSUURAS");
            nombre_de_funciones.Add("KOWALIK");
            nombre_de_funciones.Add("LAGERMAN");
            nombre_de_funciones.Add("LEON");
            nombre_de_funciones.Add("LEVY_8");
            nombre_de_funciones.Add("MATYAS");
            nombre_de_funciones.Add("MC_CORMICK");
            nombre_de_funciones.Add("MICHALEWICZ");
            nombre_de_funciones.Add("NEUMAIER_PERM");
            nombre_de_funciones.Add("NEUMAIER_PERM_0");
            nombre_de_funciones.Add("PARSOPOULOS");
            nombre_de_funciones.Add("PATHOLOGICAL");
            nombre_de_funciones.Add("PAVIANI");
            nombre_de_funciones.Add("PEN_HOLDER");
            nombre_de_funciones.Add("PINTER");
            nombre_de_funciones.Add("POWELL");
            nombre_de_funciones.Add("POWELL_SINGULAR_2");
            nombre_de_funciones.Add("POWELL_SUM");
            nombre_de_funciones.Add("PRICE_1");
            nombre_de_funciones.Add("PRICE_2");
            nombre_de_funciones.Add("PRICE_4");
            nombre_de_funciones.Add("QING");
            nombre_de_funciones.Add("QUADRATIC");
            nombre_de_funciones.Add("QUARTIC_WITH_NOISE");
            nombre_de_funciones.Add("QUARTIC_WITHOUT_NOISE");
            nombre_de_funciones.Add("QUINTIC");
            nombre_de_funciones.Add("RANA");
            nombre_de_funciones.Add("RASTRIGIN");
            nombre_de_funciones.Add("ROTATED_RASTRIGIN");
            nombre_de_funciones.Add("ROTATED_NON_CONTINUOUS_RASTRIGIN");
            nombre_de_funciones.Add("NON_CONTINUOUS_RASTRIGIN");
            nombre_de_funciones.Add("ROSENBROCK");
            nombre_de_funciones.Add("ROTATED_HYPERELLIPSOID");
            nombre_de_funciones.Add("RUMP");
            nombre_de_funciones.Add("SALOMON");
            nombre_de_funciones.Add("SCHWEFEL");
            nombre_de_funciones.Add("ROTATED_SCHWEFEL");
            nombre_de_funciones.Add("SCHWEFEL_221");
            nombre_de_funciones.Add("SCHWEFEL_222");
            nombre_de_funciones.Add("SHEKEL_FOXHOLE");
            nombre_de_funciones.Add("SHEKEL_4_5");
            nombre_de_funciones.Add("SHEKEL_4_7");
            nombre_de_funciones.Add("SHEKEL_4_10");
            nombre_de_funciones.Add("SHUBERT");
            nombre_de_funciones.Add("SIX_HUMP_CAMEL");
            nombre_de_funciones.Add("SPHERE");
            nombre_de_funciones.Add("STEP_1");
            nombre_de_funciones.Add("STEP_2");
            nombre_de_funciones.Add("STEP_3");
            nombre_de_funciones.Add("STYBLINSKI_TANG");
            nombre_de_funciones.Add("SUM_OF_DIFFERENT_POWERS");
            nombre_de_funciones.Add("SUM_SQUARES");
            nombre_de_funciones.Add("THREE_HUMP_CAMEL");
            nombre_de_funciones.Add("TRID");
            nombre_de_funciones.Add("WEIERSTRASS");
            nombre_de_funciones.Add("ROTATED_WEIERSTRASS");
            nombre_de_funciones.Add("XIN_SHE_YANG_1");
            nombre_de_funciones.Add("XIN_SHE_YANG_2");
            nombre_de_funciones.Add("XIN_SHE_YANG_3");
            nombre_de_funciones.Add("XIN_SHE_YANG_4");
            nombre_de_funciones.Add("XIN_SHE_YANG_5");
            nombre_de_funciones.Add("XIN_SHE_YANG_6");
            nombre_de_funciones.Add("XIN_SHE_YANG_7");
            nombre_de_funciones.Add("XIN_SHE_YANG_8");
            nombre_de_funciones.Add("YAOLIU09");
            nombre_de_funciones.Add("YAOLIU04");
            nombre_de_funciones.Add("ZIRILLI");
            nombre_de_funciones.Add("ZETTL");
            nombre_de_funciones.Add("ZEROSUM");
            nombre_de_funciones.Add("ZACHAROV");

            //nombre_de_funciones.Add("PROBLEMA_WELDED_BEAM");
            //nombre_de_funciones.Add("PROBLEMA_TENSION_COMPRESSION_SPRING");
            //nombre_de_funciones.Add("FUNCION_RESTRICCIONES_G1_CEC_2006");
            //nombre_de_funciones.Add("PROBLEMA_PRESSURE_VESSEL");
            //nombre_de_funciones.Add("PROBLEMA_SPEED_REDUCER");
            //nombre_de_funciones.Add("PROBLEMA_ROLLING_BEARING_ELEMENT");
            //nombre_de_funciones.Add("PROBLEMA_DISPLACEMENT_OF_LOADED_STRUCTURE");

            return (nombre_de_funciones);
        }

        public enum tipo_funciones
        {
            ACKLEY_1, // :)
            ROTATED_ACKLEY_1,// :)
            ACKLEY_2,// :)
            ACKLEY_3,// :)
            ACKLEY_4,// :)
            ADJIMAN, // :)
            ALPINE_1,// :)
            ALPINE_2,// :)
            BARTELS,// :)
            BEALE,// :)
            BIRD,// :)
            BIGGS_EXP2,// :)
            BIGGS_EXP3,// :)
            BIGGS_EXP4,// :)
            BIGGS_EXP5,// :)
            BIGGS_EXP6,// :)
            BOHACHEVSKY_1,// :)
            BOHACHEVSKY_2,// :)
            BOHACHEVSKY_3,// :)
            BOOTH,// :)
            BOX_BETTS,// :)
            BRAD,
            BRANIN_1,// :)
            BRANIN_2,//NO POR DIMENSIONES
            BRENT,// :)
            BROWN,// :)
            BUKIN_2,// :)
            BUKIN_4,// :)
            BUKIN_6,// :)
            CEC_2005_CF1,// :)
            CEC_2005_CF2,// :)
            CEC_2005_CF3,// :)
            CEC_2005_CF4,// :)
            CEC_2005_CF5,// :)
            CEC_2005_CF6,// :)
            CHEN_BIRD,// :)
            CHEN_V,// :)
            CHICHINADZE,// :)
            CHUNG_REYNOLDS,// :)
            COLVILLE,// :)
            CORANA,// :)
            COSINE_MIXTURE_2,// :)
            COSINE_MIXTURE_4,// :)
            CROSS_IN_TRAY,
            CSENDES,
            CUBE,
            DAMAVANDI,
            DEB_1,
            DEB_3,
            DECKKERS_AARTS,
            DE_VILLIERS_GLASSER_1,
            DE_VILLIERS_GLASSER_2,
            DIXON_PRICE,
            DOLAN,
            EASOM,// :)
            EGG_CRATE,// :)
            EGG_HOLDER,// :)
            EL_ATTAR_VIDYASAGAR_DUTTA,
            EXPONENTIAL,
            EXP_2,// :)
            FREUDENSTEIN_ROTH,
            GEAR,// :)
            GENERALIZED_PENALIZED_1,// :)
            GENERALIZED_PENALIZED_2,// :)
            GIUNTA,
            GOLDSTEIN_PRICE,// :)
            GRIEWANK,// :)
            HELICAL_VALLEY,
            JENNRICH_SAMPSON,
            KEANE,
            MIELE_CANTRELL,
            ROTATED_GRIEWANK,// :)
            HANSEN,// :)
            HARTMANN_3D,// :)
            HARTMANN_6D,// :)
            HIMMELBLAU,// :)
            HYPERELLIPSOID,// :)
            HOLZMAN_1,// :)
            HOLZMAN_2,// :)
            HOSAKI,// :)
            KATSUURAS,// :)
            KOWALIK,// :)
            LAGERMAN, //MODIFICAR ES LANGERMAN :)
            LEON,// :)
            LEVY_8,// :)
            MATYAS,// :)
            MC_CORMICK,// :)
            MICHALEWICZ,// :)
            NEUMAIER_PERM,// :)
            NEUMAIER_PERM_0,// :)
            PARSOPOULOS,// :)
            PATHOLOGICAL,// :)
            PAVIANI,// :)
            PEN_HOLDER,// :)
            PINTER,
            POWELL,
            POWELL_SINGULAR_2,
            POWELL_SUM,
            PRICE_1,
            PRICE_2,
            PRICE_4,
            QING,
            QUADRATIC,
            QUARTIC_WITH_NOISE,// :)
            QUARTIC_WITHOUT_NOISE,// :)
            QUINTIC,
            RANA,
            RASTRIGIN,// :)
            ROTATED_RASTRIGIN,// :)
            ROTATED_NON_CONTINUOUS_RASTRIGIN,// :)
            NON_CONTINUOUS_RASTRIGIN,// :)
            ROSENBROCK,// :)
            ROTATED_HYPERELLIPSOID,// :)
            RUMP,
            SALOMON,
            SCHWEFEL,// :)
            ROTATED_SCHWEFEL,// :)
            SCHWEFEL_221,// :)
            SCHWEFEL_222,// :)
            SHEKEL_FOXHOLE,// :)
            SHEKEL_4_5,// :)
            SHEKEL_4_7,// :)
            SHEKEL_4_10,// :)
            SHUBERT,
            SIX_HUMP_CAMEL,// :)
            SPHERE,// :)
            CEC_2013_1_SPHERE,// :)
            CEC_2013_2_ROTATED_HIGH_CONDITIONED_ELLIPTIC,// :)
            CEC_2013_3_ROTATED_BENT_CIGAR,// :)
            CEC_2013_4_ROTATED_DISCUS,// :)
            CEC_2013_5_DIFFERENT_POWERS,// :)
            CEC_2013_5A_ROTATED_DIFFERENT_POWERS,// :)
            CEC_2013_6_ROTATED_ROSENBROCK,// :)
            CEC_2013_7_ROTATED_SCHAFFER_F7,// :)
            CEC_2013_8_ROTATED_ACKLEY,// :)
            CEC_2013_9_ROTATED_WEIERSTRASS,// :)
            CEC_2013_10_ROTATED_GRIEWANK,// :)
            CEC_2013_11_RASTRIGIN,// :)
            CEC_2013_12_ROTATED_RASTRIGIN,// :)
            CEC_2013_13_NON_CONTINUOUS_ROTATED_RASTRIGIN,// :)
            CEC_2013_14_SCHWEFEL,// :)
            CEC_2013_15_ROTATED_SCHWEFEL,// :)
            CEC_2013_16_ROTATED_KATSUURA,// :)
            CEC_2013_17_LUNACEK_BI_RASTRIGIN,// :)
            CEC_2013_18_ROTATED_LUNACEK_BI_RASTRIGIN,// :)
            CEC_2013_19_ROTATED_EXPANDED_GRIEWANK_PLUS_ROSENBROCK,// :)
            CEC_2013_20_ROTATED_EXPANDED_SCAFFER_6,// :)
            CEC_2013_21_COMPOSITION_FUNCTION_1,// :)
            CEC_2013_22_COMPOSITION_FUNCTION_2,// :)
            CEC_2013_23_COMPOSITION_FUNCTION_3,// :)
            CEC_2013_24_COMPOSITION_FUNCTION_4,// :)
            CEC_2013_25_COMPOSITION_FUNCTION_5,// :)
            CEC_2013_26_COMPOSITION_FUNCTION_6,// :)
            CEC_2013_27_COMPOSITION_FUNCTION_7,// :)
            CEC_2013_28_COMPOSITION_FUNCTION_8,// :)
            STEP_1,// :)
            STEP_2,// :)
            STEP_3,// :)
            STYBLINSKI_TANG,
            SUM_OF_DIFFERENT_POWERS,
            SUM_SQUARES,
            THREE_HUMP_CAMEL,// :)
            TRID,// :)
            WEIERSTRASS,// :)
            ROTATED_WEIERSTRASS,// :)
            XIN_SHE_YANG_1,// :)
            XIN_SHE_YANG_2,// :)
            XIN_SHE_YANG_3,// :)
            XIN_SHE_YANG_4,// :)
            XIN_SHE_YANG_5,// :)
            XIN_SHE_YANG_6,// :)
            XIN_SHE_YANG_7,// :)
            XIN_SHE_YANG_8,// :)
            YAOLIU09,// :)
            YAOLIU04,// :)
            ZIRILLI,// :)
            ZETTL,// :)
            ZEROSUM,// :)
            ZACHAROV,// :)
            //PROBLEMA_WELDED_BEAM,
            //PROBLEMA_TENSION_COMPRESSION_SPRING,
            //FUNCION_RESTRICCIONES_G1_CEC_2006,
            //PROBLEMA_PRESSURE_VESSEL,
            //PROBLEMA_SPEED_REDUCER,
            //PROBLEMA_ROLLING_BEARING_ELEMENT,
            //PROBLEMA_DISPLACEMENT_OF_LOADED_STRUCTURE
        };


    }
}
