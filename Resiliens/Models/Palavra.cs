using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Resiliens.Models
{
    public class Palavra
    {
        private static List<string> PalavrasChave = new List<string>();

        public Palavra()
        {
            QuantidadeDeDocumentosEmQueOcorre = 1;
            PalavrasChave.Add("pedir");
            PalavrasChave.Add("requerer");
            PalavrasChave.Add("citar");
            PalavrasChave.Add("cite");
            PalavrasChave.Add("requer");
            PalavrasChave.Add("solicita");
            PalavrasChave.Add("solicitar");
            PalavrasChave.Add("citada");
            PalavrasChave.Add("pedido");
            PalavrasChave.Add("condenação");
        }

        public string Texto { get; set; }
        private double peso { get; set; }
        public double Peso
        {
            get
            {
                return peso;
            }
            set
            {
                peso = value;
                if (PalavrasChave.Contains(Texto.ToLower()))
                    peso = peso * 10;
            }
        }
        public int QuantidadeOcorrencias { get; set; }
        public int QuantidadeDeDocumentosEmQueOcorre { get; set; }

    }
}