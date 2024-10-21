using System.Collections.Generic;
using System.Linq;

namespace MauiProyecto.Global
{
    public static class ResultadosGlobales
    {
        // Diccionario para almacenar aciertos y errores por materia
        private static Dictionary<string, (int aciertos, int errores)> resultadosPorMateria = new Dictionary<string, (int aciertos, int errores)>();

        // Método para agregar resultados de una materia
        public static void AgregarResultado(string materia, int aciertos, int errores)
        {
            // Verificamos si ya existe la materia en el diccionario
            if (resultadosPorMateria.ContainsKey(materia))
            {
                // Sumamos los aciertos y errores al valor existente
                var resultadoActual = resultadosPorMateria[materia];
                resultadosPorMateria[materia] = (resultadoActual.aciertos + aciertos, resultadoActual.errores + errores);
            }
            else
            {
                // Si es la primera vez que se agrega la materia, simplemente la añadimos
                resultadosPorMateria[materia] = (aciertos, errores);
            }
        }

        // Obtener resultados globales sumando todos los aciertos y errores
        public static (int totalAciertos, int totalErrores) ObtenerResultadosGlobales()
        {
            // Sumamos todos los aciertos y errores de cada materia
            int totalAciertos = resultadosPorMateria.Sum(r => r.Value.aciertos);
            int totalErrores = resultadosPorMateria.Sum(r => r.Value.errores);
            return (totalAciertos, totalErrores);
        }

        // Obtener los resultados por materia
        public static Dictionary<string, (int aciertos, int errores)> ObtenerResultadosPorMateria()
        {
            return resultadosPorMateria;
        }

        // Obtener la materia con más aciertos
        public static string MateriaConMasAciertos()
        {
            if (resultadosPorMateria.Count == 0)
                return "No hay datos";

            return resultadosPorMateria.OrderByDescending(r => r.Value.aciertos).FirstOrDefault().Key;
        }

        // Obtener la materia con más errores
        public static string MateriaConMasErrores()
        {
            if (resultadosPorMateria.Count == 0)
                return "No hay datos";

            return resultadosPorMateria.OrderByDescending(r => r.Value.errores).FirstOrDefault().Key;
        }

        // Obtener la materia con menos aciertos
        public static string MateriaConMenosAciertos()
        {
            if (resultadosPorMateria.Count == 0)
                return "No hay datos";

            return resultadosPorMateria.OrderBy(r => r.Value.aciertos).FirstOrDefault().Key;
        }

        // Obtener la materia con menos errores
        public static string MateriaConMenosErrores()
        {
            if (resultadosPorMateria.Count == 0)
                return "No hay datos";

            return resultadosPorMateria.OrderBy(r => r.Value.errores).FirstOrDefault().Key;
        }
    }
}
