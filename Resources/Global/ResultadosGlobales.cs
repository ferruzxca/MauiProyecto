using System.Collections.Generic;
using System.Linq;

namespace MauiProyecto.Global
{
    public static class ResultadosGlobales
    {
        // Diccionario para almacenar aciertos y errores por materia
        private static Dictionary<string, (int aciertos, int errores)> resultadosPorMateria = new Dictionary<string, (int aciertos, int errores)>();

        // Método para inicializar el diccionario con todas las materias sin acentos
        public static void InicializarMaterias()
        {
            var materias = new List<string>
            {
                "Espanol", "Matematicas", "Biologia", "Quimica",
                "Fisica", "Historia", "Geografia", "Computacion",
                "Logica", "Civica"
            };

            foreach (var materia in materias)
            {
                // Inicializamos con aciertos y errores en 0
                if (!resultadosPorMateria.ContainsKey(materia))
                {
                    resultadosPorMateria[materia] = (0, 0);
                }
            }
        }

        // Método para agregar resultados de una materia
        public static void AgregarResultado(string materia, int aciertos, int errores)
        {
            // Si la materia ya existe, sumamos los aciertos y errores a los ya existentes
            if (resultadosPorMateria.ContainsKey(materia))
            {
                var resultadoActual = resultadosPorMateria[materia];
                resultadosPorMateria[materia] = (resultadoActual.aciertos + aciertos, resultadoActual.errores + errores);
            }
            else
            {
                // Si no existe, la agregamos con los aciertos y errores proporcionados
                resultadosPorMateria[materia] = (aciertos, errores);
            }
        }

        // Obtener resultados globales sumando todos los aciertos y errores
        public static (int totalAciertos, int totalErrores) ObtenerResultadosGlobales()
        {
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

        // Método para calcular la calificación final (porcentaje de aciertos globales)
        public static double CalcularCalificacionFinal()
        {
            var (totalAciertos, totalErrores) = ObtenerResultadosGlobales();
            int totalPreguntas = totalAciertos + totalErrores;

            // Calcular porcentaje de aciertos
            return totalPreguntas > 0 ? ((double)totalAciertos / totalPreguntas) * 100 : 0;
        }

        



    }
}
