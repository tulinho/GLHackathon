using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Resiliens.Models
{
    public class Paragrafo
    {
        public string Texto { get; set; }
        public int QuantidadeDePalavras { get; set; }
        public int OrdemNoTexto { get; set; }
        public double Peso { get; set; }
        public List<Palavra> Palavras = new List<Palavra>();
    }
}