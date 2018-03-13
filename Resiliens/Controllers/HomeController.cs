using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Resiliens.Entidades;
using Resiliens.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Resiliens.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listar()
        {
            return View("Listar");
        }

        public void SubmeterPdf(HttpPostedFileBase arquivo)
        {
            string textoPeticao = LerArquivoPdf(arquivo);
            Peticao peticao = ProcessarPeticao(textoPeticao);
            SalvarPeticao(peticao);
            EnviarEmailNotificacao(peticao);
        }

        public string LerArquivoPdf(HttpPostedFileBase arquivo)
        {
            try
            {
                using (PdfReader leitor = new PdfReader(arquivo.InputStream))
                {
                    StringBuilder texto = new StringBuilder();
                    for (int i = 1; i <= leitor.NumberOfPages; i++)
                    {
                        texto.Append(PdfTextExtractor.GetTextFromPage(leitor, i));
                    }
                    return texto.ToString();
                }
            }
            catch
            {
                throw;
            }
        }

        private Peticao ProcessarPeticao(string textoPeticao)
        {
            Peticao peticao = new Peticao();
            peticao.CpfReclamante = ExtrairCpfReclamanteDaPeticao(textoPeticao);
            peticao.Comarca = ExtrairComarcaDaPeticao(textoPeticao);
            peticao.CnpjRequerido = ExtrairCnpjRequeridoDaPeticao(textoPeticao);
            peticao.Foro = ExtrairForoDaPeticao(textoPeticao);
            peticao.NaturezaAcao = ExtrairNaturezaAcaoDaPeticao(textoPeticao);
            peticao.NaturezaProcesso = ExtrairNaturezaProcessoDaPeticao(textoPeticao);
            peticao.Reclamante = ExtrairReclamanteDaPeticao(textoPeticao);
            peticao.Requerido = ExtrairRequeridoDaPeticao(textoPeticao);

            return peticao;
        }

        private string ExtrairCpfReclamanteDaPeticao(string textoPeticao)
        {
            Regex regex = new Regex(@"[0-9]{3}.[0-9]{3}.[0-9]{3}-[0-9]{2}");
            Match match = regex.Match(textoPeticao);

            return match.Value;
        }

        private string ExtrairComarcaDaPeticao(string textoPeticao)
        {
            return string.Empty; //TODO: pendente implementação
        }

        private string ExtrairCnpjRequeridoDaPeticao(string textoPeticao)
        {
            Regex regex = new Regex(@"[0-9]{2}.[0-9]{3}.[0-9]{3}\/[0-9]{4}-[0-9]{2}");
            Match match = regex.Match(textoPeticao);

            return match.Value;
        }

        private string ExtrairForoDaPeticao(string textoPeticao)
        {
            Regex regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]{3,}");
            Match match = regex.Match(textoPeticao);
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+S\/A");
                match = regex.Match(textoPeticao);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+SA");
                match = regex.Match(textoPeticao);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+LTDA");
                match = regex.Match(textoPeticao);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+ME");
                match = regex.Match(textoPeticao);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+EPP");
                match = regex.Match(textoPeticao);
            }

            return match.Value;
        }

        private string ExtrairNaturezaAcaoDaPeticao(string textoPeticao)
        {
            Regex regex = new Regex(@"AÇÃO [A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]{3,}[A-Z]");
            Match match = regex.Match(textoPeticao);
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+DANOS[A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]{3,}[A-Z]");
                match = regex.Match(textoPeticao);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"..."); //TODO: adicionar mais padrões
                match = regex.Match(textoPeticao);
            }

            return match.Value;
        }

        private string ExtrairNaturezaProcessoDaPeticao(string textoPeticao)
        {
            return string.Empty; //TODO: pendente implementação
        }

        private string ExtrairReclamanteDaPeticao(string textoPeticao)
        {
            return string.Empty; //TODO: pendente implementação
        }

        private string ExtrairRequeridoDaPeticao(string textoPeticao)
        {
            Regex regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+S\/A");
            Match match = regex.Match(textoPeticao);
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+SA");
                match = regex.Match(textoPeticao);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+LTDA");
                match = regex.Match(textoPeticao);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+ME");
                match = regex.Match(textoPeticao);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"[A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+EPP");
                match = regex.Match(textoPeticao);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"em face de [A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+");
                match = regex.Match(textoPeticao);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"em desfavor de [A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+");
                match = regex.Match(textoPeticao);
            }
            if (string.IsNullOrWhiteSpace(match.Value))
            {
                regex = new Regex(@"em contende com [A-Z][A-ZÁÉÍÓÚÂÊÎÔÛÃÕÇ\/ ]+");
                match = regex.Match(textoPeticao);
            }

            return match.Value;
        }

        private void SalvarPeticao(Peticao peticao)
        {
            PeticaoRepositorio repositorio = new PeticaoRepositorio();

            repositorio.Inserir(peticao);
        }

        private void EnviarEmailNotificacao(Peticao peticao)
        {
            string corpoEmail = ObterCorpoEmail();
            Colaborador colaborador = ObterColaboradorResponsavel(peticao.NaturezaAcao);
            String.Format(corpoEmail, 
        }

        private string ObterCorpoEmail()
        {
            string corpo = @"
<html>
<head>
<style>
table {
    font-family: arial, sans-serif;
    border-collapse: collapse;
    width: 100%;
}

td, th {
    border: 1px solid #dddddd;
    text-align: left;
    padding: 8px;
}

tr:nth-child(even) {
    background-color: #dddddd;
}
</style>
</head>

Prezado(a) {0},
<br>
<br>
A seguinte petição foi processada em {1}, classificada como {2}, e requer sua análise:
<br>
<br>
<table>
    <tr>
        <th>Reclamante</th>
        <th>Natureza Processo</th>
        <th>Requerido</th>
        <th>Foro</th>
        <th>Comarca</th>
    </tr>
    <tr>
        <td>{3}</td>
        <td>{4}</td>
        <td>{5}</td>
        <td>{6}</td>
        <td>{7}</td>
    </tr>
</table>
<br>
<br>
Mais informações a respeito do processo, bem como a documentação completa da petição, podem ser obtidas clicando aqui.
<br>
<br>
Cordialmente,
<br>
<br>
Análise de documentos
</html>";

            return corpo;
        }

        private Colaborador ObterColaboradorResponsavel(string naturezaAcao)
        {
            throw new NotImplementedException();
        }
    }
}