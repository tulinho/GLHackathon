using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Resiliens.Entidades
{
    public class Peticao
    {
        public string CodigoPeticao { get; set; }
        public string Comarca { get; set; }
        public string CpfReclamante { get; set; }
        public string Foro { get; set; }
        public string NaturezaAcao { get; set; }
        public string NaturezaProcesso { get; set; }
        public string Reclamante { get; set; }
        public string Requerido { get; set; }
        public string CnpjRequerido { get; set; }
    }
}