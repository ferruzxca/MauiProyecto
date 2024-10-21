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
            CargarPreguntasYGenerarPestanas(); // Cargamos las preguntas y generamos las pesta�as
            CrearPestanaResultadoGlobal(); // Crear pesta�a de Resultado Global
        }

        // M�todo para cargar todas las preguntas desde los archivos .json y generar las pesta�as
        private void CargarPreguntasYGenerarPestanas()
        {
            // Lista de las materias y los archivos JSON correspondientes
            var materiasArchivos = new Dictionary<string, string>
            {
                { "Espa�ol", "MauiProyecto.Resources.preguntas.preguntas_espanol.json" },
                { "Matem�ticas", "MauiProyecto.Resources.preguntas.preguntas_matematicas.json" },
                { "Biolog�a", "MauiProyecto.Resources.preguntas.preguntas_biologia.json" },
                { "Qu�mica", "MauiProyecto.Resources.preguntas.preguntas_quimica.json" },
                { "F�sica", "MauiProyecto.Resources.preguntas.preguntas_fisica.json" },
                { "Historia", "MauiProyecto.Resources.preguntas.preguntas_historia.json" },
                { "Geograf�a", "MauiProyecto.Resources.preguntas.preguntas_geografia.json" },
                { "Computaci�n", "MauiProyecto.Resources.preguntas.preguntas_computacion.json" },
                { "L�gica", "MauiProyecto.Resources.preguntas.preguntas_logica.json" },
                { "C�vica", "MauiProyecto.Resources.preguntas.preguntas_civica.json" }
            };

            // Procesamos cada materia y archivo
            foreach (var materiaArchivo in materiasArchivos)
            {
                string materia = materiaArchivo.Key;
                string archivoJson = materiaArchivo.Value;

                // Cargar preguntas desde el archivo JSON correspondiente
                var preguntas = CargarPreguntasDesdeArchivo(archivoJson);
                if (preguntas != null)
                {
                    // Validar respuestas reales y calcular los aciertos y errores (los datos deben venir del usuario)
                    var (aciertos, errores) = ObtenerAciertosYErroresReales(materia);

                    // Guardar los resultados de cada materia para c�lculo global
                    ResultadosGlobales.AgregarResultado(materia, aciertos, errores);

                    // Crear una pesta�a para la materia
                    var contentPage = new ContentPage
                    {
                        Title = materia,
                        Content = CrearContenidoMateria(materia, preguntas, aciertos, errores)
                    };

                    this.Children.Add(contentPage);
                }
            }
        }

        // M�todo para cargar preguntas desde el archivo JSON
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

        // M�todo para obtener los aciertos y errores reales de la materia
        private (int aciertos, int errores) ObtenerAciertosYErroresReales(string materia)
        {
            // Aqu� es donde deber�as obtener los aciertos y errores reales del usuario
            // Los datos deben venir de la l�gica de validaci�n de respuestas del cuestionario de la aplicaci�n
            int aciertos = ResultadosGlobales.ObtenerResultadosPorMateria()[materia].aciertos;
            int errores = ResultadosGlobales.ObtenerResultadosPorMateria()[materia].errores;

            return (aciertos, errores);
        }

        // M�todo para crear el contenido de cada materia
        private ScrollView CrearContenidoMateria(string materia, List<(string pregunta, string respuestaCorrecta)> preguntas, int aciertos, int errores)
        {
            var stackLayout = new StackLayout { Padding = 20, Spacing = 10 };

            stackLayout.Children.Add(new Label
            {
                Text = $"Resultados de {materia}",
                FontAttributes = FontAttributes.Bold,
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Center
            });

            // Mostrar todas las preguntas y sus respuestas correctas
            for (int i = 0; i < preguntas.Count; i++)
            {
                stackLayout.Children.Add(new Label
                {
                    Text = $"Pregunta {i + 1}: {preguntas[i].pregunta}",
                    FontSize = 16
                });

                stackLayout.Children.Add(new Label
                {
                    Text = $"Respuesta Correcta: {preguntas[i].respuestaCorrecta}",
                    FontSize = 16,
                    FontAttributes = FontAttributes.Italic
                });
            }

            // Mostrar aciertos y errores
            stackLayout.Children.Add(new Label
            {
                Text = $"Aciertos: {aciertos}",
                FontSize = 16,
                TextColor = Colors.Green
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Errores: {errores}",
                FontSize = 16,
                TextColor = Colors.Red
            });

            // Calcular porcentajes
            int totalPreguntas = aciertos + errores;
            double porcentajeAciertos = totalPreguntas > 0 ? (double)aciertos / totalPreguntas * 100 : 0;
            double porcentajeErrores = totalPreguntas > 0 ? (double)errores / totalPreguntas * 100 : 0;

            stackLayout.Children.Add(new Label
            {
                Text = $"Porcentaje de Aciertos: {porcentajeAciertos:F2}%",
                FontSize = 16,
                TextColor = Colors.Blue
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Porcentaje de Errores: {porcentajeErrores:F2}%",
                FontSize = 16,
                TextColor = Colors.Orange
            });

            return new ScrollView { Content = stackLayout };
        }

        // Crear la pesta�a de Resultado Global
        private void CrearPestanaResultadoGlobal()
        {
            var (totalAciertos, totalErrores) = ResultadosGlobales.ObtenerResultadosGlobales();
            int totalPreguntasGlobal = totalAciertos + totalErrores;

            // Calcular los porcentajes globales
            double porcentajeGlobalAciertos = totalPreguntasGlobal > 0 ? (double)totalAciertos / totalPreguntasGlobal * 100 : 0;
            double porcentajeGlobalErrores = totalPreguntasGlobal > 0 ? (double)totalErrores / totalPreguntasGlobal * 100 : 0;

            // Obtener la materia con m�s/menos aciertos y errores
            var materiaMasAciertos = ResultadosGlobales.MateriaConMasAciertos();
            var materiaMasErrores = ResultadosGlobales.MateriaConMasErrores();
            var materiaMenosAciertos = ResultadosGlobales.MateriaConMenosAciertos();
            var materiaMenosErrores = ResultadosGlobales.MateriaConMenosErrores();

            // Crear el contenido de la pesta�a de resultado global
            var stackLayout = new StackLayout { Padding = 20, Spacing = 10 };

            stackLayout.Children.Add(new Label
            {
                Text = "Resumen Global",
                FontAttributes = FontAttributes.Bold,
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Center
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Total de Aciertos: {totalAciertos}",
                FontSize = 16,
                TextColor = Colors.Green
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Total de Errores: {totalErrores}",
                FontSize = 16,
                TextColor = Colors.Red
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Porcentaje de Aciertos Global: {porcentajeGlobalAciertos:F2}%",
                FontSize = 16,
                TextColor = Colors.Blue
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Porcentaje de Errores Global: {porcentajeGlobalErrores:F2}%",
                FontSize = 16,
                TextColor = Colors.Orange
            });

            // Mostrar la materia con m�s/menos aciertos y errores
            stackLayout.Children.Add(new Label
            {
                Text = $"Materia con m�s aciertos: {materiaMasAciertos}",
                FontSize = 16,
                TextColor = Colors.Green
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Materia con menos aciertos: {materiaMenosAciertos}",
                FontSize = 16,
                TextColor = Colors.Blue
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Materia con m�s errores: {materiaMasErrores}",
                FontSize = 16,
                TextColor = Colors.Red
            });

            stackLayout.Children.Add(new Label
            {
                Text = $"Materia con menos errores: {materiaMenosErrores}",
                FontSize = 16,
                TextColor = Colors.Orange
            });

            var resumenGlobalPage = new ContentPage
            {
                Title = "Resultado Global",
                Content = new ScrollView { Content = stackLayout }
            };

            this.Children.Add(resumenGlobalPage);
        }
    }
}

