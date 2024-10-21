using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiProyecto.Models
{
    public class Pregunta
    {
        public string PreguntaTexto { get; set; }
        public string[] Opciones { get; set; }
        public int RespuestaCorrecta { get; set; } // Índice de la respuesta correcta
    }
}
