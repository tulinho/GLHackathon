using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Resiliens.Entidades
{
    public class Peticao
    {
        #region Propriedades públicas

        public string CodigoPeticao { get; set; }
        public string Comarca
        {
            get
            {
                return ObterTermoPorExpressaoRegular(@"COMARCA DE [A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]{3,}[A-Z]").Replace("COMARCA DE ", "");
            }
        }
        public string CpfReclamante
        {
            get
            {
                return ObterTermoPorExpressaoRegular(@"[0-9]{3}.[0-9]{3}.[0-9]{3}-[0-9]{2}");
            }
        }
        public string CnpjRequerido
        {
            get
            {
                return ObterTermoPorExpressaoRegular(@"[0-9]{2}.[0-9]{3}.[0-9]{3}\/[0-9]{4}-[0-9]{2}");
            }
        }
        public string NaturezaAcao
        {
            get
            {
                return ObterNaturezaAcao();
            }
        }
        public string NaturezaProcesso
        {
            get
            {
                return ObterNaturezaProcesso();
            }
        }
        public string Reclamante { get; set; } //TODO: Pendente
        public string Requerido
        {
            get
            {
                return ObterRequerido();
            }
        }

        #endregion

        #region Propriedades internas

        private string Conteudo { get; set; }

        #endregion

        public Peticao(string conteudo)
        {
            Conteudo = conteudo;
            //if (processador == null)
            //    processador = new ProcessadorDeTexto(textoPeticao);
            //else
            //    processador.Processar(textoPeticao);

            //var media = processador.Paragrafos.Sum(p => p.Peso)/processador.Paragrafos.Count();
            //string texto = String.Join("<br><br><br>", processador.Paragrafos.Where(p => p.Peso > media)
            //    .OrderBy(p => p.OrdemNoTexto)
            //    .Select(t => t.Texto));
            //return texto;
        }

        private string ObterTermoPorExpressaoRegular(string expressaoRegular)
        {
            Regex regex = new Regex(expressaoRegular);
            Match match = regex.Match(Conteudo);
            return match.Value;
        }

        private string ObterNaturezaAcao()
        {
            string natureza = ObterTermoPorExpressaoRegular(@"AÇÃO [A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]{3,}[A-Z]");
            if (!String.IsNullOrWhiteSpace(natureza))
                return natureza;
            natureza = ObterTermoPorExpressaoRegular(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+DANOS[A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]{3,}[A-Z]");
            if (!String.IsNullOrWhiteSpace(natureza))
                return natureza;
            natureza = ObterTermoPorExpressaoRegular(@"..."); //TODO: adicionar mais padrões
            if (!String.IsNullOrWhiteSpace(natureza))
                return natureza;
            return natureza;

        }

        private string ObterNaturezaProcesso()
        {
            string natureza = ObterTermoPorExpressaoRegular(@"[cC][íiIÍ][vV][eE][lL]");
            if (!String.IsNullOrWhiteSpace(natureza))
                return natureza;
            natureza = ObterTermoPorExpressaoRegular(@"[cC][rR][iI][mM][iI][nN][aA][lL]");
            if (!String.IsNullOrWhiteSpace(natureza))
                return natureza;
            natureza = ObterTermoPorExpressaoRegular(@"[tT][rR][aA][bB][aA][lL][hH][Ii][Ss][Tt][Aa]");
            if (!String.IsNullOrWhiteSpace(natureza))
                return natureza;
            natureza = ObterTermoPorExpressaoRegular(@"[Tt][Rr][Ii][Bb][Uu][Tt][Aa][Rr][Ii][Oo]");
            if (!String.IsNullOrWhiteSpace(natureza))
                return natureza;
            return natureza;
        }

        private string ObterRequerido()
        {
            Regex regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+S\/A");
            Match match = regex.Match(Conteudo);
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+SA");
                match = regex.Match(Conteudo);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+LTDA");
                match = regex.Match(Conteudo);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+ME");
                match = regex.Match(Conteudo);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+EPP");
                match = regex.Match(Conteudo);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"em face de [A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+");
                match = regex.Match(Conteudo);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"em desfavor de [A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+");
                match = regex.Match(Conteudo);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"em contende com [A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+");
                match = regex.Match(Conteudo);
            }

            return match.Value;
        }

    }
}