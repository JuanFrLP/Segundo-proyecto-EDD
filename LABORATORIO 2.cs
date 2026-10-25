using System;
using System.Collections.Generic;
using System.Linq;

namespace CallCenterApp
{
    public class NodoHash
    {
        public string Clave;
        public Agente Valor;
        public NodoHash Siguiente;

        public NodoHash(string clave, Agente valor)
        {
            Clave = clave;
            Valor = valor;
            Siguiente = null;
        }
    }
    public class TablaHash
    {
        private int capacidad;
        private NodoHash[] tabla;
        public int Tamanio { get; private set; }

        public TablaHash(int capacidad = 100)
        {
            this.capacidad = capacidad;
            this.tabla = new NodoHash[capacidad];
            this.Tamanio = 0;
        }
        private int ObtenerHash(string clave)
        {
            long hashVal = 0;
            foreach (char c in clave)
            {
                hashVal = (hashVal * 31 + c) % capacidad;
            }
            return (int)hashVal;
        }
        public void Insertar(string clave, Agente valor)
        {
            int indice = ObtenerHash(clave);
            NodoHash actual = tabla[indice];
            while (actual != null)
            {
                if (actual.Clave == clave)
                {
                    actual.Valor = valor;
                    return;
                }
                actual = actual.Siguiente;
            }

            NodoHash nuevoNodo = new NodoHash(clave, valor);
            nuevoNodo.Siguiente = tabla[indice];
            tabla[indice] = nuevoNodo;
            Tamanio++;
        }
        public Agente Buscar(string clave)
        {
            int indice = ObtenerHash(clave);
            NodoHash actual = tabla[indice];
            while (actual != null)
            {
                if (actual.Clave == clave) return actual.Valor;
                actual = actual.Siguiente;
            }
            return null;
        }
        public bool Eliminar(string clave)
        {
            int indice = ObtenerHash(clave);
            NodoHash actual = tabla[indice];
            NodoHash anterior = null;

            while (actual != null)
            {
                if (actual.Clave == clave)
                {
                    if (anterior == null)
                        tabla[indice] = actual.Siguiente;
                    else
                        anterior.Siguiente = actual.Siguiente;
                    Tamanio--;
                    return true;
                }
                anterior = actual;
                actual = actual.Siguiente;
            }
            return false;
        }
    }
    public class NodoTrie
    {
        public NodoTrie[] Hijos { get; set; }
        public bool EsFin { get; set; }
        public string[] CodigosAsociados { get; set; }
        public int TotalCodigos { get; set; }

        public NodoTrie()
        {
            Hijos = new NodoTrie[256];
            EsFin = false;
            CodigosAsociados = new string[10];
            TotalCodigos = 0;
        }
        public void AgregarCodigo(string codigo)
        {
            for (int i = 0; i < TotalCodigos; i++)
            {
                if (CodigosAsociados[i] == codigo) return;
            }
            if (TotalCodigos >= CodigosAsociados.Length)
            {
                string[] nuevoArr = new string[CodigosAsociados.Length * 2];
                for (int i = 0; i < TotalCodigos; i++) nuevoArr[i] = CodigosAsociados[i];
                CodigosAsociados = nuevoArr;
            }
            CodigosAsociados[TotalCodigos++] = codigo;
        }
    }
    public class Trie
    {
        private NodoTrie raiz;

        public Trie()
        {
            raiz = new NodoTrie();
        }
        public void Insertar(string nombre, string codigo)
        {
            NodoTrie actual = raiz;
            string nombreLower = nombre.ToLower();
            foreach (char c in nombreLower)
            {
                int index = (int)c;
                if (index < 0 || index >= 256) continue;
                if (actual.Hijos[index] == null)
                {
                    actual.Hijos[index] = new NodoTrie();
                }
                actual = actual.Hijos[index];
                actual.AgregarCodigo(codigo);
            }
            actual.EsFin = true;
        }
        public string[] BuscarPorPrefijo(string prefijo, out int totalEncontrados)
        {
            NodoTrie actual = raiz;
            string prefijoLower = prefijo.ToLower();
            foreach (char c in prefijoLower)
            {
                int index = (int)c;
                if (index < 0 || index >= 256 || actual.Hijos[index] == null)
                {
                    totalEncontrados = 0;
                    return new string[0];
                }
                actual = actual.Hijos[index];
            }
            string[] resultados = new string[50];
            totalEncontrados = 0;
            RecolectarRecursivo(actual, ref resultados, ref totalEncontrados);
            return resultados;
        }
        private void RecolectarRecursivo(NodoTrie nodo, ref string[] resultados, ref int totalEncontrados)
        {
            if (nodo == null) return;
            for (int i = 0; i < nodo.TotalCodigos; i++)
            {
                string cod = nodo.CodigosAsociados[i];
                bool duplicado = false;
                for (int j = 0; j < totalEncontrados; j++)
                {
                    if (resultados[j] == cod) { duplicado = true; break; }
                }
                if (!duplicado)
                {
                    if (totalEncontrados >= resultados.Length)
                    {
                        string[] nuevoArr = new string[resultados.Length * 2];
                        for (int k = 0; k < totalEncontrados; k++) nuevoArr[k] = resultados[k];
                        resultados = nuevoArr;
                    }
                    resultados[totalEncontrados++] = cod;
                }
            }

            for (int i = 0; i < 256; i++)
            {
                if (nodo.Hijos[i] != null)
                {
                    RecolectarRecursivo(nodo.Hijos[i], ref resultados, ref totalEncontrados);
                }
            }
        }
    }
    public class Grafo
    {
        private string[] vertices;
        private int totalVertices;
        private RutaEscalamiento[][] adyacencias;
        public Grafo(int capacidadMax = 100)
        {
            vertices = new string[capacidadMax];
            adyacencias = new RutaEscalamiento[capacidadMax][];
            totalVertices = 0;
        }
        public void AgregarVertice(string codigo)
        {
            if (ObtenerIndiceVertice(codigo) != -1) return;
            if (totalVertices >= vertices.Length)
            {
                string[] nuevoV = new string[vertices.Length * 2];
                RutaEscalamiento[][] nuevoA = new RutaEscalamiento[vertices.Length * 2][];
                for (int i = 0; i < totalVertices; i++)
                {
                    nuevoV[i] = vertices[i];
                    nuevoA[i] = adyacencias[i];
                }
                vertices = nuevoV;
                adyacencias = nuevoA;
            }
            vertices[totalVertices] = codigo;
            adyacencias[totalVertices] = new RutaEscalamiento[5];
            totalVertices++;
        }
        public int ObtenerIndiceVertice(string codigo)
        {
            for (int i = 0; i < totalVertices; i++)
            {
                if (vertices[i] == codigo) return i;
            }
            return -1;
        }
        public void RegistrarRuta(string origen, RutaEscalamiento ruta)
        {
            AgregarVertice(origen);
            AgregarVertice(ruta.Destino.Codigo);
            int idxOrig = ObtenerIndiceVertice(origen);

            RutaEscalamiento[] arr = adyacencias[idxOrig];
            int count = 0;
            while (count < arr.Length && arr[count] != null) count++;

            if (count >= arr.Length)
            {
                RutaEscalamiento[] nuevoArr = new RutaEscalamiento[arr.Length * 2];
                for (int i = 0; i < arr.Length; i++) nuevoArr[i] = arr[i];
                adyacencias[idxOrig] = nuevoArr;
                arr = nuevoArr;
            }
            arr[count] = ruta;
        }
        public RutaEscalamiento[] ObtenerRutas(string codigo)
        {
            int idx = ObtenerIndiceVertice(codigo);
            if (idx == -1) return new RutaEscalamiento[0];
            
            int count = 0;
            RutaEscalamiento[] arr = adyacencias[idx];
            while (count < arr.Length && arr[count] != null) count++;

            RutaEscalamiento[] resultado = new RutaEscalamiento[count];
            for (int i = 0; i < count; i++) resultado[i] = arr[i];
            return resultado;
        }
        public (string[] ruta, int tiempoTotal) Dijkstra(string inicio, string destino)
        {
            int idxInicio = ObtenerIndiceVertice(inicio);
            int idxDestino = ObtenerIndiceVertice(destino);
            if (idxInicio == -1 || idxDestino == -1) return (new string[0], int.MaxValue);

            int[] distancias = new int[totalVertices];
            int[] predecesores = new int[totalVertices];
            bool[] visitados = new bool[totalVertices];

            for (int i = 0; i < totalVertices; i++)
            {
                distancias[i] = int.MaxValue;
                predecesores[i] = -1;
            }
            distancias[idxInicio] = 0;

            for (int count = 0; count < totalVertices - 1; count++)
            {
                int min = int.MaxValue;
                int u = -1;

                for (int v = 0; v < totalVertices; v++)
                {
                    if (!visitados[v] && distancias[v] <= min)
                    {
                        min = distancias[v];
                        u = v;
                    }
                }

                if (u == -1 || min == int.MaxValue) break;
                visitados[u] = true;

                if (u == idxDestino) break;

                RutaEscalamiento[] arr = adyacencias[u];
                for (int i = 0; i < arr.Length && arr[i] != null; i++)
                {
                    string vecinoStr = arr[i].Destino.Codigo;
                    int peso = arr[i].TiempoMinutos;
                    int vIdx = ObtenerIndiceVertice(vecinoStr);

                    if (!visitados[vIdx] && distancias[u] + peso < distancias[vIdx])
                    {
                        distancias[vIdx] = distancias[u] + peso;
                        predecesores[vIdx] = u;
                    }
                }
            }

            if (distancias[idxDestino] == int.MaxValue)
            {
                return (new string[0], int.MaxValue);
            }

            string[] tempRuta = new string[totalVertices];
            int rutaCount = 0;
            int actual = idxDestino;
            while (actual != -1)
            {
                tempRuta[rutaCount++] = vertices[actual];
                actual = predecesores[actual];
            }

            string[] rutaFinal = new string[rutaCount];
            for (int i = 0; i < rutaCount; i++)
            {
                rutaFinal[i] = tempRuta[rutaCount - 1 - i];
            }

            return (rutaFinal, distancias[idxDestino]);
        }

        public void MostrarRed(TablaHash tablaAgentes)
        {
            for (int i = 0; i < totalVertices; i++)
            {
                string codigo = vertices[i];
                Agente ag = tablaAgentes.Buscar(codigo);
                if (ag == null) continue;

                Console.Write($"{ag} escala hacia: ");
                RutaEscalamiento[] rutas = ObtenerRutas(codigo);
                if (rutas.Length == 0)
                    Console.WriteLine("(Nadie)");
                else
                {
                    string[] descripciones = new string[rutas.Length];
                    for (int j = 0; j < rutas.Length; j++)
                    {
                        descripciones[j] = $"{rutas[j].Destino.Nombre} [{rutas[j].Destino.Codigo}] ({rutas[j].TiempoMinutos} min)";
                    }
                    Console.WriteLine(string.Join(", ", descripciones));
                }
            }
        }
    }

    public class UnionFind
    {
        private TablaHash padreMap = new TablaHash(200);

        public void HacerConjunto(string i)
        {
            if (padreMap.Buscar(i) == null)
            {
                padreMap.Insertar(i, new Agente(i, i, 0));
            }
        }

        public string Encontrar(string i)
        {
            HacerConjunto(i);
            Agente p = padreMap.Buscar(i);
            if (p.Nombre == i)
            {
                return i;
            }
            string raiz = Encontrar(p.Nombre);
            padreMap.Insertar(i, new Agente(i, raiz, 0)); // Compresión de camino
            return raiz;
        }

        public bool Unir(string i, string j)
        {
            string raizI = Encontrar(i);
            string raizJ = Encontrar(j);

            if (raizI != raizJ)
            {
                padreMap.Insertar(raizI, new Agente(raizI, raizJ, 0));
                return true;
            }
            return false;
        }

        public bool EstanConectados(string i, string j)
        {
            HacerConjunto(i);
            HacerConjunto(j);
            return Encontrar(i) == Encontrar(j);
        }
    }
    public class Agente
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int Nivel { get; set; } 

        public Agente(string codigo, string nombre, int nivel)
        {
            Codigo = codigo.ToUpper();
            Nombre = nombre;
            Nivel = nivel;
        }

        public override string ToString()
        {
            return $"[{Codigo}] {Nombre} (Nivel {Nivel})";
        }
    }

    public class RutaEscalamiento
    {
        public Agente Destino { get; set; }
        public int TiempoMinutos { get; set; }

        public RutaEscalamiento(Agente destino, int tiempoMinutos)
        {
            Destino = destino;
            TiempoMinutos = tiempoMinutos;
        }
    }

    public class SistemaCallCenter
    {
        private TablaHash tablaAgentes = new TablaHash();
        private Grafo grafoEscalamiento = new Grafo();
        private Trie trieAgentes = new Trie();
        private UnionFind unionFindRed = new UnionFind();

        public bool RegistrarAgente(string codigo, string nombre, int nivel)
        {
            codigo = codigo.ToUpper();
            if (tablaAgentes.Buscar(codigo) != null)
                return false;

            Agente nuevo = new Agente(codigo, nombre, nivel);
            tablaAgentes.Insertar(codigo, nuevo);
            trieAgentes.Insertar(nombre, codigo);
            grafoEscalamiento.AgregarVertice(codigo);
            unionFindRed.HacerConjunto(codigo);
            return true;
        }

        public Agente BuscarPorCodigo(string codigo)
        {
            codigo = codigo.ToUpper();
            return tablaAgentes.Buscar(codigo);
        }

        public List<Agente> SugerirPorNombre(string texto)
        {
            string[] codigos = trieAgentes.BuscarPorPrefijo(texto, out int total);
            List<Agente> sugerencias = new List<Agente>();
            for (int i = 0; i < total; i++)
            {
                Agente ag = tablaAgentes.Buscar(codigos[i]);
                if (ag != null)
                {
                    sugerencias.Add(ag);
                }
            }
            return sugerencias;
        }

        public string AgregarRutaEscalamiento(string codOrigen, string codDestino, int tiempo)
        {
            codOrigen = codOrigen.ToUpper();
            codDestino = codDestino.ToUpper();

            Agente origen = BuscarPorCodigo(codOrigen);
            Agente destino = BuscarPorCodigo(codDestino);

            if (origen == null || destino == null)
                return "Error: Uno o ambos agentes no existen.";

            if (origen.Nivel >= destino.Nivel)
                return $"Error: El escalamiento debe ir a un nivel superior ({origen.Nombre} es Nivel {origen.Nivel} y {destino.Nombre} es Nivel {destino.Nivel}).";

            RutaEscalamiento[] rutasExistentes = grafoEscalamiento.ObtenerRutas(codOrigen);
            for (int i = 0; i < rutasExistentes.Length; i++)
            {
                if (rutasExistentes[i].Destino.Codigo == codDestino)
                    return "Error: Ya existe una ruta directa entre estos dos agentes.";
            }

            RutaEscalamiento nuevaRuta = new RutaEscalamiento(destino, tiempo);
            grafoEscalamiento.RegistrarRuta(codOrigen, nuevaRuta);
            unionFindRed.Unir(codOrigen, codDestino);
            return "Ruta de escalamiento registrada con éxito.";
        }

        public void CalcularRutaMasRapida(string codInicio, string codFin)
        {
            codInicio = codInicio.ToUpper();
            codFin = codFin.ToUpper();

            if (tablaAgentes.Buscar(codInicio) == null || tablaAgentes.Buscar(codFin) == null)
            {
                Console.WriteLine("Uno o ambos códigos de agente no existen.");
                return;
            }

            if (!unionFindRed.EstanConectados(codInicio, codFin))
            {
                Console.WriteLine($"\nNo existe una ruta de escalamiento válida desde {codInicio} hasta {codFin} (Verificado por Union-Find).");
                return;
            }

            var (rutaCodigos, tiempoTotal) = grafoEscalamiento.Dijkstra(codInicio, codFin);

            if (rutaCodigos.Length == 0 || tiempoTotal == int.MaxValue)
            {
                Console.WriteLine($"\nNo existe una ruta de escalamiento válida desde {codInicio} hasta {codFin}.");
                return;
            }

            var camino = new List<string>();
            for (int i = 0; i < rutaCodigos.Length; i++)
            {
                Agente ag = tablaAgentes.Buscar(rutaCodigos[i]);
                if (ag != null)
                {
                    camino.Add(ag.ToString());
                }
            }

            Console.WriteLine($"\n--- Ruta más rápida ({tiempoTotal} minutos en total) ---");
            Console.WriteLine(string.Join(" -> ", camino));
        }

        public bool PertenecenMismaCadena(string cod1, string cod2)
        {
            cod1 = cod1.ToUpper();
            cod2 = cod2.ToUpper();

            if (tablaAgentes.Buscar(cod1) == null || tablaAgentes.Buscar(cod2) == null)
                return false;

            return unionFindRed.EstanConectados(cod1, cod2);
        }

        public void MostrarRed()
        {
            Console.WriteLine("\n--- Red de Escalamiento Actual ---");
            grafoEscalamiento.MostrarRed(tablaAgentes);
        }
    }
 
    class Program
    {
        static void Main(string[] args)
        {
            SistemaCallCenter sistema = new SistemaCallCenter();
            CargarDatosDePrueba(sistema);

            bool salir = false;
            while (!salir)
            {
                Console.WriteLine("\n----------------------------------------------");
                Console.WriteLine("   SISTEMA DE CALL CENTER");
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine("1. Registrar nuevo agente");
                Console.WriteLine("2. Agregar ruta de escalamiento");
                Console.WriteLine("3. Buscar agente por código (Tabla Hash)");
                Console.WriteLine("4. Autocompletar / Buscar agente por nombre (Trie)");
                Console.WriteLine("5. Calcular ruta más rápida entre agentes (Dijkstra)");
                Console.WriteLine("6. Verificar si 2 agentes están en la misma cadena");
                Console.WriteLine("7. Ver toda la red de agentes");
                Console.WriteLine("0. Salir");
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine();
                Console.WriteLine();

                switch (opcion)
                {
                    case "1":
                        Console.Write("Código del agente (ej. A06): ");
                        string cod = Console.ReadLine();
                        Console.Write("Nombre del agente: ");
                        string nom = Console.ReadLine();
                        Console.Write("Nivel de especialidad (1, 2, 3...): ");
                        if (int.TryParse(Console.ReadLine(), out int nivel))
                        {
                            if (sistema.RegistrarAgente(cod, nom, nivel))
                                Console.WriteLine("Agente registrado correctamente.");
                            else
                                Console.WriteLine("Error: El código ya existe.");
                        }
                        break;

                    case "2":
                        Console.Write("Código del agente origen (menor nivel): ");
                        string orig = Console.ReadLine();
                        Console.Write("Código del agente destino (mayor nivel): ");
                        string dest = Console.ReadLine();
                        Console.Write("Tiempo promedio de resolución (minutos): ");
                        if (int.TryParse(Console.ReadLine(), out int tiempo))
                        {
                            Console.WriteLine(sistema.AgregarRutaEscalamiento(orig, dest, tiempo));
                        }
                        break;

                    case "3":
                        Console.Write("Ingrese el código a buscar: ");
                        Agente encontrado = sistema.BuscarPorCodigo(Console.ReadLine());
                        Console.WriteLine(encontrado != null ? $"Encontrado: {encontrado}" : "Agente no encontrado.");
                        break;

                    case "4":
                        Console.Write("Escriba las primeras letras del nombre: ");
                        var sugerencias = sistema.SugerirPorNombre(Console.ReadLine());
                        if (sugerencias.Count > 0)
                            sugerencias.ForEach(a => Console.WriteLine($" -> {a}"));
                        else
                            Console.WriteLine("No hay coincidencias.");
                        break;

                    case "5":
                        Console.Write("Código del agente inicial: ");
                        string ini = Console.ReadLine();
                        Console.Write("Código del especialista final: ");
                        string fin = Console.ReadLine();
                        sistema.CalcularRutaMasRapida(ini, fin);
                        break;

                    case "6":
                        Console.Write("Código del primer agente: ");
                        string c1 = Console.ReadLine();
                        Console.Write("Código del segundo agente: ");
                        string c2 = Console.ReadLine();
                        bool conectados = sistema.PertenecenMismaCadena(c1, c2);
                        Console.WriteLine(conectados
                            ? "SÍ pertenecen a la misma cadena de escalamiento (Pueden compartir caso)."
                            : "NO tienen conexión en la red de escalamiento.");
                        break;

                    case "7":
                        sistema.MostrarRed();
                        break;

                    case "0":
                        salir = true;
                        break;

                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }
            }
        }

        static void CargarDatosDePrueba(SistemaCallCenter s)
        {
            s.RegistrarAgente("A01", "Ana Perez", 1);
            s.RegistrarAgente("A02", "Carlos Gomez", 2);
            s.RegistrarAgente("A03", "Carlos Ruiz", 2);
            s.RegistrarAgente("A04", "Elena Torres", 3);

            s.RegistrarAgente("B01", "Pedro Diaz", 1);
            s.RegistrarAgente("B02", "Sofia Castro", 2);

            s.AgregarRutaEscalamiento("A01", "A02", 15);
            s.AgregarRutaEscalamiento("A02", "A04", 20);
            s.AgregarRutaEscalamiento("A01", "A03", 5);
            s.AgregarRutaEscalamiento("A03", "A04", 10);

            s.AgregarRutaEscalamiento("B01", "B02", 12);
        }
    }
}