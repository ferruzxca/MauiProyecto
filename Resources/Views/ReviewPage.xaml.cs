using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Maui.Controls;
using Newtonsoft.Json.Linq;
using MauiProyecto.Global;

namespace MauiProyecto
{
    public partial class ReviewPage : TabbedPage
    {
        public ReviewPage()
        {
            InitializeComponent();

            // Inicializar las materias en ResultadosGlobales antes de cargar las pestañas
            ResultadosGlobales.InicializarMaterias();

            // Cargamos las preguntas y generamos las pestañas por cada materia
            CargarPreguntasYGenerarPestanas();

            // Crear pestaña de Resultado Global
            CrearPestanaResultadoGlobal();
        }

        // Método para cargar todas las preguntas desde los archivos .json y generar las pestañas
        private void CargarPreguntasYGenerarPestanas()
        {
            var materiasArchivos = new Dictionary<string, string>
            {
                { "Espanol", "MauiProyecto.Resources.preguntas.preguntas_espanol.json" },
                { "Matematicas", "MauiProyecto.Resources.preguntas.preguntas_matematicas.json" },
                { "Biologia", "MauiProyecto.Resources.preguntas.preguntas_biologia.json" },
                { "Quimica", "MauiProyecto.Resources.preguntas.preguntas_quimica.json" },
                { "Fisica", "MauiProyecto.Resources.preguntas.preguntas_fisica.json" },
                { "Historia", "MauiProyecto.Resources.preguntas.preguntas_historia.json" },
                { "Geografia", "MauiProyecto.Resources.preguntas.preguntas_geografia.json" },
                { "Computacion", "MauiProyecto.Resources.preguntas.preguntas_computacion.json" },
                { "Logica", "MauiProyecto.Resources.preguntas.preguntas_logica.json" },
                { "Civica", "MauiProyecto.Resources.preguntas.preguntas_civica.json" }
            };

            foreach (var materiaArchivo in materiasArchivos)
            {
                string materia = materiaArchivo.Key;
                string archivoJson = materiaArchivo.Value;

                var preguntas = CargarPreguntasDesdeArchivo(archivoJson);
                if (preguntas != null)
                {
                    // Crear una pestaña para la materia y mostrar las preguntas, respuestas, aciertos y errores
                    var contentPage = new ContentPage
                    {
                        Title = materia,
                        BackgroundColor = Color.FromArgb("#F0F4F8"),  // Color de fondo suave
                        Content = CrearContenidoMateria(materia, preguntas)
                    };

                    this.Children.Add(contentPage);
                }
            }
        }

        // Método para cargar preguntas desde el archivo JSON
        private List<(string pregunta, string respuestaCorrecta)> CargarPreguntasDesdeArchivo(string archivoJson)
        {
            var preguntasList = new List<(string pregunta, string respuestaCorrecta)>();

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(archivoJson))
                {
                    if (stream == null) return null;

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string json = reader.ReadToEnd();
                        var preguntasJson = JArray.Parse(json);

                        foreach (var preguntaJson in preguntasJson)
                        {
                            string pregunta = (string)preguntaJson["pregunta"];
                            int respuestaCorrectaIndex = (int)preguntaJson["respuesta_correcta"];
                            string respuestaCorrecta = (string)preguntaJson["opciones"][respuestaCorrectaIndex];

                            preguntasList.Add((pregunta, respuestaCorrecta));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"No se pudo cargar el archivo de preguntas para esta materia: {ex.Message}", "OK");
            }

            return preguntasList;
        }

        // Método para crear el contenido de cada materia
        private ScrollView CrearContenidoMateria(string materia, List<(string pregunta, string respuestaCorrecta)> preguntas)
        {
            var stackLayout = new StackLayout { Padding = 20, Spacing = 15 };

            stackLayout.Children.Add(new Label
            {
                Text = $"Resultados de {materia}",
                FontAttributes = FontAttributes.Bold,
                FontSize = 24,
                TextColor = Color.FromArgb("#2C3E50"),  // Título color azul oscuro
                HorizontalOptions = LayoutOptions.Center
            });

            // Mostrar todas las preguntas y sus respuestas correctas
            for (int i = 0; i < preguntas.Count; i++)
            {
                stackLayout.Children.Add(new Label
                {
                    Text = $"Pregunta {i + 1}: {preguntas[i].pregunta}",
                    FontSize = 18,
                    TextColor = Colors.Black
                });

                stackLayout.Children.Add(new Label
                {
                    Text = $"Respuesta Correcta: {preguntas[i].respuestaCorrecta}",
                    FontSize = 16,
                    FontAttributes = FontAttributes.Italic,
                    TextColor = Color.FromArgb("#2980B9")  // Color de respuestas correcto
                });
            }

            // Obtener los aciertos y errores de la materia desde ResultadosGlobales
            var (aciertos, errores) = ResultadosGlobales.ObtenerResultadosPorMateria()[materia];

            // Mostrar aciertos y errores
            stackLayout.Children.Add(new Label
            {
                Text = $"Aciertos: {aciertos}",
                FontSize = 18,
                TextColor = Colors.Green
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Errores: {errores}",
                FontSize = 18,
                TextColor = Colors.Red
            });

            // Calcular porcentajes
            int totalPreguntas = aciertos + errores;
            double porcentajeAciertos = totalPreguntas > 0 ? (double)aciertos / totalPreguntas * 100 : 0;
            double porcentajeErrores = totalPreguntas > 0 ? (double)errores / totalPreguntas * 100 : 0;

            stackLayout.Children.Add(new Label
            {
                Text = $"Porcentaje de Aciertos: {porcentajeAciertos:F2}%",
                FontSize = 18,
                TextColor = Color.FromArgb("#3498DB")  // Azul para porcentaje de aciertos
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Porcentaje de Errores: {porcentajeErrores:F2}%",
                FontSize = 18,
                TextColor = Color.FromArgb("#E67E22")  // Naranja para porcentaje de errores
            });

            return new ScrollView { Content = stackLayout };
        }

        // Crear la pestaña de Resultado Global
        private void CrearPestanaResultadoGlobal()
        {
            var (totalAciertos, totalErrores) = ResultadosGlobales.ObtenerResultadosGlobales();
            int totalPreguntasGlobal = totalAciertos + totalErrores;

            double porcentajeGlobalAciertos = totalPreguntasGlobal > 0 ? (double)totalAciertos / totalPreguntasGlobal * 100 : 0;
            double porcentajeGlobalErrores = totalPreguntasGlobal > 0 ? (double)totalErrores / totalPreguntasGlobal * 100 : 0;

            var materiaMasAciertos = ResultadosGlobales.MateriaConMasAciertos();
            var materiaMasErrores = ResultadosGlobales.MateriaConMasErrores();
            var materiaMenosAciertos = ResultadosGlobales.MateriaConMenosAciertos();
            var materiaMenosErrores = ResultadosGlobales.MateriaConMenosErrores();

            double calificacionFinal = ResultadosGlobales.CalcularCalificacionFinal();

            var stackLayout = new StackLayout { Padding = 20, Spacing = 15 };

            stackLayout.Children.Add(new Label
            {
                Text = "Resumen Global",
                FontAttributes = FontAttributes.Bold,
                FontSize = 24,
                TextColor = Color.FromArgb("#2C3E50"),
                HorizontalOptions = LayoutOptions.Center
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Total de Aciertos: {totalAciertos}",
                FontSize = 18,
                TextColor = Colors.Green
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Total de Errores: {totalErrores}",
                FontSize = 18,
                TextColor = Colors.Red
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Porcentaje de Aciertos Global: {porcentajeGlobalAciertos:F2}%",
                FontSize = 18,
                TextColor = Color.FromArgb("#3498DB")
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Porcentaje de Errores Global: {porcentajeGlobalErrores:F2}%",
                FontSize = 18,
                TextColor = Color.FromArgb("#E67E22")
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Materia con más aciertos: {materiaMasAciertos}",
                FontSize = 18,
                TextColor = Colors.Green
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Materia con menos aciertos: {materiaMenosAciertos}",
                FontSize = 18,
                TextColor = Color.FromArgb("#3498DB")
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Materia con más errores: {materiaMasErrores}",
                FontSize = 18,
                TextColor = Colors.Red
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Materia con menos errores: {materiaMenosErrores}",
                FontSize = 18,
                TextColor = Color.FromArgb("#E67E22")
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Calificación Final: {calificacionFinal:F2}%",
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Purple,
                HorizontalOptions = LayoutOptions.Center
            });

            var resumenGlobalPage = new ContentPage
            {
                Title = "Resultado Global",
                BackgroundColor = Color.FromArgb("#F0F4F8"),
                Content = new ScrollView { Content = stackLayout }
            };

            this.Children.Add(resumenGlobalPage);
        }
    }
}
