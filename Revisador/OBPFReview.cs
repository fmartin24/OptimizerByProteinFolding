using System;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace Revisador
{
    public partial class OBPFReview : Form
    {
        public OBPFReview()
        {
            InitializeComponent();
        }

        ArrayList archivos_de_corrida = new ArrayList();
        ArrayList archivos_de_ejecucion = new ArrayList();
        ArrayList lineas_de_ejecucion_propuestas = new ArrayList();

        struct datos
        {
            public string linea_de_comandos;
            public int ejecuciones;
        }

        datos[] valores_leidos = new datos[20000];
        int cantidad_valores_leidos;
        long cantidad_de_lineas_por_revisar;
        long cantidad_de_repeticiones_completas;

        void elimina_comas_del_final(ref string cadena)
        {
            while (cadena.EndsWith(","))
                cadena = cadena.Substring(0, cadena.Length - 1);
        }

        void inserta_espacios_entre_componentes(ref string cadena)
        {
            string[] pedazos_linea_comandos = cadena.Split(',');
            cadena = "";
            foreach (string pedazo in pedazos_linea_comandos)
                cadena += pedazo.Trim() + ", ";
            cadena = cadena.Trim();
            elimina_comas_del_final(ref cadena);

        }

        bool compara_dos_lineas_de_ejecucion(string comando, string cadena)
        {
            string[] pedazos_cadena = cadena.Trim().Split(',');
            string[] pedazos_linea_comandos = comando.Trim().Split(',');
            int i;
            for (i = 0; i < pedazos_cadena.Length; i++)
                if (pedazos_cadena[i].Trim() != pedazos_linea_comandos[i].Trim())
                    return (false);
            return (true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string cadena;
            archivos_de_corrida.Clear();
            archivos_de_ejecucion.Clear();
            lineas_de_ejecucion_propuestas.Clear();
            cantidad_de_lineas_por_revisar = 0;
            cantidad_de_repeticiones_completas = 0;
            openFileDialog1.InitialDirectory = Application.StartupPath;
            openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.FileName = "";
            cantidad_valores_leidos = 0;
            int cuenta_lecturas = 0;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (string names in openFileDialog1.FileNames)
                    archivos_de_corrida.Add(names);
                for (int i = 0; i < archivos_de_corrida.Count; i++)
                {
                    StreamReader reader = new StreamReader(archivos_de_corrida[i].ToString());
                    while (!reader.EndOfStream)
                    {
                        cadena = reader.ReadLine().Trim();
                        if (cadena.StartsWith("#"))
                            continue;
                        if (cadena != "")
                        {
                            cuenta_lecturas++;
                            cadena = cadena.Trim();
                            elimina_comas_del_final(ref cadena);
                            inserta_espacios_entre_componentes(ref cadena);
                            lineas_de_ejecucion_propuestas.Add(cadena);
                        }
                    }
                    reader.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.InitialDirectory = Application.StartupPath;
            openFileDialog2.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog2.FilterIndex = 1;
            openFileDialog2.RestoreDirectory = true;
            openFileDialog2.FileName = "";
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                foreach (string names in openFileDialog2.FileNames)
                    archivos_de_ejecucion.Add(names);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string cadena;
            cantidad_de_lineas_por_revisar = lineas_de_ejecucion_propuestas.Count;
            textBox2.Text = cantidad_de_lineas_por_revisar.ToString();
            cantidad_de_repeticiones_completas = 0;
            saveFileDialog1.InitialDirectory = Application.StartupPath;
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = "Result.txt";
            cantidad_de_repeticiones_completas = 0;
            textBox3.Text = cantidad_de_repeticiones_completas.ToString();
            textBox3.Refresh();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                for (int i = 0; i < archivos_de_ejecucion.Count; i++)
                {
                    StreamReader reader = new StreamReader(archivos_de_ejecucion[i].ToString());
                    while (!reader.EndOfStream)
                    {
                        cadena = reader.ReadLine();
                        
                    retrocede:
                        if (cadena != "" && cadena.Contains("Linea de comandos:"))
                        {
                            textBox2.Text = (--cantidad_de_lineas_por_revisar).ToString();
                            textBox2.Refresh();
                            cadena = cadena.Substring(cadena.IndexOf(':') + 1).Trim();
                            cantidad_valores_leidos++;
                            valores_leidos[cantidad_valores_leidos].linea_de_comandos = cadena;
                            valores_leidos[cantidad_valores_leidos].ejecuciones = 0;
                            while (!reader.EndOfStream)
                            {
                                cadena = reader.ReadLine();
                                if (cadena != "" && cadena.Contains("Mejor valor obtenido:"))
                                {
                                    valores_leidos[cantidad_valores_leidos].ejecuciones++;
                                    cantidad_de_repeticiones_completas++;
                                    textBox3.Text = cantidad_de_repeticiones_completas.ToString();
                                    textBox3.Refresh();
                                }
                                if (cadena != "" && cadena.Contains("Linea de comandos:"))
                                    goto retrocede;
                            }
                        }
                    }
                    reader.Close();
                }
                StreamWriter writer = new StreamWriter(saveFileDialog1.FileName);
                writer.Flush();
                writer.WriteLine("Resultados Completos" + Environment.NewLine);
                for (int i = 1; i <= cantidad_valores_leidos; i++)
                {
                    string[] pedazos_linea_comandos = valores_leidos[i].linea_de_comandos.Split(',');
                    if (pedazos_linea_comandos[1].Trim() == valores_leidos[i].ejecuciones.ToString())
                        writer.WriteLine(valores_leidos[i].ejecuciones.ToString() + "   " + valores_leidos[i].linea_de_comandos);
                }
                writer.Flush();
                writer.WriteLine(Environment.NewLine + "Resultados Incompletos" + Environment.NewLine);
                for (int i = 1; i <= cantidad_valores_leidos; i++)
                {
                    string[] pedazos_linea_comandos = valores_leidos[i].linea_de_comandos.Split(',');
                    if (pedazos_linea_comandos[1].Trim() != valores_leidos[i].ejecuciones.ToString())
                        writer.WriteLine(valores_leidos[i].ejecuciones.ToString() + "   " + valores_leidos[i].linea_de_comandos);
                }
                writer.WriteLine(Environment.NewLine + "Resultados sin ejecuciones" + Environment.NewLine);
                writer.Flush();
                string cadena2;
                string cadena1;
                for (int i = 1; i <= cantidad_valores_leidos; i++)
                {
                    textBox2.Text=i.ToString();
                    textBox2.Refresh();
                    cadena2 = valores_leidos[i].linea_de_comandos;
                    for (int j = 0; j < lineas_de_ejecucion_propuestas.Count; j++)
                    {
                        cadena1 = lineas_de_ejecucion_propuestas[j].ToString();
                        if (compara_dos_lineas_de_ejecucion(cadena1, cadena2))
                        {
                            lineas_de_ejecucion_propuestas.RemoveAt(j);
                            break;
                        }
                    }
                }
                foreach (string linea_comando in lineas_de_ejecucion_propuestas)
                {
                    cadena = linea_comando;
                    cadena = cadena.Trim();
                    elimina_comas_del_final(ref cadena);
                    writer.WriteLine("0" + "    " + cadena);
                }
                writer.Flush();
                writer.Close();
                Cursor.Current = Cursors.Default;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog3.InitialDirectory = Application.StartupPath;
            openFileDialog3.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog3.FilterIndex = 1;
            openFileDialog3.RestoreDirectory = true;
            openFileDialog3.FileName = "";
            lineas_de_ejecucion_propuestas.Clear();
            if (openFileDialog3.ShowDialog() == DialogResult.OK)
            {
                StreamReader reader = new StreamReader(openFileDialog3.FileName);
                while (!reader.EndOfStream)
                {
                    string cadena = reader.ReadLine().Trim();
                    if (cadena.StartsWith("#"))
                        continue;
                    if (cadena != "")
                    {
                        cadena = cadena.Trim();
                        elimina_comas_del_final(ref cadena);
                        inserta_espacios_entre_componentes(ref cadena);
                        lineas_de_ejecucion_propuestas.Add(cadena);
                    }
                }
                reader.Close();
                textBox5.Text = lineas_de_ejecucion_propuestas.Count.ToString();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int cuenta;
            int cantidad_veces = (int)numericUpDown1.Value;
            int resto;
            string nuevo_nombre;
            string extension;
            StreamReader reader = new StreamReader(openFileDialog3.FileName);
            StreamWriter writer;
            cuenta = lineas_de_ejecucion_propuestas.Count / cantidad_veces;
            resto = lineas_de_ejecucion_propuestas.Count % cantidad_veces;
            if (resto == 0)
            {
                for (int contador = 1; contador <= cantidad_veces; contador++)
                {
                    nuevo_nombre = openFileDialog3.FileName;
                    extension = nuevo_nombre.Substring(nuevo_nombre.LastIndexOf('.'));
                    nuevo_nombre = nuevo_nombre.Substring(0, nuevo_nombre.LastIndexOf('.'));
                    nuevo_nombre = nuevo_nombre + "_" + contador + extension;
                    writer = new StreamWriter(nuevo_nombre);
                    for (int i = 0; i < cuenta; i++)
                        writer.WriteLine(lineas_de_ejecucion_propuestas[cuenta * (contador - 1) + i]);
                    writer.Close();
                }
            }
            else
            {
                for (int contador = 1; contador < cantidad_veces; contador++)
                {
                    nuevo_nombre = openFileDialog3.FileName;
                    extension = nuevo_nombre.Substring(nuevo_nombre.LastIndexOf('.'));
                    nuevo_nombre = nuevo_nombre.Substring(0, nuevo_nombre.LastIndexOf('.'));
                    nuevo_nombre = nuevo_nombre + "_" + contador + extension;
                    writer = new StreamWriter(nuevo_nombre);
                    for (int i = 0; i < cuenta; i++)
                        writer.WriteLine(lineas_de_ejecucion_propuestas[cuenta * (contador - 1) + i]);
                    writer.Close();
                }
                nuevo_nombre = openFileDialog3.FileName;
                extension = nuevo_nombre.Substring(nuevo_nombre.LastIndexOf('.'));
                nuevo_nombre = nuevo_nombre.Substring(0, nuevo_nombre.LastIndexOf('.'));
                nuevo_nombre = nuevo_nombre + "_" + cantidad_veces + extension;
                writer = new StreamWriter(nuevo_nombre);
                for (int i = cuenta * cantidad_veces; i < lineas_de_ejecucion_propuestas.Count; i++)
                    writer.WriteLine(lineas_de_ejecucion_propuestas[i]);
                writer.Close();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            openFileDialog4.InitialDirectory = Application.StartupPath;
            openFileDialog4.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog4.FilterIndex = 1;
            openFileDialog4.RestoreDirectory = true;
            openFileDialog4.FileName = "";
            if (openFileDialog4.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = cantidad_de_lineas_por_revisar.ToString();
                textBox4.Refresh();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            StreamReader reader;
            StreamWriter writer;
            string linea_leida;
            saveFileDialog2.InitialDirectory = Application.StartupPath;
            saveFileDialog2.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog2.FilterIndex = 1;
            saveFileDialog2.RestoreDirectory = true;
            saveFileDialog2.FileName = "Joined.txt";
            cantidad_de_repeticiones_completas = 0;
            textBox4.Text = cantidad_de_repeticiones_completas.ToString();
            textBox4.Refresh();
            ArrayList archivos_ordenados = new ArrayList(openFileDialog4.FileNames);
            archivos_ordenados.Sort();
            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                writer = new StreamWriter(saveFileDialog2.FileName);
                foreach (string archivos in archivos_ordenados)
                {
                    reader = new StreamReader(archivos);
                    while (!reader.EndOfStream)
                    {
                        linea_leida = reader.ReadLine().Trim();
                        if (linea_leida.Contains("Linea de comandos:"))
                        {
                            cantidad_de_repeticiones_completas++;
                            textBox4.Text = cantidad_de_repeticiones_completas.ToString();
                            textBox4.Refresh();
                        }
                        writer.WriteLine(linea_leida);
                    }
                    reader.Close();
                }
                writer.Close();
               
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Focus();

            toolTip1.SetToolTip(this.label6, "Con este conjunto de herramientas se pueden verificar las ejecuciones de\n" +
                                            "varios archivos de ejecución por lotes así como varios archivos con resultados.\" +" +
                                            "En la ventana de texto inferior se mostrarán las ejecuciones completas encontradas,\n" +
                                            "las ejecuciones que le faltaron repeticiones y las que no tienen ninguna información.");
            toolTip1.SetToolTip(this.panel3, "Con este conjunto de herramientas se pueden verificar las ejecuciones de\n" +
                                            "varios archivos de ejecución por lotes así como varios archivos con resultados.\" +" +
                                            "En la ventana de texto inferior se mostrarán las ejecuciones completas encontradas,\n" +
                                            "las ejecuciones que le faltaron repeticiones y las que no tienen ninguna información.");

            toolTip1.SetToolTip(this.button1, "Seleccionar uno o varios archivos de líneas de comandos para ejecución en lotes.\n" +
                                                "todas estas órdenes de ejecución serán verificadas en los archivos de resultados.");

            toolTip1.SetToolTip(this.button2, "Seleccionar uno o varios archivos de resultados de ejecución en lotes, para verificar\n" +
                                              "las diferentes órdenes de ejecución en los archivos de línea de comandos.");

            toolTip1.SetToolTip(this.button3, "Seleccionar un archivo para escribir el resumen del análisis realizado. Este resumen\n" +
                                              "es un archivo de texto plano que puede utilizarce para ejecutar nuevamente las opciones\n" +
                                              "que no están completas.");

            toolTip1.SetToolTip(this.label1, "Aquí se muestra el total de líneas de ejecución que fueron encontradas en todos los archivos\n" +
                                              "de línea de comandos seleccionados.");
            toolTip1.SetToolTip(this.textBox2, "Aquí se muestra el total de líneas de ejecución que fueron encontradas en todos los archivos\n" +
                                              "de línea de comandos seleccionados.");

            toolTip1.SetToolTip(this.label2, "Muestra el total de repeticiones completas encontradas en los archivos de resultados de\n" +
                                              "ejecución seleccionados.");
            toolTip1.SetToolTip(this.textBox3, "Muestra el total de repeticiones completas encontradas en los archivos de resultados de\n" +
                                              "ejecución seleccionados.");

            
            toolTip1.SetToolTip(this.label7, "Aquí puede seleccionar un archivo de ejecución por lotes y dividirlo en varios archivos.\n" +
                                            "Esto es muy útili pues permite la ejecución de varias instancias del optimizador para\n" +
                                            "aprovechar todo el recurso de cómputo del equipo donde se ejecuta (memoria y núcleas) del\n" +
                                            "procesador.");
            toolTip1.SetToolTip(this.panel2, "Aquí puede seleccionar un archivo de ejecución por lotes y dividirlo en varios archivos.\n" +
                                            "Esto es muy útili pues permite la ejecución de varias instancias del optimizador para\n" +
                                            "aprovechar todo el recurso de cómputo del equipo donde se ejecuta (memoria y núcleas) del\n" +
                                            "procesador.");

            toolTip1.SetToolTip(this.button4, "Selecciona un archivo de ejecución por lotes para dividirlo en varios archivos.");

            toolTip1.SetToolTip(this.textBox5, "Muestra el total líneas de ejecución que tiene el archivo de lotes seleccionado.");
            toolTip1.SetToolTip(this.label4, "Muestra el total líneas de ejecución que tiene el archivo de lotes seleccionado.");

            toolTip1.SetToolTip(this.numericUpDown1, "Seleccionar la cantidad de archivos a generar a partir del original. Las líneas\n" +
                                                      "líneas de comandos del archivo original son distribuidas equitativamente en los\n" +
                                                      "nuevos archivos creados, los cuales mantienen el nombre del archivo original más\n" +
                                                      "un número consecutivo.");
            toolTip1.SetToolTip(this.label3, "Seleccionar la cantidad de archivos a generar a partir del original. Las líneas\n" +
                                                      "líneas de comandos del archivo original son distribuidas equitativamente en los\n" +
                                                      "nuevos archivos creados, los cuales mantienen el nombre del archivo original más\n" +
                                                      "un número consecutivo.");

            toolTip1.SetToolTip(this.button5, "Realiza la creación de los nuevos archivos de líneas de ejecución. En el cuadro de texto inferior\n" +
                                               "se mostrará cada uno de los archivos creados y las líneas que fueron colocadas en él.");

            toolTip1.SetToolTip(this.label8, "Con estas herramientas puede seleccionar varios archivos de resultados de ejecuciones y unirlos\n" +
                                            "en un único archivo de resultados.");
            toolTip1.SetToolTip(this.panel1, "Con estas herramientas puede seleccionar varios archivos de resultados de ejecuciones y unirlos\n" +
                                            "en un único archivo de resultados.");

            toolTip1.SetToolTip(this.button6, "Selecciona varios archivos de resultados de ejecuciones del optimizador.");
            
            toolTip1.SetToolTip(this.textBox4, "Muestra la cantidad de líneas de ejecución encontradas en los archivos de resultados de\n" +
                                                "que se están uniendo en uno solo.");
            toolTip1.SetToolTip(this.label5, "Muestra la cantidad de líneas de ejecución encontradas en los archivos de resultados de\n" +
                                                "que se están uniendo en uno solo.");

            toolTip1.SetToolTip(this.button8, "Selecciona el archivo a crear en el que se integrarán todos los datos de ejecuciones de los archivos\n" +
                                                "seleccioandos para unir en uno solo. En la ventana de texto inferior se muestran todas las líneas de\n" +
                                                "ejecución que han sido integradas así como el nombre y carpeta del archivo que las contiene.");

            toolTip1.SetToolTip(this.button7, "Elimina de los archivos seleccionados las ejecuciones inconclusas. Es bueno realizar este proceso antes de unificarlos.");


        }

        private void button7_Click(object sender, EventArgs e)
        {
            StreamReader reader;
            StreamWriter writer;
            ArrayList para_escribir = new ArrayList();
            int repeticiones;
            int repeticiones_para_encontrar = -1;
            string carpeta_de_lectura;
            string archivo_original;
            long cantidad_de_repeticiones_completas_encontradas;
            string linea_leida;
            cantidad_de_repeticiones_completas_encontradas = 0;
            textBox4.Text = cantidad_de_repeticiones_completas_encontradas.ToString();
            textBox4.Refresh();
            ArrayList archivos_ordenados = new ArrayList(openFileDialog4.FileNames);
            archivos_ordenados.Sort();
            foreach (string archivos in archivos_ordenados)
            {
                carpeta_de_lectura = archivos;
                archivo_original = archivos;
                carpeta_de_lectura = carpeta_de_lectura.Substring(0, carpeta_de_lectura.LastIndexOf("\\") + 1);
                reader = new StreamReader(archivos);
                writer = new StreamWriter(carpeta_de_lectura + "Temporal.txt");
                para_escribir.Clear();
                repeticiones = 0;
                while (!reader.EndOfStream)
                {
                    linea_leida = reader.ReadLine().Trim();
                    para_escribir.Add(linea_leida);
                    if (linea_leida.Contains("Repeticion:"))
                        repeticiones++;
                    if (linea_leida.Contains("Linea de comandos:"))
                    {
                        string [] separados = linea_leida.Split(',');
                        repeticiones_para_encontrar = System.Convert.ToInt16(separados[1]);
                    }
                    if (repeticiones == repeticiones_para_encontrar)
                    {
                        cantidad_de_repeticiones_completas_encontradas++;
                        textBox4.Text = cantidad_de_repeticiones_completas_encontradas.ToString();
                        textBox4.Refresh();
                        linea_leida = reader.ReadLine().Trim();
                        para_escribir.Add(linea_leida);
                        linea_leida = reader.ReadLine().Trim();
                        para_escribir.Add(linea_leida);
                        linea_leida = reader.ReadLine().Trim();
                        para_escribir.Add(linea_leida);
                        linea_leida = reader.ReadLine().Trim();
                        para_escribir.Add(linea_leida);
                        linea_leida = reader.ReadLine().Trim();
                        para_escribir.Add(linea_leida);
                        foreach (string cadena in para_escribir)
                            writer.WriteLine(cadena);
                        writer.Flush();
                        para_escribir.Clear();
                        repeticiones = 0;
                        repeticiones_para_encontrar = -1;
                    }
                }
                reader.Close();
                writer.Close();
                File.SetAttributes(archivo_original, FileAttributes.Normal);
                File.Delete(archivo_original);
                File.Move(carpeta_de_lectura + "Temporal.txt", archivo_original);
            }
        }
    }
}
