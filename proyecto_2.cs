using System;
using System.Collections.Generic;
using System.Linq;

namespace CallCenterApp
{
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
        private Dictionary<string, Agente> tablaAgentes = new Dictionary<string, Agente>();

        private Dictionary<string, List<RutaEscalamiento>> grafoEscalamiento = new Dictionary<string, List<RutaEscalamiento>>();

        public bool RegistrarAgente(string codigo, string nombre, int nivel)
        {
            codigo = codigo.ToUpper();
            if (tablaAgentes.ContainsKey(codigo))
                return false;

            Agente nuevo = new Agente(codigo, nombre, nivel);
            tablaAgentes.Add(codigo, nuevo);
            grafoEscalamiento.Add(codigo, new List<RutaEscalamiento>());
            return true;
        }

        // Buscar  en la Tabla Hash 
        public Agente BuscarPorCodigo(string codigo)
        {
            codigo = codigo.ToUpper();
            tablaAgentes.TryGetValue(codigo, out Agente agente);
            return agente;
        }

        // Autocompletado
        public List<Agente> SugerirPorNombre(string texto)
        {
            return tablaAgentes.Values
                .Where(a => a.Nombre.StartsWith(texto, StringComparison.OrdinalIgnoreCase) ||
                            a.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Agregar arista dirigida al Grafo
        public string AgregarRutaEscalamiento(string codOrigen, string codDestino, int tiempo)
        {
            Agente origen = BuscarPorCodigo(codOrigen);
            Agente destino = BuscarPorCodigo(codDestino);

            if (origen == null || destino == null)
                return "Error: Uno o ambos agentes no existen.";

            if (origen.Nivel >= destino.Nivel)
                return $"Error: El escalamiento debe ir a un nivel superior ({origen.Nombre} es Nivel {origen.Nivel} y {destino.Nombre} es Nivel {destino.Nivel}).";

            // Evitar duplicados
            if (grafoEscalamiento[origen.Codigo].Any(r => r.Destino.Codigo == destino.Codigo))
                return "Error: Ya existe una ruta directa entre estos dos agentes.";

            grafoEscalamiento[origen.Codigo].Add(new RutaEscalamiento(destino, tiempo));
            return "Ruta de escalamiento registrada con éxito.";
        }

        // Algoritmo de Dijkstra: Ruta más rápida de A hacia B
        public void CalcularRutaMasRapida(string codInicio, string codFin)
        {
            codInicio = codInicio.ToUpper();
            codFin = codFin.ToUpper();

            if (!tablaAgentes.ContainsKey(codInicio) || !tablaAgentes.ContainsKey(codFin))
            {
                Console.WriteLine("Uno o ambos códigos de agente no existen.");
                return;
            }

            var tiempos = new Dictionary<string, int>();
            var previos = new Dictionary<string, string>();
            var colaPrioridad = new PriorityQueue<string, int>();

            foreach (var cod in tablaAgentes.Keys)
                tiempos[cod] = int.MaxValue;

            tiempos[codInicio] = 0;
            colaPrioridad.Enqueue(codInicio, 0);

            while (colaPrioridad.Count > 0)
            {
                string actual = colaPrioridad.Dequeue();

                if (actual == codFin) break; 

                foreach (var ruta in grafoEscalamiento[actual])
                {
                    int nuevoTiempo = tiempos[actual] + ruta.TiempoMinutos;
                    string vecino = ruta.Destino.Codigo;

                    if (nuevoTiempo < tiempos[vecino])
                    {
                        tiempos[vecino] = nuevoTiempo;
                        previos[vecino] = actual;
                        colaPrioridad.Enqueue(vecino, nuevoTiempo);
                    }
                }
            }

            // Reconstruir el camino
            if (tiempos[codFin] == int.MaxValue)
            {
                Console.WriteLine($"\nNo existe una ruta de escalamiento válida desde {codInicio} hasta {codFin}.");
                return;
            }

            var camino = new List<string>();
            string paso = codFin;
            while (paso != null)
            {
                camino.Add(tablaAgentes[paso].ToString());
                previos.TryGetValue(paso, out paso);
            }
            camino.Reverse();

            Console.WriteLine($"\n--- Ruta más rápida ({tiempos[codFin]} minutos en total) ---");
            Console.WriteLine(string.Join(" -> ", camino));
        }

        // Verificar si hay dos conexiones
        public bool PertenecenMismaCadena(string cod1, string cod2)
        {
            cod1 = cod1.ToUpper();
            cod2 = cod2.ToUpper();

            if (!tablaAgentes.ContainsKey(cod1) || !tablaAgentes.ContainsKey(cod2))
                return false;

            var visitados = new HashSet<string>();
            var cola = new Queue<string>();

            cola.Enqueue(cod1);
            visitados.Add(cod1);

            while (cola.Count > 0)
            {
                string actual = cola.Dequeue();
                if (actual == cod2) return true;

                foreach (var ruta in grafoEscalamiento[actual])
                {
                    if (!visitados.Contains(ruta.Destino.Codigo))
                    {
                        visitados.Add(ruta.Destino.Codigo);
                        cola.Enqueue(ruta.Destino.Codigo);
                    }
                }

                foreach (var par in grafoEscalamiento)
                {
                    if (par.Value.Any(r => r.Destino.Codigo == actual) && !visitados.Contains(par.Key))
                    {
                        visitados.Add(par.Key);
                        cola.Enqueue(par.Key);
                    }
                }
            }

            return false;
        }

        // Mostrar todo el grafo
        public void MostrarRed()
        {
            Console.WriteLine("\n--- Red de Escalamiento Actual ---");
            foreach (var codigo in tablaAgentes.Keys)
            {
                Agente ag = tablaAgentes[codigo];
                Console.Write($"{ag} escala hacia: ");
                var rutas = grafoEscalamiento[codigo];
                if (rutas.Count == 0)
                    Console.WriteLine("(Nadie)");
                else
                    Console.WriteLine(string.Join(", ", rutas.Select(r => $"{r.Destino.Nombre} [{r.Destino.Codigo}] ({r.TiempoMinutos} min)")));
            }
        }
    }

    // MENÚ 
    class Program
    {
        static void Main(string[] args)
        {
            SistemaCallCenter sistema = new SistemaCallCenter();
            CargarDatosDePrueba(sistema);

            bool salir = false;
            while (!salir)
            {
                Console.WriteLine("\n==============================================");
                Console.WriteLine("   SISTEMA DE ESCALAMIENTO DE CALL CENTER");
                Console.WriteLine("==============================================");
                Console.WriteLine("1. Registrar nuevo agente");
                Console.WriteLine("2. Agregar ruta de escalamiento");
                Console.WriteLine("3. Buscar agente por código (Tabla Hash)");
                Console.WriteLine("4. Autocompletar / Buscar agente por nombre");
                Console.WriteLine("5. Calcular ruta más rápida (Dijkstra)");
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

        // Datos precargados 
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