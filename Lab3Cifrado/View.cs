using System;
using System.Collections.Generic;
using System.Text;
using static Lab3Cifrado.Controller;
using static Lab3Cifrado.Model;
using System.Text.Encodings.Web;
using System.Linq;
using System.Text.Json.Serialization;
using System.IO;
using System.Text.Json;
using System.Linq;


namespace Lab3Cifrado
{
    class View
    {
        //COMPRESION usando metodo de transposicion por columna simple
        static string Compresion(string mensaje, string clave)
        {
            //Tamaño de la llave
            int filas = clave.Length;
            // Calculamos el número de columnas dividiendo la longitud del mensaje entre el número de filas
            int columnas = (int)Math.Ceiling((double)mensaje.Length / filas);
            //Se crea la matriz con las columanas calculadas
            char[,] matriz = new char[filas, columnas];
            //Variable k para seguir la posicion actual
            int k = 0;
            //Ciclo for para rellenar la matriz con caracteres del mensaje
            for (int i = 0; i < columnas; i++)
            {
                for (int j = 0; j < filas; j++)
                {
                    //Comprobar si hay caracteres en el mensaje
                    if (k < mensaje.Length)
                        matriz[j, i] = mensaje[k++];
                    else
                        matriz[j, i] = '_'; // Rellenamos con caracteres vacíos
                }
            }
            //Se genera el StringBuilder para reconstruir la cadena cifrada
            StringBuilder cifrado = new StringBuilder();
            //Se recorre la matriz para construir la cadena cifrada por medio de sus elementos
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    cifrado.Append(matriz[i, j]);
                }
            }
            // Devolvemos la cadena cifrada como una cadena de texto.
            return cifrado.ToString();
        }

        // Función para DESCOMPRIMIR un mensaje cifrado con transposición por columna simple
        static string Descompresion(string cifrado, string clave)
        {
            //Tamaño de la llave
            int filas = clave.Length;
            // Calculamos el número de columnas dividiendo la longitud del mensaje entre el número de filas

            int columnas = cifrado.Length / filas;

            //Se crea la matriz con las columanas calculadas

            char[,] matriz = new char[filas, columnas];
            //Se crea variable k, para hacer seguimiento
            int k = 0;
            
            // Llenamos la matriz con caracteres de la cadena cifrada.

            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    matriz[i, j] = cifrado[k++];
                }
            }
            //Creamos el string para la cadena
            StringBuilder mensaje = new StringBuilder();
            //Se recorre la matriz para construir la cadena
            for (int i = 0; i < columnas; i++)
            {
                for (int j = 0; j < filas; j++)
                {
                    mensaje.Append(matriz[j, i]);
                }
            }
            //Se convierte la cadena en string y se eliminan caracteres vacios
            return mensaje.ToString().Replace("_", ""); // Eliminamos caracteres vacíos
        }

        public static void Mostrar()
        {
            //Arbol AVL
            AVLTree arbolPersonas = new AVLTree();
            List<Persona> personas = new List<Persona>();
            //Ruta en donde se encuentran los clientes
            string route = @"C:\Users\usuario\inputslab3\inputs3.csv";

            if (File.Exists(route))
            {
                string[] FileData = File.ReadAllLines(route);
                foreach (var item in FileData)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        //Se lee por lineas
                        string[] valor = item.Split(";");

                        string valorSinEscape = valor[1].Replace("\u0027", " ");
                        Persona persona = JsonSerializer.Deserialize<Persona>(valorSinEscape);
                        //Comando insertt en donde se van a insertar los clientes
                        if (valor[0] == "INSERT")
                        {
                            arbolPersonas.Add(persona);
                        }
                        //Borrar clientes
                        else if (valor[0] == "DELETE")
                        {
                            arbolPersonas.Delete(persona);
                        }
                        //Actualizar datos de clientes
                        else if (valor[0] == "PATCH")
                        {
                            arbolPersonas.Patch(persona);
                        }

                    }
                }

                personas.Clear();
                Console.WriteLine("");
                //Obtener todas las personas
                List<Persona> todasLasPersonas = arbolPersonas.GetAllPersons();
                Console.WriteLine("ARBOL:");
                //Visualizar todo el arbol completo
                foreach (var persona in todasLasPersonas)
                {
                    string serializedPersona = JsonSerializer.Serialize(persona);

                    Console.WriteLine(JsonSerializer.Serialize(persona));
                    Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------");

                }
                //Busqueda de DPI
                Console.Write("Ingrese el DPI del cliente que desea buscar: ");
                string dpiABuscar = Console.ReadLine();

                List<Persona> resultados = new List<Persona>();
                arbolPersonas.QueryResults(arbolPersonas.Root, new Persona { dpi = dpiABuscar }, resultados);

                // ...

                //Resultado al buscar por DPI

                if (resultados.Count > 0)
                {
                    Console.WriteLine("----------------------------------------------------------------------");
                    Console.WriteLine("Resultados de la búsqueda:");
                    Console.WriteLine("");

                    foreach (var resultado in resultados)
                    {
                        string companiesJson = JsonSerializer.Serialize(resultado.companies);
                        Console.WriteLine("name: " + resultado.name + ", dpi: " + resultado.dpi + ", dateBirth: " + resultado.datebirth + ", address: " + resultado.address + ", companies: " + companiesJson);

                        Console.WriteLine("----------------------------------------------------------------------");

                        // Buscar archivos de texto correspondientes al DPI
                        string dpiFileName = "REC-" + resultado.dpi + "-";
                        string inputFolderPath = @"C:\Users\usuario\inputslab3\inputs"; // Ruta de la carpeta "inputs"

                        DirectoryInfo directoryInfo = new DirectoryInfo(inputFolderPath);
                        FileInfo[] files = directoryInfo.GetFiles(dpiFileName + "*.txt");

                        if (files.Length > 0)
                        {
                            Console.WriteLine("Cartas encontradas:");

                            foreach (var file in files)
                            {
                                //Clave para realizar la transposicion
                                string clave = "MILLAVE";
                                //Llamando metodos de comprension y descompresion
                                string cartaContenido = File.ReadAllText(file.FullName);
                                string compresion = Compresion(cartaContenido, clave);
                                string descompresion = Descompresion(compresion, clave);

                                Console.WriteLine("Nombre del archivo: " + file.Name);
                                Console.WriteLine("Contenido de la carta:\n" + cartaContenido);
                                Console.WriteLine("----------------------------------------------------------------------");
                                Console.WriteLine("COMPRESION de la carta:\n" + compresion);
                                Console.WriteLine("----------------------------------------------------------------------");
                                Console.WriteLine("DESCOMPRESION de la carta:\n" + descompresion);
                                Console.WriteLine("----------------------------------------------------------------------");

                                // Guardar la carta en un archivo separado
                                string cartaOutputFileName = "CartaNormal_" + resultado.dpi + "_" + Path.GetFileNameWithoutExtension(file.Name) + ".txt";
                                string cartaOutputFileNameComprimido = "CartaComprimida_" + resultado.dpi + "_" + Path.GetFileNameWithoutExtension(file.Name) + ".txt";
                                string cartaOutputFileNameDescomprimido = "CartaDescomprimida_" + resultado.dpi + "_" + Path.GetFileNameWithoutExtension(file.Name) + ".txt";

                                string cartaOutputPath = @"C:\Users\usuario\inputslab3"; // Ruta de la carpeta donde se guardarán las cartas

                                if (!Directory.Exists(cartaOutputPath))
                                {
                                    Directory.CreateDirectory(cartaOutputPath);
                                }

                                string cartaOutputFilePath = Path.Combine(cartaOutputPath, cartaOutputFileName);
                                string cartaOutputFilePathComprimido = Path.Combine(cartaOutputPath, cartaOutputFileNameComprimido);
                                string cartaOutputFilePathDescomprimido = Path.Combine(cartaOutputPath, cartaOutputFileNameDescomprimido);

                                File.WriteAllText(cartaOutputFilePath, cartaContenido);

                                File.WriteAllText(cartaOutputFilePathComprimido, compresion);

                                File.WriteAllText(cartaOutputFilePathDescomprimido, descompresion);
                                //Ubicacion en donde se guardaron las cartas
                                Console.WriteLine("La carta normal se ha guardado en: " + cartaOutputFilePath);
                                Console.WriteLine("La carta comprimida se ha guardado en: " + cartaOutputFilePathComprimido);
                                Console.WriteLine("La carta descomprimida se ha guardado en: " + cartaOutputFilePathDescomprimido);

                                Console.WriteLine("----------------------------------------------------------------------");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No se encontraron cartas para este DPI.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron resultados para el DPI ingresado.");
                }
            }
        }
    }
}
