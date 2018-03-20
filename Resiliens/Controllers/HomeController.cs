using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Resiliens.Entidades;
using Resiliens.Models;
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
        private static ProcessadorDeTexto processador { get; set; }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listar()
        {
            return View("Listar");
        }

        public ActionResult SubmeterPdf(HttpPostedFileBase[] arquivos)
        {
            List<Peticao> Peticoes = new List<Peticao>();
            if (arquivos == null || arquivos.Length == 0)
                return RedirectToAction("Index");
            foreach(var arquivo in arquivos)
            {
                string textoPeticao = LerArquivoPdf(arquivo);
                Peticoes.Add(new Peticao(textoPeticao));
            }
            Peticoes.ForEach(peticao => EnviarEmailNotificacao(peticao));
            
            return View("ExibirPeticoes", Peticoes);
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
            Peticao peticao = new Peticao(textoPeticao);

            return peticao;
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
            corpoEmail = corpoEmail.Replace("<<<NomeColaborador>>>", colaborador.Nome)
                .Replace("<<<DataProcessamento>>>", DateTime.Now.ToShortDateString())
                .Replace("<<<NaturezaAcao>>>", peticao.NaturezaAcao)
                .Replace("<<<Reclamante>>>", peticao.Reclamante)
                .Replace("<<<NaturezaProcesso>>>", peticao.NaturezaProcesso)
                .Replace("<<<Requerido>>>", peticao.Requerido)
                .Replace("<<<Comarca>>>", peticao.Comarca);

            EmailRepositorio emailRepositorio = new EmailRepositorio();
            emailRepositorio.EnviarEmail("Teste assunto", "raquini@gmail.com", "resiliensteste@gmail.com", "resiliensteste@gmail.com", corpoEmail, true);
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

Prezado(a) <<<NomeColaborador>>>,
<br>
<br>
A seguinte petição foi processada em <<<DataProcessamento>>>, classificada como <<<NaturezaAcao>>>, e requer sua análise:
<br>
<br>
<table>
    <tr>
        <th>Reclamante</th>
        <th>Natureza Processo</th>
        <th>Requerido</th>
        <th>Comarca</th>
    </tr>
    <tr>
        <td><<<Reclamante>>></td>
        <td><<<NaturezaProcesso>>></td>
        <td><<<Requerido>>></td>
        <td><<<Comarca>>></td>
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
            return new Colaborador() { Email = "raquini@gmail.com", Nome = "Raphael" };
        }
    }
}