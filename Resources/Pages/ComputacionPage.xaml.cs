using System;
using System.IO;
using Microsoft.Maui.Controls;
using Newtonsoft.Json.Linq;
using System.Reflection;
using MauiProyecto.Global;

namespace MauiProyecto
{
    public partial class ComputacionPage : ContentPage
    {
        private JArray preguntas; // Almacena las preguntas cargadas desde el archivo JSON
        private int preguntaActual = 0; // Índice de la pregunta actual
        private int aciertos = 0; // Contador de aciertos
        private int errores = 0; // Contador de errores

        public ComputacionPage()
        {
            InitializeComponent();
            CargarPreguntas(); // Cargar preguntas al iniciar la página
        }

        // Método para cargar las preguntas desde un archivo JSON incrustado
        private void CargarPreguntas()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "MauiProyecto.Resources.preguntas.preguntas_computacion.json"; // Ruta completa al recurso JSON

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null) // Verificar si el recurso no existe
                    {
                        DisplayAlert("Error", "No se pudo cargar el archivo de preguntas. Verifica el nombre y la ruta.", "OK");
                        return;
                    }

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string json = reader.ReadToEnd();
                        preguntas = JArray.Parse(json); // Parsear el JSON y asignarlo a la variable preguntas
                    }
                }

                if (preguntas == null || preguntas.Count == 0)
                {
                    DisplayAlert("Error", "El archivo de preguntas está vacío o no contiene datos válidos.", "OK");
                    return;
                }

                MostrarPregunta(); // Si todo está bien, mostramos la primera pregunta
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Ocurrió un error al cargar las preguntas: {ex.Message}", "OK");
            }
        }

        // Método para mostrar la pregunta actual
        private void MostrarPregunta()
        {
            if (preguntaActual < preguntas?.Count) // Verificamos que preguntas no sea null y que el índice esté dentro del rango
            {
                var pregunta = preguntas[preguntaActual];
                PreguntaLabel.Text = (string)pregunta["pregunta"];
                Opcion1.Content = (string)pregunta["opciones"][0];
                Opcion2.Content = (string)pregunta["opciones"][1];
                Opcion3.Content = (string)pregunta["opciones"][2];
            }
            else
            {
                DisplayAlert("Error", "No hay preguntas disponibles.", "OK");
            }
        }

        // Método para validar la respuesta seleccionada
        private async void OnValidateClicked(object sender, EventArgs e)
        {
            if (preguntaActual < preguntas?.Count)
            {
                var pregunta = preguntas[preguntaActual];
                int respuestaCorrecta = (int)pregunta["respuesta_correcta"];
                int respuestaSeleccionada = -1;

                if (Opcion1.IsChecked) respuestaSeleccionada = 0;
                if (Opcion2.IsChecked) respuestaSeleccionada = 1;
                if (Opcion3.IsChecked) respuestaSeleccionada = 2;

                if (respuestaSeleccionada == respuestaCorrecta)
                {
                    aciertos++;
                }
                else
                {
                    errores++;
                }

                preguntaActual++;

                if (preguntaActual < preguntas.Count)
                {
                    MostrarPregunta(); // Mostrar la siguiente pregunta si hay más
                }
                else
                {
                    
                    // Mostrar los resultados y calcular el porcentaje
                    int totalPreguntas = aciertos + errores;
                    double porcentajeAciertos = (double)aciertos / totalPreguntas * 100;
                    double porcentajeErrores = (double)errores / totalPreguntas * 100;

                    // Guardar los resultados en ResultadosGlobales
                    ResultadosGlobales.AgregarResultado("Computacion", aciertos, errores);

                    await DisplayAlert("Resultado", $"Aciertos: {aciertos}, Errores: {errores}\n" +
                                                    $"Porcentaje de Aciertos: {porcentajeAciertos:F2}%\n" +
                                                    $"Porcentaje de Errores: {porcentajeErrores:F2}%", "OK");

                    // Aquí puedes navegar a la siguiente página de materia, o si es la última, mostrar un resumen
                    await Navigation.PushAsync(new FisicaPage()); // Si esta es la última materia, se puede mostrar un resumen
                }
            }
            else
            {
                DisplayAlert("Error", "No se pudo validar la respuesta porque no hay preguntas cargadas.", "OK");
            }
        }
    }
}
