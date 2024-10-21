using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiProyecto.Resources.Models
{
    public class ResultadoMateria
    {
        public string Materia { get; set; }
        public int Aciertos { get; set; }
        public int Errores { get; set; }
        public double PorcentajeAciertos => (Aciertos / (double)(Aciertos + Errores)) * 100;
        public double PorcentajeErrores => (Errores / (double)(Aciertos + Errores)) * 100;
    }

}
